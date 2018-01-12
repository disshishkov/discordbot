using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using DSH.DiscordBot.Contract.Dto;
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
                $"Inserted a new entity:{Environment.NewLine}{_serializer.Value.Serialize(entity)}");

            _db?.Insert(entity);
        }

        public void Insert<T>(IEnumerable<T> entities)
        {
            var enumerable = entities as T[] ?? entities.ToArray();

            if (!enumerable.Any())
                throw new ArgumentNullException(nameof(entities));

            _log.Value.Debug(
                $"Inserted a new entities:{Environment.NewLine}{_serializer.Value.Serialize(enumerable)}");

            _db?.Insert(enumerable);
        }

        public void Update<T>(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _log.Value.Debug(
                $"Updated existing entity to:{Environment.NewLine}{_serializer.Value.Serialize(entity)}");

            _db?.Update(entity);
        }

        public void Update<T>(IEnumerable<T> entities)
        {
            var enumerable = entities as T[] ?? entities.ToArray();

            if (!enumerable.Any())
                throw new ArgumentNullException(nameof(entities));

            _log.Value.Debug(
                $"Updated existing entities to:{Environment.NewLine}{_serializer.Value.Serialize(enumerable)}");

            _db?.Update(enumerable);
        }

        public void Delete<T>(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            _log.Value.Debug("Deleted existing entity");

            _db?.Delete(predicate);
        }

        public void Drop<T>()
        {
            var collectionName = _db?.Database?.Mapper?.ResolveCollectionName(typeof(T));
            
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));
                
            _log.Value.Debug($"Drop collection {collectionName}");
            
            _db?.Database?.DropCollection(collectionName);

            var files = _db?.Database?.FileStorage.FindAll()?.ToArray();

            if (files != null)
            {
                _log.Value.Debug($"Drop {files.Length} files");
                
                foreach (var file in files)
                {
                    _log.Value.Trace($"Delete {file.Id} file");
                    _db?.Database?.FileStorage.Delete(file.Id);
                }
            }
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

        public void InsertData(string id, byte[] data)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _log.Value.Debug($"InsertData with id {id}");
            
            using (var ms = new MemoryStream(data))
            {
                try
                {
                    _db?.Database?.FileStorage.Upload(id, null, ms);
                }
                catch (Exception e)
                {
                    _log.Value.Error(e);
                }
            }
        }

        public void DeleteData(string id)
        {
            _log.Value.Debug($"DeleteData with id {id}");
            
            var files = _db?.Database?.FileStorage.Find(id);

            if (files != null)
            {
                foreach (var file in files)
                {
                    _db?.Database?.FileStorage.Delete(file.Id);
                }
            }
        }

        public byte[] GetData(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            _log.Value.Debug($"GetData for id {id}");
            
            var file = _db?.Database?.FileStorage.FindById(id);
            if (file == null)
                return null;
            
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private static void Configure()
        {
            var mapper = BsonMapper.Global;
            
            mapper.RegisterType(
                (uri) => uri.AbsoluteUri,
                (bson) => new Uri(bson.AsString));

            mapper.Entity<Build>().Ignore(_ => _.Screen);
        }
    }
}