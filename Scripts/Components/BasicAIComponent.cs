using UnityEngine;

namespace ECS
{
    /// <summary>
    /// Basic AI component
    /// </summary>
    public class BasicAIComponent : Component
    {
        public float DecisionInterval { get; set; } = 5.0f;
        private float decisionTimer = 0;

        public bool NeedsNewDecision => decisionTimer <= 0;

        public void ResetDecisionTimer()
        {
            decisionTimer = DecisionInterval;
        }

        public void UpdateTimer(float deltaTime)
        {
            decisionTimer -= deltaTime;
        }
    }
}
