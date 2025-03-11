using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MotorcycleEmissionUI.Police
{
    public partial class ExpiredInspectionsView : UserControl
    {
        private readonly IInspectionService _inspectionService;
        
        public ExpiredInspectionsView()
        {
            InitializeComponent();
            
            // Get services
            _inspectionService = new InspectionService();
            
            // Load expired vehicles
            LoadExpiredVehicles();
        }
        
        private void LoadExpiredVehicles()
        {
            try
            {
                // Get list of expired vehicles
                var expiredVehicles = _inspectionService.GetExpiredVehicles();
                
                // Display in DataGrid
                ExpiredVehiclesGrid.ItemsSource = expiredVehicles;
                
                // Update statistics
                TotalExpiredText.Text = expiredVehicles.Count.ToString();
                SeriouslyExpiredText.Text = expiredVehicles
                    .Count(v => v.DaysOverdue >= 30)
                    .ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách xe quá hạn: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ReportViolation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string plateNumber)
            {
                // In a real application, this would open a dialog to report the violation
                MessageBox.Show($"Đã ghi nhận xe biển số {plateNumber} vi phạm quy định kiểm định.", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Commented code shows how you might implement a dialog
                /*
                var violationDialog = new ViolationReportDialog(plateNumber);
                violationDialog.Owner = Window.GetWindow(this);
                
                if (violationDialog.ShowDialog() == true)
                {
                    MessageBox.Show("Vi phạm đã được ghi nhận thành công!", "Thông báo", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                */
            }
        }
    }
}