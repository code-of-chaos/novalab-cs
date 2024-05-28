// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Extensions;

using System.Linq.Expressions;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class LinqExtensions {
    public static IQueryable<T> ConditionalWhere<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate) {
        return condition 
            ? source.Where(predicate)
            : source;
    }
    
    public static IQueryable<T> ConditionalTake<T>(this IQueryable<T> source, bool condition, int count) {
        return condition && count > 0 
            ? source.Take(count)
            : source;
    }
    public static IQueryable<T>  ConditionalTake<T>(this IQueryable<T> source, bool condition, Range range) {
        return condition 
            ? source.Take(range)
            : source;
    }
}
