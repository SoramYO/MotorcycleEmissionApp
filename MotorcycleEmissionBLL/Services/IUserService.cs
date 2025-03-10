using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MotorcycleEmissionBLL.Services
{
    public interface IUserService
    {
        User GetUserById(int userId);
        IEnumerable<User> GetAllUsers();
        IEnumerable<User> GetUsersByRole(string role);
        User Authenticate(string email, string password);
        void RegisterUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int userId);
        bool ValidateUserCredentials(string email, string password);
        bool EmailExists(string email);
    }
} 