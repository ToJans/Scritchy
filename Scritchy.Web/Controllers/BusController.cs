using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Example.Domain.Infrastructure;
using Example.Domain.Implementation.Commands;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Example.Domain.Implementation.Readmodel;

namespace Scritchy.Web.Controllers
{
    public class BusController : Controller
    {
        ExampleBus bus;
        StockDictionary readmodel;

        public BusController(ExampleBus bus,StockDictionary readmodel)
        {
            this.bus = bus;
            this.readmodel = readmodel;
        }

        //
        // GET: /Bus/

        public ActionResult Index()
        {
            base.ViewData.Add("PublishedEvents", bus.PublishedEvents);
            return View(readmodel);
        }

        public ActionResult Command(string commandtype)
        {
            object command = LoadObjectValuesFromRequestForm(commandtype);
            bus.RunCommand(command);
            return this.RedirectToAction("Index");
        }

        protected object LoadObjectValuesFromRequestForm(string typename)
        {
            var t = typeof(Example.Domain.Implementation.Commands.AddItems);
            var ctype = t.Assembly.GetType(typename);
            var inst = Activator.CreateInstance(ctype);
            MethodInfo mi = this.GetType().GetMethod("InternalUpdateModel", BindingFlags.NonPublic|BindingFlags.Instance);
            return mi.MakeGenericMethod(ctype).Invoke(this,new object[]{inst});
        }

        private T InternalUpdateModel<T>(T o) where T:class
        {
            UpdateModel<T>(o);
            return o;
        }


    }
}
