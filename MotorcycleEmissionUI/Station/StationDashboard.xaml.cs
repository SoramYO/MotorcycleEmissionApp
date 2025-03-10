using Microsoft.Extensions.DependencyInjection;
using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MotorcycleEmissionUI.Station
{
    public partial class StationDashboard : Window
    {
        private readonly IInspectionService _inspectionService;
        private readonly IStationService _stationService;
		private User _user;

		public StationDashboard()
        {
            InitializeComponent();
            
            // Lấy service từ DI container
            _inspectionService = new InspectionService();
            _stationService = new StationService();
			_user = Application.Current.Properties["User"] as User;
			// Hiển thị thông tin người dùng
			UserNameDisplay.Text = $"Xin chào, {_user.FullName}";
            
            // Mặc định hiển thị màn hình Lịch Hẹn
            AppointmentsButton_Click(null, null);
        }
        
        private void AppointmentsButton_Click(object sender, RoutedEventArgs e)
        {

        }
        
        private void DailyScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            //var dailyScheduleView = App.ServiceProvider.GetRequiredService<DailyScheduleView>();
            //ContentArea.Content = dailyScheduleView;
        }
        
        private void InspectionReportsButton_Click(object sender, RoutedEventArgs e)
        {
            var reportsView = new ReportsView();
            ContentArea.Content = reportsView;
        }
        
        private void StationProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị UserControl thông tin trung tâm
            // Có thể tạo StationProfileView
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