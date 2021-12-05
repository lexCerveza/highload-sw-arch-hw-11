# highload-sw-arch-hw-11

# Prerequisites
* docker
* linux + bash

# 1. Run Redis cluster
```
./run.sh
```

# 2. Observe keyspace info after 20k kvp without ttl inserted

```
docker-compose logs redis-data-filler
```

# 3. Comparison of different maxmemory-policy with 1mb maxmemory and 20k kvp without ttl inserted

| Policy | Keys count after insertion | Errors
| :----: |:--------------------------:| -----:|
| volatile-lru | 2224 | OOM command not allowed when used memory > 'maxmemory' |
| allkeys-lru | 2225 | no errors |
| volatile-lfu | 2225 | OOM command not allowed when used memory > 'maxmemory' |
| allkeys-lfu | 1575 | no errors |
| volatile-random | 2224 | OOM command not allowed when used memory > 'maxmemory' |
| allkeys-random | 1484 | no errors |
| volatile-ttl | 2225 | OOM command not allowed when used memory > 'maxmemory' |
| noeviction | 2225 | OOM command not allowed when used memory > 'maxmemory' |  

# Note : if kvp is set (without ttl), for volatile policies error `OOM command not allowed` will occur. If use setex (with ttl) error is not reproduced

# 4. Redis client wrapper with cache stampede prevention is located in `./redis-client-wrapper/RedisClientWrapper.cs`

# 5. Cleanup
```
./cleanup.sh
```