using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MotorcycleEmissionUI.Police
{
    /// <summary>
    /// Interaction logic for ViolationReportView.xaml
    /// </summary>
    public partial class ViolationReportView : UserControl
    {
        private readonly IVehicleService _vehicleService;
        private readonly IInspectionService _inspectionService;
        private Vehicle _currentVehicle;

        public ViolationReportView()
        {
            InitializeComponent();
            
            // Initialize services
            _vehicleService = new VehicleService();
            _inspectionService = new InspectionService();
            
            // Set default date
            ViolationDatePicker.SelectedDate = DateTime.Today;
            
            // Focus on search box
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
                
                // Search for vehicle
                _currentVehicle = _vehicleService.GetVehicleByPlateNumber(plateNumber);
                
                if (_currentVehicle != null)
                {
                    // Show vehicle information
                    VehicleInfoGroupBox.Visibility = Visibility.Visible;
                    ViolationGroupBox.Visibility = Visibility.Visible;
                    ReportViolationButton.IsEnabled = true;
                    
                    // Populate vehicle details
                    DetailPlateNumber.Text = _currentVehicle.PlateNumber;
                    DetailOwner.Text = _currentVehicle.Owner?.FullName ?? "Không có thông tin";
                    DetailVehicleType.Text = $"{_currentVehicle.Brand} {_currentVehicle.Model}";
                    
                    // Get inspection status
                    bool isValid = _inspectionService.IsInspectionValid(_currentVehicle.VehicleId);
                    DetailInspectionStatus.Text = isValid ? "Đã kiểm định (Còn hạn)" : "Chưa kiểm định hoặc hết hạn";
                    DetailInspectionStatus.Foreground = isValid ? 
                        System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy phương tiện với biển số này!", "Không tìm thấy", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                ClearForm();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ReportViolationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentVehicle == null)
                {
                    MessageBox.Show("Vui lòng tìm kiếm phương tiện trước khi báo cáo vi phạm!", 
                        "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (ViolationTypeComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn loại vi phạm!", "Thiếu thông tin", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (!decimal.TryParse(FineAmountTextBox.Text, out decimal fineAmount))
                {
                    MessageBox.Show("Vui lòng nhập mức phạt hợp lệ!", "Lỗi nhập liệu", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // In real implementation, you would save the violation report to database
                string violationType = ((ComboBoxItem)ViolationTypeComboBox.SelectedItem).Content.ToString();
                string notes = ViolationNotesTextBox.Text;
                DateTime violationDate = ViolationDatePicker.SelectedDate ?? DateTime.Today;
                
                // Show success message
                MessageBox.Show($"Đã ghi nhận vi phạm của phương tiện biển số {_currentVehicle.PlateNumber}:\n" +
                    $"- Loại vi phạm: {violationType}\n" +
                    $"- Ngày vi phạm: {violationDate:dd/MM/yyyy}\n" +
                    $"- Mức phạt: {fineAmount:N0} VNĐ", 
                    "Báo cáo vi phạm thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Clear form after successful submission
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi ghi nhận vi phạm: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            _currentVehicle = null;
            VehicleInfoGroupBox.Visibility = Visibility.Collapsed;
            ViolationGroupBox.Visibility = Visibility.Collapsed;
            ReportViolationButton.IsEnabled = false;
            SearchPlateTextBox.Clear();
            FineAmountTextBox.Clear();
            ViolationNotesTextBox.Clear();
            if (ViolationTypeComboBox.Items.Count > 0)
                ViolationTypeComboBox.SelectedIndex = 0;
            ViolationDatePicker.SelectedDate = DateTime.Today;
        }
    }
}