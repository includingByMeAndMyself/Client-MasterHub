using Common.Models;
using System.Collections.Generic;

namespace TestClient.Strategies
{
    internal interface IConflictResolutionStrategy
    {
        List<Item> ResolveConflicts(List<Item> serverItems, List<Item> localItems);
    }
}
