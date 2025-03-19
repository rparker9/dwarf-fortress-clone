using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    // Inventory component
    public class InventoryComponent : Component
    {
        private Dictionary<string, int> items = new Dictionary<string, int>();

        public void AddItem(string itemType, int amount)
        {
            if (!items.ContainsKey(itemType))
                items[itemType] = 0;

            items[itemType] += amount;
        }

        public bool RemoveItem(string itemType, int amount)
        {
            if (!items.ContainsKey(itemType) || items[itemType] < amount)
                return false;

            items[itemType] -= amount;

            if (items[itemType] <= 0)
                items.Remove(itemType);

            return true;
        }

        public int GetItemCount(string itemType)
        {
            if (items.ContainsKey(itemType))
                return items[itemType];
            return 0;
        }

        public Dictionary<string, int> GetAllItems()
        {
            return new Dictionary<string, int>(items);
        }
    }
}
