using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DocumentManagment.DataAccess.Extensions
{
    public static class ListOrderExtension
    {
        public static IQueryable<TDocument> SortBy<TDocument>(this IQueryable<TDocument> source, string orderByProperty,
            bool desc)
        {
            string command = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TDocument);
            var property = type.GetProperty(orderByProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TDocument>(resultExpression);
        }
    }
}
