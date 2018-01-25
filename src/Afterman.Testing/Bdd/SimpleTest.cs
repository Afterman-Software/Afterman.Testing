namespace Afterman.Testing.Bdd
{
    using Afterman.Testing.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    public static class SimpleTest
    {

        public static IConfigureTest<TTestable> Test<TTestable>()
        {
            return new InternalTest<TTestable>() as IConfigureTest<TTestable>;
        }

        private class InternalTest<T> : IConfigureTest<T>, IRunTest<T>, IValidateTest<T>
        {
            private object[] _params;
            private T _result;

            IRunTest<T> IConfigureTest<T>.Given(params object[] parameters)
            {
                _params = parameters;
                return this as IRunTest<T>;
            }

            ValidationResult IValidateTest<T>.Then(Func<T, bool> validator)
            {
                try
                {
                    Assert.IsTrue(validator(_result));
                    return ValidationResult.Success;
                }
                catch (AssertFailedException ex)
                {
                    return new ValidationResult(new[] { ex });
                }
            }

            IValidateTest<T> IRunTest<T>.When(Func<object[], T> runner)
            {
                _result = runner(_params);
                return this as IValidateTest<T>;
            }
        }


    }

    public interface IConfigureTest<T>
    {
        IRunTest<T> Given(params object[] parameters);
    }

    public interface IRunTest<T>
    {
        IValidateTest<T> When(Func<object[], T> runner);
    }

    public interface IValidateTest<T>
    {
        ValidationResult Then(Func<T, bool> validator);
    }
}
