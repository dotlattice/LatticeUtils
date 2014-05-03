using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LatticeUtils.Core.ExampleTests.Examples
{
    /// <summary>
    /// Selects properties from entity queryables dynamically.
    /// </summary>
    /// <remarks>
    /// This is just a basic sample to make sure the dynamic anonymous types work with EntityFramework.
    /// </remarks>
    public class QueryableDynamicSelector
    {
        /// <summary>
        /// Selects the properties from type <c>T</c> from the specified queryable.
        /// </summary>
        /// <remarks>
        /// This basically lets you do queryable.Select(x => new { x.Property1, x.Property2 }) without 
        /// having to know which properties you want to select at compile time.
        /// </remarks>
        /// <typeparam name="T">the type of entity from which to select the properties</typeparam>
        /// <param name="queryable">the queryable to apply the select to</param>
        /// <param name="propertyNames">the names of the properties to select</param>
        /// <returns>a queryable of dynamically generated anonymous types</returns>
        public IQueryable SelectProperties<T>(IQueryable<T> queryable, ISet<string> propertyNames)
        {
            var properties = typeof(T).GetProperties().Where(p => propertyNames.Contains(p.Name));

            var entityParameterExpression = Expression.Parameter(typeof(T));
            var propertyExpressions = properties.Select(p => Expression.Property(entityParameterExpression, p));

            var anonymousType = AnonymousTypeUtils.CreateType(properties.ToDictionary(p => p.Name, p => p.PropertyType), isPropertyOrderSignificant: true);
            var anonymousTypeConstructor = anonymousType.GetConstructors().Single();
            var anonymousTypeMembers = anonymousType.GetProperties().Cast<MemberInfo>().ToArray();

            // It's important to include the anonymous type members in the New expression, otherwise EntityFramework 
            // won't recognize this as the constructor of an anonymous type.
            var anonymousTypeNewExpression = Expression.New(anonymousTypeConstructor, propertyExpressions, anonymousTypeMembers);

            var selectLambdaMethod = GetExpressionLambdaMethod(entityParameterExpression.Type, anonymousType);
            var selectBodyLambdaParameters = new object[] { anonymousTypeNewExpression, new[] { entityParameterExpression } };
            var selectBodyLambdaExpression = (LambdaExpression)selectLambdaMethod.Invoke(null, selectBodyLambdaParameters);

            var selectMethod = GetQueryableSelectMethod(typeof(T), anonymousType);
            var selectedQueryable = selectMethod.Invoke(null, new object[] { queryable, selectBodyLambdaExpression }) as IQueryable;
            return selectedQueryable;
        }

        private static MethodInfo GetExpressionLambdaMethod(Type entityType, Type funcReturnType)
        {
            var prototypeLambdaMethod = GetStaticMethod(() => System.Linq.Expressions.Expression.Lambda<Func<object, object>>(default(Expression), default(IEnumerable<ParameterExpression>)));
            var lambdaGenericMethodDefinition = prototypeLambdaMethod.GetGenericMethodDefinition();
            var funcType = typeof(Func<,>).MakeGenericType(entityType, funcReturnType);
            var lambdaMethod = lambdaGenericMethodDefinition.MakeGenericMethod(funcType);
            return lambdaMethod;
        }

        private static MethodInfo GetQueryableSelectMethod(Type entityType, Type returnType)
        {
            var prototypeSelectMethod = GetStaticMethod(() => System.Linq.Queryable.Select(default(IQueryable<object>), default(Expression<Func<object, object>>)));
            var selectGenericMethodDefinition = prototypeSelectMethod.GetGenericMethodDefinition();
            return selectGenericMethodDefinition.MakeGenericMethod(new[] { entityType, returnType });
        }

        public static MethodInfo GetStaticMethod(Expression<Action> expression)
        {
            var lambda = expression as LambdaExpression;
            var methodCallExpression = lambda.Body as MethodCallExpression;
            return methodCallExpression.Method;
        }
    }
}
