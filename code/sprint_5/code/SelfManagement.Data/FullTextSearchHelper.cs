namespace System.Linq
{
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ObjectContextExtensions
    {
        /// <summary>
        /// Searches in all string properties for the specifed search key.
        /// It is also able to search for several words. If the searchKey is for example 'John Travolta' then
        /// with exactMatch set to false all records which contain either 'John' or 'Travolta' in some string property
        /// are returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="searchKey"></param>
        /// <param name="exactMatch">Specifies if only the whole word or every single word should be searched.</param>
        /// <returns></returns>
        public static IQueryable<T> FullTextSearch<T>(this IQueryable<T> queryable, string searchKey, bool exactMatch = false)
        {
            var parameter = Expression.Parameter(typeof(T), "c");
            var containsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
            var toStringMethod = typeof(object).GetMethod("ToString", new Type[] { });

            var publicProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(p => p.PropertyType == typeof(string));
            Expression orExpressions = null;

            string[] searchKeyParts;
            if (exactMatch)
            {
                searchKeyParts = new[] { searchKey };
            }
            else
            {
                searchKeyParts = searchKey.Split(' ');
            }

            foreach (var property in publicProperties)
            {
                Expression nameProperty = Expression.Property(parameter, property);
                foreach (var searchKeyPart in searchKeyParts)
                {
                    Expression searchKeyExpression = Expression.Constant(searchKeyPart);
                    Expression callContainsMethod = Expression.Call(nameProperty, containsMethod, searchKeyExpression);
                    if (orExpressions == null)
                    {
                        orExpressions = callContainsMethod;
                    }
                    else
                    {
                        orExpressions = Expression.Or(orExpressions, callContainsMethod);
                    }
                }
            }

            MethodCallExpression whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { queryable.ElementType },
                queryable.Expression,
                Expression.Lambda<Func<T, bool>>(orExpressions, new ParameterExpression[] { parameter }));

            return queryable.Provider.CreateQuery<T>(whereCallExpression);
        }
    }
}