using System.Linq.Expressions;
using System.Reflection;

namespace Codon.Codec;

public static class PropertyRef
{
    public static PropertyInfo Get<TSource, TResult>(Expression<Func<TSource, TResult>> propertySelector)
    {
        return propertySelector.Body is not MemberExpression { Member: PropertyInfo property } ? throw new ArgumentException("Expression must be a simple property access (p => p.Property).") : property;
    }
}