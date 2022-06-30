using Confluent.Kafka;
using System.Net;

public class MyKafkaConsumer
{
    public ConsumerConfig Config { get; set; }
    public long MessageNum { get; set; }
    public int ClientDelay { get; set; }
    public string KafkaTopic { get; set; }

    public MyKafkaConsumer(string bootStrapServers, string groupId, bool offsetReset, string SslCertificateLocation, string SslKeyLocation, string SslCaLocation, long messageNum = 20, string kafkaTopic = "tuttopic", int clientDelay = 0)
    {
        Config = new ConsumerConfig
        {
            BootstrapServers = bootStrapServers,
            GroupId = groupId,
            AutoOffsetReset = offsetReset ? AutoOffsetReset.Earliest : AutoOffsetReset.Latest,
            SecurityProtocol = SecurityProtocol.Ssl,
            SslCertificateLocation = SslCertificateLocation,
            SslKeyLocation = SslKeyLocation,
            SslCaLocation = SslCaLocation,
<<<<<<< HEAD
            EnableSslCertificateVerification = false,
=======
            EnableSslCertificateVerification = true,
>>>>>>> 72ff4c2316d20c29b663cbb2a775c7a6bb8d5f65
            MaxInFlight = 2,
        };
        MessageNum = messageNum;
        KafkaTopic = kafkaTopic;
        ClientDelay = clientDelay;
    }
    
    public void ConsumeMessages() {

        using (var consumer = new ConsumerBuilder<Ignore, string>(Config).Build())
        {
            Console.WriteLine("I'm in a consumer!");
            consumer.Subscribe(KafkaTopic);
            for (int i = 0; i < MessageNum; i++) {
                var consumeResult = consumer.Consume();
                Console.WriteLine($"I have consumed a message: {consumeResult.Message.Value} \nPartition: {consumeResult.Partition.Value} \nTimestamp: {consumeResult.Message.Timestamp.UtcDateTime}\n");
                if (ClientDelay > 0) {
                    System.Threading.Thread.Sleep(1000 * ClientDelay);
                }
            }
        }
    }
}
