using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Linq;

namespace FubuDinner.Web.Infrastructure
{
    public interface IRepository
    {
        IEnumerable<T> GetAll<T>() where T : class;
        T Get<T>(string id) where T : class;
        void Save<T>(T entity) where T : class;
        IQueryable<T> Find<T>(Expression<Func<T, bool>> where) where T : class;
    }

    public class MongoDbRepository : IRepository
    {
        private readonly IMongoConnection _connection;
        private readonly IMongoEntityTransformer _transformer;

        public MongoDbRepository(IMongoConnection connection, IMongoEntityTransformer transformer)
        {
            _connection = connection;
            _transformer = transformer;
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            return All<T>().FindAll().ToEntities<T>(_transformer);
        }

        public T Get<T>(string id) where T : class
        {
            var spec = new Document();
            spec["_id"] = id;
            return All<T>().FindOne(spec).ToEntity<T>(_transformer);
        }

        public IMongoCollection All<T>() where T : class
        {
            return _connection.Current.GetCollection(typeof (T).Name);
        }

        public void Save<T>(T t) where T : class
        {
            var doc = t.ToDocument(_transformer);
            var idDoc = new Document();
            idDoc["_id"] = doc["_id"];
            var col = All<T>();

            if (col.Count(idDoc) > 0)
            {
                col.Update(doc);
            }
            else
            {
                All<T>().Insert(doc);
            }
        }

        public IQueryable<T> Find<T>(Expression<Func<T, bool>> where) where T : class
        {
            var provider = new MongoQueryProvider(All<T>());
            return provider.CreateQuery<T>(where);
        }
    }

    public static class MongoExtensions
    {
        public static T ToEntity<T>(this Document mongoDoc, IMongoEntityTransformer transformer) where T : class
        {
            return transformer.Transform<T>(mongoDoc);
        }

        public static IEnumerable<T> ToEntities<T>(this ICursor mongoCursor, IMongoEntityTransformer transformer) where T : class
        {
            return mongoCursor.Documents.Select(d => transformer.Transform<T>(d));
        }

        public static Document ToDocument<T>(this T entity, IMongoEntityTransformer transformer) where T : class
        {
            return transformer.Transform(entity);
        }
    }
}