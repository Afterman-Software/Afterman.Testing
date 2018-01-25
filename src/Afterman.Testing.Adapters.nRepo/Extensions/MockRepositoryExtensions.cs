namespace Afterman.Testing.Extensions
{
    using Afterman.nRepo;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System;
    using System.Linq;
    public static class MockRepositoryExtensions
    {
        public static void SetupRepoGet<T>(this Mock<IRepository<T>> repo, string keyName = "Id")
            where T : class
        {
            foreach (var t in repo.Object.GetAll())
            {
                var found = typeof(T).GetProperties().Where(p => p.Name == keyName);
                Assert.IsTrue(found.Any(),
                    typeof(T).Name.ToString() + " must have a property named " + keyName);

                var prop = found.Single();
                var key = prop.GetValue(t);

                Assert.IsInstanceOfType(key, typeof(Guid));

                repo.Setup(r => r.Get(key)).Returns(t);
            }
        }

        public static void SetupRepoGet<T, U>(this Mock<IRepository<T>> repo, string keyName = "Id")
            where T : class
        {
            foreach (var t in repo.Object.GetAll())
            {
                var found = typeof(T).GetProperties().Where(p => p.Name == keyName);
                Assert.IsTrue(found.Any(),
                    typeof(T).Name.ToString() + " must have a property named " + keyName);

                var prop = found.Single();
                var key = prop.GetValue(t);

                Assert.IsInstanceOfType(key, typeof(U));

                repo.Setup(r => r.Get(key)).Returns(t);
            }
        }
    }
}