using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MotorcycleEmissionUI.Dialogs
{
    /// <summary>
    /// Interaction logic for ScheduleInspectionDialog.xaml
    /// </summary>
    public partial class ScheduleInspectionDialog : Window
    {
        public InspectionRecord InspectionRecord { get; private set; }
        
        private readonly IStationService _stationService;
        private readonly IUserService _userService;
        private readonly Vehicle _vehicle;

        public ScheduleInspectionDialog(Vehicle vehicle, IStationService stationService, IUserService userService)
        {
            InitializeComponent();
            
            _vehicle = vehicle;
            _stationService = stationService;
            _userService = userService;
            
            // Set vehicle info
            VehicleInfoTextBlock.Text = $"{_vehicle.PlateNumber} - {_vehicle.Brand} {_vehicle.Model} ({_vehicle.ManufactureYear})";
            
            // Set default date to tomorrow
            InspectionDatePicker.SelectedDate = DateTime.Now.AddDays(1);
            
            // Load stations and inspectors
            LoadStationsAndInspectors();
        }

        private void LoadStationsAndInspectors()
        {
            try
            {
                // Load stations
                var stations = _stationService.GetAllStations();
                StationComboBox.ItemsSource = stations;
                
                if (stations.Any())
                    StationComboBox.SelectedIndex = 0;
                
                // Load inspectors (users with Inspector role)
                var inspectors = _userService.GetUsersByRole("Inspector");
                InspectorComboBox.ItemsSource = inspectors;
                
                if (inspectors.Any())
                    InspectorComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (StationComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select an inspection station.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (InspectorComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select an inspector.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!InspectionDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a date for the inspection.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validate date is in the future
            if (InspectionDatePicker.SelectedDate.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Please select a future date for the inspection.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create inspection record
            InspectionRecord = new InspectionRecord
            {
                VehicleId = _vehicle.VehicleId,
                StationId = (int)StationComboBox.SelectedValue,
                InspectorId = (int)InspectorComboBox.SelectedValue,
                InspectionDate = InspectionDatePicker.SelectedDate.Value,
                Result = "Pending",
                Co2emission = 0, // Will be filled during inspection
                Hcemission = 0,  // Will be filled during inspection
                Comments = ""    // Will be filled during inspection
            };

            DialogResult = true;
            Close();
        }
    }
} 