using BlazorHostApp.ViewModel;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorHostApp.Pages
{
    public partial class Index
    {
        
        [Inject]
        public ReactiveMessage FetchViewModel
        {
            get => ViewModel;
            set => ViewModel = value;
        }

        protected override void Dispose(bool disposing)
        {
           
            this.FetchViewModel.Dispose();
            base.Dispose(disposing);
        }


    }
}
