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

namespace MotorcycleEmissionUI.Dialogs
{
	public partial class SendReportDialog : Window
	{
		public string ReportPeriod { get; private set; }
		public string TargetAuthority { get; private set; }
		public string Notes { get; private set; }
		public SendReportDialog()
		{
			InitializeComponent();
		}
		private void SendButton_Click(object sender, RoutedEventArgs e)
		{
			// Lấy thông tin từ form
			ReportPeriod = ((ComboBoxItem)ReportPeriodComboBox.SelectedItem).Content.ToString();
			TargetAuthority = ((ComboBoxItem)AuthorityComboBox.SelectedItem).Content.ToString();
			Notes = NotesTextBox.Text;
			this.DialogResult = true;
			this.Close();
		}
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}
	}
}