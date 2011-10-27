using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Example.Infrastructure;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Example.Domain.Readmodel;
using Scritchy.Infrastructure;
using Scritchy.Infrastructure.Exceptions;

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
        IEventApplier applier;
        StockDictionary readmodel;
        CommandHistory cmdhist;

        public BusController(ICommandBus bus, IEventApplier applier, StockDictionary readmodel, CommandHistory cmdhist)
        {
            this.bus = bus;
            this.readmodel = readmodel;
            this.cmdhist = cmdhist;
            this.applier = applier;
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
                applier.ApplyNewEventsToAllHandlers();
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
