using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;

namespace MotorcycleEmissionBLL.Services
{
    public interface IVehicleService
    {
        Vehicle GetVehicleById(int vehicleId);
        IEnumerable<Vehicle> GetAllVehicles();
        IEnumerable<Vehicle> GetVehiclesByOwner(int ownerId);
        Vehicle GetVehicleByPlateNumber(string plateNumber);
        void AddVehicle(Vehicle vehicle);
        void UpdateVehicle(Vehicle vehicle);
        void DeleteVehicle(int vehicleId);
        bool VehicleExists(string plateNumber);
    }
} 