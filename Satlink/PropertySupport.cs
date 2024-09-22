using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;

namespace Satlink
{
    /// <summary>
    /// Defines the <see cref="PropertySupport" />
    /// </summary>
    public static class PropertySupport
    {
        /// <summary>
        /// The ExtractPropertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpresssion">The propertyExpresssion<see cref="Expression{Func{T}}"/></param>
        /// <returns>The <see cref="String"/></returns>
        public static String ExtractPropertyName<T>(Expression<Func<T>> propertyExpresssion)
        {
            try
            {
                if (propertyExpresssion == null)
                {
                    throw new ArgumentNullException("propertyExpresssion");
                }

                var memberExpression = propertyExpresssion.Body as MemberExpression;
                if (memberExpression == null)
                {
                    throw new ArgumentException("The expression is not a member access expression.", "propertyExpresssion");
                }

                var property = memberExpression.Member as PropertyInfo;
                if (property == null)
                {
                    throw new ArgumentException("The member access expression does not access a property.", "propertyExpresssion");
                }

                var getMethod = property.GetGetMethod(true);
                if (getMethod.IsStatic)
                {
                    throw new ArgumentException("The referenced property is a static property.", "propertyExpresssion");
                }

                return memberExpression.Member.Name;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [PropertySupport], en el procedimiento [public static String ExtractPropertyName<T>(Expression<Func<T>> propertyExpresssion)]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.WriteLog($"[PropertySupport] - [public static String ExtractPropertyName<T>(Expression<Func<T>> propertyExpresssion)] : {ex.Message}.{ex.StackTrace}");
                return null;
            }
        }
    }
}
