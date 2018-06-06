using NLog;
using SIBF.UserManagement.Api.Cache;
using SIBF.UserManagement.Api.Model;
using SIBF.UserManagement.Api.Util;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;

namespace SIBF.UserManagement.Api
{
    public class ProductServices: IProductService
    {
        private string _connectionString;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ProductServices(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public List<RequirementList> GetAllRequestedProductList()
        {
            List<RequirementList> allUsers = new List<RequirementList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(ProductServiceSQL.GET_ALL_REQUEST))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            allUsers.Add(ReadProductList(reader));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error("Error while executing SQL Statement", ex);
                    throw new DbException("Error while executing SQL Statement");
                }
            }
            return allUsers;
        }

        public List<RequirementList> GetRequestedProductListByID(int RequirementID)
        {
            List<RequirementList> allUsers = new List<RequirementList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(ProductServiceSQL.GET_ALL_REQUEST_ID))
                    {
                        command.Parameters.AddWithValue("@id", RequirementID);
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            allUsers.Add(ReadProductListByID(reader));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error("Error while executing SQL Statement", ex);
                    throw new DbException("Error while executing SQL Statement");
                }
            }
            return allUsers;
        }

        public RequirementList ReadProductList(SqlDataReader reader)
        {
            RequirementList requirementList = new RequirementList();
             requirementList.CompanyID = reader.GetInt32(0);
             requirementList.DepartmentID = reader.GetInt32(1);
             requirementList.ProductID = reader.GetInt32(2);
             requirementList.AssignedProductQuantity = reader.GetInt32(3);
             requirementList.CompanyName = reader.GetString(4);
             requirementList.DepartmentName = reader.GetString(5);
             requirementList.UserName = reader.GetString(6);
             requirementList.CategoryName = reader.GetString(7);
             requirementList.SubCategoryName = reader.GetString(8); 
             requirementList.ProductName = reader.GetString(9);
             requirementList.AvailableProductQuantity = reader.IsDBNull(10)? 0 : reader.GetInt32(10);
             requirementList.RowID = reader.GetInt32(11);
            return requirementList;
        }

        public RequirementList ReadProductListByID(SqlDataReader reader)
        {
            RequirementList requirementList = new RequirementList();
            requirementList.RowID = reader.GetInt32(0);
            requirementList.UserName = reader.GetString(1);
            requirementList.RequestedProductQuantity = reader.GetInt32(2);
            requirementList.CategoryID = reader.GetInt32(3);
            requirementList.CategoryName = reader.GetString(4);
            requirementList.SubCategoryID = reader.GetInt32(5);
            requirementList.SubCategoryName = reader.GetString(6);
            requirementList.ProductID = reader.GetInt32(7);
            requirementList.ProductName = reader.GetString(8);
            requirementList.RequestedDate = reader.GetDateTime(9);
            return requirementList;
        }

        public bool UpdatedProductAssigned(string UserName, string ProductName, int ProductID, int SelRowID, int Quantity, string Reason, string currentUser)
        {
            using (  SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.ASSIGN_PRODUCT_USER))
                    {
                        DateTime CreatedOn = DateTime.Now;
                        command.Parameters.AddWithValue("@UserName", UserName);
                        command.Parameters.AddWithValue("@Quantity", Quantity);
                        command.Parameters.AddWithValue("@ProductID", ProductID);
                        command.Parameters.AddWithValue("@CreatedOn", CreatedOn);
                        command.Parameters.AddWithValue("@CurrentUser", currentUser);
                        command.Parameters.AddWithValue("@Reason", Reason);
                        command.Parameters.AddWithValue("@RequirementID", SelRowID);
                        command.Parameters.AddWithValue("@StockID", '0');
                        command.Connection = connection;
                        connection.Open();
                        int rowsEffected = command.ExecuteNonQuery();
                        //object result = command.ExecuteScalar();
                        if (rowsEffected == 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error("Error while executing SQL Statement", ex);
                    throw new DbException("Error while executing SQL Statement");
                }
            }
        }

        public List<RequirementList> GetStockByProductID(int ProductID)
        {
            List<RequirementList> prodByID = new List<RequirementList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(ProductServiceSQL.GET_PROD_QTYID))
                    {
                        command.Parameters.AddWithValue("@ProductID", ProductID);
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            RequirementList requirementList = new RequirementList();
                            requirementList.ProductName = reader.GetString(0);

                            requirementList.CompanyID = reader.GetInt32(0);
                            requirementList.DepartmentID = reader.GetInt32(1);
                            requirementList.ProductID = reader.GetInt32(2);
                            requirementList.AssignedProductQuantity = reader.GetInt32(3);
                            requirementList.CompanyName = reader.GetString(4);
                            requirementList.DepartmentName = reader.GetString(5);
                            requirementList.UserName = reader.GetString(6);
                            requirementList.CategoryName = reader.GetString(7);
                            requirementList.SubCategoryName = reader.GetString(8);
                            requirementList.AvailableProductQuantity = reader.IsDBNull(10) ? 0 : reader.GetInt32(10);
                            requirementList.RowID = reader.GetInt32(11);

                        }
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error("Error while executing SQL Statement", ex);
                    throw new DbException("Error while executing SQL Statement");
                }
            }
            return prodByID;
        }


        public List<RequirementList> GetAssignedStockByRowID(int AssignedRowID)
        {
            List<RequirementList> prodByID = new List<RequirementList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(ProductServiceSQL.GET_ASSIGN_PROD))
                    {
                        command.Parameters.AddWithValue("@AssignedRowID", AssignedRowID);
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            RequirementList requirementList = new RequirementList();
                            prodByID.Add(new RequirementList
                            {
                                RowID = reader.GetInt32(0),
                                RequirementID = reader.GetInt32(1),
                                UserName = reader.GetString(2),
                                RequestedProductQuantity = reader.GetInt32(3),
                                AssignedProductQuantity = reader.GetInt32(4),
                                CategoryID = reader.GetInt32(5),
                                CategoryName = reader.GetString(6),
                                SubCategoryID = reader.GetInt32(7),
                                SubCategoryName = reader.GetString(8),
                                ProductID = reader.GetInt32(9),
                                ProductName = reader.GetString(10),
                                AssignedDate = reader.GetDateTime(11),
                                CompanyID = reader.GetInt32(12)
                            });
                        }
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error("Error while executing SQL Statement", ex);
                    throw new DbException("Error while executing SQL Statement");
                }
            }
            return prodByID;
        }

        public bool AssignProduct(string UserName, int Quantity, int ProductID, string CurrentUser, int StockID, int RequirementID, string Reason)
        {
            using (SqlConnection conneciton = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.ASSIGN_PRODUCT_USER))
                    {
                        DateTime createdAt = DateTime.Now;

                        command.Parameters.AddWithValue("@UserName", UserName);
                        command.Parameters.AddWithValue("@ProductID", ProductID);
                        command.Parameters.AddWithValue("@CurrentUser", CurrentUser);
                        command.Parameters.AddWithValue("@CreatedOn", createdAt);
                        command.Parameters.AddWithValue("@Quantity", Quantity);
                        command.Parameters.AddWithValue("@StockID", StockID);
                        command.Parameters.AddWithValue("@RequirementID", RequirementID);
                        command.Parameters.AddWithValue("@Reason", Reason);

                        command.Connection = conneciton;
                        conneciton.Open();

                        int rowEffected = command.ExecuteNonQuery();
                        if (rowEffected == 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error(ex, "Error while executing SQL Statement");
                    throw new DbException("Error while executing SQL Statement");
                }
            }
        }



        public bool ReduceQuantity(int StockID, int QtyReduce)
        {
            // this.ProductDataByID(int ProductID);

            using (SqlConnection conneciton = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.Reduce_Quantity))
                    {
                        DateTime createdAt = DateTime.Now;
                        command.Parameters.AddWithValue("@StockID", StockID);
                        command.Parameters.AddWithValue("@QtyReduce", QtyReduce);
                        command.Connection = conneciton;
                        conneciton.Open();

                        int rowEffected = command.ExecuteNonQuery();
                        if (rowEffected == 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error(ex, "Error while executing SQL Statement");
                    throw new DbException("Error while executing SQL Statement");
                }
            }
        }

        public bool updateProductRtnQty(int stockID, int quantity)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.UPDATEPRODQTYRTN))
                    {
                        command.Parameters.AddWithValue("@stockID", stockID);
                        command.Parameters.AddWithValue("@quantity", quantity);
                        command.Connection = connection;
                        connection.Open();

                        int cnt = command.ExecuteNonQuery();
                        if (cnt == 1)
                            return true;
                        else
                            return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool updateReduceRtnQty(int RowID, int ReturnQuantity, string currentUser, DateTime currentdatetime, string status)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(ProductServiceSQL.UPDATEREDUCERTNQTY))
                    {
                        command.Parameters.AddWithValue("@RowID", RowID);
                        command.Parameters.AddWithValue("@ReturnQuantity", ReturnQuantity);
                        command.Parameters.AddWithValue("@currentUser", currentUser);
                        command.Parameters.AddWithValue("@currentdatetime", currentdatetime);
                        command.Parameters.AddWithValue("@status", status);
                        command.Connection = connection;
                        connection.Open();
                        int cnt = command.ExecuteNonQuery();
                        if (cnt == 1)
                            return true;
                        else
                            return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

    }
}

#region SQL Statement
internal class ProductServiceSQL
{
    public static readonly string GET_ALL_REQUEST = "SELECT GR.Company_ID, GR.Department_ID, GR.Product_Name, GR.Product_Quantity," +
                                                    " C.Company_FullName, D.Department_Name, R.Username," +
                                                    " CAT.item_category_title, SUBCAT.sub_item_category_title, P.Product_Name, AvailableQuantity, GR.ID" +
                                                    " FROM  [SIBFInventory].[dbo].[GeneralRequirementForm] GR" +
                                                    " INNER JOIN[SIBFInventory].[dbo].[CreateDepartment] D ON GR.Department_ID = D.Department_ID" +
                                                    " INNER JOIN[SIBFInventory].[dbo].[Users] R ON GR.Submitted_For = R.Username" +
                                                    " INNER JOIN[SIBFInventory].[dbo].[CreateCompany] C ON GR.Company_ID = C.Company_ID" +
                                                    " INNER JOIN[SIBFInventory].[dbo].[Products] P ON GR.Product_Name = P.Product_ID" +
                                                    " INNER JOIN[SIBFInventory].[dbo].[ItemCategory] CAT ON GR.[Category_ID] = CAT.item_category_id" +
                                                    " INNER JOIN[SIBFInventory].[dbo].[SubItemCategory] SUBCAT ON GR.[SubCategory_ID] = SUBCAT.sub_item_category_id" +
                                                    " AND GR.[ID] NOT IN (SELECT Requirement_ID FROM UserProducts)" +
                                                    " LEFT JOIN (SELECT Product_ID, sum(Quantity)AS AvailableQuantity" +
                                                    " FROM[SIBFInventory].[dbo].[StockHistory] " +
                                                    "GROUP BY Product_ID) SHIS ON(GR.[Product_Name] = SHIS.Product_ID)";

    public static readonly string ASSIGN_PRODUCT_USER = "INSERT INTO [SIBFInventory].[dbo].[UserProducts]" +
                                                        "([Username],[Product_ID],[Requirement_ID],[Quantity],[Reason],[Assigned_Date],[Assigned_By])" +
                                                        "VALUES(@UserName, @ProductID, @RequirementID, @Quantity, @Reason, @CreatedOn, @CurrentUser)";

    public static readonly string GET_PROD_QTYID = "SELECT P.Product_Name, SH.Quantity, SH.Stock_ID, SH.Created_On, SH.Manufacture_Date, SH.Expiry_Date" +
                                                   " FROM[SIBFInventory].[dbo].[StockHistory]SH LEFT JOIN[SIBFInventory].[dbo].[Products]P" +
                                                    " ON SH.Product_ID = P.Product_ID WHERE  SH.Product_ID = @ProductID";
    public static readonly string GET_ALL_REQUEST_ID = "SELECT gr.ID, gr.Submitted_For, gr.Product_Quantity, cat.item_category_id, cat.item_category_title, " +
                                                       "subcat.sub_item_category_id, subcat.sub_item_category_title, P.Product_ID, p.Product_Name, gr.Creation_Date FROM " +
                                                       "GeneralRequirementForm gr INNER JOIN ItemCategory cat ON gr.Category_ID = cat.item_category_id "+
                                                       "INNER JOIN SubItemCategory subcat ON gr.SubCategory_ID = subcat.sub_item_category_id "+
                                                       "INNER JOIN Products p ON gr.Product_Name = p.Product_ID WHERE ID = @id";
    public static readonly string GET_ASSIGN_PROD = "SELECT up.Row_ID,gr.ID, gr.Submitted_For, gr.Product_Quantity,up.Quantity, cat.item_category_id, cat.item_category_title, " +
                                                    "subcat.sub_item_category_id, subcat.sub_item_category_title, up.Product_ID, p.Product_Name,gr.Creation_Date, up.Stock_ID " +
                                                    "FROM GeneralRequirementForm gr INNER JOIN ItemCategory cat ON gr.Category_ID = cat.item_category_id "+
                                                    "INNER JOIN SubItemCategory subcat ON gr.SubCategory_ID = subcat.sub_item_category_id INNER JOIN UserProducts up ON up.Requirement_ID = gr.ID "+
                                                    "INNER JOIN Products p ON up.Product_ID = p.Product_ID WHERE up.Row_ID = @AssignedRowID";
    public static readonly string UPDATEREDUCERTNQTY = "UPDATE [SIBFInventory].[dbo].[UserProducts] SET " +
                                                       "Quantity= (SELECT Quantity FROM [UserProducts] WHERE Row_ID=@RowID) - @ReturnQuantity, "+
                                                       "Updated_By=@currentUser, Updated_Date=@currentdatetime, Status=@status WHERE Row_ID=@RowID";

}
#endregion
