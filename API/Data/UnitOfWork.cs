using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMapper _map;
        private readonly DataContext _contextData;
        public UnitOfWork(DataContext contextData, IMapper map)
        {
            _contextData = contextData;
            _map = map;

        }
        // returning our repositories ->  Creating a new instance of our repositories.
        public IUserRepository RepositoryUser => new UserRepository(_contextData, _map);

        public IMessageUserRepository RepositoryMessageUser => new MessageUserRepository(_contextData, _map);

        public ILikeRepository RepositoryLike => new LikeRepository(_contextData);

        public IPhotoRepository PhotoRepository => new
        PhotoRepository(_contextData);

        public bool ContainsChanges()
        {
            return _contextData.ChangeTracker.HasChanges(); // If there are changes to entities, will return true
        }

        public async Task<bool> TransactionComplete()
        {
            return await _contextData.SaveChangesAsync() > 0; // If changes saved to DB, will return true
        }
    }
}