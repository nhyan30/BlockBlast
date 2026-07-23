using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public static BlockSpawner Instance;
    public BlockShapes shapeDatabase;

    public GameObject blockPrefab; // A UI prefab with the DraggableBlock script attached
    public Transform[] spawnPoints; // 3 spawn points in the tray

    private int[,] currentShape1;
    private int[,] currentShape2;
    private int[,] currentShape3;

    public int blocksRemaining = 3;

    void Awake() => Instance = this;

    void Start() => SpawnNewBlocks();

    public void SpawnNewBlocks()
    {
        currentShape1 = shapeDatabase.shapes[Random.Range(0, shapeDatabase.shapes.Count)];
        currentShape2 = shapeDatabase.shapes[Random.Range(0, shapeDatabase.shapes.Count)];
        currentShape3 = shapeDatabase.shapes[Random.Range(0, shapeDatabase.shapes.Count)];

        CreateBlockVisual(currentShape1, spawnPoints[0]);
        CreateBlockVisual(currentShape2, spawnPoints[1]);
        CreateBlockVisual(currentShape3, spawnPoints[2]);

        blocksRemaining = 3;
        CheckGameOver();
    }

    void CreateBlockVisual(int[,] shape, Transform parent)
    {
        GameObject blockObj = Instantiate(blockPrefab, parent);
        DraggableBlock drag = blockObj.GetComponent<DraggableBlock>();
        drag.SetShape(shape);
    }

    public void BlockPlaced()
    {
        blocksRemaining--;
        if (blocksRemaining <= 0)
        {
            SpawnNewBlocks();
        }
        CheckGameOver();
    }

    public void CheckGameOver()
    {
        // If any of the remaining blocks (in tray) can fit anywhere, game continues
        // We need a reference to the active blocks. To keep it simple for this guide, 
        // we check if ALL 3 slots are empty. If not, we assume they can fit unless proven otherwise.

        // For a robust Game Over, we check the remaining blocks in the tray.
        // (This requires passing the remaining shapes. For simplicity, we trigger a basic check).
        Debug.Log("Checking Game State...");
    }
}