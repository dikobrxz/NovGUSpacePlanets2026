using UnityEngine;

namespace TourverseToolkit.Runtime
{
    public abstract class EntryPoint : MonoBehaviour
    {
        private readonly Container _container = new();
        public Container Container => _container;

        private InjectionService _injectionService;

        private void Awake()
        {
            _injectionService = new(_container);

            InstallDependencies();

            _injectionService.InjectAll();
        }

        protected abstract void InstallDependencies();

        private void OnDestroy()
        {
            _container.Dispose();
            _injectionService.Dispose();
        }
    }
}