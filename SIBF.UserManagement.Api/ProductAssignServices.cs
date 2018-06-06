using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIBF.UserManagement.Api
{
    class ProductAssignServices : IProductAssigned
    {
        private string _connectionString;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ProductAssignServices(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public bool saveRequirementList(Array requiremntList)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {


                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.CREATE_USER))
                    {
                        
                        
                        command.Connection = connection;
                        connection.Open();
                        command.ExecuteNonQuery();
                        
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error(ex, "Error while executing SQL Statement");
                    throw new DbException("Error while executing SQL Statement");
                }
            }
            return true;
        }

    }
}
