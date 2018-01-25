namespace Afterman.Testing.Validation
{
    using System;
    using System.Reflection;

    public class PropertyMapping
    {
        public PropertyMapping(string propertyName, Func<object, object> transform)
        {
            _predicate = transform;
            _propertyName = propertyName;
        }

        private readonly string _propertyName;
        private readonly Func<object, object> _predicate;

        public object Transform(object prototype)
        {
            return _predicate(prototype.GetType().GetProperty(_propertyName).GetValue(prototype));
        }
    }
}
