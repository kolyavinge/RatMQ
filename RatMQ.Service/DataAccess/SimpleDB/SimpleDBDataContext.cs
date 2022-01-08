using RatMQ.Service.DataAccess.Poco;
using SimpleDB;

namespace RatMQ.Service.DataAccess.SimpleDB
{
    public class SimpleDBDataContext : DataContext
    {
        private IDBEngine _engine;

        public override void Init(DataContextInitParams initParams)
        {
            var builder = DBEngineBuilder.Make();

            builder.DatabaseFilePath(initParams.DatabaseFilPath);

            builder.Map<MessagePoco>()
                .PrimaryKey(x => x.Id)
                .Field(0, x => x.QueueName)
                .Field(1, x => x.Headers)
                .Field(2, x => x.Body);

            builder.Map<CommitedMessagePoco>()
                .PrimaryKey(x => x.MessageId);

            _engine = builder.BuildEngine();

            AddRepository(new Repository<MessagePoco>(_engine));
            AddRepository(new Repository<CommitedMessagePoco>(_engine));
        }
    }
}
