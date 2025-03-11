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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;

namespace MotorcycleEmissionUI.VehicleOwner
{
	/// <summary>
	/// Interaction logic for OwnerDashboard.xaml
	/// </summary>
	public partial class OwnerDashboard : Window
	{
		private readonly VehicleService _vehicleService = new VehicleService();
		private readonly InspectionService _inspectionService = new InspectionService();
		private readonly NotificationService _notificationService = new NotificationService();


		public OwnerDashboard()
		{
			InitializeComponent();

			// Lấy service từ DI container
			 


			// Hiển thị thông tin người dùng
			var user = Application.Current.Properties["User"] as User;
			UserNameDisplay.Text = $"Xin chào, {user.FullName}";

			// Mặc định hiển thị màn hình My Vehicles
			MyVehiclesButton_Click(null, null);
		}

		private void MyVehiclesButton_Click(object sender, RoutedEventArgs e)
		{
			// Tạo view
			var myVehiclesView = new MyVehiclesView();

			// Thiết lập nội dung trực tiếp
			ContentArea.Content = myVehiclesView;
		}


		private void NotificationsButton_Click(object sender, RoutedEventArgs e)
		{
			var notificationsView = new NotificationsView();
			ContentArea.Content = notificationsView;
		}

		private void LogoutButton_Click(object sender, RoutedEventArgs e)
		{
			// Xóa thông tin người dùng hiện tại
			Application.Current.Properties["User"] = null;

			// Mở cửa sổ đăng nhập
			var loginWindow = new LoginWindow();
			loginWindow.Show();

			// Đóng cửa sổ hiện tại
			this.Close();
		}
	}
}
