using Microsoft.Practices.Unity;
using SIBF.UserManagement.Api;
using System.Configuration;

namespace SIBF.UserManagement.App_Start
{
    public class SIBFBootStrapper
    {
        public IUnityContainer Container { get; set; }

        public SIBFBootStrapper()
        {
            Container = new UnityContainer();
            ConfigureContainer();
        }

        private void ConfigureContainer()
        {
            
        }
    }
}