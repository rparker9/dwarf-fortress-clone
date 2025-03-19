using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    /// <summary>
    /// Manages the initial setup and configuration of the simulation.
    /// Handles entity creation and obstacle generation.
    /// </summary>
    public class SimulationManager : MonoBehaviour
    {
        /// <summary>
        /// The prefab to use for creating entities.
        /// </summary>
        public GameObject entityPrefab;

        /// <summary>
        /// The number of entities to create at the start of the simulation.
        /// </summary>
        public int initialEntityCount = 50;

        /// <summary>
        /// Initializes the simulation by creating entities and obstacles.
        /// </summary>
        private void Start()
        {
            // Create the initial entities
            for (int i = 0; i < initialEntityCount; i++)
            {
                // Generate a random position
                int x = Random.Range(0, World.Instance.Grid.Width);
                int y = Random.Range(0, World.Instance.Grid.Height);

                // Ensure the position is walkable
                while (!World.Instance.Grid.IsWalkable(x, y))
                {
                    x = Random.Range(0, World.Instance.Grid.Width);
                    y = Random.Range(0, World.Instance.Grid.Height);
                }

                // Create an AI entity at the valid position
                World.Instance.CreateAIEntity(x, y, entityPrefab);
            }

            // Generate obstacles in the world
            GenerateObstacles();
        }

        /// <summary>
        /// Generates random obstacles throughout the grid.
        /// Creates clusters of obstacles for more natural layouts.
        /// </summary>
        private void GenerateObstacles()
        {
            Grid grid = World.Instance.Grid;

            // Create obstacle clusters throughout the grid
            for (int i = 0; i < grid.Width * grid.Height / 20; i++)
            {
                // Choose a random center point for the cluster
                int x = Random.Range(0, grid.Width);
                int y = Random.Range(0, grid.Height);

                // Create obstacles in the 3x3 area around the center
                for (int ox = -1; ox <= 1; ox++)
                {
                    for (int oy = -1; oy <= 1; oy++)
                    {
                        int nx = x + ox;
                        int ny = y + oy;

                        // Randomly decide if this cell should be an obstacle
                        if (grid.IsInBounds(nx, ny) && Random.value > 0.5f)
                        {
                            grid.SetWalkable(nx, ny, false);
                        }
                    }
                }
            }
        }
    }
}