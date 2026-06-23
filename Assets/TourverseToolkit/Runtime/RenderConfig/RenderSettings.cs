using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TourverseToolkit.Runtime
{
    public class RenderSettings : MonoBehaviour
    {
        [SerializeField] private List<RenderConfig> renderConfigs;

        // Текущий активный конфиг (только для чтения)
        public RenderConfig CurrentConfig { get; private set; } = null;

        // Событие, вызываемое при смене конфига
        public event Action<RenderConfig> OnConfigChanged;

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Выбирает и применяет конфиг по умолчанию (где defaultConfig = true)
        /// Если такого нет, ничего не делает.
        /// </summary>
        public void Initialize()
        {
            if (renderConfigs == null || renderConfigs.Count == 0)
            {
                Debug.LogWarning("Нет доступных RenderConfigs.");
                return;
            }

            // Поиск конфига по умолчанию
            RenderConfig defaultConfig = renderConfigs.Find(cfg => cfg.defaultConfig);
            if (defaultConfig != null)
            {
                ApplyConfig(defaultConfig);
            }
            else
            {
                Debug.LogWarning("Не найден RenderConfig по умолчанию (defaultConfig = true)");
            }
        }

        /// <summary>
        /// Применяет конфиг по ключу.
        /// </summary>
        /// <param name="key">Ключ искомого конфига</param>
        /// <returns>true если конфиг найден и применён, иначе false</returns>
        public bool SetByKey(string key)
        {
            if (renderConfigs == null) return false;

            RenderConfig target = renderConfigs.Find(cfg => cfg.key == key);
            if (target == null)
            {
                Debug.LogWarning($"Конфиг с ключом '{key}' не найден.");
                return false;
            }

            ApplyConfig(target);
            return true;
        }

        /// <summary>
        /// Применяет переданный RenderConfig (устанавливает URP Asset).
        /// </summary>
        private void ApplyConfig(RenderConfig config)
        {
            if (config == null)
            {
                Debug.LogError("Попытка применить null конфиг.");
                return;
            }

            if (config.urpAsset == null)
            {
                Debug.LogError($"URP Asset в конфиге '{config.key}' равен null.");
                return;
            }

            // Если уже установлен этот же конфиг – ничего не делаем
            if (CurrentConfig == config)
                return;

            // Устанавливаем URP Asset глобально
            GraphicsSettings.defaultRenderPipeline = config.urpAsset;
            QualitySettings.renderPipeline = config.urpAsset;

            // Для обратной совместимости (некоторые камеры могут держать свой pipeline)
            var allCameras = FindObjectsOfType<Camera>();
            foreach (var cam in allCameras)
            {
                if (cam.cameraType != CameraType.SceneView)
                    cam.renderingPath = RenderingPath.UsePlayerSettings;
            }

            CurrentConfig = config;
            OnConfigChanged?.Invoke(config);
            Debug.Log($"Применён конфиг: {config.key} -> {config.urpAsset.name}");
        }
    }
}
