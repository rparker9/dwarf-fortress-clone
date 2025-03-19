using UnityEngine;

namespace ECS
{
    /// <summary>
    /// Position component
    /// </summary>
    public class PositionComponent : Component
    {
        public Vector2Int Position { get; set; }

        public PositionComponent(int x, int y)
        {
            Position = new Vector2Int(x, y);
        }
    }
}
