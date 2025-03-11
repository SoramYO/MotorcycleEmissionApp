using Microsoft.Extensions.DependencyInjection;
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

namespace MotorcycleEmissionUI.VehicleOwner
{
    /// <summary>
    /// Interaction logic for NotificationsView.xaml
    /// </summary>
    public partial class NotificationsView : UserControl
    {
		private readonly INotificationService _notificationService;
		private User _user;
		public NotificationsView()
		{
			InitializeComponent();

			// Lấy service từ DI container
			_notificationService = new NotificationService();
			_user = Application.Current.Properties["User"] as User;

			// Tải danh sách thông báo
			LoadNotifications();
		}

		private void LoadNotifications()
		{
			try
			{
				// Lấy danh sách thông báo của người dùng hiện tại
				int userId = _user.UserId;
				List<Notification> notifications = _notificationService.GetNotificationsByUser(userId);

				// Hiển thị lên ListView
				NotificationsListView.ItemsSource = notifications;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi tải thông báo: {ex.Message}", "Lỗi",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}