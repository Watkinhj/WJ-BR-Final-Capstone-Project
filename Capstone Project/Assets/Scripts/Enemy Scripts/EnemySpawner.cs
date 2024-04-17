using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> Enemies = new List<GameObject>();
    public float spawnRate;
    public Tilemap walkableTilemap; // Assign your walkable tilemap here
    public int maxEnemies = 10; // Maximum number of enemies allowed on the map
    public GameObject player; // Reference to the player GameObject
    public float spawnRadius = 5f; // Radius within which enemies can spawn around the player

    private BoundsInt walkableBounds;
    private int currentEnemyCount = 0;

    private void Start()
    {
        walkableBounds = walkableTilemap.cellBounds;
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        if (currentEnemyCount < maxEnemies)
        {
            Vector3 spawnPos = GetRandomSpawnPositionAroundPlayer();

            int randomEnemyIndex = Random.Range(0, Enemies.Count); // Randomly select an index from 0 to the count of enemies
            GameObject enemyPrefab = Enemies[randomEnemyIndex]; // Get the enemy GameObject corresponding to the random index

            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            currentEnemyCount++;
        }

        yield return new WaitForSeconds(spawnRate);
        StartCoroutine(SpawnEnemy());
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
