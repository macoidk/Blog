using System;
using System.Threading.Tasks;
using BlogSystem.Abstractions;
using BlogSystem.BLL.DTO;
using BlogSystem.BLL.Exceptions;
using BlogSystem.BLL.Interfaces;
using BlogSystem.BLL.Utils;
using BlogSystem.Models;
using NSubstitute;
using NUnit.Framework;
using AutoFixture;
using Ninject;

namespace BlogSystem.Tests
{
    [TestFixture]
    public class UserServiceTests : TestBase
    {
        private IUserService _userService;
        private IUnitOfWork _unitOfWork;
        private IPasswordHasher _passwordHasher;
        private Fixture _fixture;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _passwordHasher = Substitute.For<IPasswordHasher>();
            
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            
            Rebind<IUnitOfWork>(_unitOfWork);
            Rebind<IPasswordHasher>(_passwordHasher);
            
            _userService = Kernel.Get<IUserService>();
            
        }

        [Test]
        public async Task RegisterAsync_ValidData_CreatesUser()
        {
            var username = "testuser";
            var email = "test@example.com";
            var password = "password123";
            _unitOfWork.Users.GetByUsernameAsync(username).Returns(Task.FromResult<User>(null));
            _unitOfWork.Users.GetByEmailAsync(email).Returns(Task.FromResult<User>(null));
            _unitOfWork.Users.AddAsync(Arg.Any<User>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(1));

            string hash = "hash";
            string salt = "salt";
            _passwordHasher.When(x => x.CreatePasswordHash(password, out hash, out salt))
                .Do(callInfo =>
                {
                    callInfo[1] = "hash"; // Налаштування out-параметра hash
                    callInfo[2] = "salt"; // Налаштування out-параметра salt
                });

            var result = await _userService.RegisterAsync(username, email, password);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(username));
            Assert.That(result.Email, Is.EqualTo(email));
            await _unitOfWork.Users.Received(1).AddAsync(Arg.Any<User>());
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Test]
        public void RegisterAsync_ExistingUsername_ThrowsServiceException()
        {
            var username = "existinguser";
            _unitOfWork.Users.GetByUsernameAsync(username).Returns(Task.FromResult(_fixture.Create<User>()));

            Assert.ThrowsAsync<ServiceException>(() => _userService.RegisterAsync(username, "test@example.com", "password123"));
        }

        [Test]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsUserDto()
        {
            // Arrange
            var username = "testuser";
            var password = "password123";
            var user = _fixture.Build<User>()
                .With(u => u.Username, username)
                .With(u => u.PasswordHash, "hash")
                .With(u => u.PasswordSalt, "salt")
                .With(u => u.Id, 1)
                .With(u => u.Email, "test@example.com") // Додайте Email якщо потрібно
                .Create();

            _unitOfWork.Users.GetByUsernameAsync(username).Returns(Task.FromResult(user));
            _passwordHasher.VerifyPasswordHash(password, "hash", "salt").Returns(true);

            // Act
            var result = await _userService.AuthenticateAsync(username, password);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(username));
            Assert.That(result.Id, Is.EqualTo(user.Id));
        }

        [Test]
        public void AuthenticateAsync_InvalidPassword_ThrowsServiceException()
        {
            // Arrange
            var username = "testuser";
            var wrongPassword = "wrongpassword";
            var user = _fixture.Build<User>()
                .With(u => u.Username, username)
                .With(u => u.PasswordHash, "hash")
                .With(u => u.PasswordSalt, "salt")
                .Create();

            var userRepository = Substitute.For<IUserRepository>();
            _unitOfWork.Users.Returns(userRepository);
            userRepository.GetByUsernameAsync(username).Returns(Task.FromResult(user));
            _passwordHasher.VerifyPasswordHash(wrongPassword, "hash", "salt").Returns(false);

            // Act & Assert
            var exception = Assert.ThrowsAsync<ServiceException>(
                async () => await _userService.AuthenticateAsync(username, wrongPassword));
            Assert.That(exception.Message, Is.EqualTo("Invalid username or password"));
        }


        [Test]
        public async Task UsernameExistsAsync_ExistingUsername_ReturnsTrue()
        {
            _unitOfWork.Users.GetByUsernameAsync("testuser").Returns(Task.FromResult(_fixture.Create<User>()));

            var result = await _userService.UsernameExistsAsync("testuser");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task UsernameExistsAsync_NonExistingUsername_ReturnsFalse()
        {
            _unitOfWork.Users.GetByUsernameAsync("testuser").Returns(Task.FromResult<User>(null));

            var result = await _userService.UsernameExistsAsync("testuser");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EmailExistsAsync_ExistingEmail_ReturnsTrue()
        {
            _unitOfWork.Users.GetByEmailAsync("test@example.com").Returns(Task.FromResult(_fixture.Create<User>()));

            var result = await _userService.EmailExistsAsync("test@example.com");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task EmailExistsAsync_NonExistingEmail_ReturnsFalse()
        {
            _unitOfWork.Users.GetByEmailAsync("test@example.com").Returns(Task.FromResult<User>(null));

            var result = await _userService.EmailExistsAsync("test@example.com");

            Assert.That(result, Is.False);
        }
    }
}