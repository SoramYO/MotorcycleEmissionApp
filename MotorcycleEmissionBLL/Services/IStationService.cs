using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;

namespace MotorcycleEmissionBLL.Services
{
	public interface IStationService
	{
		// Core station operations
		InspectionStation GetStation(int? id = null, string email = null);
		IEnumerable<InspectionStation> GetAllStations();
		void AddStation(InspectionStation station);
		void UpdateStation(InspectionStation station);
		void DeleteStation(int stationId);
		bool StationExists(string email);

		// Station location methods
		IEnumerable<string> GetProvincesByRegion(string region);
		List<InspectionStation> GetStationsByProvince(string province);


		// Statistics and reporting
		Dictionary<int, int> GetStationInspectionCounts();
		Dictionary<int, double> GetStationPassRates();
	}
}