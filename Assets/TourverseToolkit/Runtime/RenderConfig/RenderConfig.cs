using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TourverseToolkit.Runtime
{   
    [CreateAssetMenu(fileName = "RenderConfig", menuName = "Rendering/Render Config")]
    public class RenderConfig : ScriptableObject
    {
        public string key;                  // ”никальный идентификатор
        public UniversalRenderPipelineAsset urpAsset; // —сылка на URP Asset
        public bool defaultConfig = false;  // ‘лаг конфигурации по умолчанию
    }
}
