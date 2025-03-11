using System;
using System.Windows;

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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

		private void SendButton_Click(object sender, RoutedEventArgs e)
		{
			ReportPeriod = ReportPeriodComboBox.Text;
			Notes = NotesTextBox.Text;

			DialogResult = true;
			Close();
		}

	}
}