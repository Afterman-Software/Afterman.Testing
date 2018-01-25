namespace Afterman.Testing.Mocks
{
    using Afterman.nRepo;
    using Moq;

    public abstract class BaseMockData<TDomain>
        where TDomain : class
    {
        protected virtual Mock<IRepository<TDomain>> _repo { get; set; }

        public void ExpectRepoAdd()
        {
            _repo.Setup(r => r
                .Add(It.IsAny<TDomain>()))
                .Verifiable("Add() was not called");
        }

        public void VerifyRepoAddWasNotCalled()
        {
            _repo.Verify(r => r.Add(It.IsAny<TDomain>()), Times.Never(), "Add() should not have been called");
        }
    }
}
