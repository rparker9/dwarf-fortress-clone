using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    /// <summary>
    /// Visual update system
    /// </summary>
    public class VisualsSystem : ISystem
    {
        public void Initialize() { }

        public void Process(List<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (!entity.HasComponent<PositionComponent>() || !entity.HasComponent<VisualsComponent>())
                    continue;

                var position = entity.GetComponent<PositionComponent>();
                var visuals = entity.GetComponent<VisualsComponent>();

                visuals.UpdatePosition(position.Position);
            }
        }
    }
}
