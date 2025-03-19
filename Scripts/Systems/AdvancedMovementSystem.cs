using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    // Updated movement system to work with paths
    public class AdvancedMovementSystem : ISystem
    {
        private Grid grid;

        public AdvancedMovementSystem(Grid grid)
        {
            this.grid = grid;
        }

        public void Initialize() { }

        public void Process(List<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (!entity.HasComponent<MovementComponent>() ||
                    !entity.HasComponent<PositionComponent>())
                    continue;

                var movement = entity.GetComponent<MovementComponent>();
                var position = entity.GetComponent<PositionComponent>();

                // If we have a path, follow it
                if (entity.HasComponent<PathComponent>())
                {
                    var pathComponent = entity.GetComponent<PathComponent>();

                    if (pathComponent.IsPathCompleted())
                    {
                        entity.RemoveComponent<PathComponent>();
                        continue;
                    }

                    Vector2Int nextWaypoint = pathComponent.GetNextWaypoint();

                    // If we've reached the current waypoint, advance to the next one
                    if (position.Position == nextWaypoint)
                    {
                        pathComponent.AdvanceWaypoint();

                        if (pathComponent.IsPathCompleted())
                        {
                            entity.RemoveComponent<PathComponent>();
                            continue;
                        }

                        nextWaypoint = pathComponent.GetNextWaypoint();
                    }

                    // Move towards the next waypoint
                    Vector2Int direction = new Vector2Int(
                        Math.Sign(nextWaypoint.x - position.Position.x),
                        Math.Sign(nextWaypoint.y - position.Position.y)
                    );

                    Vector2Int newPosition = position.Position + direction;

                    // Check if the new position is valid
                    if (grid.IsWalkable(newPosition.x, newPosition.y))
                    {
                        position.Position = newPosition;
                    }
                    else
                    {
                        // Path is blocked, need to recalculate
                        entity.RemoveComponent<PathComponent>();
                    }
                }
            }
        }
    }
}
