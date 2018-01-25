namespace Afterman.Testing.Tests.Classic
{
    using Afterman.Testing.Bdd;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;


    public class Testable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class Name
    {
        public string First { get; set; }
        public string Last { get; set; }
        public override string ToString()
        {
            return $"{First} {Last}";
        }
    }

    public class ComplexTestable
    {
        public Guid Id { get; set; }
        public Name Name { get; set; }
    }

    public interface ITestableFactory
    {
        Testable Get(Guid id, string name);
    }

    public class TestableFactory : ITestableFactory
    {
        public Testable Get(Guid id, string name)
        {
            return new Testable { Id = id, Name = name };
        }
    }

    public class PassingTest : TestBase<Testable>
    {
        public PassingTest()
        {
            _expectId = Guid.NewGuid();
            _expectName = DateTime.Now.Ticks.ToString();
        }

        private readonly Guid _expectId;
        private readonly string _expectName;

        protected override void Given(IConfigureContainer container)
        {
            container.RegisterInstance<ITestableFactory>(new TestableFactory());
        }

        protected override Testable When(IResolveInstance container)
        {
            return container.GetInstance<ITestableFactory>().Get(_expectId, _expectName);
        }

        protected override void Then(Testable assertions)
        {
            Assert.AreEqual(_expectId, assertions.Id);
            Assert.AreEqual(_expectName, assertions.Name);
        }

    }


    public class FailingTest : TestBase<Testable>
    {
        public FailingTest()
        {
            _expectId = Guid.NewGuid();
            _expectName = DateTime.Now.Ticks.ToString();
        }

        private readonly Guid _expectId;
        private readonly string _expectName;

        protected override void Given(IConfigureContainer container)
        {
            container.RegisterInstance<ITestableFactory>(new TestableFactory());
        }

        protected override Testable When(IResolveInstance container)
        {
            return container.GetInstance<ITestableFactory>().Get(Guid.Empty, _expectName);
        }

        protected override void Then(Testable assertions)
        {
            Assert.AreEqual(_expectId, assertions.Id);
            Assert.AreEqual(_expectName, assertions.Name);
        }

    }
}
