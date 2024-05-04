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

    void Start()
    {
        walkableBounds = walkableTilemap.cellBounds;
        StartCoroutine(DelayInitialSpawn(1f));
        FindObjectOfType<TimerController>().OnHourChanged += HandleHourlyUpdate; // Subscribe to the hour change event
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
        while (true) // Changed to loop continuously
        {
            if (currentEnemyCount < maxEnemies)
            {
                Vector3 spawnPos = GetRandomSpawnPositionAroundPlayer();
                GameObject enemyPrefab = Enemies[Random.Range(0, Enemies.Count)];
                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                currentEnemyCount++;
            }
            yield return new WaitForSeconds(spawnRate);
        }
    }

    Vector3 GetRandomSpawnPositionAroundPlayer()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = player.transform.position + new Vector3(randomDirection.x, randomDirection.y, 0);
        Vector3Int cellPos = walkableTilemap.WorldToCell(spawnPos);

        if (walkableTilemap.GetTile(cellPos) == null) // If the cell is not walkable, find the nearest walkable cell
        {
            cellPos = FindNearestWalkableCell(cellPos);
            spawnPos = walkableTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f); // Center of the cell
        }

        return spawnPos;
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

            // Explore neighbors
            foreach (Vector3Int neighbor in GetNeighbors(currentCell))
            {
                if (!visited.Contains(neighbor))
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
}
