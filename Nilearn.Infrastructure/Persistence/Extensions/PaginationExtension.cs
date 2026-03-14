using Nilearn.Shared.Models;
using Microsoft.EntityFrameworkCore;


namespace Nilearn.Infrastructure.Persistence.Extensions
{
    public static class PaginationExtension
    {
        public static async Task<PagedResponse<T>> ToPagedAsync<T>(
       this IQueryable<T> query,
       int pageNumber,
       int pageSize)
        {
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
