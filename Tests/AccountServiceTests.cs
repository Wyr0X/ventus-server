using Moq;
using VentusServer.DataAccess;
using VentusServer.Services;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;

namespace VentusServer.Tests.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountDAO> _mockAccountDao;
        private readonly Mock<RoleService> _mockRoleService;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _mockAccountDao = new Mock<IAccountDAO>();
            _mockRoleService = new Mock<RoleService>();
            _accountService = new AccountService(_mockAccountDao.Object, _mockRoleService.Object);
        }

        [Fact]
        public async Task GetAllAccountsAsync_ShouldReturnAllAccounts()
        {
            // Arrange
            var accountList = new List<AccountModel>
            {
                new AccountModel
                {
                    AccountId = Guid.NewGuid(),
                    Email = "test1@example.com",
                    AccountName = "Test1",
                    RoleId = "admin",  // RoleId as string
                    IsBanned = false,
                    CreatedAt = DateTime.UtcNow,
                    LastIpAddress = "localhost",
                    SessionId = Guid.Empty
                },
                new AccountModel
                {
                    AccountId = Guid.NewGuid(),
                    Email = "test2@example.com",
                    AccountName = "Test2",
                    RoleId = "user",  // RoleId as string
                    IsBanned = false,
                    CreatedAt = DateTime.UtcNow,
                    LastIpAddress = "localhost",
                    SessionId = Guid.Empty
                }
            };

            var role = new RoleModel { RoleId = "admin", DisplayName = "Admin" };
            _mockAccountDao.Setup(x => x.GetAllAccountsAsync()).ReturnsAsync(accountList);
            _mockRoleService.Setup(x => x.GetRoleByIdAsync(It.IsAny<string>())).ReturnsAsync(role);

            // Act
            var result = await _accountService.GetAllAccountsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Email.Should().Be("test1@example.com");
            result[0].RoleName.Should().Be("Admin");
            result[1].RoleName.Should().Be("Admin");
        }

        [Fact]
        public async Task GetAccountByEmailAsync_ShouldReturnAccount_WhenFoundInDb()
        {
            // Arrange
            var email = "test@example.com";
            var account = new AccountModel
            {
                AccountId = Guid.NewGuid(),
                Email = email,
                AccountName = "Test",
                RoleId = "user",  // RoleId as string
                IsBanned = false,
                CreatedAt = DateTime.UtcNow,
                LastIpAddress = "localhost",
                SessionId = Guid.Empty
            };
            _mockAccountDao.Setup(x => x.GetAccountByEmailAsync(email)).ReturnsAsync(account);

            // Act
            var result = await _accountService.GetAccountByEmailAsync(email);

            // Assert
            result.Should().NotBeNull();
            result?.Email.Should().Be(email);
        }

        [Fact]
        public async Task GetAccountByEmailAsync_ShouldReturnNull_WhenNotFoundInDb()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _mockAccountDao.Setup(x => x.GetAccountByEmailAsync(email)).ReturnsAsync((AccountModel)null);

            // Act
            var result = await _accountService.GetAccountByEmailAsync(email);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAccountAsync_ShouldThrowException_WhenEmailIsTaken()
        {
            // Arrange
            var account = new AccountModel
            {
                AccountId = Guid.NewGuid(),
                Email = "taken@example.com",
                AccountName = "NewAccount",
                RoleId = "user",  // RoleId as string
                LastIpAddress = "localhost",
                SessionId = Guid.Empty
            };
            _mockAccountDao.Setup(x => x.IsEmailTakenAsync(account.Email)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _accountService.CreateAccountAsync(account));
        }

        [Fact]
        public async Task CreateAccountAsync_ShouldThrowException_WhenNameIsTaken()
        {
            // Arrange
            var account = new AccountModel
            {
                AccountId = Guid.NewGuid(),
                Email = "new@example.com",
                AccountName = "TakenName",
                RoleId = "user",  // RoleId as string
                LastIpAddress = "localhost",
                SessionId = Guid.Empty
            };
            _mockAccountDao.Setup(x => x.IsNameTakenAsync(account.AccountName)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _accountService.CreateAccountAsync(account));
        }

        [Fact]
        public async Task SaveAccountAsync_ShouldUpdateAccount_WhenValid()
        {
            // Arrange
            var account = new AccountModel
            {
                AccountId = Guid.NewGuid(),
                Email = "valid@example.com",
                AccountName = "ValidAccount",
                RoleId = "user",  // RoleId as string
                LastIpAddress = "localhost",
                SessionId = Guid.Empty
            };
            _mockAccountDao.Setup(x => x.UpdateAccountAsync(account)).Returns((Task<bool>)Task.CompletedTask);

            // Act
            await _accountService.SaveAccountAsync(account);

            // Assert
            _mockAccountDao.Verify(x => x.UpdateAccountAsync(account), Times.Once);
        }

        [Fact]
        public async Task UpdateAccountPasswordAsync_ShouldReturnTrue_WhenSuccessful()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var newPassword = "NewPassword123";
            _mockAccountDao.Setup(x => x.UpdateAccountPasswordAsync(accountId, newPassword)).ReturnsAsync(true);

            // Act
            var result = await _accountService.UpdateAccountPasswordAsync(accountId, newPassword);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAccountNameAsync_ShouldUpdateName_WhenValid()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var newName = "NewName";
            var account = new AccountModel
            {
                AccountId = accountId,
                AccountName = "OldName",
                RoleId = "user",  // RoleId as string
                LastIpAddress = "localhost",
                SessionId = Guid.Empty
            };
            _mockAccountDao.Setup(x => x.UpdateAccountNameAsync(accountId, newName)).ReturnsAsync(true);
            _mockAccountDao.Setup(x => x.GetAccountByAccountIdAsync(accountId)).ReturnsAsync(account);

            // Act
            var result = await _accountService.UpdateAccountNameAsync(accountId, newName);

            // Assert
            result.Should().BeTrue();
            account.AccountName.Should().Be(newName);
        }

        [Fact]
        public async Task GetActivePlayerAsync_ShouldReturnActivePlayerId_WhenFound()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var account = new AccountModel
            {
                AccountId = accountId,
                ActivePlayerId = 123,
                LastIpAddress = "localhost",
                RoleId = "user",  // RoleId as string
                SessionId = Guid.Empty
            };
            _mockAccountDao.Setup(x => x.GetAccountByAccountIdAsync(accountId)).ReturnsAsync(account);

            // Act
            var result = await _accountService.GetActivePlayerAsync(accountId);

            // Assert
            result.Should().Be(123);
        }
    }
}
