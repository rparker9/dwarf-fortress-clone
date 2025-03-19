using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ECS
{
    public class GridVisualizer : MonoBehaviour
    {
        [Header("Tilemaps")]
        public Tilemap floorTilemap;
        public Tilemap featureTilemap;
        public Tilemap overlayTilemap;

        [Header("Tile Assets")]
        public TileBase[] floorTiles;  // Different floor types (grass, dirt, stone, etc.)
        public TileBase[] featureTiles; // Obstacles, walls, etc.
        public TileBase highlightTile;  // For path visualization, selection, etc.

        [Header("Debug")]
        public bool showGridLines = true;
        public Color gridLineColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        private Grid grid;
        private Dictionary<Vector3Int, int> tileTypeMap = new Dictionary<Vector3Int, int>();
        private List<Vector3Int> highlightedCells = new List<Vector3Int>();

        private void Awake()
        {
            if (floorTilemap == null || featureTilemap == null || overlayTilemap == null)
                Debug.LogError("One or more Tilemap components are missing!");
        }

        public void Initialize(Grid grid)
        {
            this.grid = grid;
            RefreshTilemap();

            grid.OnCellChanged += OnCellChanged;
        }

        private void OnCellChanged(int x, int y)
        {
            UpdateTile(x, y);
        }

        private void UpdateTile(int x, int y)
        {
            // Convert from your grid coordinates to tilemap coordinates
            // If using center-based cell coordinates, no offset needed
            // If using corner-based coordinates, offset may be needed
            Vector3Int cellPos = new Vector3Int(x, y, 0);
            Cell cell = grid.GetCell(x, y);

            // Determine floor tile type (could be based on cell properties)
            int floorType = GetFloorType(cell);

            // Update floor tilemap
            floorTilemap.SetTile(cellPos, floorTiles[floorType]);

            // Update feature tilemap based on walkability
            if (!cell.IsWalkable)
            {
                // Choose a feature tile based on surrounding context
                int featureType = GetFeatureType(x, y);
                featureTilemap.SetTile(cellPos, featureTiles[featureType]);
            }
            else
            {
                featureTilemap.SetTile(cellPos, null);
            }

            // Store tile type for later reference
            tileTypeMap[cellPos] = floorType;
        }

        private int GetFloorType(Cell cell)
        {
            // This is where you'd implement logic to determine the floor type
            // Could be based on moisture, elevation, temperature, etc.
            // For now, we'll just use a simple random assignment
            if (tileTypeMap.TryGetValue(new Vector3Int(cell.X, cell.Y, 0), out int existingType))
                return existingType;

            return Random.Range(0, floorTiles.Length);
        }

        private int GetFeatureType(int x, int y)
        {
            // This would determine what kind of feature to place
            // Based on context (e.g., connecting walls, etc.)
            // For now, return a random feature
            return Random.Range(0, featureTiles.Length);
        }

        private void RefreshTilemap()
        {
            // Clear all tilemaps
            floorTilemap.ClearAllTiles();
            featureTilemap.ClearAllTiles();
            overlayTilemap.ClearAllTiles();

            // Populate tilemaps
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    UpdateTile(x, y);
                }
            }
        }

        public void HighlightPath(List<Vector2Int> path)
        {
            // Clear previous highlights
            ClearHighlights();

            // Highlight each cell in the path
            foreach (var position in path)
            {
                Vector3Int cellPos = new Vector3Int(position.x, position.y, 0);
                overlayTilemap.SetTile(cellPos, highlightTile);
                highlightedCells.Add(cellPos);
            }
        }

        public void ClearHighlights()
        {
            foreach (var cell in highlightedCells)
            {
                overlayTilemap.SetTile(cell, null);
            }
            highlightedCells.Clear();
        }

        private void OnDrawGizmos()
        {
            if (grid == null || !showGridLines) return;

            Gizmos.color = gridLineColor;

            // Draw grid lines
            for (int x = 0; x <= grid.Width; x++)
            {
                Gizmos.DrawLine(
                    new Vector3(x, 0, 0),
                    new Vector3(x, grid.Height, 0)
                );
            }

            for (int y = 0; y <= grid.Height; y++)
            {
                Gizmos.DrawLine(
                    new Vector3(0, y, 0),
                    new Vector3(grid.Width, y, 0)
                );
            }
        }
    }
}