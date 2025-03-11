using Microsoft.Extensions.DependencyInjection;
using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using MotorcycleEmissionUI.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MotorcycleEmissionUI.VehicleOwner
{
    public partial class MyVehiclesView : UserControl
    {
        private readonly IVehicleService _vehicleService;
		private User _user;
		public MyVehiclesView()
        {
            InitializeComponent();
            
            // Lấy service từ DI container
            _vehicleService = new VehicleService();
			_user = Application.Current.Properties["User"] as User;
			// Tải dữ liệu xe
			LoadVehicles();
        }
        
        private void LoadVehicles()
        {
            try
            {
                // Di chuyển code từ MainAppWindow.xaml.cs, phương thức LoadVehicles
                var vehicles = _vehicleService.GetVehiclesByOwner(_user.UserId);
                VehiclesDataGrid.ItemsSource = vehicles;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu xe: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void AddVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            // Di chuyển code từ MainAppWindow.xaml.cs, phương thức AddVehicleButton_Click
            var addVehicleDialog = new AddVehicleDialog(_user.UserId);
            addVehicleDialog.Owner = Window.GetWindow(this);
            
            if (addVehicleDialog.ShowDialog() == true)
            {
                // Nếu thêm thành công, tải lại danh sách xe
                LoadVehicles();
            }
        }
        
        private void RefreshVehiclesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadVehicles();
        }
        
        private void EditVehicle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int vehicleId)
            {
                var editVehicleDialog = new EditVehicleDialog(_user.UserId, vehicleId);
                editVehicleDialog.Vehicle = _vehicleService.GetVehicleById(vehicleId);
				editVehicleDialog.Owner = Window.GetWindow(this);
                
                if (editVehicleDialog.ShowDialog() == true)
                {
                    // Nếu sửa thành công, tải lại danh sách xe
                    LoadVehicles();
                }
            }
        }
        
        private void ScheduleInspection_Click(object sender, RoutedEventArgs e)
        {

            if (sender is Button button && button.Tag is int vehicleId)
            {
                var scheduleDialog = new ScheduleInspectionDialog();
                scheduleDialog.vehicle = _vehicleService.GetVehicleById(vehicleId);

				scheduleDialog.Owner = Window.GetWindow(this);

                if (scheduleDialog.ShowDialog() == true)
                {
                    MessageBox.Show("Đặt lịch kiểm định thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        
        private void ViewVehicleHistory_Click(object sender, RoutedEventArgs e)
        {
            // Di chuyển code từ MainAppWindow.xaml.cs, phương thức ViewVehicleHistory_Click
            if (sender is Button button && button.Tag is int vehicleId)
            {
                // Chuyển đến UserControl lịch sử kiểm định và đặt vehicleId đang chọn
                var historyView = new InspectionHistoryView();
                historyView.Vehicle = _vehicleService.GetVehicleById(vehicleId);

				
                var ownerDashboard = Window.GetWindow(this) as OwnerDashboard;
                ownerDashboard.ContentArea.Content = historyView;
			}
        }
    }
}