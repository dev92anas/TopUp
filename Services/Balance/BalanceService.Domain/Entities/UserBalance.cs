using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceService.Domain.Entities
{
    public class UserBalance
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public decimal Balance { get; set; }
    }
}
