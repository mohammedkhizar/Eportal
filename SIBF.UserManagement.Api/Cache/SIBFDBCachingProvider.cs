using System.Collections.Generic;

namespace SIBF.UserManagement.Api.Cache
{
    public class SIBFDBCachingProvider : CachingProviderBase, IGlobalCachingProvider
    {
        protected SIBFDBCachingProvider()
        { }

        public static SIBFDBCachingProvider Instance
        {
            get { return Nested.instance; }
        }

        class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested() { }

            internal static readonly SIBFDBCachingProvider instance = new SIBFDBCachingProvider();
        }

        #region Role Cache

        public virtual void SetAllRoles(List<MembershipRole> roles)
        {
            this.AddItem("Roles", roles);
        }

        public virtual void AddRole(MembershipRole role)
        {
            List<MembershipRole> allRoles = (List<MembershipRole>)GetItem("Roles");
            allRoles.Add(role);
            SetAllRoles(allRoles);
        }

        public virtual bool IsRoleExists(string role)
        {
            List<MembershipRole> roles = (List<MembershipRole>) GetItem("Roles");
            if (roles == null)
                return false;

            if (roles.Find(r => r.Name == role) != null)
                return true;
            else
                return false;
        }

        #endregion
        #region IGlobalCachingProvider

        public virtual new void AddItem(string key, object value)
        {
            base.AddItem(key, value);
        }

        public virtual object GetItem(string key)
        {
            return base.GetItem(key, true);//Remove defulat is true because it's Global Cache!
        }

        public virtual new object GetItem(string key, bool remove)
        {
            return base.GetItem(key, remove);
        }
        #endregion
    }
}
