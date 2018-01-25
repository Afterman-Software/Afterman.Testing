namespace Afterman.Testing.Tests.Classic
{
    using Afterman.Testing.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class ResultValidatorSpec
    {
        [TestMethod]
        public void SameTypesShouldValidateWithNoConfig()
        {
            var val = ValidatorFactory.GetValidator<Testable>();
            var t1 = new Testable { Id = Guid.NewGuid(), Name = DateTime.Now.Ticks.ToString() };
            var t2 = new Testable { Id = t1.Id, Name = t1.Name };
            var t3 = new Testable { Id = Guid.NewGuid(), Name = "Not Valid" };

            var exec = val.Against(t1);
            exec.Validate(t2);

            try
            {
                exec.Validate(t3);
                Assert.Fail("Expected an AssertFailedException from the result validator");
            }
            catch (AssertFailedException e)
            {
                Assert.IsInstanceOfType(e, typeof(AssertFailedException));
            }

            var r1 = exec.TryValidate(t2);
            Assert.IsTrue(r1.IsValid);
            Assert.AreEqual(0, r1.FailureList.Length);

            var r2 = exec.TryValidate(t3);
            Assert.IsFalse(r2.IsValid);
            Assert.AreEqual(2, r2.FailureList.Length);
        }

        [TestMethod]
        public void ShouldNeverAllowNullResultToBeValid()
        {
            var val = ValidatorFactory.GetValidator<Testable>();
            var t1 = new Testable { Id = Guid.NewGuid(), Name = DateTime.Now.Ticks.ToString() };

            var exec = val.Against(t1);
            try
            {
                exec.Validate(null);
                Assert.Fail("Expected failure when result is null.");
            }
            catch { }

            var r2 = exec.TryValidate(null);
            Assert.IsFalse(r2.IsValid);
            Assert.AreEqual(1, r2.FailureList.Length);
        }

        [TestMethod]
        public void ExcludedFieldsShouldNotAffectValidation()
        {
            var val = ValidatorFactory.GetValidator<Testable>();
            var t1 = new Testable { Id = Guid.NewGuid(), Name = DateTime.Now.Ticks.ToString() };
            var t2 = new Testable { Id = t1.Id, Name = "Not the same" };
            var t3 = new Testable { Id = Guid.NewGuid(), Name = "Not Valid" };

            val.Excluding("Name");
            var exec = val.Against(t1);
            exec.Validate(t2);

            try
            {
                exec.Validate(t3);
                Assert.Fail("Expected an AssertFailedException from the result validator");
            }
            catch (AssertFailedException e)
            {
                Assert.IsInstanceOfType(e, typeof(AssertFailedException));
            }

            var r1 = exec.TryValidate(t2);
            Assert.IsTrue(r1.IsValid);
            Assert.AreEqual(0, r1.FailureList.Length);

            var r2 = exec.TryValidate(t3);
            Assert.IsFalse(r2.IsValid);
            Assert.AreEqual(1, r2.FailureList.Length);
        }

        [TestMethod]
        public void ShouldAllowCustomMappingBetweenFields()
        {
            var val = ValidatorFactory.GetValidator<Testable>();
            var t1 = new Testable { Id = Guid.NewGuid(), Name = "Foo Bar Baz" };
            var t2 = new Testable { Id = t1.Id, Name = t1.Id.ToString() };
            var t3 = new Testable { Id = Guid.NewGuid(), Name = "Not Valid" };

            val.WithMapping("Name", "Id", (p) => p.ToString());
            var exec = val.Against(t1);
            exec.Validate(t2);

            try
            {
                exec.Validate(t3);
                Assert.Fail("Expected an AssertFailedException from the result validator");
            }
            catch (AssertFailedException e)
            {
                Assert.IsInstanceOfType(e, typeof(AssertFailedException));
            }

            var r1 = exec.TryValidate(t2);
            Assert.IsTrue(r1.IsValid);
            Assert.AreEqual(0, r1.FailureList.Length);

            var r2 = exec.TryValidate(t3);
            Assert.IsFalse(r2.IsValid);
            Assert.AreEqual(2, r2.FailureList.Length);
        }

        [TestMethod]
        public void ShouldAllowNestingValidatorsForComplexPropertyValues()
        {
            var val = ValidatorFactory.GetValidator<ComplexTestable>();
            var t1 = new ComplexTestable { Id = Guid.NewGuid(), Name = new Name { First = "one", Last = "two" } };
            var t2 = new ComplexTestable { Id = t1.Id, Name = new Name { First = "one", Last = "two" } };

            val.WithChildValidator<Name, Name>("Name", "Name");

            var exec = val.Against(t1);
            exec.Validate(t2);

            try
            {
                var t3 = new ComplexTestable { Id = t1.Id, Name = new Name { First = "three", Last = "four" } };
                exec.Validate(t3);
                Assert.Fail("Expected an AssertFailedException from the result validator");
            }
            catch (AssertFailedException e)
            {
                Assert.IsInstanceOfType(e, typeof(AssertFailedException));
            }
        }

        [TestMethod]
        public void ShouldAllowComparingDifferentTypes()
        {
            var val = ValidatorFactory.GetValidator<Testable, ComplexTestable>();
            var t1 = new ComplexTestable { Id = Guid.NewGuid(), Name = new Name { First = "Foo", Last = "Bar" } };
            var t2 = new Testable { Id = t1.Id, Name = $"{t1.Name.First} {t1.Name.Last}" };
            var t3 = new Testable { Id = t1.Id, Name = "Not Valid" };

            val.WithMapping("Name", "Name", (p) => p.ToString());
            var exec = val.Against(t1);
            exec.Validate(t2);

            try
            {
                exec.Validate(t3);
                Assert.Fail("Expected an AssertFailedException from the result validator");
            }
            catch (AssertFailedException e)
            {
                Assert.IsInstanceOfType(e, typeof(AssertFailedException));
            }

            var r1 = exec.TryValidate(t2);
            Assert.IsTrue(r1.IsValid);
            Assert.AreEqual(0, r1.FailureList.Length);

            var r2 = exec.TryValidate(t3);
            Assert.IsFalse(r2.IsValid);
            Assert.AreEqual(1, r2.FailureList.Length);
        }
    }
}
