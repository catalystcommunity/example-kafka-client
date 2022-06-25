package main

import (
	"fmt"
	"os"
	"time"

	"github.com/confluentinc/confluent-kafka-go/kafka"
)

// KafkaProducerExample implements a kafka producer and sends example messages
// to the configured topic
func KafkaProducerExample(cfg KafkaConfig) {
	p, err := kafka.NewProducer(&kafka.ConfigMap{
		"bootstrap.servers":                   cfg.BootstrapServers,
		"security.protocol":                   cfg.SecurityProtocol,
		"ssl.ca.location":                     cfg.CaCertPath,
		"ssl.certificate.location":            cfg.TLSCertPath,
		"ssl.key.location":                    cfg.TLSKeyPath,
		"enable.ssl.certificate.verification": cfg.EnableSSLCerterification,
	})
	if err != nil {
		fmt.Printf("Failed to create producer: %s\n", err)
		os.Exit(1)
	}

	fmt.Println("connected to kafka as producer")

	defer p.Close()

	for i := 0; i < cfg.MessageCount; i++ {
		value := fmt.Sprintf("test producer message %d", i)

		err = p.Produce(&kafka.Message{
			TopicPartition: kafka.TopicPartition{Topic: &cfg.Topic, Partition: kafka.PartitionAny},
			Value:          []byte(value),
		}, nil)
		if err != nil {
			if err.(kafka.Error).Code() == kafka.ErrQueueFull {
				// Producer queue is full, wait 1s for messages
				// to be delivered then try again.
				time.Sleep(time.Second)
				continue
			}
			fmt.Printf("Failed to produce message: %v\n", err)
		}
	}

	// wait for all messages to send
	p.Flush(1000)

	fmt.Printf("producer sent %d messages\n", cfg.MessageCount)
}
