namespace Afterman.Testing.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class DomainFactories
    {
        private static DomainFactories _current;

        public static DomainFactories Current
        {
            get { return (_current ?? (_current = new DomainFactories())); }
        }

        private DomainFactories()
        {
            _registry = new List<FactoryRegistration>();
        }

        private void InitializeRegistry()
        {

        }

        private readonly List<FactoryRegistration> _registry;

        public T Create<T>(string name = "")
            where T : class
        {
            var used = _registry.SingleOrDefault(r => true);
            return null;
        }

        private class FactoryRegistration
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RegisterDomainFactoryAttribute : Attribute
    {
        public RegisterDomainFactoryAttribute() : this(string.Empty)
        {
        }

        public RegisterDomainFactoryAttribute(string name)
        {
            UniqueName = name;
        }

        public string UniqueName { get; set; }
    }
}
