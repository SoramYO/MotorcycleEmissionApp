using MotorcycleEmissionDAL;
using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MotorcycleEmissionBLL.Services
{
    public class NotificationService : INotificationService
    {

		private readonly GenericRepository<Notification> _notifications;
        private readonly GenericRepository<Vehicle> _vehicles;
		private readonly GenericRepository<InspectionRecord> _inspectionRecords;
        private readonly GenericRepository<User> _users;
		public NotificationService( )
        {
			_notifications = new GenericRepository<Notification>();
			_vehicles = new GenericRepository<Vehicle>();
			_inspectionRecords = new GenericRepository<InspectionRecord>();
			_users = new GenericRepository<User>();
		}

        public void DeleteNotification(int notificationId)
        {
			_notifications.DeleteById(notificationId);
        }

        public IEnumerable<Notification> GetAllNotifications()
        {
            return _notifications.GetAll();
        }

        public Notification GetNotificationById(int notificationId)
        {
            return _notifications.GetById(notificationId);
        }

		public List<Notification> GetNotificationsByUser(int userId)
		{
			return _notifications.GetAll().Where(n => n.UserId == userId)
				.OrderByDescending(n => n.SentDate)
				.ToList();
		}


		public IEnumerable<Notification> GetUnreadNotificationsByUser(int userId)
		{
			return _notifications.GetAll().Where(n => n.UserId == userId && n.IsRead == false)
				.OrderByDescending(n => n.SentDate);
		}

		public void MarkAllNotificationsAsRead(int userId)
        {
            var notifications = GetUnreadNotificationsByUser(userId);
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
				_notifications.Update(notification);
            }
        }

        public void MarkNotificationAsRead(int notificationId)
        {
            var notification = _notifications.GetById(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
				_notifications.Update(notification);
            }
        }

        public void SendInspectionDueReminder(int vehicleId, int days)
        {
            var vehicle = _vehicles.GetById(vehicleId);
            if (vehicle == null)
                throw new Exception("Vehicle not found");

            var lastInspection = _inspectionRecords.GetAll()
				.Where(i => i.VehicleId == vehicleId)
                .OrderByDescending(i => i.InspectionDate)
                .FirstOrDefault();

            // If no previous inspection or if the latest inspection passed
            if (lastInspection == null || lastInspection.Result == "Pass")
            {
                string message = $"Your vehicle with plate number {vehicle.PlateNumber} is due for emission inspection in {days} days.";
                SendNotification(vehicle.OwnerId, message);
            }
        }

		public void SendInspectionResultNotification(int inspectionId)
		{
			var inspection = _inspectionRecords.GetById(inspectionId);
			if (inspection == null)
				throw new Exception("Inspection record not found");

			var vehicle = _vehicles.GetById(inspection.VehicleId);
			if (vehicle == null)
				throw new Exception("Vehicle not found");

			string status = inspection.Result == "Pass" ? "passed" : "failed";
			string inspectionDate = inspection.InspectionDate.HasValue ? inspection.InspectionDate.Value.ToShortDateString() : "unknown date";
			string message = $"Your vehicle with plate number {vehicle.PlateNumber} has {status} the emission inspection on {inspectionDate}.";

			SendNotification(vehicle.OwnerId, message);
		}

        public void SendNotification(int userId, string message)
        {
            var user = _users.GetById(userId);
            if (user == null)
                throw new Exception("User not found");

            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                SentDate = DateTime.Now,
                IsRead = false
            };

            _notifications.Add(notification);
        }
    }
} 