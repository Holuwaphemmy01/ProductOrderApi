using Microsoft.EntityFrameworkCore;
using ProductOrderAPI.Domain.Entities;

namespace ProductOrderAPI.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
