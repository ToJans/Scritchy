using System;
using Scritchy.Infrastructure.Configuration;
using Scritchy.Infrastructure.Implementations;
using Scritchy.Infrastructure.Implementations.EventStorage;
using System.ComponentModel;

namespace Scritchy.Infrastructure
{
    public class ScritchyBus : ICommandBus
    {
        public IEventStore EventStore { get; private set; }

        HandlerRegistry Registry;
        ICommandBus Bus;
        IHandlerInstanceResolver resolver;
        IEventApplier applier;
        bool DoNotApplyEvents = false;
        
        public ScritchyBus(Func<Type, object> LoadHandler = null, IEventstoreAdapter adapter = null, bool DoNotApplyEvents = false)
        {
            if (LoadHandler == null)
                LoadHandler = x => Activator.CreateInstance(x);
            Registry = new ConventionBasedRegistry();
            EventStore = new Scritchy.Infrastructure.Implementations.EventStorage.EventStore(adapter: adapter);
            resolver = new HandlerInstanceResolver(EventStore, Registry, LoadHandler);
            Bus = new CommandBus(EventStore, Registry, resolver);
            applier = new EventApplier(EventStore, Registry, resolver);
            this.DoNotApplyEvents = DoNotApplyEvents;
        }

        public void RunCommand(object Command)
        {
            Bus.RunCommand(Command);
            if (DoNotApplyEvents == false)
            {
                applier.ApplyNewEventsToAllHandlers();
            }
        }

        public void RunCommandAsync(object Command,Action<RunWorkerCompletedEventArgs> RunWorkerCompleted)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler((o, ea) => (ea.Argument as ScritchyBus).RunCommand(Command));
            if (RunWorkerCompleted != null)
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler((o,rwce)=>RunWorkerCompleted(rwce));
            bw.RunWorkerAsync(this);
        }

        public void ApplyNewEventsToAllHandlers()
        {
            applier.ApplyNewEventsToAllHandlers();
        }

        public void ApplyNewEventsToAllHandlersAsync(Action<RunWorkerCompletedEventArgs> RunWorkerCompleted)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler((o, ea) => (ea.Argument as EventApplier).ApplyNewEventsToAllHandlers());
            if (RunWorkerCompleted != null)
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler((o, rwce) => RunWorkerCompleted(rwce));
            bw.RunWorkerAsync(applier);
        }
    }


}
