using System;

namespace SIBF.UserManagement.Api
{
    public class ProductCatSubCatInstance
    {
        public string ProductName { get; set; }
        public string CategoryName {get;set;}
        public string SubCategoryName { get; set; }
    }

    public class CreateSubCategory
    {
        public string SubCategoryName { get; set; }
    }
}
