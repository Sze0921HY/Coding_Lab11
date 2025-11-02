using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Pathfinding : MonoBehaviour
{
    private List<Vector2Int> path = new List<Vector2Int>();
    private Vector2Int start = new Vector2Int(0, 1);
    private Vector2Int goal = new Vector2Int(4, 4);
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    private int[,] grid;
    private int[,] terrainCost;

    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    [Range(0f, 1f)] public float obstacleProbability = 0.2f;

    private bool selectingStart = true;

    private void Start()
    {
        GenerateRandomGrid(width, height, obstacleProbability);
        FindPath(start, goal);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2Int clickedCell = new Vector2Int(
                    Mathf.FloorToInt(hit.point.x),
                    Mathf.FloorToInt(hit.point.z)
                );

                if (!IsInBounds(clickedCell)) return;
                if (grid[clickedCell.y, clickedCell.x] == 1) return;

                if (selectingStart)
                {
                    start = clickedCell;
                    Debug.Log("Start set to: " + start);
                }
                else
                {
                    goal = clickedCell;
                    Debug.Log("Goal set to: " + goal);
                    FindPath(start, goal);
                }

                selectingStart = !selectingStart;
            }
        }
        if (Mouse.current.rightButton.wasPressedThisFrame) 
        {
            AddObstacle(new Vector2Int(Random.Range(0, width), Random.Range(0, height)));

        }
    }

    public void GenerateRandomGrid(int width, int height, float obstacleProbability)
    {
        grid = new int[height, width];
        terrainCost = new int[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[y, x] = (Random.value < obstacleProbability) ? 1 : 0;
                terrainCost[y, x] = (grid[y, x] == 1) ? int.MaxValue : Random.Range(1, 6);
            }
        }

        grid[start.y, start.x] = 0;
        grid[goal.y, goal.x] = 0;
        terrainCost[start.y, start.x] = 1;
        terrainCost[goal.y, goal.x] = 1;
    }

    public void AddObstacle(Vector2Int position)
    {
        if (IsInBounds(position))
        {
            grid[position.y, position.x] = 1;
            Debug.Log("Obstacle added at " + position);
        }
        else
        {
            Debug.LogWarning("Position " + position + " is out of grid bounds");
        }
    }

    private void OnDrawGizmos()
    {
        if (grid == null) return;

        float cellSize = 1f;
        // Draw grid cells
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, 0, y * cellSize);
                Gizmos.color = grid[y, x] == 1 ? Color.black : Color.white;
                Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f,
                cellSize));
            }
        }
        // Draw path
        foreach (var step in path)
        {
            Vector3 cellPosition = new Vector3(step.x, 0, step.y);
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(cellPosition, new Vector3(1f, 0.1f, 1f));
        }
        // Draw start and goal
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(start.x, 0, start.y), new
        Vector3(1f, 0.1f, 1f));
        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(goal.x, 0, goal.y), new
        Vector3(1f, 0.1f, 1f));
    }

    private bool IsInBounds(Vector2Int point)
    {
        return point.x >= 0 && point.x < grid.GetLength(1) && point.y >= 0 &&
        point.y < grid.GetLength(0);
    }

    private void FindPath(Vector2Int start, Vector2Int goal)
    {
        path.Clear();

        var frontier = new PriorityQueue<Vector2Int>();
        frontier.Enqueue(start, 0);
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var costSoFar = new Dictionary<Vector2Int, float>();

        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            
            if (current == goal)
            {
                break;
            }

            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current + direction;
                if (!IsInBounds(next) || grid[next.y, next.x] == 1) continue;
                int moveCost = terrainCost[next.y, next.x];
                float newCost = costSoFar[current] + moveCost;

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost + Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }
        
        if (!cameFrom.ContainsKey(goal))
        {
            Debug.Log("Path not found.");
            return;
        }
        
        // Trace path from goal to start
        Vector2Int step = goal;
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Add(start);
        path.Reverse();

        float totalCost;
        if (costSoFar.ContainsKey(goal))
        {
            totalCost = costSoFar[goal];
        }
        else
        {
            totalCost = path.Count - 1;
        }

        Debug.Log("Path found! Total cost: " + totalCost + " Total steps: " + (path.Count - 1));
    }

    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public class PriorityQueue<T>
    {
        private List<(T item, float priority)> elements = new List<(T, float)>();

        public int Count => elements.Count;

        public void Enqueue(T item, float priority)
        {
            elements.Add((item, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;
            float bestPriority = elements[0].priority;

            for (int i = 1; i < elements.Count; i++)
            {
                if (elements[i].priority < bestPriority)
                {
                    bestPriority = elements[i].priority;
                    bestIndex = i;
                }
            }

            T bestItem = elements[bestIndex].item;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }

}