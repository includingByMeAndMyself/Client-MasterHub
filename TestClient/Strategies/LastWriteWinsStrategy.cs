using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestClient.Strategies
{
    internal class LastWriteWinsStrategy : IConflictResolutionStrategy
    {
        public List<Item> ResolveConflicts(List<Item> serverItems, List<Item> localItems)
        {
            var result = new List<Item>();
            var allItems = serverItems.Concat(localItems).ToList();
            
            var groupedItems = allItems.GroupBy(x => x.Id);
            
            foreach (var group in groupedItems)
            {
                var latestItem = group.OrderByDescending(x => x.LastModified).First();
                result.Add(latestItem);
            }
            
            return result;
        }
    }
}
