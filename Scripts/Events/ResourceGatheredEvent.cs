using UnityEngine;

namespace ECS
{
    // Resource related events
    public class ResourceGatheredEvent : GameEvent
    {
        public Entity ResourceEntity { get; private set; }
        public string ResourceType { get; private set; }
        public int Amount { get; private set; }

        public ResourceGatheredEvent(Entity gatherer, Entity resource, string resourceType, int amount)
            : base(gatherer)
        {
            ResourceEntity = resource;
            ResourceType = resourceType;
            Amount = amount;
        }
    }
}
