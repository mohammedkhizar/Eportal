using SIBF.UserManagement.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using X.PagedList;

namespace SIBF.UserManagement.Models
{
    public class CategorysModels
    {
       public CategoryDisplayModel ListDataModel { get; set; }
       public CategorySubmitModel SubmitFormModel { get; set; }

      // public SubCategoryDisplayModel SubListDataModel { get; set; }
      // public SubCategorySubmitModel SubSubmitFormModel { get; set; }
    }

    public class CategorySubmitModel
    {
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public string CategoryDescription { get; set; }
        public int CategoryID { get; set; }
    }

    public class CategoryDisplayModel
    {

        public List<CategoryList> Categorys { get; set; }
        public IPagedList<CategoryList> CategoryPage { get; set; }
    }

    public class SubCategoryModels
    {
        public SubCategoryDisplayModel ListDataModel { get; set; }
        public SubCategorySubmitModel SubmitFormModel { get; set; }
    }

    public class SubCategorySubmitModel
    {
        [Required]
        public string SubCategoryName { get; set; }
        [Required]
        public string SubCategoryDescription { get; set; }
        public int SubCategoryID { get; set; }
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
    }

    public class SubCategoryDisplayModel
    {

        public List<SubCategoryList> SubCategorys { get; set; }
        public IPagedList<SubCategoryList> CategoryPage { get; set; }
    }

}