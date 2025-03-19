using UnityEngine;

namespace ECS
{
    // Resource component
    public class ResourceComponent : Component
    {
        public int Amount { get; set; }
        public string ResourceType { get; private set; }

        public ResourceComponent(string type, int initialAmount)
        {
            ResourceType = type;
            Amount = initialAmount;
        }
    }
}
