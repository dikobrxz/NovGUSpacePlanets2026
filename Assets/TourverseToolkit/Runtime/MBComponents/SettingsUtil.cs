using System.Linq;
using UnityEngine;

namespace TourverseToolkit.Runtime
{
    internal sealed class SettingsUtil : MonoBehaviour
    {
        [SerializeField] private SettingsItem[] settingsItem;

        public void PowerOff()
        {
#if !DEV
            if (settingsItem.Count() <= 0)
            {
                return;
            }

            for (int i = 0; i < settingsItem.Length; i++)
            {
                settingsItem[i].gameObject.SetActive(false);
            }
#endif
        }
    }
}