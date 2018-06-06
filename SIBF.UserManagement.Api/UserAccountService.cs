using NLog;
using SIBF.UserManagement.Api.Cache;
using SIBF.UserManagement.Api.Util;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;

namespace SIBF.UserManagement.Api
{
    public class UserAccountService : IUserAccountService
    {
        private string _connectionString;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public UserAccountService(string connectionString)
        {
            this._connectionString = connectionString;

            //Cache all roles
            // GetAllRoles();
        }

        #region Role Management
        public void AddRemoveRoleForUser(string username, string rolename, bool isInRole)
        {
            throw new NotImplementedException();
        }

        public void AddUserToRoles(MembershipUser user, string assignedBy, string[] roles)
        {
            #region Check data validity
            if (!isUserExists(user.Username))
            {
                throw new DbException("Unable to assign role as user does not exist");
            }

            foreach (string role in roles)
            {
                if (!isRoleExists(role))
                    throw new DbException("Unable to assign role as role " + role + " does not exist");
            }
            string roleNames = string.Join(",", roles);

            #endregion

            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.ASSIGN_ROLE))
                    {
                        command.Parameters.AddWithValue("@Username", user.Username);
                        command.Parameters.AddWithValue("@AssignedBy", assignedBy);
                        command.Parameters.AddWithValue("@AssignedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@roles", roleNames);
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

            throw new NotImplementedException();
        }

        public void CreateRole(MembershipRole role)
        {
            #region Check data validity

            if (role == null)
                throw new ArgumentNullException("role cannot be null");

            if (isRoleExists(role.Name))
            {
                throw new DuplicateRoleNameException("Unable to create role as it already exist");
            }

            #endregion

            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.CREATE_USER))
                    {
                        command.Parameters.AddWithValue("@name", role.Name);

                        if (String.IsNullOrEmpty(role.Description))
                            command.Parameters.AddWithValue("@description", role.Description);
                        else
                            command.Parameters.AddWithValue("@description", role.Description);

                        if (String.IsNullOrEmpty(role.DescriptionArabic))
                            command.Parameters.AddWithValue("@descriptionArabic", role.DescriptionArabic);
                        else
                            command.Parameters.AddWithValue("@descriptionArabic", role.DescriptionArabic);

                        command.Connection = connection;
                        connection.Open();
                        command.ExecuteNonQuery();
                        SIBFDBCachingProvider.Instance.AddRole(role);
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error(ex, "Error while executing SQL Statement");
                    throw new DbException("Error while executing SQL Statement");
                }
            }
        }

        public List<MembershipRole> GetAllRoles()
        {
            List<MembershipRole> roles = (List<MembershipRole>)SIBFDBCachingProvider.Instance.GetItem("Roles");
            if (roles != null)
            {
                return roles;
            }

            roles = new List<MembershipRole>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_ALL_ROLES))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            roles.Add(new MembershipRole
                            {
                                Name = reader.GetString(0),
                                Description = reader.IsDBNull(1) ? null : reader.GetString(1),
                                DescriptionArabic = reader.IsDBNull(2) ? null : reader.GetString(2)
                            });
                        }
                    }
                    SIBFDBCachingProvider.Instance.SetAllRoles(roles);
                    return roles;
                }
                catch (SqlException ex)
                {
                    logger.Error("Error while executing SQL Statement", ex);
                    throw new DbException("Error while executing SQL Statement");
                }
            }
        }

        public List<MembershipRole> GetRolesForUser(string username)
        {
            List<MembershipRole> roles = new List<MembershipRole>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_ALL_ROLES_BY_USER))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        command.Parameters.AddWithValue("@username", username);
                        while (reader.Read())
                        {
                            roles.Add(new MembershipRole
                            {
                                Name = reader.GetString(0),
                                Description = reader.IsDBNull(1) ? null : reader.GetString(1),
                                DescriptionArabic = reader.IsDBNull(2) ? null : reader.GetString(2)
                            });
                        }
                    }
                    SIBFDBCachingProvider.Instance.SetAllRoles(roles);
                    return roles;
                }
                catch (SqlException ex)
                {
                    logger.Error("Error while executing SQL Statement", ex);
                    throw new DbException("Error while executing SQL Statement");
                }
            }
        }

        public List<ProductDetails> ProductDetails(int ProductID)
        {
            List<ProductDetails> allProductByID = new List<ProductDetails>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_PRODUCT_BYID))
                    {
                        command.Connection = connection;
                        connection.Open();
                        command.Parameters.AddWithValue("@productID", ProductID);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            allProductByID.Add(new ProductDetails
                            {
                                
                                StockID = reader.GetInt32(0),
                                Quantity = reader.IsDBNull(1) ?  0 : reader.GetInt32(1),
                                ManufactureDate = reader.GetDateTime(2),
                                ExpiryDate = reader.GetDateTime(3)
                                //productDetails = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                                //ProductName = reader.GetString(0).ToString(),
                                //ProductDesc = reader.GetString(1).ToString(),
                                //CategoryName = reader.GetString(2).ToString(),
                                //SubCategoryName = reader.GetString(3).ToString(),
                                //CompanyName = reader.GetString(4).ToString(),
                                //AvaliableQuantity = reader.GetString(5).ToString()
                            });
                        }
                    }
                    return allProductByID;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public bool DeleteRole(string roleName)
        {
            if (!isRoleExists(roleName))
            {
                throw new RoleNotFoundException(roleName + " not found in db");
            }
            if (isUserFoundForRole(roleName))
            {
                throw new UsersAvialbleForRoleException("Role can't be deleted as Users are assigned to role");
            }
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.DELETE_ROLE))
                    {
                        command.Parameters.AddWithValue("@name", roleName);
                        command.Connection = connection;
                        connection.Open();
                        Object result = command.ExecuteScalar();
                        if (result == null)
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

        #endregion

        #region User Management
        public MembershipUser CreateUser(string username, string password, string email, string createdBy, string comment, int companyID, int departmentID,
            out MembershipCreateStatus createStatus)
        {

            #region Check data validity
            if (!IsValidEmail(email))
            {
                createStatus = MembershipCreateStatus.InvalidEmail;
                return null;
            }
            if (string.IsNullOrEmpty(username))
            {
                createStatus = MembershipCreateStatus.InvalidUserName;
                return null;
            }
            if (string.IsNullOrEmpty(password))
            {
                createStatus = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if (isUserExists(username))
            {
                createStatus = MembershipCreateStatus.DuplicateUserName;
                return null;
            }
            if (isEmailExists(email))
            {
                createStatus = MembershipCreateStatus.DuplicateEmail;
                return null;
            }
            #endregion

            string passwordHashed = PasswordHash.CreateHash(password);
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.CREATE_USER))
                    {
                        DateTime createdAt = DateTime.Now;
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", passwordHashed);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@creationDate", createdAt);

                        if (string.IsNullOrEmpty(comment))
                            command.Parameters.AddWithValue("@comment", DBNull.Value);
                        else
                            command.Parameters.AddWithValue("@comment", comment);

                        if (string.IsNullOrEmpty(createdBy))
                        {
                            command.Parameters.AddWithValue("@createdBy", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@createdBy", createdBy);
                        }

                        command.Parameters.AddWithValue("@companyid", companyID);
                        command.Parameters.AddWithValue("@departmentid", departmentID);

                        command.Connection = connection;
                        connection.Open();
                        int rowsEffected = command.ExecuteNonQuery();
                        if (rowsEffected == 1)
                        {
                            createStatus = MembershipCreateStatus.Success;
                            return new MembershipUser
                            {
                                Username = username,
                                Password = password,
                                Email = email,
                                CreationDate = createdAt,
                                CreatedBy = createdBy,
                                Comment = comment,
                                IsApproved = true,
                                IsLockedout = false,
                                LastActivityDate = null,
                                LastLoginDate = null,
                                LastPasswordChangedDate = null,
                                ModifiedBy = null
                            };
                        }
                        else
                        {
                            createStatus = MembershipCreateStatus.UserRejected;
                            return null;
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

        public bool UnlockUser(string username, bool isLocked)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.LOCK_UNLOCK_ACCOUNT))
                    {
                        command.Connection = connection;
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@isLockedOut", isLocked);
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

        public bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                return false;
            if (string.IsNullOrEmpty(password))
                return false;

            string dbHash = GetPassword(username);
            if (dbHash == null)
            {
                throw new UserNotFoundException("user not found in the database");
            }
            bool isPasswordValid = PasswordHash.ValidatePassword(password, dbHash);
            if (!isPasswordValid)
            {
                throw new InvalidPasswordException("user not found in the database");
            }
            return true;
        }

        public List<MembershipUser> GetAllUsers(int page, int size, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public List<SubCategoryList> SubCategory()
        {
            List<SubCategoryList> allSUbCategory = new List<SubCategoryList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_CAT_SUBCAT))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            allSUbCategory.Add(ReadSubCategory(reader));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error("Error while executing SQL Statement", ex);
                    throw new DbException("Error while executing SQL Statement");
                }
            }
            return allSUbCategory;
        }

        public List<SupplierList> SupplierList()
        {
            List<SupplierList> allsupplierinfo = new List<SupplierList>();
            using (SqlConnection conneciton = new SqlConnection(this._connectionString))
            {
                using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_ALL_SUPPLIERS))
                {
                    command.Connection = conneciton;
                    conneciton.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        allsupplierinfo.Add(ReadSupplierInfo(reader));
                    }
                }
            }
            return allsupplierinfo;
        }

        public List<UAEStatesList> UAEStatesList()
        {
            List<UAEStatesList> UAEStatesList = new List<UAEStatesList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_ALL_STATES_UAE))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            UAEStatesList.Add(ReadUAESates(reader));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error("Error while executing SQL Statement", ex);
                    throw new DbException("Error while executing SQL Statement");
                }
            }
            return UAEStatesList;
        }

        public List<MembershipUser> GetAllUsers()
        {
            List<MembershipUser> allUsers = new List<MembershipUser>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_ALL_USERS))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            allUsers.Add(ReadUser(reader));
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

        public List<ProductList> ProductList()
        {
            List<ProductList> allProduct = new List<ProductList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_ALL_PRODUCTS))
                {
                    command.Connection = connection;
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        allProduct.Add(ReadProduct(reader));
                    }
                }
            }
            return allProduct;
        }

        public List<CountriesList> CountriesList()
        {
            List<CountriesList> allCountries = new List<CountriesList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_ALL_COUNTRIES))
                {
                    command.Connection = connection;
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        allCountries.Add(ReadCounties(reader));
                    }
                }
            }
            return allCountries;
        }

        public List<StatesList> StatesList(int CountryID)
        {
            List<StatesList> allStates = new List<StatesList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_ALL_STATES))
                {
                    command.Parameters.AddWithValue("@CountryID", CountryID);
                    command.Connection = connection;
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        allStates.Add(ReadStates(reader));
                    }
                }
            }
            return allStates;
        }


        public List<CitiesList> CitiesList(int StateID)
        {
            List<CitiesList> allCities = new List<CitiesList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_ALL_CITIES))
                {
                    command.Parameters.AddWithValue("@StateID", StateID);
                    command.Connection = connection;
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        allCities.Add(ReadCities(reader));
                    }
                }
            }
            return allCities;
        }

        #endregion


        #region Private Methods

        private string GetPassword(string username)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.USER_PASSWORD))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Connection = connection;
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result == null)
                        {
                            return null;
                        }
                        else
                        {
                            return result.ToString();
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

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool isUserExists(string username)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.USER_FOUND))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Connection = connection;
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
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

        private bool isDataExists(string columnData)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.USER_FOUND))
                    {
                        command.Parameters.AddWithValue("@columnData", columnData);
                        command.Connection = connection;
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
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

        private bool isEmailExists(string email)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.EMAIL_FOUND))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        command.Connection = connection;
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
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

        private bool isRoleExists(string role)
        {
            return SIBFDBCachingProvider.Instance.IsRoleExists(role);
        }

        public bool isUserFoundForRole(string roleName)
        {
            if (!isRoleExists(roleName))
            {
                throw new RoleNotFoundException(roleName + " not found in db");
            }

            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.USER_FOUND_FOR_ROLE))
                    {
                        command.Parameters.AddWithValue("@name", roleName);
                        command.Connection = connection;
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
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

        private MembershipUser ReadUser(SqlDataReader reader)
        {
            return new MembershipUser
            {
                Username = reader.GetString(0).ToString(),
                Password = reader.GetString(1).ToString(),
                Email = reader.IsDBNull(2) ? null : reader.GetString(2).ToString(),
                CreationDate = reader.GetDateTime(3),
                //CreationDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                IsApproved = reader.GetBoolean(4),
                IsLockedout = reader.GetBoolean(5),
                LastActivityDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                LastLoginDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
                LastPasswordChangedDate = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                Comment = reader.IsDBNull(9) ? null : reader.GetString(9),
                CreatedBy = reader.IsDBNull(10) ? null : reader.GetString(10),
                ModifiedBy = reader.IsDBNull(11) ? null : reader.GetString(11),
                UserMustChangePassword = reader.IsDBNull(12) ? false : reader.GetBoolean(12)
            };
        }

        private UserList ReadUserById(SqlDataReader reader)
        {
            return new UserList
            {
                Username = reader.GetString(0)
            };
        }

        private SubCategoryList ReadSubCategory(SqlDataReader reader)
        {
            //string CategoryNameCombined = reader.GetString(5).ToString();
            return new SubCategoryList
            {
                CategoryId = reader.GetInt32(1),
                SubCategoryId = reader.GetInt32(0),
                SubCategoryName = reader.GetString(2).ToString(),
                SubCategoryDesc = reader.IsDBNull(3) ? null : reader.GetString(3),
                CategoryName = reader.GetString(5).ToString()
            };
        }

        //private SubCategoryList ReadCategory(SqlDataReader reader)
        //{
        //    string CategoryNameCombined = reader.GetString(1).ToString();
        //    return new SubCategoryList
        //    {
        //        CategoryId = reader.GetInt32(0).ToString(),
        //        SubCategoryName = reader.GetString(1).ToString(),
        //        SubCategoryDesc = reader.GetString(2).ToString(),
        //        CategoryName = CategoryNameCombined
        //    };
        //}

        private SupplierList ReadSupplierInfo(SqlDataReader reader)
        {
            return new SupplierList
            {
                SupplierID = reader.GetInt32(0).ToString(),
                SupplierName = reader.GetString(1).ToString()
            };
        }

        private ProductList ReadProduct(SqlDataReader reader)
        {
            return new ProductList
            {
                ProductID = reader.GetInt32(0),
                ProductName = reader.GetString(2).ToString()
            };
        }

        private CountriesList ReadCounties(SqlDataReader reader)
        {
            return new CountriesList
            {
                CountryID = reader.GetInt32(0),
                CountryName = reader.GetString(2).ToString()
            };
        }

        private StatesList ReadStates(SqlDataReader reader)
        {
            return new StatesList
            {
                StateID = reader.GetInt32(0),
                StateName = reader.GetString(1).ToString(),
                CountryID = reader.GetInt32(2)
            };
        }
        private CitiesList ReadCities(SqlDataReader reader)
        {
            return new CitiesList
            {
                CityID = reader.GetInt32(0),
                CityName = reader.GetString(1).ToString(),
                StateID = reader.GetInt32(2)
            };
        }

        private UAEStatesList ReadUAESates(SqlDataReader reader)
        {
            return new Api.UAEStatesList
            {
                StateID = reader.GetInt32(0),
                StateName = reader.GetString(1),
                CountryID = reader.GetInt32(2)
            };
        }
        #endregion

        #region addcategory

        public MembershipUser CreateCategory(string CategoryName, string CategoryDescription, out MembershipCreateStatus createStatus)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.CREATE_CATEGORY))
                    {
                        DateTime createddate = DateTime.Now;
                        command.Parameters.AddWithValue("@categoryname", CategoryName);
                        command.Parameters.AddWithValue("@categorydescription", CategoryDescription);
                        command.Parameters.AddWithValue("@createddate", createddate);

                        command.Connection = connection;
                        connection.Open();

                        int rowsEffected = command.ExecuteNonQuery();
                        if (rowsEffected == 1)
                        {
                            createStatus = MembershipCreateStatus.Success;
                            return new MembershipUser
                            {
                                CategoryName = CategoryName
                            };
                        }
                        else
                        {
                            createStatus = MembershipCreateStatus.UserRejected;
                            return null;
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

        public List<CategoryList> GetAllCategory()
        {
            List<CategoryList> allCategory = new List<CategoryList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_ALL_CATEGORY))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            allCategory.Add(ReadCategory(reader));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error("Error while executing SQL Statement", ex);
                    throw new DbException("Error while executing SQL Statement");
                }
            }
            return allCategory;
        }

        private CategoryList ReadCategory(SqlDataReader reader)
        {
            return new CategoryList
            {
                // CategoryId = reader.GetInt32(0).Equals(),
                CategoryId = reader.GetInt32(0).ToString(),
                CategoryName = reader.GetString(1).ToString(),
                CategoryDescription = reader.GetString(2).ToString()
            };
        }
        #endregion


        public ProductCatSubCatInstance CreateSubCategory(string CategoryID, string SubCategoryName, string SubCategoryDescription,
            out ProductCategorySubCategory createSubCategory)
        {
            if (isDataExistsCondition("SubItemCategory", "item_category_id", "sub_item_category_title", CategoryID, SubCategoryName))
            {
                createSubCategory = ProductCategorySubCategory.DuplicateName;
                return null;
            }
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.CREATE_SUBCATEGORY))
                    {

                        DateTime createddate = DateTime.Now;
                        command.Parameters.AddWithValue("@categoryid", CategoryID);
                        command.Parameters.AddWithValue("@subcategoryname", SubCategoryName);
                        command.Parameters.AddWithValue("@subcategorydescription", SubCategoryDescription);
                        command.Parameters.AddWithValue("@createddate", createddate);

                        command.Connection = connection;
                        connection.Open();

                        int rowsEffected = command.ExecuteNonQuery();
                        if (rowsEffected == 1)
                        {
                            createSubCategory = ProductCategorySubCategory.Success;
                            return new ProductCatSubCatInstance
                            {
                                SubCategoryName = SubCategoryName
                            };
                        } 
                        else
                        {
                            createSubCategory = ProductCategorySubCategory.DuplicateName;
                            return null;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error(ex, "Error while executing SQL Statement");
                    throw new DuplicateColumnException("Unable to create subcategory as subcategory " + SubCategoryName + " already exist");
                }
            }
        }

        public bool CreateWareHouse(string StoreName, string StoreManager, string StoreRoomNumber, string StoreType, string currentUser)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.CREATE_WAREHOUSE))
                    {
                        DateTime createdAt = DateTime.Now;
                        command.Parameters.AddWithValue("@StoreName", StoreName);
                        command.Parameters.AddWithValue("@StoreManager", StoreManager);
                        command.Parameters.AddWithValue("@StoreRoomNumber", StoreRoomNumber);
                        command.Parameters.AddWithValue("@StoreType", StoreType);
                        command.Parameters.AddWithValue("@currentUser", currentUser);
                        command.Parameters.AddWithValue("@creationDate", createdAt);
                        command.Connection = connection;
                        connection.Open();

                        int rowsEffected = command.ExecuteNonQuery();

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
                    logger.Error(ex, "Error while executing SQL Statement");
                    throw new DbException("Error while executing SQL Statement");
                }
            }
        }

        public bool CreateSupplier(string FullName, string Currency, string Address, string State, string PoCode, string Website, string Email, string ContactNumber, string ContactPerson, string Description, string CurrentUser)
        {
            using (SqlConnection conneciton = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.CREATE_SUPPLIER))
                    {
                        DateTime createdAt = DateTime.Now;
                        command.Parameters.AddWithValue("@FullName", FullName);
                        command.Parameters.AddWithValue("@Currency", Currency);
                        command.Parameters.AddWithValue("@Address", Address);
                        command.Parameters.AddWithValue("@Country", State);
                        command.Parameters.AddWithValue("@City", State);
                        command.Parameters.AddWithValue("@State", State);
                        command.Parameters.AddWithValue("@PoCode", PoCode);
                        command.Parameters.AddWithValue("@Website", Website);
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@ContactNumber", ContactNumber);
                        command.Parameters.AddWithValue("@ContactPerson", ContactPerson);
                        command.Parameters.AddWithValue("@Description", Description);
                        command.Parameters.AddWithValue("@createdAt", createdAt);
                        command.Parameters.AddWithValue("@CurrentUser", CurrentUser);

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

        public bool CreateProduct(int CategoryID, int SupplierID, string ProductName, string ProductDesc, string CreatedBy, string ProductType)
        {
            using (SqlConnection conneciton = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.CREATE_PRODUCT))
                    {
                        DateTime createdAt = DateTime.Now;

                        command.Parameters.AddWithValue("@Category_ID", CategoryID);
                        command.Parameters.AddWithValue("@Supplier_ID", SupplierID);
                        command.Parameters.AddWithValue("@Product_Name", ProductName);
                        command.Parameters.AddWithValue("@Product_Desc", ProductDesc);
                        command.Parameters.AddWithValue("@Created_By", CreatedBy);
                        command.Parameters.AddWithValue("@Created_On", createdAt);
                        command.Parameters.AddWithValue("@ProductType", ProductType);
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

        public bool EnterStock(string ProductID, string Quantity, DateTime ManufactureDate, DateTime ExpiryDate, string BarCode, string currentUser)
        {
            using (SqlConnection conneciton = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.ADD_STOCK))
                    {
                        DateTime createdAt = DateTime.Now;

                        command.Parameters.AddWithValue("@ProductID", ProductID);
                        command.Parameters.AddWithValue("@Quantity", Quantity);
                        command.Parameters.AddWithValue("@ManufactureDate", ManufactureDate);
                        command.Parameters.AddWithValue("@ExpiryDate", ExpiryDate);
                        command.Parameters.AddWithValue("@BarCode", BarCode);
                        command.Parameters.AddWithValue("@CurrentUser", currentUser);
                        command.Parameters.AddWithValue("@Created_On", createdAt);

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

        /*
        public List<ProductList> ProductDataByID(int ProductID)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                string productDetails = null;
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_PRODUCT_BYID))
                    {

                        command.Connection = connection;
                        connection.Open();
                        command.Parameters.AddWithValue("@ProductID", ProductID);
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            // productDetails = reader.GetString(2).ToString() + ',' + reader.GetString(3) + ',' + reader.GetString(4) + ',' + reader.GetString(0) + ',' + reader.GetString(1) + ',' + reader.GetInt32(5).ToString();
                            productDetails = reader.GetInt32(5).ToString();
                        }
                    }
                    return productDetails;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }
        */

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

        public int ProductData(int ProductID)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                int productDetails = 0;
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_PRODUCT_BYID))
                    {

                        command.Connection = connection;
                        connection.Open();
                        command.Parameters.AddWithValue("@ProductID", ProductID);
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            // productDetails = reader.GetString(2).ToString() + ',' + reader.GetString(3) + ',' + reader.GetString(4) + ',' + reader.GetString(0) + ',' + reader.GetString(1) + ',' + reader.GetInt32(5).ToString();
                            productDetails = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                            //StockID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0)
                        }
                    }
                    return productDetails;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public List<ProductDetailsByID> ProductDetailsByID()
        {
            throw new NotImplementedException();
        }

        public bool DeleteData(int id, string TableName, string ColName, string CurrentUser)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.DELETE_ELEMENT))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@tableName", TableName);
                        command.Parameters.AddWithValue("@colName", ColName);
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
                    //throw (ex);
                    return false;
                }
            }
        }

        public List<CategoryList> GetCateogryDataByID(int id)
        {
            List<CategoryList> CategoryDetails = new List<CategoryList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.CATEGORY_BYID))
                    {
                        command.Connection = connection;
                        connection.Open();

                        command.Parameters.AddWithValue("@id", id);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            CategoryDetails.Add(new CategoryList
                            {
                                CategoryId = reader.GetInt32(0).ToString(),
                                CategoryName = reader.GetString(1).ToString(),
                                CategoryDescription = reader.GetString(2).ToString()
                            });
                        }
                    }
                    return CategoryDetails;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public bool UpdateCategory(string CategoryName, string CategoryDescription, int CategoryID)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.UPDATE_CATEGORY))
                    {
                        command.Parameters.AddWithValue("@catName", CategoryName);
                        command.Parameters.AddWithValue("@catDesc", CategoryDescription);
                        command.Parameters.AddWithValue("@Id", CategoryID);
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

        public bool DeleteSubCategory(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.DELETE_SUBCATEGORY))
                    {
                        command.Parameters.AddWithValue("@id", id);
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
                    //throw (ex);
                    return false;
                }
            }
        }

        public bool UpdateCategory(string CategoryID, string SubCategoryName, string SubCategoryDescription, int SubCategoryID)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.UPDATE_SUBCATEGORY))
                    {
                        command.Parameters.AddWithValue("@catName", SubCategoryName);
                        command.Parameters.AddWithValue("@catDesc", SubCategoryDescription);
                        command.Parameters.AddWithValue("@Id", CategoryID);
                        command.Parameters.AddWithValue("@catId", SubCategoryID);
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

        public List<SubCategoryList> GetSubCateogryDataByID(int id)
        {
            List<SubCategoryList> SubCategoryDetails = new List<SubCategoryList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.SUBCATEGORY_BYID))
                    {
                        command.Connection = connection;
                        connection.Open();

                        command.Parameters.AddWithValue("@id", id);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            SubCategoryDetails.Add(new SubCategoryList
                            {
                                SubCategoryId = reader.GetInt32(0),
                                CategoryId = reader.GetInt32(1),
                                CategoryName = reader.GetString(2).ToString(),
                                SubCategoryName = reader.GetString(3).ToString(),
                                SubCategoryDesc = reader.IsDBNull(3) ? null : reader.GetString(3)
                            });
                        }
                    }
                    return SubCategoryDetails;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public List<ProductList> GetProductByID(int id)
        {
            List<ProductList> SubCategoryDetails = new List<ProductList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.PRODUCT_BYID))
                    {
                        command.Connection = connection;
                        connection.Open();

                        command.Parameters.AddWithValue("@id", id);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            SubCategoryDetails.Add(new ProductList
                            {
                                ProductID = reader.GetInt32(0),
                                ProductName = reader.GetString(1).ToString()
                            });
                        }
                    }
                    return SubCategoryDetails;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public List<StoreData> StoreData()
        {
            List<StoreData> StoreList = new List<StoreData>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.WHEREHOUSE_INFO))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            StoreList.Add(new StoreData
                            {
                                StoreID = reader.GetInt32(0),
                                StoreManager = reader.GetString(1).ToString(),
                                StoreName = reader.GetString(2).ToString(),
                                StoreRoomNumber = reader.GetString(3).ToString(),
                                StoreType = reader.GetString(4).ToString()
                            });
                        }
                    }
                    return StoreList;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public bool UpdateWareHouse(string StoreName, string StoreManager, string StoreRoomNumber, string StoreType, int StoreID)
        {
            using (SqlConnection connction = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand sqlcommand = new SqlCommand(UserAccountServiceSQL.UPDATE_WHAREHOUSE))
                    {
                        sqlcommand.Parameters.AddWithValue("@storeID", StoreID);
                        sqlcommand.Parameters.AddWithValue("@storeName", StoreName);
                        sqlcommand.Parameters.AddWithValue("@storeManager", StoreManager);
                        sqlcommand.Parameters.AddWithValue("@storeRoomNumber", StoreRoomNumber);
                        sqlcommand.Parameters.AddWithValue("@storeType", StoreType);


                        sqlcommand.Connection = connction;
                        connction.Open();


                        int cnt = sqlcommand.ExecuteNonQuery();
                        if (cnt == 1)
                            return true;
                        else
                            return false;
                    }
                }
                catch (SqlException ex)
                {
                    return false;
                }
            }
        }

        public bool DeleteWhareHouse(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.DELETE_WHAREHOUSE))
                    {
                        command.Parameters.AddWithValue("@id", id);
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

        public List<SupplierData> SupplierData()
        {
            List<SupplierData> SupplierList = new List<SupplierData>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.SUPPLIER_INFO))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            SupplierList.Add(new SupplierData
                            {
                                SupplierID = reader.GetInt32(0),
                                FullName = reader.GetString(1).ToString(),
                                Currency = reader.GetString(2).ToString(),
                                Address = reader.GetString(3).ToString(),
                                State = reader.GetString(6).ToString(),
                                PoCode = reader.GetString(7).ToString(),
                                Website = reader.GetString(8).ToString(),
                                Email = reader.GetString(9).ToString(),
                                ContactNumber = reader.GetString(10).ToString(),
                                ContactPerson = reader.GetString(11).ToString(),
                                Description = reader.GetString(12).ToString()
                            });
                        }
                    }
                    return SupplierList;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public bool UpdateSupplier(string FullName, string Currency, string Address, string State, string PoCode, string Website, string Email,
                            string ContactNumber, string ContactPerson, string Description, int SupplierID)
        {
            using (SqlConnection connction = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand sqlcommand = new SqlCommand(UserAccountServiceSQL.UPDATE_SUPPLIER))
                    {
                        sqlcommand.Parameters.AddWithValue("@supplierID", SupplierID);
                        sqlcommand.Parameters.AddWithValue("@fullName", FullName);
                        sqlcommand.Parameters.AddWithValue("@currency", Currency);
                        sqlcommand.Parameters.AddWithValue("@address", Address);
                        sqlcommand.Parameters.AddWithValue("@state", State);
                        sqlcommand.Parameters.AddWithValue("@poCode", PoCode);
                        sqlcommand.Parameters.AddWithValue("@webSite", Website);
                        sqlcommand.Parameters.AddWithValue("@email", Email);
                        sqlcommand.Parameters.AddWithValue("@contactNumber", ContactNumber);
                        sqlcommand.Parameters.AddWithValue("@contactPerson", ContactPerson);
                        sqlcommand.Parameters.AddWithValue("@description", Description);

                        sqlcommand.Connection = connction;
                        connction.Open();


                        int cnt = sqlcommand.ExecuteNonQuery();
                        if (cnt == 1)
                            return true;
                        else
                            return false;
                    }
                }
                catch (SqlException ex)
                {
                    return false;
                }
            }
        }

        public bool DeleteSupplier(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.DELETE_SUPPLIER))
                    {
                        command.Parameters.AddWithValue("@id", id);
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

        public List<ProductDataList> ProductDataList()
        {
            List<ProductDataList> ProductDataList = new List<ProductDataList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.PRODUCT_INFO))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            ProductDataList.Add(new ProductDataList
                            {
                                SupplierID = reader.GetInt32(0).ToString(),
                                SupplierName = reader.GetString(1).ToString(),
                                ProductID = reader.GetInt32(2),
                                ProductName = reader.IsDBNull(3) ? null : reader.GetString(3).ToString(),
                                ProductDesc = reader.IsDBNull(4) ? null : reader.GetString(4).ToString(),
                                CategoryID = reader.GetInt32(5),
                                CategoryName = reader.GetString(6).ToString(),
                                SubCategoryID = reader.GetInt32(7),
                                SubCategoryName = reader.GetString(8).ToString()
                            });
                        }
                    }
                    return ProductDataList;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public bool UpdateProduct(int ProductId, int CategoryID, int SupplierID, string ProductName, string ProductDesc, string currentUser)
        {
            using (SqlConnection connction = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand sqlcommand = new SqlCommand(UserAccountServiceSQL.UPDATE_PRODUCT))
                    {
                        sqlcommand.Parameters.AddWithValue("@productId", ProductId);
                        sqlcommand.Parameters.AddWithValue("@categoryID", CategoryID);
                        sqlcommand.Parameters.AddWithValue("@supplierID", SupplierID);
                        sqlcommand.Parameters.AddWithValue("@productName", ProductName);
                        sqlcommand.Parameters.AddWithValue("@productDesc", ProductDesc);


                        sqlcommand.Connection = connction;
                        connction.Open();


                        int cnt = sqlcommand.ExecuteNonQuery();
                        if (cnt == 1)
                            return true;
                        else
                            return false;
                    }
                }
                catch (SqlException ex)
                {
                    return false;
                }
            }
        }

        public bool DeleteProductById(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.DELETE_PRODUCT))
                    {
                        command.Parameters.AddWithValue("@id", id);
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

        public List<StockDataList> StockInfo()
        {
            List<StockDataList> StockList = new List<StockDataList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.STOCK_INFO))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            StockList.Add(new StockDataList
                            {

                                StockID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                ProductID = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                ProductName = reader.IsDBNull(9) ? null : reader.GetString(9).ToString(),
                                Quantity = reader.IsDBNull(2) ? null : reader.GetInt32(2).ToString(),
                                CategoryName = reader.IsDBNull(10) ? null : reader.GetString(10).ToString(),
                                SubCategoryName = reader.IsDBNull(11) ? null : reader.GetString(11).ToString(),
                                ManufactureDate = reader.GetDateTime(3).ToString("MM/dd/yyyy"),
                                ExpiryDate = reader.GetDateTime(4).ToString("MM/dd/yyyy"),
                                BarCode = reader.GetString(5).ToString(),
                                CreatedBy = reader.GetString(7).ToString(),
                                CreatedOn = reader.GetDateTime(8).ToString("MM/dd/yyyy")
                            });
                        }
                    }
                    return StockList;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public bool UpdateStock(int StockID, string ProductID, string Quantity, DateTime ManufactureDate, DateTime ExpiryDate, string BarCode)
        {
            using (SqlConnection connction = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand sqlcommand = new SqlCommand(UserAccountServiceSQL.UPDATE_STOCK))
                    {
                        sqlcommand.Parameters.AddWithValue("@ProductID", ProductID);
                        sqlcommand.Parameters.AddWithValue("@StockID", StockID);
                        sqlcommand.Parameters.AddWithValue("@Quantity", Quantity);
                        sqlcommand.Parameters.AddWithValue("@ManufactureDate", ManufactureDate);
                        sqlcommand.Parameters.AddWithValue("@ExpiryDate", ExpiryDate);
                        sqlcommand.Parameters.AddWithValue("@BarCode", BarCode);

                        sqlcommand.Connection = connction;
                        connction.Open();


                        int cnt = sqlcommand.ExecuteNonQuery();
                        if (cnt == 1)
                            return true;
                        else
                            return false;
                    }
                }
                catch (SqlException ex)
                {
                    return false;
                }
            }
        }

        public bool DeleteStockById(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.DELETE_STOCK))
                    {
                        command.Parameters.AddWithValue("@id", id);
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

        public bool DeleteCompanyById(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.DELETE_COMPANY))
                    {
                        command.Parameters.AddWithValue("@id", id);
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


        public List<ProductAssignedList> AssignedDataList()
        {
            List<ProductAssignedList> AssignedList = new List<ProductAssignedList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.PRODUCT_ASSIGNED_INFO))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            AssignedList.Add(new ProductAssignedList
                            {
                                UserName = reader.GetString(0).ToString(),
                                ProductId = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                Quantity = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                                AssignedDate = reader.GetDateTime(3).ToString(),
                                ProductName = reader.IsDBNull(5) ? null : reader.GetString(5).ToString(),
                                SubCategoryName = reader.IsDBNull(6) ? null : reader.GetString(6).ToString(),
                                CategoryName = reader.IsDBNull(7) ? null : reader.GetString(7).ToString(),
                                SubCategoryId = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                                CategoryId = reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                                RowId = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),
                                StockID = reader.GetInt32(11)
                            });
                        }
                    }
                    return AssignedList;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public bool AssignProductUpdate(string UserName, int Quantity, int ProductID, int Id)
        {
            using (SqlConnection connction = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand sqlcommand = new SqlCommand(UserAccountServiceSQL.UPDATE_ASSIGNED_PROD))
                    {
                        sqlcommand.Parameters.AddWithValue("@UserName", UserName);
                        sqlcommand.Parameters.AddWithValue("@Quantity", Quantity);
                        sqlcommand.Parameters.AddWithValue("@ProductID", ProductID);
                        sqlcommand.Parameters.AddWithValue("@Id", Id);

                        sqlcommand.Connection = connction;
                        connction.Open();


                        int cnt = sqlcommand.ExecuteNonQuery();
                        if (cnt == 1)
                            return true;
                        else
                            return false;
                    }
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        public bool DeleteAssignedProductId(int id)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.DELETE_ASSIGNED_PROD))
                    {
                        command.Parameters.AddWithValue("@id", id);
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
        public List<StockDataList> StockHistory()
        {
            List<StockDataList> AssignedList = new List<StockDataList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.STOCK_QTY))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            AssignedList.Add(new StockDataList
                            {
                                ProductID = reader.GetInt32(0),
                                IntQuantity = reader.GetInt32(1),
                                ProductName = reader.GetString(2).ToString(),
                                SubCategoryName = reader.GetString(3).ToString(),
                                CategoryName = reader.GetString(4).ToString()
                            });
                        }
                    }
                    return AssignedList;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public List<StockDataList> AssignedStockQty()
        {
            List<StockDataList> AssignedList = new List<StockDataList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.ASSIGNED_STOCK))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            AssignedList.Add(new StockDataList
                            {
                                ProductID = reader.GetInt32(0),
                                IntQuantity = reader.GetInt32(1)
                            });
                        }
                    }
                    return AssignedList;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }


        public bool CreateCompany(string CompanyShortName, string CompanyFullName, string CompanyLocation)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.CREATE_COMPANY))
                    {
                        command.Parameters.AddWithValue("@companyshortname", CompanyShortName);
                        command.Parameters.AddWithValue("@companyfullname", CompanyFullName);
                        command.Parameters.AddWithValue("@companylocation", CompanyLocation);



                        command.Connection = connection;
                        connection.Open();

                        int rowsEffected = command.ExecuteNonQuery();
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
                    logger.Error(ex, "Error while creating company");
                    throw new DbException("Error while executing Query");
                    //throw ex;
                }
            }

        }

        public List<CompanyList> GetAllCompany()
        {
            List<CompanyList> allCompany = new List<CompanyList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_ALL_COMPANY))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            allCompany.Add(ReadCompany(reader));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    logger.Error("Error while executing SQL Statement", ex);
                    throw new DbException("Error while executing SQL Statement");
                }
            }
            return allCompany;
        }

        private CompanyList ReadCompany(SqlDataReader reader)
        {
            return new CompanyList
            {
                // CompanyId = reader.GetInt32(0).Equals(),
                CompanyID = reader.GetInt32(0).ToString(),
                CompanyShortName = reader.GetString(1).ToString(),
                CompanyFullName = reader.GetString(2).ToString(),
                CompanyLocation = reader.GetString(3).ToString()
            };
        }

        public bool CreateDepartment(string CompanyID, string DepartmentName, string DepartmentDirNme, string DepartmentFloorno, string DepartmentRoomno)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.CREATE_DEPARTMENT))
                    {
                        command.Parameters.AddWithValue("@companyid", CompanyID);
                        command.Parameters.AddWithValue("@departmentname", DepartmentName);
                        command.Parameters.AddWithValue("@departmentdirnme", DepartmentDirNme);
                        command.Parameters.AddWithValue("@departmentfloorno", DepartmentFloorno);
                        command.Parameters.AddWithValue("@departmentroomno", DepartmentRoomno);

                        command.Connection = connection;
                        connection.Open();

                        int rowsEffected = command.ExecuteNonQuery();
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
                    logger.Error(ex, "Error while Creating Department");
                    throw new DbException("Error while executing SQL Query");
                }
            }
        }

        #region
        public List<DepartmentList> GetAllDepartment()
        {
            List<DepartmentList> allDepartment = new List<DepartmentList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_ALL_DEPARTMENT))
                    {
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            allDepartment.Add(ReadDepartment(reader));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
            return allDepartment;
        }

        public List<DepartmentList> GetAllDepartmentByCompanyId(int id)
        {
            List<DepartmentList> allDepartment = new List<DepartmentList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_ALL_DEPARTMENT_COMPANY_ID))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            allDepartment.Add(ReadDepartment(reader));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
            return allDepartment;
        }

        private DepartmentList ReadDepartment(SqlDataReader reader)
        {
            return new DepartmentList
            {

                DepartmentID = reader.GetInt32(0),
                DepartmentName = reader.GetString(1).ToString(),
                DepartmentDirNme = reader.GetString(2).ToString(),
                DepartmentFloorno = reader.GetString(3).ToString(),
                DepartmentRoomno = reader.GetInt32(4),
                CompanyID = reader.GetInt32(5),
                CompanyFullName = reader.GetString(6)

            };
        }
        #endregion

        public bool RequirementGeneralForm(string CompanyID, string DepartmentID, string ProductName, string ProductDescription, string ProductQuantity, string CreationDate, string RequirementDate, string OfficeNo, string SubmittedBy)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {

                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.REQUIREMENT_GENERALFORM))
                    {
                        DateTime CreationDates = DateTime.Now;
                        command.Parameters.AddWithValue("@companyid", CompanyID);
                        command.Parameters.AddWithValue("@departmentid", DepartmentID);
                        command.Parameters.AddWithValue("@productname", ProductName);
                        command.Parameters.AddWithValue("@productdescription", ProductDescription);
                        command.Parameters.AddWithValue("@productquantity", ProductQuantity);
                        command.Parameters.AddWithValue("@creationdate", CreationDates);
                        command.Parameters.AddWithValue("@requirementdate", RequirementDate);
                        command.Parameters.AddWithValue("@officeno", OfficeNo);
                        command.Parameters.AddWithValue("@submittedby", SubmittedBy);

                        command.Connection = connection;
                        connection.Open();

                        int rowsEffected = command.ExecuteNonQuery();
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
                    throw ex;
                }
            }
        }

        private bool isDataExistsCondition(string tableName, string col1, string col2, string col1Data, string col2Data)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.CheckDublicate))
                    {
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@col1", col1);
                        command.Parameters.AddWithValue("@col2", col2);
                        command.Parameters.AddWithValue("@col1Data", col1Data);
                        command.Parameters.AddWithValue("@col2Data", col2Data);
                        command.Connection = connection;
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
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

        public bool CreateEmployee(int CompanyID, int DepartmentID, string Designation, string UserName, string Email, out bool userExist)
        {
            if (isUserExists(UserName))
            {
                userExist = true;
                return true;
            }
            if (!string.IsNullOrEmpty(Email))
            {
                if (isEmailExists(Email))
                {
                    userExist = true;
                    return true;
                }
            }

            if (isUserUserTab(UserName, Email, CompanyID, DepartmentID))
            {
                if (isUserEmpTab(CompanyID, DepartmentID, Designation, UserName, Email))
                {
                    userExist = false;
                    return true;
                }
                else
                {
                    DeleteUser(UserName);
                    userExist = false;
                    return false;
                }
            }
            else
            {
                userExist = true;
                return false;
            }
        }

        private bool isUserEmpTab(int CompanyID, int DepartmentID, string Designation, string UserName, string Email)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.CREATE_EMPLOYEE))
                    {
                        DateTime createdAt = DateTime.Now;
                        command.Parameters.AddWithValue("@companyid", CompanyID);
                        command.Parameters.AddWithValue("@departmentid", DepartmentID);
                        command.Parameters.AddWithValue("@designation", Designation);
                        command.Parameters.AddWithValue("@username", UserName);

                        if (string.IsNullOrEmpty(Email))
                            command.Parameters.AddWithValue("@email", "");
                        else
                            command.Parameters.AddWithValue("@email", Email);

                        command.Connection = connection;
                        connection.Open();
                        int rowsEffected = command.ExecuteNonQuery();
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
                    return false;
                }
            }
        }
        private bool isUserUserTab(string UserName, string Email, int CompanyId, int DepartmentId)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.CREATE_USER))
                    {
                        string passwordHashed = PasswordHash.CreateHash("password");
                        DateTime createdAt = DateTime.Now;
                        command.Parameters.AddWithValue("@username", UserName);
                        command.Parameters.AddWithValue("@password", passwordHashed);
                        if (string.IsNullOrEmpty(Email))
                            command.Parameters.AddWithValue("@email", "");
                        else
                            command.Parameters.AddWithValue("@email", Email);

                        command.Parameters.AddWithValue("@creationDate", createdAt);
                        command.Parameters.AddWithValue("@comment", "");
                        command.Parameters.AddWithValue("@createdBy", "");

                        command.Parameters.AddWithValue("@companyid", CompanyId);
                        command.Parameters.AddWithValue("@departmentid", DepartmentId);

                        command.Connection = connection;
                        connection.Open();
                        int rowsEffected = command.ExecuteNonQuery();
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

        private bool DeleteUser(string UserName)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.DELETE_USER))
                    {
                        command.Parameters.AddWithValue("@username", UserName);
                        command.Connection = connection;
                        connection.Open();
                        int rowsEffected = command.ExecuteNonQuery();
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

        public bool RequirementSaved(int catID, int subCatID, int prodID, int quantityNumber, string currentUser, string userName)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.SAVE_REQUIREMENT))
                    {
                        DateTime createdAt = DateTime.Now;
                        string randomNumber = DateTime.Now.ToString("yyyyMMddHHmmss");

                        command.Parameters.AddWithValue("@companyid", '1');
                        command.Parameters.AddWithValue("@deptid", '1');
                        command.Parameters.AddWithValue("@catid", catID);
                        command.Parameters.AddWithValue("@subcatid", subCatID);
                        command.Parameters.AddWithValue("@prodname", prodID);
                        command.Parameters.AddWithValue("@prodquantity", quantityNumber);
                        command.Parameters.AddWithValue("@requestuid", randomNumber);
                        command.Parameters.AddWithValue("@cdate", createdAt);
                        command.Parameters.AddWithValue("@submitedby", currentUser);
                        command.Parameters.AddWithValue("@username", userName);

                        command.Connection = connection;
                        connection.Open();

                        int rowsEffected = command.ExecuteNonQuery();
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
                    return false;
                }
            }
        }

        public List<UserList> GetUserByDeptComID(int deptID, int CompanyID)
        {
            List<UserList> allUsers = new List<UserList>();
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(UserAccountServiceSQL.GET_ALL_USERS_ID))
                    {

                        command.Parameters.AddWithValue("@deptid", deptID);
                        command.Parameters.AddWithValue("@companyid", CompanyID);
                        command.Connection = connection;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            allUsers.Add(ReadUserById(reader));
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
    }
}

    #region SQL Statements
    internal class UserAccountServiceSQL
    {
        public static readonly string CREATE_USER = "INSERT INTO[SIBFInventory].[dbo].[Users]([Username],[Password],[Email],[CreationDate],[IsApproved] " +
                         ",[IsLockedOut],[LastActivityDate],[LastLoginDate],[LastPasswordChangedDate],[Comment],[CreatedBy],[ModifiedBy], [CompanyID], [DepartmentID]) " +
                         "VALUES(@username, @password, @email, @creationDate, 1, 0, null, null, null, @comment, @createdBy, null, @companyid, @departmentid)";

        public static readonly string CREATE_CATEGORY = "INSERT INTO [SIBFInventory].[dbo].[ItemCategory]" +
                                                        "([item_category_title], [item_category_desc], [created_date])" +
                                                         "VALUES(@categoryname, @categorydescription, @createddate)";

        public static readonly string CREATE_SUBCATEGORY = "INSERT INTO [SIBFInventory].[dbo].[SubItemCategory]" +
                                                            "([sub_item_category_title],[item_category_id], [sub_item_category_desc], [created_date])" +
                                                            "VALUES(@subcategoryname, @categoryid,@subcategorydescription, @createddate)";

        public static readonly string USER_FOUND = "SELECT 1 FROM [SIBFInventory].[dbo].[Users] WHERE [Username] = @username";
        public static readonly string USER_PASSWORD = "SELECT [Password] FROM [SIBFInventory].[dbo].[Users] WHERE [Username] = @username";
        public static readonly string EMAIL_FOUND = "SELECT 1 FROM [SIBFInventory].[dbo].[Users] WHERE [Email] = @email";

        public static readonly string GET_ALL_USERS = "SELECT [Username],[Password],[Email],[CreationDate],[IsApproved],[IsLockedOut],[LastActivityDate],[LastLoginDate],[LastPasswordChangedDate],[Comment],[CreatedBy],[ModifiedBy],[UserMustChangePassword] FROM [SIBFInventory].[dbo].[Users]";
        public static readonly string GET_ALL_CATEGORY = "SELECT [item_category_id],[item_category_title],[item_category_desc] FROM[SIBFInventory].[dbo].[ItemCategory]";

        public static readonly string GET_ALL_SUBCATEGORY = "SELECT [sub_item_category_id], [item_category_id],[sub_item_category_title],[sub_item_category_desc] FROM[SIBFInventory].[dbo].[SubItemCategory] ORDER BY [created_date] DESC";

        public static readonly string GET_CAT_SUBCAT = "SELECT SubItemCategory.sub_item_category_id, SubItemCategory.item_category_id" +
                                                       ",SubItemCategory.sub_item_category_title,SubItemCategory.sub_item_category_desc" +
                                                       ",ItemCategory.item_category_id ,ItemCategory.item_category_title " +
                                                       "FROM SubItemCategory JOIN ItemCategory ON SubItemCategory.item_category_id =ItemCategory.item_category_id";


    public static readonly string GET_CAT_SUBCAT2 = "SELECT [item_category_id], [item_category_title], [item_category_desc], [created_date]" +
                                                      "FROM [SIBFInventory].[dbo].[ItemCategory]";


    public static readonly string GET_ALL_SUPPLIERS = "SELECT [ID],[FullName],[Currency],[Address],[Country],[City],[State],[PoCode],[Website]" +
                                                          ",[Email],[ContactNumber],[ContactPerson],[Description],[CreatedOn],[CreateBy] " +
                                                          "FROM [SIBFInventory].[dbo].[ProductSupplier]";

        public static readonly string GET_ALL_PRODUCTS = "SELECT [Product_ID],[SubCategory_ID],[Product_Name],[Product_Desc],[Created_By],[Created_On],[Supplier_ID]" +
                                                         " FROM [SIBFInventory].[dbo].[Products]";

        public static readonly string LOCK_UNLOCK_ACCOUNT = "UPDATE [SIBFInventory].[dbo].[Users] SET IsLockedOut=@isLockedOut WHERE [Username] = @username";

    //public static readonly string CheckDublicate = "SELECT * FROM @tableName WHERE @col1=@col1Data AND @col2=@col2Data";
    public static readonly string CheckDublicate = "SELECT * FROM SubItemCategory WHERE item_category_id=@col1Data AND sub_item_category_title=@col2Data";
    #region ROLES
    public static readonly string GET_ALL_ROLES_BY_USER = "SELECT [Name],[Description],[Description_AR] " +
                                                      "FROM [SIBFInventory].[dbo].[Roles] r, [SIBFInventory].[dbo].[UserRole] ur " +
                                                      "WHERE r.Name = ur.RoleName AND ur.Username = @username";
        public static readonly string GET_ALL_ROLES = "SELECT [Name],[Description],[Description_AR] FROM [SIBFInventory].[dbo].[Roles]";
        public static readonly string CREATE_ROLE = "INSERT INTO [SIBFInventory].[dbo].[Roles]([Name],[Description],[Description_AR]) " +
                                                    "VALUES(@name, @description, @descriptionArabic)";
        public static readonly string ROLE_FOUND = "SELECT 1 FROM[SIBFInventory].[dbo].[Roles] WHERE [Name] = @name";
        public static readonly string USER_FOUND_FOR_ROLE = "SELECT TOP 1 1 FROM[SIBFInventory].[dbo].[UserRole] WHERE [RoleName] = @name";
        public static readonly string DELETE_ROLE = "DELETE FROM[SIBFInventory].[dbo].[UserRole] WHERE [RoleName] = @name";
        public static readonly string ASSIGN_ROLE = "[dbo].[sp_assign_roles]";

        public static readonly string CREATE_WAREHOUSE = "INSERT INTO [SIBFInventory].[dbo].[WareHouse]([StoreName],[StoreManager],[StoreRoomNumber]" +
                                                         ",[StoreType],[CreatedBy],[CreatedOn])" +
                                                         "VALUES(@StoreName, @StoreManager, @StoreRoomNumber, @StoreType, @currentUser, @creationDate)";
        public static readonly string CREATE_SUPPLIER = "INSERT INTO [SIBFInventory].[dbo].[ProductSupplier]([FullName],[Currency],[Address],[Country],[City]" +
                                                        ",[State],[PoCode],[Website],[Email],[ContactNumber],[ContactPerson],[Description],[CreatedOn],[CreateBy])" +
                                                         "VALUES(@FullName, @Currency, @Address, @Country, @City, @State, @PoCode, @Website, @Email, @ContactNumber, @ContactPerson, @Description, @createdAt, @CurrentUser)";

        public static readonly string CREATE_PRODUCT = "INSERT INTO [SIBFInventory].[dbo].[Products]([SubCategory_ID],[Supplier_ID], [Product_Name],[Product_Desc],[Created_By],[Created_On], [Product_Type])" +
                                                        "VALUES(@Category_ID,@Supplier_ID,@Product_Name,@Product_Desc,@Created_By,@Created_On,@ProductType)";

        public static readonly string ADD_STOCK = "INSERT INTO [SIBFInventory].[dbo].[StockHistory]([Product_ID],[Quantity]" +
                                                  ",[Manufacture_Date],[Expiry_Date],[BarCode],[Img],[Created_By],[Created_On])" +
                                                  "VALUES(@ProductID, @Quantity, @ManufactureDate, @ExpiryDate, @BarCode, NULL, @CurrentUser, @Created_On)";
        public static readonly string ASSIGN_PRODUCT_USER = "INSERT INTO [SIBFInventory].[dbo].[UserProducts]" +
                                                            "([Username],[Product_ID],[Requirement_ID],[Stock_ID],[Quantity],[Reason],[Assigned_Date],[Assigned_By])" +
                                                            "VALUES(@UserName, @ProductID, @RequirementID, @StockID,@Quantity, @Reason, @CreatedOn, @CurrentUser)";

        public static readonly string GET_ALL_COUNTRIES = "SELECT [Country_ID],[Sort_Name],[Name],[Phone_Code] FROM [SIBFInventory].[dbo].[Countries]";

        public static readonly string GET_ALL_STATES = "SELECT [State_ID],[Name],[Country_ID] FROM [SIBFInventory].[dbo].[States] WHERE [Country_ID] = @CountryID";

        public static readonly string GET_ALL_CITIES = "SELECT [City_ID],[Name],[State_ID] FROM [SIBFInventory].[dbo].[States] WHERE [Country_ID] = @StateID";

        public static readonly string GET_ALL_STATES_UAE = "SELECT [State_ID],[Name],[Country_ID] FROM [SIBFInventory].[dbo].[States] WHERE[Country_ID] = 229";

        public static readonly string GET_PRODUCT_BYIDOLD = "SELECT Product_Name, Product_Desc," +
                                                         "(SELECT item_category_title FROM[ItemCategory] WHERE item_category_id = " +
                                                         "(select item_category_id FROM[SIBFInventory].[dbo].[subItemCategory] WHERE sub_item_category_id = " +
                                                         "(SELECT SubCategory_ID FROM[SIBFInventory].[dbo].[Products] WHERE Product_ID = @ProductID))) AS CategoryName, " +
                                                         "(SELECT sub_item_category_title FROM[SIBFInventory].[dbo].[subItemCategory] WHERE sub_item_category_id = " +
                                                         "(SELECT SubCategory_ID FROM[SIBFInventory].[dbo].[Products] WHERE Product_ID = @ProductID)) AS SubCategoryName, " +
                                                         "(SELECT FullName FROM[SIBFInventory].[dbo].[ProductSupplier] WHERE ID = " +
                                                         "(SELECT Supplier_ID FROM[SIBFInventory].[dbo].[Products] WHERE Product_ID = @ProductID)) AS CompanyName," +
                                                         "(SELECT ISNULL(SUM(Quantity),0) FROM[SIBFInventory].[dbo].[StockHistory] WHERE Product_ID = @ProductID) - " +
                                                         "((SELECT ISNULL(SUM(Quantity),0) FROM[SIBFInventory].[dbo].[UserProducts] WHERE Product_ID = @ProductID)) AS AQuantity " +
                                                         "FROM[SIBFInventory].[dbo].[Products] " +
                                                         "WHERE Product_ID = @ProductID";
       public static readonly string GET_PRODUCT_BYID = "SELECT SH.Stock_ID, SH.Quantity, SH.Manufacture_Date, SH.Expiry_Date " +
                                                        "FROM[SIBFInventory].[dbo].[StockHistory] SH LEFT JOIN[SIBFInventory].[dbo].[Products] P " +
                                                        "ON SH.Product_ID = P.Product_ID WHERE  SH.Product_ID = @ProductID AND SH.Quantity !=0 ORDER BY SH.Created_On DESC";

        public static readonly string DELETE_ELEMENT = "DELETE [SIBFInventory].[dbo].[ItemCategory] WHERE [item_category_id] = @id";

        public static readonly string CATEGORY_BYID = "SELECT [item_category_id], [item_category_title], [item_category_desc],[created_date] FROM [SIBFInventory].[dbo].[ItemCategory] WHERE [item_category_id] = @id";

        public static readonly string UPDATE_CATEGORY = "UPDATE [SIBFInventory].[dbo].[ItemCategory] SET [item_category_title]= @catName, [item_category_desc]= @catDesc  WHERE [item_category_id] = @Id";

        public static readonly string DELETE_SUBCATEGORY = "DELETE [SIBFInventory].[dbo].[SubItemCategory] WHERE [sub_item_category_id] = @id";

        public static readonly string UPDATE_SUBCATEGORY = "UPDATE [SIBFInventory].[dbo].[SubItemCategory] SET [item_category_id]= @Id, [sub_item_category_title]= @catName, [sub_item_category_desc]= @catDesc  WHERE [sub_item_category_id] = @catId";

        public static readonly string SUBCATEGORY_BYID = "SELECT SubItemCategory.sub_item_category_id, SubItemCategory.item_category_id, ItemCategory.item_category_title," +
                                                         "SubItemCategory.sub_item_category_title,SubItemCategory.sub_item_category_desc FROM SubItemCategory JOIN ItemCategory " +
                                                         "ON SubItemCategory.item_category_id = ItemCategory.item_category_id WHERE SubItemCategory.item_category_id = @id";

        public static readonly string WHEREHOUSE_INFO = "SELECT ID, StoreManager, StoreName, StoreRoomNumber, StoreType FROM  [SIBFInventory].[dbo].[WareHouse]";

        public static readonly string UPDATE_WHAREHOUSE = "UPDATE [SIBFInventory].[dbo].[WareHouse] SET [StoreName] = @storeName,[StoreManager] = @storeManager," +
                                                          "[StoreRoomNumber] = @storeRoomNumber,[StoreType] = @storeType" +
                                                          " WHERE ID=@storeID";

        public static readonly string DELETE_WHAREHOUSE = "DELETE [SIBFInventory].[dbo].[WareHouse] WHERE [ID] = @id";

        public static readonly string SUPPLIER_INFO = "SELECT [ID],[FullName],[Currency],[Address],[Country],[City],[State],[PoCode]" +
                                                      ",[Website],[Email],[ContactNumber],[ContactPerson],[Description],[CreatedOn],[CreateBy]" +
                                                       "FROM [SIBFInventory].[dbo].[ProductSupplier]";

        public static readonly string UPDATE_SUPPLIER = "UPDATE [SIBFInventory].[dbo].[ProductSupplier] SET" +
                                                        "[FullName] =@fullName,[Currency] = @currency,[Address] = @address," +
                                                        "[State] =@state,[PoCode]=@poCode,[Website]=@webSite," +
                                                        "[Email] =@email,[ContactNumber] = @contactNumber,[ContactPerson] = @contactPerson," +
                                                        "[Description]= @description WHERE ID = @supplierID";

        public static readonly string DELETE_SUPPLIER = "DELETE [SIBFInventory].[dbo].[ProductSupplier] WHERE [ID] = @id";

        public static readonly string PRODUCT_INFO = "SELECT[Products].Supplier_ID,[ProductSupplier].FullName,[Products].Product_ID, [Products].Product_Name, " +
                                                        "[Products].Product_Desc,[ItemCategory].item_category_id,[ItemCategory].item_category_title, " +
                                                        "[Products].SubCategory_ID, [SubItemCategory].sub_item_category_title FROM " +
                                                        "[SIBFInventory].[dbo].[Products] " +
                                                        "INNER JOIN [SIBFInventory].[dbo].[ProductSupplier] " +
                                                        "ON [Products].Supplier_ID=[ProductSupplier].ID " +
                                                        "INNER JOIN[SIBFInventory].[dbo].[SubItemCategory] " +
                                                        "ON [Products].SubCategory_ID = [SubItemCategory].sub_item_category_id " +
                                                        "INNER JOIN[SIBFInventory].[dbo].[ItemCategory] " +
                                                        "ON [SubItemCategory].item_category_id = [ItemCategory].item_category_id";

        public static readonly string UPDATE_PRODUCT = "UPDATE [SIBFInventory].[dbo].[Products] SET " +
                                                       "SubCategory_ID = @categoryID, Product_Name = @productName, Product_Desc = @ProductDesc," +
                                                       "Supplier_ID=@supplierID WHERE Product_ID =@productId";

        public static readonly string DELETE_PRODUCT = "DELETE [SIBFInventory].[dbo].[Products] WHERE [Product_ID] = @id";

        public static readonly string STOCK_INFO = "SELECT [StockHistory].[Stock_ID], [StockHistory].[Product_ID], [StockHistory].[Quantity], [StockHistory].[Manufacture_Date]" +
                                                   ",[StockHistory].[Expiry_Date], [StockHistory].[BarCode],[StockHistory].[Img]" +
                                                   ",[StockHistory].[Created_By],[StockHistory].[Created_On], [Products].Product_Name, [SubItemCategory].sub_item_category_title , " +
                                                   "[ItemCategory].item_category_title FROM[SIBFInventory].[dbo].[StockHistory] " +
                                                   "INNER JOIN[SIBFInventory].[dbo].[Products] ON [StockHistory].Product_ID = [Products].Product_ID " +
                                                   "INNER JOIN[SIBFInventory].[dbo].[SubItemCategory]ON [Products].SubCategory_ID = [SubItemCategory] .sub_item_category_id " +
                                                   "INNER JOIN[SIBFInventory].[dbo].[ItemCategory] ON [SubItemCategory] .item_category_id = [ItemCategory].item_category_id " +
                                                   "ORDER BY [StockHistory].[Product_ID]";
        public static readonly string UPDATE_STOCK = "UPDATE [SIBFInventory].[dbo].[StockHistory] SET " +
                                                     "Product_ID = @ProductID, Quantity= @Quantity, Manufacture_Date= @ManufactureDate," +
                                                     "Expiry_Date = @ExpiryDate, BarCode= @BarCode WHERE Stock_ID=@StockID";

        public static readonly string DELETE_STOCK = "DELETE [SIBFInventory].[dbo].[StockHistory] WHERE [Stock_ID] = @id";

        public static readonly string PRODUCT_ASSIGNED_INFO = "SELECT [UserProducts].[Username],[UserProducts].[Product_ID],[UserProducts].[Quantity]" +
                                                              ",[UserProducts].[Assigned_Date],[UserProducts].Product_ID,[Products].Product_Name" +
                                                              ",[SubItemCategory].sub_item_category_title,[ItemCategory].item_category_title " +
                                                              ",[SubItemCategory].sub_item_category_id,[ItemCategory].item_category_id, [UserProducts].[Row_ID], [UserProducts].[Stock_ID] " +
                                                              "FROM [SIBFInventory].[dbo].[UserProducts] JOIN [SIBFInventory].[dbo].[Products] " +
                                                              "ON [UserProducts].Product_ID = [Products].Product_ID JOIN [SIBFInventory].[dbo].[SubItemCategory] " +
                                                              "ON [Products].SubCategory_ID = [SubItemCategory].sub_item_category_id JOIN [SIBFInventory].[dbo].[ItemCategory] " +
                                                              "ON [SubItemCategory] .item_category_id = [ItemCategory].item_category_id";

        public static readonly string UPDATE_ASSIGNED_PROD = "UPDATE [SIBFInventory].[dbo].[UserProducts] SET " +
                                                             "Product_ID = @ProductID, Quantity= @Quantity, Username=@UserName WHERE Row_ID=@Id";

        public static readonly string DELETE_ASSIGNED_PROD = "DELETE [SIBFInventory].[dbo].[UserProducts] WHERE [Row_ID] = @id";

        public static readonly string STOCK_QTY = "SELECT [StockHistory].[Product_ID], SUM([StockHistory].[Quantity]) As HQuantity " +
                                                      ",[Products].Product_Name,[SubItemCategory].sub_item_category_title,[ItemCategory].item_category_title " +
                                                      "FROM[SIBFInventory].[dbo].[StockHistory] INNER JOIN[SIBFInventory].[dbo].[Products] ON[StockHistory].Product_ID = [Products].Product_ID " +
                                                      "INNER JOIN[SIBFInventory].[dbo].[SubItemCategory]ON[Products].SubCategory_ID = [SubItemCategory].sub_item_category_id " +
                                                      "INNER JOIN[SIBFInventory].[dbo].[ItemCategory] ON[SubItemCategory].item_category_id = [ItemCategory].item_category_id " +
                                                      //"WHERE[StockHistory].Product_ID IN(SELECT[UserProducts].[Product_ID] FROM[SIBFInventory].[dbo].[UserProducts]) " +
                                                      "GROUP BY[StockHistory].Product_ID,[Products].Product_Name,[SubItemCategory].sub_item_category_title,[ItemCategory].item_category_title " +
                                                      "order by Product_ID";
        public static readonly string ASSIGNED_STOCK = "SELECT Product_ID, SUM(Quantity) AS QUNAITTY FROM [UserProducts] GROUP BY[UserProducts].Product_ID";

        public static readonly string CREATE_COMPANY = "INSERT INTO [SIBFInventory].[dbo].[CreateCompany]" +
                                                         "([Company_ShortName],[Company_FullName],[Company_Location])" +
                                                          "VALUES(@companyshortname, @companyfullname, @companylocation)";

        public static readonly string CREATE_DEPARTMENT = "INSERT INTO [SIBFInventory].[dbo].[CreateDepartment]" +
                                                            "([Company_ID],[Department_Name],[Department_DirNme],[Department_FloorNo],[Department_RoomNo])" +
                                                            "VALUES(@companyid, @departmentname,@departmentdirnme, @departmentfloorno,@departmentroomno)";

        public static readonly string GET_ALL_COMPANY = "SELECT [Company_ID],[Company_ShortName],[Company_FullName],[Company_Location] FROM[SIBFInventory].[dbo].[CreateCompany]";

        public static readonly string GET_ALL_DEPARTMENT = "SELECT [Department_ID],[Department_Name], [Department_DirNme],[Department_FloorNo], [Department_RoomNo], [CreateDepartment].[Company_ID], [Company_FullName]" +
                                                            " FROM [SIBFInventory].[dbo].[CreateDepartment] INNER JOIN [SIBFInventory].[dbo].[CreateCompany] ON[CreateDepartment].Company_ID = [CreateCompany].Company_ID";

        public static readonly string REQUIREMENT_GENERALFORM = "INSERT INTO[SIBFInventory].[dbo].[GeneralRequirementForm]" +
                                                            "([Company_ID], [Department_ID],[Product_Name],[Product_Description],[Product_Quantity],[Creation_Date],[Requirement_Date],[Office_No],[Submitted_By])" +
                                                             "VALUES(@companyid,@departmentid,@productname,@productdescription,@productquantity,@creationdate,@requirementdate,@officeno,@submittedby)";
    public static readonly string PRODUCT_BYID = "SELECT [Product_ID], [Product_Name] FROM [SIBFInventory].[dbo].[Products] WHERE [SubCategory_ID] =@id";

    public static readonly string DELETE_COMPANY = "DELETE FROM [SIBFInventory].[dbo].[CreateCompany] WHERE [Company_ID] = @id";

    public static readonly string GET_ALL_DEPARTMENT_COMPANY_ID = "SELECT [Department_ID],[Department_Name], [Department_DirNme],[Department_FloorNo], [Department_RoomNo], [CreateDepartment].[Company_ID], [Company_FullName]" +
                                                            " FROM [SIBFInventory].[dbo].[CreateDepartment] INNER JOIN [SIBFInventory].[dbo].[CreateCompany] ON[CreateDepartment].Company_ID = [CreateCompany].Company_ID" +
                                                            " WHERE [CreateDepartment].Company_ID = @id";

    public static readonly string CREATE_EMPLOYEE = "INSERT INTO[SIBFInventory].[dbo].[Employee]([full_name],[designation],[email], [company_id], [department_id])" +
                                                         "VALUES(@username, @designation, @email, @companyid, @departmentid)";

    public static readonly string DELETE_USER = "DELETE [SIBFInventory].[dbo].[Users] WHERE [Username] = @username";

    public static readonly string SAVE_REQUIREMENT = "INSERT INTO [SIBFInventory].[dbo].[GeneralRequirementForm] ([Company_ID],[Department_ID], [Product_Name], [Product_Quantity]" +
                                                     ", [Creation_Date], [Submitted_By], [Requested_UID], [Category_ID], [SubCategory_ID], [Submitted_For])" +
                                                     "VALUES(@companyid, @deptid, @prodname, @prodquantity, @cdate, @submitedby, @requestuid, @catid, @subcatid, @username)";
    public static readonly string GET_ALL_USERS_ID = "SELECT [Username],[Email],[CreationDate],[CompanyID],[DepartmentID] FROM[SIBFInventory].[dbo].[Users] "+
                                                     " WHERE [DepartmentID]=@deptid AND [CompanyID]=@companyid";

    public static readonly string Reduce_Quantity = "UPDATE [SIBFInventory].[dbo].[StockHistory] SET "+
                                                    "Quantity = (SELECT Quantity FROM [StockHistory] WHERE Stock_ID=@StockID) - @QtyReduce "+
                                                    "WHERE Stock_ID=@StockID";

    public static readonly string UPDATEPRODQTYRTN= "UPDATE [SIBFInventory].[dbo].[StockHistory] SET " +
                                                    "Quantity = (SELECT Quantity FROM [StockHistory] WHERE Stock_ID=@stockID) + @quantity " +
                                                    "WHERE Stock_ID=@stockID";

    #endregion
}
#endregion
