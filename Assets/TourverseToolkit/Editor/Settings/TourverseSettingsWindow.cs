                 using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace TourverseToolkit.Editor
{
    internal sealed class TourverseSettingsWindow : SettingsProvider
    {
        private SerializedObject tourverseSettings;
        private SerializedProperty toursList;
        private SerializedProperty generateHybrid;
        private SerializedProperty generateOnceObject;
        private SerializedProperty onceObjectName;

        private const string SEVEN_ZIP_PREFS_KEY = "Tourverse.SevenZipPath";

        private string _devDomain, _devAuthEndpoint, _devUploadDomain, _devUploadEndpoint, _devUuid;
        private string _prodDomain, _prodAuthEndpoint, _prodUploadDomain, _prodUploadEndpoint, _prodUuid;
        private bool   _isDev;
        private string _login, _password;

        private const string TOKEN_SESSION_KEY = "Tourverse.Token";

        private string _token
        {
            get => SessionState.GetString(TOKEN_SESSION_KEY, string.Empty);
            set => SessionState.SetString(TOKEN_SESSION_KEY, value ?? string.Empty);
        }

        private string _sevenZipPath;
        private bool   _isProcessing;
        private bool   _showPassword;

        public TourverseSettingsWindow(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            tourverseSettings?.Dispose();
        }

        private void InitGUI()
        {
            TourverseSettings settings = TourverseSettingsUtils.LoadOrCreateSettings();
            tourverseSettings = new(settings);
            toursList          = tourverseSettings.FindProperty("toursList");
            generateHybrid     = tourverseSettings.FindProperty("generateHybrid");
            generateOnceObject = tourverseSettings.FindProperty("generateOnceObject");
            onceObjectName     = tourverseSettings.FindProperty("onceObjectName");

            _devDomain         = EditorPrefs.GetString("Tourverse.Dev.Domain",          string.Empty);
            _devAuthEndpoint   = EditorPrefs.GetString("Tourverse.Dev.AuthEndpoint",    string.Empty);
            _devUploadDomain   = EditorPrefs.GetString("Tourverse.Dev.UploadDomain",    string.Empty);
            _devUploadEndpoint = EditorPrefs.GetString("Tourverse.Dev.UploadEndpoint",  string.Empty);
            _devUuid           = EditorPrefs.GetString("Tourverse.Dev.UUID",            string.Empty);

            _prodDomain         = EditorPrefs.GetString("Tourverse.Prod.Domain",         string.Empty);
            _prodAuthEndpoint   = EditorPrefs.GetString("Tourverse.Prod.AuthEndpoint",   string.Empty);
            _prodUploadDomain   = EditorPrefs.GetString("Tourverse.Prod.UploadDomain",   string.Empty);
            _prodUploadEndpoint = EditorPrefs.GetString("Tourverse.Prod.UploadEndpoint", string.Empty);
            _prodUuid           = EditorPrefs.GetString("Tourverse.Prod.UUID",           string.Empty);

            _isDev    = EditorPrefs.GetBool  ("Tourverse.IsDev",    true);
            _login    = EditorPrefs.GetString("Tourverse.Login",    string.Empty);
            _password = EditorPrefs.GetString("Tourverse.Password", string.Empty);

            _sevenZipPath = EditorPrefs.GetString(SEVEN_ZIP_PREFS_KEY, string.Empty);
        }

        public override void OnGUI(string searchContext)
        {
            if (tourverseSettings == null || !tourverseSettings.targetObject)
                InitGUI();

            tourverseSettings.Update();

            
            bool showOnceObject = generateOnceObject.boolValue;
            bool isAuthorized   = !string.IsNullOrEmpty(_token);


            EditorGUILayout.PropertyField(toursList);
            EditorGUILayout.PropertyField(generateHybrid);
            EditorGUILayout.PropertyField(generateOnceObject);

            if (GUILayout.Button("Generate"))
                OnGenerateClicked();

            if (showOnceObject)
            {
                EditorGUILayout.PropertyField(onceObjectName);
                if (GUILayout.Button("Generate Once"))
                    GenerateByName(onceObjectName.stringValue);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Upload", EditorStyles.boldLabel);

            DrawServerConfig("Dev Config",
                ref _devDomain,         "Tourverse.Dev.Domain",
                ref _devAuthEndpoint,   "Tourverse.Dev.AuthEndpoint",
                ref _devUploadDomain,   "Tourverse.Dev.UploadDomain",
                ref _devUploadEndpoint, "Tourverse.Dev.UploadEndpoint",
                ref _devUuid,           "Tourverse.Dev.UUID");

            EditorGUILayout.Space();

            DrawServerConfig("Prod Config",
                ref _prodDomain,         "Tourverse.Prod.Domain",
                ref _prodAuthEndpoint,   "Tourverse.Prod.AuthEndpoint",
                ref _prodUploadDomain,   "Tourverse.Prod.UploadDomain",
                ref _prodUploadEndpoint, "Tourverse.Prod.UploadEndpoint",
                ref _prodUuid,           "Tourverse.Prod.UUID");

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(isAuthorized);
            bool newIsDev = EditorGUILayout.Toggle("isDev", _isDev);
            if (newIsDev != _isDev)
            {
                _isDev = newIsDev;
                EditorPrefs.SetBool("Tourverse.IsDev", _isDev);
            }
            EditorGUI.EndDisabledGroup();

            string newLogin = EditorGUILayout.TextField("Login", _login);
            if (newLogin != _login)
            {
                _login = newLogin;
                EditorPrefs.SetString("Tourverse.Login", _login);
            }

            EditorGUILayout.BeginHorizontal();
            string newPassword = _showPassword
                ? EditorGUILayout.TextField("Password", _password)
                : EditorGUILayout.PasswordField("Password", _password);
            if (newPassword != _password)
            {
                _password = newPassword;
                EditorPrefs.SetString("Tourverse.Password", _password);
            }
            if (GUILayout.Button(_showPassword ? "Hide" : "Show", GUILayout.Width(40)))
                _showPassword = !_showPassword;
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(_isProcessing);
            if (GUILayout.Button(isAuthorized ? "Logout" : "Login"))
            {
                if (isAuthorized)
                {
                    _token = string.Empty;
                }
                else
                {
                    OnLoginClicked();
                }
            }
            EditorGUI.EndDisabledGroup();

            if (isAuthorized)
                EditorGUILayout.HelpBox("Authorized.", MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Select 7z", "Select 7-Zip executable (7z.exe)"), GUILayout.Width(80)))
            {
                string picked = EditorUtility.OpenFilePanel("Select 7z executable", "", "");
                if (!string.IsNullOrEmpty(picked))
                {
                    _sevenZipPath = picked;
                    EditorPrefs.SetString(SEVEN_ZIP_PREFS_KEY, _sevenZipPath);
                }
            }
            EditorGUILayout.LabelField(string.IsNullOrEmpty(_sevenZipPath) ? "not selected" : _sevenZipPath);
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_sevenZipPath));
            if (GUILayout.Button("x", GUILayout.Width(20)))
            {
                _sevenZipPath = string.Empty;
                EditorPrefs.DeleteKey(SEVEN_ZIP_PREFS_KEY);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!isAuthorized || _isProcessing);
            if (GUILayout.Button("Upload"))
                OnUploadClicked();
            EditorGUI.EndDisabledGroup();

            tourverseSettings.ApplyModifiedProperties();
            TourverseSettingsUtils.Save();
        }

        private void DrawServerConfig(
            string label,
            ref string domain,         string domainKey,
            ref string authEndpoint,   string authKey,
            ref string uploadDomain,   string uploadDomainKey,
            ref string uploadEndpoint, string uploadKey,
            ref string uuid,           string uuidKey)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

            string newDomain = EditorGUILayout.TextField("Auth Domain", domain);
            if (newDomain != domain) { domain = newDomain; EditorPrefs.SetString(domainKey, domain); }

            string newUploadDomain = EditorGUILayout.TextField("Upload Domain", uploadDomain);
            if (newUploadDomain != uploadDomain) { uploadDomain = newUploadDomain; EditorPrefs.SetString(uploadDomainKey, uploadDomain); }

            EditorGUILayout.Space(6);

            string newAuth = EditorGUILayout.TextField("Auth Endpoint", authEndpoint);
            if (newAuth != authEndpoint) { authEndpoint = newAuth; EditorPrefs.SetString(authKey, authEndpoint); }

            string newUpload = EditorGUILayout.TextField("Upload Endpoint", uploadEndpoint);
            if (newUpload != uploadEndpoint) { uploadEndpoint = newUpload; EditorPrefs.SetString(uploadKey, uploadEndpoint); }

            string newUuid = EditorGUILayout.TextField("addressables_info_uuid", uuid);
            if (newUuid != uuid) { uuid = newUuid; EditorPrefs.SetString(uuidKey, uuid); }
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            TourverseSettingsUtils.Save();
        }

        private async void OnLoginClicked()
        {
            string domain   = _isDev ? _devDomain        : _prodDomain;
            string endpoint = _isDev ? _devAuthEndpoint  : _prodAuthEndpoint;

            if (string.IsNullOrWhiteSpace(domain) || string.IsNullOrWhiteSpace(endpoint))
            {
                EditorUtility.DisplayDialog("Tourverse Auth Error", "Auth Domain and Auth Endpoint must not be empty.", "OK");
                return;
            }

            _isProcessing = true;
            Repaint();
            try
            {
                string authUrl = domain + endpoint;
                Debug.Log($"[Tourverse] POST {authUrl} | login={_login}");
                _token = await UploadService.GetTokenAsync(_login, _password, authUrl);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Tourverse] Auth failed: {ex.Message}");
                EditorUtility.DisplayDialog("Tourverse Auth Error", ex.Message, "OK");
                _token = string.Empty;
            }
            finally
            {
                _isProcessing = false;
                Repaint();
            }
        }

        private async void OnUploadClicked()
        {
            string uploadDomain   = _isDev ? _devUploadDomain   : _prodUploadDomain;
            string uploadEndpoint = _isDev ? _devUploadEndpoint : _prodUploadEndpoint;
            string uuid           = _isDev ? _devUuid           : _prodUuid;

            if (string.IsNullOrWhiteSpace(uploadDomain) || string.IsNullOrWhiteSpace(uploadEndpoint))
            {
                EditorUtility.DisplayDialog("Tourverse Upload Error", "Upload Domain and Upload Endpoint must not be empty.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(uuid))
            {
                EditorUtility.DisplayDialog("Tourverse Upload Error", "addressables_info_uuid must not be empty.", "OK");
                return;
            }

            _isProcessing = true;
            Repaint();
            try
            {
                string uploadUrl = uploadDomain + uploadEndpoint;

                int count = toursList.arraySize;
                string projectRoot = Application.dataPath.Replace("/Assets", "").Replace("\\Assets", "");

                var tourNames = new System.Collections.Generic.List<string>();
                for (int i = 0; i < count; i++)
                {
                    string name = toursList.GetArrayElementAtIndex(i).FindPropertyRelative("tourName").stringValue;
                    string tourDir = System.IO.Path.Combine(projectRoot, "ServerData", name);
                    if (System.IO.Directory.Exists(tourDir))
                        tourNames.Add(name);
                    else
                        Debug.LogWarning($"[Tourverse] Skipping '{name}': build output not found at {tourDir}");
                }

                if (tourNames.Count == 0)
                {
                    EditorUtility.DisplayDialog("Tourverse Upload", "No built tours found in ServerData.", "OK");
                    return;
                }

                var archives = new string[tourNames.Count];

                for (int i = 0; i < tourNames.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("Tourverse Upload", $"Packing {tourNames[i]}...", (float)i / tourNames.Count * 0.5f);
                    archives[i] = PackerService.PackTour(tourNames[i], _sevenZipPath);
                }

                for (int i = 0; i < tourNames.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("Tourverse Upload", $"Uploading {tourNames[i]}...", 0.5f + (float)i / tourNames.Count * 0.5f);
                    await UploadService.UploadTourAsync(_token, archives[i], uuid, uploadUrl);
                    PackerService.DeleteArchive(archives[i]);
                    Debug.Log($"[Tourverse] Uploaded: {tourNames[i]}");
                }

                EditorUtility.DisplayDialog("Tourverse Upload", "All tours uploaded successfully.", "OK");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Tourverse] Upload failed: {ex.Message}");
                EditorUtility.DisplayDialog("Tourverse Upload Error", ex.Message, "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                _isProcessing = false;
                Repaint();
            }
        }

        private void OnGenerateClicked()
        {
            CollectTourItems();
            for (int i = 0; i < _toursData.Length; i++)
            {
                var data = _toursData[i];
                GenerateData(data.tourName, data.dllName, data.tourAddressablesUuid, data.scene, data.dllPath);
            }
        }

        private void GenerateByName(string tourName)
        {
            if (string.IsNullOrWhiteSpace(tourName))
            {
                Debug.LogError("Tour name is empty");
                return;
            }

            if (_toursData == null || _toursData.Length == 0)
                CollectTourItems();

            for (int i = 0; i < _toursData.Length; i++)
            {
                var data = _toursData[i];
                if (!string.Equals(data.tourName, tourName, StringComparison.Ordinal))
                    continue;

                Debug.Log(data.tourName);
                GenerateData(data.tourName, data.dllName, data.tourAddressablesUuid, data.scene, data.dllPath);
                return;
            }

            Debug.LogError($"Tour with name '{tourName}' not found");
        }

        private void CollectTourItems()
        {
            int count = toursList.arraySize;
            _toursData = new (string tourName, AssemblyDefinitionAsset dllName, string tourAddressablesUuid, SceneAsset scene, string dllPath)[count];
            BuildDllService.BuildDlls();

            for (int i = 0; i < count; i++)
            {
                var tour = toursList.GetArrayElementAtIndex(i);
                string tourName            = tour.FindPropertyRelative("tourName").stringValue;
                SceneAsset scene           = tour.FindPropertyRelative("tourScene").objectReferenceValue as SceneAsset;
                string dllPath             = BuildDllService.GenerateDllForAndroid(tourName);
                string tourAddressablesUuid = tour.FindPropertyRelative("tourAddressablesUuid").stringValue;
                AssemblyDefinitionAsset dllName = tour.FindPropertyRelative("tourAssembly").objectReferenceValue as AssemblyDefinitionAsset;
                _toursData[i] = (tourName, dllName, tourAddressablesUuid, scene, dllPath);
            }
        }

        private void GenerateData(string tourName, AssemblyDefinitionAsset dllName, string tourAddressablesUuid, SceneAsset scene, string dllPath)
        {
            //TODO: validation
            if (!BuildValidation.Validate(dllName))
            {
                Debug.LogError("[BuildValidate] Сan't make a build because it hasn't passed validation.");
                return;
            }

            BuildAddressablesService.GenerateOrUseAddressablesGroups(tourName);
            BuildAddressablesService.BuildGroups(tourName, tourAddressablesUuid, scene, dllPath);
        }

        private (string tourName, AssemblyDefinitionAsset dllName, string tourAddressablesUuid, SceneAsset scene, string dllPath)[] _toursData;
    }
}