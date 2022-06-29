using Confluent.Kafka;
using System.Net;

public class MyKafkaProducer
{
    public ProducerConfig Config { get; set; }
    public long MessageNum { get; set; }
    public int ClientDelay { get; set; }
    public string KafkaTopic { get; set; }

    public MyKafkaProducer(string bootStrapServers, string ClientId, string SslCertificateLocation, string SslKeyLocation, string SslCaLocation, long messageNum = 20, string kafkaTopic = "tuttopic", int clientDelay = 0)
    {
        Config = new ProducerConfig
        {
            BootstrapServers = bootStrapServers,
            ClientId = ClientId,
            SslCertificateLocation = SslCertificateLocation,
            SecurityProtocol = SecurityProtocol.Ssl,
            SslKeyLocation = SslKeyLocation,
            SslCaLocation = SslCaLocation,
            EnableSslCertificateVerification = true,
            Partitioner = Partitioner.Random,
            MaxInFlight = 1,
        };
        MessageNum = messageNum;
        KafkaTopic = kafkaTopic;
        ClientDelay = clientDelay;
    }
    
    public void ProduceMessages() {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var randChars = new char[8];
        var random = new Random();
        string stringChars = "";

        using (var producer = new ProducerBuilder<string, string>(Config).Build())
        {
            Console.WriteLine("I'm in a producer!");
            for (int i = 0; i < MessageNum; i++) {
                for (int a = 0; a < randChars.Length; a++)
                {
                    randChars[a] = chars[random.Next(chars.Length)];
                }
                stringChars = new string (randChars);
                Console.WriteLine($"Producing a message with these chars: {stringChars}");
                producer.Poll(TimeSpan.Zero);
                producer.Produce(KafkaTopic, new Message<string, string> {
                    // Keys are hashed to determine partition, but you probably don't need to worry about this
                    // unless you need ordering on a keyspace, but that destroys throughput
                    // so typically you want to do all this with <Null, string> for ProducerBuilder and Message
                    // and just don't specify the Key at all
                    Key=$"{stringChars}",
                    Value=$@"{{ ""msg"":: ""This is a message:: {stringChars}"" }}"
                    });
                if (i % 100 == 0) {
                    // This avoids filling the local queue too fast if producing a huge number of messages
                    producer.Flush();
                }
                if (ClientDelay > 0) {
                    System.Threading.Thread.Sleep(1000 * ClientDelay);
                }
            }
            producer.Flush();
            Console.WriteLine("I've in theory produced!");
        };
    }
}
