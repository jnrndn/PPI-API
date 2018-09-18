using System.Threading.Tasks;
using PPI.API.Models;

namespace PPI.API.Data
{
    public interface IAuthRrepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExist(string username);
    }
}