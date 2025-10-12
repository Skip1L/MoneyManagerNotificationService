namespace MoneyManagerNotificationServiceAPI.Configs
{
    public class MessageBrokerConfig
    {
        public string KafkaHost { get; set; }
        public string RabbitMQHost { get; set; }
        public string RabbitMQPassword { get; set; }
        public string RabbitMQUsername { get; set; }
    }
}
