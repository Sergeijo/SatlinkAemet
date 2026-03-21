using System;
using System.Linq.Expressions;

namespace Satlink.Domain.Specifications;

/// <summary>
/// Defines a query specification that can be applied to a repository.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Returns the predicate expression that represents this specification.
    /// </summary>
    Expression<Func<T, bool>> ToExpression();
}
