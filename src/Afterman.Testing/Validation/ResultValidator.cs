namespace Afterman.Testing.Validation
{
    using Afterman.Testing.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Validates the results of an operation against a provided prototype.
    /// </summary>
    /// <typeparam name="TPrototype">Type of the prototype expectation</typeparam>
    /// <typeparam name="TResult"></typeparam>
    internal sealed class ResultValidator<TResult, TPrototype> : IConfigureValidator<TResult, TPrototype>, IDoResultValidation<TResult, TPrototype>, IDoResultListValidation<TResult, TPrototype>
        where TResult : class
        where TPrototype : class
    {

        /// <summary>
        /// Creates a new instance of the <see cref="ResultValidator{TResult, TPrototype}"/> class.
        /// </summary>
        public ResultValidator()
        {
            _exclusions = new List<string>();
            _mappings = new Dictionary<string, PropertyMapping>();
            _childValidators = new Dictionary<string, Action<object>>();
            _enforceListOrder = false;
            _resultSelector = (p) => (r) => true;
        }

        private TPrototype _expectation;
        private IEnumerable<TPrototype> _listExpectation;
        private bool _enforceListOrder;
        private Func<TPrototype, Func<TResult, bool>> _resultSelector;
        private readonly IList<string> _exclusions;
        private readonly IDictionary<string, PropertyMapping> _mappings;
        private readonly IDictionary<string, Action<object>> _childValidators;

        public IConfigureValidator<TResult, TPrototype> Excluding(string propertyName)
        {
            _exclusions.Add(propertyName);
            return this;
        }

        public IConfigureValidator<TResult, TPrototype> WithMapping(string resultPropertyName, string prototypePropertyName)
        {
            return WithMapping(resultPropertyName, prototypePropertyName, o => o);
        }

        public IConfigureValidator<TResult, TPrototype> WithMapping(string resultPropertyName, string prototypePropertyName, Func<object, object> prototypePropertyTransform)
        {
            return WithMapping(resultPropertyName, new PropertyMapping(prototypePropertyName, prototypePropertyTransform));
        }

        public IConfigureValidator<TResult, TPrototype> WithMapping(string resultPropertyName, PropertyMapping mapping)
        {
            _mappings.Add(resultPropertyName, mapping);
            return this;
        }
        
        public IConfigureValidator<TResult, TPrototype> SetListOrderExpectation(bool listOrderMustMatch)
        {
            _enforceListOrder = listOrderMustMatch;
            return this;
        }

        public IConfigureValidator<TResult, TPrototype> SetListSelectors(Func<TPrototype, Func<TResult, bool>> resultSelector)
        {
            _resultSelector = resultSelector;
            return this;
        }

        public IConfigureValidator<TChildResult, TChildPrototype> WithChildValidator<TChildResult, TChildPrototype>(string resultPropertyName, string prototypePropertyName)
            where TChildPrototype : class
            where TChildResult : class
        {
            var configurator = new ResultValidator<TChildResult, TChildPrototype>();

            Action<object> predicate = (o) =>
            {
                var childProto = typeof(TPrototype).GetProperty(prototypePropertyName).GetValue(_expectation) as TChildPrototype;
                if (childProto == null)
                    Assert.Fail($"Type mismatch in child validator prototype for property '{resultPropertyName}'");

                var ox = o as TChildResult;
                if (ox == null)
                    Assert.Fail($"Type mismatch in child validator result for property '{resultPropertyName}'");

                var validator = configurator.Against(childProto);
                validator.Validate(ox);
            };

            _childValidators.Add(resultPropertyName, predicate);
            return configurator;
        }

        public IDoResultValidation<TResult, TPrototype> Against(TPrototype expectation)
        {
            _expectation = expectation;
            return this;
        }

        public IDoResultListValidation<TResult, TPrototype> AgainstList(IEnumerable<TPrototype> expectations)
        {
            _listExpectation = expectations;
            return this;
        }
        
        /// <summary>
        /// Validate the result against the expectation.
        /// </summary>
        /// <param name="result">Instance to compare against the expectation</param>
        /// <exception cref="AssertFailedException">Throws upon first validation failure</exception>
        public void Validate(TResult result)
        {
            DoValidate(result, true);
        }

        /// <summary>
        /// Validate the result against the expectation.
        /// </summary>
        /// <param name="result">Instance to compare against the expectation</param>
        /// <returns><see cref="ValidationResult"/> containing all validation failures</returns>
        public ValidationResult TryValidate(TResult result)
        {
            return new ValidationResult(DoValidate(result));
        }


        public void Validate(IEnumerable<TResult> results)
        {
            DoValidateList(results, true);
        }

        public ValidationResult TryValidate(IEnumerable<TResult> result)
        {
            return new ValidationResult(DoValidateList(result));
        }

        private IEnumerable<AssertFailedException> DoValidate(TResult result, bool throwOnError = false)
        {
            var errorList = new List<AssertFailedException>();

            try
            {
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(TResult));

                var useProps = typeof(TResult).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  .Where(p => _exclusions.Contains(p.Name) == false && p.IsShadowedOn(typeof(TResult)) == false)
                                  .ToArray();

                foreach (var p in useProps)
                {
                    var resultValue = p.GetValue(result);

                    if (_childValidators.ContainsKey(p.Name))
                    {
                        _childValidators[p.Name].Invoke(resultValue);
                    }
                    else
                    {
                        var compValue = (_mappings.ContainsKey(p.Name)) ?
                            _mappings[p.Name].Transform(_expectation) :
                            typeof(TPrototype).GetProperty(p.Name).GetValue(_expectation);

                        try
                        {
                            Assert.AreEqual(compValue, resultValue);
                        }
                        catch (AssertFailedException aex)
                        {
                            if (throwOnError == true)
                                throw aex;
                            else
                                errorList.Add(aex);
                        }
                    }
                }
            }
            catch (AssertFailedException e)
            {
                if (throwOnError == true)
                    throw e;
                else
                    errorList.Add(e);
            }

            return errorList.AsEnumerable();
        }

        private IEnumerable<AssertFailedException> DoValidateList(IEnumerable<TResult> result, bool throwOnError = false)
        {
            var failures = new List<AssertFailedException>();

            if (_enforceListOrder == true)
            {
                try
                {
                    ValidateListOrder(result.ToArray(), _listExpectation.ToArray());
                }
                catch (AssertFailedException e)
                {
                    failures.Add(e);
                    return failures.AsEnumerable();
                }
            }

            var expectation = _listExpectation.ToArray();
            for (var i = 0; i < expectation.Length; i++)
            {
                var e = expectation[i];
                var r = result.FirstOrDefault(rst => _resultSelector(e)(rst));

                Assert.IsNotNull(r);

                _expectation = e;
                failures.AddRange(DoValidate(r, throwOnError));
            }

            return failures.AsEnumerable();
        }

        private void ValidateListOrder(TResult[] results, TPrototype[] prototypes)
        {
            Assert.AreEqual(prototypes.Length, results.Length);

            for (var i = 0; i < prototypes.Length; i++)
                Assert.IsTrue(_resultSelector(prototypes[i])(results[i]));
        }
    }
}
