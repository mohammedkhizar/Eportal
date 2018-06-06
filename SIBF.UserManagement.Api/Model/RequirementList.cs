using System;

namespace SIBF.UserManagement.Api
{
    public class RequirementList
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int SubCategoryID { get; set; }
        public string SubCategoryName { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int RequestedProductQuantity { get; set; }
        public int AssignedProductQuantity { get; set; }
        public int AvailableProductQuantity { get; set; }
        public string UserName { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime AssignedDate { get; set; }
        public int RowID { get; set;}
        public string AssignedQuanity { get; set; }
        public int RequirementID { get; set; }
        public string Status { get; set; }
        public int ReturnQuantity { get; set; }
    }

    public class ProductListData
    {
        public int ProductID { get; set; }
        public int StockID { get; set; }
        public int AvailableProductQuantity { get; set; }
        public int AssignedProductQuantity { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
