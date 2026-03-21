using System;
using System.Linq.Expressions;

using Satlink.Domain.Models;

namespace Satlink.Domain.Specifications;

/// <summary>
/// Specification that matches a <see cref="PersistedRequest"/> by its download date.
/// </summary>
public sealed class PersistedRequestByFechaDescargaSpecification : ISpecification<PersistedRequest>
{
    private readonly DateOnly _fechaDescarga;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistedRequestByFechaDescargaSpecification"/> class.
    /// </summary>
    /// <param name="fechaDescarga">The download date to filter by.</param>
    public PersistedRequestByFechaDescargaSpecification(DateOnly fechaDescarga)
    {
        _fechaDescarga = fechaDescarga;
    }

    /// <inheritdoc />
    public Expression<Func<PersistedRequest, bool>> ToExpression()
        => request => request.FechaDescarga == _fechaDescarga;
}
