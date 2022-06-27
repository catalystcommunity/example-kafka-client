
using System.Net;

public class Example
{
    public static string GetEnvStringOrDefault(string envName, string envDefault = "") {
        string retval = Environment.GetEnvironmentVariable(envName) is string e && e.Length > 0 ? e : envDefault;
        return retval;
    }

    public static int Main()
    {
        // First get all the env vars we need
        string missingVars = "";
        bool varsSet = true;
        bool offsetReset;
        if (!Boolean.TryParse(Environment.GetEnvironmentVariable("MYKAFKA_OFFSET_RESET"), out offsetReset)) {
            offsetReset = true;
        }
        long messageNum = -1;
        Int64.TryParse(GetEnvStringOrDefault("MYKAFKA_MESSAGE_NUM", "20"), out messageNum);
        string bootstrapServers = GetEnvStringOrDefault("MYKAFKA_BOOTSTRAP_SERVERS");
        string sslCertificateLocation = GetEnvStringOrDefault("MYKAFKA_TLSCERT_PATH");
        string sslKeyLocation = GetEnvStringOrDefault("MYKAFKA_TLSKEY_PATH");
        string sslCaLocation = GetEnvStringOrDefault("MYKAFKA_CACRT_PATH");
        string kafkaTopic = GetEnvStringOrDefault("MYKAFKA_TOPIC", "tuttopic");
        int clientDelay = -1;
        Int32.TryParse(GetEnvStringOrDefault("MYKAFKA_CLIENT_DELAY_SECONDS", "0"), out clientDelay);
        string groupID = GetEnvStringOrDefault("MYKAFKA_GROUPID", "tutgroup");
        Console.WriteLine($"Cert related paths:\n{sslCertificateLocation}\n{sslKeyLocation}\n{sslCaLocation}");

        // Then validate we got them all
        if (bootstrapServers.Length <= 0) {
            varsSet = false;
            missingVars += "MYKAFKA_BOOTSTRAP_SERVERS\n";
        }
        if (messageNum == -1) {
            varsSet = false;
            missingVars += "MYKAFKA_MESSAGE_NUM\n";
        }
        if (sslCertificateLocation.Length <= 0) {
            varsSet = false;
            missingVars += "MYKAFKA_TLSCERT_PATH\n";
        }
        if (sslKeyLocation.Length <= 0) {
            varsSet = false;
            missingVars += "MYKAFKA_TLSKEY_PATH\n";
        }
        if (sslCaLocation.Length <= 0) {
            varsSet = false;
            missingVars += "MYKAFKA_CACRT_PATH\n";
        }


        if (varsSet) {
            string skipProduce = GetEnvStringOrDefault("MYKAFKA_SKIP_PRODUCE", "false");
            string skipConsume = GetEnvStringOrDefault("MYKAFKA_SKIP_CONSUME", "false");

            if (skipProduce == "false") {
                MyKafkaProducer producer = new MyKafkaProducer(bootstrapServers, groupID, sslCertificateLocation, sslKeyLocation, sslCaLocation, messageNum, kafkaTopic, clientDelay);
                producer.ProduceMessages();
            }
            if (skipConsume == "false") {
                MyKafkaConsumer consumer = new MyKafkaConsumer(bootstrapServers, groupID, offsetReset, sslCertificateLocation, sslKeyLocation, sslCaLocation, messageNum, kafkaTopic, clientDelay);
                consumer.ConsumeMessages();
            }
        } else {
            Console.WriteLine($"Missing Env Vars:\n{missingVars}");
            return -1;
        }
        return 0;
    }
}
