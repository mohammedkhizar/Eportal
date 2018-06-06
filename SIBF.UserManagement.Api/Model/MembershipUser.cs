using System;

namespace SIBF.UserManagement.Api
{
    public class MembershipUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedout { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool UserMustChangePassword { get; set; }

        public int DeptID { get; set; }
        public int CompanyID { get; set; }
        public string CategoryName { get; set; }

        //public int CategoryId { get; set; }
        public string SubCategoryName { get; set; }

        public static MembershipUser CreateNewMembershipInstance()
        {
            return new MembershipUser
            {
                CreationDate = DateTime.Now,
                IsApproved = true,
                IsLockedout = false,
                LastActivityDate = null,
                LastLoginDate = null,
                LastPasswordChangedDate = null
            };
        }
    }

    public class MembershipRole
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DescriptionArabic { get; set; }
    }
    public class CreateUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
    }

    public class AssignRole
    {
        public string Username { get; set; }
        public string Rolename { get; set; }
        public string AssignedBy { get; set; }
    }

    public class CreateCategory
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
    }

    public class CategoryList
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
    }

    //public class CreateSubCategory
    //{
    //    public string CategoryId { get; set; }
    //    public string CategoryName { get; set; }
    //    public string SubCategoryId { get; set; }
    //    public string SubCategoryName { get; set; }
    //    public string SubCategoryDescription { get; set; }
    //}

    public class SubCategoryList
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryDesc { get; set; }
    }

    public class SupplierList
    {
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
    }

    public class ProductList
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
    }

    public class CitiesList
    {
        public int CityID { get; set; }
        public string CityName { get; set; }
        public int StateID { get; set; }
    }

    public class StatesList
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
        public int CountryID { get; set; }
    }

    public class CountriesList
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
    }

    public class UAEStatesList
    {
        public int StateID { get; set; }
        public int CountryID { get; set; }
        public string StateName { get; set; }
    }

    public class ProductDetails
    {
        public int CategoryID { get; set; }
        public int SubCategoryID { get; set; }
        public int SupplierID { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string CompanyName { get; set; }
        public int Quantity { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string BarCode { get; set; }
        public string AvaliableQuantity { get; set; }
        public int StockID { get; set; }
    }

    public class ProductDetailsByID
    {
        public int CategoryID { get; set; }
        public int SubCategoryID { get; set; }
        public int SupplierID { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string CompanyName { get; set; }
        public int Quantity { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string BarCode { get; set; }
        public string AvaliableQuantity { get; set; }
    }

    public class DeleteData
    {
        public int ID { get; set; }
        public string TableName { get; set; }
        public string ColName { get; set; }
    }

    public class StoreData
    {
    public int StoreID { get; set; }
    public string StoreName { get; set; }
    public string StoreType { get; set; }
    public string StoreRoomNumber { get; set; }
    public string StoreManager { get; set; }
    public string UsersId { get; set; }
    }

    public class SupplierData
    {
        public int SupplierID { get; set; }
        public string FullName { get; set; }
        public string Currency { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string PoCode { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string ContactPerson { get; set; }
        public string Description { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public int CityID { get; set; }
    }

    public class ProductDataList
    {
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int SubCategoryID { get; set; }
        public string SubCategoryName { get; set; }
    }

    public class StockDataList
    {
        public int StockID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public int IntQuantity { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string ManufactureDate { get; set; }
        public string ExpiryDate { get; set; }
        public string BarCode { get; set; }
        public string Img { get; set; }
        public String CreatedBy {get;set;}
        public string CreatedOn { get; set; }   
    }

    public class ProductAssignedList
    {
        public string UserName { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string AssignedDate { get; set; }
        public string ProductName { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int RowId { get; set; }
        public int StockID { get; set; }
    }

    public class CreateCompany
    {
        public string CompanyShortName { get; set; }
        public string CompanyFullName { get; set; }
        public string CompanyLocation { get; set; }
    }

    public class CompanyList
    {
        public string CompanyID { get; set; }
        public string CompanyShortName { get; set; }
        public string CompanyFullName { get; set; }
        public string CompanyLocation { get; set; }
    }

    public class CreateDepartment
    {
        public string CompanyID { get; set; }
        public string CompanyShortName { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDirNme { get; set; }
        public string DepartmentFloorno { get; set; }
        public string DepartmentRoomno { get; set; }
    }

    public class DepartmentList
    {

        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDirNme { get; set; }
        public string DepartmentFloorno { get; set; }
        public int DepartmentRoomno { get; set; }
        public int CompanyID { get; set; }
        public string CompanyFullName { get; set; }
    }

    public class RequirementGeneralForm
    {
        public string CompanyID { get; set; }
        public string CompanyShortName { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductQuantity { get; set; }
        public string CreationDate { get; set; }
        public string RequirementDate { get; set; }
        public string OfficeNo { get; set; }
        public string RequestStatus { get; set; }
        public string SubmittedBy { get; set; }
    }

    public class UserList
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public int DeptID { get; set; }
        public int CompanyID { get; set; }
    }
}    
