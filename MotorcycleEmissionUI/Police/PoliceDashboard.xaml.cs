using Microsoft.Extensions.DependencyInjection;
using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MotorcycleEmissionUI.Police
{
    public partial class PoliceDashboard : Window
    {
        private readonly IVehicleService _vehicleService;
        private readonly IInspectionService _inspectionService;
		private User _user;

		public PoliceDashboard()
        {
            InitializeComponent();
            
            // Lấy service từ DI container
            _vehicleService = new VehicleService();
            _inspectionService = new InspectionService();

			_user = Application.Current.Properties["User"] as User;
			// Hiển thị thông tin người dùng
			UserNameDisplay.Text = $"Xin chào, {_user.FullName}";
            
            // Mặc định hiển thị màn hình Tra Cứu Phương Tiện
            VehicleSearchButton_Click(null, null);
        }
        
        private void VehicleSearchButton_Click(object sender, RoutedEventArgs e)
        {
            var searchView = new InspectionService();
			ContentArea.Content =  searchView;
        }
        
        private void ExpiredInspectionsButton_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị danh sách xe hết hạn kiểm định
            var expiredView = new ExpiredInspectionsView();
            ContentArea.Content = expiredView;
        }
        
        private void ViolationReportButton_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị màn hình xử lý vi phạm
            var violationView = new ViolationReportView();
            ContentArea.Content = violationView;
        }
        
        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            var statsView = new StatisticsView();
            ContentArea.Content = statsView;
        }
        
        private void ExpiredVehiclesButton_Click(object sender, RoutedEventArgs e)
        {
            var expiredVehiclesView = new ExpiredInspectionsView();
			ContentArea.Content = expiredVehiclesView;
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