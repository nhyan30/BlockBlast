using UnityEngine;
using System.Collections.Generic;

public class BlockSpawner : MonoBehaviour
{
    public static BlockSpawner Instance;
    public BlockShapes shapeDatabase;

    public GameObject blockPrefab;
    public Transform[] spawnPoints;

    private List<DraggableBlock> activeBlocks = new List<DraggableBlock>();

    void Awake() => Instance = this;
    void Start() => SpawnNewBlocks();

    public void SpawnNewBlocks()
    {
        // Clear any old references just in case
        activeBlocks.Clear();

        SpawnBlockAt(spawnPoints[0]);
        SpawnBlockAt(spawnPoints[1]);
        SpawnBlockAt(spawnPoints[2]);

        CheckGameOver();
    }

    void SpawnBlockAt(Transform parent)
    {
        int[,] shape = shapeDatabase.shapes[Random.Range(0, shapeDatabase.shapes.Count)];

        // Pick a random image from GameManager
        Sprite sprite = GameManager.Instance.blockSprites[Random.Range(0, GameManager.Instance.blockSprites.Length)];

        GameObject blockObj = Instantiate(blockPrefab, parent);
        DraggableBlock drag = blockObj.GetComponent<DraggableBlock>();
        drag.SetShape(shape, sprite);

        activeBlocks.Add(drag);
    }

    public void BlockPlaced(DraggableBlock placedBlock)
    {
        if (activeBlocks.Contains(placedBlock))
        {
            activeBlocks.Remove(placedBlock);
        }

        if (activeBlocks.Count <= 0)
        {
            SpawnNewBlocks();
        }
        else
        {
            CheckGameOver();
        }
    }

    public void CheckGameOver()
    {
        // Don't check if we are spawning a new set
        if (activeBlocks.Count == 0) return;

        bool canPlaceAny = false;

        // Test every active block against every cell on the 8x8 grid
        for (int x = 0; x < GridManager.Instance.gridSize; x++)
        {
            for (int y = 0; y < GridManager.Instance.gridSize; y++)
            {
                foreach (DraggableBlock block in activeBlocks)
                {
                    if (GridManager.Instance.CanPlaceBlock(block.shape, new Vector2Int(x, y)))
                    {
                        canPlaceAny = true;
                        break; // Found a valid move, no need to check this cell further
                    }
                }
                if (canPlaceAny) break; // Break Y loop
            }
            if (canPlaceAny) break; // Break X loop
        }

        if (!canPlaceAny)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }
}