using UnityEngine;
using TourverseToolkit.Runtime;
using System.Collections;
using System;

namespace Tour_Endi_Moon
{
    public class TourEntryPoint : MonoBehaviour
    {
        [SerializeField]
        private PlayerCamera _playerCamera;

        private void Start()
        {
            TourController.TourStart(_playerCamera);
            StartCoroutine(CallabackAfterTime(CheckPoint));
        }

        private IEnumerator CallabackAfterTime(Action callback)
        {
            yield return new WaitForSeconds(30.0f);
            callback?.Invoke();
        }

        private void CheckPoint()
        {
            TourController.CheckPoint(1);
        }
    }
}
