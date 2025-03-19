using UnityEngine;

namespace ECS
{
    // Example goal: Find food
    public class FindFoodGoal : Goal
    {
        private Entity targetFoodEntity;

        public FindFoodGoal(Entity entity) : base(entity)
        {

        }

        public override void CalculatePriority()
        {
            if (Entity.HasComponent<NeedsComponent>())
            {
                var needs = Entity.GetComponent<NeedsComponent>();
                Priority = needs.Hunger / needs.MaxHunger;
            }
            else
            {
                Priority = 0;
            }
        }

        public override void Activate()
        {
            // Find nearest food source
            Vector2Int entityPos = Entity.GetComponent<PositionComponent>().Position;
            float closestDistance = float.MaxValue;

            foreach (var potentialTarget in World.Instance.GetEntitiesWithComponent<ResourceComponent>())
            {
                var resource = potentialTarget.GetComponent<ResourceComponent>();

                if (resource.ResourceType == "Food" && resource.Amount > 0)
                {
                    var targetPos = potentialTarget.GetComponent<PositionComponent>().Position;
                    float distance = Vector2.Distance(entityPos, targetPos);

                    if (distance < closestDistance)
                    {
                        targetFoodEntity = potentialTarget;
                        closestDistance = distance;
                    }
                }
            }

            if (targetFoodEntity != null)
            {
                // Set destination to food source
                var movement = Entity.GetComponent<MovementComponent>();
                var targetPos = targetFoodEntity.GetComponent<PositionComponent>().Position;
                movement.Destination = targetPos;
            }
        }

        public override bool IsCompleted()
        {
            // Check if the entity reached the food and ate it
            if (targetFoodEntity == null)
                return true;

            if (!World.Instance.EntityExists(targetFoodEntity))
                return true;

            var entityPos = Entity.GetComponent<PositionComponent>().Position;
            var targetPos = targetFoodEntity.GetComponent<PositionComponent>().Position;

            if (entityPos == targetPos)
            {
                // At food source, consume it
                if (Entity.HasComponent<ResourceGathererComponent>())
                {
                    var gatherer = Entity.GetComponent<ResourceGathererComponent>();
                    gatherer.TargetResourceType = "Food";

                    if (gatherer.CanGather)
                    {
                        // The ResourceGatheringSystem will handle the actual gathering
                        return true;
                    }
                }
            }

            return false;
        }

        public override void Process()
        {
            // Check if target still exists
            if (targetFoodEntity == null || !World.Instance.EntityExists(targetFoodEntity))
            {
                // Target no longer exists, find a new one
                Activate();
                return;
            }

            // Check if we need to update movement
            var entityPos = Entity.GetComponent<PositionComponent>().Position;
            var targetPos = targetFoodEntity.GetComponent<PositionComponent>().Position;
            var movement = Entity.GetComponent<MovementComponent>();

            if (movement.Destination != targetPos)
            {
                movement.Destination = targetPos;
            }
        }

        public override void Terminate()
        {
            targetFoodEntity = null;
        }
    }
}
