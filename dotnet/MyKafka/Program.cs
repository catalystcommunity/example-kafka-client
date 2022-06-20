
public class Example
{
   public static int Main()
   {
        // First get all the env vars we need
        bool varsSet = true;
        string missingVars = "";
        string? boostrapServers = Environment.GetEnvironmentVariable("MYKAFKA_BOOTSTRAP_SERVERS");
        long messageNum = -1;
        Int64.TryParse(Environment.GetEnvironmentVariable("MYKAFKA_MESSAGE_NUM"), out messageNum);
        string? sslCertificateLocation = Environment.GetEnvironmentVariable("MYKAFKA_TLSCERT_PATH");
        string? sslKeyLocation = Environment.GetEnvironmentVariable("MYKAFKA_TLSKEY_PATH");
        string? sslCaLocation = Environment.GetEnvironmentVariable("MYKAFKA_CACRT_PATH");

        // Then validate we got them all
        if (boostrapServers == null) {
            varsSet = false;
            missingVars += "MYKAFKA_BOOTSTRAP_SERVERS\n";
        }
        if (messageNum == -1) {
            varsSet = false;
            missingVars += "MYKAFKA_MESSAGE_NUM\n";
        }
        if (sslCertificateLocation == null) {
            varsSet = false;
            missingVars += "MYKAFKA_TLSCERT_PATH\n";
        }
        if (sslKeyLocation == null) {
            varsSet = false;
            missingVars += "MYKAFKA_TLSKEY_PATH\n";
        }
        if (sslCaLocation == null) {
            varsSet = false;
            missingVars += "MYKAFKA_CACRT_PATH\n";
        }


        if (varsSet) {
            MyKafkaProducer producer = new MyKafkaProducer();
            producer.ProduceMessages();
            MyKafkaConsumer consumer = new MyKafkaConsumer();
            consumer.ConsumeMessages();
        } else {
            Console.WriteLine($"Missing Env Vars:\n{missingVars}");
            return -1;
        }
        return 0;
   }
}
