namespace SIBF.UserManagement.Api.Cache
{
    public interface IGlobalCachingProvider
    {
        void AddItem(string key, object value);
        object GetItem(string key);
    }
}
