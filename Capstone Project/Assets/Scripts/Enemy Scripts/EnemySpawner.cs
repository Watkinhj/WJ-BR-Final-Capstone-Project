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
            Vector3Int randomCell = GetRandomWalkableCell();
            Vector3 spawnPos = walkableTilemap.CellToWorld(randomCell) + new Vector3(0.5f, 0.5f, 0f); // Center of the cell

            int randomEnemyIndex = Random.Range(0, Enemies.Count); // Randomly select an index from 0 to the count of enemies
            GameObject enemyPrefab = Enemies[randomEnemyIndex]; // Get the enemy GameObject corresponding to the random index

            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            currentEnemyCount++;
        }

        yield return new WaitForSeconds(spawnRate);
        StartCoroutine(SpawnEnemy());
    }

    Vector3Int GetRandomWalkableCell()
    {
        Vector3Int randomCell = new Vector3Int(Random.Range(walkableBounds.xMin, walkableBounds.xMax),
                                                Random.Range(walkableBounds.yMin, walkableBounds.yMax),
                                                0);

        TileBase tile = walkableTilemap.GetTile(randomCell);
        while (tile == null) // Keep trying until a walkable tile is found
        {
            randomCell = new Vector3Int(Random.Range(walkableBounds.xMin, walkableBounds.xMax),
                                        Random.Range(walkableBounds.yMin, walkableBounds.yMax),
                                        0);
            tile = walkableTilemap.GetTile(randomCell);
        }

        return randomCell;
    }
}
