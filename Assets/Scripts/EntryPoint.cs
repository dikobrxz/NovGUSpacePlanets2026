using System;
using System.Collections;
using TourverseToolkit.Runtime;
using UnityEngine;

namespace Tour_ENDI_TourStub5
{
    public class EntryPoint : MonoBehaviour
    {
        public PlayerCamera playerCamera;
        [SerializeField] private TourverseToolkit.Runtime.RenderSettings _renderSettings;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        private IEnumerator CallbackAfterTime(Action callback)
        {
            yield return new WaitForSeconds(30.0f);
            callback?.Invoke();
        }
        private void CheckPoint()
        {
            TourController.CheckPoint(1);
        }
        private void Start()
        {
            _renderSettings.Initialize();
            TourController.TourStart(playerCamera);
            StartCoroutine(CallbackAfterTime(CheckPoint));
        }
    }
}
