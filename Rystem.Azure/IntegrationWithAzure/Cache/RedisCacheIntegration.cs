﻿using Rystem.Concurrency;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Azure.IntegrationWithAzure.Cache
{
    internal class RedisCacheIntegration
    {
        private int RoundRobin = -1;
        private readonly int NumberOfClients = 1;
        private IDatabase Cache
        {
            get
            {
                int value = 0;
                if (this.NumberOfClients > 1)
                    value = this.RoundRobin = (this.RoundRobin + 1) % this.NumberOfClients;
                return Connections[value].Value.GetDatabase();
            }
        }
        public Task WarmUp()
        {
            foreach (var connection in Connections)
                _ = connection.Value.GetDatabase();
            return Task.CompletedTask;
        }

        private readonly List<Lazy<ConnectionMultiplexer>> Connections;
        private readonly TimeSpan ExpireDefault;
        private readonly string ConnectionString;
        internal RedisCacheIntegration(string connectionString, TimeSpan expiringDefault = default, int numberOfClients = 1)
        {
            this.NumberOfClients = numberOfClients;
            this.ConnectionString = connectionString;
            this.ExpireDefault = expiringDefault;
            Connections = new List<Lazy<ConnectionMultiplexer>>();
            for (int i = 0; i < NumberOfClients; i++)
                Connections.Add(new Lazy<ConnectionMultiplexer>(() =>
                {
                    ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(ConnectionString);
                    return connectionMultiplexer;
                }));
        }
        public async Task<string> InstanceAsync(string key)
        {
            string value = await Cache.StringGetAsync(key).NoContext();
            return value;
        }
        public async Task<bool> UpdateAsync(string key, string value, TimeSpan expiringTime = default)
        {
            bool code;
            if (expiringTime == default)
                expiringTime = ExpireDefault;
            if (expiringTime.Ticks > 0)
                code = await Cache.StringSetAsync(key, value, expiringTime).NoContext();
            else
                code = await Cache.StringSetAsync(key, value).NoContext();
            return code;
        }
        public async Task<bool> ExistsAsync(string key)
            => await Cache.KeyExistsAsync(key).NoContext();
        public async Task<bool> DeleteAsync(string key)
            => await Cache.KeyDeleteAsync(key).NoContext();
        public async IAsyncEnumerable<string> ListKeysAsync(string prefix)
        {
            await foreach (string redisKey in Cache.Multiplexer.GetServer(Cache.Multiplexer.GetEndPoints().First()).KeysAsync())
                if (redisKey.Contains(prefix))
                    yield return redisKey.Replace(prefix, string.Empty);
        }
        private readonly ConcurrentDictionary<string, string> TokenAcquireds = new();
        public async Task<bool> AcquireLockAsync(string key, TimeSpan expiringTime = default)
        {
            try
            {
                if (expiringTime == default)
                    expiringTime = TimeSpan.FromSeconds(10);
                RaceConditionResponse response = await RaceCondition.RunAsync(async () =>
                {
                    await Cache.LockTakeAsync(new RedisKey(key), new RedisValue(string.Empty), expiringTime);
                }, key).NoContext();
                return response.IsExecuted && !response.InException;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> LockIsAcquiredAsync(string key)
        {
#warning AR - To check how it really works
            return (await Cache.LockQueryAsync(new RedisKey(key))).HasValue;
        }
        public async Task<bool> ReleaseLockAsync(string key)
        {
            await RaceCondition.RunAsync(async () =>
            {
                await Cache.LockReleaseAsync(new RedisKey(key), new RedisValue(string.Empty));
            }, key).NoContext();
            return true;
        }
    }
}