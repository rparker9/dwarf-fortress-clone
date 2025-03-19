using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    // Resource gathering system
    public class ResourceGatheringSystem : ISystem
    {
        public void Initialize() { }

        public void Process(List<Entity> entities)
        {
            // Group resource entities by position for efficient lookup
            Dictionary<Vector2Int, List<Entity>> resourcesByPosition = new Dictionary<Vector2Int, List<Entity>>();

            foreach (var entity in entities)
            {
                if (entity.HasComponent<ResourceComponent>() && entity.HasComponent<PositionComponent>())
                {
                    var position = entity.GetComponent<PositionComponent>();

                    if (!resourcesByPosition.ContainsKey(position.Position))
                        resourcesByPosition[position.Position] = new List<Entity>();

                    resourcesByPosition[position.Position].Add(entity);
                }
            }

            // Process gatherers
            foreach (var entity in entities)
            {
                if (!entity.HasComponent<ResourceGathererComponent>() ||
                    !entity.HasComponent<PositionComponent>())
                    continue;

                var gatherer = entity.GetComponent<ResourceGathererComponent>();
                var position = entity.GetComponent<PositionComponent>();

                gatherer.UpdateTimer(World.Instance.TickRate);

                if (gatherer.CanGather &&
                    resourcesByPosition.TryGetValue(position.Position, out List<Entity> resources))
                {
                    // Find matching resource type
                    foreach (var resource in resources)
                    {
                        var resourceComp = resource.GetComponent<ResourceComponent>();

                        if (resourceComp.ResourceType == gatherer.TargetResourceType &&
                            resourceComp.Amount > 0)
                        {
                            // Gather resource
                            int gatherAmount = Mathf.Min(gatherer.GatherAmount, resourceComp.Amount);
                            resourceComp.Amount -= gatherAmount;

                            // Add to gatherer's inventory or create one
                            if (!entity.HasComponent<InventoryComponent>())
                                entity.AddComponent(new InventoryComponent());

                            var inventory = entity.GetComponent<InventoryComponent>();
                            inventory.AddItem(gatherer.TargetResourceType, gatherAmount);

                            // Raise event
                            EventSystem.Instance.QueueEvent(new ResourceGatheredEvent(
                                entity, resource, gatherer.TargetResourceType, gatherAmount));

                            gatherer.ResetTimer();

                            // If resource is depleted, destroy it
                            if (resourceComp.Amount <= 0)
                            {
                                EventSystem.Instance.QueueEvent(new ResourceDepletedEvent(resource));
                                World.Instance.DestroyEntity(resource);
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}
