using SmartHotel.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Repositories.Interface
{
    public interface IAccountRepository
    {
        Task<Account> GetAccountByEmailnameAsync(string email); 
        Task<Account> GetAccountByIdAsync(int id);
        Task<bool> IsEmailExistsAsync(string email);
        Task CreateAccountAsync(Account account);
        Task UpdatePasswordAsync(int accountId, string newPasswordHash); 
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken); 
        Task RemoveOldRefreshTokensAsync(int accountId);
    }
}
