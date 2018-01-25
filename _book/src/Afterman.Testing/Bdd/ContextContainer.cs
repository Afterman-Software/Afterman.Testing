namespace Afterman.Testing.Bdd
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal sealed class ContextContainer : IConfigureContainer, IResolveInstance
    {
        public ContextContainer()
        {
            _container = new Dictionary<Type, object>();
        }

        private readonly IDictionary<Type, object> _container;

        public IConfigureContainer RegisterInstance<TType>(TType instance) where TType : class
        {
            return RegisterInstance(typeof(TType), instance);
        }

        public IConfigureContainer RegisterInstance(Type type, object instance)
        {
            if (_container.ContainsKey(type))
                _container[type] = instance;
            else
                _container.Add(type, instance);

            return this as IConfigureContainer;
        }

        public IConfigureContainer Clear()
        {
            _container.Clear();
            return this;
        }

        public TType Resolve<TType>() where TType : class
        {
            return Resolve<TType>(new Dictionary<string, object>());
        }

        public TType Resolve<TType>(IDictionary<string, object> constructorArguments) where TType : class
        {
            return Resolve(typeof(TType), constructorArguments) as TType;
        }

        public object Resolve(Type type)
        {
            return Resolve(type, new Dictionary<string, object>());
        }

        public object Resolve(Type type, IDictionary<string, object> constructorArguments)
        {
            var bestCtor = type.GetConstructors()
                .Where(c =>
                {
                    var parms = c.GetParameters();
                    return parms.All(p => constructorArguments.ContainsKey(p.Name) || _container.ContainsKey(p.ParameterType));
                })
                .OrderByDescending(o => o.GetParameters().Length)
                .FirstOrDefault();


            if (bestCtor != null)
            {
                var args = bestCtor.GetParameters()
                    .Select(p =>
                    {
                        return (constructorArguments.ContainsKey(p.Name)) ? constructorArguments[p.Name] :
                                (_container.ContainsKey(p.ParameterType)) ? _container[p.ParameterType] : null;
                    })
                    .ToArray();

                return bestCtor.Invoke(args);
            }
            else
            {
                return null;
            }
        }

        public TType GetInstance<TType>() where TType : class
        {
            return (_container.ContainsKey(typeof(TType))) ? _container[typeof(TType)] as TType : null;
        }
    }
}
