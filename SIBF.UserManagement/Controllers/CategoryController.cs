using System.Web.Mvc;
using SIBF.UserManagement.Models;
using SIBF.UserManagement.Api;
using System.Web.Security;
using System.Collections.Generic;
using System;

namespace SIBF.UserManagement.Controllers
{
    public class CategoryController : Controller
    {
        private static readonly int PAGE_SIZE = 20;
        private IUserAccountService _accountService;
        //private ICategoryService _categoryService;

        public CategoryController(IUserAccountService accountService)
        {
            _accountService = accountService;
            // _categoryService = categoryService; ICategoryService categoryService
        }

        [HttpGet]
        [Authorize]
        public ActionResult Index(int? Page, string SortBy = "Name", bool Ascending = true)
        {
            ViewBag.SuccessMsg = ViewBag.Failuremessage= "";
            CategorysModels cvm = new CategorysModels();
            List<CategoryList> allCategories = _accountService.GetAllCategory();
            CategoryDisplayModel model = new CategoryDisplayModel();
            model.Categorys = allCategories;
            cvm.ListDataModel = model;
            return View(cvm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Index(CategorysModels md)
        {

            if (ModelState.IsValid)
            {
                Api.MembershipCreateStatus createStatus;
                string currentUser = HttpContext.User.Identity.Name;

                int seltedID = md.SubmitFormModel.CategoryID;

                if (seltedID == 0)
                {
                    _accountService.CreateCategory(md.SubmitFormModel.CategoryName, md.SubmitFormModel.CategoryDescription, out createStatus);
                    if (createStatus == Api.MembershipCreateStatus.Success)
                    {
                        ModelState.Clear();
                        ViewBag.SuccessMsg = "Category created successfully";
                    }
                }
                else
                {
                    bool result= _accountService.UpdateCategory(md.SubmitFormModel.CategoryName, md.SubmitFormModel.CategoryDescription, md.SubmitFormModel.CategoryID);
                    if (result == true)
                    {
                        ModelState.Clear();
                        ViewBag.SuccessMsg = "Category updated successfully";
                    }
                    else
                    {
                        ViewBag.Failuremessage = "Unable to update please try again later!";
                    }
                }
            }
            CategorysModels cvm = new CategorysModels();
            List<CategoryList> allCategories = _accountService.GetAllCategory();
            CategoryDisplayModel model = new CategoryDisplayModel();
            model.Categorys = allCategories;
            cvm.ListDataModel = model;
            return View(cvm);
        }
        
        [HttpPost]
        [Authorize]
        public JsonResult DeleteElement(int id, string tableName, string colName)
        {
            string currentUser = HttpContext.User.Identity.Name;
            bool ProductDetails = _accountService.DeleteData(id, tableName, colName, currentUser);
            return Json(ProductDetails, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Authorize]
        public JsonResult EditElement(int id)
        {
            string currentUser = HttpContext.User.Identity.Name;
            List<CategoryList> CategoryDetails = _accountService.GetCateogryDataByID(id);
            return Json(CategoryDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public ActionResult SubCategory()
        {
            ViewBag.SuccessMsg = ViewBag.Failuremessage = "";
            SubCategoryModels scvm = new SubCategoryModels();
            List<SubCategoryList> allSubCategories = _accountService.SubCategory();
            SubCategoryDisplayModel model = new SubCategoryDisplayModel();
            ViewBag.Categories = _accountService.GetAllCategory();
            model.SubCategorys = allSubCategories;
            scvm.ListDataModel = model;
            return View(scvm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SubCategory(SubCategoryModels model)
        {
            ViewBag.SuccessMsg = ViewBag.Failuremessage = "";
            if (ModelState.IsValid)
            {
                int seltedID = model.SubmitFormModel.SubCategoryID;
                //Api.MembershipCreateStatus createStatus;
                Api.ProductCategorySubCategory createStatus;
                string currentUser = HttpContext.User.Identity.Name;

                if (seltedID == 0)
                {
                    _accountService.CreateSubCategory(model.SubmitFormModel.CategoryID, model.SubmitFormModel.SubCategoryName,
                                                      model.SubmitFormModel.SubCategoryDescription, out createStatus);

                    if (createStatus == Api.ProductCategorySubCategory.Success)
                    {
                        ModelState.Clear();
                        ViewBag.SuccessMsg = "SubCategory careated successfully";
                    }
                    else if (createStatus == Api.ProductCategorySubCategory.DuplicateName)
                    {
                        ViewBag.Failuremessage = "Unable to create Subcategor, As Subcategory already exist!";
                    }
                }
                else
                {
                    bool returnResult = _accountService.UpdateCategory(model.SubmitFormModel.CategoryID, model.SubmitFormModel.SubCategoryName,
                                                      model.SubmitFormModel.SubCategoryDescription, model.SubmitFormModel.SubCategoryID);

                    if (returnResult == true)
                    {
                        ModelState.Clear();
                        ViewBag.SuccessMsg = "Category updated successfully";
                    }
                    else
                    {
                        ViewBag.Failuremessage = "Unable to update please try again later!";
                    }
                } 
            }
            SubCategoryModels scvm = new SubCategoryModels();
            List<SubCategoryList> allSubCategories = _accountService.SubCategory();
            SubCategoryDisplayModel modeldisplay = new SubCategoryDisplayModel();
            ViewBag.Categories = _accountService.GetAllCategory();
            modeldisplay.SubCategorys = allSubCategories;
            scvm.ListDataModel = modeldisplay;
            return View(scvm);
        }

        [HttpPost]
        [Authorize]
        public JsonResult EditSubCategoryById(int id)
        {
            string currentUser = HttpContext.User.Identity.Name;
            List<SubCategoryList> SubCategoryDetails = _accountService.GetSubCateogryDataByID(id);
            return Json(SubCategoryDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult DeleteSubCategory(int id)
        {
            string currentUser = HttpContext.User.Identity.Name;
            bool ProductDetails = _accountService.DeleteSubCategory(id);
            return Json(ProductDetails, JsonRequestBehavior.AllowGet);
        }
    }
}