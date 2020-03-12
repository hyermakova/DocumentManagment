using DocumentManagment.Domain.Model;

namespace DocumentManagment.Services.Providers
{
    public class UserProvider : IUserProvider
    {
        public User User => new User { Id = "123" };
    }
}
