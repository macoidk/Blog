using BlogSystem.BLL.DTO;
using BlogSystem.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Blog.WebAPI.Models;

namespace Blog.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UsersController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Route("api/users/{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("register")]
        [Route("api/users/register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userService.RegisterAsync(model.Username, model.Email, model.Password);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPost("authenticate")]
        [Route("api/users/authenticate")]
        public async Task<ActionResult<UserDto>> Authenticate([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userService.AuthenticateAsync(model.Username, model.Password);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            var token = GenerateJwtToken(user);
            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(_configuration.GetSection("Jwt:ExpiryInMinutes").Get<int>())
            });
            return Ok(user);
        }

        [HttpPost("logout")]
        [Route("api/users/logout")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("jwt");
            return Ok("Logged out successfully");
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, UserDto userDto, string password = null)
        {
            if (id != userDto.Id) return BadRequest();
            await _userService.UpdateAsync(userDto, password);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("/Users/Register")]
        public IActionResult RegisterView()
        {
            return View("Register");
        }

        [HttpPost("/Users/Register")]
        public async Task<IActionResult> RegisterView([FromForm] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please fill in all fields correctly.";
                return View("Register");
            }
            try
            {
                await _userService.RegisterAsync(model.Username, model.Email, model.Password);
                return RedirectToAction("LoginView");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Register");
            }
        }

        [HttpGet("/Users/Login")]
        public IActionResult LoginView()
        {
            return View("Login");
        }

        [HttpPost("/Users/Login")]
        public async Task<IActionResult> LoginView([FromForm] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please fill in all fields correctly.";
                return View("Login");
            }
            try
            {
                var user = await _userService.AuthenticateAsync(model.Username, model.Password);
                if (user == null)
                {
                    ViewBag.Error = "Invalid username or password.";
                    return View("Login");
                }
                var token = GenerateJwtToken(user);
                HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(_configuration.GetSection("Jwt:ExpiryInMinutes").Get<int>())
                });
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Login");
            }
        }

        [HttpGet("/Users/Logout")]
        public IActionResult LogoutView()
        {
            HttpContext.Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("/Users/List")]
        public async Task<IActionResult> List()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }

        private string GenerateJwtToken(UserDto user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(jwtSettings.GetValue<int>("ExpiryInMinutes")),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}