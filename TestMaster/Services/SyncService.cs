using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMaster.Interfaces;

namespace TestMaster.Services
{
    internal class SyncService : ISyncService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IOutboxRepository _outboxRepository;

        public SyncService(IItemRepository itemRepository, IOutboxRepository outboxRepository)
        {
            _itemRepository = itemRepository;
            _outboxRepository = outboxRepository;
        }

        public async Task<List<Item>> GetLatestDataAsync()
        {
            return (await _itemRepository.GetAllAsync()).ToList();
        }

        public async Task PushClientChangesAsync(List<Item> clientChanges)
        {
            foreach (var item in clientChanges)
            {
                await _itemRepository.AddOrUpdateAsync(item);
            }
        }

        public async Task MasterUpdateAsync(Item item)
        {
            // Сохраняем изменения мастера
            await _itemRepository.AddOrUpdateAsync(item);

            // Добавляем в outbox для отправки клиентам
            await _outboxRepository.AddAsync(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "ItemUpdate",
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(item),
                CreatedAt = DateTime.UtcNow,
                Processed = false
            });
        }
    }
}
