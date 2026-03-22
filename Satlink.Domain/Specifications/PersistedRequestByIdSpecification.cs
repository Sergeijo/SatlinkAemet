using System;
using System.Linq.Expressions;

using Satlink.Domain.Models;

namespace Satlink.Domain.Specifications;

/// <summary>
/// Specification that matches a <see cref="PersistedRequest"/> by its string identifier.
/// </summary>
public sealed class PersistedRequestByIdSpecification : ISpecification<PersistedRequest>
{
    private readonly string _id;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistedRequestByIdSpecification"/> class.
    /// </summary>
    /// <param name="id">The string identifier to filter by.</param>
    public PersistedRequestByIdSpecification(string id)
    {
        _id = id ?? throw new ArgumentNullException(nameof(id));
    }

    /// <inheritdoc />
    public Expression<Func<PersistedRequest, bool>> ToExpression()
        => request => request.id == _id;
}
