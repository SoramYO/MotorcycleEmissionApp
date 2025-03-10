using MotorcycleEmissionDAL.Models;
using System;
using System.Collections.Generic;

namespace MotorcycleEmissionBLL.Services
{
    public interface INotificationService
    {
        Notification GetNotificationById(int notificationId);
        IEnumerable<Notification> GetAllNotifications();
		List<Notification> GetNotificationsByUser(int userId);
        IEnumerable<Notification> GetUnreadNotificationsByUser(int userId);
        void SendNotification(int userId, string message);
        void MarkNotificationAsRead(int notificationId);
        void MarkAllNotificationsAsRead(int userId);
        void DeleteNotification(int notificationId);
        void SendInspectionDueReminder(int vehicleId, int days);
        void SendInspectionResultNotification(int inspectionId);
    }
} 