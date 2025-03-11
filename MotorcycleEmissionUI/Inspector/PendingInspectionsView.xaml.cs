using Microsoft.Extensions.DependencyInjection;
using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using MotorcycleEmissionUI.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MotorcycleEmissionUI.Inspector
{
    public partial class PendingInspectionsView : UserControl
    {
        private readonly IInspectionService _inspectionService;
        private User _user;

        public PendingInspectionsView()
        {
            InitializeComponent();

            // Lấy service từ DI container
            _inspectionService = new InspectionService();
            _user = Application.Current.Properties["User"] as User;

            // Tải danh sách kiểm định đang chờ
            LoadPendingInspections();
        }

        private void LoadPendingInspections()
        {
            try
            {
                int inspectorId = _user.UserId;
                var pendingRecords = _inspectionService.GetPendingInspections(inspectorId);

                // Map to a format that matches the DataGrid bindings

                PendingInspectionsDataGrid.ItemsSource = pendingRecords;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách kiểm định: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshPendingButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPendingInspections();
        }

        private void RecordResult_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int recordId)
            {
                var recordDialog = new RecordResultDialog(recordId);
                recordDialog.Owner = Window.GetWindow(this);

                if (recordDialog.ShowDialog() == true)
                {
                    MessageBox.Show("Kết quả kiểm định đã được ghi nhận!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Reload the list
                    LoadPendingInspections();
                }
            }
        }
    }
}