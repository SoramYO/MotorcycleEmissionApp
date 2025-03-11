using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace MotorcycleEmissionUI.VehicleOwner
{
    /// <summary>
    /// Interaction logic for InspectionHistoryView.xaml
    /// </summary>
    public partial class InspectionHistoryView : UserControl, INotifyPropertyChanged
    {
        private Vehicle _vehicle;
        private ObservableCollection<InspectionRecord> _inspectionRecords;
        private readonly InspectionService _inspectionService = new InspectionService();

		public event PropertyChangedEventHandler PropertyChanged;

        public Vehicle Vehicle 
        { 
            get => _vehicle;
            set
            {
                _vehicle = value;
                OnPropertyChanged(nameof(Vehicle));
                LoadInspectionRecords();
            }
        }

        public ObservableCollection<InspectionRecord> InspectionRecords
        {
            get => _inspectionRecords;
            set
            {
                _inspectionRecords = value;
                OnPropertyChanged(nameof(InspectionRecords));
            }
        }

        public InspectionHistoryView()
        {
            InitializeComponent();
            this.DataContext = this;
            InspectionRecords = new ObservableCollection<InspectionRecord>();
        }

        private void LoadInspectionRecords()
        {
            if (Vehicle != null && Vehicle.VehicleId > 0)
            {
                try
                {
                    // Get inspection records from database
                    // This would typically call a service from BLL
                    var records = GetInspectionRecordsForVehicle(Vehicle.VehicleId);
                    InspectionRecords = new ObservableCollection<InspectionRecord>(records);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading inspection records: {ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private List<InspectionRecord> GetInspectionRecordsForVehicle(int vehicleId)
        {
         var record =   _inspectionService.GetInspectionHistory(vehicleId);
            return record;

		}

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}