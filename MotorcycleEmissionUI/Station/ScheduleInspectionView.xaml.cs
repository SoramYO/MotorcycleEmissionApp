using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MotorcycleEmissionUI.Station
{
	public partial class ScheduleInspectionView : UserControl
	{
		private readonly IInspectionService _inspectionService;
		private readonly IVehicleService _vehicleService;
		private readonly IUserService _userService;
		private readonly IStationService _stationService;
		private User _user;

		public ScheduleInspectionView()
		{
			InitializeComponent();

			// Initialize services
			_inspectionService = new InspectionService();
			_vehicleService = new VehicleService();
			_userService = new UserService();
			_stationService = new StationService();

			// Get current user info
			_user = Application.Current.Properties["User"] as User;

			// Set today's date as default
			ScheduleDatePicker.SelectedDate = DateTime.Today;
			InspectionDatePicker.SelectedDate = DateTime.Today.AddDays(1);
			InspectionDatePicker.DisplayDateStart = DateTime.Today.AddDays(1);

			// Load inspectors
			LoadInspectors();



			// Load station info
			LoadStation();
		}

		private void LoadStation()
		{
			try
			{
				var station = _stationService.GetStations();
				StationComboBox.ItemsSource = station;
				if (station.Any())
				{
					StationComboBox.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi tải thông tin trạm: {ex.Message}", "Lỗi",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void LoadInspectors()
		{
			try
			{
				// Get all inspectors from the user service
				var inspectors = _userService.GetUsersByRole("Inspector");
				InspectorComboBox.ItemsSource = inspectors;

				if (inspectors.Any())
				{
					InspectorComboBox.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi tải danh sách nhân viên kiểm định: {ex.Message}", "Lỗi",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private void SearchButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string plateNumber = SearchPlateTextBox.Text.Trim();

				if (string.IsNullOrWhiteSpace(plateNumber))
				{
					MessageBox.Show("Vui lòng nhập biển số xe để tìm kiếm.", "Thông báo",
						MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}

				// Search for vehicle
				var vehicle = _vehicleService.GetVehicleByPlateNumber(plateNumber);

				if (vehicle != null)
				{
					// Populate vehicle info
					PlateNumberTextBox.Text = vehicle.PlateNumber;

					// Get full vehicle details including owner
					var vehicleWithOwner = _vehicleService.GetVehicleById(vehicle.VehicleId);
					if (vehicleWithOwner != null && vehicleWithOwner.Owner != null)
					{
						OwnerNameTextBox.Text = vehicleWithOwner.Owner.FullName;
					}
					else
					{
						OwnerNameTextBox.Text = "Không có thông tin chủ xe";
					}
				}
				else
				{
					MessageBox.Show("Không tìm thấy phương tiện với biển số này.", "Không tìm thấy",
						MessageBoxButton.OK, MessageBoxImage.Information);

					// Clear fields
					PlateNumberTextBox.Text = string.Empty;
					OwnerNameTextBox.Text = string.Empty;
				}

				// Also refresh the schedule list for the selected date
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ClearButton_Click(object sender, RoutedEventArgs e)
		{
			// Clear all input fields
			SearchPlateTextBox.Text = string.Empty;
			PlateNumberTextBox.Text = string.Empty;
			OwnerNameTextBox.Text = string.Empty;
			NotesTextBox.Text = string.Empty;

			// Reset date picker and comboboxes to defaults
			InspectionDatePicker.SelectedDate = DateTime.Today.AddDays(1);
			if (InspectorComboBox.Items.Count > 0)
				InspectorComboBox.SelectedIndex = 0;
		}

		private void ScheduleButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Validate input
				if (string.IsNullOrWhiteSpace(PlateNumberTextBox.Text))
				{
					MessageBox.Show("Vui lòng nhập biển số xe.", "Thiếu thông tin",
						MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				if (InspectionDatePicker.SelectedDate == null)
				{
					MessageBox.Show("Vui lòng chọn ngày kiểm định.", "Thiếu thông tin",
						MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}


				if (InspectorComboBox.SelectedItem == null)
				{
					MessageBox.Show("Vui lòng chọn nhân viên kiểm định.", "Thiếu thông tin",
						MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				// Get the vehicle
				var vehicle = _vehicleService.GetVehicleByPlateNumber(PlateNumberTextBox.Text);
				if (vehicle == null)
				{
					MessageBox.Show("Không tìm thấy thông tin xe. Vui lòng kiểm tra lại biển số.", "Lỗi",
						MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				// Extract time from selected time slot
				TimeSpan time = DateTime.Now.TimeOfDay; // Default



				// Create inspection record with date and time combined
				var dateTime = InspectionDatePicker.SelectedDate.Value.Date.Add(time);
				var stationId = StationComboBox.SelectedValue != null ?
	(int)StationComboBox.SelectedValue : 0;

				// Validate station exists before using
				if (stationId <= 0)
				{
					MessageBox.Show("Please select a valid station", "Error",
						MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				var inspectionRecord = new InspectionRecord
				{
					VehicleId = vehicle.VehicleId,
					StationId = stationId,
					InspectorId = (InspectorComboBox.SelectedItem as User)?.UserId ?? 0,
					InspectionDate = dateTime,
					Result = "Pending",
					Comments = NotesTextBox.Text,
					// Default emission values to 0
					Co2emission = 0,
					Hcemission = 0
				};

				// Schedule the inspection
				_inspectionService.ScheduleInspection(inspectionRecord);

				MessageBox.Show("Đã lưu lịch kiểm định thành công!", "Thành công",
					MessageBoxButton.OK, MessageBoxImage.Information);

				// Clear form and reload schedules
				ClearButton_Click(null, null);
			}
			catch (Exception ex)
			{
				// Show both the main exception and inner exception details
				var innerMessage = ex.InnerException != null ? $"\n\nChi tiết: {ex.InnerException.Message}" : "";
				MessageBox.Show($"Lỗi khi đặt lịch kiểm định: {ex.Message}{innerMessage}", "Lỗi",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CancelInspection_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (sender is Button button && button.Tag is int recordId)
				{
					// Confirm deletion
					var result = MessageBox.Show("Bạn có chắc muốn hủy lịch kiểm định này?", "Xác nhận",
						MessageBoxButton.YesNo, MessageBoxImage.Question);

					if (result == MessageBoxResult.Yes)
					{
						// Delete the inspection record
						_inspectionService.DeleteInspection(recordId);

						MessageBox.Show("Đã hủy lịch kiểm định thành công!", "Thành công",
							MessageBoxButton.OK, MessageBoxImage.Information);

					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi hủy lịch kiểm định: {ex.Message}", "Lỗi",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void EditInspection_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (sender is Button button && button.Tag is int recordId)
				{
					// Get the inspection record
					var inspection = _inspectionService.GetInspectionById(recordId);
					if (inspection == null)
					{
						MessageBox.Show("Không tìm thấy thông tin lịch kiểm định.", "Lỗi",
							MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}

					// Populate form with inspection data
					var vehicle = _vehicleService.GetVehicleById(inspection.VehicleId);
					if (vehicle != null)
					{
						SearchPlateTextBox.Text = vehicle.PlateNumber;
						PlateNumberTextBox.Text = vehicle.PlateNumber;
						OwnerNameTextBox.Text = vehicle.Owner.FullName;
					}

					InspectionDatePicker.SelectedDate = inspection.InspectionDate;



					// Set inspector
					foreach (User inspector in InspectorComboBox.Items)
					{
						if (inspector.UserId == inspection.InspectorId)
						{
							InspectorComboBox.SelectedItem = inspector;
							break;
						}
					}

					NotesTextBox.Text = inspection.Comments;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi chỉnh sửa lịch kiểm định: {ex.Message}", "Lỗi",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}