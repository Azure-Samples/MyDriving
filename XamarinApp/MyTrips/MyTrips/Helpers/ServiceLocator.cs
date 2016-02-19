using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrips.Helpers
{
    /// <summary>
    /// Simple ServiceLocator implementation.
    /// </summary>
    public sealed class ServiceLocator
    {
        static readonly Lazy<ServiceLocator> instance = new Lazy<ServiceLocator>(() => new ServiceLocator());
        readonly Dictionary<Type, Lazy<object>> registeredServices = new Dictionary<Type, Lazy<object>>();

        /// <summary>
        /// Singleton instance for default service locator
        /// </summary>
        public static ServiceLocator Instance
        {
            get { return instance.Value; }
        }

        /// <summary>
        /// Add a new contract + service implementation
        /// </summary>
        /// <typeparam name="TContract">Contract type</typeparam>
        /// <typeparam name="TService">Service type</typeparam>
        public void Add<TContract, TService>() where TService : new()
        {
            this.registeredServices[typeof(TContract)] =
                new Lazy<object>(() => Activator.CreateInstance(typeof(TService)));
        }

        /// <summary>
        /// This resolves a service type and returns the implementation. Note that this
        /// assumes the key used to register the object is of the appropriate type or
        /// this method will throw an InvalidCastException!
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Implementation</returns>
        public T Resolve<T>()
        {
            Lazy<object> service;
            if (registeredServices.TryGetValue(typeof(T), out service))
            {
                return (T)service.Value;
            }

            throw new Exception("No service found for " + typeof(T).Name);
        }
    }
}
