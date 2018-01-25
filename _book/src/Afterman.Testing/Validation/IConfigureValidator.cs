namespace Afterman.Testing.Validation
{
    using System;
    using System.Collections.Generic;

    public interface IConfigureValidator<TResult, TPrototype>
        where TResult : class
        where TPrototype : class
    {
        IConfigureValidator<TResult, TPrototype> Excluding(string propertyName);

        IConfigureValidator<TResult, TPrototype> WithMapping(string resultPropertyName, string prototypePropertyName);

        IConfigureValidator<TResult, TPrototype> WithMapping(string resultPropertyName, string prototypePropertyName, Func<object, object> prototypePropertyTransform);

        IConfigureValidator<TResult, TPrototype> WithMapping(string resultPropertyName, PropertyMapping mapping);

        IConfigureValidator<TResult, TPrototype> SetListOrderExpectation(bool listOrderMustMatch);

        IConfigureValidator<TResult, TPrototype> SetListSelectors(Func<TPrototype, Func<TResult, bool>> resultSelector);

        IConfigureValidator<TChildResult, TChildPrototype> WithChildValidator<TChildResult, TChildPrototype>(string resultPropertyName, string prototypePropertyName)
            where TChildPrototype : class
            where TChildResult : class;

        IDoResultValidation<TResult, TPrototype> Against(TPrototype expectation);

        IDoResultListValidation<TResult, TPrototype> AgainstList(IEnumerable<TPrototype> expectations);
    }
}
