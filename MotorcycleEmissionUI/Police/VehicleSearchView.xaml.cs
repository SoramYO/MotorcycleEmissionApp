using Microsoft.Extensions.DependencyInjection;
using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MotorcycleEmissionUI.Police
{
    public partial class VehicleSearchView : UserControl
    {
        private readonly IVehicleService _vehicleService;
        private readonly IInspectionService _inspectionService;
        
        public VehicleSearchView()
        {
            InitializeComponent();
            
            // Lấy service từ DI container
            _vehicleService = new VehicleService();
			_inspectionService = new InspectionService();
            
            // Focus vào trường tìm kiếm khi load
            Loaded += (s, e) => SearchPlateTextBox.Focus();
        }
        
        private void SearchVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string plateNumber = SearchPlateTextBox.Text.Trim();
                
                if (string.IsNullOrWhiteSpace(plateNumber))
                {
                    MessageBox.Show("Vui lòng nhập biển số xe cần tìm!", "Lỗi tìm kiếm", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Tìm xe theo biển số
                var vehicle = _vehicleService.GetVehicleByPlateNumber(plateNumber);
                
                if (vehicle != null)
                {
                    // Hiển thị thông tin xe
                    VehicleDetailsGrid.Visibility = Visibility.Visible;
                    DetailPlateNumber.Text = vehicle.PlateNumber;
                    DetailBrand.Text = vehicle.Brand;
                    DetailModel.Text = vehicle.Model;
                    DetailOwner.Text = vehicle.Owner.FullName;
                    
                    // Lấy lịch sử kiểm định của xe
                    var history = _inspectionService.GetInspectionHistory(vehicle.VehicleId);
                    PoliceInspectionHistoryGrid.ItemsSource = history;
                    
                    // Kiểm tra trạng thái kiểm định
                    bool isValid = _inspectionService.IsInspectionValid(vehicle.VehicleId);
                    string status = isValid ? "Hợp lệ" : "Hết hạn";
                    
                    // Có thể thêm hiển thị trạng thái ở đây
                }
                else
                {
                    MessageBox.Show("Không tìm thấy phương tiện với biển số này!", "Không tìm thấy", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    VehicleDetailsGrid.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                VehicleDetailsGrid.Visibility = Visibility.Collapsed;
            }
        }
        
        // Phương thức để xử lý phương tiện vi phạm
        private void ReportViolation_Click(object sender, RoutedEventArgs e)
        {
            //if (DetailPlateNumber.Text != "")
            //{
            //    var violationDialog = new ViolationReportDialog(DetailPlateNumber.Text);
            //    violationDialog.Owner = Window.GetWindow(this);
                
            //    if (violationDialog.ShowDialog() == true)
            //    {
            //        MessageBox.Show("Vi phạm đã được ghi nhận thành công!", "Thông báo", 
            //            MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //}
        }
    }
}