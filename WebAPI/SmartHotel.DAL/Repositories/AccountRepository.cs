using Microsoft.EntityFrameworkCore;
using SmartHotel.DAL.Data;
using SmartHotel.DAL.Models;
using SmartHotel.DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly SmartHotelDbContext _context;

        public AccountRepository(SmartHotelDbContext context)
        {
            _context = context;
        }

        public async Task<Account> GetAccountByEmailnameAsync(string email)
        {
            return await _context.Accounts
                .Include(a => a.AccountRoles)
                    .ThenInclude(ar => ar.Role) 
                .Include(a => a.User)   
                .Include(a => a.Tenant) 
                .FirstOrDefaultAsync(a => a.Email == email && a.IsActive);
        }
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Accounts.AnyAsync(a => a.Email == email);
        }
        public async Task<Account> GetAccountByIdAsync(int id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task CreateAccountAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }


        public async Task UpdatePasswordAsync(int accountId, string newPasswordHash)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account != null)
            {
                account.PasswordHash = newPasswordHash;
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.Account)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveOldRefreshTokensAsync(int accountId)
        {
            var oldTokens = _context.RefreshTokens
                .Where(rt => rt.AccountId == accountId && (rt.IsRevoked || rt.ExpiredAt <= DateTime.Now));

            if (oldTokens.Any())
            {
                _context.RefreshTokens.RemoveRange(oldTokens);
                await _context.SaveChangesAsync();
            }
        }
    }
}
