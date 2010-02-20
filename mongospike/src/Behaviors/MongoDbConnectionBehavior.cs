using FubuMVC.Core;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.NerdDinner.Web.Infrastructure.Behaviors
{
    public class MongoDbConnectionBehavior : BasicBehavior
    {
        private readonly IMongoConnection _connection;

        public MongoDbConnectionBehavior(IMongoConnection connection) 
            : base(PartialBehavior.Ignored /* do not fire this behavior during a partial */)
        {
            _connection = connection;
        }

        protected override DoNext performInvoke()
        {
            _connection.Initialize();
            return DoNext.Continue;
        }
    }
}