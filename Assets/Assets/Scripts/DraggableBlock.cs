using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int[,] shape;
    public Sprite blockSprite;
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

    public void SetShape(int[,] newShape, Sprite sprite)
    {
        shape = newShape;
        blockSprite = sprite;
        DrawShape();
    }

    void DrawShape()
    {
        int width = shape.GetLength(0);
        int height = shape.GetLength(1);

        rect.sizeDelta = new Vector2(width * cellSize, height * cellSize);

        foreach (Transform child in transform) Destroy(child.gameObject);

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
                    img.sprite = blockSprite;
                    img.color = Color.white;
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

        // Calculate grid position while dragging for the Ghost Preview
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GridManager.Instance.gridBackground,
            Input.mousePosition,
            null,
            out localPoint);

        int width = shape.GetLength(0);
        int height = shape.GetLength(1);

        Vector2Int targetGridPos = GridManager.Instance.GetGridPosition(localPoint, width, height);

        bool canPlace = GridManager.Instance.CanPlaceBlock(shape, targetGridPos);
        if (canPlace)
        {
            GridManager.Instance.UpdateGhost(shape, targetGridPos, true, blockSprite);
        }
        else
        {
            GridManager.Instance.ClearGhost();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GridManager.Instance.gridBackground,
            Input.mousePosition,
            null,
            out localPoint);

        int width = shape.GetLength(0);
        int height = shape.GetLength(1);
        Vector2Int targetGridPos = GridManager.Instance.GetGridPosition(localPoint, width, height);

        if (GridManager.Instance.CanPlaceBlock(shape, targetGridPos))
        {
            GridManager.Instance.PlaceBlock(shape, targetGridPos, blockSprite);
            GridManager.Instance.ClearGhost();
            BlockSpawner.Instance.BlockPlaced(this);
            Destroy(gameObject); // Remove from tray
        }
        else
        {
            GridManager.Instance.ClearGhost();
            transform.SetParent(originalParent);
            rect.anchoredPosition = originalPosition;
        }
    }
}