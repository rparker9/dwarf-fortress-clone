using UnityEngine;

namespace ECS
{
    /// <summary>
    /// Visual representation component
    /// </summary>
    public class VisualsComponent : Component
    {
        public GameObject GO { get; private set; }
        public SpriteRenderer SR { get; private set; }

        public VisualsComponent(GameObject prefab)
        {
            GO = GameObject.Instantiate(prefab);
            SR = GO.GetComponent<SpriteRenderer>();
        }

        public void UpdatePosition(Vector2 position)
        {
            GO.transform.position = new Vector3(position.x, position.y, 0);
        }
    }
}
