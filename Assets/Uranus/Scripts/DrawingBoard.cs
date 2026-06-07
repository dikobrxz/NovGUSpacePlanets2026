using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DrawingBoard : MonoBehaviour
{
    [Header("Drawing Settings")]
    public RenderTexture drawingTexture;
    public int brushSize = 30;
    public Color brushColor = new Color(1f, 0f, 0f, 1f);

    [Header("Brush Detection")]
    public Transform brushTip;
    public float brushDistance = 0.1f;

    [Header("References")]
    public Collider canvasCollider;

    private Texture2D texture2D;
    private Vector2 textureSize;
    private int canvasLayerMask;
    private XRGrabInteractable grabInteractable;
    private Vector3 colliderSize;
    private Vector3 colliderCenter;
    private bool isDrawing = false;
    private Vector2 lastUV;

    void Start()
    {
        if (drawingTexture == null)
        {
            Debug.LogError("[DrawingBoard] DrawingTexture не назначен!");
            return;
        }

        if (brushTip == null)
        {
            Debug.LogError("[DrawingBoard] BrushTip не назначен!");
            return;
        }

        if (canvasCollider == null)
        {
            Debug.LogError("[DrawingBoard] CanvasCollider не назначен!");
            return;
        }

        textureSize = new Vector2(drawingTexture.width, drawingTexture.height);
        texture2D = new Texture2D(drawingTexture.width, drawingTexture.height);

        canvasLayerMask = LayerMask.GetMask("Canvas");

        BoxCollider boxCollider = canvasCollider as BoxCollider;
        if (boxCollider != null)
        {
            colliderSize = boxCollider.size;
            colliderCenter = boxCollider.center;
        }
        else
        {
            colliderSize = canvasCollider.bounds.size;
            colliderCenter = canvasCollider.bounds.center - canvasCollider.transform.position;
        }

        if (brushTip.GetComponentInParent<XRGrabInteractable>() != null)
        {
            grabInteractable = brushTip.GetComponentInParent<XRGrabInteractable>();
        }

        Clear();
        Debug.Log("[DrawingBoard] Инициализация завершена");
    }

    void Update()
    {
        if (brushTip == null || canvasCollider == null) return;

        Ray ray = new Ray(brushTip.position, brushTip.forward);

        bool isHit = Physics.Raycast(ray, out RaycastHit hit, brushDistance, canvasLayerMask);
        bool isOnCanvas = isHit && hit.collider == canvasCollider;
        bool isHeld = grabInteractable != null ? grabInteractable.isSelected : true;

        if (isOnCanvas && isHeld)
        {
            Vector3 localPoint = canvasCollider.transform.InverseTransformPoint(hit.point);
            Debug.Log("LocalPoint "+ localPoint);
            float u = (localPoint.x - colliderCenter.x + colliderSize.x / 2f) / colliderSize.x;
            float v = (localPoint.y - colliderCenter.y + colliderSize.y / 2f) / colliderSize.y;
            v = 1f - v;
            Vector2 currentUV = new Vector2(u, v);


            if (currentUV.x < 0 || currentUV.x > 1 || currentUV.y < 0 || currentUV.y > 1)
            {
                if (isDrawing) FinishDrawing();
                return;
            }

            if (!isDrawing)
            {
                isDrawing = true;
                lastUV = currentUV;
                DrawPointAtUV(currentUV);
            }
            else
            {
                Debug.Log("isDrawing " + isDrawing);
                if (Vector2.Distance(lastUV, currentUV) > 0.001f)
                {
                    Debug.Log("Начинаем рисовать");
                    DrawLine(lastUV, currentUV);
                    lastUV = currentUV;
                }
            }
        }
        else
        {
            if (isDrawing) FinishDrawing();
        }
    }

    private void FinishDrawing()
    {
        isDrawing = false;
        ApplyTexture();
    }

    private void DrawLine(Vector2 fromUV, Vector2 toUV)
    {
        Vector2 fromPx = new Vector2(fromUV.x * textureSize.x, fromUV.y * textureSize.y);
        Vector2 toPx = new Vector2(toUV.x * textureSize.x, toUV.y * textureSize.y);

        float distance = Vector2.Distance(fromPx, toPx);
        int steps = Mathf.Max(2, Mathf.RoundToInt(distance / 1.5f));

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 point = Vector2.Lerp(fromPx, toPx, t);
            DrawPointAtPixel(point);
            Debug.Log("Рисуем");
        }
        
    }

    private void DrawPointAtUV(Vector2 uv)
    {
        Vector2 pixel = new Vector2(uv.x * textureSize.x, uv.y * textureSize.y);
        DrawPointAtPixel(pixel);
    }

    private void DrawPointAtPixel(Vector2 pixel)
    {
        int x = Mathf.RoundToInt(pixel.x);
        int y = Mathf.RoundToInt(pixel.y);

        if (x < 0 || x >= drawingTexture.width || y < 0 || y >= drawingTexture.height) return;

        int radius = brushSize / 2;
        float radiusSq = radius * radius;

        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                if (i * i + j * j > radiusSq) continue;

                int px = x + i;
                int py = y + j;

                if (px >= 0 && px < drawingTexture.width && py >= 0 && py < drawingTexture.height)
                {
                    float falloff = 1f - (i * i + j * j) / radiusSq;
                    Color currentColor = texture2D.GetPixel(px, py);
                    Color blended = Color.Lerp(currentColor, brushColor, falloff * 0.9f);
                    texture2D.SetPixel(px, py, blended);
                }
            }
        }
    }

    private void ApplyTexture()
    {
        if (texture2D == null || drawingTexture == null) return;

        texture2D.Apply();

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = drawingTexture;
        Graphics.Blit(texture2D, drawingTexture);
        RenderTexture.active = previous;
    }

    void LateUpdate()
    {
        if (isDrawing) ApplyTexture();
    }

    public void Clear()
    {
        for (int x = 0; x < drawingTexture.width; x++)
            for (int y = 0; y < drawingTexture.height; y++)
                texture2D.SetPixel(x, y, Color.white);
        ApplyTexture();
    }

    public void SetBrushColor(Color color) => brushColor = color;
    public void SetBrushSize(int size) => brushSize = Mathf.Clamp(size, 2, 50);
}