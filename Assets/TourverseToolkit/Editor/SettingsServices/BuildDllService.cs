using System.IO;
using UnityEditor;

namespace TourverseToolkit.Editor
{
    internal sealed class BuildDllService
    {
        private const string ANDROID_DLL_BUILD_PATH = "HybridCLR/CompileDll/Android";
        private const string MAIN_TOURS_PATH = "Assets/Tours/";
        private const string PATH_TO_DLL = "HybridCLRData/HotUpdateDlls/Android/";

        internal static void BuildDlls()
        {
            EditorApplication.ExecuteMenuItem(ANDROID_DLL_BUILD_PATH);
        }

        internal static string GenerateDllForAndroid(string tourName)
        {
            return CopyToFolder(tourName);
        }

        private static bool FileContains(string tourName)
        {
            string filePath = Path.Combine(PATH_TO_DLL, $"{tourName}.dll");

            if (!File.Exists(filePath))
            {
                return false;
            }

            return true;
        }

        private static string CopyToFolder(string tourName)
        {
            string filePath = Path.Combine(PATH_TO_DLL, $"{tourName}.dll");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            string dllPath = Path.Combine(PATH_TO_DLL, $"{tourName}.dll.bytes");

            File.Move(filePath, dllPath);

            string directoryPath = Path.Combine(MAIN_TOURS_PATH, tourName, "DLL");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string assetPath = Path.Combine(directoryPath, Path.GetFileName(dllPath));

            File.Copy(dllPath, assetPath, true);

            return assetPath;
        }
    }
}