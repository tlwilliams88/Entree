using System.Linq;
using System.Linq.Expressions;

namespace KeithLink.Svc.Core.Extensions
{
    public static class LinqSortHelpers
    {
        /// <summary>
        /// Add a sort call to the query tree for a given type of item 
        /// </summary>
        /// <param name="source">A queryable source of data</param>
        /// <param name="propertyName">A property on the type in the queryable source</param>
        /// <param name="descending">A boolean indicating whether the sort call to be added is descending</param>
        /// <param name="anotherLevel">A boolean indicating whether the sort call to be added in the first sort call</param>
        /// <example>stmt = stmt.OrderingHelper(fld, ord.Equals("desc", StringComparison.CurrentCultureIgnoreCase), index > 0)</example>
        /// <returns>A shopping cart</returns>
        public static IOrderedQueryable<T> OrderingHelper<T>(this IQueryable<T> source, string propertyName, bool descending, bool anotherLevel)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), string.Empty); // I don't care about some naming
            MemberExpression property = Expression.PropertyOrField(param, propertyName);
            LambdaExpression sort = Expression.Lambda(property, param);
            MethodCallExpression call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));
            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }
    }
}