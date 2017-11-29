using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DSH.DiscordBot.Infrastructure.Configuration;
using DSH.DiscordBot.Infrastructure.Logging;
using DSH.DiscordBot.Infrastructure.Serialization;
using LiteDB;

namespace DSH.DiscordBot.Storage
{
    public sealed class LiteDbStorage : IStorage
    {
        private readonly LiteRepository _db;
        private readonly Lazy<ILog> _log;
        private readonly Lazy<ISerializer> _serializer;

        public LiteDbStorage(
            Lazy<ILog> log,
            Lazy<IConfig> config,
            Lazy<ISerializer> serializer)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            _log = log ?? throw new ArgumentNullException(nameof(log));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _db = new LiteRepository(config.Value.DbConnectionString);

            Configure();
        }

        public void Dispose()
        {
            _db?.Dispose();
        }

        public void Insert<T>(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _log.Value.Debug(
                "Inserted a new entity:{1}{0}",
                _serializer.Value.Serialize(entity),
                Environment.NewLine);

            _db?.Insert(entity);
        }

        public void Insert<T>(IEnumerable<T> entities)
        {
            var enumerable = entities as T[] ?? entities.ToArray();

            if (!enumerable.Any())
                throw new ArgumentNullException(nameof(entities));

            _log.Value.Debug(
                "Inserted a new entities:{1}{0}",
                _serializer.Value.Serialize(enumerable),
                Environment.NewLine);

            _db?.Insert(enumerable);
        }

        public void Update<T>(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _log.Value.Debug(
                "Updated existing entity to:{1}{0}",
                _serializer.Value.Serialize(entity),
                Environment.NewLine);

            _db?.Update(entity);
        }

        public void Update<T>(IEnumerable<T> entities)
        {
            var enumerable = entities as T[] ?? entities.ToArray();

            if (!enumerable.Any())
                throw new ArgumentNullException(nameof(entities));

            _log.Value.Debug(
                "Updated existing entities to:{1}{0}",
                _serializer.Value.Serialize(enumerable),
                Environment.NewLine);

            _db?.Update(enumerable);
        }

        public void Delete<T>(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            _log.Value.Debug("Deleted existing entity");

            _db?.Delete(predicate);
        }

        public List<T> Fetch<T>(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            _log.Value.Debug("Fetching entities");

            return _db?.Fetch(predicate);
        }

        public List<T> All<T>()
        {
            _log.Value.Debug("Getting all entities");

            return _db?.Fetch<T>();
        }

        private static void Configure()
        {
            BsonMapper.Global.RegisterType(
                (uri) => uri.AbsoluteUri,
                (bson) => new Uri(bson.AsString));
        }
    }
}