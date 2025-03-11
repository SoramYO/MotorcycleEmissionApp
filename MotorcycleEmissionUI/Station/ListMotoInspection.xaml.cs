using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MotorcycleEmissionUI.Station
{
    /// <summary>
    /// Interaction logic for ListMotoInspection.xaml
    /// </summary>
    public partial class ListMotoInspection : UserControl
    {
        private readonly IStationService _stationService;
        private readonly IInspectionService _inspectionService;

        public ListMotoInspection()
        {
            InitializeComponent();

            // Lấy service từ DI container
            _stationService = new StationService();
            _inspectionService = new InspectionService();

            // Tải danh sách trung tâm kiểm định
            LoadStations();
        }

        private void LoadStations()
        {
            try
            {
                var stations = _stationService.GetAllStations();
                StationComboBox.ItemsSource = stations;
                StationComboBox.DisplayMemberPath = "Name";
                StationComboBox.SelectedValuePath = "StationId";

                if (stations.Any())
                {
                    StationComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách trung tâm kiểm định: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadVehicles();
        }

        private void LoadVehicles()
        {
            try
            {
                if (StationComboBox.SelectedItem != null)
                {
                    int stationId = (int)StationComboBox.SelectedValue;
                    var vehicles = _inspectionService.GetInspectionsByStation(stationId)
                        .Select(i => new
                        {
                            i.Vehicle.PlateNumber,
                            i.Vehicle.Brand,
                            i.Vehicle.Model,
                            i.Vehicle.ManufactureYear,
                            i.Vehicle.EngineNumber,
                            i.InspectionDate
                        }).ToList();

                    VehiclesDataGrid.ItemsSource = vehicles;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách xe: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}