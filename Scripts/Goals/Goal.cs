using UnityEngine;

namespace ECS
{
    // Goal-based AI
    public abstract class Goal
    {
        public Entity Entity { get; protected set; }
        public float Priority { get; protected set; }

        public Goal(Entity entity)
        {
            Entity = entity;
        }

        public abstract void CalculatePriority();
        public abstract void Activate();
        public abstract bool IsCompleted();
        public abstract void Process();
        public abstract void Terminate();
    }
}
