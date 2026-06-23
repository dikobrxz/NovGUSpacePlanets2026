using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace TourverseToolkit.Editor
{
    internal sealed class BuildAddressablesService
    {
        private static AddressableAssetSettings settings;

        internal static void BuildGroups(string tourName, string tourAddressablesUuid, SceneAsset sceneAsset, string dllPath)
        {
            Debug.Log(dllPath);
            settings = AddressableAssetSettingsDefaultObject.Settings;

            CreateLabels(tourName);
            SetActiveGroups(tourName);
            SetBuildAndLoadPath(tourName, tourAddressablesUuid);
            AddSceneInGroup(sceneAsset, tourName);
            AddAssemblyInGroup(dllPath, tourName);
            SimplifyActiveGroups(tourName);
            BuildAddressables();
        }

        internal static void GenerateOrUseAddressablesGroups(string tourName)
        {
            settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings == null)
            {
                Debug.LogError("AddressableAssetSettings not found");
                return;
            }

            AddressableAssetGroup groupForScene = settings.FindGroup(tourName);

            if (groupForScene == null)
            {
                groupForScene = CreateGroup(tourName);
                Debug.Log($"Created Addressables group: {tourName}");
            }
            else
            {
                Debug.Log($"Using existing Addressables group: {tourName}");
            }

            string dllPath = $"{tourName}DLL";
            AddressableAssetGroup groupForDll = settings.FindGroup(dllPath);

            if (groupForDll == null)
            {
                groupForDll = CreateGroup(dllPath);
                Debug.Log($"Created Addressables group: {dllPath}");
            }
            else
            {
                Debug.Log($"Using existing Addressables group: {dllPath}");
            }
        }

        private static void BuildAddressables()
        {
            if (settings == null)
            {
                Debug.LogError("Addressables settings not found.");
                return;
            }

            AddressableAssetSettings.BuildPlayerContent();
        }

        private static void CreateLabels(string tourName)
        {
            if (!settings.GetLabels().Contains(tourName) && !settings.GetLabels().Contains($"{tourName}.dll"))
            {
                settings.AddLabel(tourName);
                settings.AddLabel($"{tourName}.dll");
            }
        }

        private static void SimplifyActiveGroups(string tourName)
        {
            settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings == null)
            {
                Debug.LogError("Addressables settings not found");
                return;
            }

            var group = settings.FindGroup(tourName);
            if (group == null)
            {
                Debug.LogError($"Group '{tourName}' not found");
                return;
            }

            foreach (var entry in group.entries)
            {
                string path = entry.address;
                string simplified = System.IO.Path.GetFileNameWithoutExtension(path);
                entry.SetAddress(simplified);
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, group, true);

            var group1 = settings.FindGroup($"{tourName}DLL");
            if (group1 == null)
            {
                Debug.LogError($"Group '{tourName}' not found");
                return;
            }

            foreach (var entry in group1.entries)
            {
                string path = entry.address;
                string simplified = System.IO.Path.GetFileNameWithoutExtension(path);
                entry.SetAddress(simplified);
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, group1, true);
            AssetDatabase.SaveAssets();
        }

        private static void AddAssemblyInGroup(string dllPath, string tourName)
        {
            if (string.IsNullOrEmpty(dllPath)) return;

            if (settings == null) return;

            if (!System.IO.File.Exists(dllPath))
            {
                Debug.LogError($"DLL not found at path: {dllPath}");
                return;
            }

            string guid = AssetDatabase.AssetPathToGUID(dllPath);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogError($"Failed to get GUID for DLL at path: {dllPath}");
                return;
            }

            string groupName = $"{tourName}DLL";
            AddressableAssetGroup targetGroup = null;

            for (int i = 0; i < settings.groups.Count; i++)
            {
                var gr = settings.groups[i];
                if (gr.name.Equals(groupName))
                {
                    targetGroup = gr;
                    break;
                }
            }

            if (targetGroup == null)
            {
                Debug.LogError($"Group '{groupName}' not found!");
                return;
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var existingEntry = settings.FindAssetEntry(assetPath);

            if (existingEntry != null)
            {
                settings.MoveEntry(existingEntry, targetGroup);
                SetLable($"{tourName}.dll", existingEntry);
            }
            else
            {
                var e = settings.CreateOrMoveEntry(guid, targetGroup);
                SetLable($"{tourName}.dll", e);
            }

            var entry = settings.FindAssetEntry(assetPath);
            if (entry != null)
            {
                entry.address = $"{tourName}DLL";
            }

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

        private static void AddSceneInGroup(SceneAsset sceneAsset, string tourName)
        {
            if (sceneAsset == null) return;

            if (settings == null) return;

            string assetPath = AssetDatabase.GetAssetPath(sceneAsset);
            if (string.IsNullOrEmpty(assetPath)) return;

            string directory = System.IO.Path.GetDirectoryName(assetPath);

            string newPath = System.IO.Path.Combine(directory, tourName + ".unity");

            string error = AssetDatabase.RenameAsset(assetPath, tourName);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError("Failed to rename scene: " + error);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            for (int i = 0; i < settings.groups.Count; i++)
            {
                var gr = settings.groups[i];
                if (!gr.name.Equals(tourName)) continue;

                string newAssetPath = AssetDatabase.GetAssetPath(sceneAsset);
                var existingEntry = settings.FindAssetEntry(newAssetPath);

                if (existingEntry != null)
                {
                    settings.MoveEntry(existingEntry, gr);
                    SetLable(tourName, existingEntry);
                }
                else
                {
                    var e = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(newAssetPath), gr);
                    SetLable(tourName, e);
                }
            }

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

        private static AddressableAssetGroup CreateGroup(string groupName)
        {
            AddressableAssetGroup group =
                settings.CreateGroup(
                    groupName,
                    false,
                    false,
                    false,
                    null,
                    typeof(BundledAssetGroupSchema),
                    typeof(ContentUpdateGroupSchema)
                );

            var bundleSchema = group.GetSchema<BundledAssetGroupSchema>();
            bundleSchema.BuildPath.SetVariableByName(
                settings,
                AddressableAssetSettings.kRemoteBuildPath
            );
            bundleSchema.LoadPath.SetVariableByName(
                settings,
                AddressableAssetSettings.kRemoteLoadPath
            );
            bundleSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;

            var updateSchema = group.GetSchema<ContentUpdateGroupSchema>();
            updateSchema.StaticContent = false;

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();

            return group;
        }

        private static string GetLoadPath(string tourName, string tourAddressablesUuid)
        {
            if (string.IsNullOrEmpty(tourAddressablesUuid))
            {
                return "{AddressablesUuid}/" + $"{tourName}/Android";
            }
            else
            {
                return $"{tourAddressablesUuid}/{tourName}/Android";
            }
        }

        private static string GetBuildPath(string tourName)
        {
            return $"ServerData/{tourName}/Android";
        }


        private static void SetActiveGroups(string tourName)
        {
            foreach (var gr in settings.groups)
            {
                var schema = gr.GetSchema<BundledAssetGroupSchema>();
                if (schema == null) continue;

                schema.IncludeInBuild = false;

                if (gr.name.Contains(tourName) || gr.name.Contains("Default Local Group"))
                {
                    schema.IncludeInBuild = true;
                }
            }
        }

        private static void SetBuildAndLoadPath(string tourName, string tourAddressablesUuid)
        {
            var profiles = settings.profileSettings;
            string profileId = settings.activeProfileId;

            string variableLoadId = profiles.GetValueByName(profileId, "Remote.LoadPath");
            string variableBuildId = profiles.GetValueByName(profileId, "Remote.BuildPath");

            profiles.SetValue(profileId, "Remote.LoadPath", GetLoadPath(tourName, tourAddressablesUuid));
            profiles.SetValue(profileId, "Remote.BuildPath", GetBuildPath(tourName));
        }

        private static void SetLable(string label, AddressableAssetEntry entry)
        {
            entry.SetLabel(label, true);
        }
    }
}
