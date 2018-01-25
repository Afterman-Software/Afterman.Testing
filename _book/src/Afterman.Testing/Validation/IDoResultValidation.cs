namespace Afterman.Testing.Validation
{
    public interface IDoResultValidation<TResult, TPrototype>
        where TResult : class
        where TPrototype : class
    {
        void Validate(TResult result);

        ValidationResult TryValidate(TResult result);
    }
}
