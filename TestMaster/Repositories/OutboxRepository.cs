using Common.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TestMaster.DataAccess;
using TestMaster.Interfaces;

namespace TestMaster.Repositories
{
    internal class OutboxRepository : IOutboxRepository
    {
        private readonly MasterDbContext _context;

        public OutboxRepository(MasterDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(OutboxMessage message)
        {
            _context.OutboxMessages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OutboxMessage>> GetUnprocessedAsync()
        {
            return await _context.OutboxMessages
                .Where(m => !m.Processed)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsProcessedAsync(Guid id)
        {
            var message = await _context.OutboxMessages.FindAsync(id);
            if (message != null)
            {
                message.Processed = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
