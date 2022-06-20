#!/bin/bash

OLD_PWD="$(pwd)"

cd "${0%/*}"
cd ./MyKafka

skaffold 

cd -
cd "${OLD_PWD}"