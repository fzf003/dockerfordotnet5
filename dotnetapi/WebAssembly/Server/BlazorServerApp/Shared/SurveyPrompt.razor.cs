using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Shared
{
    public partial class SurveyPrompt
    {
        [Parameter]
        public string Title { get; set; }
    }
}
