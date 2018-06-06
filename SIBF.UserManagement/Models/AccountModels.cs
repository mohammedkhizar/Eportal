using SIBF.UserManagement.Api;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace SIBF.UserManagement.Models
{
    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public int CompanyID { get; set; }
        public int DepartmentID { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "User Require Change Password")]
        public bool UserMustChangePassword { get; set; }

        public string UserType { get; set; }
        public List<MembershipUser> Users { get; set; }
        public IPagedList<MembershipUser> UsersPage { get; set; }
    }

    public class CategoryModel
    {
        [Required]
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        
        public List<CategoryList> Categorys { get; set; } 
        public IPagedList<CategoryList> CategoryPage { get; set; }
    }

    public class MembershipUserModel
    {
        public MembershipUserModel()
        {
            this.SortBy = "Username";
            this.SortAscending = true;
        }

        public List<MembershipUser> Users { get; set; }
        public IPagedList<MembershipUser> UsersPage { get; set; }
        public string SortBy { get; set; }
        public bool SortAscending { get; set; }
        public string SortExpression
        {
            get
            {
                return this.SortAscending ? this.SortBy + " asc" : this.SortBy + " desc";
            }
        }
        
    }

    public class SubCategoryModel
    {
        [Required]
       // public string CategoryId { get; set; }
        public string SelectedCategoryId { get; set; }
        //public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryDescription { get; set; }
        public List<CategoryList> Categorys { get; set; }
        
    }

    public class CategoryListModel
    {
        public List<CategoryList> CategoryList { get; set; }
        public IPagedList<CategoryList> CategoryPage { get; set; }
      
    }

    public class LoginRegisterModel
    {
        public LogOnModel LogIn { get; set; }
        public RegisterModel RegisterUser { get; set; }
    }


    public class FormSubmitDepartment
    {
        [Required]
        public string SelectedCompanyID { get; set; }

        public string DepartmentName { get; set; }
        public string DepartmentDirNme { get; set; }

        public string DepartmentFloorno { get; set; }

        public string DepartmentRoomno { get; set; }
        public List<CompanyList> Company { get; set; }

    }


    public class CompanyListModel
    {
        public List<CompanyList> CompanyList { get; set; }
        public IPagedList<CompanyList> CompanyPage { get; set; }

    }

    public class CompanyModel
    {
        public FormSubmitCompany FormSubmit { get; set; }
        public DisplayCompanyData DisplayCompany { get; set; }
    }

    public class FormSubmitCompany
    {
        [Required]
        public string CompanyShortName { get; set; }
        public string CompanyFullName { get; set; }
        public string CompanyLocation { get; set; }

        public List<CompanyList> Company { get; set; }
        public IPagedList<CompanyList> CompanyPage { get; set; }
    }

    public class DisplayCompanyData
    {
        public int CompanyID { get; set; }
        public string FullCompanyName { get; set; }
        public string ShortName { get; set; }
        public string Location { get; set; }
        public List<CompanyList> Company { get; set; }
        public IPagedList<CompanyList> CompanyPage { get; set; }
    }

    public class DepartmentModel
    {
        public FormSubmitDepartment FormSubmit { get; set; }
        public DepartmentListModel DisplayDepartment { get; set; }
    }

    public class DepartmentListModel
    {
        public List<DepartmentList> DepartmentList { get; set; }
        //public IPagedList<DepartmentList> DepartmentPage { get; set; }
    }


    public class RequirementGeneralFormModel
    {
        [Required]
        public string SelectedCompanyID { get; set; }
        public string SelectedDepartmentID { get; set; }
       
        public string ProductDescription { get; set; }
        [Display(Name = "Product Quantity")]
        public string ProductQuantity { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string CreationDate { get; set; }

        [Display(Name = "Product Requirement Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string RequirementDate { get; set; }
        public string OfficeNo { get; set; }
        public string RequestStatus { get; set; }
        public string SubmittedBy { get; set; }
        public List<CompanyList> Company { get; set; }
        public List<DepartmentList> Department { get; set; }

        [Display(Name ="Category")]
        public int CategoryID { get; set; }
        [Display(Name = "Sub Category")]
        public int SubCategoryID { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string ProductName { get; set; }
        [Display(Name = "Product")]
        public int ProductID { get; set; }

        [Display(Name = "Company Name")]
        public int CompanyId { get; set; }
        [Display(Name = "Department Name")]
        public int DepratmentId { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }
        [Display(Name = "User Designation")]
        public string UserRoll { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }

    public class RegisterEmployeeModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        [Display(Name ="Designation")]
        public string UserRoll { get; set; }
        public int CompanyId { get; set; }
        public int DepratmentId { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyName { get; set; }
        [Display(Name = "Comment")]
        public string Comment { get; set; }
        public List<CompanyList> CompanyList { get; set; }
        public List<DepartmentList> DepartmentList { get; set; }
        public IPagedList<MembershipUser> UsersPage { get; set; }
        public List<MembershipRole> UserRoles { get; set; }
    }

    public class RegisterEmployeeDetails
    {
        public List<MembershipUser> UserPage { get; set; }
    }

    public class Employee
    {
        public RegisterEmployeeModel FormSubmit { get; set; }
        public RegisterEmployeeDetails DisplayData { get; set; }
    }
}