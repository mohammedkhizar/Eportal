using System;
using System.Collections.Generic;
using System.Data;

namespace SIBF.UserManagement.Api
{
    public interface IUserAccountService
    {
        List<MembershipUser> GetAllUsers(int page, int size, out int totalRecords);
        List<MembershipUser> GetAllUsers();

        List<CategoryList> GetAllCategory();

        List<SubCategoryList> SubCategory();

        List<SupplierList> SupplierList();

        List<ProductList> ProductList();
        
        List<CountriesList> CountriesList();

        List<UAEStatesList> UAEStatesList();
        List<StatesList> StatesList(int CountryID);

        List<CitiesList> CitiesList(int StateID);
        List<ProductDetails> ProductDetails(int ProductID);

        List<ProductDetailsByID> ProductDetailsByID();
        MembershipUser CreateUser(string username, string password, string email, string createdBy, string comment, int companyID, int departmentID,
            out MembershipCreateStatus createStatus);
        //void DeleteUser(string username);

        List<MembershipRole> GetAllRoles();

        List<MembershipRole> GetRolesForUser(string username);

        void AddUserToRoles(MembershipUser user, string assignedBy, string[] roles);

        void CreateRole(MembershipRole role);

        bool DeleteRole(string roleName);

        bool UnlockUser(string username, bool isLocked);

        void AddRemoveRoleForUser(string username, string rolename, bool isInRole);
        bool ValidateUser(string username, string password);
        MembershipUser CreateCategory(string categoryname, string categorydescription, out MembershipCreateStatus createStatus);
        //MembershipUser CreateSubCategory(string categoryid, string subcategoryname, string subcategorydescription, out MembershipCreateStatus createStatus);
        ProductCatSubCatInstance CreateSubCategory(string categoryid, string subcategoryname, string subcategorydescription, out ProductCategorySubCategory createStatus);
        //MembershipUser CreateSubCategory(string subCategoryName, string subCategoryDescription, out MembershipCreateStatus createStatus);
        bool CreateWareHouse(string StoreName, string StoreManager, string StoreRoomNumber, string StoreType, string currentUser);

        bool CreateSupplier(string FullName, string Currency, string Address, string State, string PoCode, string Website, string Email, string ContactNumber, string ContactPerson, string Description, string CurrentUser);

        bool CreateProduct(int CategoryID, int SupplierID, string ProductName, string ProductDesc, string CreatedBy, string ProductType);

        bool EnterStock(string ProductID, string Quantity, DateTime ManufactureDate, DateTime ExpiryDate, string BarCode, string currentUser);

        bool AssignProduct(string UserName, int Quantity, int ProductID, string CurrentUser, int stockID, int Requirement_ID, string Reason);
        bool ReduceQuantity(int Quantity, int ProductID);
        int ProductData(int ProductID);
        bool DeleteData(int id, string TableName, string ColName, string CurrentUser);
        List<CategoryList> GetCateogryDataByID(int id);
        bool UpdateCategory(string CategoryName, string CategoryDescription, int CategoryID);
        bool DeleteSubCategory(int id);
        bool UpdateCategory(string CategoryID, string SubCategoryName, string SubCategoryDescription, int SubCategoryID);

        List<SubCategoryList> GetSubCateogryDataByID(int id);

        List<ProductList> GetProductByID(int id);
        List<StoreData> StoreData();

        bool UpdateWareHouse(string StoreName, string StoreManager, string StoreRoomNumber, string StoreType, int StoreID);

        bool DeleteWhareHouse(int id);

        List<SupplierData> SupplierData();

        bool UpdateSupplier(string FullName, string Currency,string Address, string State, string PoCode, string Website, string Email, 
                            string ContactNumber, string ContactPerson, string Description, int SupplierID);

        bool DeleteSupplier(int id);

        List<ProductDataList> ProductDataList();
        bool UpdateProduct(int ProductId, int CategoryID, int SupplierID, string ProductName, string ProductDesc, string currentUser);
        bool DeleteProductById(int id);
        List<StockDataList> StockInfo();
        bool UpdateStock(int StockID, string ProductID, string Quantity, DateTime ManufactureDate, DateTime ExpiryDate, string BarCode);

        bool DeleteStockById(int id);
        bool DeleteCompanyById(int id);
        List<ProductAssignedList> AssignedDataList();

        bool AssignProductUpdate(string UserName,int Quantity, int ProductID, int Id);
        bool DeleteAssignedProductId(int id);

        bool updateProductRtnQty(int stockID, int quantity);
        List<StockDataList> StockHistory();
        List<StockDataList> AssignedStockQty();
        
        List<CompanyList> GetAllCompany();
        List<DepartmentList> GetAllDepartment();
        List<DepartmentList> GetAllDepartmentByCompanyId(int id);
        bool CreateCompany(string CompanyShortName, string CompanyFullName, string CompanyLocation);

        bool CreateDepartment(string CompanyID, string DepartmentName, string DepartmentDirNme, string DepartmentFloorno, string DepartmentRoomno);
        bool RequirementGeneralForm(string CompanyID, string DepartmentID, string ProductName, string ProductDescription, string ProductQuantity, string CreationDate, string RequirementDate, string OfficeNo, string SubmittedBy);

        bool CreateEmployee(int CompanyID,int DepartmentID, string Designation, string UserName, string Email, out bool userExist);

        bool RequirementSaved(int catID, int subCatID, int prodID, int quanlityNumber, string currentUser, string userName);
        List<UserList> GetUserByDeptComID(int deptID, int CompanyID);
    }

}
