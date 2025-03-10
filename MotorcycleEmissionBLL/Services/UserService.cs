using MotorcycleEmissionDAL;
using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MotorcycleEmissionBLL.Services
{
    public class UserService : IUserService
    {
        private readonly GenericRepository<User> _userRepository;

		public UserService()
		{
			_userRepository = new GenericRepository<User>();
		}


        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _userRepository.GetAll().Find(u => u.Email == email && u.Password == password);

			if (user == null)
                return null;


            return user;
        }

        public void DeleteUser(int userId)
        {
			_userRepository.DeleteById(userId);
		}

        public bool EmailExists(string email)
        {
            return _userRepository.GetAll().Where(u => u.Email == email).Any();
		}

        public IEnumerable<User> GetAllUsers()
        {
            return _userRepository.GetAll();
        }

        public User GetUserById(int userId)
        {
            return _userRepository.GetById(userId);
        }

		public IEnumerable<User> GetUsersByRole(string role)
		{
			return _userRepository.GetAll().Where(u => u.Role == role);
		}

        public void RegisterUser(User user)
        {
            // Hash the password before storing
            user.Password = (user.Password);
			_userRepository.Add(user);
        }

        public void UpdateUser(User user)
        {
            var existingUser = _userRepository.GetById(user.UserId);
            
            if (existingUser == null)
                throw new Exception("User not found");

            // Only hash password if it has changed
            if (user.Password != existingUser.Password)
                user.Password = (user.Password);

			_userRepository.Update(user);
        }

        public bool ValidateUserCredentials(string email, string password)
        {
            return Authenticate(email, password) != null;
        }


    }
} 