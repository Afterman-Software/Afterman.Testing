namespace Afterman.Testing.Validation
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Result of the validation of a <see cref="BddTest{TTestable}"/> output.
    /// </summary>
    public sealed class ValidationResult
    {
        /// <summary>
        /// Gets a new <see cref="ValidationResult"/> for a successful test.
        /// </summary>
        public static ValidationResult Success => new ValidationResult(new AssertFailedException[0]);

        /// <summary>
        /// Creates a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="failures"></param>
        public ValidationResult(IEnumerable<AssertFailedException> failures)
        {
            FailureList = failures.ToArray();
        }

        /// <summary>
        /// Gets a value that indicates whether the validation failed.
        /// </summary>
        public bool IsValid => FailureList.Length == 0;

        /// <summary>
        /// Gets an array of <see cref="AssertFailedException"/> resulting from the validation attempt.
        /// </summary>
        public AssertFailedException[] FailureList { get; private set; }
    }
}
