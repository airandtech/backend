using AirandWebAPI.Core.Repositories;
using System;

namespace AirandWebAPI.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IOrderRepository Orders { get; }
        int Complete();
    }
}