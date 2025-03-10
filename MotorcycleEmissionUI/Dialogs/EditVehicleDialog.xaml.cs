using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MotorcycleEmissionDAL.Models;

namespace MotorcycleEmissionUI.Dialogs
{
	/// <summary>
	/// Interaction logic for EditVehicleDialog.xaml
	/// </summary>
	public partial class EditVehicleDialog : Window
	{
		public Vehicle Vehicle { get; set; }
		private readonly int _ownerID;
		private readonly int _vehicleId;
		public EditVehicleDialog(int ownerID, int vehicleId)
		{
			InitializeComponent();
			_ownerID = ownerID;
			_vehicleId = vehicleId;
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(PlateNumberTextBox.Text) ||
	string.IsNullOrWhiteSpace(BrandTextBox.Text) ||
	string.IsNullOrWhiteSpace(ModelTextBox.Text) ||
	string.IsNullOrWhiteSpace(YearTextBox.Text) ||
	string.IsNullOrWhiteSpace(EngineNumberTextBox.Text))
			{
				MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (!int.TryParse(YearTextBox.Text, out int year) || year < 1900 || year > DateTime.Now.Year)
			{
				MessageBox.Show($"Please enter a valid manufacturing year between 1900 and {DateTime.Now.Year}.",
					"Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			Vehicle = new Vehicle
			{
				VehicleId = _vehicleId,
				OwnerId = _ownerID,
				PlateNumber = PlateNumberTextBox.Text.Trim(),
				Brand = BrandTextBox.Text.Trim(),
				Model = ModelTextBox.Text.Trim(),
				ManufactureYear = year,
				EngineNumber = EngineNumberTextBox.Text.Trim()
			};


		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
