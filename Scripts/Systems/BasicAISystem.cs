using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    /// <summary>
    /// Basic AI system
    /// </summary>
    public class BasicAISystem : ISystem
    {
        private System.Random random = new System.Random();
        private Grid grid;

        public BasicAISystem(Grid grid)
        {
            this.grid = grid;
        }

        public void Initialize() { }

        public void Process(List<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (!entity.HasComponent<BasicAIComponent>() || !entity.HasComponent<MovementComponent>())
                    continue;

                BasicAIComponent ai = entity.GetComponent<BasicAIComponent>();
                MovementComponent movement = entity.GetComponent<MovementComponent>();

                ai.UpdateTimer(World.Instance.TickRate);

                if (ai.NeedsNewDecision && !movement.IsMoving)
                {
                    // Make a random movement decision
                    PositionComponent position = entity.GetComponent<PositionComponent>();

                    int maxAttempts = 10;
                    for (int i = 0; i < maxAttempts; i++)
                    {
                        int dx = random.Next(-5, 6);
                        int dy = random.Next(-5, 6);

                        Vector2Int target = position.Position + new Vector2Int(dx, dy);

                        if (grid.IsWalkable(target.x, target.y))
                        {
                            movement.Destination = target;
                            break;
                        }
                    }

                    ai.ResetDecisionTimer();
                }
            }
        }
    }
}
