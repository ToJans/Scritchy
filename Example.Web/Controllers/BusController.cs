using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Example.Domain.Readmodel;
using Scritchy.Infrastructure;
using Scritchy.Infrastructure.Exceptions;
using Scritchy.Infrastructure.Configuration;

namespace Example.Web.Controllers
{

    public class BusController : Controller
    {
        public class CommandHistory
        {
            List<FailedCommandException> hist = new List<FailedCommandException>();

            public void Add(FailedCommandException ex)
            {
                hist.Insert(0,ex);
                if (hist.Count > 20)
                    hist = hist.Take(20).ToList();
            }

            public IEnumerable<FailedCommandException> Items
            {
                get
                {
                    return hist;
                }
            }
        }

        ICommandBus bus;
        StockDictionary readmodel;
        CommandHistory cmdhist;

        public BusController(ICommandBus bus , StockDictionary readmodel, CommandHistory cmdhist)
        {
            this.bus = bus;
            this.readmodel = readmodel;
            this.cmdhist = cmdhist;
        }

        public ActionResult Index()
        {
            base.ViewData.Add("RecentCommands", cmdhist);
            return View(readmodel);
        }

        public ActionResult Command(string commandtype)
        {
            object command = LoadObjectValuesFromRequestForm(commandtype);
            try
            {
                bus.RunCommand(command);
                cmdhist.Add(new FailedCommandException(new Exception("OK"), command));
            }
            catch (FailedCommandException e)
            {
                cmdhist.Add(e);
            }
            return this.RedirectToAction("Index");
        }

        protected object LoadObjectValuesFromRequestForm(string typename)
        {
            var t = typeof(Example.Domain.Commands.AddItems);
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
