using UnityEngine;

namespace ECS
{
    // Needs component for entities
    public class NeedsComponent : Component
    {
        public float Hunger { get; set; } = 0;
        public float Thirst { get; set; } = 0;
        public float Fatigue { get; set; } = 0;

        public float MaxHunger { get; set; } = 100;
        public float MaxThirst { get; set; } = 100;
        public float MaxFatigue { get; set; } = 100;

        public float HungerRate { get; set; } = 0.5f;
        public float ThirstRate { get; set; } = 0.8f;
        public float FatigueRate { get; set; } = 0.3f;
    }
}
