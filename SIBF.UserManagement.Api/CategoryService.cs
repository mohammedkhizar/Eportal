using NLog;
using SIBF.UserManagement.Api.Cache;
using SIBF.UserManagement.Api.Util;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SIBF.UserManagement.Api
{
    class CategoryService : ICategoryService
    {
        private string _connectionString;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public CategoryService(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public bool DeleteData(int id, string TableName, string ColName, string CurrentUser)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(CategoryServiceSQL.DELETE_ELEMENT))
                    {
                        command.Connection = connection;
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@tableName", TableName);
                        command.Parameters.AddWithValue("@colName", ColName);
                        connection.Open();

                        int cnt = command.ExecuteNonQuery();
                        if (cnt == 1)
                            return true;
                        else
                            return false;
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error("Error while executing SQL Statement", ex);
                    throw new DbException("Error while executing SQL Statement");
                }
            }
        }
    }

    internal class CategoryServiceSQL
    {
        public static readonly string DELETE_ELEMENT = "DELETE [SIBFInventory].[dbo].[@tableName]" +
                                                       "WHERE [@colName] = @id";
    }
}
