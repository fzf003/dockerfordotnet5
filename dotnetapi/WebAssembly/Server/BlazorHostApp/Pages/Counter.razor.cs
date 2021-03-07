using BlazorHostApp.Service;
using BlazorHostApp.ViewModel;
using Microsoft.AspNetCore.Components;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BlazorHostApp.Pages
{
    public partial class Counter 
    {
        private int currentCount = 0;
        string Message = string.Empty;
 
        [Inject]
        public CounterViewModel CounterViewModel
        {
            get => ViewModel;
            set => ViewModel = value;
        }

        private async Task IncrementCount()
        {
            
           await CounterViewModel.Increment.Execute().ToTask();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Console.WriteLine($"per-{propertyName}");
            base.OnPropertyChanged(propertyName);
        }
    }
}
