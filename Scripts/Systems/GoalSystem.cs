using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    // Goal management system
    public class GoalSystem : ISystem
    {
        public void Initialize() { }

        public void Process(List<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.HasComponent<GoalManager>())
                {
                    var goalManager = entity.GetComponent<GoalManager>();
                    goalManager.Process();
                }
            }
        }
    }
}
