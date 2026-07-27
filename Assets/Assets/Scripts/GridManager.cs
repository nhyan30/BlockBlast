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

    public int[,] gridArray;
    public Image[,] cellVisuals;
    public Image[,] ghostVisuals;

    public Sprite emptyCellSprite;
    public Color emptyColor;

    private List<Vector2Int> currentGhostCells = new List<Vector2Int>();

    void Awake()
    {
        Instance = this;
        gridArray = new int[gridSize, gridSize];
        cellVisuals = new Image[gridSize, gridSize];
        ghostVisuals = new Image[gridSize, gridSize];
        CreateGridVisuals();
    }

    public void ClearGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                gridArray[x, y] = 0;
                cellVisuals[x, y].sprite = emptyCellSprite;
                cellVisuals[x, y].color = emptyColor;
                cellVisuals[x, y].rectTransform.sizeDelta = new Vector2(cellSize - 4, cellSize - 4);
                ghostVisuals[x, y].enabled = false;
            }
        }
        currentGhostCells.Clear();
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
                rt.sizeDelta = new Vector2(cellSize - 4, cellSize - 4);
                rt.anchoredPosition = new Vector2(startX + (x * cellSize), startY - (y * cellSize));

                Image img = cell.AddComponent<Image>();
                img.sprite = emptyCellSprite;
                img.color = emptyColor;
                img.type = Image.Type.Sliced;
                img.pixelsPerUnitMultiplier = 35;
                cellVisuals[x, y] = img;

                GameObject ghostCell = new GameObject($"Ghost_{x}_{y}");
                ghostCell.transform.SetParent(gridBackground, false);
                RectTransform ghostRt = ghostCell.AddComponent<RectTransform>();
                ghostRt.sizeDelta = new Vector2(cellSize + 3, cellSize + 3);
                ghostRt.anchoredPosition = new Vector2(startX + (x * cellSize), startY - (y * cellSize));

                Image ghostImg = ghostCell.AddComponent<Image>();
                ghostImg.enabled = false;
                ghostVisuals[x, y] = ghostImg;
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
                        return false;

                    if (gridArray[targetX, targetY] == 1)
                        return false;
                }
            }
        }
        return true;
    }

    public void PlaceBlock(int[,] shape, Vector2Int gridPos, Sprite blockSprite)
    {
        int width = shape.GetLength(0);
        int height = shape.GetLength(1);
        int cellsPlaced = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (shape[x, y] == 1)
                {
                    int targetX = gridPos.x + x;
                    int targetY = gridPos.y + y;

                    gridArray[targetX, targetY] = 1;
                    cellVisuals[targetX, targetY].sprite = blockSprite;
                    cellVisuals[targetX, targetY].color = Color.white;
                    cellVisuals[targetX, targetY].rectTransform.sizeDelta = new Vector2(cellSize + 3, cellSize + 3);

                    cellsPlaced++;
                }
            }
        }

        GameManager.Instance.AddScore(cellsPlaced);

        if (AudioManager.Instance != null) AudioManager.Instance.Play(SoundType.Place);

        ClearGhost();
        StartCoroutine(ClearLines());
    }

    IEnumerator ClearLines()
    {
        List<int> rowsToClear = new List<int>();
        List<int> colsToClear = new List<int>();

        for (int y = 0; y < gridSize; y++)
        {
            bool isFull = true;
            for (int x = 0; x < gridSize; x++)
            {
                if (gridArray[x, y] == 0) { isFull = false; break; }
            }
            if (isFull) rowsToClear.Add(y);
        }

        for (int x = 0; x < gridSize; x++)
        {
            bool isFull = true;
            for (int y = 0; y < gridSize; y++)
            {
                if (gridArray[x, y] == 0) { isFull = false; break; }
            }
            if (isFull) colsToClear.Add(x);
        }

        if (rowsToClear.Count > 0 || colsToClear.Count > 0)
        {
            int linesCleared = rowsToClear.Count + colsToClear.Count;
            int scoreToAdd = (10 * linesCleared) + (linesCleared > 1 ? (5 * linesCleared * (linesCleared - 1)) : 0);

            GameManager.Instance.AddScore(scoreToAdd);

            if (UIManager.Instance != null) UIManager.Instance.ShowComboText(linesCleared);
            if (AudioManager.Instance != null) AudioManager.Instance.Play(SoundType.Clear);

            // Flash effect before clearing
            foreach (int y in rowsToClear)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    cellVisuals[x, y].color = Color.white;
                }
            }
            foreach (int x in colsToClear)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    cellVisuals[x, y].color = Color.white;
                }
            }

            yield return new WaitForSeconds(0.1f); // Let the flash show

            // Actually clear visual and logic
            foreach (int y in rowsToClear)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    gridArray[x, y] = 0;
                    cellVisuals[x, y].sprite = emptyCellSprite;
                    cellVisuals[x, y].color = emptyColor;
                    cellVisuals[x, y].rectTransform.sizeDelta = new Vector2(cellSize - 4, cellSize - 4);
                }
            }

            foreach (int x in colsToClear)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    gridArray[x, y] = 0;
                    cellVisuals[x, y].sprite = emptyCellSprite;
                    cellVisuals[x, y].color = emptyColor;
                    cellVisuals[x, y].rectTransform.sizeDelta = new Vector2(cellSize - 4, cellSize - 4);
                }
            }
        }

        BlockSpawner.Instance.CheckGameOver();
    }

    public void UpdateGhost(int[,] shape, Vector2Int gridPos, bool canPlace, Sprite blockSprite)
    {
        ClearGhost();
        if (!canPlace) return;

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

                    if (targetX >= 0 && targetX < gridSize && targetY >= 0 && targetY < gridSize)
                    {
                        ghostVisuals[targetX, targetY].enabled = true;
                        ghostVisuals[targetX, targetY].sprite = blockSprite;
                        ghostVisuals[targetX, targetY].color = new Color(1, 1, 1, 0.5f);
                        currentGhostCells.Add(new Vector2Int(targetX, targetY));
                    }
                }
            }
        }
    }

    public void ClearGhost()
    {
        foreach (Vector2Int cell in currentGhostCells)
        {
            if (cell.x >= 0 && cell.x < gridSize && cell.y >= 0 && cell.y < gridSize)
            {
                ghostVisuals[cell.x, cell.y].enabled = false;
            }
        }
        currentGhostCells.Clear();
    }

    public Vector2Int GetGridPosition(Vector2 localPoint, int width, int height)
    {
        float totalSize = cellSize * gridSize;
        float startX = -totalSize / 2;
        float startY = totalSize / 2;

        int gridX = Mathf.FloorToInt((localPoint.x - startX) / cellSize);
        int gridY = Mathf.FloorToInt((startY - localPoint.y) / cellSize);

        return new Vector2Int(gridX - (width / 2), gridY - (height / 2));
    }
}