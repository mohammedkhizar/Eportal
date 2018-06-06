using System;
using System.Configuration;
using SIBF.UserManagement.Api;
using NLog;
using System.Collections.Generic;

namespace TestUsermanagementApi
{
    class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string
            _connectionString = ConfigurationManager.ConnectionStrings["UserManagementConnectionString"].ConnectionString;
        static void Main(string[] args)
        {

            string[] roles = { "ADMIN", "SEC_ADMIN" };
            Console.WriteLine(string.Join(",", roles));
            //createUser();
            //roleTest();
            allUserTest();
            Console.Read();
        }

        static void allUserTest()
        {
            IUserAccountService userAccountService = new UserAccountService(_connectionString);
            List<MembershipUser> allUsers = userAccountService.GetAllUsers();
            foreach(MembershipUser user in allUsers)
            {
                logger.Info(user.Username);
            }
        }
        static void roleTest()
        {
            IUserAccountService userAccountService = new UserAccountService(_connectionString);
            List<MembershipRole> allRoles = userAccountService.GetAllRoles();
            logger.Info("Listing all roles");
            int i = 0;
            foreach(MembershipRole role in allRoles){
                logger.Info (++i + ": " + role.Name);
            }
        }
        static void createUser()
        {
            IUserAccountService userAccountService = new UserAccountService(_connectionString);
            MembershipCreateStatus createStatus;

            string username = "ayesha_noor2";
            MembershipUser user = userAccountService.CreateUser(username, "ayesha", "ayesha_noor2@myworld.com", null, "power button user", out createStatus);
            logger.Info("Creating user, " + username);
            logger.Info("create status for user " + username + " is " + createStatus.ToString());
        }
    }
}
