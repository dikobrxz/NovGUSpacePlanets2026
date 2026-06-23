using System;

namespace TourverseToolkit.Runtime
{
    internal sealed class Dependency
    {
        private readonly object _instance;
        public object Instance => _instance;

        public Dependency(object instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
    }
}