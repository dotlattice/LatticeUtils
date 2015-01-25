using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LatticeUtils
{
    /// <summary>
    /// Methods for reflecting a type using expressions.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Returns a property's info.
        /// </summary>
        /// <typeparam name="T">the declaring type of the property</typeparam>
        /// <param name="expression">selects the property</param>
        /// <returns>the selected <c>PropertyInfo</c> (never null)</returns>
        /// <exception cref="System.ArgumentException">if the expression does not select a property</exception>
        public static PropertyInfo Property<T>(Expression<Func<T, object>> expression)
        {
            return GetPropertyInfo(typeof(T), expression);
        }

        /// <summary>
        /// Returns a property's info using a strongly typed expression.
        /// </summary>
        /// <typeparam name="T">the declaring type of the property</typeparam>
        /// <typeparam name="TProperty">the type of the property's value</typeparam>
        /// <param name="expression">selects the property</param>
        /// <returns>the selected <c>PropertyInfo</c> (never null)</returns>
        /// <exception cref="System.ArgumentException">if the expression does not select a property</exception>
        public static PropertyInfo Property<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            return GetPropertyInfo(typeof(T), expression);
        }

        /// <summary>
        /// Returns a property's info using an expression with no declaring type parameter.
        /// </summary>
        /// <param name="expression">selects the property</param>
        /// <returns>the selected <c>PropertyInfo</c> (never null)</returns>
        /// <exception cref="System.ArgumentException">if the expression does not select a property</exception>
        public static PropertyInfo Property(Expression<Func<object>> expression)
        {
            return GetPropertyInfo(default(Type), expression);
        }

        /// <summary>
        /// Returns a static property's info using a strongly typed expression.
        /// </summary>
        /// <typeparam name="TProperty">the type of the property</typeparam>
        /// <param name="expression">selects the property</param>
        /// <returns>the selected <c>PropertyInfo</c> (never null)</returns>
        /// <exception cref="System.ArgumentException">if the expression does not select a property</exception>
        public static PropertyInfo StaticProperty<TProperty>(Expression<Func<TProperty>> expression)
        {
            return GetPropertyInfo(default(Type), expression);
        }

        private static PropertyInfo GetPropertyInfo(Type type, Expression expression)
        {            
            var memberInfo = GetMemberInfo(expression);
            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format("Expression \"{0}\" is not a property expression.", expression.ToString()));
            }
            if (type != null && type != propertyInfo.ReflectedType && !type.IsSubclassOf(propertyInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format("Expresion \"{0}\" selects a property that is not from type \"{1}\".", expression.ToString(), type.FullName));
            }
            return propertyInfo;
        }

        /// <summary>
        /// Returns a field's info.
        /// </summary>
        /// <typeparam name="T">the declaring type of the field</typeparam>
        /// <param name="expression">selects the field</param>
        /// <returns>the selected <c>FieldInfo</c> (never null)</returns>
        /// <exception cref="System.ArgumentException">if the expression does not select a field</exception>
        public static FieldInfo Field<T>(Expression<Func<T, object>> expression)
        {
            return GetFieldInfo(typeof(T), expression);
        }

        /// <summary>
        /// Returns a field's info using a strongly typed expression.
        /// </summary>
        /// <typeparam name="T">the declaring type of the field</typeparam>
        /// <typeparam name="TField">the type of the field</typeparam>
        /// <param name="expression">selects the field</param>
        /// <returns>the selected <c>FieldInfo</c> (never null)</returns>
        /// <exception cref="System.ArgumentException">if the expression does not select a field</exception>
        public static FieldInfo Field<T, TField>(Expression<Func<T, TField>> expression)
        {
            return GetFieldInfo(typeof(T), expression);
        }

        /// <summary>
        /// Returns a field's info using an expression with no declaring type parameter.
        /// </summary>
        /// <param name="expression">selects the field</param>
        /// <returns>the selected <c>FieldInfo</c> (never null)</returns>
        /// <exception cref="System.ArgumentException">if the expression does not select a field</exception>
        public static FieldInfo Field(Expression<Func<object>> expression)
        {
            return GetFieldInfo(default(Type), expression);
        }

        /// <summary>
        /// Returns a static field's info using a strongly typed expression.
        /// </summary>
        /// <typeparam name="TField">the type of the field</typeparam>
        /// <param name="expression">selects the field</param>
        /// <returns>the selected <c>FieldInfo</c> (never null)</returns>
        /// <exception cref="System.ArgumentException">if the expression does not select a field</exception>
        public static FieldInfo StaticField<TField>(Expression<Func<TField>> expression)
        {
            return GetFieldInfo(default(Type), expression);
        }

        private static FieldInfo GetFieldInfo(Type type, Expression expression)
        {
            var memberInfo = GetMemberInfo(expression);
            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo == null)
            {
                throw new ArgumentException(string.Format("Expression \"{0}\" is not a field expression.", expression.ToString()));
            }
            if (type != null && type != fieldInfo.ReflectedType && !type.IsSubclassOf(fieldInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format("Expresion \"{0}\" selects a property that is not from type \"{1}\".", expression.ToString(), type.FullName));
            }
            return fieldInfo;
        }

        private static MemberInfo GetMemberInfo(Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression == null)
            {
                throw new ArgumentException(string.Format("Expression \"{0}\" is not a lambda expression.", expression.ToString()));
            }

            Expression bodyExpression = lambdaExpression.Body;
            if (bodyExpression.NodeType == ExpressionType.Convert)
            {
                bodyExpression = ((UnaryExpression)bodyExpression).Operand;
            }

            // Special handling for indexed properties.  
            // We get a MethodCallExpression for the getter method of these properties instead of a MemberAccess expression,
            // so we have to do some work to get back to the property info from that.
            if (bodyExpression.NodeType == ExpressionType.Call)
            {
                var indexedPropertyInfo = GetPropertyInfoFromMethodCallExpression(bodyExpression as MethodCallExpression);
                if (indexedPropertyInfo != null)
                {
                    return indexedPropertyInfo;
                }
            }

            MemberExpression memberExpression = null;
            if (bodyExpression.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = bodyExpression as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException(string.Format("Expression \"{0}\" is not a member expression.", expression.ToString()));
            }
            return memberExpression.Member;
        }

        private static PropertyInfo GetPropertyInfoFromMethodCallExpression(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression == null)
            {
                return null;
            }

            var methodInfo = methodCallExpression.Method;
            if (!methodInfo.IsSpecialName || methodInfo.GetParameters().Length != 1)
            {
                return null;
            }

            return methodInfo.DeclaringType.GetProperties().Where(p => p.GetGetMethod() == methodInfo).FirstOrDefault();
        }

        /// <summary>
        /// Returns a method's info.
        /// </summary>
        /// <typeparam name="T">the declaring type of the method</typeparam>
        /// <param name="expression">selects the method</param>
        /// <returns>the selected <c>MethodInfo</c> (never null)</returns>
        /// <exception cref="System.ArgumentException">if the expression does not select a method</exception>
        public static MethodInfo Method<T>(Expression<Func<T, object>> expression)
        {
            return GetMethodInfo(typeof(T), expression);
        }

        /// <summary>
        /// Returns a method's info using a strongly typed expression.
        /// </summary>
        /// <typeparam name="T">the declaring type of the method</typeparam>
        /// <typeparam name="TReturn">the return type of the method</typeparam>
        /// <param name="expression">selects the method</param>
        /// <returns>the selected <c>MethodInfo</c> (never null)</returns>
        /// <exception cref="System.ArgumentException">if the expression does not select a method</exception>
        public static MethodInfo Method<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            return GetMethodInfo(typeof(T), expression);
        }

        /// <summary>
        /// Returns a void method's info.
        /// </summary>
        /// <typeparam name="T">the declaring type of the method</typeparam>
        /// <param name="expression">selects the method</param>
        /// <returns>the selected <c>MethodInfo</c> (never null)</returns>
        /// <exception cref="System.ArgumentException">if the expression does not select a method</exception>
        public static MethodInfo Method<T>(Expression<Action<T>> expression)
        {
            return GetMethodInfo(typeof(T), expression);
        }

        /// <summary>
        /// Returns a method's info using a non-generic action expression.
        /// </summary>
        /// <param name="expression">selects the method</param>
        /// <returns>the selected <c>MethodInfo</c> (never null)</returns>
        /// <exception cref="System.ArgumentException">if the expression does not select a method</exception>
        public static MethodInfo Method(Expression<Action> expression)
        {
            return GetMethodInfo(default(Type), expression);
        }

        /// <summary>
        /// Returns a static method's info using a strongly typed expression.
        /// </summary>
        /// <typeparam name="TReturn">the return type of the method</typeparam>
        /// <param name="expression">selects the method</param>
        /// <returns>the selected <c>MethodInfo</c> (never null)</returns>
        /// <exception cref="System.ArgumentException">if the expression does not select a method</exception>
        public static MethodInfo StaticMethod<TReturn>(Expression<Func<TReturn>> expression)
        {
            return GetMethodInfo(default(Type), expression);
        }

        private static MethodInfo GetMethodInfo(Type type, Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression == null)
            {
                throw new ArgumentException(string.Format("Expression \"{0}\" is not a lambda expression.", expression.ToString()));
            }

            var bodyExpression = lambdaExpression.Body;
            if (bodyExpression.NodeType == ExpressionType.Convert)
            {
                bodyExpression = ((UnaryExpression)bodyExpression).Operand;
            }

            MethodCallExpression methodCallExpression = null;
            if (bodyExpression.NodeType == ExpressionType.Call)
            {
                methodCallExpression = bodyExpression as MethodCallExpression;
            }

            if (methodCallExpression == null)
            {
                throw new ArgumentException(string.Format("Expression \"{0}\" is not a method call expression.", methodCallExpression.ToString()));
            }

            var methodInfo = methodCallExpression.Method;
            if (type != null && type != methodInfo.ReflectedType && !type.IsSubclassOf(methodInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format("Expresion \"{0}\" selects a method that is not from type \"{1}\".", methodCallExpression.ToString(), type));
            }
            return methodInfo;
        }

        /// <summary>
        /// Returns a constructor's info.
        /// </summary>
        /// <param name="expression">selects the constructor</param>
        /// <returns>the selected <c>ConstructorInfo</c> (never null)</returns>
        public static ConstructorInfo Constructor(Expression<Action> expression)
        {
            return GetConstructorInfo(default(Type), expression);
        }

        /// <summary>
        /// Returns a constructor's info using a strongly typed expression.
        /// </summary>
        /// <typeparam name="T">the declaring type of the constructor</typeparam>
        /// <param name="expression">selects the constructor</param>
        /// <returns>the selected <c>ConstructorInfo</c> (never null)</returns>
        public static ConstructorInfo Constructor<T>(Expression<Func<T>> expression)
        {
            return GetConstructorInfo(default(Type), expression);
        }

        private static ConstructorInfo GetConstructorInfo(Type type, Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression == null)
            {
                throw new ArgumentException(string.Format("Expression \"{0}\" is not a lambda expression.", expression.ToString()));
            }

            var bodyExpression = lambdaExpression.Body;
            if (bodyExpression.NodeType == ExpressionType.Convert)
            {
                bodyExpression = ((UnaryExpression)bodyExpression).Operand;
            }

            NewExpression newExpression = null;
            if (bodyExpression.NodeType == ExpressionType.New)
            {
                newExpression = bodyExpression as NewExpression;
            }

            if (newExpression == null)
            {
                throw new ArgumentException(string.Format("Expression \"{0}\" is not a constructor call expression.", newExpression.ToString()));
            }

            var constructorInfo = newExpression.Constructor;
            if (type != null && type != constructorInfo.ReflectedType && !type.IsSubclassOf(constructorInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format("Expresion \"{0}\" selects a constructor that is not from type \"{1}\".", newExpression.ToString(), type));
            }
            return constructorInfo;
        }
    }
}
