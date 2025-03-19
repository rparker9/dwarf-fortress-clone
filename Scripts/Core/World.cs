using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    /// <summary>
    /// The main simulation manager for the ECS system.
    /// Manages entities, systems, and the grid world.
    /// Implements the Singleton pattern.
    /// </summary>
    public class World : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the time in seconds between simulation updates.
        /// </summary>
        public float TickRate = 0.1f;

        /// <summary>
        /// Reference to the grid visualizer component.
        /// </summary>
        [SerializeField] private GridVisualizer gridVisualizer;

        /// <summary>
        /// List of all entities in the world.
        /// </summary>
        private List<Entity> entities = new List<Entity>();

        /// <summary>
        /// List of all systems that process entities.
        /// </summary>
        private List<ISystem> systems = new List<ISystem>();

        /// <summary>
        /// Counter for generating unique entity IDs.
        /// </summary>
        private int nextEntityID = 0;

        /// <summary>
        /// Timer for tracking when to update the simulation.
        /// </summary>
        private float tickTimer = 0;

        /// <summary>
        /// Singleton instance of the World.
        /// </summary>
        private static World instance;

        /// <summary>
        /// Gets the singleton instance of the World.
        /// </summary>
        public static World Instance => instance;

        /// <summary>
        /// Gets the grid that represents the world.
        /// </summary>
        public Grid Grid { get; private set; }

        /// <summary>
        /// Initializes the world on scene start.
        /// </summary>
        private void Awake()
        {
            // Ensure singleton pattern
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            // Create the grid with specified dimensions
            Grid = new Grid(100, 100);

            // Initialize the grid visualizer if assigned
            if (gridVisualizer != null)
            {
                gridVisualizer.Initialize(Grid);
            }
            else
            {
                Debug.LogWarning("GridVisualizer not assigned in the inspector!");
            }

            // Register all systems
            RegisterSystem(new PathfindingSystem(Grid));
            RegisterSystem(new AdvancedMovementSystem(Grid));
            RegisterSystem(new VisualsSystem());
            RegisterSystem(new BasicAISystem(Grid));
        }

        /// <summary>
        /// Updates the simulation based on the tick rate.
        /// </summary>
        private void Update()
        {
            // Increment the timer
            tickTimer += Time.deltaTime;

            // Check if it's time for a simulation update
            if (tickTimer >= TickRate)
            {
                Tick();
                tickTimer = 0;
            }
        }

        /// <summary>
        /// Performs a single simulation update.
        /// </summary>
        public void Tick()
        {
            // Process all systems
            foreach (var system in systems)
            {
                system.Process(entities);
            }

            // Process any pending events
            EventSystem.Instance.ProcessEvents();
        }

        /// <summary>
        /// Creates a new entity with a unique ID.
        /// </summary>
        /// <returns>The newly created entity.</returns>
        public Entity CreateEntity()
        {
            // Create entity with a unique ID
            var entity = new Entity(nextEntityID++);
            entities.Add(entity);
            return entity;
        }

        /// <summary>
        /// Removes an entity from the world.
        /// </summary>
        /// <param name="entity">The entity to destroy.</param>
        public void DestroyEntity(Entity entity)
        {
            entities.Remove(entity);
        }

        /// <summary>
        /// Registers a system with the world.
        /// </summary>
        /// <param name="system">The system to register.</param>
        public void RegisterSystem(ISystem system)
        {
            systems.Add(system);
            system.Initialize();
        }

        /// <summary>
        /// Creates a basic entity with position and visuals components.
        /// </summary>
        /// <param name="x">The x-coordinate of the entity.</param>
        /// <param name="y">The y-coordinate of the entity.</param>
        /// <param name="visualPrefab">The prefab to use for visuals.</param>
        /// <returns>The newly created entity.</returns>
        public Entity CreateBasicEntity(int x, int y, GameObject visualPrefab)
        {
            // Create a new entity
            Entity entity = CreateEntity();

            // Add components in dependency order
            var posComp = entity.AddComponent(new PositionComponent(x, y));
            var moveComp = entity.AddComponent(new MovementComponent());
            var visComp = entity.AddComponent(new VisualsComponent(visualPrefab));

            // Ensure explicit initialization has occurred
            posComp.Initialize();
            moveComp.Initialize();
            visComp.Initialize();

            return entity;
        }

        /// <summary>
        /// Creates an AI-controlled entity with basic AI behavior.
        /// </summary>
        /// <param name="x">The x-coordinate of the entity.</param>
        /// <param name="y">The y-coordinate of the entity.</param>
        /// <param name="visualPrefab">The prefab to use for visuals.</param>
        /// <returns>The newly created entity with AI capabilities.</returns>
        public Entity CreateAIEntity(int x, int y, GameObject visualPrefab)
        {
            // Create a basic entity first
            Entity entity = CreateBasicEntity(x, y, visualPrefab);

            // Add AI component
            entity.AddComponent(new BasicAIComponent());

            return entity;
        }
    }
}