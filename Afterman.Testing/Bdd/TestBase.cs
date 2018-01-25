namespace Afterman.Testing.Bdd
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TToValidate"></typeparam>
    [TestClass]
    public abstract class TestBase<TToValidate>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TestBase{TToValidate}"/> class.
        /// </summary>
        protected TestBase()
        {
            var container = new ContextContainer();
            _configureContainer = container as IConfigureContainer;
            _resolveInstances = container as IResolveInstance;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TestBase{TToValidate}"/> class.
        /// </summary>
        /// <param name="configureContainer">Custom <see cref="IConfigureContainer"/> implementation</param>
        /// <param name="resolveInstances">Custom <see cref="IResolveInstance"/> implementation</param>
        protected TestBase(IConfigureContainer configureContainer, IResolveInstance resolveInstances)
        {
            _configureContainer = configureContainer;
            _resolveInstances = resolveInstances;
        }

        private readonly IConfigureContainer _configureContainer;
        private readonly IResolveInstance _resolveInstances;

        /// <summary>
        /// When overridden in a derived class, sets up known quantities to facilitate the unit test.
        /// </summary>
        /// <param name="container"></param>
        protected abstract void Given(IConfigureContainer container);

        /// <summary>
        /// When overridden in a derived class, executes the unit of work that is to be tested.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        protected abstract TToValidate When(IResolveInstance container);

        /// <summary>
        /// When overridden in a derived class, validates the result of the execution of the unit of work to be tested.
        /// </summary>
        /// <param name="assertions"></param>
        protected abstract void Then(TToValidate assertions);

        /// <summary>
        /// Executes the test using a Give/When/Then format.
        /// </summary>
        public virtual void Execute()
        {
            Given(_configureContainer);
            Then(When(_resolveInstances));
        }
    }
}
