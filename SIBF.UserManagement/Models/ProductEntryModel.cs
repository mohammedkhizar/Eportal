using SIBF.UserManagement.Api;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using X.PagedList;
using System;
using System.Web.Mvc;

namespace SIBF.UserManagement.Models
{
    public class ProductEntryModel
    {
        public WarehouseFormSubmit FormSubmit { get; set; }
        public WarehouseDisplay Display { get; set; }
    }

    public class WarehouseFormSubmit
    {
        public int StoreID { get; set; }
        [Required]
        public string StoreName { get; set; }
        public string StoreType { get; set; }
        public string StoreRoomNumber { get; set; }
        public string StoreManager { get; set; }
        public List<MembershipUser> Users { get; set; }

    }

    public class WarehouseDisplay
    {
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public string StoreType { get; set; }
        public string StoreRoomNumber { get; set; }
        public string StoreManager { get; set; }
        public string UsersId { get; set; }

        public List<StoreData> StoreData { get; set; }
    }

    public class SupplierModel
    {
        public SupplierEntryModel SubmitFormModel { get; set; }
        public SupplierDisplayModel DisplayModel { get; set; }
    }

    public class SupplierDisplayModel
    {
        public List<SupplierData> SupplierData { get; set; }
    }

    public class SupplierEntryModel
    {
        public int SupplierID { get; set; }
        [Required]
        public string FullName { get; set; }
        public string Currency { get; set; }
        public string Address { get; set; }
        //public string Country { get; set; }
        // public string City { get; set; }
        public string State { get; set; }
        [Display(Name = "Po.Box")]
        public string PoCode { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string ContactPerson { get; set; }
        public string Description { get; set; }

        public int CountryID { get; set; }
        public int StateID { get; set; }
        public int CityID { get; set; }
        //public string CurrentUser
    }

    public class CreateProductModel
    {
        public CreateProduct FormSubmit { get; set; }
        public ProductDisplay DisplayData { get; set; }
    }

    public class CreateProduct
    {
        [Required]
        [Display(Name ="Supplier Name")]
        public int SupplierID { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Display(Name = "Product Description")]
        public string ProductDesc { get; set; }
        [Display(Name = "SubCategory Name")]
        public int SubCategoryId { get; set; }
        public int ProductId { get; set; }
        [Display(Name = "Category Name")]
        public int CategoryId { get; set; }
        public string ProductType { get; set; }
    }

    //public enum ProductType
    //{
    //    Return,
    //    [Display(Name ="Non-Return")]
    //    NonReturn
    //}

    public class ProductDisplay
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
        public List<ProductDataList> ProductDataList { get; set; }
        public IEnumerable<SelectListItem> ProductType { get; set; }
    }

    public class StockModel
    {
        public AddStockModel FormSubmit { get; set; }
        public ViewStockModel DisplayData { get; set; }
    }
    public class AddStockModel
    {
        public int StockID { get; set; }
        [Required]
        public string ProductID { get; set; }
        public string Quantity { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ManufactureDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpiryDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }

        public string BarCode { get; set; }
        //public byte[] Img { get; set; }

        public int CategoryID { get; set; }
        public int SubCategoryID { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }


    }

    public class ViewStockModel
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
        public String CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<StockDataList> AllStockData { get; set; }
        public List<StockDataList> AssignedStock { get; set; }
    }


    public class AssignProductModel
    {
        public AssignProductModelFormSubmit FormSubmit { get; set; }
        public ProductAssignedDisplay DisplayData { get; set; }
    }
    public class AssignProductModelFormSubmit
    {
        [Required]
        public int ProductID { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public int AvaliableQuantity { get; set; }
        [Required]
        public int RowID { get; set; }

        public int CategoryID { get; set; }
        public int SubCategoryID { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }

        public string AvaliableQuantityDeatils { get; set; }
        public int StockID { get; set; }
        public string AssignedQuanity { get; set; }
    }

    public class ProductAssignedDisplay
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
        public int RowID { get; set; }
        public List<ProductAssignedList> AllProductAssignedData { get; set; }
    }

    public class ProductAvaliableQuantity
    {
        public int CategoryID { get; set; }
        public int SubCategoryID { get; set; }
        public string SupplierID { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public int Quantity { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ManufactureDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpiryDate { get; set; }
        public string BarCode { get; set; }
        public string AvaliableQuantity { get; set; }
        public List<ProductDetails> productDetails { get; set; }
    }
}