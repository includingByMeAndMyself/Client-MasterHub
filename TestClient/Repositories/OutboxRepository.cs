using Common.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TestClient.DataAccess;
using TestClient.Interfaces;

namespace TestClient.Repositories
{
    internal class OutboxRepository : IOutboxRepository
    {
        private readonly ClientDbContext _context;

        public OutboxRepository(ClientDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<OutboxMessage>> GetUnprocessedAsync()
        {
            return await _context.OutboxMessages
                .Where(m => !m.Processed)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(OutboxMessage message)
        {
            _context.OutboxMessages.Add(message);
            await _context.SaveChangesAsync();
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

        public async Task DeleteProcessedAsync()
        {
            var processedMessages = await _context.OutboxMessages
                .Where(m => m.Processed)
                .ToListAsync();
            
            _context.OutboxMessages.RemoveRange(processedMessages);
            await _context.SaveChangesAsync();
        }
    }
}
