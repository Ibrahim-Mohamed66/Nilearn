using Nilearn.Shared.Models;
using Microsoft.EntityFrameworkCore;


namespace Nilearn.Application.Common.Extensions
{
    public static class PaginationExtension
    {
        public static async Task<PagedResponse<T>> ToPagedAsync<T>(this IQueryable<T> query,int pageNumber,int pageSize,
            CancellationToken cancellationToken = default)
        {
            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

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
