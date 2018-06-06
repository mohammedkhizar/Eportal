using System;
using System.Collections.Generic;

namespace SIBF.UserManagement.Api
{
    public interface IProductService
    {
        List<RequirementList> GetAllRequestedProductList();
        bool UpdatedProductAssigned(string UserName, string ProductName, int ProductID, int SelRowID, int Quantity, string Reason, string currentUser);

        List<RequirementList> GetStockByProductID(int ProductID);
        List<RequirementList> GetRequestedProductListByID(int RequirementID);
        bool AssignProduct(string UserName, int Quantity, int ProductID, string CurrentUser, int stockID, int Requirement_ID, string Reason);
        bool ReduceQuantity(int Quantity, int ProductID);
        List<RequirementList> GetAssignedStockByRowID(int AssignedRowID);
        bool updateProductRtnQty(int stockID, int quantity);
        bool updateReduceRtnQty(int RowID, int ReturnQuantity, string currentUser, DateTime currentdatetime, string Status);
    }
}
