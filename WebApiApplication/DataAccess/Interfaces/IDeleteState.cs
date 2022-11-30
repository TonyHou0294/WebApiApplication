using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiApplication.DataAccess.Interfaces
{
    public interface IDeleteState
    {
        bool IsDeleted { get; set; }
    }
}
