using System;
using System.Reflection;

namespace TourverseToolkit.Runtime
{
    internal sealed class InjectionService : IDisposable
    {
        private Container _container;

        public InjectionService(Container container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        internal void InjectAll()
        {
            if (_container == null)
                throw new InvalidOperationException("Container is null");

            foreach (var typePair in _container.GetAllDependencies())
            {
                foreach (var dependency in typePair.Value)
                {
                    InjectInto(dependency.Instance);
                }
            }
        }

        private void InjectInto(object target)
        {
            if (target == null)
                return;

            Type type = target.GetType();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                if (method.GetCustomAttribute<InjectAttribute>() == null)
                    continue;

                ParameterInfo[] parameters = method.GetParameters();
                object[] resolvedParams = new object[parameters.Length];

                for (int j = 0; j < parameters.Length; j++)
                {
                    Type paramType = parameters[j].ParameterType;
                    object dependency = _container.Resolve(paramType);

                    if (dependency == null)
                        throw new InvalidOperationException(
                            $"Dependency of type {paramType.Name} not found for method {method.Name}");

                    resolvedParams[j] = dependency;
                }

                method.Invoke(target, resolvedParams);
            }
        }

        public void Dispose()
        {
            _container = null;
        }
    }
}