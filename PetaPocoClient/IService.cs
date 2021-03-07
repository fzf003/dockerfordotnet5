using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace PetaPocoClient
{
    public interface ISampleService
    {

        Task<string> Invoke();
    }

    public class SampleService : ISampleService
    {
        readonly ILogger _logger;
        public SampleService(ILogger logger)
        {
            _logger = logger;
        }

      
        public Task<string> Invoke()
        {
            _logger.Info("执行.....");
            return Task.FromResult(Guid.NewGuid().ToString());
        }
    }

    public interface ILogger
    {
        void Info(string message);
    }

    public class ConsoleLogger : ILogger
    {
       
        public void Info(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class Intercept : IInterceptor
    {
        void IInterceptor.Intercept(IInvocation invocation)
        {
           
            invocation.Proceed();
        }
    }

    public class StandardInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            PreProceed(invocation);
            PerformProceed(invocation);
            PostProceed(invocation);
        }

        protected virtual void PerformProceed(IInvocation invocation)
        {
            Console.WriteLine($"执行:{invocation.Method.Name}之中");
           
            invocation.ReturnValue = "opp";
            invocation.Proceed();
        }

        protected virtual void PreProceed(IInvocation invocation)
        {
            Console.WriteLine($"执行:{invocation.Method.Name}之前");

        }

        protected virtual void PostProceed(IInvocation invocation)
        {
            Console.WriteLine($"执行:{invocation.Method.Name}之后");

        }
    }
}
