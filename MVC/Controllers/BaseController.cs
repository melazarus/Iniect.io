using System.Web.Mvc;
using Iniect.io;

namespace MVC.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            Factory.Inject(this);
        }
    }
}