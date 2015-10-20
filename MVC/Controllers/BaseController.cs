using Iniect.io;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            Factory.Static.Inject(this);
        }
    }
}