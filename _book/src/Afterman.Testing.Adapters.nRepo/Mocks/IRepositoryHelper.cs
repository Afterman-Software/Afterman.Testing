using System;
using System.Collections.Generic;
using Afterman.nRepo;
using Moq;

namespace Afterman.Testing.Mocks
{
    public interface IRepositoryHelper<TRepository, TDomain>
        where TDomain : class
        where TRepository : class, IRepository<TDomain>
    {
        IEnumerable<TDomain> GenerateModels(params Guid[] ids);

        Mock<TRepository> GetMockRepository(params TDomain[] data);
    }
}
