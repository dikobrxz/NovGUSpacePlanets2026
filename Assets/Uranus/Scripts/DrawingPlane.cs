using UnityEngine;

public class DrawingPlane : MonoBehaviour
{
    [Header("References")]
    public RenderTexture drawingTexture;
    public Transform brushTip;

    [Header("Settings")]
    public int brushSize = 8;
    public Color brushColor = Color.red;

    [Header("Plane Size (укажите реальные размеры)")]
    public float planeWidth = 3.15f;
    public float planeHeight = 3.0f;

    private Texture2D texture2D;
    private Vector2 lastPos;
    private bool drawing = false;

    void Start()
    {
        if (drawingTexture == null)
        {
            Debug.LogError("DrawingTexture не назначен!");
            return;
        }

        if (brushTip == null)
        {
            Debug.LogError("Brush Tip не назначен! Перетащите Tip в инспекторе");
            return;
        }

        texture2D = new Texture2D(drawingTexture.width, drawingTexture.height);
        ClearTexture();
        Debug.Log("Инициализация завершена. Размер плоскости: " + planeWidth + " x " + planeHeight);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.transform == brushTip)
        {
            Vector3 localPoint = transform.InverseTransformPoint(other.transform.position);

            float u = localPoint.x / planeWidth + 0.5f;
            float v = localPoint.z / planeHeight + 0.5f;

            v = 1f - v;

            float texX = u * drawingTexture.width;
            float texY = v * drawingTexture.height;

            Debug.Log("localPoint: " + localPoint);
            Debug.Log("UV: u=" + u + ", v=" + v);

            if (u < 0 || u > 1 || v < 0 || v > 1)
            {
                Debug.LogWarning("UV вне доски! u=" + u + ", v=" + v);
                return;
            }

            Vector2 currentPos = new Vector2(texX, texY);

            if (!drawing)
            {
                lastPos = currentPos;
                drawing = true;
            }

            DrawLine(lastPos, currentPos);
            lastPos = currentPos;
        }
    }

    void DrawLine(Vector2 from, Vector2 to)
    {
        float distance = Vector2.Distance(from, to);
        if (distance < 0.1f)
        {
            DrawPoint(to);
            return;
        }

        for (float t = 0; t <= distance; t += 0.5f)
        {
            Vector2 point = Vector2.Lerp(from, to, t / distance);
            DrawPoint(point);
        }
    }

    void DrawPoint(Vector2 point)
    {
        int centerX = Mathf.RoundToInt(point.x);
        int centerY = Mathf.RoundToInt(point.y);

        for (int x = -brushSize; x <= brushSize; x++)
        {
            for (int y = -brushSize; y <= brushSize; y++)
            {
                if (x * x + y * y <= brushSize * brushSize)
                {
                    int px = centerX + x;
                    int py = centerY + y;

                    if (px >= 0 && px < drawingTexture.width && py >= 0 && py < drawingTexture.height)
                    {
                        texture2D.SetPixel(px, py, brushColor);
                    }
                }
            }
        }

        ApplyTexture();
    }

    void ApplyTexture()
    {
        texture2D.Apply();
        RenderTexture.active = drawingTexture;
        Graphics.Blit(texture2D, drawingTexture);
        RenderTexture.active = null;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == brushTip)
        {
            drawing = false;
            Debug.Log("Рисование прекращено");
        }
    }

    void ClearTexture()
    {
        for (int x = 0; x < drawingTexture.width; x++)
        {
            for (int y = 0; y < drawingTexture.height; y++)
            {
                texture2D.SetPixel(x, y, Color.white);
            }
        }
        ApplyTexture();
        Debug.Log("Текстура очищена");
    }
}