using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ProductApi
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }

    public class UserSetues
    {
        public string Name { get; set;}
    }

    public class WebClient
    {
        private readonly UserSetues _userSetues;

        private readonly HttpClient httpClient;

        public WebClient(IOptions<UserSetues> usersetes, HttpClient httpClient)
        {
            _userSetues = usersetes?.Value;

            this.httpClient = httpClient;
        }
    }

}
