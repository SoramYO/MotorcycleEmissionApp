using MotorcycleEmissionDAL;
using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace MotorcycleEmissionBLL.Services
{
	public class InspectionService : IInspectionService
	{
		private readonly GenericRepository<InspectionRecord> _inspectionRecords;
		private readonly GenericRepository<Vehicle> _vehicles;
		private readonly GenericRepository<InspectionStation> _inspectionStations;


		public InspectionService()
		{
			_inspectionRecords = new GenericRepository<InspectionRecord>();
			_vehicles = new GenericRepository<Vehicle>();
			_inspectionStations = new GenericRepository<InspectionStation>();
		}

		public void DeleteInspection(int recordId)
		{
			_inspectionRecords.DeleteById(recordId);
		}

		public IEnumerable<InspectionRecord> GetAllInspections()
		{
			return _inspectionRecords.GetAll();
		}

		public InspectionRecord GetInspectionById(int recordId)
		{
			return _inspectionRecords.GetById(recordId);
		}

		public IEnumerable<InspectionRecord> GetInspectionsByDateRange(DateTime startDate, DateTime endDate)
		{
			return _inspectionRecords.GetAll().Where(r => r.InspectionDate >= startDate && r.InspectionDate <= endDate);
		}

		public IEnumerable<InspectionRecord> GetInspectionsByInspector(int inspectorId)
		{
			return _inspectionRecords.GetAll().Where(r => r.InspectorId == inspectorId);
		}

		public IEnumerable<InspectionRecord> GetInspectionsByResult(string result)
		{
			return _inspectionRecords.GetAll().Where(r => r.Result == result);
		}

		public IEnumerable<InspectionRecord> GetInspectionsByStation(int stationId)
		{
			return _inspectionRecords.GetAll().Where(r => r.StationId == stationId);
		}

		public IEnumerable<InspectionRecord> GetInspectionsByVehicle(int vehicleId)
		{
			return _inspectionRecords.GetAll().Where(r => r.VehicleId == vehicleId);
		}

		public Dictionary<DateTime, int> GetInspectionStatsByDate(DateTime startDate, DateTime endDate)
		{
			var inspections = GetInspectionsByDateRange(startDate, endDate);
			return inspections
				.GroupBy(i => i.InspectionDate.Value)
				.ToDictionary(g => g.Key, g => g.Count());
		}

		public Dictionary<string, int> GetInspectionStatsByResult()
		{
			var inspections = _inspectionRecords.GetAll();
			return inspections
				.GroupBy(i => i.Result)
				.ToDictionary(g => g.Key, g => g.Count());
		}

		public void RecordInspectionResult(int recordId, string result, decimal co2Emission, decimal hcEmission, string comments)
		{
			var record = _inspectionRecords.GetById(recordId);

			if (record == null)
				throw new Exception("Inspection record not found");

			record.Result = result;
			record.Co2emission = co2Emission;
			record.Hcemission = hcEmission;
			record.Comments = comments;

			_inspectionRecords.Update(record);
		}

		public void ScheduleInspection(InspectionRecord inspectionRecord)
		{
			_inspectionRecords.Add(inspectionRecord);
		}

		public void UpdateInspection(InspectionRecord record)
		{
			var existingRecord = _inspectionRecords.GetById(record.RecordId);

			if (existingRecord == null)
				throw new Exception("Inspection record not found");

			_inspectionRecords.Update(record);
		}

		public List<InspectionRecord> GetInspectionHistory(string plateNumber)
		{
			try
			{
				// Tìm xe dựa trên biển số
				var vehicle = _vehicles.GetAll()
					.Where(v => v.PlateNumber == plateNumber)
					.FirstOrDefault();
				if (vehicle == null)
					return new List<InspectionRecord>();

				return _inspectionRecords.GetAll()
					.Where(i => i.Vehicle.PlateNumber == plateNumber)
					.OrderByDescending(i => i.InspectionDate)
					.ToList();
			}
			catch (Exception ex)
			{
				// Ghi log lỗi
				Console.WriteLine($"Error in GetInspectionHistory by plateNumber: {ex.Message}");
				return new List<InspectionRecord>();
			}
		}

		public List<InspectionRecord> GetInspectionHistory(int vehicleId)
		{
			try
			{
				return _inspectionRecords.GetAll()
					.AsQueryable()
					.Include(i => i.Inspector)
					.Include(i => i.Station)
					.Where(i => i.VehicleId == vehicleId)
					.OrderByDescending(i => i.InspectionDate)
					.ToList();
			}
			catch (Exception ex)
			{
				// Ghi log lỗi
				Console.WriteLine($"Error in GetInspectionHistory by vehicleId: {ex.Message}");
				return new List<InspectionRecord>();
			}
		}


		public List<InspectionRecord> GetPendingInspections(int inspectorId)
		{
			try
			{
				// Lấy danh sách kiểm định đang chờ của một nhân viên kiểm định
				return _inspectionRecords.GetAll()
					.Where(i => i.InspectorId == inspectorId && i.Result == "Pending")
					.ToList();
			}
			catch (Exception ex)
			{
				// Ghi log lỗi
				Console.WriteLine($"Error in GetPendingInspections: {ex.Message}");
				return new List<InspectionRecord>();

			}
		}

		public bool IsInspectionValid(int vehicleId)
		{
			try
			{
				// Lấy lần kiểm định gần nhất
				var latestInspection = _inspectionRecords.GetAll()
					.Where(i => i.VehicleId == vehicleId && i.Result == "Pass")
					.OrderByDescending(i => i.InspectionDate)
					.FirstOrDefault();

				if (latestInspection == null)
				{
					// Không có lần kiểm định nào đạt
					return false;
				}

				// Kiểm tra ngày hết hạn kiểm định
				return latestInspection.InspectionDate.HasValue &&
					   latestInspection.InspectionDate.Value >= DateTime.Today;
			}
			catch (Exception ex)
			{
				// Ghi log lỗi
				Console.WriteLine($"Error in IsInspectionValid: {ex.Message}");
				return false;
			}
		}
		public List<ExpiredVehicleInfo> GetExpiredVehicles()
		{
			try
			{
				var today = DateTime.Today;

				return _inspectionRecords
					.GetAll()
					.Where(i => i.InspectionDate < today)
					.GroupBy(i => i.VehicleId)
					.Select(g => g.OrderByDescending(i => i.InspectionDate).First())
					.Select(i => new ExpiredVehicleInfo
					{
						VehicleID = i.VehicleId,
						PlateNumber = i.Vehicle.PlateNumber,
						Brand = i.Vehicle.Brand,
						Model = i.Vehicle.Model,
						OwnerName = i.Vehicle.Owner.FullName,
						OwnerPhone = i.Vehicle.Owner.Phone,
						LastInspectionDate = i.InspectionDate.Value,
						ExpiryDate = i.InspectionDate.Value,
						DaysOverdue = (today - i.InspectionDate.Value).Days
					})
					.OrderByDescending(v => v.DaysOverdue)
					.ToList();
			}
			catch (Exception ex)
			{
				// Log error
				Console.WriteLine($"Error in GetExpiredVehicles: {ex.Message}");
				return new List<ExpiredVehicleInfo>();
			}
		}

		// ...

		public InspectionReportData GetInspectionReport(int stationId, DateTime startDate, DateTime endDate)
		{
			try
			{
				var report = new InspectionReportData();

				// Lấy tất cả các lần kiểm định trong khoảng thời gian
				var inspections = _inspectionRecords
					.GetAll()
					.AsQueryable() // Convert to IQueryable to use Include
					.Include(i => i.Vehicle)
					.Include(i => i.Vehicle.Owner)
					.Where(i => i.StationId == stationId &&
						i.InspectionDate >= startDate &&
						i.InspectionDate <= endDate)
					.ToList();

				// Tạo chi tiết báo cáo
				report.Details = inspections.Select(i => new InspectionDetail
				{
					InspectionDate = i.InspectionDate ?? DateTime.MinValue,
					PlateNumber = i.Vehicle.PlateNumber,
					OwnerName = i.Vehicle.Owner.FullName,
					CO = i.Co2emission,
					HC = i.Hcemission,
					Result = i.Result
				}).ToList();

				// Đếm số lượng đạt/không đạt
				report.PassCount = inspections.Count(i => i.Result == "Pass");
				report.FailCount = inspections.Count(i => i.Result == "Fail");

				// Thống kê theo ngày
				report.DailyInspections = inspections
					.GroupBy(i => i.InspectionDate.Value.Date)
					.ToDictionary(g => g.Key, g => g.Count());

				return report;
			}
			catch (Exception ex)
			{
				// Log error
				Console.WriteLine($"Error in GetInspectionReport: {ex.Message}");
				return new InspectionReportData();
			}
		}

		// Add these methods to InspectionService.cs
		public RegionStatistics GetRegionStatistics(string region)
		{
			try
			{
				var result = new RegionStatistics();

				// Get all stations in the region
				var stationsInRegion = _inspectionStations.GetAll()
					.Where(s => s.Address.Contains(region))
					.ToList();

				// Get all inspections from these stations
				var stationIds = stationsInRegion.Select(s => s.StationId).ToList();
				var inspections = _inspectionRecords.GetAll()
					.Where(i => stationIds.Contains(i.StationId))
					.ToList();

				// Calculate compliance rate by province
				var provinces = stationsInRegion
					.Select(s => ExtractProvince(s.Address))
					.Distinct()
					.ToList();

				foreach (var province in provinces)
				{
					var provinceStations = stationsInRegion
						.Where(s => ExtractProvince(s.Address) == province)
						.ToList();

					var provinceStationIds = provinceStations.Select(s => s.StationId).ToList();
					var provinceInspections = inspections
						.Where(i => provinceStationIds.Contains(i.StationId))
						.ToList();

					int totalInspections = provinceInspections.Count;
					int passedInspections = provinceInspections.Count(i => i.Result == "Pass");

					double complianceRate = totalInspections > 0
						? (double)passedInspections / totalInspections
						: 0;

					result.ComplianceRate.Add(province, complianceRate);

					// Calculate average emission levels
					decimal avgEmission = totalInspections > 0
						? provinceInspections.Average(i => i.Co2emission)
						: 0;

					result.EmissionTrend.Add(province, avgEmission);
				}

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GetRegionStatistics: {ex.Message}");
				return new RegionStatistics();
			}
		}

		

		// Helper method to extract province from an address
		private string ExtractProvince(string address)
		{
			if (string.IsNullOrEmpty(address))
				return "Unknown";

			// This is a simple implementation - in a real app, you'd have more robust logic
			var parts = address.Split(',');
			if (parts.Length >= 2)
				return parts[parts.Length - 1].Trim();

			return address;
		}



	}
}