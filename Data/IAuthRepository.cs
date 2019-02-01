using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using PPI.API.Models;

namespace PPI.API.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExist(string username);
        Task<bool> Recover(string email);
    }
}