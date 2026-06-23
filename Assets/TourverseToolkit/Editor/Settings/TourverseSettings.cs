using System;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;

namespace TourverseToolkit.Editor
{
    internal sealed class TourverseSettings : ScriptableObject
    {
        [SerializeField] private List<TourverseTour> toursList;
        [SerializeField] private bool generateHybrid = false;
        [SerializeField] private bool generateOnceObject = false;
        [SerializeField] private string onceObjectName;

        [Serializable]
        private sealed class TourverseTour
        {
            [SerializeField] private string tourName;
            [SerializeField] private SceneAsset tourScene;
            [SerializeField] private AssemblyDefinitionAsset tourAssembly;
            [SerializeField] private string tourAddressablesUuid;
        }
    }
}