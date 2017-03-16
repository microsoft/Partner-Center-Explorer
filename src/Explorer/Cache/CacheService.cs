// -----------------------------------------------------------------------
// <copyright file="CacheService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Cache
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Logic;
    using Newtonsoft.Json;
    using Security;
    using StackExchange.Redis;

    /// <summary>
    /// Provides quick access to frequently utilized resources.
    /// </summary>
    public class CacheService : ICacheService
    {
        /// <summary>
        /// Provides the ability to protect data.
        /// </summary>
        private readonly IDataProtector protector;

        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private readonly IExplorerService service;

        /// <summary>
        /// Provides the ability to interact with an instance of Redis Cache.
        /// </summary>
        private IConnectionMultiplexer connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public CacheService(IExplorerService service)
        {
            service.AssertNotNull(nameof(service));

            this.protector = new MachineKeyDataProtector(new[] { typeof(CacheService).FullName });
            this.service = service;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.c</param>
        /// <param name="connection">Connection to utilized to communicate with the Redis Cache instance.</param>
        /// <param name="dataProtector">Provides protection for data being cached.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// or
        /// <paramref name="connection"/> is null.
        /// or
        /// <paramref name="dataProtector"/> is null.
        /// </exception>
        public CacheService(IExplorerService service, IConnectionMultiplexer connection, IDataProtector dataProtector)
        {
            service.AssertNotNull(nameof(service));
            connection.AssertNotNull(nameof(connection));
            dataProtector.AssertNotNull(nameof(dataProtector));

            this.connection = connection;
            this.protector = dataProtector;
            this.service = service;
        }

        /// <summary>
        /// Gets a value indicating whether caching is enabled.
        /// </summary>
        public bool IsEnabled => !string.IsNullOrEmpty(this.service.Configuration.RedisCacheConnectionString);

        /// <summary>
        /// Removes all entities from the specified cache database. 
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task ClearAsync(CacheDatabaseType database)
        {
            EndPoint[] endpoints;
            IServer server;

            try
            {
                if (!this.IsEnabled)
                {
                    return;
                }

                if (this.connection == null)
                {
                    this.connection = await ConnectionMultiplexer.ConnectAsync(
                        this.service.Configuration.RedisCacheConnectionString);
                }

                endpoints = this.connection.GetEndPoints(true);

                foreach (EndPoint ep in endpoints)
                {
                    server = this.connection.GetServer(ep);
                    await server.FlushDatabaseAsync((int)database);
                }
            }
            finally
            {
                endpoints = null;
                server = null;
            }
        }

        /// <summary>
        /// Deletes the entity with specified key.
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <param name="key">The key of the entity to be deleted.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> is empty or null.
        /// </exception>
        public async Task DeleteAsync(CacheDatabaseType database, string key = null)
        {
            key.AssertNotEmpty(nameof(key));

            if (!this.IsEnabled)
            {
                return;
            }

            IDatabase cache = this.GetCacheReference(database);

            await cache.KeyDeleteAsync(key);
        }

        /// <summary>
        /// Fetches the specified entity from the cache.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity in cache.</typeparam>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <returns>
        /// The entity associated with the specified key.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> is empty or null.
        /// </exception>
        public TEntity Fetch<TEntity>(CacheDatabaseType database, string key) where TEntity : class
        {
            key.AssertNotEmpty(nameof(key));

            if (!this.IsEnabled)
            {
                return null;
            }

            IDatabase cache = this.GetCacheReference(database);
            RedisValue value = cache.StringGet(key);

            return value.HasValue ?
                JsonConvert.DeserializeObject<TEntity>(this.protector.Unprotect(value)) : null;
        }

        /// <summary>
        /// Fetches the specified entity from the cache.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity in cache.</typeparam>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <returns>
        /// The entity associated with the specified key.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> is empty or null.
        /// </exception>
        public async ValueTask<TEntity> FetchAsync<TEntity>(CacheDatabaseType database, string key) where TEntity : class
        {
            key.AssertNotEmpty(nameof(key));

            if (!this.IsEnabled)
            {
                return null;
            }

            IDatabase cache = this.GetCacheReference(database);
            RedisValue value = await cache.StringGetAsync(key);

            return value.HasValue ? JsonConvert.DeserializeObject<TEntity>(this.protector.Unprotect(value)) : null;
        }

        /// <summary>
        /// Stores the specified entity in the cache.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity in cache.</typeparam>
        /// <param name="database">Cache database type where the data should be stored.</param>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="entity">The object to be cached.</param>
        /// <param name="expiration">When the entity in the cache should expire.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException"> 
        /// entity
        /// </exception>
        public async Task StoreAsync<TEntity>(CacheDatabaseType database, string key, TEntity entity, TimeSpan? expiration = null)
            where TEntity : class
        {
            key.AssertNotEmpty(nameof(key));
            entity.AssertNotNull(nameof(entity));

            if (!this.IsEnabled)
            {
                return;
            }

            IDatabase cache = await this.GetCacheReferenceAsync(database);
            await cache.StringSetAsync(
                key, this.protector.Protect(JsonConvert.SerializeObject(entity)), expiration);
        }

        /// <summary>
        /// Removes all entities from the specified cache database. 
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        public void Clear(CacheDatabaseType database)
        {
            EndPoint[] endpoints;
            IServer server;

            try
            {
                if (!this.IsEnabled)
                {
                    return;
                }

                if (this.connection == null)
                {
                    this.connection = ConnectionMultiplexer.Connect(
                        this.service.Configuration.RedisCacheConnectionString);
                }

                endpoints = this.connection.GetEndPoints(true);

                foreach (EndPoint ep in endpoints)
                {
                    server = this.connection.GetServer(ep);
                    server.FlushDatabase((int)database);
                }
            }
            finally
            {
                endpoints = null;
                server = null;
            }
        }

        /// <summary>
        /// Deletes the entity with specified key.
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <param name="key">The key of the entity to be deleted.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> is empty or null.
        /// </exception>
        public void Delete(CacheDatabaseType database, string key)
        {
            key.AssertNotEmpty(nameof(key));

            if (!this.IsEnabled)
            {
                return;
            }

            IDatabase cache = this.GetCacheReference(database);
            cache.KeyDelete(key);
        }

        /// <summary>
        /// Stores the specified entity in the cache.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity in cache.</typeparam>
        /// <param name="cacheDatabase">Cache database type where the data should be stored.</param>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="entity">The object to be cached.</param>
        /// <param name="expiration">When the entity in the cache should expire.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException"> 
        /// entity
        /// </exception>
        public void Store<TEntity>(CacheDatabaseType cacheDatabase, string key, TEntity entity, TimeSpan? expiration = null)
            where TEntity : class
        {
            key.AssertNotEmpty(nameof(key));
            entity.AssertNotNull(nameof(entity));

            if (!this.IsEnabled)
            {
                return;
            }

            IDatabase cache = this.GetCacheReference(cacheDatabase);
            cache.StringSet(
                key, this.protector.Protect(JsonConvert.SerializeObject(entity)), expiration);
        }

        /// <summary>
        /// Obtains a reference to the specified cache database.
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <returns>A reference to the appropriate cache database.</returns>
        private IDatabase GetCacheReference(CacheDatabaseType database)
        {
            if (this.connection != null)
            {
                return this.connection.GetDatabase((int)database);
            }

            try
            {
                this.connection = ConnectionMultiplexer.Connect(
                    this.service.Configuration.RedisCacheConnectionString);

                return this.connection.GetDatabase((int)database);
            }
            catch (RedisConnectionException ex)
            {
                throw new CacheException("Failed to connect to the instance of Redis Cache.", ex);
            }
        }

        /// <summary>
        /// Obtains a reference to the specified cache database.
        /// </summary>
        /// <param name="database">Cache database type where the data is stored.</param>
        /// <returns>A reference to the appropriate cache database.</returns>
        private async Task<IDatabase> GetCacheReferenceAsync(CacheDatabaseType database)
        {
            if (this.connection != null)
            {
                return this.connection.GetDatabase((int)database);
            }

            try
            {
                this.connection = await ConnectionMultiplexer.ConnectAsync(
                    this.service.Configuration.RedisCacheConnectionString);

                return this.connection.GetDatabase((int)database);
            }
            catch (RedisConnectionException ex)
            {
                throw new CacheException("Failed to connect to the instance of Redis Cache.", ex);
            }
        }
    }
}