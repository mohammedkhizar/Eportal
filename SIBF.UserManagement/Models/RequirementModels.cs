using SIBF.UserManagement.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIBF.UserManagement.Models
{
    public class RequirementModels
    {
        public int CompanyID { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        public int DepartmentID { get; set; }
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }
        public int CategoryID { get; set; }
        [Display(Name ="Category Name")]
        public string CategoryName { get; set; }
        public int SubCategoryID { get; set; }
        [Display(Name ="Sub Category Name")]
        public string SubCategoryName { get; set; }
        public int ProductID { get; set; }
        [Display(Name ="Product Name")]
        public string ProductName { get; set; }
        [Display(Name ="Requested Quantity")]
        public int RequestedProductQuantity { get; set; }
        [Display(Name ="Assigned Quantity")]
        public int AssignedProductQuantity { get; set; }
        [Display(Name ="User Name")]
        public string UserName { get; set; }
        [Display(Name ="Requested Date")]
        public DateTime RequestedDate { get; set; }
        [Display(Name ="Assigned Date")]
        public DateTime AssignedDate { get; set; }
        public int AvailableProductQuantity { get; set; }
        public int RowID { get; set; }
        public string AssignedQuanity { get; set; }
        public int RequirementID { get; set; }
        public int ReturnQuantity { get; set; }
    }


    public class RequirementModelsList
    {
        public List<RequirementModels> RequirementList { get; set; }
        public IEnumerable<RequirementModels> IRequirementList { get; set; }

        public static List<RequirementModels> convertDTo(List<RequirementList> requirements)
        {
            List<RequirementModels> response = new List<RequirementModels>(requirements.Count);
            foreach (RequirementList requirement in requirements)
            {
                response.Add(new RequirementModels
                {
                    CategoryID = requirement.CategoryID,
                    CategoryName = requirement.CategoryName,
                    AssignedDate = requirement.AssignedDate,
                    AssignedProductQuantity = requirement.AssignedProductQuantity,
                    CompanyID = requirement.CompanyID,
                    CompanyName = requirement.CompanyName,
                    DepartmentID = requirement.DepartmentID,
                    DepartmentName = requirement.DepartmentName,
                    ProductID = requirement.ProductID,
                    ProductName = requirement.ProductName,
                    RequestedDate = requirement.RequestedDate,
                    RequestedProductQuantity = requirement.RequestedProductQuantity,
                    SubCategoryID = requirement.SubCategoryID,
                    SubCategoryName = requirement.SubCategoryName,
                    UserName = requirement.UserName,
                    AvailableProductQuantity = requirement.AvailableProductQuantity,
                    RowID = requirement.RowID,
                    AssignedQuanity = requirement.AssignedQuanity,
                    RequirementID =requirement.RequirementID
                });
            }
            return response;
        }
    }

    public class AssignStockByRequirementByID
    {
        public int RowID { get; set; }
        public int StockID { get; set; }
        public int RequirementID { get; set; }
        public int ProductID { get; set; }
        public string UserName { get; set; }
        public int AvaliableQuantity { get; set; }
        public int AssignedQuantity { get; set; }
        public string Reason { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpireDate { get; set; }


    }
}