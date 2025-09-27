using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestMaster.Interfaces
{
    internal interface IOutboxRepository
    {
        Task AddAsync(OutboxMessage message);
        Task<List<OutboxMessage>> GetUnprocessedAsync();
        Task MarkAsProcessedAsync(Guid id);
    }
}
