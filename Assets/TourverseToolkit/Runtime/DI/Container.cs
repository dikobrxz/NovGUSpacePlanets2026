using System;
using UnityEngine;
using System.Collections.Generic;

namespace TourverseToolkit.Runtime
{
    public sealed class Container : IDisposable
    {
        private readonly Dictionary<Type, List<Dependency>> _dependencies = new();

        public void Register<T>() where T : new()
        {
            Type type = typeof(T);
            T obj = new T();

            if (!_dependencies.TryGetValue(type, out var list))
            {
                list = new List<Dependency>(1);
                _dependencies[type] = list;
            }

            list.Add(new Dependency(obj));
        }

        public void Register<T>(T instance) where T : MonoBehaviour
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Type type = typeof(T);

            if (!_dependencies.TryGetValue(type, out var list))
            {
                list = new List<Dependency>(1);
                _dependencies[type] = list;
            }

            list.Add(new Dependency(instance));
        }

        public object Resolve(Type type)
        {
            if (_dependencies.TryGetValue(type, out var list) && list.Count > 0)
                return list[0].Instance;

            return null;
        }

        public T Resolve<T>() where T : class
        {
            var obj = Resolve(typeof(T)) as T;
            if (obj == null)
                throw new InvalidOperationException($"Dependency of type {typeof(T).Name} not found");
            return obj;
        }

        internal Dictionary<Type, List<Dependency>> GetAllDependencies()
        {
            return _dependencies;
        }

        public void Dispose()
        {
            _dependencies.Clear();
        }
    }
}