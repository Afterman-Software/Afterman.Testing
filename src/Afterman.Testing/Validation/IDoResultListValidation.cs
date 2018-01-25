namespace Afterman.Testing.Validation
{
    using System.Collections.Generic;

    public interface IDoResultListValidation<TResult, TPrototype>
        where TResult : class
        where TPrototype : class
    {
        void Validate(IEnumerable<TResult> results);

        ValidationResult TryValidate(IEnumerable<TResult> result);

    }
}
