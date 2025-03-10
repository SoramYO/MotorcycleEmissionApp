using Microsoft.Extensions.DependencyInjection;
using MotorcycleEmissionBLL.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MotorcycleEmissionUI.Police
{
    public partial class ExpiredVehiclesView : UserControl
    {
        private readonly IInspectionService _inspectionService;
        
        public ExpiredVehiclesView()
        {
            InitializeComponent();
            
            // Lấy service từ DI container
            _inspectionService = new InspectionService();
            
            // Tải danh sách xe quá hạn
            LoadExpiredVehicles();
        }
        
        private void LoadExpiredVehicles()
        {
            try
            {
                // Lấy danh sách xe quá hạn
                var expiredVehicles = _inspectionService.GetExpiredVehicles();
                
                // Hiển thị lên DataGrid
                ExpiredVehiclesGrid.ItemsSource = expiredVehicles;
                
                // Cập nhật thống kê
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
            //if (sender is Button button && button.Tag is string plateNumber)
            //{
            //    var violationDialog = new ViolationReportDialog(plateNumber);
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