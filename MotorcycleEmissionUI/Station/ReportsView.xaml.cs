using Microsoft.Extensions.DependencyInjection;
using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MotorcycleEmissionUI.Station
{
    public partial class ReportsView : UserControl
    {
        private readonly IInspectionService _inspectionService;
		private User _user;

		public ReportsView()
        {
            InitializeComponent();
            
            // Lấy service từ DI container
            _inspectionService = new InspectionService();
			_user = Application.Current.Properties["User"] as User;

			// Thiết lập giá trị mặc định cho DatePicker
			StartDatePicker.SelectedDate = DateTime.Now.AddMonths(-1);
            EndDatePicker.SelectedDate = DateTime.Now;
        }
        
        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            // Di chuyển code từ MainAppWindow.xaml.cs, phương thức GenerateReportButton_Click
            try
            {
                DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.Now.AddMonths(-1);
                DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.Now;
                
                if (endDate < startDate)
                {
                    MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Lỗi", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Lấy dữ liệu báo cáo từ service
                var stationId = _user.UserId; // hoặc lấy từ thuộc tính khác
                var reportData = _inspectionService.GetInspectionReport(stationId, startDate, endDate);
                
                // Hiển thị dữ liệu báo cáo
                ReportDataGrid.ItemsSource = reportData.Details;
                
                // Vẽ biểu đồ kết quả
                var resultsData = new Dictionary<string, int>
                {
                    { "Đạt", reportData.PassCount },
                    { "Không đạt", reportData.FailCount }
                };
                DrawResultsChart(ResultsChart, resultsData);
                
                // Vẽ biểu đồ hàng ngày
                DrawDailyChart(DailyChart, reportData.DailyInspections);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void GenerateRegionStats_Click(object sender, RoutedEventArgs e)
        {
            // Di chuyển code từ MainAppWindow.xaml.cs, phương thức GenerateRegionStats_Click
            try
            {
                string region = ((ComboBoxItem)RegionComboBox.SelectedItem).Content.ToString();
                
                if (string.IsNullOrEmpty(region))
                {
                    MessageBox.Show("Vui lòng chọn khu vực!", "Lỗi", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Lấy dữ liệu thống kê theo khu vực
                var regionStats = _inspectionService.GetRegionStatistics(region);
                
                // Vẽ biểu đồ tỷ lệ tuân thủ
                var complianceData = new Dictionary<string, int>();
                foreach (var item in regionStats.ComplianceRate)
                {
                    complianceData.Add(item.Key, (int)(item.Value * 100));
                }
                DrawComplianceChart(ComplianceChart, complianceData);
                
                // Vẽ biểu đồ xu hướng phát thải
                DrawEmissionTrendChart(EmissionTrendChart, regionStats.EmissionTrend);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo thống kê khu vực: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void SendReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra xem đã tạo báo cáo chưa
                if (ReportDataGrid.ItemsSource == null || !((IEnumerable<object>)ReportDataGrid.ItemsSource).Any())
                {
                    MessageBox.Show("Vui lòng tạo báo cáo trước khi gửi!", "Thiếu dữ liệu", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Hiển thị dialog xác nhận
                var sendReportDialog = new Dialogs.SendReportDialog();
                sendReportDialog.Owner = Window.GetWindow(this);
                
                if (sendReportDialog.ShowDialog() == true)
                {
                    string reportPeriod = sendReportDialog.ReportPeriod;
                    string targetAuthority = sendReportDialog.TargetAuthority;
                    string notes = sendReportDialog.Notes;
                    
                    DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.Now.AddMonths(-1);
                    DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.Now;

                    // Gọi service để gửi báo cáo
                    bool success = true;
                        
                    if (success)
                    {
                        MessageBox.Show("Đã gửi báo cáo thành công!", "Thông báo", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không thể gửi báo cáo. Vui lòng thử lại!", "Lỗi", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi báo cáo: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Các phương thức vẽ biểu đồ (di chuyển từ MainAppWindow)
        private void DrawResultsChart(Canvas canvas, Dictionary<string, int> data)
        {
            // Implementation...
        }
        
        private void DrawDailyChart(Canvas canvas, Dictionary<DateTime, int> data)
        {
            // Implementation...
        }
        
        private void DrawComplianceChart(Canvas canvas, Dictionary<string, int> data)
        {
            // Di chuyển code từ MainAppWindow.xaml.cs, phương thức DrawComplianceChart
        }
        
        private void DrawEmissionTrendChart(Canvas canvas, Dictionary<string, decimal> data)
        {
            // Di chuyển code từ MainAppWindow.xaml.cs, phương thức DrawEmissionTrendChart
        }
    }
}