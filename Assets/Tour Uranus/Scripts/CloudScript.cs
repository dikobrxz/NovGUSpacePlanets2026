using UnityEngine;

namespace Tour_ENDI_TourStub3
{

    public class LazyClouds : MonoBehaviour
    {
        public float LS_CloudTimeScale = 2;
        public float LS_CloudScale = 4;
        public float LS_CloudScattering = 0.6f;
        public float LS_CloudIntensity = 4;
        public float LS_CloudSharpness = 0.75f;
        public float LS_CloudThickness = 1.0f;
        public float LS_ShadowScale = 0.75f;
        public float LS_DistScale = 10.0f;
        public Vector3 LS_CloudColor = new Vector3(1, 0.9f, 0.95f);

        private Material cloudMaterial;
        private float time = 0f;

        void Start()
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                cloudMaterial = renderer.material;

                Color cloudColor = new Color(0.447f, 0.678f, 0.737f, 0.8f);
                cloudMaterial.SetColor("_BaseColor", cloudColor);
            }
        }

        void Update()
        {
            if (cloudMaterial == null) return;

            time += Time.deltaTime * LS_CloudTimeScale * 0.25f;

            Vector2 offset = new Vector2(time * 0.1f, time * 0.05f);
            cloudMaterial.SetTextureOffset("_BaseMap", offset);

            Vector2 tiling = new Vector2(LS_CloudScale, LS_CloudScale);
            cloudMaterial.SetTextureScale("_BaseMap", tiling);
        }
    }

}