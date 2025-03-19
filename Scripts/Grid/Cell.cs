using System;
using UnityEngine;

namespace ECS
{
    /// <summary>
    /// Represents a single cell in the grid world.
    /// Stores terrain information and pathfinding data.
    /// </summary>
    public class Cell : IComparable<Cell>
    {
        /// <summary>
        /// Gets the X-coordinate of the cell in the grid.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Gets the Y-coordinate of the cell in the grid.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Gets or sets whether entities can move through this cell.
        /// </summary>
        public bool IsWalkable { get; set; } = true;

        /// <summary>
        /// Gets or sets the priority of this cell for pathfinding algorithms.
        /// </summary>
        public float Priority { get; set; }

        /// <summary>
        /// Gets or sets the terrain type of this cell.
        /// </summary>
        public TerrainType Terrain { get; set; } = TerrainType.Grass;

        /// <summary>
        /// Gets or sets the feature type of this cell (e.g., Wall, Tree).
        /// </summary>
        public FeatureType Feature { get; set; } = FeatureType.None;

        /// <summary>
        /// Gets or sets the movement cost for traversing this cell.
        /// Higher values make the cell more expensive to move through.
        /// </summary>
        public float MovementCost { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the elevation of this cell (for height-based terrain).
        /// </summary>
        public float Elevation { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the moisture level of this cell (for terrain generation).
        /// </summary>
        public float Moisture { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the temperature of this cell (for biome simulation).
        /// </summary>
        public float Temperature { get; set; } = 0f;

        /// <summary>
        /// Creates a new cell at the specified coordinates.
        /// </summary>
        /// <param name="x">The X-coordinate of the cell.</param>
        /// <param name="y">The Y-coordinate of the cell.</param>
        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Compares this cell with another cell based on priority.
        /// Used for priority queues in pathfinding.
        /// </summary>
        /// <param name="other">The other cell to compare with.</param>
        /// <returns>-1 if this cell has lower priority, 0 if equal, 1 if higher priority.</returns>
        public int CompareTo(Cell other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }

    /// <summary>
    /// Defines the different types of terrain that can exist in the grid.
    /// </summary>
    public enum TerrainType
    {
        /// <summary>Basic green terrain.</summary>
        Grass,
        /// <summary>Soil terrain.</summary>
        Dirt,
        /// <summary>Rocky terrain.</summary>
        Stone,
        /// <summary>Desert terrain.</summary>
        Sand,
        /// <summary>Liquid terrain, typically unwalkable.</summary>
        Water,
        /// <summary>Frozen terrain.</summary>
        Snow,
        /// <summary>Wet terrain with higher movement cost.</summary>
        Mud
    }

    /// <summary>
    /// Defines the different types of features that can exist on a cell.
    /// </summary>
    public enum FeatureType
    {
        /// <summary>No feature.</summary>
        None,
        /// <summary>Solid wall, typically unwalkable.</summary>
        Wall,
        /// <summary>Tree obstacle.</summary>
        Tree,
        /// <summary>Rock obstacle.</summary>
        Rock,
        /// <summary>Door feature, can be opened or closed.</summary>
        Door,
        /// <summary>Stairs feature, for level transitions.</summary>
        Stairs
    }
}