using System;
using System.Linq;

using Satlink.Api.Contracts;

namespace Satlink.Api.Utilities;

/// <summary>
/// Provides pagination utilities.
/// </summary>
public static class PaginationHelper
{
    /// <summary>
    /// Paginates an <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="query">The query to paginate.</param>
    /// <param name="pageNumber">The 1-based page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A paged result set.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="pageNumber"/> or <paramref name="pageSize"/> are invalid.</exception>
    public static PagedResult<T> Paginate<T>(IQueryable<T> query, int pageNumber, int pageSize)
    {
        // Validate arguments.
        ArgumentNullException.ThrowIfNull(query);

        if (pageNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
        }

        // Evaluate total count.
        int totalItems = query.Count();

        // Calculate total pages.
        int totalPages = CalculateTotalPages(totalItems, pageSize);

        // Calculate skip.
        int skip = GetSkipCount(pageNumber, pageSize);

        // Materialize current page.
        T[] items = query.Skip(skip).Take(pageSize).ToArray();

        // Return paged result.
        return new PagedResult<T>(items, totalItems, pageNumber, totalPages);
    }

    /// <summary>
    /// Calculates the total page count.
    /// </summary>
    /// <param name="totalItems">The total number of items.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>The total number of pages.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="totalItems"/> or <paramref name="pageSize"/> are invalid.</exception>
    public static int CalculateTotalPages(int totalItems, int pageSize)
    {
        // Validate arguments.
        if (totalItems < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalItems), "Total items cannot be negative.");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
        }

        // Compute ceiling division.
        return (int)Math.Ceiling(totalItems / (double)pageSize);
    }

    /// <summary>
    /// Calculates the number of records to skip.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>The skip count.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="pageNumber"/> or <paramref name="pageSize"/> are invalid.</exception>
    public static int GetSkipCount(int pageNumber, int pageSize)
    {
        // Validate arguments.
        if (pageNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
        }

        // Calculate skip based on 1-based page number.
        return (pageNumber - 1) * pageSize;
    }

    /// <summary>
    /// Determines whether a page number is valid.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number.</param>
    /// <param name="totalPages">The total number of pages.</param>
    /// <returns><see langword="true"/> if the page number is valid; otherwise <see langword="false"/>.</returns>
    public static bool IsValidPageNumber(int pageNumber, int totalPages)
    {
        // Validate bounds.
        if (pageNumber <= 0)
        {
            return false;
        }

        if (totalPages <= 0)
        {
            return false;
        }

        // Ensure requested page is within range.
        return pageNumber <= totalPages;
    }
}
