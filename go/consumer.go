package main

import (
	"fmt"
	"os"
	"os/signal"
	"syscall"

	"github.com/confluentinc/confluent-kafka-go/kafka"
	"github.com/samber/lo"
)

// KafkaConsumerExample implements a kafka consumer and recieves messages until
// it has recieved the configured message count
func KafkaConsumerExample(cfg KafkaConfig) {
	sigchan := make(chan os.Signal, 1)
	signal.Notify(sigchan, syscall.SIGINT, syscall.SIGTERM)

	c, err := kafka.NewConsumer(&kafka.ConfigMap{
		"bootstrap.servers":                   cfg.BootstrapServers,
		"auto.offset.reset":                   lo.Ternary(cfg.OffsetReset, "earliest", "latest"),
		"group.id":                            cfg.GroupID,
		"security.protocol":                   cfg.SecurityProtocol,
		"ssl.ca.location":                     cfg.CaCertPath,
		"ssl.certificate.location":            cfg.TLSCertPath,
		"ssl.key.location":                    cfg.TLSKeyPath,
		"enable.ssl.certificate.verification": cfg.EnableSSLCerterification,
	})
	if err != nil {
		fmt.Printf("Failed to create consumer: %s\n", err)
		os.Exit(1)
	}

	fmt.Println("connected to kafka as consumer")

	defer c.Close()

	err = c.SubscribeTopics([]string{cfg.Topic}, nil)
	if err != nil {
		fmt.Printf("Failed to subscribe to topic %s: %s\n", cfg.Topic, err)
	}

	messagesRecieved := 0
	run := true

	for run {
		select {
		case sig := <-sigchan:
			fmt.Printf("Caught signal %v: terminating\n", sig)
			run = false
		default:
			msg, err := c.ReadMessage(-1)
			if err == nil {
				fmt.Printf("recieved message (%s): %s\n", msg.TopicPartition, string(msg.Value))

				// exit when we've recieved all of the messages that we expect
				messagesRecieved++
				if messagesRecieved >= cfg.MessageCount {
					run = false
				}
			} else {
				// The client will automatically try to recover from all errors.
				fmt.Printf("Consumer error: %v (%v)\n", err, msg)
			}
		}
	}
}
