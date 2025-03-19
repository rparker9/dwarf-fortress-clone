using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ECS
{
    // Hierarchical pathfinding for enhanced performance with many entities
    public class HierarchicalPathfinder
    {
        private Grid grid;
        private int chunkSize = 16; // Size of each chunk
        private Dictionary<Vector2Int, bool> chunkConnections = new Dictionary<Vector2Int, bool>();

        public HierarchicalPathfinder(Grid grid)
        {
            this.grid = grid;
            BuildChunkGraph();
        }

        private void BuildChunkGraph()
        {
            int chunksX = (grid.Width + chunkSize - 1) / chunkSize;
            int chunksY = (grid.Height + chunkSize - 1) / chunkSize;

            // For each chunk, calculate connections to neighboring chunks
            for (int cx = 0; cx < chunksX; cx++)
            {
                for (int cy = 0; cy < chunksY; cy++)
                {
                    Vector2Int chunk = new Vector2Int(cx, cy);

                    // Check connections to each neighboring chunk
                    int[] dx = { 1, 0, -1, 0 };
                    int[] dy = { 0, 1, 0, -1 };

                    for (int i = 0; i < 4; i++)
                    {
                        int nx = cx + dx[i];
                        int ny = cy + dy[i];

                        if (nx < 0 || nx >= chunksX || ny < 0 || ny >= chunksY)
                            continue;

                        Vector2Int neighbor = new Vector2Int(nx, ny);
                        Vector2Int key = EncodeChunkConnection(chunk, neighbor);

                        // Test if there's a valid path between these chunks
                        bool connected = TestChunkConnection(chunk, neighbor);
                        chunkConnections[key] = connected;
                    }
                }
            }
        }

        private Vector2Int EncodeChunkConnection(Vector2Int a, Vector2Int b)
        {
            // Ensure consistent encoding regardless of order
            if (a.x < b.x || (a.x == b.x && a.y < b.y))
                return new Vector2Int(a.x * 1000000 + a.y * 1000 + b.x * 100 + b.y, 0);
            else
                return new Vector2Int(b.x * 1000000 + b.y * 1000 + a.x * 100 + a.y, 0);
        }

        private bool TestChunkConnection(Vector2Int chunkA, Vector2Int chunkB)
        {
            // Find border cells and test for walkable paths
            // This is a simplified version - in a real implementation,
            // you would test multiple potential crossing points

            int ax = chunkA.x * chunkSize + chunkSize / 2;
            int ay = chunkA.y * chunkSize + chunkSize / 2;
            int bx = chunkB.x * chunkSize + chunkSize / 2;
            int by = chunkB.y * chunkSize + chunkSize / 2;

            // Find a point on the border between chunks
            Vector2Int borderPoint;

            if (chunkA.x == chunkB.x)
            {
                // Chunks are above/below each other
                borderPoint = new Vector2Int(ax, chunkA.y < chunkB.y ?
                                                 (chunkA.y + 1) * chunkSize - 1 :
                                                 chunkA.y * chunkSize);
            }
            else
            {
                // Chunks are beside each other
                borderPoint = new Vector2Int(chunkA.x < chunkB.x ?
                                             (chunkA.x + 1) * chunkSize - 1 :
                                             chunkA.x * chunkSize,
                                             ay);
            }

            // Check if the border point is walkable
            return grid.IsWalkable(borderPoint.x, borderPoint.y);
        }

        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
        {
            // If start and end are in the same chunk or adjacent chunks
            // just use the normal A* pathfinding
            Vector2Int startChunk = new Vector2Int(start.x / chunkSize, start.y / chunkSize);
            Vector2Int endChunk = new Vector2Int(end.x / chunkSize, end.y / chunkSize);

            if (startChunk == endChunk || AreChunksAdjacent(startChunk, endChunk))
            {
                return grid.FindPath(start, end);
            }

            // Otherwise, use hierarchical pathfinding
            // 1. Find path between chunks
            List<Vector2Int> chunkPath = FindChunkPath(startChunk, endChunk);

            if (chunkPath == null || chunkPath.Count == 0)
                return null; // No path found at the chunk level

            // 2. Find detailed paths within and between chunks
            List<Vector2Int> detailedPath = new List<Vector2Int>();

            // Start point to first chunk exit
            Vector2Int firstExit = GetExitPoint(startChunk, chunkPath[0]);
            List<Vector2Int> firstSegment = grid.FindPath(start, firstExit);

            if (firstSegment != null)
                detailedPath.AddRange(firstSegment);

            // Connect through intermediate chunks
            for (int i = 0; i < chunkPath.Count - 1; i++)
            {
                Vector2Int entry = GetExitPoint(chunkPath[i], chunkPath[i + 1]);
                Vector2Int exit = GetExitPoint(chunkPath[i + 1], i + 2 < chunkPath.Count ? chunkPath[i + 2] : endChunk);

                List<Vector2Int> segment = grid.FindPath(entry, exit);

                if (segment != null)
                    detailedPath.AddRange(segment);
            }

            // Final chunk to destination
            Vector2Int lastEntry = GetExitPoint(chunkPath.Last(), endChunk);
            List<Vector2Int> lastSegment = grid.FindPath(lastEntry, end);

            if (lastSegment != null)
                detailedPath.AddRange(lastSegment);

            return detailedPath;
        }

        private bool AreChunksAdjacent(Vector2Int a, Vector2Int b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) == 1;
        }

        private List<Vector2Int> FindChunkPath(Vector2Int startChunk, Vector2Int endChunk)
        {
            // A* implementation at the chunk level
            // Simplified for brevity - this would be similar to the grid A* but working with chunks

            // In a real implementation, this would be a full A* search on the chunk graph
            // For now, we'll return a simplified path
            List<Vector2Int> path = new List<Vector2Int>();

            int dx = endChunk.x - startChunk.x;
            int dy = endChunk.y - startChunk.y;

            Vector2Int current = startChunk;

            // Move horizontally
            for (int i = 0; i < Math.Abs(dx); i++)
            {
                current = new Vector2Int(current.x + Math.Sign(dx), current.y);
                path.Add(current);
            }

            // Move vertically
            for (int i = 0; i < Math.Abs(dy); i++)
            {
                current = new Vector2Int(current.x, current.y + Math.Sign(dy));
                path.Add(current);
            }

            return path;
        }

        private Vector2Int GetExitPoint(Vector2Int fromChunk, Vector2Int toChunk)
        {
            // Find a good exit point between chunks
            int exitX, exitY;

            if (fromChunk.x < toChunk.x)
            {
                // Exit on right edge
                exitX = (fromChunk.x + 1) * chunkSize - 1;
                exitY = fromChunk.y * chunkSize + chunkSize / 2;
            }
            else if (fromChunk.x > toChunk.x)
            {
                // Exit on left edge
                exitX = fromChunk.x * chunkSize;
                exitY = fromChunk.y * chunkSize + chunkSize / 2;
            }
            else if (fromChunk.y < toChunk.y)
            {
                // Exit on top edge
                exitX = fromChunk.x * chunkSize + chunkSize / 2;
                exitY = (fromChunk.y + 1) * chunkSize - 1;
            }
            else
            {
                // Exit on bottom edge
                exitX = fromChunk.x * chunkSize + chunkSize / 2;
                exitY = fromChunk.y * chunkSize;
            }

            // Make sure the exit point is valid
            exitX = Mathf.Clamp(exitX, 0, grid.Width - 1);
            exitY = Mathf.Clamp(exitY, 0, grid.Height - 1);

            // Find a nearby walkable cell if needed
            if (!grid.IsWalkable(exitX, exitY))
            {
                // Search nearby for a walkable cell
                for (int r = 1; r < chunkSize; r++)
                {
                    for (int dx = -r; dx <= r; dx++)
                    {
                        for (int dy = -r; dy <= r; dy++)
                        {
                            if (Math.Abs(dx) + Math.Abs(dy) == r)
                            {
                                int nx = exitX + dx;
                                int ny = exitY + dy;

                                if (grid.IsWalkable(nx, ny))
                                    return new Vector2Int(nx, ny);
                            }
                        }
                    }
                }
            }

            return new Vector2Int(exitX, exitY);
        }
    }

    // Path smoothing to improve the appearance of movement
    public class PathSmoother
    {
        private Grid grid;

        public PathSmoother(Grid grid)
        {
            this.grid = grid;
        }

        public List<Vector2Int> SmoothPath(List<Vector2Int> originalPath)
        {
            if (originalPath == null || originalPath.Count <= 2)
                return originalPath;

            List<Vector2Int> smoothedPath = new List<Vector2Int>();
            smoothedPath.Add(originalPath[0]);

            int current = 0;

            while (current < originalPath.Count - 1)
            {
                int furthestVisible = current + 1;

                // Try to find the furthest visible point
                for (int i = current + 2; i < originalPath.Count; i++)
                {
                    if (HasLineOfSight(originalPath[current], originalPath[i]))
                    {
                        furthestVisible = i;
                    }
                    else
                    {
                        break;
                    }
                }

                // Add the furthest visible point
                smoothedPath.Add(originalPath[furthestVisible]);
                current = furthestVisible;
            }

            return smoothedPath;
        }

        private bool HasLineOfSight(Vector2Int start, Vector2Int end)
        {
            // Check if there's a clear line of sight between two points
            // Implementation of Bresenham's line algorithm
            int x0 = start.x;
            int y0 = start.y;
            int x1 = end.x;
            int y1 = end.y;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (x0 != x1 || y0 != y1)
            {
                if (!grid.IsWalkable(x0, y0))
                    return false;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return true;
        }
    }
}
