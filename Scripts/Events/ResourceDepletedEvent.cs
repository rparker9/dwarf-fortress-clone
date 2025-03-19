using UnityEngine;

namespace ECS
{
    public class ResourceDepletedEvent : GameEvent
    {
        public ResourceDepletedEvent(Entity resource) : base(resource)
        {
        }
    }

}
