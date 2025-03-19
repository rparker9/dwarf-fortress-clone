using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    // Batch pathfinding system for handling many entities efficiently
    public class BatchPathfindingSystem : ISystem
    {
        private Grid grid;
        private HierarchicalPathfinder hierarchicalPathfinder;
        private PathSmoother pathSmoother;
        private int maxPathfindingRequestsPerTick = 20; // Limit requests per tick to prevent performance issues
        private Queue<Entity> pathfindingQueue = new Queue<Entity>();

        public BatchPathfindingSystem(Grid grid)
        {
            this.grid = grid;
            hierarchicalPathfinder = new HierarchicalPathfinder(grid);
            pathSmoother = new PathSmoother(grid);
        }

        public void Initialize() { }

        public void Process(List<Entity> entities)
        {
            // Add new entities that need pathfinding to the queue
            foreach (var entity in entities)
            {
                if (!entity.HasComponent<MovementComponent>() || !entity.HasComponent<PositionComponent>())
                    continue;

                var movement = entity.GetComponent<MovementComponent>();
                var position = entity.GetComponent<PositionComponent>();

                // If entity needs a path and isn't already in queue or processing
                if (position.Position != movement.Destination &&
                    !entity.HasComponent<PathComponent>() &&
                    !entity.HasComponent<PathRequestComponent>() &&
                    !pathfindingQueue.Contains(entity))
                {
                    entity.AddComponent(new PathRequestComponent());
                    pathfindingQueue.Enqueue(entity);
                }
            }

            // Process a limited number of pathfinding requests this tick
            int requestsProcessed = 0;
            while (pathfindingQueue.Count > 0 && requestsProcessed < maxPathfindingRequestsPerTick)
            {
                Entity entity = pathfindingQueue.Dequeue();

                if (!entity.HasComponent<PathRequestComponent>())
                    continue;

                var movement = entity.GetComponent<MovementComponent>();
                var position = entity.GetComponent<PositionComponent>();

                // Find the path
                List<Vector2Int> path;

                // For longer paths, use hierarchical pathfinding
                if (Vector2Int.Distance(position.Position, movement.Destination) > 20)
                {
                    path = hierarchicalPathfinder.FindPath(position.Position, movement.Destination);
                }
                else
                {
                    path = grid.FindPath(position.Position, movement.Destination);
                }

                // If we found a path, smooth it and add to the entity
                if (path != null && path.Count > 0)
                {
                    List<Vector2Int> smoothedPath = pathSmoother.SmoothPath(path);
                    entity.AddComponent(new PathComponent(smoothedPath));
                }
                else
                {
                    // No path found, reset destination
                    movement.Destination = position.Position;
                }

                // Remove the request component
                entity.RemoveComponent<PathRequestComponent>();
                requestsProcessed++;
            }
        }
    }
}
