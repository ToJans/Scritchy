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
        public class FailedCommandExceptionList : List<FailedCommandException>
        { 
        }

        ICommandBus bus;
        IEventApplier applier;
        StockDictionary readmodel;
        FailedCommandExceptionList FailedCommands;

        public BusController(ICommandBus bus, IEventApplier applier,StockDictionary readmodel, FailedCommandExceptionList FailedCommands)
        {
            this.bus = bus;
            this.readmodel = readmodel;
            this.FailedCommands = FailedCommands;
            this.applier = applier;
        }

        public ActionResult Index()
        {
            base.ViewData.Add("FailedCommands", FailedCommands);
            return View(readmodel);
        }

        public ActionResult Command(string commandtype)
        {
            object command = LoadObjectValuesFromRequestForm(commandtype);
            try
            {
                bus.RunCommand(command);
                applier.ApplyNewEventsToAllHandlers();
            }
            catch (FailedCommandException e)
            {
                FailedCommands.Add(e);
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
