using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopUp.Domain.Entities
{
    public class TopUpTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BeneficiaryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string IdempotencyKey { get; set; } // property for idempotency key

    }
}
