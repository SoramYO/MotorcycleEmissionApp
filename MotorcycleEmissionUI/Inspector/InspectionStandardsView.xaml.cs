using System.Windows.Controls;

namespace MotorcycleEmissionUI.Inspector
{
	/// <summary>
	/// Interaction logic for InspectionStandardsView.xaml
	/// </summary>
	public partial class InspectionStandardsView : UserControl
	{
		public InspectionStandardsView()
		{
			InitializeComponent();
		}
	}
}

// Move the class outside of InspectionStandardsView class
namespace MotorcycleEmissionUI.Inspector
{
	// Simple class for binding to DataGrid
	public class EmissionStandard
	{
		public string VehicleType { get; set; }
		public string ProductionYear { get; set; }
		public string CO { get; set; }
		public string HC { get; set; }
		public string Notes { get; set; }
	}
}