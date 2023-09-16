using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork // Can use UOW to implement all repositories in one place -> since EF tracks changes to entities, we can save changes to DB in one place
    {
        IUserRepository RepositoryUser { get; }
        IMessageUserRepository RepositoryMessageUser { get; }
        ILikeRepository RepositoryLike { get; }

        bool ContainsChanges(); // UOW will check if there are any changes to entities -> if there are (tracked by EF), will save changes to DB
        Task<bool> TransactionComplete(); // UOW will save changes to DB and return true if successful -> If fail, will roll back changes and return false


    }
}