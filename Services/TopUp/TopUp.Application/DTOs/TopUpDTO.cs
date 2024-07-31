using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopUp.Application.DTOs
{
    public class TopUpDTO
    {

        public int LookupAmountValueId { get; set; }
        public int BeneficiaryId { get; set; }
        public string idempotencyKey { get; set; }
        

    }
}
