using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceService.Application.DTOs
{
    public class CreateUserBalanceRequest
    {
        public string Username { get; set; }
        public decimal InitialBalance { get; set; }
    }
}
