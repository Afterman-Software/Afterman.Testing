namespace Afterman.Testing.Validation
{
    public static class ValidatorFactory
    {
        public static IConfigureValidator<TResult, TResult> GetValidator<TResult>()
            where TResult : class
        {
            return new ResultValidator<TResult, TResult>();
        }

        public static IConfigureValidator<TActual, TExpectation> GetValidator<TActual, TExpectation>()
            where TActual : class
            where TExpectation : class
        {
            return new ResultValidator<TActual, TExpectation>();
        }
    }
}
