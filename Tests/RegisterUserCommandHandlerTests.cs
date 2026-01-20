using Application.Auth.Commands;
using Application.Contracts.Data;
using Application.Contracts.Providers;
using Domain.Entitites;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using SharedKernel.Exceptions;
using System.Reflection;

namespace Tests
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly Mock<IAppDbContext> _contextMock;
        private readonly Mock<DbSet<User>> _userDbSetMock;
        private readonly Mock<IJwtTokenProvider> _jwtTokenProviderMock;
        private readonly Mock<IPasswordProvider> _passwordProviderMock;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTests()
        {
            _contextMock = new Mock<IAppDbContext>();
            _userDbSetMock = new Mock<DbSet<User>>();
            _jwtTokenProviderMock = new Mock<IJwtTokenProvider>();
            _passwordProviderMock = new Mock<IPasswordProvider>();

            _contextMock.Setup(x => x.Users).Returns(_userDbSetMock.Object);

            _handler = new RegisterUserCommandHandler(
                _contextMock.Object,
                _jwtTokenProviderMock.Object,
                _passwordProviderMock.Object
            );
        }

        public async Task Handle_ValidRequest_ShouldRegisterUserSuccessfully()
        {
            // Arrange
            var command = new RegisterUserCommand("John", "Middle", "Doe", "john.doe@example.com", "SecurePassword123!");

            var hashedPassword = "hashed_password_123";
            var expectedToken = "jwt_token_xyz";

            // Mock empty user list (no existing users)
            var emptyUserList = new List<User>().AsQueryable();
            var mockUserDbSet = emptyUserList.BuildMockDbSet();
            _contextMock.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            _passwordProviderMock
                .Setup(x => x.CustomHashPassword(command.Password))
                .Returns(hashedPassword);

            _jwtTokenProviderMock
                .Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns(expectedToken);

            _contextMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("User registration successful");
            result.Token.Should().Be(expectedToken);

            _userDbSetMock.Verify(x => x.AddAsync(
                It.Is<User>(u =>
                    u.FirstName == command.FirstName &&
                    u.LastName == command.LastName &&
                    u.MiddleName == command.MiddleName &&
                    u.Email == command.Email.Trim() &&
                    u.PasswordHash == hashedPassword &&
                    u.CreatedAtUtc != default),
                It.IsAny<CancellationToken>()),
                Times.Once);

            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _passwordProviderMock.Verify(x => x.CustomHashPassword(command.Password), Times.Once);
            _jwtTokenProviderMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyFirstName_ShouldThrowCustomValidationException()
        {
            // Arrange
            var command = new RegisterUserCommand(string.Empty, string.Empty, "Doe", "john.doe@example.com", "SecurePassword123!");

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .WithMessage("First name and last name are required");

            _userDbSetMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NullFirstName_ShouldThrowCustomValidationException()
        {
            // Arrange
            var command = new RegisterUserCommand(null, string.Empty, "Doe", "john.doe@example.com", "SecurePassword123!");

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .WithMessage("First name and last name are required");
        }

        [Fact]
        public async Task Handle_EmptyLastName_ShouldThrowCustomValidationException()
        {
            // Arrange
            var command = new RegisterUserCommand("John", string.Empty, "", "john.doe@example.com", "SecurePassword123!");

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .WithMessage("First name and last name are required");
        }

        [Fact]
        public async Task Handle_NullLastName_ShouldThrowCustomValidationException()
        {
            // Arrange
            var command = new RegisterUserCommand("John", string.Empty, null, "john.doe@example.com", "SecurePassword123!");

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .WithMessage("First name and last name are required");
        }

        [Fact]
        public async Task Handle_EmptyPassword_ShouldThrowCustomValidationException()
        {
            // Arrange
            var command = new RegisterUserCommand("John", string.Empty, "Doe", "john.doe@example.com", "");

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .WithMessage("Password is required required");
        }

        [Fact]
        public async Task Handle_NullPassword_ShouldThrowCustomValidationException()
        {
            // Arrange
            var command = new RegisterUserCommand("John", string.Empty, "Doe", "john.doe@example.com", null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .WithMessage("Password is required required");
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@example.com")]
        [InlineData("user@")]
        [InlineData("user @example.com")]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Handle_InvalidEmail_ShouldThrowCustomValidationException(string invalidEmail)
        {
            // Arrange
            var command = new RegisterUserCommand("John", string.Empty, "Doe", invalidEmail, "SecurePassword123!");

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .WithMessage("Email is not a valid email address");
        }

        [Fact]
        public async Task Handle_NullEmail_ShouldThrowCustomValidationException()
        {
            // Arrange
            var command = new RegisterUserCommand("John", string.Empty, "Doe", null, "SecurePassword123!");

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .WithMessage("Email is not a valid email address");
        }

        [Fact]
        public async Task Handle_ExistingUser_ShouldThrowCustomValidationException()
        {
            // Arrange
            var existingEmail = "existing@example.com";
            var command = new RegisterUserCommand("John", string.Empty, "Doe", existingEmail, "SecurePassword123!");

            var existingUsers = new List<User>
        {
            new User
            {
                Email = existingEmail,
                FirstName = "Existing",
                LastName = "User",
                PasswordHash = "hash"
            }
        }.AsQueryable();

            var mockUserDbSet = existingUsers.BuildMockDbSet();
            _contextMock.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .WithMessage("User with this email already exists.");

            _userDbSetMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ExistingUserWithDifferentCasing_ShouldThrowCustomValidationException()
        {
            // Arrange
            var command = new RegisterUserCommand("John", string.Empty, "Doe", "EXISTING@EXAMPLE.COM", "SecurePassword123!");

            var existingUsers = new List<User>
        {
            new User
            {
                Email = "existing@example.com",
                FirstName = "Existing",
                LastName = "User",
                PasswordHash = "hash"
            }
        }.AsQueryable();

            var mockUserDbSet = existingUsers.BuildMockDbSet();
            _contextMock.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<CustomValidationException>()
                .WithMessage("User with this email already exists.");
        }

        [Fact]
        public async Task Handle_EmailWithWhitespace_ShouldTrimAndRegisterSuccessfully()
        {
            // Arrange
            var command = new RegisterUserCommand("John", string.Empty, "Doe", "  john.doe@example.com  ", "SecurePassword123!");

            var emptyUserList = new List<User>().AsQueryable();
            var mockUserDbSet = emptyUserList.BuildMockDbSet();
            _contextMock.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            _passwordProviderMock
                .Setup(x => x.CustomHashPassword(It.IsAny<string>()))
                .Returns("hashed");

            _jwtTokenProviderMock
                .Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns("token");

            _contextMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            mockUserDbSet.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<EntityEntry<User>>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            mockUserDbSet.Verify(x => x.AddAsync(
                It.Is<User>(u => u.Email == "john.doe@example.com"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name@example.co.uk")]
        [InlineData("first.last@subdomain.example.com")]
        [InlineData("user+tag@example.com")]
        public async Task Handle_ValidEmailFormats_ShouldRegisterSuccessfully(string validEmail)
        {
            // Arrange
            var command = new RegisterUserCommand("John", string.Empty, "Doe", validEmail, "SecurePassword123!");

            var emptyUserList = new List<User>().AsQueryable();
            var mockUserDbSet = emptyUserList.BuildMockDbSet();
            _contextMock.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            _passwordProviderMock
                .Setup(x => x.CustomHashPassword(It.IsAny<string>()))
                .Returns("hashed");

            _jwtTokenProviderMock
                .Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns("token");

            _contextMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("User registration successful");
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldSetCreatedAtUtcToCurrentTime()
        {
            // Arrange
            var command = new RegisterUserCommand("John", string.Empty, "Doe", "john.doe@example.com", "SecurePassword123!");

            var emptyUserList = new List<User>().AsQueryable();
            var mockUserDbSet = emptyUserList.BuildMockDbSet();
            _contextMock.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            _passwordProviderMock
                .Setup(x => x.CustomHashPassword(It.IsAny<string>()))
                .Returns("hashed");

            _jwtTokenProviderMock
                .Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns("token");

            _contextMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            User capturedUser = null;
            mockUserDbSet.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .Returns(new ValueTask<EntityEntry<User>>());

            var beforeExecution = DateTime.UtcNow;

            // Act
            await _handler.Handle(command, CancellationToken.None);

            var afterExecution = DateTime.UtcNow;

            // Assert
            capturedUser.Should().NotBeNull();
            capturedUser.CreatedAtUtc.Should().BeOnOrAfter(beforeExecution);
            capturedUser.CreatedAtUtc.Should().BeOnOrBefore(afterExecution);
        }
    }
}
