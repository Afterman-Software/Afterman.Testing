namespace Afterman.Testing
{
    using Afterman.nRepo;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.Linq;

    public static class MockRepositoryHelper
    {
        public static Mock<IRepository<TDomain>> GetMockRepositoryFor<TDomain, TDomainKey>(string keyField, params TDomain[] included)
            where TDomain : class
        {
            var keyProp = typeof(TDomain).GetProperties().FirstOrDefault(p => p.Name == keyField);
            Assert.IsTrue(keyProp != null, $"{typeof(TDomain).Name.ToString()} must have a property named {keyField}.");

            var queryable = included.AsQueryable();

            var repo = new Mock<IRepository<TDomain>>();

            repo.Setup(r => r.CreateQuery())
                .Returns(() => queryable);

            repo.Setup(r => r.Get(It.IsAny<TDomainKey>()))
                .Returns((TDomainKey key) =>
                {
                    var val = included.AsEnumerable().SingleOrDefault(i =>
                    {
                        var iteration_key = keyProp.GetValue(i);
                        Assert.IsInstanceOfType(iteration_key, typeof(TDomainKey));
                        return key.Equals((TDomainKey)iteration_key);
                    });
                    return val;
                });

            repo.Setup(r => r.GetAll())
                .Returns(() => included.ToList());

            // repo needs to implement iQueryable<TDomain>
            repo.Setup(r => r.GetEnumerator()).Returns(queryable.GetEnumerator());
            repo.Setup(r => r.Provider).Returns(queryable.Provider);
            repo.Setup(r => r.ElementType).Returns(queryable.ElementType);
            repo.Setup(r => r.Expression).Returns(queryable.Expression);

            return repo;
        }
    }
}
