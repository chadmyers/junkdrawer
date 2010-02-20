using System;
using MongoDB.Driver;

namespace FubuMVC.NerdDinner.Web.Infrastructure
{
    public interface IMongoConnection
    {
        void Initialize();
        Database Current { get; }
    }

    public class MongoConnection : IMongoConnection, IDisposable
    {
        private readonly object _syncRoot = new object();
        private Mongo _mongo;
        private Database _db;
        private bool _isInitialized;

        public void Initialize()
        {
            should_not_already_be_initialized();
            lock (_syncRoot)
            {
                should_not_already_be_initialized();
                _mongo = new Mongo();
                _mongo.Connect();
                _db = _mongo.getDB("FubuDinner");
                _isInitialized = true;
            }
        }

        private void should_not_already_be_initialized()
        {
            if( _isInitialized ) throw new InvalidOperationException("MongConnection is already initialized");
        }

        public Database Current
        {
            get { return _db; }
        }

        public void Dispose()
        {
            if( _mongo != null ) _mongo.Dispose();
        }
    }
}