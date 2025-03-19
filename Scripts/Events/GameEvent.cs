using UnityEngine;

namespace ECS
{
    // Base class for all game events
    public abstract class GameEvent
    {
        public Entity Sender { get; protected set; }

        public GameEvent(Entity sender)
        {
            Sender = sender;
        }
    }
}
