using System;
using SIBF.UserManagement.Api;
using SIBF.UserManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;
using System.Data;

namespace SIBF.UserManagement.Controllers
{
    public class ProductEntryController : Controller
    {
        private IUserAccountService _accountService;
        //private IProductAssigned _ProductAssignedService;

        public ProductEntryController(IUserAccountService accountService)
        {
            this._accountService = accountService;
            //this._ProductAssignedService = productAssingedService;
        }

        // GET: ProductEntry
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.SuccessMsg = ViewBag.Failuremessage = "";
            ViewBag.UsersInfo = _accountService.GetAllUsers();
            ProductEntryModel wvm = new ProductEntryModel();
            List<StoreData> WherehouseInfo = _accountService.StoreData();
            WarehouseDisplay model = new WarehouseDisplay();
            model.StoreData = WherehouseInfo;
            wvm.Display = model;
            return View(wvm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Index(ProductEntryModel md)
        {
            if (ModelState.IsValid)
            {
                string currentUser = HttpContext.User.Identity.Name;
                int seltedID = md.FormSubmit.StoreID;

                if (seltedID == 0)
                {
                    bool retuenResult = _accountService.CreateWareHouse(md.FormSubmit.StoreName, md.FormSubmit.StoreManager,
                                                                    md.FormSubmit.StoreRoomNumber, md.FormSubmit.StoreType, currentUser);
                    if (retuenResult == true)
                    {
                        ModelState.Clear();
                        ViewBag.SuccessMsg = "Wharehouse created successfully";
                    }
                }
                else
                {
                    bool result = _accountService.UpdateWareHouse(md.FormSubmit.StoreName, md.FormSubmit.StoreManager,
                                                                 md.FormSubmit.StoreRoomNumber, md.FormSubmit.StoreType, md.FormSubmit.StoreID);
                    if (result == true)
                    {
                        ModelState.Clear();
                        ViewBag.SuccessMsg = "Wharehouse updated successfully";
                    }
                    else
                    {
                        ViewBag.Failuremessage = "Unable to update please try again later!";
                    }
                }
            }

            ViewBag.UsersInfo = _accountService.GetAllUsers();
            ProductEntryModel wvm = new ProductEntryModel();
            List<StoreData> WherehouseInfo = _accountService.StoreData();
            WarehouseDisplay model = new WarehouseDisplay();
            model.StoreData = WherehouseInfo;
            wvm.Display = model;
            return View(wvm);
        }

        [HttpPost]
        [Authorize]
        public JsonResult DeleteWhareHouse(int id)
        {
            string currentUser = HttpContext.User.Identity.Name;
            bool delwharehouse = _accountService.DeleteWhareHouse(id);
            return Json(delwharehouse, JsonRequestBehavior.AllowGet);
        }

        /*
        [HttpPost]
        [Authorize]
        public JsonResult EditWhereHouse(int id)
        {
            string currentUser = HttpContext.User.Identity.Name;
            List<WharehouseList> WhareHouseDetails = _accountService.GetWhareHouseDataByID(id);
            return Json(WhareHouseDetails, JsonRequestBehavior.AllowGet);
        } */
        [Authorize]
        public ActionResult SupplierEntry()
        {
            ViewBag.SuccessMsg = ViewBag.Failuremessage = "";
            ViewBag.StatesList = _accountService.UAEStatesList();
            SupplierModel svm = new SupplierModel();
            List<SupplierData> SupplierInfo = _accountService.SupplierData();
            SupplierDisplayModel model = new SupplierDisplayModel();
            model.SupplierData = SupplierInfo;
            svm.DisplayModel = model;
            return View(svm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SupplierEntry(SupplierModel md)
        {
            if (ModelState.IsValid)
            {
                string currentUser = HttpContext.User.Identity.Name;
                int supperID =md.SubmitFormModel.SupplierID;
                if (supperID == 0)
                {
                    bool returnresult = _accountService.CreateSupplier(md.SubmitFormModel.FullName, md.SubmitFormModel.Currency, md.SubmitFormModel.Address, 
                                         md.SubmitFormModel.State, md.SubmitFormModel.PoCode, md.SubmitFormModel.Website, md.SubmitFormModel.Email, 
                                         md.SubmitFormModel.ContactNumber, md.SubmitFormModel.ContactPerson, md.SubmitFormModel.Description, currentUser);

                    if (returnresult == true)
                    {
                        ModelState.Clear();
                        ViewBag.SuccessMsg = "Supplier created successfully";
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "Error raised while creating supplier successfully";
                    }
                }
                else
                {
                    bool returnresult = _accountService.UpdateSupplier(md.SubmitFormModel.FullName, md.SubmitFormModel.Currency, md.SubmitFormModel.Address, 
                                        md.SubmitFormModel.State, md.SubmitFormModel.PoCode, md.SubmitFormModel.Website, md.SubmitFormModel.Email, 
                                        md.SubmitFormModel.ContactNumber, md.SubmitFormModel.ContactPerson, md.SubmitFormModel.Description, 
                                        md.SubmitFormModel.SupplierID);
                    if (returnresult == true)
                    {
                        ModelState.Clear();
                        ViewBag.SuccessMsg = "Supplier information updated successfully";
                    }
                    else
                    {
                        ViewBag.Failuremessage = "Unable to update please try again later!";
                    }
                }
            }

            ViewBag.StatesList = _accountService.UAEStatesList();
            SupplierModel svm = new SupplierModel();
            List<SupplierData> SupplierInfo = _accountService.SupplierData();
            SupplierDisplayModel model = new SupplierDisplayModel();
            model.SupplierData = SupplierInfo;
            svm.DisplayModel = model;
            return View(svm);
        }

        public JsonResult DeleteSupplier(int id)
        {
           // string currentUser = HttpContext.User.Identity.Name;
            bool SupplierDelResult = _accountService.DeleteSupplier(id);
            return Json(SupplierDelResult, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        [Authorize]
        public ActionResult ProductEntry()
        {
            ViewBag.CategoryList = _accountService.GetAllCategory(); //_accountService.SubCategory();
            ViewBag.SubCategoryList = _accountService.SubCategory();
            ViewBag.SupplierList = _accountService.SupplierList();
            CreateProductModel cpm = new CreateProductModel();
            List<ProductDataList> AllProductData = _accountService.ProductDataList();
            ProductDisplay model = new ProductDisplay();
            model.ProductDataList = AllProductData;
            cpm.DisplayData = model;
            model.ProductType = new List<SelectListItem>
            {
                new SelectListItem {Text = "Return", Value ="Return"},
                    new SelectListItem {Text = "Non Return", Value ="Non Return"},
            };
            return View(cpm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ProductEntry(CreateProductModel md)
        {
            if (ModelState.IsValid)
            {
                string currentUser = HttpContext.User.Identity.Name;
                int ProductId = md.FormSubmit.ProductId;
                if (ProductId == 0)
                {
                    bool returnresult = _accountService.CreateProduct(md.FormSubmit.SubCategoryId, md.FormSubmit.SupplierID,
                                                                   md.FormSubmit.ProductName, md.FormSubmit.ProductDesc,
                                                                   currentUser, md.FormSubmit.ProductType);
                    if (returnresult == true)
                    {
                        ModelState.Clear();
                        ViewBag.SuccessMsg = "Product created successfully";
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "Error raised while creating product successfully";
                    }
                }
                else
                {
                    bool returnresult = _accountService.UpdateProduct(md.FormSubmit.ProductId, md.FormSubmit.SubCategoryId, md.FormSubmit.SupplierID,
                                                                  md.FormSubmit.ProductName, md.FormSubmit.ProductDesc,
                                                                  currentUser);
                    if (returnresult == true)
                    {
                        ModelState.Clear();
                        ViewBag.SuccessMsg = "Product information updated successfully";
                    }
                    else
                    {
                        ViewBag.Failuremessage = "Unable to update please try again later!";
                    }
                }
            }

            ViewBag.CategoryList = _accountService.GetAllCategory(); //_accountService.SubCategory();
            ViewBag.SubCategoryList = _accountService.SubCategory();
            ViewBag.SupplierList = _accountService.SupplierList();
            CreateProductModel cpm = new CreateProductModel();
            List<ProductDataList> AllProductData = _accountService.ProductDataList();
            ProductDisplay model = new ProductDisplay();
            model.ProductDataList = AllProductData;
            cpm.DisplayData = model;
            model.ProductType = new List<SelectListItem>
            {
                new SelectListItem {Text = "Return", Value ="Return"},
                    new SelectListItem {Text = "Non Return", Value ="Non Return"},
            };
            return View(cpm);
        }

        public JsonResult DeleteProduct(int id)
        {
            bool SupplierDelResult = _accountService.DeleteProductById(id);
            return Json(SupplierDelResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubCategoryByID(int id)
        {
            List<SubCategoryList> SubcategoryList = _accountService.GetSubCateogryDataByID(id);//_accountService.SubCategoryByID(id);
            var subCategoryList = SubcategoryList;
            return Json(subCategoryList, JsonRequestBehavior.AllowGet);
        }

       

        public JsonResult GetProductBySubCategoryID(int id)
        {
            List<ProductList> ProductList = _accountService.GetProductByID(id);
            var productList = ProductList;
            return Json(productList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public JsonResult saveRequirementList(int catID, int subCatID, int prodID, int quanlityNumber, string userName)
        {
            string currentUser = HttpContext.User.Identity.Name;
            bool equirementSaved = _accountService.RequirementSaved(catID, subCatID, prodID, quanlityNumber, currentUser, userName);
            return Json(equirementSaved, JsonRequestBehavior.AllowGet);
        }
        public ActionResult StockEntry()
        {
            ViewBag.CategoryList = _accountService.GetAllCategory(); //_accountService.SubCategory();
            ViewBag.SubCategoryList = _accountService.SubCategory();
            ViewBag.ProductList = _accountService.ProductList();
            StockModel sem = new StockModel();
            List<StockDataList> AllStockData = _accountService.StockInfo();
            ViewStockModel model = new ViewStockModel();
            model.AllStockData = AllStockData;
            sem.DisplayData = model;
            return View(sem);
        }

        [HttpPost]
        [Authorize]
        public ActionResult StockEntry(StockModel md)
        {

            if (ModelState.IsValid)
            {
                string currentUser = HttpContext.User.Identity.Name;
                int stockID = md.FormSubmit.StockID;
                if (stockID == 0)
                {
                    bool returnresult = _accountService.EnterStock(md.FormSubmit.ProductID, md.FormSubmit.Quantity, md.FormSubmit.ManufactureDate, 
                                                                    md.FormSubmit.ExpiryDate, md.FormSubmit.BarCode, currentUser);
                    if (returnresult == true)
                    {
                        ModelState.Clear();
                        ViewBag.SuccessMsg = "Product created successfully";
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "Error raised while creating product successfully";
                    }
                }
                else
                {
                    bool returnresult = _accountService.UpdateStock(md.FormSubmit.StockID, md.FormSubmit.ProductID, md.FormSubmit.Quantity, 
                                                                    md.FormSubmit.ManufactureDate,md.FormSubmit.ExpiryDate, 
                                                                    md.FormSubmit.BarCode);
                    if (returnresult == true)
                    {
                        ModelState.Clear();
                        ViewBag.SuccessMsg = "Product information updated successfully";
                    }
                    else
                    {
                        ViewBag.Failuremessage = "Unable to update please try again later!";
                    }
                }
            }

            ViewBag.CategoryList = _accountService.GetAllCategory(); 
            ViewBag.SubCategoryList = _accountService.SubCategory();
            ViewBag.ProductList = _accountService.ProductList();
            StockModel sem = new StockModel();
            List<StockDataList> AllStockData = _accountService.StockInfo();
            ViewStockModel model = new ViewStockModel();
            model.AllStockData = AllStockData;
            sem.DisplayData = model;
            return View(sem);
        }
        [HttpPost]
        [Authorize]
        public JsonResult DeleteStock(int id)
        {
            bool SupplierDelResult = _accountService.DeleteStockById(id);
            return Json(SupplierDelResult, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult AssignProduct()
        {
            ViewBag.CategoryList = _accountService.GetAllCategory();
            ViewBag.SubCategoryList = _accountService.SubCategory();
            ViewBag.ProductList = _accountService.ProductList();
            ViewBag.UsersInfo = _accountService.GetAllUsers();
            AssignProductModel sem = new AssignProductModel();
            List<ProductAssignedList> AllProductAssignedData = _accountService.AssignedDataList();
            ProductAssignedDisplay model = new ProductAssignedDisplay();
            model.AllProductAssignedData = AllProductAssignedData;
            sem.DisplayData = model;
            return View(sem);
        }

        [HttpPost]
        [Authorize]
        public ActionResult AssignProduct(AssignProductModel model)
        {
            if (ModelState.IsValid)
            {
                string currentUser = HttpContext.User.Identity.Name;
                int selRow = model.FormSubmit.RowID;
                int j = 0;
                int RequirementID = 0;
                string Reason = "";
                if (selRow == 0)
                {       string[] prodToReduce = model.FormSubmit.AssignedQuanity.Split(',');
                        for(int i=0; i<prodToReduce.Length; i++)
                        {
                            string[] stockqunaity=prodToReduce[i].Split('-');
                            int stockID = Int32.Parse(stockqunaity[0]); int quantityReduce = Int32.Parse(stockqunaity[1]);
                            bool reduceQuantity = _accountService.ReduceQuantity(stockID, quantityReduce);
                        if (reduceQuantity == true)
                        {
                            bool returnresult = _accountService.AssignProduct(model.FormSubmit.UserName, quantityReduce, 
                                                                              model.FormSubmit.ProductID, currentUser, stockID, RequirementID, Reason);
                            if (returnresult == true)
                            {
                                j = j + 1;
                            }
                        }
                        else
                        {
                            ViewBag.SuccessMsg = "Error raised while assigning product please try again!";
                        }
                    }

                    ModelState.Clear();
                    ViewBag.SuccessMsg = "Product assigned to user successfully";
                }
                else
                {
                    bool returnresult = _accountService.AssignProductUpdate(model.FormSubmit.UserName, model.FormSubmit.Quantity, model.FormSubmit.ProductID, selRow);
                    if (returnresult == true)
                    {
                        ModelState.Clear();
                        ViewBag.SuccessMsg = "Assigned product updated successfully";
                    }
                    else
                    {
                        ViewBag.Failuremessage = "Unable to update please try again later!";
                    }
                }
            }
            ViewBag.CategoryList = _accountService.GetAllCategory();
            ViewBag.SubCategoryList = _accountService.SubCategory();
            ViewBag.ProductList = _accountService.ProductList();
            ViewBag.UsersInfo = _accountService.GetAllUsers();
            AssignProductModel sem = new AssignProductModel();
            List<ProductAssignedList> AllProductAssignedData = _accountService.AssignedDataList();
            ProductAssignedDisplay md = new ProductAssignedDisplay();
            md.AllProductAssignedData = AllProductAssignedData;
            sem.DisplayData = md;
            return View(sem);
        }

        public JsonResult DeleteAssignedProduct(int id, int stockID, int quantity)
        {
            bool updateProductReturnQuantity = _accountService.updateProductRtnQty(stockID, quantity);
            if (updateProductReturnQuantity == true)
            {
                bool DelResult = _accountService.DeleteAssignedProductId(id);
                return Json(DelResult, JsonRequestBehavior.AllowGet);
            }
            return Json(updateProductReturnQuantity, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult GetAvaliableQuantity(int id)
        {
            List<ProductDetails> ProductDetails = _accountService.ProductDetails(id);//_accountService.SubCategoryByID(id);
            var ProductDetailsList = ProductDetails;
            return Json(ProductDetailsList, JsonRequestBehavior.AllowGet);

           // int ProductDetails =_accountService.ProductData(id);
           // return Json(ProductDetails, JsonRequestBehavior.AllowGet);
            //return ProductDetails;
        }

        [HttpGet]
        [Authorize]
        public ActionResult AvaliableStock()
        {
            ViewStockModel model = new ViewStockModel();
            // List<StockDataList> AssignedStock = _accountService.AssignedStockQty();
            List<StockDataList> StockHistory = _accountService.StockHistory();
            //List<StockDataList> AssignedStock = _accountService.AssignedStockQty();
            model.AllStockData = StockHistory;
            ///model.AssignedStock = AssignedStock;
            return View(model);
        }

    }

}