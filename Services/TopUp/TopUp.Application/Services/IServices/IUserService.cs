using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopUp.Application.DTOs;
using TopUp.Domain.Entities;

namespace TopUp.Application.Services.IServices
{
    public interface IUserService
    {
        Task AddUserAsync(string username);

    }
}
