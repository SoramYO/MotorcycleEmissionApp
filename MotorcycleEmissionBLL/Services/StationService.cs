using MotorcycleEmissionDAL;
using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MotorcycleEmissionBLL.Services
{
    public class StationService : IStationService
    {
        private readonly GenericRepository<InspectionStation> _stationRepository;
		private readonly GenericRepository<InspectionRecord> _inspectionRecordRepository;

		public StationService( )
        {
			_stationRepository = new GenericRepository<InspectionStation>();
			_inspectionRecordRepository = new GenericRepository<InspectionRecord>();

		}
        
        #region Core Station Operations
        
        public InspectionStation GetStation(int? id = null, string email = null)
        {
            if (id.HasValue)
                return _stationRepository.GetById(id.Value);
            else if (!string.IsNullOrEmpty(email))
                return _stationRepository.GetAll().Where(s => s.Email == email).FirstOrDefault();
            
            throw new ArgumentException("Either id or email must be provided");
        }
        
        public IEnumerable<InspectionStation> GetAllStations()
        {
            return _stationRepository.GetAll();
        }
        
        public void AddStation(InspectionStation station)
        {
            if (StationExists(station.Email))
                throw new Exception("A station with this email already exists");

			_stationRepository.Add(station);
        }
        
        public void UpdateStation(InspectionStation station)
        {
            var existingStation = _stationRepository.GetById(station.StationId);
            
            if (existingStation == null)
                throw new Exception("Station not found");

            // Check if email is changed and already exists
            if (existingStation.Email != station.Email &&
				_stationRepository.GetAll().Where(s => s.Email == station.Email && s.StationId != station.StationId).Any())
                throw new Exception("A station with this email already exists");

			_stationRepository.Update(station);
        }
        
        public void DeleteStation(int stationId)
        {
            _stationRepository.DeleteById(stationId);
        }
        
        public bool StationExists(string email)
        {
            return _stationRepository.GetAll().Where(s => s.Email == email).Any();
        }
        
        #endregion
        
        #region Station Location Methods
        
        public IEnumerable<string> GetProvincesByRegion(string region)
        {
            // Map region to provinces
            switch (region.ToLower())
            {
                case "miền bắc":
                    return new[] { "Hà Nội", "Hải Phòng", "Quảng Ninh", "Bắc Ninh", "Thái Nguyên" };
                case "miền trung":
                    return new[] { "Đà Nẵng", "Huế", "Quảng Nam", "Bình Định", "Khánh Hòa" };
                case "miền nam":
                    return new[] { "Hồ Chí Minh", "Đồng Nai", "Bình Dương", "Cần Thơ", "Vũng Tàu" };
                default:
                    return new string[] { };
            }
        }

		public List<InspectionStation> GetStationsByProvince(string province)
		{
			return _stationRepository.GetAll().Where(s => s.Address.Contains(province)).ToList();
		}
        
        #endregion
        

        
        
        #region Statistics and Reporting
        
        public Dictionary<int, int> GetStationInspectionCounts()
        {
            var allInspections = _inspectionRecordRepository.GetAll();
            return allInspections
                .GroupBy(i => i.StationId)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        
        public Dictionary<int, double> GetStationPassRates()
        {
            var allInspections = _inspectionRecordRepository.GetAll();
            var stationGroups = allInspections.GroupBy(i => i.StationId);
            
            var passRates = new Dictionary<int, double>();
            
            foreach (var group in stationGroups)
            {
                int stationId = group.Key;
                int totalInspections = group.Count();
                int passedInspections = group.Count(i => i.Result == "Pass");
                
                double passRate = totalInspections > 0 
                    ? (double)passedInspections / totalInspections * 100 
                    : 0;
                
                passRates.Add(stationId, Math.Round(passRate, 2));
            }
            
            return passRates;
        }
        
        #endregion
    }
}