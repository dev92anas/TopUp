using TopUp.Application.DTOs;
using TopUp.Application.ExternalServices;
using TopUp.Application.Services.IServices;
using TopUp.Domain.Entities;
using TopUp.Domain.Interfaces;
using TopUp.Application.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using TopUp.Infrastructure.Data;

namespace TopUpService.Application.Services
{
    public class BeneficiaryService : IBeneficiaryService
    {
        private readonly IBeneficiaryRepository _beneficiaryRepository;
        private readonly ITopUpTransactionRepository _topUpTransactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILookupsRepository _lookupRepository;
        private readonly ILogger<BeneficiaryService> _logger;
        IExternalBalanceService _externalBalanceService;
        private readonly IConfiguration _configuration;
        private readonly TopUpDbContext _context;
        public BeneficiaryService(IBeneficiaryRepository beneficiaryRepository,
                                  ITopUpTransactionRepository topUpTransactionRepository,
                                  ILookupsRepository lookupRepository,
                                  ILogger<BeneficiaryService> logger,
                                  IUserRepository userRepository,
                                  IExternalBalanceService externalBalanceService,
                                  IConfiguration configuration,
                                  TopUpDbContext context)
        {
            _beneficiaryRepository = beneficiaryRepository;
            _topUpTransactionRepository = topUpTransactionRepository;
            _lookupRepository = lookupRepository;
            _logger = logger;
            _userRepository = userRepository;
            _externalBalanceService = externalBalanceService;
            _configuration = configuration;
            _context = context;
        }

        public async Task<IEnumerable<Beneficiary>> GetBeneficiariesAsync(string username)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                return await _beneficiaryRepository.GetAllBeneficiaryForUserAsync(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving beneficiaries for user {Username}", username);
                throw new Exception("An error occurred while retrieving beneficiaries.");
            }
        }

        public async Task AddBeneficiaryAsync(string username, string nickname, string phoneNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(nickname) || nickname.Length > 20)
                    throw new Exception("The Beneficiary nickname is not valid.");

                var user = await _userRepository.GetUserByUsernameAsync(username);

                var beneficiaries = await _beneficiaryRepository.GetAllBeneficiaryForUserAsync(user.Id);
                if (beneficiaries != null && beneficiaries.Count() >= 5)
                    throw new Exception("You have excced the maximum numbers of beneficiaries.");

                if (beneficiaries != null && beneficiaries.Where(x => x.Nickname == nickname).Count() > 0)
                    throw new Exception("beneficiary name dublicated.");

                var beneficiary = new Beneficiary
                {
                    UserId = user.Id,
                    Nickname = nickname,
                    PhoneNumber = phoneNumber
                };
                await _beneficiaryRepository.AddAsync(beneficiary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding beneficiary for user {Username}", username);
                throw new Exception(ex.Message);
            }
        }

        public async Task TopUpBeneficiaryAsync(string username, int beneficiaryId, int lookupAmountValueId, string idempotencyKey)
        {
            /// hold database transaction for the hole proccess
            using var transaction = await _context.Database.BeginTransactionAsync();
            bool debitSuccessful = false;
            decimal rollbackAmount = decimal.Zero;

            try
            {
                if(idempotencyKey.Length < 20 || idempotencyKey.Length > 200)
                    throw new Exception("wrong idempotency Key (the key length should be between 20 to 200 character");

                // Check for existing idempotent transaction --> to ensure that multiple identical requests are processed only once
                var existingTransaction = await _topUpTransactionRepository.GetByIdempotencyKeyAsync(idempotencyKey);
                if (existingTransaction != null)
                {
                    throw new Exception("the transaction already saved");
                }

                var user = await _userRepository.GetUserByUsernameAsync(username);
                if (user == null) throw new Exception("User not found");

                var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId);
                if (beneficiary == null || beneficiary.UserId != user.Id)
                    throw new Exception("Beneficiary not found");

                var lookupAmountValue = await _lookupRepository.GetByLookupIdAsync(lookupAmountValueId);
                if (lookupAmountValue == null || lookupAmountValue.GroupId != (int)LookupsGroups.TopUpOptions) throw new Exception("Invalid amount value - key not found");

                var amount = Convert.ToDecimal(lookupAmountValue.Value);
                var currentBalance = await _externalBalanceService.GetBalanceAsync(user.Username);
                if (currentBalance < amount + 1) throw new Exception("Insufficient balance");

                decimal maxMonthlyTopUp = 0;
                decimal totalTopUpLimit = 0;

                /// set topup config for the users
                var VerifiedUserLimit = Convert.ToDecimal(_configuration["TopUpsLimit:VerifiedUserLimit"]);
                var UnVerifiedUserLimit = Convert.ToDecimal(_configuration["TopUpsLimit:UnVerifiedUserLimit"]);
                maxMonthlyTopUp = user.IsVerified ? VerifiedUserLimit : UnVerifiedUserLimit;
                totalTopUpLimit = Convert.ToDecimal(_configuration["TopUpsLimit:TotalTopUpLimitForTheUser"]);

                var now = DateTime.UtcNow;
                var transactions = await _topUpTransactionRepository.GetBeneficiaryAndUserTransactionsAsync(user.Id, beneficiaryId, now.AddDays(-30));
                var totalTopUpForBeneficiary = transactions != null ? transactions.Sum(t => t.Amount) : decimal.Zero;

                if (totalTopUpForBeneficiary + amount > maxMonthlyTopUp)
                    throw new Exception("Monthly top-up limit exceeded for beneficiary");

                var totalTopUpForUser = await _topUpTransactionRepository.GetTotalTopUpForUserAsync(user.Id, now.AddDays(-30));

                if (totalTopUpForUser + amount > totalTopUpLimit)
                    throw new Exception("Total monthly top-up limit exceeded");

                // Debit balance first
                debitSuccessful = await _externalBalanceService.DebitBalanceAsync(user.Username, amount + 1);
                if (!debitSuccessful)
                    throw new Exception("Failed to debit balance");

                rollbackAmount = amount + 1;

                // Record the transaction
                var topUpTransaction = new TopUpTransaction
                {
                    UserId = user.Id,
                    BeneficiaryId = beneficiaryId,
                    Amount = amount,
                    Date = now,
                    IdempotencyKey = idempotencyKey
                };

                // save the transaction
                await _topUpTransactionRepository.AddAsync(topUpTransaction);

                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                /// rollback the transaction if failure
                await transaction.RollbackAsync();

                if (debitSuccessful && rollbackAmount > 0)
                {
                    // if topUp transaction faild, refund the user ---> i recommend to use async messaging (rabbitMQ, azure serviceBus....etc)
                    var creditSuccessful = await _externalBalanceService.CreditBalanceAsync(username, rollbackAmount);
                    if (!creditSuccessful)
                        throw new Exception("Error while trying to save the topUp transaction, And the user can't be refunded at this time");
                    else
                        throw new Exception("Error while trying to save the topUp transaction, the user refunded");

                }

                _logger.LogError(ex, "Error topping up beneficiary for user {Username}", username);
                throw new Exception(ex.Message);
            }
        }
    }
}
