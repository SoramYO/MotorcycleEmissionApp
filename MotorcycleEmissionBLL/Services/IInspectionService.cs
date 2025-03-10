
using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;

namespace MotorcycleEmissionBLL.Services
{
	public interface IInspectionService
	{
		InspectionRecord GetInspectionById(int recordId);
		IEnumerable<InspectionRecord> GetAllInspections();
		IEnumerable<InspectionRecord> GetInspectionsByVehicle(int vehicleId);
		IEnumerable<InspectionRecord> GetInspectionsByStation(int stationId);
		IEnumerable<InspectionRecord> GetInspectionsByInspector(int inspectorId);
		IEnumerable<InspectionRecord> GetInspectionsByDateRange(DateTime startDate, DateTime endDate);
		IEnumerable<InspectionRecord> GetInspectionsByResult(string result);
		void ScheduleInspection(InspectionRecord inspectionRecord);
		void RecordInspectionResult(int recordId, string result, decimal co2Emission, decimal hcEmission, string comments);
		void UpdateInspection(InspectionRecord record);
		void DeleteInspection(int recordId);
		Dictionary<string, int> GetInspectionStatsByResult();
		Dictionary<DateTime, int> GetInspectionStatsByDate(DateTime startDate, DateTime endDate);
		List<InspectionRecord> GetInspectionHistory(string plateNumber);
		List<InspectionRecord> GetInspectionHistory(int vehicleId);
		List<InspectionRecord> GetPendingInspections(int inspectorId);
		List<ExpiredVehicleInfo> GetExpiredVehicles();
		bool IsInspectionValid(int vehicleId);
		InspectionReportData GetInspectionReport(int stationId, DateTime startDate, DateTime endDate);

		// Add these two methods
		RegionStatistics GetRegionStatistics(string region);
	}

	// Existing classes...
}

public class ExpiredVehicleInfo
{
	public int VehicleID { get; set; }
	public string PlateNumber { get; set; }
	public string Brand { get; set; }
	public string Model { get; set; }
	public string OwnerName { get; set; }
	public string OwnerPhone { get; set; }
	public DateTime LastInspectionDate { get; set; }
	public DateTime ExpiryDate { get; set; }
	public int DaysOverdue { get; set; }
}
public class InspectionReportData
{
	public List<InspectionDetail> Details { get; set; } = new List<InspectionDetail>();
	public int PassCount { get; set; }
	public int FailCount { get; set; }
	public Dictionary<DateTime, int> DailyInspections { get; set; } = new Dictionary<DateTime, int>();
}

public class InspectionDetail
{
	public DateTime InspectionDate { get; set; }
	public string PlateNumber { get; set; }
	public string OwnerName { get; set; }
	public decimal CO { get; set; }
	public decimal HC { get; set; }
	public string Result { get; set; }
}

public class RegionStatistics
{
	public Dictionary<string, double> ComplianceRate { get; set; } = new Dictionary<string, double>();
	public Dictionary<string, decimal> EmissionTrend { get; set; } = new Dictionary<string, decimal>();
}
public class Report
{
	public int ReportID { get; set; }
	public int StationID { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public string ReportPeriod { get; set; }
	public string TargetAuthority { get; set; }
	public string Notes { get; set; }
	public DateTime SubmissionDate { get; set; }
	public string Status { get; set; }

	// Navigation properties
	public virtual InspectionStation Station { get; set; }
}
