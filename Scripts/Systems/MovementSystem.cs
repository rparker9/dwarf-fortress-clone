using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    /// <summary>
    /// Movement system
    /// </summary>
    public class MovementSystem : ISystem
    {
        private Grid grid;

        public MovementSystem(Grid grid)
        {
            this.grid = grid;
        }

        public void Initialize() { }

        public void Process(List<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (!entity.HasComponent<MovementComponent>() || !entity.HasComponent<PositionComponent>())
                    continue;

                var movement = entity.GetComponent<MovementComponent>();
                var position = entity.GetComponent<PositionComponent>();

                if (position.Position == movement.Destination)
                    continue;

                // Simple direct movement for now
                Vector2Int direction = new Vector2Int(
                    Math.Sign(movement.Destination.x - position.Position.x),
                    Math.Sign(movement.Destination.y - position.Position.y)
                );

                Vector2Int newPosition = position.Position + direction;

                // Check if the new position is valid
                if (grid.IsWalkable(newPosition.x, newPosition.y))
                {
                    position.Position = newPosition;
                }
                else
                {
                    // Path is blocked, could trigger pathfinding here
                    movement.Destination = position.Position; // Stop moving for now
                }
            }
        }
    }
}
