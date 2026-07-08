using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tour_ENDI_PlanetsUranus
{

    public class DrawingPlane : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RenderTexture drawingTexture;
        [SerializeField] private Transform brushTip;

        [Header("Settings")]
        [SerializeField] private int brushSize = 20;
        [SerializeField] private Color brushColor = Color.red;

        [Header("Audio")]
        [SerializeField] private AudioSource drawingAudioSource;
        [SerializeField] private AudioClip drawingClip;

        [SerializeField] private float minDistanceForSound = 1f;

        private Texture2D texture2D;
        private Vector2 lastUV;
        private bool isDrawing;
        private Collider canvasCollider;
        private XRGrabInteractable grab;
        private bool isInitialized = false;
        private float distanceSinceLastSound = 0f;

        private float width;
        private float height;

        private void Start()
        {
            canvasCollider = GetComponent<Collider>();

            if (drawingTexture == null)
            {
                Debug.LogError("[DrawingPlane] drawingTexture не назначен!");
                return;
            }

            if (brushTip == null)
            {
                Debug.LogError("[DrawingPlane] brushTip не назначен!");
                return;
            }

            if (canvasCollider == null)
            {
                Debug.LogError("[DrawingPlane] Collider не найден!");
                return;
            }

            CalculatePlaneSize();

            texture2D = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
            grab = brushTip.GetComponentInParent<XRGrabInteractable>();

            ClearTexture();
            isInitialized = true;
        }

        private void CalculatePlaneSize()
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                Vector3 size = renderer.bounds.size;
                width = size.x * 0.182f;
                height = size.y * 0.127f;
                return;
            }
        }

        private void Update()
        {
            if (!isInitialized) return;
            if (drawingTexture == null || texture2D == null || brushTip == null) return;

            if (!IsTouchingCanvas(out Vector3 hitPoint))
            {
                if (isDrawing)
                {
                    isDrawing = false;
                }
                return;
            }

            bool isHeld = grab != null ? grab.isSelected : true;
            if (!isHeld)
            {
                if (isDrawing)
                {
                    isDrawing = false;
                }
                return;
            }

            Vector2 uv = WorldPointToUV(hitPoint);

            if (!isDrawing)
            {
                lastUV = uv;
                isDrawing = true;
                DrawPoint(uv);
                PlayDrawingSound();
                return;
            }

            float distance = Vector2.Distance(lastUV, uv);
            if (distance > 0.01f)
            {
                DrawLine(lastUV, uv);
                lastUV = uv;

                distanceSinceLastSound += distance;
                if (distanceSinceLastSound > minDistanceForSound)
                {
                    PlayDrawingSound();
                    distanceSinceLastSound = 0f;
                }
            }
        }

        private bool IsTouchingCanvas(out Vector3 hitPoint)
        {
            Ray ray = new Ray(brushTip.position, brushTip.forward);
            bool hit = Physics.Raycast(ray, out RaycastHit info, 0.6f);
            hitPoint = hit ? info.point : Vector3.zero;
            return hit && info.collider == canvasCollider;
        }

        private Vector2 WorldPointToUV(Vector3 worldPoint)
        {
            Vector3 local = transform.InverseTransformPoint(worldPoint);
            float u = (local.x + width * 0.5f) / width;
            float v = (local.y + height * 0.5f) / height;
            return new Vector2(u, v);
        }

        private void DrawLine(Vector2 from, Vector2 to)
        {
            Vector2 fromPx = from * 1024;
            Vector2 toPx = to * 1024;
            float distance = Vector2.Distance(fromPx, toPx);
            int steps = Mathf.Max(2, Mathf.CeilToInt(distance / 2f));

            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                Vector2 pixel = Vector2.Lerp(fromPx, toPx, t);
                int x = Mathf.RoundToInt(pixel.x);
                int y = Mathf.RoundToInt(pixel.y);

                x = Mathf.Clamp(x, 0, 1023);
                y = Mathf.Clamp(y, 0, 1023);

                DrawPoint(x, y);
            }
        }

        private void DrawPoint(Vector2 uv)
        {
            int x = Mathf.RoundToInt(uv.x * 1024);
            int y = Mathf.RoundToInt(uv.y * 1024);

            x = Mathf.Clamp(x, 0, 1023);
            y = Mathf.Clamp(y, 0, 1023);

            DrawPoint(x, y);
        }

        private void DrawPoint(int x, int y)
        {
            if (texture2D == null || drawingTexture == null) return;
            if (x < 0 || x >= 1024 || y < 0 || y >= 1024) return;

            int radius = brushSize;

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    if (dx * dx + dy * dy > radius * radius) continue;

                    int px = x + dx;
                    int py = y + dy;
                    if (px >= 0 && px < 1024 && py >= 0 && py < 1024)
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

        private void PlayDrawingSound()
        {
            if (drawingAudioSource == null || drawingClip == null) return;

            drawingAudioSource.volume = 1;
            drawingAudioSource.PlayOneShot(drawingClip);
        }

        private void ClearTexture()
        {
            if (texture2D == null) return;

            for (int x = 0; x < 1024; x++)
                for (int y = 0; y < 1024; y++)
                    texture2D.SetPixel(x, y, Color.white);

            texture2D.Apply();

            if (drawingTexture != null)
            {
                RenderTexture.active = drawingTexture;
                Graphics.Blit(texture2D, drawingTexture);
                RenderTexture.active = null;
            }
        }

        public void Clear() => ClearTexture();
        public void SetBrushColor(Color color) => brushColor = color;
        public void SetBrushSize(int size) => brushSize = Mathf.Clamp(size, 2, 50);
    }

}