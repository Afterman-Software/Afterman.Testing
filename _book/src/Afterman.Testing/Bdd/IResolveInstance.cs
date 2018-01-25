namespace Afterman.Testing.Bdd
{
    using System;
    using System.Collections.Generic;

    public interface IResolveInstance
    {
        TType Resolve<TType>() where TType : class;

        TType Resolve<TType>(IDictionary<string, object> constructorArguments) where TType : class;

        object Resolve(Type type);

        TType GetInstance<TType>() where TType : class;
    }
}
