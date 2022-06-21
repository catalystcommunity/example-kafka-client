using Confluent.Kafka;
using System.Net;

public class MyKafkaConsumer
{
    public ConsumerConfig Config { get; set; }

    public MyKafkaConsumer() // Empty constructor
    {
        bool outOffsetReset;
        if (!Boolean.TryParse(Environment.GetEnvironmentVariable("MYKAFKA_OFFSET_RESET"), out outOffsetReset)) {
            outOffsetReset = true;
        }
        Config = new ConsumerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("MYKAFKA_BOOTSTRAP_SERVERS"),
            GroupId = Environment.GetEnvironmentVariable("MYKAFKA_GROUPID"),
            AutoOffsetReset = outOffsetReset ? AutoOffsetReset.Earliest : AutoOffsetReset.Latest,
            SslCertificateLocation = Environment.GetEnvironmentVariable("MYKAFKA_TLSCERT_PATH"),
            SslKeyLocation = Environment.GetEnvironmentVariable("MYKAFKA_TLSKEY_PATH"),
            SslCaLocation = Environment.GetEnvironmentVariable("MYKAFKA_CACRT_PATH"),
            EnableSslCertificateVerification = false,
        };
    }

    public MyKafkaConsumer(string bootStrapServers, string groupId, bool offsetReset, string SslCertificateLocation, string SslKeyLocation, string SslCaLocation)
    {
        Config = new ConsumerConfig
        {
            BootstrapServers = bootStrapServers,
            GroupId = groupId,
            AutoOffsetReset = offsetReset ? AutoOffsetReset.Earliest : AutoOffsetReset.Latest,
            SslCertificateLocation = SslCertificateLocation,
            SslKeyLocation = SslKeyLocation,
            SslCaLocation = SslCaLocation,
            EnableSslCertificateVerification = false,
        };
    }
    
    public void ConsumeMessages() {

        using (var consumer = new ConsumerBuilder<Ignore, string>(Config).Build())
        {
            Console.WriteLine("I'm in a consumer!");
        }
    }
}
