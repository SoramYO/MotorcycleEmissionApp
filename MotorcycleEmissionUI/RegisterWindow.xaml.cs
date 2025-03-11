using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MotorcycleEmissionUI
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
		private readonly IUserService _userService;
		public RegisterWindow()
        {
            InitializeComponent();
			_userService = new UserService();
		}

        private void SubmitRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
			string phoneNumber = PhoneNumberTextBox.Text;
			string fullName = FullNameTextBox.Text;
            string address = AddressTextBox.Text;
			string role = "Owner";
			if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword) || string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(address))
			{
				MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi đăng ký",
					MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (password != confirmPassword)
			{
				MessageBox.Show("Mật khẩu không khớp!", "Lỗi đăng ký",
					MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (_userService.EmailExists(email))
			{
				MessageBox.Show("Email đã tồn tại!", "Lỗi đăng ký",
					MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			// Tạo mới người dùng
			User user = new User
			{
				Email = email,
				Password = password,
				Phone = phoneNumber,
				FullName = fullName,
				Address = address,
				Role = role
			};
			// Đăng ký người dùng
			_userService.RegisterUser(user);
			MessageBox.Show("Đăng ký thành công!", "Đăng ký",
				MessageBoxButton.OK, MessageBoxImage.Information);
			// Đóng cửa sổ đăng ký
			LoginWindow loginWindow = new LoginWindow();
			loginWindow.Show();
			this.Close();
		}
		private void LoginLink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			LoginWindow loginWindow = new LoginWindow();
			loginWindow.Show();
			this.Close();
		}
	}
}
