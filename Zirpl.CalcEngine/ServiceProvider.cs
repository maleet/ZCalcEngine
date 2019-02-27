using System;
using System.Collections.Generic;
using System.Management.Instrumentation;

namespace Zirpl.CalcEngine
{
    /// <summary>
    /// Default implementation of IServiceProvider.
    /// Uses MVC's built in DependencyResolver to get a requested service.
    /// In my app I used Ninject, but you should be able to use anything compatible with the DependencyResolver.
    /// </summary>
    public class ServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public T GetService<T>() where T : class
        {
            var serviceType = typeof(T);
            return (T) GetService(serviceType);
        }

        public object GetService(Type serviceType)
        {
            if (!_services.ContainsKey(serviceType))
            {
                throw new InstanceNotFoundException($"No registred service: {serviceType}");
            }

            return _services[serviceType];
        }
    }
}