using System.Web.Mvc;
using SIBF.UserManagement.Models;
using SIBF.UserManagement.Api;
using System.Web.Security;
using System.Collections.Generic;
using System;

namespace SIBF.UserManagement.Controllers
{
    public class RequirementController : Controller
    {
        private IProductService _productService;

        public RequirementController(IProductService productService)
        {
            _productService = productService;
        }
        // GET: Requirement
        [Authorize]
        public ActionResult UserRequirments()
        {
            RequirementModelsList requiremntsModels = new RequirementModelsList();
            requiremntsModels.RequirementList = RequirementModelsList.convertDTo(_productService.GetAllRequestedProductList());
            //ViewBag.requirementlist = _productService.GetAllRequestedProductList();
            return View(requiremntsModels);
        }

        [HttpPost]
        [Authorize]
        public JsonResult UpdatedProductAssigned(string UserName, string ProductName, int ProductID, int SelRowID, int Quantity, string Reason)
        {
            string currentUser = HttpContext.User.Identity.Name;
            bool ProductDetails = _productService.UpdatedProductAssigned(UserName, ProductName, ProductID, SelRowID, Quantity, Reason, currentUser);
            return Json(ProductDetails, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult AssignStock(int ProductID)
        {
            RequirementModelsList requiremntsModels = new RequirementModelsList();
            requiremntsModels.RequirementList = RequirementModelsList.convertDTo(_productService.GetStockByProductID(ProductID));
            return View(requiremntsModels);
        }

        [HttpGet]
        [Authorize]
        public ActionResult AssignStockRequirment(int RequirementID)
        {
            RequirementModelsList requiremntsModels = new RequirementModelsList();
            requiremntsModels.RequirementList = RequirementModelsList.convertDTo(_productService.GetRequestedProductListByID(RequirementID));
            return View(requiremntsModels);
        }


        [HttpPost]
        [Authorize]
        public ActionResult AssignStockRequirment(RequirementModelsList model)
        {
            if (ModelState.IsValid)
            {
                string currentUser = HttpContext.User.Identity.Name;
                int j = 0;
                int RequirementID = 0;
                string Reason = "";
                int RequirementIDFrom = model.RequirementList[0].RowID;
                if (RequirementIDFrom != 0) { RequirementID = RequirementIDFrom; }
                string[] prodToReduce = model.RequirementList[0].AssignedQuanity.Split(',');
                for (int i = 0; i < prodToReduce.Length; i++)
                {
                    string[] stockqunaity = prodToReduce[i].Split('-');
                    int stockID = Int32.Parse(stockqunaity[0]); int quantityReduce = Int32.Parse(stockqunaity[1]);
                    bool reduceQuantity = _productService.ReduceQuantity(stockID, quantityReduce);
                    if (reduceQuantity == true)
                    {
                        bool returnresult = _productService.AssignProduct(model.RequirementList[0].UserName, quantityReduce,
                                                                          model.RequirementList[0].ProductID, currentUser, stockID, RequirementID, Reason);
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
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ReturnStockRequirment(int ReturnID)
        {
            RequirementModelsList requiremntsModels = new RequirementModelsList();
            requiremntsModels.RequirementList = RequirementModelsList.convertDTo(_productService.GetAssignedStockByRowID(ReturnID));
            return View(requiremntsModels);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ReturnStockRequirment(RequirementModelsList model, int ReturnQuantity)
        {
            if (ModelState.IsValid)
            {
                string currentUser = HttpContext.User.Identity.Name;
                DateTime currentdatetime = DateTime.Now;
                //int ReturnQuantity = ReturnQuantity;
                int AssignedProductQuantity = model.RequirementList[0].AssignedProductQuantity;
                int RowID = model.RequirementList[0].RowID;
                int ProductID = model.RequirementList[0].ProductID;
                int RequirementID = model.RequirementList[0].RequirementID;
                int StockID = model.RequirementList[0].CompanyID;
                string Status = "returned";
                bool rtnQuantityForStockHistory = _productService.updateProductRtnQty(StockID, ReturnQuantity);
                
                if (rtnQuantityForStockHistory == true)
                {
                    bool rtnQuantityForUserProduct = _productService.updateReduceRtnQty(RowID, ReturnQuantity, currentUser, currentdatetime, Status);
                    if (rtnQuantityForUserProduct == false)
                    {
                        bool reduceagainfromstock = _productService.ReduceQuantity(StockID, ReturnQuantity);
                        ViewBag.SuccessMsg = "Error while returning product please try again!";

                    }
                    else
                    {
                       ViewBag.SuccessMsg = "Product return is successfully done";
                    }

                }
            }
            ModelState.Clear();
            return View();
        }
    }
}