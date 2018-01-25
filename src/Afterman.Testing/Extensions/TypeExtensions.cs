namespace Afterman.Testing.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;
    
    public static class TypeExtensions
    {
        public static bool IsShadowedOn(this PropertyInfo property, Type reflectedType)
        {

            if (property.DeclaringType == reflectedType)
                return false;

            try
            {
                var prop = reflectedType.GetProperty(property.Name);
                if (prop == null)
                    return false;

                var getMethod = prop.GetGetMethod();

                if ((getMethod.Attributes & MethodAttributes.Virtual) != 0 && (getMethod.Attributes & MethodAttributes.NewSlot) == 0)
                {
                    return false;
                }
                else if (getMethod.IsHideBySig)
                {
                    var flags = getMethod.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
                    flags |= getMethod.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
                    var paramTypes = getMethod.GetParameters().Select(p => p.ParameterType).ToArray();
                    return (getMethod.DeclaringType.GetTypeInfo().BaseType.GetMethod(getMethod.Name, flags) != null);
                }
                else
                {
                    var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
                    return (getMethod.DeclaringType.GetTypeInfo().BaseType.GetMethods(flags).Any(m => m.Name == getMethod.Name));
                }
            }
            catch (AmbiguousMatchException)
            {
                return true;
            }
        }
    }
}
