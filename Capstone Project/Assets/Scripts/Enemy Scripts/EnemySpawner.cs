using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> Enemies = new List<GameObject>();
    public float spawnRate;
    public Tilemap walkableTilemap;
    public int maxEnemies = 10;
    public GameObject player;
    public float spawnRadius = 5f;

    private BoundsInt walkableBounds;
    private int currentEnemyCount = 0;
    //public LineRenderer lineRenderer; Debug tool
    private int maxSpawnAttempts = 10;


    void Start()
    {
        walkableBounds = walkableTilemap.cellBounds;
        StartCoroutine(DelayInitialSpawn(1f));
        FindObjectOfType<TimerController>().OnHourChanged += HandleHourlyUpdate;
    }

    void HandleHourlyUpdate(int hour)
    {
        maxEnemies += 2;
        spawnRate = Mathf.Max(spawnRate - 1f, 0.1f); // Ensures spawn rate does not go below 0.1
    }

    IEnumerator DelayInitialSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            if (currentEnemyCount < maxEnemies)
            {
                Vector3? spawnPos = GetRandomSpawnPositionAroundPlayer();
                if (spawnPos.HasValue)
                {
                    GameObject enemyPrefab = Enemies[Random.Range(0, Enemies.Count)];
                    Instantiate(enemyPrefab, spawnPos.Value, Quaternion.identity);
                    currentEnemyCount++;
                }
            }
            yield return new WaitForSeconds(spawnRate);
        }
    }

    Vector3? GetRandomSpawnPositionAroundPlayer()
    {
        for (int attempts = 0; attempts < maxSpawnAttempts; attempts++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 potentialSpawnPos = player.transform.position + new Vector3(randomDirection.x, randomDirection.y, 0);
            Vector3Int cellPos = walkableTilemap.WorldToCell(potentialSpawnPos);

            if (walkableTilemap.GetTile(cellPos) != null) // Check if the tile is walkable
            {
                return walkableTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0); // Center of the tile
            }
        }
        return null; // No valid position found
    }

    Vector3Int FindNearestWalkableCell(Vector3Int startCell)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        queue.Enqueue(startCell);
        visited.Add(startCell);

        while (queue.Count > 0)
        {
            Vector3Int currentCell = queue.Dequeue();

            if (walkableTilemap.GetTile(currentCell) != null)
                return currentCell;

            // Explore neighbors within bounds
            foreach (Vector3Int neighbor in GetNeighbors(currentCell))
            {
                if (!visited.Contains(neighbor) && IsWithinBounds(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        // If no walkable cell is found, return the original startCell
        return startCell;
    }

    List<Vector3Int> GetNeighbors(Vector3Int cell)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        neighbors.Add(cell + new Vector3Int(1, 0, 0));
        neighbors.Add(cell + new Vector3Int(-1, 0, 0));
        neighbors.Add(cell + new Vector3Int(0, 1, 0));
        neighbors.Add(cell + new Vector3Int(0, -1, 0));

        return neighbors;
    }

    bool IsWithinBounds(Vector3Int cell)
    {
        return walkableBounds.xMin <= cell.x && cell.x <= walkableBounds.xMax &&
               walkableBounds.yMin <= cell.y && cell.y <= walkableBounds.yMax;
    }
}
