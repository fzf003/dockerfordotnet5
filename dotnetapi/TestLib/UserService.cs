using System;

namespace TestLib
{
    public class UserService
    {
        public string GetUser()
        {
            return $"fzf-{Guid.NewGuid().ToString("N")}";
        }
    }
}
