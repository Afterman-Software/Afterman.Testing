namespace Afterman.Testing.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using Afterman.nRepo;
    using Moq;

    public static class RepositoryExtensions
    {
        public static void SetupRepository<T, U>(this Mock<T> mock, IList<U> list)
            where T : class, IRepository<U>
            where U : class
        {
            var queryable = list.AsQueryable();
            mock.Setup(r => r.GetEnumerator()).Returns(queryable.GetEnumerator());
            mock.Setup(r => r.Provider).Returns(queryable.Provider);
            mock.Setup(r => r.ElementType).Returns(queryable.ElementType);
            mock.Setup(r => r.Expression).Returns(queryable.Expression);
        }
    }
}
