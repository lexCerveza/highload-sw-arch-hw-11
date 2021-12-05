using System;
using System.Threading.Tasks;
using Moq;
using Projectr.RedisClientWrapper;
using StackExchange.Redis;

const int totalFetches = 100;
int taskDelay = (int)TimeSpan.FromMilliseconds(100).TotalMilliseconds;
TimeSpan cacheTime = TimeSpan.FromMilliseconds(200);

int callbackCount = 0;

var connectionMultiplexer = ConnectionMultiplexer.Connect(
    new ConfigurationOptions
    {
        AbortOnConnectFail = false,
        EndPoints = { { "127.0.0.1", 6379 } },
    });

var storage = new Mock<IStorage>();

storage.Setup(m => m.GetAsync<TestObject>(It.IsAny<int>())).ReturnsAsync((int i) =>
{
    Task.Delay(taskDelay).Wait();
    callbackCount++;
    return new TestObject(i);
});

var cache = new RedisClientWrapper(connectionMultiplexer, storage.Object);

for (int i = 0; i < totalFetches; i++)
{
    await cache.GetAsync<TestObject>(i, "Test Cache Key 3", cacheTime);
}

Console.WriteLine("Total Fetches: {0}, Total Callbacks: {1}", totalFetches, callbackCount);

public record TestObject(int id);