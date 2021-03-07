using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyService.Core;
using TinyService.RequestResponse;
using TinyService.Utils;

namespace OrderSercice.Services
{
    public record User(string name, int age, string body);

    public record ReplyMessage(bool success, string error, string message);


    public class UserActor:ProcessActor
    {
        public record UserInfo(string name,string body);

        readonly ILogger<UserActor> logger;
        public UserActor(ILogger<UserActor> logger)
        {
            this.logger = logger;
        }

        public override void Handle(Started message)
        {
            base.Handle(message);
        }

        public void Handle(UserInfo users)
        {
            this.logger.LogInformation($"{users.name}--{users.body}");
        }

        public void Handle(Request<UserInfo> request)
        {
            try
            {
                var userinfo = request.Data;
                this.logger.LogInformation($"{userinfo.name}--{userinfo.body}");
                Context.Respond($"{userinfo.name}---{Guid.NewGuid().ToString()}");
            }
            catch (Exception ex)
            {
                Context.Respond($"Error:{ex.Message}");
            }
        }

        
    }
}
