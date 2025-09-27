using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using TestClient.DataAccess;
using TestClient.Interfaces;

namespace TestClient.Repositories
{
    internal class LocalItemRepository : ILocalItemRepository
    {
        private readonly ClientDbContext _context;

        public LocalItemRepository(ClientDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            return await _context.Items.ToListAsync();
        }

        public async Task<Item> GetByIdAsync(int id)
        {
            return await _context.Items.FindAsync(id);
        }

        public async Task AddOrUpdateAsync(Item item)
        {
            var existing = await _context.Items.FindAsync(item.Id);
            if (existing == null)
            {
                _context.Items.Add(item);
            }
            else
            {
                existing.Name = item.Name;
                existing.LastModified = item.LastModified;
                existing.ModifiedBy = item.ModifiedBy;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return false;

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Item>> GetModifiedSinceAsync(DateTime lastSync)
        {
            return await _context.Items
                .Where(i => i.LastModified > lastSync)
                .OrderBy(i => i.LastModified)
                .ToListAsync();
        }

        public async Task UpdateLastSyncTimeAsync()
        {
            var syncInfo = await _context.SyncInfos.FirstOrDefaultAsync();
            if (syncInfo == null)
            {
                syncInfo = new SyncInfo { LastSyncTime = DateTime.UtcNow };
                _context.SyncInfos.Add(syncInfo);
            }
            else
            {
                syncInfo.LastSyncTime = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<DateTime?> GetLastSyncTimeAsync()
        {
            var syncInfo = await _context.SyncInfos.FirstOrDefaultAsync();
            return syncInfo?.LastSyncTime;
        }
    }
}
