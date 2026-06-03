using UnityEngine;

public class DrawingBoard : MonoBehaviour
{
    public RenderTexture drawingTexture;
    public int brushSize = 5;
    public Color brushColor = Color.red;

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

        texture2D = new Texture2D(drawingTexture.width, drawingTexture.height);
        Clear();
        Debug.Log("Ready. Texture size: " + drawingTexture.width + "x" + drawingTexture.height);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Marker"))
        {
            Vector3 localPoint = transform.InverseTransformPoint(other.transform.position);
            float u = (localPoint.x / transform.localScale.x) + 0.5f;
            float v = (localPoint.z / transform.localScale.z) + 0.5f;

            Debug.Log("UV: u=" + u + ", v=" + v);

            Vector2 currentPos = new Vector2(u * drawingTexture.width, v * drawingTexture.height);

            Debug.Log("Pixel: x=" + currentPos.x + ", y=" + currentPos.y);

            if (!drawing)
            {
                lastPos = currentPos;
                drawing = true;
                Debug.Log("Начало рисования");
            }

            DrawLine(lastPos, currentPos);
            lastPos = currentPos;
        }
    }

    void DrawLine(Vector2 from, Vector2 to)
    {
        float distance = Vector2.Distance(from, to);
        Debug.Log("Рисуем линию. Длина: " + distance + " пикселей");

        for (float t = 0; t <= distance; t++)
        {
            Vector2 point = Vector2.Lerp(from, to, t / distance);
            DrawPoint(point);
        }
    }

    void DrawPoint(Vector2 point)
    {
        int x = Mathf.RoundToInt(point.x);
        int y = Mathf.RoundToInt(point.y);

        Debug.Log("Точка: " + x + ", " + y);

        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                int px = x + i;
                int py = y + j;
                if (px >= 0 && px < drawingTexture.width && py >= 0 && py < drawingTexture.height)
                {
                    texture2D.SetPixel(px, py, brushColor);
                }
            }
        }

        texture2D.Apply();
        RenderTexture.active = drawingTexture;
        Graphics.Blit(texture2D, drawingTexture);
        RenderTexture.active = null;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Marker"))
        {
            drawing = false;
            Debug.Log("Рисование прекращено");
        }
    }

    public void Clear()
    {
        for (int x = 0; x < drawingTexture.width; x++)
            for (int y = 0; y < drawingTexture.height; y++)
                texture2D.SetPixel(x, y, Color.white);
        texture2D.Apply();
        RenderTexture.active = drawingTexture;
        Graphics.Blit(texture2D, drawingTexture);
        RenderTexture.active = null;
        Debug.Log("Доска очищена");
    }
}
