using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Windows;

namespace MotorcycleEmissionUI.Dialogs
{
    public partial class RecordResultDialog : Window
    {
        private readonly IInspectionService _inspectionService;
        private readonly IVehicleService _vehicleService;
        private readonly int _recordId;
        private InspectionRecord _inspectionRecord;

        public RecordResultDialog(int recordId)
        {
            InitializeComponent();
            
            _recordId = recordId;
            _inspectionService = new InspectionService();
            _vehicleService = new VehicleService();
            
            // Load inspection record and vehicle details
            LoadInspectionRecord();
            
            // Default to Pass
            PassRadioButton.IsChecked = true;
        }
        
        private void LoadInspectionRecord()
        {
            try
            {
                _inspectionRecord = _inspectionService.GetInspectionById(_recordId);
                
                if (_inspectionRecord == null)
                {
                    MessageBox.Show("Không thể tải thông tin kiểm định.", "Lỗi", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    DialogResult = false;
                    Close();
                    return;
                }
                
                // Load vehicle details
                var vehicle = _vehicleService.GetVehicleById(_inspectionRecord.VehicleId);
                if (vehicle != null)
                {
                    PlateNumberText.Text = vehicle.PlateNumber;
                    BrandText.Text = vehicle.Brand;
                    ModelText.Text = vehicle.Model;
                    
                    if (vehicle.Owner != null)
                    {
                        OwnerNameText.Text = vehicle.Owner.FullName;
                    }
                }
                
                // If record already has results, populate the form
                if (_inspectionRecord.Result != "Pending")
                {
                    CO2TextBox.Text = _inspectionRecord.Co2emission.ToString();
                    HCTextBox.Text = _inspectionRecord.Hcemission.ToString();
                    CommentsTextBox.Text = _inspectionRecord.Comments;
                    
                    if (_inspectionRecord.Result == "Pass")
                    {
                        PassRadioButton.IsChecked = true;
                        FailRadioButton.IsChecked = false;
                    }
                    else
                    {
                        PassRadioButton.IsChecked = false;
                        FailRadioButton.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
                Close();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(CO2TextBox.Text) || string.IsNullOrWhiteSpace(HCTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ kết quả đo CO2 và HC.", "Thiếu thông tin", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (!decimal.TryParse(CO2TextBox.Text, out decimal co2Value))
                {
                    MessageBox.Show("Giá trị CO2 không hợp lệ.", "Lỗi nhập liệu", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                if (!decimal.TryParse(HCTextBox.Text, out decimal hcValue))
                {
                    MessageBox.Show("Giá trị HC không hợp lệ.", "Lỗi nhập liệu", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Get result
                string result = PassRadioButton.IsChecked == true ? "Pass" : "Fail";
                
                // Save results
                _inspectionService.RecordInspectionResult(
                    _recordId,
                    result,
                    co2Value,
                    hcValue,
                    CommentsTextBox.Text
                );
                
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu kết quả: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}