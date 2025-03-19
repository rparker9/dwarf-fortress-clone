using UnityEngine;

namespace ECS
{
    // Example event: Entity moved
    public class EntityMovedEvent : GameEvent
    {
        public Vector2Int OldPosition { get; private set; }
        public Vector2Int NewPosition { get; private set; }

        public EntityMovedEvent(Entity entity, Vector2Int oldPosition, Vector2Int newPosition)
            : base(entity)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
    }
}
