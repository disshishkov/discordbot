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
    public sealed class LiteDbStorage : IStorage, IDisposable
    {
        private readonly LiteRepository _db;
        private readonly Lazy<ILog> _log;
        private readonly Lazy<ISerializer> _serializer;

        public LiteDbStorage(
            Lazy<ILog> log,
            Lazy<IConfig> config,
            Lazy<ISerializer> serializer)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            _log = log;
            _serializer = serializer;
            _db = new LiteRepository(config.Value.DbConnectionString);

            BsonMapper.Global.RegisterType(
                (uri) => uri.AbsoluteUri,
                (bson) => new Uri(bson.AsString));
        }

        public void Dispose()
        {
            _db?.Dispose();
        }

        public void Insert<T>(T entity)
        {
            _log.Value.Debug(
                "Inserted a new entity:{1}{0}",
                _serializer.Value.Serialize(entity),
                Environment.NewLine);

            _db?.Insert(entity);
        }

        public void Insert<T>(IEnumerable<T> entities)
        {
            var enumerable = entities as T[] ?? entities.ToArray();
            _log.Value.Debug(
                "Inserted a new entities:{1}{0}",
                _serializer.Value.Serialize(enumerable),
                Environment.NewLine);

            _db?.Insert(enumerable);
        }

        public void Update<T>(T entity)
        {
            _log.Value.Debug(
                "Updated existing entity to:{1}{0}",
                _serializer.Value.Serialize(entity),
                Environment.NewLine);

            _db?.Update(entity);
        }

        public void Update<T>(IEnumerable<T> entities)
        {
            var enumerable = entities as T[] ?? entities.ToArray();
            _log.Value.Debug(
                "Updated existing entities to:{1}{0}",
                _serializer.Value.Serialize(enumerable),
                Environment.NewLine);

            _db?.Update(enumerable);
        }

        public void Delete<T>(Expression<Func<T, bool>> predicate)
        {
            _log.Value.Debug("Deleted existing entity");

            _db?.Delete(predicate);
        }

        public List<T> Fetch<T>(Expression<Func<T, bool>> predicate)
        {
            _log.Value.Debug("Fetching entities");

            return _db?.Fetch(predicate);
        }
    }
}