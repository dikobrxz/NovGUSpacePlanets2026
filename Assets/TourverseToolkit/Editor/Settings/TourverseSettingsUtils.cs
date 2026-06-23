using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditorInternal;
using UnityEngine;

namespace TourverseToolkit.Editor
{
    internal sealed class TourverseSettingsUtils
    {
        public const string PACKAGE_URL = "https://github.com/ini-dev2/TourverseToolkit.git#upm";
        public const string SETTINGS_PATH = "Project/Tourverse Toolkit";
        public const string MENU_ITEM_PATH = "Tourverse/";
        public const string MENU_ITEM_PATH_SETTINGS = "Settings..";
        public const string SETTINGS_DATA_PATH = "ProjectSettings/TourverseToolkitSettings.asset";
        public const string MENU_UPDATE = "Update";

        private static AddRequest _addRequest;

        [SettingsProvider]
        private static SettingsProvider CreateProvider()
        {
            return new TourverseSettingsWindow(
                SETTINGS_PATH,
                SettingsScope.Project
            );
        }

        [MenuItem(MENU_ITEM_PATH + MENU_UPDATE, priority = 1)]
        private static void UpdatePackage()
        {
            _addRequest = Client.Add(PACKAGE_URL);
            EditorApplication.update += Progress;
        }

        [MenuItem(MENU_ITEM_PATH + MENU_ITEM_PATH_SETTINGS, priority = 0)]
        private static void OpenSettingsInEditor()
        {
            SettingsService.OpenProjectSettings(SETTINGS_PATH);
        }

        private static TourverseSettings instSettings;
        internal static TourverseSettings LoadOrCreateSettings()
        {
            Object[] objs = InternalEditorUtility.LoadSerializedFileAndForget(SETTINGS_DATA_PATH);
            instSettings = objs.Length > 0 ? (TourverseSettings)objs[0] : (instSettings ?? ScriptableObject.CreateInstance<TourverseSettings>());
            return instSettings;
        }

        internal static void Save()
        {
            if (!instSettings)
            {
                return;
            }

            string directoryName = Path.GetDirectoryName(SETTINGS_DATA_PATH);
            Directory.CreateDirectory(directoryName);
            var obj = new Object[1] { instSettings };
            InternalEditorUtility.SaveToSerializedFileAndForget(obj, SETTINGS_DATA_PATH, true);
        }

        private static void Progress()
        {
            if (_addRequest == null) return;

            if (!_addRequest.IsCompleted)
            {
                // Показываем прогрессбар
                EditorUtility.DisplayProgressBar("Updating Package", "Downloading package...", 0.5f);
                return;
            }

            // Завершение
            EditorApplication.update -= Progress;
            EditorUtility.ClearProgressBar();

            if (_addRequest.Status == StatusCode.Success)
            {
                Debug.Log("Package updated successfully: " + _addRequest.Result.packageId);
            }
            else
            {
                Debug.LogError("Failed to update package: " + _addRequest.Error.message);
            }

            _addRequest = null;
        }
    }
}