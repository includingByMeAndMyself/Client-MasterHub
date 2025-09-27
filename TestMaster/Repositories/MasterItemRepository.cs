using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using TestMaster.DataAccess;
using TestMaster.Interfaces;

namespace TestMaster.Repositories
{
    public class MasterItemRepository : IItemRepository
    {
        private readonly MasterDbContext _context;

        public MasterItemRepository(MasterDbContext context)
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
    }
}
