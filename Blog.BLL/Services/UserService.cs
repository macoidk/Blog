using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogSystem.BLL.DTO;
using BlogSystem.BLL.Exceptions;
using BlogSystem.BLL.Interfaces;
using BlogSystem.BLL.Extensions;
using BlogSystem.BLL.Utils;
using BlogSystem.DAL.Entities;
using BlogSystem.DAL.UnitOfWork;

namespace BlogSystem.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                throw new EntityNotFoundException($"User with id {id} not found");

            return user.ToDto();
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return users.Select(u => u.ToDto());
        }

        public async Task<UserDto> RegisterAsync(string username, string email, string password)
        {
            if (!ValidationHelper.IsValidUsername(username))
                throw new ServiceException("Invalid username format");

            if (!ValidationHelper.IsValidEmail(email))
                throw new ServiceException("Invalid email format");

            if (!ValidationHelper.IsValidPassword(password))
                throw new ServiceException("Password must be at least 6 characters");

            if (await UsernameExistsAsync(username))
                throw new ServiceException($"Username '{username}' is already taken");

            if (await EmailExistsAsync(email))
                throw new ServiceException($"Email '{email}' is already registered");

            _passwordHasher.CreatePasswordHash(password, out string passwordHash, out string passwordSalt);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RegistrationDate = DateTime.UtcNow,
                Role = UserRole.Reader
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user.ToDto();
        }

        public async Task<UserDto> AuthenticateAsync(string username, string password)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            if (user == null)
                throw new ServiceException("Invalid username or password");

            bool isValidPassword = _passwordHasher.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
            if (!isValidPassword)
                throw new ServiceException("Invalid username or password");

            return user.ToDto();
        }

        public async Task UpdateAsync(UserDto userDto, string password = null)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userDto.Id);
            if (user == null)
                throw new EntityNotFoundException($"User with id {userDto.Id} not found");

            if (!string.IsNullOrWhiteSpace(userDto.Username) && userDto.Username != user.Username)
            {
                if (!ValidationHelper.IsValidUsername(userDto.Username))
                    throw new ServiceException("Invalid username format");

                if (await UsernameExistsAsync(userDto.Username))
                    throw new ServiceException($"Username '{userDto.Username}' is already taken");

                user.Username = userDto.Username;
            }

            if (!string.IsNullOrWhiteSpace(userDto.Email) && userDto.Email != user.Email)
            {
                if (!ValidationHelper.IsValidEmail(userDto.Email))
                    throw new ServiceException("Invalid email format");

                if (await EmailExistsAsync(userDto.Email))
                    throw new ServiceException($"Email '{userDto.Email}' is already registered");

                user.Email = userDto.Email;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                if (!ValidationHelper.IsValidPassword(password))
                    throw new ServiceException("Password must be at least 6 characters");

                _passwordHasher.CreatePasswordHash(password, out string passwordHash, out string passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            if (!string.IsNullOrWhiteSpace(userDto.Role) && Enum.TryParse<UserRole>(userDto.Role, out var role))
            {
                user.Role = role;
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                throw new EntityNotFoundException($"User with id {id} not found");

            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _unitOfWork.Users.GetByUsernameAsync(username) != null;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _unitOfWork.Users.GetByEmailAsync(email) != null;
        }
    }
}