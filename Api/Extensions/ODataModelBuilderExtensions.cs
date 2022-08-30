 // ReSharper disable once CheckNamespace
namespace System.Web.OData.Builder
{
    internal static class ODataModelBuilderExtensions
    {
        public static EntitySetConfiguration<T> EntitySet<T>(this ODataConventionModelBuilder builder)
            where T : class
        {
            return builder.EntitySet<T>(typeof(T).Name);
        }
    }
}