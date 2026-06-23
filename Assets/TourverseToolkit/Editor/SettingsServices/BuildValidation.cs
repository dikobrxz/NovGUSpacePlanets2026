using Mono.Cecil;
using System.IO;
using System.Linq;
using TourverseToolkit.Runtime;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace TourverseToolkit.Editor
{
    internal sealed class BuildValidation
    {
        public static bool Validate(AssemblyDefinitionAsset dllName)
        {
            if (!CheckPlayerCameraComponent()) return false;
            if (!CheckApiCall(dllName)) return false;

            return true;
        }

        private static PlayerCamera cam = null;
        private static bool CheckPlayerCameraComponent()
        {
            var objs = Object.FindObjectsOfType<PlayerCamera>();
            if (objs.Length == 0)
            {
                Debug.LogError("[Validator] PlayerCamera component does not contains in current scene");
                return false;
            }

            if (objs.Length > 1)
            {
                Debug.LogError("[Validator] Scene has more than one component PlayerCamera");
                return false;
            }

            cam = objs[0];
            return true;
        }

        private static bool CheckApiCall(AssemblyDefinitionAsset asmdef)
        {
            var targetAssemblyName = asmdef.name;

            var assembly = CompilationPipeline.GetAssemblies()
                .FirstOrDefault(a => a.name == targetAssemblyName);

            if (assembly == null)
            {
                Debug.LogError($"[Validator] Assembly not found: {targetAssemblyName}");
                return false;
            }

            var dllPath = assembly.outputPath;

            if (!File.Exists(dllPath))
            {
                Debug.LogError($"[Validator] DLL not found: {dllPath}");
                return false;
            }

            var bytes = File.ReadAllBytes(dllPath);
            using var stream = new MemoryStream(bytes);
            var asm = AssemblyDefinition.ReadAssembly(stream);

            bool tourStart = false;
            bool checkPoint = false;

            foreach (var type in asm.MainModule.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody) continue;

                    foreach (var ins in method.Body.Instructions)
                    {
                        if (ins.Operand is MethodReference mr)
                        {
                            var fullType = mr.DeclaringType.FullName;
                            var methodName = mr.Name;

                            if (fullType == "TourverseToolkit.Runtime.TourController" &&
                                methodName == "TourStart")
                            {
                                Debug.Log($"[Validator] Found TourStart in {method.FullName}");
                                tourStart = true;
                            }

                            if (fullType == "TourverseToolkit.Runtime.TourController" &&
                                methodName == "CheckPoint")
                            {
                                Debug.Log($"[Validator] Found CheckPoint in {method.FullName}");
                                checkPoint = true;
                            }
                        }
                    }
                }
            }

            if (!tourStart)
                Debug.LogError("[Validator] TourStart NOT called");

            if (!checkPoint)
                Debug.LogError("[Validator] CheckPoint NOT called");

            return tourStart && checkPoint;
        }
    }
}