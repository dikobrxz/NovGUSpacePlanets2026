using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace TourverseToolkit.Editor
{
    internal static class PackerService
    {
        public static string PackTour(string tourName, string sevenZipPath)
        {
            if (!File.Exists(sevenZipPath))
                throw new FileNotFoundException($"[Tourverse] 7z executable not found at: {sevenZipPath}");

            string projectRoot = Application.dataPath.Replace("/Assets", "").Replace("\\Assets", "");
            string tourDir     = Path.Combine(projectRoot, "ServerData", tourName);
            string archivePath = Path.Combine(projectRoot, "ServerData", $"{tourName}.7z");

            if (!Directory.Exists(tourDir))
                throw new DirectoryNotFoundException($"[Tourverse] Build output not found: {tourDir}");

            if (File.Exists(archivePath))
                File.Delete(archivePath);

            string serverDataDir = Path.Combine(projectRoot, "ServerData");

            var psi = new ProcessStartInfo
            {
                FileName               = sevenZipPath,
                Arguments              = $"a -t7z \"{archivePath}\" \"{tourName}\"",
                WorkingDirectory       = serverDataDir,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute        = false,
                CreateNoWindow         = true,
            };

            using var process = Process.Start(psi);
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                string err = process.StandardError.ReadToEnd();
                throw new Exception($"[Tourverse] 7z failed for '{tourName}': {err}");
            }

            return archivePath;
        }

        public static void DeleteArchive(string archivePath) => File.Delete(archivePath);
    }
}
