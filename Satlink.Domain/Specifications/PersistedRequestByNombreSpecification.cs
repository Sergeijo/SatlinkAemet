using System;
using System.Linq.Expressions;

using Satlink.Domain.Models;

namespace Satlink.Domain.Specifications;

/// <summary>
/// Specification that matches a <see cref="PersistedRequest"/> by its name.
/// </summary>
public sealed class PersistedRequestByNombreSpecification : ISpecification<PersistedRequest>
{
    private readonly string _nombre;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistedRequestByNombreSpecification"/> class.
    /// </summary>
    /// <param name="nombre">The name to filter by.</param>
    public PersistedRequestByNombreSpecification(string nombre)
    {
        _nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
    }

    /// <inheritdoc />
    public Expression<Func<PersistedRequest, bool>> ToExpression()
        => request => request.nombre == _nombre;
}
