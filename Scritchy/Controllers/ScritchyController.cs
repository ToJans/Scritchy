using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scritchy.Controllers
{
    public class ScritchyController : Controller
    {
        //
        // GET: /Scritchy/

        public ActionResult Index(string viewname)
        {
            return View(viewname??"Index");
        }

    }
}
