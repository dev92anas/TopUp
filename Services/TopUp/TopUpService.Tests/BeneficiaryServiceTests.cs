using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TopUp.Application.ExternalServices;
using TopUp.Domain.Entities;
using TopUp.Domain.Interfaces;
using TopUp.Infrastructure.Data;
using TopUpService.Application.Services;
using Xunit;

namespace TopUpService.Tests
{
    public class BeneficiaryServiceTests
    {
        private readonly Mock<IBeneficiaryRepository> _beneficiaryRepositoryMock;
        private readonly Mock<ITopUpTransactionRepository> _topUpTransactionRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILookupsRepository> _lookupRepositoryMock;
        private readonly Mock<IExternalBalanceService> _externalBalanceServiceMock;
        private readonly Mock<ILogger<BeneficiaryService>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly TopUpDbContext _context;
        private readonly BeneficiaryService _beneficiaryService;

        public BeneficiaryServiceTests()
        {
            _beneficiaryRepositoryMock = new Mock<IBeneficiaryRepository>();
            _topUpTransactionRepositoryMock = new Mock<ITopUpTransactionRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _lookupRepositoryMock = new Mock<ILookupsRepository>();
            _externalBalanceServiceMock = new Mock<IExternalBalanceService>();
            _loggerMock = new Mock<ILogger<BeneficiaryService>>();
            _configurationMock = new Mock<IConfiguration>();

            var options = new DbContextOptionsBuilder<TopUpDbContext>()
                .UseInMemoryDatabase(databaseName: "TopUpTestDb")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _context = new TopUpDbContext(options);

            _beneficiaryService = new BeneficiaryService(
                _beneficiaryRepositoryMock.Object,
                _topUpTransactionRepositoryMock.Object,
                _lookupRepositoryMock.Object,
                _loggerMock.Object,
                _userRepositoryMock.Object,
                _externalBalanceServiceMock.Object,
                _configurationMock.Object,
                _context
            );
        }

        [Fact]
        public async Task GetBeneficiariesAsync_ShouldReturnBeneficiaries()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser" };
            var beneficiaries = new List<Beneficiary>
            {
                new Beneficiary { UserId = 1, Nickname = "Friend", PhoneNumber = "1234567890" }
            };

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _beneficiaryRepositoryMock.Setup(repo => repo.GetAllBeneficiaryForUserAsync(It.IsAny<int>())).ReturnsAsync(beneficiaries);

            // Act
            var result = await _beneficiaryService.GetBeneficiariesAsync("testuser");

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task AddBeneficiaryAsync_ShouldThrowException_WhenNicknameIsTooLong()
        {
            // Arrange
            string longNickname = new string('a', 21);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _beneficiaryService.AddBeneficiaryAsync("testuser", longNickname, "1234567890"));
            Assert.Equal("The Beneficiary nickname is not valid.", exception.Message);
        }

        [Fact]
        public async Task AddBeneficiaryAsync_ShouldThrowException_WhenBeneficiaryLimitExceeded()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser" };
            var beneficiaries = new List<Beneficiary>
            {
                new Beneficiary { UserId = 1, Nickname = "Friend1", PhoneNumber = "1234567890" },
                new Beneficiary { UserId = 1, Nickname = "Friend2", PhoneNumber = "1234567890" },
                new Beneficiary { UserId = 1, Nickname = "Friend3", PhoneNumber = "1234567890" },
                new Beneficiary { UserId = 1, Nickname = "Friend4", PhoneNumber = "1234567890" },
                new Beneficiary { UserId = 1, Nickname = "Friend5", PhoneNumber = "1234567890" }
            };

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _beneficiaryRepositoryMock.Setup(repo => repo.GetAllBeneficiaryForUserAsync(It.IsAny<int>())).ReturnsAsync(beneficiaries);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _beneficiaryService.AddBeneficiaryAsync("testuser", "Friend6", "1234567890"));
            Assert.Equal("You have excced the maximum numbers of beneficiaries.", exception.Message);
        }

        [Fact]
        public async Task TopUpBeneficiaryAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _beneficiaryService.TopUpBeneficiaryAsync("testuser", 1, 1, "valid-idempotency-key"));
            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public async Task TopUpBeneficiaryAsync_ShouldThrowException_WhenBeneficiaryNotFound()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser" };

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _beneficiaryRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Beneficiary)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _beneficiaryService.TopUpBeneficiaryAsync("testuser", 1, 1, "valid-idempotency-key"));
            Assert.Equal("Beneficiary not found", exception.Message);
        }

        [Fact]
        public async Task TopUpBeneficiaryAsync_ShouldThrowException_WhenInvalidAmountValue()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser" };
            var beneficiary = new Beneficiary { UserId = 1, Nickname = "Friend", PhoneNumber = "1234567890" };

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _beneficiaryRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(beneficiary);
            _lookupRepositoryMock.Setup(repo => repo.GetByLookupIdAsync(It.IsAny<int>())).ReturnsAsync((Lookup)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _beneficiaryService.TopUpBeneficiaryAsync("testuser", 1, 1, "valid-idempotency-key"));
            Assert.Equal("Invalid amount value - key not found", exception.Message);
        }
    }
}
