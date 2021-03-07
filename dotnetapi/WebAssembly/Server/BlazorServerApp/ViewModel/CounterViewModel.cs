using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.ViewModel
{
    public class CounterViewModel : ReactiveObject
    {
        private int _currentCount;

        private readonly ObservableAsPropertyHelper<int> _count;

 
        public CounterViewModel()
        {
            Increment = ReactiveCommand.CreateFromTask(IncrementCount);

            this.ConvertToStr = ReactiveCommand.CreateFromTask<string,string>(GetObservable);

            _count = Increment.ToProperty(this, x => x.CurrentCount, scheduler: RxApp.MainThreadScheduler);
            
        }

      

        private Task<string> GetObservable(string mesage)
        {
            //this.Name = Guid.NewGuid().ToString("N");
            //this.Name = mesage;
            return Task.FromResult(mesage);
                //Observable.Return(Guid.NewGuid().ToString("N"));
        }

        public int CurrentCount => _count.Value;

        public string Name { get; set; }

        public string Message { get; set; }


        public ReactiveCommand<Unit, int> Increment { get; }

        public ReactiveCommand<string,string> ConvertToStr { get; }
        
 
        private Task<int> IncrementCount()
        {
            _currentCount++;
            this.Name = _currentCount.ToString();

            return Task.FromResult(_currentCount);
        }
    }
}
