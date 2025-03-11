using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MotorcycleEmissionUI.Inspector
{
    public partial class CompletedInspectionsView : UserControl
    {
        private readonly IInspectionService _inspectionService;
        private User _user;

        public CompletedInspectionsView()
        {
            InitializeComponent();
            
            // Get services
            _inspectionService = new InspectionService();
            _user = Application.Current.Properties["User"] as User;
            
            // Set default date range (last month to now)
            StartDatePicker.SelectedDate = DateTime.Now.AddMonths(-1);
            EndDatePicker.SelectedDate = DateTime.Now;
            
            // Load completed inspections
            LoadCompletedInspections();
        }
        
        private void LoadCompletedInspections()
        {
            try
            {
                int inspectorId = _user.UserId;
                DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.Now.AddMonths(-1);
                DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.Now;
                
                // Get inspections by this inspector with "Pass" or "Fail" result
                var completedRecords = _inspectionService.GetInspectionsByInspector(inspectorId)
                    .Where(i => (i.Result == "Pass" || i.Result == "Fail") && 
                           i.InspectionDate >= startDate && 
                           i.InspectionDate <= endDate)
                    .OrderByDescending(i => i.InspectionDate)
                    .ToList();
                
                // Load data to DataGrid
                CompletedInspectionsDataGrid.ItemsSource = completedRecords;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách kiểm định: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCompletedInspections();
        }
    }
}