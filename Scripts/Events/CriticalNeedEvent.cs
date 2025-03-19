using UnityEngine;

namespace ECS
{
    // Need-related events
    public class CriticalNeedEvent : GameEvent
    {
        public NeedsComponent Needs { get; private set; }

        public CriticalNeedEvent(Entity entity, NeedsComponent needs) : base(entity)
        {
            Needs = needs;
        }
    }
}
