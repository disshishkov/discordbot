using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DSH.DiscordBot.Storage
{
    public interface IStorage : IDisposable
    {
        void Insert<T>(T entity);
        void Insert<T>(IEnumerable<T> entities);
        void Update<T>(T entity);
        void Update<T>(IEnumerable<T> entities);
        void Delete<T>(Expression<Func<T, bool>> predicate);
        void Drop<T>();
        List<T> Fetch<T>(Expression<Func<T, bool>> predicate);
        List<T> All<T>();
    }
}