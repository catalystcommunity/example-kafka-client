using Confluent.Kafka;
using System.Net;

public class MyKafkaProducer
{    public ProducerConfig Config { get; set; }

    public MyKafkaProducer() // Empty constructor
    {
        Config = new ProducerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("MYKAFKA_BOOTSTRAP_SERVERS"),
            ClientId = Dns.GetHostName(),
            SecurityProtocol = SecurityProtocol.Ssl,
            SslCertificateLocation = Environment.GetEnvironmentVariable("MYKAFKA_TLSCERT_PATH"),
            SslKeyLocation = Environment.GetEnvironmentVariable("MYKAFKA_TLSKEY_PATH"),
            SslCaLocation = Environment.GetEnvironmentVariable("MYKAFKA_CACRT_PATH"),
            EnableSslCertificateVerification = false,
        };
    }

    public MyKafkaProducer(string bootStrapServers, string ClientId, string SslCertificateLocation, string SslKeyLocation, string SslCaLocation)
    {
        Config = new ProducerConfig
        {
            BootstrapServers = bootStrapServers,
            ClientId = ClientId,
            SslCertificateLocation = SslCertificateLocation,
            SecurityProtocol = SecurityProtocol.Ssl,
            SslKeyLocation = SslKeyLocation,
            SslCaLocation = SslCaLocation,
            EnableSslCertificateVerification = false,
        };
    }
    
    public void ProduceMessages() {
        using (var producer = new ProducerBuilder<Null, string>(Config).Build())
        {
            Console.WriteLine("I'm in a producer!");
        };
    }
}
