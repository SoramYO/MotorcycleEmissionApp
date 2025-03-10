-- Tạo database
Create DATABASE MotorcycleEmissionDB;
GO

-- Sử dụng database vừa tạo
USE MotorcycleEmissionDB;
GO

-- Tạo bảng Users
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Role VARCHAR(20) NOT NULL CHECK (Role IN ('Owner', 'Inspector', 'Station', 'Police')),
    Phone VARCHAR(15) NOT NULL,
    Address NVARCHAR(MAX) NOT NULL
);
GO

-- Tạo bảng Vehicles
CREATE TABLE Vehicles (
    VehicleID INT PRIMARY KEY IDENTITY(1,1),
    OwnerID INT NOT NULL,
    PlateNumber VARCHAR(15) NOT NULL UNIQUE,
    Brand VARCHAR(50) NOT NULL,
    Model VARCHAR(50) NOT NULL,
    ManufactureYear INT NOT NULL,
    EngineNumber VARCHAR(100) NOT NULL,
    FOREIGN KEY (OwnerID) REFERENCES Users(UserID)
);
GO

-- Tạo bảng InspectionStations
CREATE TABLE InspectionStations (
    StationID INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(100) NOT NULL,
    Address NVARCHAR(MAX) NOT NULL,
    Phone VARCHAR(15) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE
);
GO

-- Tạo bảng InspectionRecords
CREATE TABLE InspectionRecords (
    RecordID INT PRIMARY KEY IDENTITY(1,1),
    VehicleID INT NOT NULL,
    StationID INT NOT NULL,
    InspectorID INT NOT NULL,
    InspectionDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Result VARCHAR(10) NOT NULL CHECK (Result IN ('Pass', 'Fail')),
    CO2Emission DECIMAL(5,2) NOT NULL,
    HCEmission DECIMAL(5,2) NOT NULL,
    Comments NVARCHAR(MAX),
    FOREIGN KEY (VehicleID) REFERENCES Vehicles(VehicleID) ,
    FOREIGN KEY (StationID) REFERENCES InspectionStations(StationID),
    FOREIGN KEY (InspectorID) REFERENCES Users(UserID) 
);
GO

-- Tạo bảng Notifications
CREATE TABLE Notifications (
    NotificationID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    SentDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    IsRead BIT DEFAULT 0,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- Tạo bảng Logs
CREATE TABLE Logs (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    Action VARCHAR(100) NOT NULL,
    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO
