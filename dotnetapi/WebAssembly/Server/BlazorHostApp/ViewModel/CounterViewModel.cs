using BlazorHostApp.Service;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Splat;

namespace BlazorHostApp.ViewModel
{
    public class CounterViewModel : ReactiveObject
    {
        private string _currentCount;

        public ReactiveCommand<Unit, Unit> Clear { get; }

        private readonly ObservableAsPropertyHelper<string> _count;

        readonly HttpService httpService;
        public CounterViewModel(HttpService httpService)
        {
            Increment = ReactiveCommand.CreateFromTask(IncrementCount);

            _count = Increment.ToProperty(this, x=>x.Message, scheduler: RxApp.MainThreadScheduler);
                 
            this.httpService = httpService;

            this.WhenAnyValue(p => p.Message)
                .Throttle(TimeSpan.FromSeconds(.8))
                .Subscribe(pr =>
                {
                    //this.Log().ErrorException("error:",);
                    _currentCount = pr;
                });

            Clear= ReactiveCommand.Create(() => {
                this.ClearMessage = "清除....";
            });

            var can=this.WhenAnyValue(p => p.ClearMessage);

           
         }

        public string CurrentCount => _currentCount;

        public string Message => _count.Value;

        public string ClearMessage { get; set; }
 
        public ReactiveCommand<Unit, string> Increment { get; }

        private Task<string> IncrementCount()
        {
            //_currentCount++;
            
            return this.httpService.GetMessages();
        }
    }
}
