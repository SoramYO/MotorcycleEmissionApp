using Microsoft.Extensions.DependencyInjection;
using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MotorcycleEmissionUI.Inspector
{
    public partial class InspectorDashboard : Window
    {
        private readonly IInspectionService _inspectionService;
		private User _user;

		public InspectorDashboard()
        {
            InitializeComponent();
            
            // Lấy service từ DI container
            _inspectionService = new InspectionService();
			_user = Application.Current.Properties["User"] as User;


			// Hiển thị thông tin người dùng
			UserNameDisplay.Text = $"Xin chào, {_user.FullName}";
            
            // Mặc định hiển thị màn hình Phương Tiện Đang Chờ
            PendingInspectionsButton_Click(null, null);
        }
        
        private void PendingInspectionsButton_Click(object sender, RoutedEventArgs e)
        {
            var pendingView = new PendingInspectionsView();
            ContentArea.Content = pendingView;
        }
        
        private void CompletedInspectionsButton_Click(object sender, RoutedEventArgs e)
        {
            var completedView = new CompletedInspectionsView();
            ContentArea.Content = completedView;
        }
        
        private void InspectionStandardsButton_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị tiêu chuẩn kiểm định
        }
        
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Xóa thông tin người dùng hiện tại
            Application.Current.Properties["User"] = null;

			// Mở cửa sổ đăng nhập
			var loginWindow =new LoginWindow();
            loginWindow.Show();
            
            // Đóng cửa sổ hiện tại
            this.Close();
        }
    }
}