using MongoDB.Driver;

namespace FubuMVC.NerdDinner.Web.Infrastructure
{
    public interface IMongoEntityTransformer
    {
        T Transform<T>(Document doc);
        Document Transform<T>(T entity);
    }
}