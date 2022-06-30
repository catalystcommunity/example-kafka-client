#!/usr/bin/env bash

# fail script if any command fails
set -e

STRIMZI_VERSION=${STRIMZI_VERSION:-"0.29.0"}
KAFKA_VERSION=${KAFKA_VERSION:-"3.2.0"}
KAFKA_CLUSTER_NAME=${KAFKA_CLUSTER_NAME:-"mykafkacluster"}
KAFKA_NAMESPACE=${KAFKA_NAMESPACE:-"mykafka"}

cat <<EOF | kubectl apply -f -
apiVersion: v1
kind: Pod
metadata:
  name: kafkashell
  namespace: ${KAFKA_NAMESPACE}
spec:
  containers:
  - name: kafkashell
    image: quay.io/strimzi/kafka:${STRIMZI_VERSION}-kafka-${KAFKA_VERSION}
    command: [ "/bin/bash", "-c", "--" ]
    args: [ "while true; do sleep 30; done;" ]
    livenessProbe:
      exec:
        command:
        - echo
        - "0"
      initialDelaySeconds: 5
      periodSeconds: 5
    readinessProbe:
      exec:
        command:
        - echo
        - "0"
      initialDelaySeconds: 5
      periodSeconds: 5
    volumeMounts:
    - name: kafkauser
      mountPath: "/opt/kafka/usertls"
      readOnly: true
    - name: kafkaclusterca
      mountPath: "/opt/kafka/clustertls"
      readOnly: true
  volumes:
  - name: kafkauser
    secret:
      secretName: sreuser
  - name: kafkaclusterca
    secret:
      secretName: ${KAFKA_CLUSTER_NAME}-cluster-ca-cert
EOF

echo "Waiting for the pod to create"
kubectl wait --namespace mykafka --for=condition=ready pod --timeout=-1s kafkashell
kubectl exec --namespace mykafka -it kafkashell -- bash -c "cd /opt/kafka/clustertls; keytool -keystore /tmp/truststore.jks -storepass tutpass -noprompt -alias cluster-ca -import -file ca.crt -storetype PKCS12"
kubectl exec --namespace mykafka -it kafkashell -- bash -c "cd /opt/kafka/usertls; openssl pkcs12 -export -in user.crt -inkey user.key -name custom-key -password pass:tutpass -out /tmp/sreuser.p12"
kubectl exec --namespace mykafka -it kafkashell -- bash -c "printf \"security.protocol=SSL\nssl.truststore.location=/tmp/truststore.jks\nssl.truststore.password=tutpass\nssl.keystore.location=/tmp/sreuser.p12\nssl.keystore.password=tutpass\n\" > /tmp/client.properties"


echo "Now try something referencing the keystore like:"
echo "bin/kafka-topics.sh --command-config /tmp/client.properties --list --bootstrap-server ${KAFKA_CLUSTER_NAME}-kafka-0.${KAFKA_CLUSTER_NAME}-kafka-brokers.${KAFKA_NAMESPACE}.svc:9093"
kubectl exec --namespace ${KAFKA_NAMESPACE} -it kafkashell -- bash

kubectl delete pod -n ${KAFKA_NAMESPACE} kafkashell
