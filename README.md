# example-kafka-client

Examples of kafka clients (Producers and Consumers) for various tech stacks, for use with Kafka tutorials

Spec of what each client codebase should be capable of coming when that's been settled.

# Setup Strimzi

If you don't already have a kubernete cluster, I suggest creating a local one using [KinD](https://kind.sigs.k8s.io/docs/user/quick-start/) or [K3S](https://rancher.com/docs/k3s/latest/en/installation/)

Once you have a cluster, install the Strimzi Operator to it using the following:

```bash
helm repo add strimzi https://strimzi.io/charts/
helm install strimzi/strimzi-kafka-operator --name strimzi-ops --namespace mykafka -f https://raw.githubusercontent.com/catalystsquad/example-kafka-client/main/strimzi_operator_values.yaml
```

You can of course use any values file you wish, or copy that and adjust it.

Everything you need to create a kafka cluster is now accessible via `kubectl apply -f myKafka_cluster.yaml`

This will create a production-like 3 broker and 3 zookeeper node kafka cluster. It will require about 12GB of RAM in your K8s cluster, so configure appropriately. If you are low on resources for this on your local machine, you can attempt to adjust resources lower. Keep in mind that Confluents suggestions for [minimum requirements](https://docs.confluent.io/platform/current/installation/system-requirements.html) are 64GB RAM per broker and 24 CPUs, so this is a heavy system to operate.

You can also reduce the number of replicas, but this will be less useful for tutorial use since you're missing all the distributed pieces to experiment with.
