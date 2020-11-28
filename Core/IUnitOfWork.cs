using AirandWebAPI.Core.Repositories;
using System;
using System.Threading.Tasks;

namespace AirandWebAPI.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IOrderRepository Orders { get; }
        IRiderRepository Riders { get; }
        IDispatchRequestInfoRepository DispatchInfo { get; }
        IRegionRepository Regions { get; }
        IInvoiceRepository Invoices { get; }
        IOtpRepository Otps { get; }
        ICompanyRepository Companies { get; }
        Task<int> Complete();
    }
}