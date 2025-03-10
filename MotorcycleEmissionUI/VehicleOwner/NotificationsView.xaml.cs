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
    public partial class NotificationsView : Window
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

		private void MarkAsRead_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.Tag is int notificationId)
			{
				try
				{
					// Đánh dấu thông báo đã đọc
					var updatedNotification = _notificationService.GetNotificationById(notificationId);

					if (updatedNotification != null && updatedNotification.IsRead == true)
					{
						// Tải lại danh sách thông báo
						LoadNotifications();
					}
					else
					{
						MessageBox.Show("Không thể đánh dấu thông báo đã đọc. Vui lòng thử lại!", "Lỗi",
							MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi hệ thống",
						MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void DeleteNotification_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.Tag is int notificationId)
			{
				try
				{
					// Xác nhận xóa
					var result = MessageBox.Show("Bạn có chắc muốn xóa thông báo này không?", "Xác nhận",
						MessageBoxButton.YesNo, MessageBoxImage.Question);

					if (result == MessageBoxResult.Yes)
					{
						var deletedNotification = _notificationService.GetNotificationById(notificationId);

						if (deletedNotification == null)
						{
							// Tải lại danh sách thông báo
							LoadNotifications();
						}
						else
						{
							MessageBox.Show("Không thể xóa thông báo. Vui lòng thử lại!", "Lỗi",
								MessageBoxButton.OK, MessageBoxImage.Error);
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi hệ thống",
						MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
	}
}