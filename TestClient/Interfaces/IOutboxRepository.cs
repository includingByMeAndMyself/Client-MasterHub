using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestClient.Interfaces
{
    internal interface IOutboxRepository
    {
        Task<List<OutboxMessage>> GetUnprocessedAsync();
        Task AddAsync(OutboxMessage message);
        Task MarkAsProcessedAsync(Guid id);
        Task DeleteProcessedAsync();
    }
}
