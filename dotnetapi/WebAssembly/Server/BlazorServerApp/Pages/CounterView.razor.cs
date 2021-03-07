using BlazorServerApp.Service;
using BlazorServerApp.ViewModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace BlazorServerApp.Pages
{
    public partial class CounterView
    {

         ILogger<CounterView> logger;
      
         public CounterView()
        {
           
            ViewModel = new CounterViewModel();
        }

        private async Task IncrementCount()
        {
           var responser= await  this.httpService.GetMessages();
            this.ViewModel.Name = responser;
            await ViewModel.Increment.Execute().ToTask();
        }

         Task GetMessage(string messsge)
        {

             ViewModel.ConvertToStr.Execute(messsge).Subscribe();
            logger.LogInformation("response:{0}", messsge);
            return Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            logger = factory.CreateLogger<CounterView>();
            var response = await this.httpService.GetMessages();
            logger.LogInformation("response:{0}", response);
            this.ViewModel.Name = response;
            await base.OnInitializedAsync();
        }



    }
}
