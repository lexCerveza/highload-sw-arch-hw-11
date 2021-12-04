#! /bin/bash

for i in {1..20000}; do redis-cli -h redis-primary set $i "Value for key $i" > /dev/null; done
redis-cli -h redis-primary info keyspace