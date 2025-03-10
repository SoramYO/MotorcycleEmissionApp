using MotorcycleEmissionDAL;
using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MotorcycleEmissionBLL.Services
{
    public class VehicleService : IVehicleService
    {
		private readonly GenericRepository<Vehicle> _vehicleRepository;
        private readonly GenericRepository<InspectionRecord> _inspectionRecordRepository;

		public VehicleService()
        {
			_vehicleRepository = new GenericRepository<Vehicle>();
			_inspectionRecordRepository = new GenericRepository<InspectionRecord>();
		}

        public void AddVehicle(Vehicle vehicle)
        {
            if (VehicleExists(vehicle.PlateNumber))
                throw new Exception("A vehicle with this plate number already exists");

			_vehicleRepository.Add(vehicle);
        }

        public void DeleteVehicle(int vehicleId)
        {
			_vehicleRepository.DeleteById(vehicleId);
        }

        public IEnumerable<Vehicle> GetAllVehicles()
        {
            return _vehicleRepository.GetAll();
        }

        public Vehicle GetVehicleById(int vehicleId)
        {
            return _vehicleRepository.GetById(vehicleId);
        }

        public Vehicle GetVehicleByPlateNumber(string plateNumber)
        {
            return _vehicleRepository.GetAll().Find(v => v.PlateNumber == plateNumber);
        }

		public IEnumerable<Vehicle> GetVehiclesByOwner(int ownerId)
		{
			return _vehicleRepository.GetAll().Where(v => v.OwnerId == ownerId).ToList();
		}

        public void UpdateVehicle(Vehicle vehicle)
        {
            var existingVehicle = _vehicleRepository.GetById(vehicle.VehicleId);
            
            if (existingVehicle == null)
                throw new Exception("Vehicle not found");

			// If plate number changed, check if new plate number exists
			if (existingVehicle.PlateNumber != vehicle.PlateNumber && VehicleExists(vehicle.PlateNumber))
				throw new Exception("A vehicle with this plate number already exists");
            _vehicleRepository.Update(vehicle);


		}

		public bool VehicleExists(string plateNumber)
		{
			return _vehicleRepository.GetAll().Any(v => v.PlateNumber == plateNumber);
		}

        public bool VerifyVehicleDocuments(int vehicleId, string documentType, string documentNumber)
        {
            var vehicle = _vehicleRepository.GetById(vehicleId);
            if (vehicle == null)
                return false;

            // Trong thực tế, bạn sẽ tích hợp với API bên ngoài để xác minh
            // Đây chỉ là mô phỏng đơn giản
            
            // Giả lập đang xác minh với hệ thống đăng ký phương tiện
            if (documentType == "RegistrationCertificate")
            {
                // Kiểm tra xem số đăng ký có khớp với biển số xe không
                return documentNumber.Contains(vehicle.PlateNumber);
            }
            
            // Giả lập đang xác minh với hệ thống kiểm định
            if (documentType == "InspectionCertificate")
            {
                // Kiểm tra xem có lịch sử kiểm định gần đây không
                var recentInspection = _inspectionRecordRepository.GetAll().Where(i => 
                    i.VehicleId == vehicleId && 
                    i.Result == "Pass" && 
                    i.InspectionDate > DateTime.Now.AddMonths(-12))
                    .Any();
                    
                return recentInspection;
            }
            
            return false;
        }
    }
} 