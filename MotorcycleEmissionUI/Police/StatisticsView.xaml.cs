using MotorcycleEmissionBLL.Services;
using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MotorcycleEmissionUI.Police
{
    /// <summary>
    /// Interaction logic for StatisticsView.xaml
    /// </summary>
    public partial class StatisticsView : UserControl
    {
        private readonly IInspectionService _inspectionService;
        private readonly IVehicleService _vehicleService;
        private User _user;

        public StatisticsView()
        {
            InitializeComponent();

            // Initialize services
            _inspectionService = new InspectionService();
            _vehicleService = new VehicleService();
            _user = Application.Current.Properties["User"] as User;

            // Set default date range
            StartDatePicker.SelectedDate = DateTime.Now.AddMonths(-1);
            EndDatePicker.SelectedDate = DateTime.Now;

            // Set default region
            if (RegionComboBox.Items.Count > 0)
                RegionComboBox.SelectedIndex = 0;

            // Load initial data
            GenerateStatsButton_Click(null, null);
        }

        private void GenerateStatsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.Now.AddMonths(-1);
                DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.Now;

                if (endDate < startDate)
                {
                    MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get inspection statistics
                var inspectionStats = _inspectionService.GetInspectionStatsByDate(startDate, endDate);
                var resultStats = _inspectionService.GetInspectionStatsByResult();

                // Update summary numbers
                int total = resultStats.Sum(kv => kv.Value);
                int passed = resultStats.ContainsKey("Pass") ? resultStats["Pass"] : 0;
                int failed = resultStats.ContainsKey("Fail") ? resultStats["Fail"] : 0;

                TotalInspectionsText.Text = total.ToString();
                PassedInspectionsText.Text = passed.ToString();
                FailedInspectionsText.Text = failed.ToString();

                // Draw charts
                DrawResultsChart(ResultsChart, resultStats);
                DrawDailyChart(DailyChart, inspectionStats);

                // Load violations data
                LoadViolationsData(startDate, endDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo thống kê: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateRegionStatsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string region = ((ComboBoxItem)RegionComboBox.SelectedItem).Content.ToString();

                if (string.IsNullOrEmpty(region))
                {
                    MessageBox.Show("Vui lòng chọn khu vực!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get regional statistics
                var regionStats = _inspectionService.GetRegionStatistics(region);

                // Draw charts
                var complianceData = new Dictionary<string, int>();
                foreach (var item in regionStats.ComplianceRate)
                {
                    complianceData.Add(item.Key, (int)(item.Value * 100));
                }
                DrawComplianceChart(ComplianceChart, complianceData);

                // Draw emission trend chart
				DrawEmissionTrendChart(EmissionTrendChart, regionStats.EmissionTrend.ToDictionary(kv => kv.Key, kv => (double)kv.Value));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo thống kê khu vực: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Show export dialog
                var sendReportDialog = new Dialogs.SendReportDialog();
                sendReportDialog.Owner = Window.GetWindow(this);

                if (sendReportDialog.ShowDialog() == true)
                {
                    string reportPeriod = sendReportDialog.ReportPeriod;
                    string targetAuthority = sendReportDialog.TargetAuthority;
                    string notes = sendReportDialog.Notes;

                    // In a real application, you would generate a PDF or Excel file here
                    MessageBox.Show("Đã xuất báo cáo thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadViolationsData(DateTime startDate, DateTime endDate)
        {
            // In a real application, this would load violation data from the database
            // For now, we'll create some sample data
            var violations = new List<ViolationData>
            {
                new ViolationData
                {
                    PlateNumber = "29A-12345",
                    ViolationType = "Không có giấy chứng nhận kiểm định",
                    ViolationDate = DateTime.Now.AddDays(-5),
                    FineAmount = 500000,
                    Notes = "Xe đã quá hạn kiểm định 3 tháng"
                },
                new ViolationData
                {
                    PlateNumber = "30H-56789",
                    ViolationType = "Vượt tiêu chuẩn khí thải",
                    ViolationDate = DateTime.Now.AddDays(-12),
                    FineAmount = 750000,
                    Notes = "CO2 vượt 25% tiêu chuẩn cho phép"
                },
                new ViolationData
                {
                    PlateNumber = "33K-98765",
                    ViolationType = "Làm giả giấy chứng nhận kiểm định",
                    ViolationDate = DateTime.Now.AddDays(-8),
                    FineAmount = 1500000,
                    Notes = "Giấy tờ làm giả phát hiện qua kiểm tra mã QR"
                }
            };

            ViolationsDataGrid.ItemsSource = violations;
        }

        #region Chart Drawing Methods

        private void DrawResultsChart(Canvas canvas, Dictionary<string, int> data)
        {
            canvas.Children.Clear();

            if (data == null || data.Count == 0)
                return;

            // Prepare data
            var total = data.Values.Sum();
            double startAngle = 0;

            // Define colors
            var colors = new List<SolidColorBrush>
            {
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.Red),
                new SolidColorBrush(Colors.Gray)
            };

            // Draw pie chart
            double centerX = canvas.ActualWidth / 2;
            double centerY = canvas.ActualHeight / 2;
            double radius = Math.Min(centerX, centerY) * 0.8;

            // Ensure we have enough space
            if (double.IsNaN(centerX) || double.IsNaN(centerY) || centerX <= 0 || centerY <= 0)
            {
                centerX = 150;
                centerY = 150;
                radius = 120;
            }

            int colorIndex = 0;

            // Draw slices
            foreach (var kv in data)
            {
                var percentage = (double)kv.Value / total;
                var endAngle = startAngle + percentage * 360;

                // Create pie slice
                var slice = CreatePieSlice(centerX, centerY, radius, startAngle, endAngle, colors[colorIndex % colors.Count]);
                canvas.Children.Add(slice);

                // Add label
                var labelAngle = startAngle + (endAngle - startAngle) / 2;
                var labelX = centerX + (radius * 0.7) * Math.Cos(labelAngle * Math.PI / 180);
                var labelY = centerY + (radius * 0.7) * Math.Sin(labelAngle * Math.PI / 180);

                var label = new TextBlock
                {
                    Text = $"{kv.Key}: {percentage:P0}",
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold
                };

                Canvas.SetLeft(label, labelX - label.ActualWidth / 2);
                Canvas.SetTop(label, labelY - label.ActualHeight / 2);
                canvas.Children.Add(label);

                startAngle = endAngle;
                colorIndex++;
            }

            // Add legend
            double legendY = 10;
            double legendX = 10;

            colorIndex = 0;
            foreach (var kv in data)
            {
                // Draw legend rect
                var legendRect = new Rectangle
                {
                    Width = 15,
                    Height = 15,
                    Fill = colors[colorIndex % colors.Count]
                };

                Canvas.SetLeft(legendRect, legendX);
                Canvas.SetTop(legendRect, legendY);
                canvas.Children.Add(legendRect);

                // Draw legend text
                var legendText = new TextBlock
                {
                    Text = $"{kv.Key}: {kv.Value}",
                    Foreground = Brushes.Black
                };

                Canvas.SetLeft(legendText, legendX + 20);
                Canvas.SetTop(legendText, legendY);
                canvas.Children.Add(legendText);

                legendY += 25;
                colorIndex++;
            }
        }

        private Path CreatePieSlice(double centerX, double centerY, double radius, double startAngle, double endAngle, Brush fill)
        {
            var path = new Path { Fill = fill };

            var figure = new PathFigure
            {
                StartPoint = new Point(centerX, centerY),
                IsClosed = true
            };

            // Start line to arc start point
            var startRad = startAngle * Math.PI / 180;
            figure.Segments.Add(
                new LineSegment(
                    new Point(
                        centerX + radius * Math.Cos(startRad),
                        centerY + radius * Math.Sin(startRad)
                    ),
                    true
                )
            );

            // Arc segment
            figure.Segments.Add(
                new ArcSegment(
                    new Point(
                        centerX + radius * Math.Cos(endAngle * Math.PI / 180),
                        centerY + radius * Math.Sin(endAngle * Math.PI / 180)
                    ),
                    new Size(radius, radius),
                    0,
                    endAngle - startAngle > 180,
                    SweepDirection.Clockwise,
                    true
                )
            );

            // End line back to center
            figure.Segments.Add(new LineSegment(new Point(centerX, centerY), true));

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            path.Data = geometry;

            return path;
        }

        private void DrawDailyChart(Canvas canvas, Dictionary<DateTime, int> data)
        {
            canvas.Children.Clear();

            if (data == null || !data.Any())
                return;

            // Chart dimensions
            double padding = 30;
            double width = canvas.ActualWidth - 2 * padding;
            double height = canvas.ActualHeight - 2 * padding;

            if (double.IsNaN(width) || double.IsNaN(height) || width <= 0 || height <= 0)
            {
                width = 300;
                height = 200;
            }

            // Determine scale
            var maxValue = data.Values.Max();
            var dates = data.Keys.OrderBy(d => d).ToList();

            // Create axes
            var xAxis = new Line
            {
                X1 = padding,
                Y1 = padding + height,
                X2 = padding + width,
                Y2 = padding + height,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            var yAxis = new Line
            {
                X1 = padding,
                Y1 = padding,
                X2 = padding,
                Y2 = padding + height,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            canvas.Children.Add(xAxis);
            canvas.Children.Add(yAxis);

            // Draw bars
            int barCount = data.Count;
            double barWidth = width / barCount;

            int index = 0;
            foreach (var date in dates)
            {
                var value = data[date];
                double barHeight = (value / (double)maxValue) * height;

                var bar = new Rectangle
                {
                    Width = barWidth * 0.8,
                    Height = barHeight,
                    Fill = new SolidColorBrush(Colors.SteelBlue)
                };

                Canvas.SetLeft(bar, padding + index * barWidth);
                Canvas.SetTop(bar, padding + height - barHeight);
                canvas.Children.Add(bar);

                // Add date label
                var dateLabel = new TextBlock
                {
                    Text = date.ToString("dd/MM"),
                    FontSize = 10,
                    RenderTransform = new RotateTransform(-45)
                };

                Canvas.SetLeft(dateLabel, padding + index * barWidth);
                Canvas.SetTop(dateLabel, padding + height + 5);
                canvas.Children.Add(dateLabel);

                // Add value label
                var valueLabel = new TextBlock
                {
                    Text = value.ToString(),
                    FontSize = 10
                };

                Canvas.SetLeft(valueLabel, padding + index * barWidth + barWidth * 0.4 - valueLabel.ActualWidth / 2);
                Canvas.SetTop(valueLabel, padding + height - barHeight - 15);
                canvas.Children.Add(valueLabel);

                index++;
            }

            // Y-axis scale
            for (int i = 0; i <= 5; i++)
            {
                double y = padding + height - i * height / 5;
                var label = new TextBlock
                {
                    Text = (maxValue * i / 5).ToString(),
                    FontSize = 10
                };

                Canvas.SetLeft(label, padding - 25);
                Canvas.SetTop(label, y - 7);

                var tick = new Line
                {
                    X1 = padding - 3,
                    Y1 = y,
                    X2 = padding,
                    Y2 = y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                canvas.Children.Add(label);
                canvas.Children.Add(tick);
            }
        }

        private void DrawComplianceChart(Canvas canvas, Dictionary<string, int> data)
        {
            canvas.Children.Clear();

            if (data == null || !data.Any())
                return;

            // Chart dimensions
            double padding = 40;
            double width = canvas.ActualWidth - 2 * padding;
            double height = canvas.ActualHeight - 2 * padding;

            if (double.IsNaN(width) || double.IsNaN(height) || width <= 0 || height <= 0)
            {
                width = 300;
                height = 200;
            }

            // Create axes
            var xAxis = new Line
            {
                X1 = padding,
                Y1 = padding + height,
                X2 = padding + width,
                Y2 = padding + height,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            var yAxis = new Line
            {
                X1 = padding,
                Y1 = padding,
                X2 = padding,
                Y2 = padding + height,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            canvas.Children.Add(xAxis);
            canvas.Children.Add(yAxis);

            // Draw horizontal bars
            int barCount = data.Count;
            double barHeight = height / barCount;

            int index = 0;
            foreach (var kv in data)
            {
                double barWidth = (kv.Value / 100.0) * width;

                var bar = new Rectangle
                {
                    Width = barWidth,
                    Height = barHeight * 0.7,
                    Fill = new SolidColorBrush(Colors.Green)
                };

                Canvas.SetLeft(bar, padding);
                Canvas.SetTop(bar, padding + index * barHeight);
                canvas.Children.Add(bar);

                // Add province label
                var provinceLabel = new TextBlock
                {
                    Text = kv.Key,
                    FontSize = 10
                };

                Canvas.SetLeft(provinceLabel, padding - provinceLabel.ActualWidth - 5);
                Canvas.SetTop(provinceLabel, padding + index * barHeight + barHeight * 0.35 - provinceLabel.ActualHeight / 2);
                canvas.Children.Add(provinceLabel);

                // Add percentage label
                var percentLabel = new TextBlock
                {
                    Text = kv.Value + "%",
                    FontSize = 10,
                    Foreground = Brushes.White
                };

                Canvas.SetLeft(percentLabel, padding + barWidth - percentLabel.ActualWidth - 5);
                Canvas.SetTop(percentLabel, padding + index * barHeight + barHeight * 0.35 - percentLabel.ActualHeight / 2);
                canvas.Children.Add(percentLabel);

                index++;
            }

            // X-axis scale
            for (int i = 0; i <= 5; i++)
            {
                double x = padding + (i * width / 5);
                var label = new TextBlock
                {
                    Text = (i * 20).ToString() + "%",
                    FontSize = 10
                };

                Canvas.SetLeft(label, x - label.ActualWidth / 2);
                Canvas.SetTop(label, padding + height + 5);

                var tick = new Line
                {
                    X1 = x,
                    Y1 = padding + height,
                    X2 = x,
                    Y2 = padding + height + 3,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                canvas.Children.Add(label);
                canvas.Children.Add(tick);
            }
        }
        private void DrawEmissionTrendChart(Canvas canvas, Dictionary<string, double> data)
        {
            canvas.Children.Clear();

            if (data == null || !data.Any())
                return;

            // Chart dimensions
            double padding = 30;
            double width = canvas.ActualWidth - 2 * padding;
            double height = canvas.ActualHeight - 2 * padding;

            if (double.IsNaN(width) || double.IsNaN(height) || width <= 0 || height <= 0)
            {
                width = 300;
                height = 200;
            }

            // Determine scale
            var maxValue = data.Values.Max();
            var yearLabels = data.Keys.OrderBy(y => y).ToList();

            // Create axes
            var xAxis = new Line
            {
                X1 = padding,
                Y1 = padding + height,
                X2 = padding + width,
                Y2 = padding + height,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            var yAxis = new Line
            {
                X1 = padding,
                Y1 = padding,
                X2 = padding,
                Y2 = padding + height,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            canvas.Children.Add(xAxis);
            canvas.Children.Add(yAxis);

            // Draw line chart
            var linePoints = new PointCollection();
            double pointSpacing = width / (yearLabels.Count - 1);

            for (int i = 0; i < yearLabels.Count; i++)
            {
                var year = yearLabels[i];
                var value = data[year];
                double x = padding + i * pointSpacing;
                double y = padding + height - (value / maxValue * height);

                linePoints.Add(new Point(x, y));

                // Draw point
                var ellipse = new Ellipse
                {
                    Width = 6,
                    Height = 6,
                    Fill = Brushes.Red
                };

                Canvas.SetLeft(ellipse, x - 3);
                Canvas.SetTop(ellipse, y - 3);
                canvas.Children.Add(ellipse);

                // Add value label
                var valueLabel = new TextBlock
                {
                    Text = value.ToString("F1"),
                    FontSize = 10
                };

                Canvas.SetLeft(valueLabel, x - valueLabel.ActualWidth / 2);
                Canvas.SetTop(valueLabel, y - 20);
                canvas.Children.Add(valueLabel);

                // Add year label
                var yearLabel = new TextBlock
                {
                    Text = year,
                    FontSize = 10
                };

                Canvas.SetLeft(yearLabel, x - yearLabel.ActualWidth / 2);
                Canvas.SetTop(yearLabel, padding + height + 5);
                canvas.Children.Add(yearLabel);
            }

            // Draw trend line
            var polyline = new Polyline
            {
                Points = linePoints,
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };

            canvas.Children.Add(polyline);

            // Y-axis scale
            for (int i = 0; i <= 5; i++)
            {
                double y = padding + height - i * height / 5;
                var label = new TextBlock
                {
                    Text = (maxValue * i / 5).ToString("F1"),
                    FontSize = 10
                };

                Canvas.SetLeft(label, padding - 25);
                Canvas.SetTop(label, y - 7);

                var tick = new Line
                {
                    X1 = padding - 3,
                    Y1 = y,
                    X2 = padding,
                    Y2 = y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                canvas.Children.Add(label);
                canvas.Children.Add(tick);
            }

            // Add title label
            var titleLabel = new TextBlock
            {
                Text = "g/km CO²",
                FontSize = 12,
                FontWeight = FontWeights.Bold
            };

            Canvas.SetLeft(titleLabel, padding - 35);
            Canvas.SetTop(titleLabel, padding - 25);
            canvas.Children.Add(titleLabel);
        }
        #endregion
    }
	// Add this class after the StatisticsView class
	public class ViolationData
	{
		public string PlateNumber { get; set; }
		public string ViolationType { get; set; }
		public DateTime ViolationDate { get; set; }
		public decimal FineAmount { get; set; }
		public string Notes { get; set; }
	}

	// Add this class for region statistics
	public class RegionStatistics
	{
		public Dictionary<string, double> ComplianceRate { get; set; }
		public Dictionary<string, double> EmissionTrend { get; set; }

		public RegionStatistics()
		{
			ComplianceRate = new Dictionary<string, double>();
			EmissionTrend = new Dictionary<string, double>();
		}
	}
}
