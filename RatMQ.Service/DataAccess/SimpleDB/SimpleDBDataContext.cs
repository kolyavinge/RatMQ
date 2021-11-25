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

            builder.WorkingDirectory(initParams.DatabasePath);

            builder.Map<MessagePoco>()
                .Name("messages")
                .PrimaryKey(x => x.Id)
                .Field(0, x => x.QueueName)
                .Field(1, x => x.Headers)
                .Field(2, x => x.Body);

            builder.Map<CommitedMessagePoco>()
                .Name("commited_messages")
                .PrimaryKey(x => x.MessageId);

            _engine = builder.BuildEngine();

            AddRepository(new Repository<MessagePoco>(_engine));
            AddRepository(new Repository<CommitedMessagePoco>(_engine));
        }
    }
}
