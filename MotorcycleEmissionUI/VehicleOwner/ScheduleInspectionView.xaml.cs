using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.ApplicationServices;
using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using User = MotorcycleEmissionDAL.Models.User;

namespace MotorcycleEmissionUI.VehicleOwner
{
	public partial class ScheduleInspectionView : UserControl
	{
		private readonly IVehicleService _vehicleService;
		private readonly IStationService _stationService;
		private readonly IInspectionService _inspectionService;

		private List<InspectionStation> _availableStations = new List<InspectionStation>();
		private List<TimeSlot> _availableTimeSlots = new List<TimeSlot>();
		private User _user;

		public ScheduleInspectionView()
		{
			InitializeComponent();

			// Lấy service từ DI container
			_vehicleService = new VehicleService();
			_stationService = new StationService();
			_inspectionService = new InspectionService();

			// Thiết lập ngày mặc định (ngày mai)
			InspectionDatePicker.SelectedDate = DateTime.Now.AddDays(1);
			InspectionDatePicker.DisplayDateStart = DateTime.Now.AddDays(1);
			InspectionDatePicker.DisplayDateEnd = DateTime.Now.AddMonths(1);

			// Tải dữ liệu
			LoadVehicles();
			LoadProvinces();
			_user = Application.Current.Properties["User"] as User;
		}

		private void LoadVehicles()
		{
			try
			{
				var vehicles = _vehicleService.GetVehiclesByOwner(_user.UserId);

				VehicleComboBox.ItemsSource = vehicles;

				if (vehicles.Any())
				{
					VehicleComboBox.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi tải danh sách phương tiện: {ex.Message}", "Lỗi",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void LoadProvinces()
		{
			try
			{
				var regionIndex = RegionComboBox.SelectedIndex;
				string region = regionIndex >= 0 ? ((ComboBoxItem)RegionComboBox.SelectedItem).Content.ToString() : "Miền Bắc";

				var provinces = _stationService.GetProvincesByRegion(region);
				ProvinceComboBox.ItemsSource = provinces;

				if (provinces.Any())
				{
					ProvinceComboBox.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi tải danh sách tỉnh/thành: {ex.Message}", "Lỗi",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void LoadStations()
		{
			try
			{
				if (ProvinceComboBox.SelectedItem != null)
				{
					string province = ProvinceComboBox.SelectedItem.ToString();
					_availableStations = _stationService.GetStationsByProvince(province);
					StationsListBox.ItemsSource = _availableStations;

					if (_availableStations.Any())
					{
						StationsListBox.SelectedIndex = 0;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi tải danh sách trung tâm kiểm định: {ex.Message}", "Lỗi",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void RegionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LoadProvinces();
		}

		private void ProvinceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LoadStations();
		}

		private void ClearButton_Click(object sender, RoutedEventArgs e)
		{
			// Reset các controls về giá trị mặc định
			RegionComboBox.SelectedIndex = 0;
			InspectionDatePicker.SelectedDate = DateTime.Now.AddDays(1);
			NotesTextBox.Clear();
		}

		private void ScheduleButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Kiểm tra dữ liệu nhập vào
				if (VehicleComboBox.SelectedItem == null)
				{
					MessageBox.Show("Vui lòng chọn phương tiện!", "Thiếu thông tin",
						MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				if (StationsListBox.SelectedItem == null)
				{
					MessageBox.Show("Vui lòng chọn trung tâm kiểm định!", "Thiếu thông tin",
						MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				if (TimeSlotComboBox.SelectedItem == null)
				{
					MessageBox.Show("Vui lòng chọn khung giờ!", "Thiếu thông tin",
						MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				var timeSlot = TimeSlotComboBox.SelectedItem;
				string notes = NotesTextBox.Text;

				InspectionRecord inspectionRecord = new InspectionRecord
				{
					VehicleId = ((Vehicle)VehicleComboBox.SelectedItem).VehicleId,
					StationId = ((InspectionStation)StationsListBox.SelectedItem).StationId,
					InspectorId = 1,
					InspectionDate = InspectionDatePicker.SelectedDate.Value,
				};

				// Gọi service để lưu đặt lịch
				_inspectionService.ScheduleInspection(inspectionRecord);

				MessageBox.Show("Đặt lịch kiểm định thành công! Vui lòng đến trung tâm kiểm định đúng ngày giờ đã đặt.",
							   "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

				// Reset form
				ClearButton_Click(null, null);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi hệ thống",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		// Các lớp hỗ trợ
		public class TimeSlot
		{
			public int SlotID { get; set; }
			public string DisplayTime { get; set; }

			public override string ToString()
			{
				return DisplayTime;
			}
		}
	}
}