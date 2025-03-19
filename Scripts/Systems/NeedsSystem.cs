using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    // Needs system
    public class NeedsSystem : ISystem
    {
        public void Initialize() { }

        public void Process(List<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (!entity.HasComponent<NeedsComponent>())
                    continue;

                var needs = entity.GetComponent<NeedsComponent>();

                // Increase needs over time
                needs.Hunger += needs.HungerRate * World.Instance.TickRate;
                needs.Thirst += needs.ThirstRate * World.Instance.TickRate;
                needs.Fatigue += needs.FatigueRate * World.Instance.TickRate;

                // Clamp values
                needs.Hunger = Mathf.Clamp(needs.Hunger, 0, needs.MaxHunger);
                needs.Thirst = Mathf.Clamp(needs.Thirst, 0, needs.MaxThirst);
                needs.Fatigue = Mathf.Clamp(needs.Fatigue, 0, needs.MaxFatigue);

                // Check for critical needs
                if (needs.Hunger >= needs.MaxHunger * 0.8f ||
                    needs.Thirst >= needs.MaxThirst * 0.8f ||
                    needs.Fatigue >= needs.MaxFatigue * 0.8f)
                {
                    EventSystem.Instance.QueueEvent(new CriticalNeedEvent(entity, needs));
                }
            }
        }
    }
}
