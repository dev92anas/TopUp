using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopUp.Application.DTOs
{
    public class CreateUserDTO
    {
        public string Username { get; set; }
        public decimal InitialBalance { get; set; }
    }
}
