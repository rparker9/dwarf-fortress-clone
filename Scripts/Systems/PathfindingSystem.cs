using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    // Pathfinding system that integrates with our entity framework
    public class PathfindingSystem : ISystem
    {
        private Grid grid;

        public PathfindingSystem(Grid grid)
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

                // If entity is not at destination but not currently moving, find a path
                if (position.Position != movement.Destination &&
                    !entity.HasComponent<PathComponent>())
                {
                    List<Vector2Int> path = grid.FindPath(position.Position, movement.Destination);

                    if (path != null && path.Count > 0)
                    {
                        var pathComponent = entity.AddComponent(new PathComponent(path));
                    }
                    else
                    {
                        // No path found, reset destination
                        movement.Destination = position.Position;
                    }
                }
            }
        }
    }
}
