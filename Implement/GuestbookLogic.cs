using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lushi.PaperProducts.DataAccessLayer.DataBase;

namespace Lushi.PaperProducts.BusinessLogicLayer.Implement
{
    public class GuestbookLogic
    {
        protected GuestbookDao Dao { get { return new GuestbookDao(); } }
    }
}
