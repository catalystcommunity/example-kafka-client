#!/usr/bin/env bash

SOURCE="${BASH_SOURCE[0]:-$0}";
while [ -L "$SOURCE" ]; do # resolve $SOURCE until the file is no longer a symlink
  THIS_DIR="$( cd -P "$( dirname -- "$SOURCE"; )" &> /dev/null && pwd 2> /dev/null; )";
  SOURCE="$( readlink -- "$SOURCE"; )";
  [[ $SOURCE != /* ]] && SOURCE="${THIS_DIR}/${SOURCE}"; # if $SOURCE was a relative symlink, we need to resolve it relative to the path where the symlink file was located
done
THIS_DIR="$( cd -P "$( dirname -- "$SOURCE"; )" &> /dev/null && pwd 2> /dev/null; )";

export MYKAFKA_BOOTSTRAP_SERVERS="127.0.0.1:32104"
export MYKAFKA_TLSCERT_PATH="${THIS_DIR}/tutuser.crt"
export MYKAFKA_TLSKEY_PATH="${THIS_DIR}/tutuser.key"
export MYKAFKA_CACRT_PATH="${THIS_DIR}/kafkaca.crt"

