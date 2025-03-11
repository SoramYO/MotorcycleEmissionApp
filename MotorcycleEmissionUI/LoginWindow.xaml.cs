using Microsoft.Extensions.DependencyInjection;
using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL;
using MotorcycleEmissionDAL.Models;
using MotorcycleEmissionUI.Inspector;
using MotorcycleEmissionUI.Police;
using MotorcycleEmissionUI.Station;
using MotorcycleEmissionUI.VehicleOwner;
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
	/// Interaction logic for LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		private readonly IUserService _userService;

		// Sự kiện khi đăng nhập thành công
		public event EventHandler<User> LoginSuccessful;

		public LoginWindow()
		{
			InitializeComponent();

			// Focus vào trường email khi khởi động
			Loaded += (s, e) => EmailTextBox.Focus();

			_userService = new UserService();
		}

		private void SubmitLoginButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Di chuyển code từ MainAppWindow.xaml.cs, phương thức SubmitLoginButton_Click
				string email = EmailTextBox.Text;
				string password = PasswordBox.Password;

				if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
				{
					MessageBox.Show("Vui lòng nhập email và mật khẩu!", "Lỗi đăng nhập",
						MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				// Xác thực người dùng
				var user = _userService.Authenticate(email, password);

				if (user != null)
				{
					// Lưu thông tin người dùng đã đăng nhập
					Application.Current.Properties["User"] = user;

					if (user.Role == "Owner")
					{
						// Mở cửa sổ chính
						var mainWindow = new OwnerDashboard();

						mainWindow.Show();

						// Đóng cửa sổ đăng nhập
						Close();
					}
					else if (user.Role == "Inspector")
					{
						var inspectorWindow = new InspectorDashboard();
						inspectorWindow.Show();
						Close();

					}
					else if (user.Role == "Station")
					{
						var stationDashboard = new StationDashboard();
						stationDashboard.Show();
						Close();

					}
					else if (user.Role == "Police")
					{
						var policeWindow = new PoliceDashboard();
						policeWindow.Show();
						Close();
					}
					else
					{
						MessageBox.Show("Tài khoản không có quyền truy cập!", "Lỗi đăng nhập",
							MessageBoxButton.OK, MessageBoxImage.Error);

					}
				}
				else
				{
					MessageBox.Show("Email hoặc mật khẩu không đúng!", "Lỗi đăng nhập",
						MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi hệ thống",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void RegisterLink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			// Mở cửa sổ đăng ký
			var registerWindow = new RegisterWindow();
			registerWindow.Show();
			Close();
		}
	}
}
