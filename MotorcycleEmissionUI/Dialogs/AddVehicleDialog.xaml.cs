using MotorcycleEmissionDAL.Models;
using System;
using System.Windows;

namespace MotorcycleEmissionUI.Dialogs
{
    /// <summary>
    /// Interaction logic for AddVehicleDialog.xaml
    /// </summary>
    public partial class AddVehicleDialog : Window
    {
        public Vehicle Vehicle { get; private set; }
        private readonly int _ownerID;

        public AddVehicleDialog(int ownerID)
        {
            InitializeComponent();
            _ownerID = ownerID;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(PlateNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(BrandTextBox.Text) ||
                string.IsNullOrWhiteSpace(ModelTextBox.Text) ||
                string.IsNullOrWhiteSpace(YearTextBox.Text) ||
                string.IsNullOrWhiteSpace(EngineNumberTextBox.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validate year input
            if (!int.TryParse(YearTextBox.Text, out int year) || year < 1900 || year > DateTime.Now.Year)
            {
                MessageBox.Show($"Please enter a valid manufacturing year between 1900 and {DateTime.Now.Year}.", 
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create vehicle object
            Vehicle = new Vehicle
            {
                OwnerId = _ownerID,
                PlateNumber = PlateNumberTextBox.Text.Trim(),
                Brand = BrandTextBox.Text.Trim(),
                Model = ModelTextBox.Text.Trim(),
                ManufactureYear = year,
                EngineNumber = EngineNumberTextBox.Text.Trim()
            };

            DialogResult = true;
            Close();
        }
    }
} 