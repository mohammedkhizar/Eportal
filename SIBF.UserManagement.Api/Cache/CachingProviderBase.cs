using System;
using System.Runtime.Caching;

namespace SIBF.UserManagement.Api.Cache
{
    public abstract class CachingProviderBase
    {
        protected MemoryCache cache = new MemoryCache("CachingProvider");
        static readonly object padlock = new object();
        protected virtual void AddItem(string key, object value)
        {
            lock (padlock)
            {
                cache.Set(key, value, DateTimeOffset.MaxValue);
            }
        }

        protected virtual void RemoveItem(string key)
        {
            lock (padlock)
            {
                cache.Remove(key);
            }
        }

        protected virtual object GetItem(string key, bool remove)
        {
            lock (padlock)
            {
                var res = cache[key];
                if(res != null)
                {
                    if(remove == true)
                    {
                        cache.Remove(key);
                    }
                }
                return res;
            }
        }


    }
}
