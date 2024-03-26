using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemBoxSpawner : MonoBehaviour
{
    public Tilemap walkableTilemap; // Reference to your walkable tilemap
    public GameObject itemBoxPrefab; // Prefab of your item box
    public int numberOfBoxesToSpawn = 10; // Number of item boxes to spawn

    // Start is called before the first frame update
    void Start()
    {
        SpawnItemBoxes();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnItemBoxes()
    {
        if (walkableTilemap == null || itemBoxPrefab == null)
        {
            Debug.LogError("Walkable tilemap or item box prefab not assigned!");
            return;
        }

        BoundsInt bounds = walkableTilemap.cellBounds;
        Vector3Int minBound = bounds.min;
        Vector3Int maxBound = bounds.max;

        List<Vector3Int> availablePositions = new List<Vector3Int>();

        // Loop through each cell in the tilemap and check if it's walkable
        for (int x = minBound.x; x < maxBound.x; x++)
        {
            for (int y = minBound.y; y < maxBound.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (walkableTilemap.HasTile(tilePosition))
                {
                    availablePositions.Add(tilePosition);
                }
            }
        }

        // Spawn item boxes at random positions from the list of available positions
        int boxesSpawned = 0;
        while (boxesSpawned < numberOfBoxesToSpawn && availablePositions.Count > 0)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector3Int randomPosition = availablePositions[randomIndex];
            Vector3 spawnPosition = walkableTilemap.CellToWorld(randomPosition) + new Vector3(0.5f, 0.5f, 0f); // Offset to center of tile
            Instantiate(itemBoxPrefab, spawnPosition, Quaternion.identity);
            availablePositions.RemoveAt(randomIndex);
            boxesSpawned++;
        }
    }
}