using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace MoneyManagerNotificationServiceAPI.Helpers
{
    public static class KafkaTopics
    {
        public static async Task EnsureExistsAsync(string bootstrapServers, string topic)
        {
            var adminConfig = new AdminClientConfig { BootstrapServers = bootstrapServers };
            using var admin = new AdminClientBuilder(adminConfig).Build();

            try
            {
                var spec = new TopicSpecification
                {
                    Name = topic,
                    NumPartitions = 3,
                    ReplicationFactor = 3,
                    Configs = new Dictionary<string, string>
                    {
                        ["min.insync.replicas"] = "2"
                    }
                };

                await admin.CreateTopicsAsync([spec]);
            }
            catch (CreateTopicsException ex)
            {
                if (ex.Results.Count == 0 || ex.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                {
                    throw;
                }
            }
        }
    }
}
