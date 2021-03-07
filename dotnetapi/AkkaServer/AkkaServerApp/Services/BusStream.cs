using Akka;
using Akka.Streams;
using Akka.Streams.Implementation.Fusing;
using Akka.Streams.Stage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AkkaServerApp.Services
{
    public class BusStream : GraphStageWithMaterializedValue<SourceShape<string>, NotUsed>
    {
        public BusStream()
        {
            this.Out = new Outlet<string>("EventHubSource.Out");
        }
        public Outlet<string> Out { get; } 
        
        
        public override SourceShape<string> Shape => new SourceShape<string>(this.Out);

        public override ILogicAndMaterializedValue<NotUsed> CreateLogicAndMaterializedValue(Attributes inheritedAttributes)
        {
            var logic = new Logic(this,new Queue<string>());

            return new LogicAndMaterializedValue<NotUsed>(logic, NotUsed.Instance);

        }

        private sealed class Logic : GraphStageLogic
        {
            Action<TaskCompletionSource<NotUsed>> _openCallback;
            Action<string> _processCallback;
            readonly BusStream _busStream;
            readonly Queue<string> _pendingEvents;
            public Logic(BusStream busStream, Queue<string> pendingEvents) : base(busStream.Shape)
            {
                //Akka.Streams.Stage.InGraphStageLogic
                this._busStream = busStream;
                _pendingEvents = pendingEvents;
            }
            public override void PreStart()
            {
                // base.PreStart();
                _openCallback = GetAsyncCallback<TaskCompletionSource<NotUsed>>(OnOpen);
                _processCallback = GetAsyncCallback<string>(OnProcessEvents);

                _processCallback(string.Empty);
            }

            private void OnProcessEvents(string obj)
            {
                // Console.WriteLine(obj);

                _pendingEvents.Enqueue(Guid.NewGuid().ToString("N"));


                TryPush();
            }

            private void TryPush()
            {
                // Wait for new messages
                if (_pendingEvents == null || _pendingEvents.Count == 0)
                    return;

                if (IsAvailable(_busStream.Out))
                {
                    Push(_busStream.Out, _pendingEvents.Dequeue());

                    // We have processed all messages so we can handle more
                    if (_pendingEvents.Count == 0)
                    {
                        Console.WriteLine("完成...");
                    }
                       // _pendingCompletion.TrySetResult(NotUsed.Instance);
                }
            }

            private void OnOpen(TaskCompletionSource<NotUsed> comple)
            {
                comple.TrySetResult(NotUsed.Instance);

                Console.WriteLine("打开...");
            }

            public override void PostStop()
            {
                base.PostStop();
            }
        }
    }
}
