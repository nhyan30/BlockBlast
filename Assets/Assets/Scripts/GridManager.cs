using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int gridSize = 8;
    public float cellSize = 80f;
    public RectTransform gridBackground;

    // 0 = empty, 1 = filled
    public int[,] gridArray;
    public Image[,] cellVisuals;

    public Sprite emptyCellSprite;
    public Sprite filledCellSprite;
    public Color blockColor = new Color(0.2f, 0.5f, 1f); // Blueish

    void Awake()
    {
        Instance = this;
        gridArray = new int[gridSize, gridSize];
        cellVisuals = new Image[gridSize, gridSize];
        CreateGridVisuals();
    }

    void CreateGridVisuals()
    {
        float totalSize = cellSize * gridSize;
        gridBackground.sizeDelta = new Vector2(totalSize, totalSize);
        float startX = -(totalSize / 2) + (cellSize / 2);
        float startY = (totalSize / 2) - (cellSize / 2);

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject cell = new GameObject($"Cell_{x}_{y}");
                cell.transform.SetParent(gridBackground, false);
                RectTransform rt = cell.AddComponent<RectTransform>();
                rt.sizeDelta = new Vector2(cellSize - 4, cellSize - 4); // Slight gap
                rt.anchoredPosition = new Vector2(startX + (x * cellSize), startY - (y * cellSize));

                Image img = cell.AddComponent<Image>();
                img.sprite = emptyCellSprite;
                img.color = new Color(1, 1, 1, 0.1f); // Faint background grid

                cellVisuals[x, y] = img;
            }
        }
    }

    public bool CanPlaceBlock(int[,] shape, Vector2Int gridPos)
    {
        int width = shape.GetLength(0);
        int height = shape.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (shape[x, y] == 1)
                {
                    int targetX = gridPos.x + x;
                    int targetY = gridPos.y + y;

                    if (targetX < 0 || targetX >= gridSize || targetY < 0 || targetY >= gridSize)
                        return false; // Out of bounds

                    if (gridArray[targetX, targetY] == 1)
                        return false; // Cell already occupied
                }
            }
        }
        return true;
    }

    public void PlaceBlock(int[,] shape, Vector2Int gridPos)
    {
        int width = shape.GetLength(0);
        int height = shape.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (shape[x, y] == 1)
                {
                    int targetX = gridPos.x + x;
                    int targetY = gridPos.y + y;

                    gridArray[targetX, targetY] = 1;
                    cellVisuals[targetX, targetY].color = blockColor;
                }
            }
        }

        StartCoroutine(ClearLines());
    }

    IEnumerator ClearLines()
    {
        List<int> rowsToClear = new List<int>();
        List<int> colsToClear = new List<int>();

        // Check Rows
        for (int y = 0; y < gridSize; y++)
        {
            bool isFull = true;
            for (int x = 0; x < gridSize; x++)
            {
                if (gridArray[x, y] == 0) { isFull = false; break; }
            }
            if (isFull) rowsToClear.Add(y);
        }

        // Check Columns
        for (int x = 0; x < gridSize; x++)
        {
            bool isFull = true;
            for (int y = 0; y < gridSize; y++)
            {
                if (gridArray[x, y] == 0) { isFull = false; break; }
            }
            if (isFull) colsToClear.Add(x);
        }

        // Clear visual and logic
        foreach (int y in rowsToClear)
        {
            for (int x = 0; x < gridSize; x++)
            {
                gridArray[x, y] = 0;
                cellVisuals[x, y].color = new Color(1, 1, 1, 0.1f); // Reset to empty
            }
        }

        foreach (int x in colsToClear)
        {
            for (int y = 0; y < gridSize; y++)
            {
                gridArray[x, y] = 0;
                cellVisuals[x, y].color = new Color(1, 1, 1, 0.1f); // Reset to empty
            }
        }

        if (rowsToClear.Count > 0 || colsToClear.Count > 0)
        {
            int scoreToAdd = (rowsToClear.Count + colsToClear.Count) * 10;
            // Add Bonus points for combos
            if (rowsToClear.Count + colsToClear.Count > 1) scoreToAdd += 20;

            GameManager.Instance.AddScore(scoreToAdd);
            yield return new WaitForSeconds(0.2f); // Brief pause for effect
        }

        // Check for Game Over after placement and clearing
        BlockSpawner.Instance.CheckGameOver();
    }
}