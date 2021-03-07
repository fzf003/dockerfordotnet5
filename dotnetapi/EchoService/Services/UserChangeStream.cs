using Echotell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoService.Services
{
    public interface IChangeStream:IDisposable
    {
        event Func<UserResponse, Task> OnChange;
        Task OnProcess(UserRequest userRequest, CancellationToken cancellationToken);
    }
    public class UserChangeStream : IChangeStream
    {
        public event Func<UserResponse,Task> OnChange;
       

        public async Task OnProcess(UserRequest userRequest, CancellationToken cancellationToken)
        {
            var handler = OnChange;
            while(!cancellationToken.IsCancellationRequested)
            {
               await handler(new UserResponse { Body = Guid.NewGuid().ToString("N") });
            }
        }


        public void Dispose()
        {
            Console.WriteLine("释放.....");
        }
    }
}
