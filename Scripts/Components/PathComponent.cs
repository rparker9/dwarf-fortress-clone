using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ECS
{
    /// <summary>
    /// Component that stores and manages a path for an entity to follow.
    /// </summary>
    public class PathComponent : Component
    {
        /// <summary>
        /// Gets the list of waypoints that form the path.
        /// </summary>
        public List<Vector2Int> Path { get; private set; }

        /// <summary>
        /// Gets or sets the index of the current waypoint in the path.
        /// </summary>
        public int CurrentWaypoint { get; set; } = 0;

        /// <summary>
        /// Creates a new PathComponent with the specified path.
        /// </summary>
        /// <param name="path">The path as a list of Vector2Int positions.</param>
        public PathComponent(List<Vector2Int> path)
        {
            Path = path;
        }

        /// <summary>
        /// Gets the next waypoint position in the path.
        /// </summary>
        /// <returns>The next waypoint position, or the last position if the path is completed.</returns>
        public Vector2Int GetNextWaypoint()
        {
            // If we still have waypoints remaining, return the current one
            if (CurrentWaypoint < Path.Count)
                return Path[CurrentWaypoint];

            // Otherwise return the last waypoint
            return Path.Last();
        }

        /// <summary>
        /// Checks if the path has been completed.
        /// </summary>
        /// <returns>True if all waypoints have been visited, false otherwise.</returns>
        public bool IsPathCompleted()
        {
            return CurrentWaypoint >= Path.Count;
        }

        /// <summary>
        /// Advances to the next waypoint in the path.
        /// </summary>
        public void AdvanceWaypoint()
        {
            CurrentWaypoint++;
        }
    }
}