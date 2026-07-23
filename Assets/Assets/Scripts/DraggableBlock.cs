using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int[,] shape;
    public float cellSize = 80f;

    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void SetShape(int[,] newShape)
    {
        shape = newShape;
        DrawShape();
    }

    void DrawShape()
    {
        int width = shape.GetLength(0);
        int height = shape.GetLength(1);

        // Size the container
        rect.sizeDelta = new Vector2(width * cellSize, height * cellSize);

        // Clear old children
        foreach (Transform child in transform) Destroy(child.gameObject);

        // Draw cells
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (shape[x, y] == 1)
                {
                    GameObject cell = new GameObject("BlockCell");
                    cell.transform.SetParent(transform, false);
                    RectTransform rt = cell.AddComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(cellSize - 6, cellSize - 6);

                    float posX = (x * cellSize) - (width * cellSize / 2) + (cellSize / 2);
                    float posY = -(y * cellSize) + (height * cellSize / 2) - (cellSize / 2);
                    rt.anchoredPosition = new Vector2(posX, posY);

                    Image img = cell.AddComponent<Image>();
                    img.color = GridManager.Instance.blockColor;
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rect.anchoredPosition;
        originalParent = transform.parent;
        transform.SetParent(GameObject.Find("Canvas").transform); // Bring to front
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // Try to place on grid
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GridManager.Instance.gridBackground,
            Input.mousePosition,
            null,
            out localPoint);

        // Calculate which grid cell the center of the block is over
        float totalSize = GridManager.Instance.cellSize * 8;
        float startX = -totalSize / 2;
        float startY = totalSize / 2;

        int gridX = Mathf.FloorToInt((localPoint.x - startX) / GridManager.Instance.cellSize);
        int gridY = Mathf.FloorToInt((startY - localPoint.y) / GridManager.Instance.cellSize);

        // Offset by shape half size so it snaps from center visually
        int width = shape.GetLength(0);
        int height = shape.GetLength(1);
        Vector2Int targetGridPos = new Vector2Int(gridX - (width / 2), gridY - (height / 2));

        if (GridManager.Instance.CanPlaceBlock(shape, targetGridPos))
        {
            GridManager.Instance.PlaceBlock(shape, targetGridPos);
            Destroy(gameObject); // Remove from tray
            BlockSpawner.Instance.BlockPlaced();
        }
        else
        {
            // Return to tray
            transform.SetParent(originalParent);
            rect.anchoredPosition = originalPosition;
        }
    }
}