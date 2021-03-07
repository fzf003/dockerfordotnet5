using Akka.Client.Autogen.Grpc.v1;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
using static Akka.Client.Autogen.Grpc.v1.UserService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AkkaGrpcClient;

namespace AkkaCLientApp
{
    public class UserInfo
    {
        public UserInfo(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }


    class Program
    {

        static async Task Main(string[] args)
        {
            var httpclient=new System.Net.Http.HttpClient(new ClientHandler());

            var channel = GrpcChannel.ForAddress("https://localhost:5009/"); 

            var userclient = new UserServiceClient(channel);

            for (; ; )
            {
                try
                {
                    var rawresponse = await userclient.QueryUsersAsync(new RequestMessae
                    {
                        Message = Guid.NewGuid().ToString()
                    });



                    Console.WriteLine(rawresponse.Message);

                   
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Console.ReadKey();
            }

 
            Console.WriteLine("........");
            Console.ReadKey();
        }

        static Product GetProduct()
        {
            return new Product
            {
                ///Id = 1,
                Name = "刘禹锡-0791",
                Category = "古诗文-四季-秋天",
                CreateTime = DateTime.Now,
                Summary = "秋风引",
                Description = "何处秋风至？萧萧送雁群。",
                ImageFile = "www.baidu.com",
                Price = 99.8m,
                Status = 90
            };
        }

        static void EntityFrameworkCase()
        {
            var services = new ServiceCollection();
            services.AddDbContextFactory<ProductTinyContext>(options =>
            {

                options.UseSqlServer("server=.,14330;Initial Catalog=fzf0031;User ID=sa;Password=!fzf123456;MultipleActiveResultSets=true");
            });

            //services.AddDbContext<ProductTinyContext>(options => options.UseSqlServer(""));
            services.AddHttpClient();
            

            var provider = services.BuildServiceProvider();

            var factory = provider.GetService<IDbContextFactory<ProductTinyContext>>();
        }
 
    }
}
