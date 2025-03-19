using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ECS
{
    // Grid implementation
    public class Grid
    {
        private Cell[,] cells;
        public int Width { get; private set; }
        public int Height { get; private set; }

        // Add event for cell changes
        public event Action<int, int> OnCellChanged;

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
            cells = new Cell[width, height];

            // Initialize cells
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new Cell(x, y);
                }
            }
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool IsWalkable(int x, int y)
        {
            if (!IsInBounds(x, y))
                return false;

            return cells[x, y].IsWalkable;
        }

        public void SetWalkable(int x, int y, bool walkable)
        {
            if (IsInBounds(x, y))
            {
                bool oldValue = cells[x, y].IsWalkable;
                cells[x, y].IsWalkable = walkable;

                // If the value actually changed, trigger the event
                if (oldValue != walkable)
                {
                    OnCellChanged?.Invoke(x, y);
                }

                // Invalidate pathfinding cache for this cell
                InvalidatePathsForCell(x, y);
            }
        }

        public Cell GetCell(int x, int y)
        {
            if (IsInBounds(x, y))
                return cells[x, y];
            return null;
        }

        private void InvalidatePathsForCell(int x, int y)
        {
            // Simple approach: just clear entire cache when cells change
            // For more advanced usage, you might want to only invalidate paths that pass through this cell
            pathCache.Clear();
        }

        // Fast chunked pathfinding implementation
        private Dictionary<Vector2Int, List<Vector2Int>> pathCache = new Dictionary<Vector2Int, List<Vector2Int>>();
        private int pathCacheMaxSize = 1000;  // Limit cache size to prevent memory issues

        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
        {
            // Check cache first
            Vector2Int cacheKey = new Vector2Int(
                start.x * Width + end.x,
                start.y * Height + end.y
            );

            if (pathCache.TryGetValue(cacheKey, out List<Vector2Int> cachedPath))
                return new List<Vector2Int>(cachedPath);  // Return a copy to prevent modification

            // Path not in cache, calculate it
            List<Vector2Int> path = AStarPathfinding(start, end);

            // Cache the result if we found a path
            if (path != null)
            {
                if (pathCache.Count >= pathCacheMaxSize)
                {
                    // Remove oldest entry
                    var oldestKey = pathCache.Keys.First();
                    pathCache.Remove(oldestKey);
                }

                pathCache[cacheKey] = new List<Vector2Int>(path);
            }

            return path;
        }

        private List<Vector2Int> AStarPathfinding(Vector2Int start, Vector2Int end)
        {
            // A* implementation
            var openSet = new PriorityQueue<Cell>();
            var closedSet = new HashSet<Vector2Int>();
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            var gScore = new Dictionary<Vector2Int, float>();
            var fScore = new Dictionary<Vector2Int, float>();

            Cell startCell = GetCell(start.x, start.y);
            Cell goalCell = GetCell(end.x, end.y);

            if (startCell == null || goalCell == null || !startCell.IsWalkable || !goalCell.IsWalkable)
                return null;

            openSet.Enqueue(startCell, 0);

            gScore[start] = 0;
            fScore[start] = HeuristicCost(start, end);

            while (openSet.Count > 0)
            {
                Cell current = openSet.Dequeue();
                Vector2Int currentPos = new Vector2Int(current.X, current.Y);

                if (currentPos == end)
                    return ReconstructPath(cameFrom, currentPos);

                closedSet.Add(currentPos);

                foreach (Vector2Int neighbor in GetNeighbors(currentPos))
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    float tentativeGScore = gScore[currentPos] + 1; // Assuming uniform cost

                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = currentPos;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + HeuristicCost(neighbor, end);

                        Cell neighborCell = GetCell(neighbor.x, neighbor.y);
                        if (!openSet.Contains(neighborCell))
                            openSet.Enqueue(neighborCell, fScore[neighbor]);
                    }
                }
            }

            // No path found
            return null;
        }

        private float HeuristicCost(Vector2Int a, Vector2Int b)
        {
            // Manhattan distance
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        private List<Vector2Int> GetNeighbors(Vector2Int position)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { 1, 0, -1, 0 };

            for (int i = 0; i < 4; i++)
            {
                int nx = position.x + dx[i];
                int ny = position.y + dy[i];

                if (IsInBounds(nx, ny) && IsWalkable(nx, ny))
                    neighbors.Add(new Vector2Int(nx, ny));
            }

            return neighbors;
        }

        private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            path.Add(current);

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Insert(0, current);
            }

            // Remove the starting position
            if (path.Count > 0)
                path.RemoveAt(0);

            return path;
        }
    }
}