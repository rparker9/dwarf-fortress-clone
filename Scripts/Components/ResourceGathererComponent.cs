using UnityEngine;

namespace ECS
{
    // Resource gathering component
    public class ResourceGathererComponent : Component
    {
        public string TargetResourceType { get; set; }
        public int GatherAmount { get; set; } = 1;
        public float GatherInterval { get; set; } = 5.0f;
        private float gatherTimer = 0;

        public ResourceGathererComponent(string resourceType)
        {
            TargetResourceType = resourceType;
        }

        public bool CanGather => gatherTimer <= 0;

        public void ResetTimer()
        {
            gatherTimer = GatherInterval;
        }

        public void UpdateTimer(float deltaTime)
        {
            gatherTimer -= deltaTime;
        }
    }
}
