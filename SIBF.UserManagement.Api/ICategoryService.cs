using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIBF.UserManagement.Api
{
    public interface ICategoryService
    {
        bool DeleteData(int id, string TableName, string ColName, string CurrentUser);
    }
}
