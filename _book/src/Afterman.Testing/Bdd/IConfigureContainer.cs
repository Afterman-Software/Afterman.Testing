namespace Afterman.Testing.Bdd
{
    using System;

    public interface IConfigureContainer
    {
        IConfigureContainer RegisterInstance(Type type, object instance);
        IConfigureContainer RegisterInstance<TType>(TType instance) where TType : class;
        IConfigureContainer Clear();
    }
}
