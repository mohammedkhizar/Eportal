using SIBF.UserManagement.Api;

using SIBF.UserManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SIBF.UserManagement.Controllers
{
    public class AccountController : Controller
    {

        private IUserAccountService _accountService;

        public AccountController(IUserAccountService accountService)
        {
            this._accountService = accountService;
        }

        // GET: Account
        [AllowAnonymous]
        public ActionResult Logon()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult Logon(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_accountService.ValidateUser(model.UserName, model.Password))
                    {
                        //Set cookie for the current login
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        //FormsAuthentication.SetAuthCookie(model.CompanyID, model.DepartmentID);
                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    }
                }
                catch (InvalidPasswordException)
                {
                    ModelState.AddModelError("", "The Password provided is incorrect.");
                }
                catch(UserNotFoundException)
                {
                    ModelState.AddModelError("", "The username is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff
        public ActionResult LogOff()
        {
            //Destroy the cookie
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }


        //Get Reister form for outside
        // GET: /Account/CreateAccount
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CreateAccount(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                Api.MembershipCreateStatus createStatus;
                string currentUser = HttpContext.User.Identity.Name;
                int companyID = 0;
                int departmentID = 0;

                _accountService.CreateUser(model.UserName, model.Password, model.Email, currentUser, 
                    model.Comment, companyID, departmentID,  out createStatus);

                if (createStatus == Api.MembershipCreateStatus.Success)
                {
                    // FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    // return RedirectToAction("Index", "Home");
                    //new EmailService().SendMail(model.Email, model.UserName, "Authentication", "Welcome to app1");
                    ViewBag.SuccessMsg = "User created successfully!";
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [Authorize(Users = "Super Admin")]
        public ActionResult Register()
        {
          /*  List<SelectListItem> ObjItem = new List<SelectListItem>()
            {
              new SelectListItem {Text="Select",Value="0",Selected=true },
              new SelectListItem {Text="ASP.NET",Value="1" },
              new SelectListItem {Text="C#",Value="2"},
              new SelectListItem {Text="MVC",Value="3"},
              new SelectListItem {Text="SQL",Value="4" },
            };
            ViewBag.ListItem = ObjItem; */
            return View();
        }

        //
        // POST: /Account/Register
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Users = "Super Admin")]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                Api.MembershipCreateStatus createStatus;
                string currentUser = HttpContext.User.Identity.Name;
                /*if(currentUser == null)
                {
                    return RedirectToAction("Logon");
                }*/
                int companyID = 0; int departmentID = 0;
                _accountService.CreateUser(model.UserName, model.Password, model.Email, currentUser, 
                    model.Comment, companyID, departmentID, out createStatus);
                if (createStatus == Api.MembershipCreateStatus.Success)
                {                    
                    new EmailService("smtp.gmail.com",587, "khussain7@gmail.com", "Khizar1984")
                        .SendMail(model.Email, model.UserName, "Authentication", "Welcome to app1");
                    ModelState.Clear();
                    ViewBag.SuccessMsg = "User created successfully";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }                       
        }

        #region Status Codes
        private static string ErrorCodeToString(Api.MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case Api.MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case Api.MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case Api.MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case Api.MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case Api.MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        private static string ErrorCodeToStringProduct(Api.ProductCategorySubCategory createStatus)
        {
            switch (createStatus)
            {
                case Api.ProductCategorySubCategory.DuplicateName:
                    return "Data already exist please choose another name.";

                case Api.ProductCategorySubCategory.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case Api.ProductCategorySubCategory.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case Api.ProductCategorySubCategory.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #region ALL about category
        [Authorize]
        public ActionResult Category()
        {
           /* List<CategoryList> allCategory = GetAllCategory();
            CategoryListModel model = new CategoryListModel();
            model.Categorys = allCategory; */
            return View();
        }

        private List<CategoryList> GetAllCategory()
        {
            return _accountService.GetAllCategory();
        }
        
        [HttpPost]
        [Authorize]
        public ActionResult Category(CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                Api.MembershipCreateStatus createStatus;
                string currentUser = HttpContext.User.Identity.Name;
                _accountService.CreateCategory(model.CategoryName, model.CategoryDescription, out createStatus);

                if (createStatus == Api.MembershipCreateStatus.Success)
                {
                    ViewBag.SuccessMsg = 1;
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }
            return View(model);
        }
        #endregion

        [Authorize]
        public ActionResult SubCategory()
        {
            //GetAllCategory();
            ViewBag.Categories = _accountService.GetAllCategory();
            return View();
        }

        
        [HttpPost]
        [Authorize]
        public ActionResult SubCategory(SubCategoryModel model)
        {
            ViewBag.Categories = _accountService.GetAllCategory();
            if (ModelState.IsValid)
            {
                //Api.MembershipCreateStatus createStatus;
                Api.ProductCategorySubCategory createStatus;
                string currentUser = HttpContext.User.Identity.Name;
                _accountService.CreateSubCategory(model.SelectedCategoryId, model.SubCategoryName, model.SubCategoryDescription, out createStatus);

                if (createStatus == Api.ProductCategorySubCategory.Success)
                {
                    ViewBag.SuccessMsg = 1;
                }
                else
                {
                    // ModelState.AddModelError("", ErrorCodeToString(createStatus));
                    ModelState.AddModelError("", ErrorCodeToStringProduct(createStatus));
                }
            }
            return View();
        }

        //[AuthLog(HttpContext.User.Identity.Name = "superadmin")]
        #region ALL about Company & Department
        [Authorize(Users = "Super Admin")]
        public ActionResult Company()
        {
            CompanyModel Cd = new CompanyModel();
            List<CompanyList> companyData = _accountService.GetAllCompany();
            DisplayCompanyData model = new DisplayCompanyData();
            model.Company = companyData;
            Cd.DisplayCompany = model;
            return View(Cd);
        }

        private List<CompanyList> GetAllCompany()
        {
            return _accountService.GetAllCompany();
        }



        [HttpPost]
        [Authorize(Users = "Super Admin")]
        public ActionResult Company(CompanyModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                string currentUser = HttpContext.User.Identity.Name;
                /*if(currentUser == null)
                {
                    return RedirectToAction("Logon");
                }*/
                bool companystatus = _accountService.CreateCompany(model.FormSubmit.CompanyShortName, model.FormSubmit.CompanyFullName, model.FormSubmit.CompanyLocation);

                if (companystatus == true)
                {

                    ViewBag.SuccessMsg = 1;
                }
                else
                {
                    ViewBag.FailMsg = 0;
                }
            }
            CompanyModel Cd = new CompanyModel();
            List<CompanyList> companyData = _accountService.GetAllCompany();
            DisplayCompanyData model2 = new DisplayCompanyData();
            model2.Company = companyData;
            Cd.DisplayCompany = model2;
            return View(Cd);
        }
        #endregion

        [Authorize(Users = "Super Admin")]
        public ActionResult Department()
        {
            //GetAllCategory();
            DepartmentModel Dd = new DepartmentModel();
            List<DepartmentList> departmentData = _accountService.GetAllDepartment();
            DepartmentListModel model2 = new DepartmentListModel();
            model2.DepartmentList = departmentData;
            Dd.DisplayDepartment = model2;
            ViewBag.Company = _accountService.GetAllCompany();
            return View(Dd);
        }


        [HttpPost]
        [Authorize(Users = "Super Admin")]
        public ActionResult Department(DepartmentModel model)
        {
            ViewBag.Company = _accountService.GetAllCompany();
            if (ModelState.IsValid)
            {
                string currentUser = HttpContext.User.Identity.Name;
                bool departmentstatus = _accountService.CreateDepartment(model.FormSubmit.SelectedCompanyID, model.FormSubmit.DepartmentName, model.FormSubmit.DepartmentDirNme, model.FormSubmit.DepartmentFloorno, model.FormSubmit.DepartmentRoomno);

                if (departmentstatus == true)
                {

                    ViewBag.SuccessMsg = 1;
                }
                else
                {
                    ViewBag.FailMsg = 0;
                }
            }

            DepartmentModel Dd = new DepartmentModel();
            List<DepartmentList> departmentData = _accountService.GetAllDepartment();
            DepartmentListModel model2 = new DepartmentListModel();
            model2.DepartmentList = departmentData;
            Dd.DisplayDepartment = model2;
            return View(Dd);
        }

        #region All about department & Requirement form

        [Authorize(Users = "Super Admin")]
        private List<DepartmentList> GetAllDepartmen()
        {
            return _accountService.GetAllDepartment();
        }


        #endregion


        [HttpPost]
        [Authorize(Users = "Super Admin")]
        public ActionResult RequirementGeneralForm(RequirementGeneralFormModel model)
        {
            if (ModelState.IsValid)
            {
                string currentUser = HttpContext.User.Identity.Name;
                //Boolean passstatuasnull = 1;
                bool Formstatus = _accountService.RequirementGeneralForm(model.SelectedCompanyID, model.SelectedDepartmentID, model.ProductName, model.ProductDescription, model.ProductQuantity, model.CreationDate, model.RequirementDate, model.OfficeNo, currentUser);

                if (Formstatus == true)
                {

                    ViewBag.SuccessMsg = 1;
                }
                else
                {
                    ViewBag.FailMsg = -0;
                }
            }
            ViewBag.CategoryList = _accountService.GetAllCategory();
            ViewBag.SubCategoryList = _accountService.SubCategory();
            ViewBag.ProductList = _accountService.ProductList();
            ViewBag.Company = _accountService.GetAllCompany();
            ViewBag.Department = _accountService.GetAllDepartment();
            ViewBag.UserRoles = _accountService.GetAllRoles();
            return View();
        }


        public JsonResult DeleteCompany(int id)
        {
            string currentUser = HttpContext.User.Identity.Name;
            bool ProductDetails = _accountService.DeleteCompanyById(id);
            return Json(ProductDetails, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Users = "Super Admin")]
        public ActionResult RequirementGeneralForm()
        {
            ViewBag.CategoryList = _accountService.GetAllCategory();
            ViewBag.SubCategoryList = _accountService.SubCategory();
            ViewBag.ProductList = _accountService.ProductList();
            ViewBag.Company = _accountService.GetAllCompany();
            ViewBag.Department = _accountService.GetAllDepartment();
            ViewBag.UserRoles = _accountService.GetAllUsers();
            return View();
        }

        public ActionResult RequirementGeneralFormEmployee()
        {
            ViewBag.CategoryList = _accountService.GetAllCategory();
            ViewBag.SubCategoryList = _accountService.SubCategory();
            ViewBag.ProductList = _accountService.ProductList();
            //ViewBag.Company = _accountService.GetAllCompany();
            //ViewBag.Department = _accountService.GetAllDepartment();
            //ViewBag.UserRoles = _accountService.GetAllUsers();
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult RequirementGeneralFormEmployee(RequirementGeneralFormModel model)
        {
            if (ModelState.IsValid)
            {
                string currentUser = HttpContext.User.Identity.Name;
                //Boolean passstatuasnull = 1;
                bool Formstatus = _accountService.RequirementGeneralForm(model.SelectedCompanyID, model.SelectedDepartmentID, model.ProductName, model.ProductDescription, model.ProductQuantity, model.CreationDate, model.RequirementDate, model.OfficeNo, currentUser);

                if (Formstatus == true)
                {

                    ViewBag.SuccessMsg = 1;
                }
                else
                {
                    ViewBag.FailMsg = -0;
                }
            }
            ViewBag.CategoryList = _accountService.GetAllCategory();
            ViewBag.SubCategoryList = _accountService.SubCategory();
            ViewBag.ProductList = _accountService.ProductList();
            //ViewBag.Company = _accountService.GetAllCompany();
            //ViewBag.Department = _accountService.GetAllDepartment();
            //ViewBag.UserRoles = _accountService.GetAllRoles();
            return View();
        }
        [Authorize(Users = "Super Admin")]
        public ActionResult RegisterEmployee()
        {
            ViewBag.Company = _accountService.GetAllCompany();
            ViewBag.Department = _accountService.GetAllDepartment();
            ViewBag.UserRoles = _accountService.GetAllRoles();
            return View();
        }


        [HttpPost]
        [Authorize(Users = "Super Admin")]
        public ActionResult RegisterEmployee(Employee model)
        {
            //var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                string currentUser = HttpContext.User.Identity.Name;
                bool userExist = false;
                bool saveEmployeeRestult = _accountService.CreateEmployee(model.FormSubmit.CompanyId, 
                    model.FormSubmit.DepratmentId, model.FormSubmit.UserRoll, model.FormSubmit.UserName, model.FormSubmit.Email, 
                    out userExist);
                if (userExist == true)
                {
                    ViewBag.Failuremessage = "User name or Email already exist please choose another!";
                }
                else
                {
                    if (saveEmployeeRestult == true)
                    {
                        ViewBag.SuccessMsg = "Employee register successfully";
                    }
                    else
                    {
                        ViewBag.Failuremessage = "Error while adding employee, Please try again";
                    }
                }
                
            }
            
            ViewBag.Company = _accountService.GetAllCompany();
            ViewBag.Department = _accountService.GetAllDepartment();
            ViewBag.UserRoles = _accountService.GetAllRoles();
            return View();
        }

       
        public JsonResult GetUserByDepartmentCompanyID(int deptid, int companyid)
        {
            List<UserList> userlist = _accountService.GetUserByDeptComID(deptid,companyid);
            return Json(userlist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentByCompanyID(int id)
        {
            string currentUser = HttpContext.User.Identity.Name;
            List<DepartmentList> departmentDataById = _accountService.GetAllDepartmentByCompanyId(id);
            return Json(departmentDataById, JsonRequestBehavior.AllowGet);
        }

        
       

    }
}