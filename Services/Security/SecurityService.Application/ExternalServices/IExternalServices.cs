using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityService.Application.ExternalServices
{
    public interface IExternalServices
    {
        Task AddUserBalanceAsync(string username);
        Task AddUserTopUpAsync(string username, bool isVerified);
    }
}
