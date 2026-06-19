using System;
using System.Collections;
using TourverseToolkit.Runtime;
using UnityEngine;

namespace Tour_Endi_Saturn
{
    public class EntryPoint : MonoBehaviour
    {       
        [SerializeField] private PlayerCamera _playerCamera;

        private void OnValidate()
        {            
            _playerCamera = FindObjectOfType<PlayerCamera>();
        }      

        private void Start()
        {
            TourController.TourStart(_playerCamera);
            StartCoroutine(CallbackAfterTime(CheckPoint));
        }

        private IEnumerator CallbackAfterTime(Action callback)
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
