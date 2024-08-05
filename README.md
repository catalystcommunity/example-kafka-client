# example-kafka-client

Examples of kafka clients (Producers and Consumers) for various tech stacks, for use with Kafka tutorials. This is using a KinD cluster with Strimzi Operator installed, but you could replace it with any cluster

Spec of what each client codebase should be capable of coming when that's been settled.

## Setup Strimzi

If you don't already have a kubernetes cluster, we suggest creating a local one using [KinD](https://kind.sigs.k8s.io/docs/user/quick-start/) or [K3S](https://rancher.com/docs/k3s/latest/en/installation/) and we have an example [kind_cluster.yaml](kind_cluster.yaml) with extra ports for accessing the Kafka brokers from external to the kubernetes cluster, and you can use it like thus:

```bash
kind create cluster --name strimzi --config https://raw.githubusercontent.com/catalystcommunity/example-kafka-client/main/kind_cluster_config.yaml
```

Once you have a cluster, install the Strimzi Operator to it using the following:

```bash
helm repo add strimzi https://strimzi.io/charts/
helm install strimzi-ops strimzi/strimzi-kafka-operator --namespace mykafka --create-namespace -f https://raw.githubusercontent.com/catalystcommunity/example-kafka-client/main/strimzi_operator_values.yaml
```

You can of course use any values file you wish, or copy that as a starting point and adjust it.

Everything you need to create a kafka cluster is now accessible via:

```bash
kubectl apply -f https://raw.githubusercontent.com/catalystcommunity/example-kafka-client/main/myKafka_cluster.yaml
```

This will create a production-like 3 broker and 3 zookeeper node kafka cluster. It will request about 12GB of RAM in your K8s cluster, so configure appropriately. If you are low on resources for this on your local machine, you can attempt to adjust resources lower. Keep in mind that Confluent's suggestions for [minimum requirements](https://docs.confluent.io/platform/current/installation/system-requirements.html) are 64GB RAM per broker and 24 CPUs, so this is a heavy system to operate.

You can also reduce the number of replicas, but this will be less useful for tutorial use since you're missing all the distributed pieces to experiment with.

## Download Certs (external)

In order to do anything with this external to the cluster, we'll need to get the user cert, key, and CA cert from the kubernetes cluster. If you are going to use a pod inside the kubernetes cluster or something like Telepresence, you can just mount these secrets as files in the pod.

To download, these are commands that should work if you are in the roo of the repo of this Readme:

```bash
kubectl get secret mykafkacluster-cluster-ca-cert -n mykafka -o json | jq -r '.data."ca.crt"' | base64 -d > kafkaca.crt
kubectl get secret tutuser -n mykafka -o json | jq -r '.data."user.crt"' | base64 -d > tutuser.crt
kubectl get secret tutuser -n mykafka -o json | jq -r '.data."user.key"' | base64 -d > tutuser.key
```

Move them around appropriately and make sure you update your environment vars according to where they have moved.

## Setup Keystore (optional)

We don't recommend setting this up, but it is useful if you want to play with clients (including the kafka cli scripts) that require java keystores and the like. This is largely untested, so be prepared to fiddle quite a bit for your specific instance of a kafka cluster. As an example, it is useful in how close it will be to whatever you need, but Kafka has never made TLS management or usage easy.

If we want to use kafka cli commands or similar like the [Strimzi CLI (kfk)](https://github.com/systemcraftsman/strimzi-kafka-cli#installation) (third party tool) we'll need to setup a keystore, because Java. The keytool command comes with a JDK, so install your favorite and do the following:

```bash
echo "yes" | keytool -import -trustcacerts -file kafkaca.crt -keystore truststore.jks -storepass tutpass
openssl pkcs12 -export -in tutuser.crt -inkey tutuser.key -name tutuser -password pass:tutpass -out tutuser.p12
```

And a client.properties config file for Kafka that looks like this:

```text
security.protocol=SSL
ssl.keystore.location=./tutuser.p12
ssl.keystore.password=tutpass
ssl.truststore.location=./truststore.jks
ssl.truststore.password=tutpass
```

## Environment

There is a shell script you can `source` in the root of this repo. `envsource.sh` should have sane defaults. You can override your own env vars. All clients should use these env vars for options on behavior and configuration. This is a cloud native approach, and not typically how Kafka itself is run, but in the Kubernetes world this makes a lot of composability easier.

Once sourced, you should be set to run clients in this repo as needed.

Env vars that are not included in that file which you may wish to set and manipulate as you play with the tools:

- MYKAFKA_MESSAGE_NUM : The number of messages to produce and/or consume, defaulting to 20
- MYKAFKA_TOPIC : The topic to work with producing and consuming, defaulting to "tuttopic"
- MYKAFKA_CLIENT_DELAY_SECONDS : The amount of seconds producers and consumers should wait in between messages, which is useful to watch logs with. This defaults to 0 for no delays.
- MYKAFKA_GROUPID : The group ID used for consuming, defaulting to "tutgroup". You should change this to tutgroup unless you have a different group control mechanism in mind.
- MYKAFKA_SKIP_PRODUCE : Skips producing if set to "true" exactly, defaults to "false" which does not skip producing
- MYKAFKA_SKIP_CONSUME : Skips consuming if set to "true" exactly, defaults to "false" which does not skip consuming

If you wish to have a parallel producing and consuming, rather than a synchronous production followed by consumption, we recommend setting up two terminals, one where you set MYKAFKA_SKIP_CONSUME to "true" and the other you set MYKAFKA_SKIP_PRODUCE to "true" and then you can run them independently of each other. You can do this multiple times, but you will want to change the MYKAFKA_GROUPID if you want consumers to act separately from each other.
