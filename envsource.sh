#!/bin/bash

export MYKAFKA_BOOTSTRAP_SERVERS="localhost:9092"
export MYKAFKA_MESSAGE_NUM=20
export MYKAFKA_TLSCERT_PATH="./myclient.crt"
export MYKAFKA_TLSKEY_PATH="./myclient.key"
export MYKAFKA_CACRT_PATH="./myclientca.crt"
