# highload-sw-arch-hw-11

# Prerequisites
* docker
* linux + bash

# 1. Run Redis cluster
```
./run.sh
```

# 2. Observe keyspace info after 20k kvp inserted

```
docker-compose logs redis-data-filler
```

# 3. Comparison of different maxmemory-policy with 1mb maxmemory and 20k kvp inserted

| Policy | Keys count after insertion
| ------ |:---------:|
| volatile-lru | |
| allkeys-lru | |
| volatile-lfu | |
| allkeys-lfu | |
| volatile-random | |
| allkeys-random | |
| volatile-ttl | |
| noeviction | |