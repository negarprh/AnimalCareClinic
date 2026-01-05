/* 
    Animal-Care Veterinary Clinic – Database Script
    Team: Negar Pirasteh • Betty Dang • Hope Jeanine Ukundimana • Ngoc Yen Nhi Pham
*/

/* ===========================
   1. CREATE / SELECT DATABASE
   =========================== */
IF DB_ID('AnimalCareClinic') IS NULL
    CREATE DATABASE AnimalCareClinic;
GO

USE AnimalCareClinic;
GO


/* ======================
   2. MAIN DATA TABLES
   ====================== */

-- 2.1 Owner
CREATE TABLE dbo.[Owner]
(
    OwnerID       INT           IDENTITY(1,1) PRIMARY KEY,
    FirstName     VARCHAR(50)   NOT NULL,
    LastName      VARCHAR(50)   NOT NULL,
    Address       VARCHAR(255)  NULL,
    PhoneNumber   VARCHAR(20)   NULL,
    Email         VARCHAR(100)  NULL,

    CONSTRAINT UQ_Owner_Email UNIQUE (Email),
    CONSTRAINT UQ_Owner_Phone UNIQUE (PhoneNumber)
);
GO

-- 2.2 Veterinarian
CREATE TABLE dbo.Veterinarian
(
    VeterinarianID INT           IDENTITY(1,1) PRIMARY KEY,
    FirstName      VARCHAR(50)   NOT NULL,
    LastName       VARCHAR(50)   NOT NULL,
    Speciality     VARCHAR(100)  NULL,
    PhoneNumber    VARCHAR(20)   NULL,
    Email          VARCHAR(100)  NULL,

    CONSTRAINT UQ_Vet_Email UNIQUE (Email),
    CONSTRAINT UQ_Vet_Phone UNIQUE (PhoneNumber)
);
GO

-- 2.3 Animal (each animal belongs to one owner)
CREATE TABLE dbo.Animal
(
    AnimalID        INT           IDENTITY(1,1) PRIMARY KEY,
    OwnerID         INT           NOT NULL,
    Name            VARCHAR(50)   NOT NULL,
    Species         VARCHAR(40)   NOT NULL,
    Age             INT           NULL,
    Gender          CHAR(1)       NULL,            -- M, F, U (Unknown)
    MedicalHistory  VARCHAR(255)  NULL,

    CONSTRAINT FK_Animal_Owner FOREIGN KEY (OwnerID)
        REFERENCES dbo.[Owner](OwnerID),

    CONSTRAINT CK_Animal_Gender CHECK (Gender IN ('M','F','U'))
);
GO

-- 2.4 Schedule (available time slots per veterinarian)
CREATE TABLE dbo.Schedule
(
    ScheduleID      INT           IDENTITY(1,1) PRIMARY KEY,
    VeterinarianID  INT           NOT NULL,
    [Date]          DATE          NOT NULL,
    TimeSlot        VARCHAR(20)   NOT NULL,        -- e.g. '09:00-09:30'
    [Status]        VARCHAR(20)   NOT NULL,        -- available / unavailable

    CONSTRAINT FK_Schedule_Vet FOREIGN KEY (VeterinarianID)
        REFERENCES dbo.Veterinarian(VeterinarianID),

    CONSTRAINT CK_Schedule_Status CHECK ([Status] IN ('available','unavailable')),

    -- a vet cannot have two entries for the same date and time slot
    CONSTRAINT UQ_Schedule UNIQUE (VeterinarianID, [Date], TimeSlot)
);
GO

-- 2.5 Appointment (links an animal to one schedule slot)
CREATE TABLE dbo.Appointment
(
    AppointmentID   INT           IDENTITY(1,1) PRIMARY KEY,
    ScheduleID      INT           NOT NULL,
    AnimalID        INT           NOT NULL,
    AppointmentDate DATE          NOT NULL,
    AppointmentTime TIME(0)       NOT NULL,
    Reason          VARCHAR(255)  NULL,
    [Status]        VARCHAR(20)   NOT NULL DEFAULT('scheduled'),
        -- scheduled / confirmed / completed / cancelled

    CONSTRAINT FK_Appt_Schedule FOREIGN KEY (ScheduleID)
        REFERENCES dbo.Schedule(ScheduleID),

    CONSTRAINT FK_Appt_Animal FOREIGN KEY (AnimalID)
        REFERENCES dbo.Animal(AnimalID),

    CONSTRAINT CK_Appt_Status CHECK ([Status] IN ('scheduled','confirmed','completed','cancelled'))
);
GO

-- 2.6 VisitHistory (medical record of each visit)
CREATE TABLE dbo.VisitHistory
(
    VisitID         INT           IDENTITY(1,1) PRIMARY KEY,
    AppointmentID   INT           NOT NULL,
    AnimalID        INT           NOT NULL,
    VeterinarianID  INT           NOT NULL,
    VisitDate       DATE          NOT NULL,
    Diagnosis       VARCHAR(255)  NULL,
    Treatment       VARCHAR(255)  NULL,
    Prescription    VARCHAR(255)  NULL,

    CONSTRAINT FK_Visit_Appt FOREIGN KEY (AppointmentID)
        REFERENCES dbo.Appointment(AppointmentID),

    CONSTRAINT FK_Visit_Animal FOREIGN KEY (AnimalID)
        REFERENCES dbo.Animal(AnimalID),

    CONSTRAINT FK_Visit_Vet FOREIGN KEY (VeterinarianID)
        REFERENCES dbo.Veterinarian(VeterinarianID)
);
GO

-- User accounts:
CREATE TABLE [UserAccount] (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL, 
    Role NVARCHAR(20) NOT NULL         -- 'Admin', 'Secretary', 'Veterinarian'
);

INSERT INTO UserAccount (Username, Password, Role) VALUES
('admin1',    'admin123',    'Admin'),
('sec1',      'sec123',      'Secretary'),
('vet1',      'vet123',      'Veterinarian');



/* ======================
   3. INDEXES
   (help joins and searches)
   ====================== */

CREATE INDEX IX_Animal_Owner         ON dbo.Animal(OwnerID);
CREATE INDEX IX_Schedule_Vet_Date    ON dbo.Schedule(VeterinarianID, [Date]);
CREATE INDEX IX_Appt_Schedule        ON dbo.Appointment(ScheduleID);
CREATE INDEX IX_Appt_Animal          ON dbo.Appointment(AnimalID);
CREATE INDEX IX_Visit_Appt           ON dbo.VisitHistory(AppointmentID);
CREATE INDEX IX_Visit_Animal_VetDate ON dbo.VisitHistory(AnimalID, VeterinarianID, VisitDate);
GO


/* ==========================
   4. SAMPLE DATA
   ========================== */

-- 4.1 Owners
INSERT dbo.[Owner](FirstName, LastName, Address, PhoneNumber, Email)
VALUES 
('Sara','Miller','123 Park Ave','514-000-0001','sara@example.com'),
('Alex','Chen','77 King St','514-000-0002','alex@example.com'),
('Betty','Dang','Montreal, QC','514-111-2222','betty.owner@clinic.com'),
('Ngoc','Pham','Montreal, QC','514-333-4444','ngoc.owner@clinic.com');
GO

-- 4.2 Veterinarians
INSERT dbo.Veterinarian(FirstName, LastName, Speciality, PhoneNumber, Email)
VALUES 
('Negar','Pirasteh','General Medicine','514-555-1001','negar.pirasteh@vetclinic.com'),
('Betty','Dang','Surgery','514-555-1002','betty.dang@vetclinic.com'),
('Hope','Jeanine Ukundimana','Dermatology','514-555-1003','hope.ukundimana@vetclinic.com'),
('Ngoc','Yen Nhi Pham','Dentistry','514-555-1004','ngoc.pham@vetclinic.com');
GO

-- 4.3 Animals
INSERT dbo.Animal(OwnerID, Name, Species, Age, Gender, MedicalHistory)
VALUES
(1, 'Buddy', 'Dog', 4, 'M', 'Vaccinated, regular checkup'),
(2, 'Luna',  'Cat', 2, 'F', 'Allergic to fish'),
(3, 'Coco',  'Rabbit', 1, 'F', 'Dental follow-up required'),
(4, 'Max',   'Dog', 3, 'M', 'Recovering from surgery');
GO

-- 4.4 Simple schedules for Negar and Betty
INSERT dbo.Schedule(VeterinarianID, [Date], TimeSlot, [Status])
VALUES
(1, CONVERT(date,GETDATE()+1), '09:00-09:30','available'),
(1, CONVERT(date,GETDATE()+1), '09:30-10:00','available'),
(2, CONVERT(date,GETDATE()+1), '10:00-10:30','available'),
(2, CONVERT(date,GETDATE()+1), '10:30-11:00','available');
GO

-- 4.5 Two appointments based on those slots
INSERT dbo.Appointment(ScheduleID, AnimalID, AppointmentDate, AppointmentTime, Reason, [Status])
VALUES
(1, 1, (SELECT [Date] FROM dbo.Schedule WHERE ScheduleID = 1), '09:00', 'General check-up', 'scheduled'),
(3, 2, (SELECT [Date] FROM dbo.Schedule WHERE ScheduleID = 3), '10:00', 'Vaccination',     'scheduled');
GO


/* ======================
   5. USEFUL VIEWS
   ====================== */

-- 5.1 View: Vet calendar (slots + appointments)
CREATE VIEW dbo.vw_VetCalendar
AS
SELECT
    v.VeterinarianID,
    v.FirstName + ' ' + v.LastName AS Veterinarian,
    s.ScheduleID,
    s.[Date],
    s.TimeSlot,
    s.[Status]     AS SlotStatus,
    a.AppointmentID,
    a.AnimalID,
    a.[Status]     AS AppointmentStatus
FROM dbo.Veterinarian v
JOIN dbo.Schedule s ON s.VeterinarianID = v.VeterinarianID
LEFT JOIN dbo.Appointment a ON a.ScheduleID = s.ScheduleID;
GO

-- 5.2 View: Visit summary (for reports)
CREATE VIEW dbo.vw_VisitSummary
AS
SELECT
    vh.VisitID,
    vh.VisitDate,
    an.AnimalID,
    an.Name       AS AnimalName,
    o.OwnerID,
    o.FirstName + ' ' + o.LastName AS OwnerName,
    v.VeterinarianID,
    v.FirstName + ' ' + v.LastName AS VetName,
    vh.Diagnosis,
    vh.Treatment,
    vh.Prescription
FROM dbo.VisitHistory vh
JOIN dbo.Appointment a  ON a.AppointmentID   = vh.AppointmentID
JOIN dbo.Animal an      ON an.AnimalID       = vh.AnimalID
JOIN dbo.[Owner] o      ON o.OwnerID         = an.OwnerID
JOIN dbo.Veterinarian v ON v.VeterinarianID  = vh.VeterinarianID;
GO


/* ===========================
   6. STORED PROCEDURES
   =========================== */

-- 6.1 Set or update availability for a vet
CREATE PROCEDURE dbo.sp_SetAvailability
    @VeterinarianID INT,
    @Date           DATE,
    @TimeSlot       VARCHAR(20),
    @Status         VARCHAR(20)   -- 'available' or 'unavailable'
AS
BEGIN
    SET NOCOUNT ON;

    -- If the slot exists, update it; otherwise insert a new one
    IF EXISTS (SELECT 1 FROM dbo.Schedule
               WHERE VeterinarianID = @VeterinarianID
                 AND [Date] = @Date
                 AND TimeSlot = @TimeSlot)
    BEGIN
        UPDATE dbo.Schedule
        SET [Status] = @Status
        WHERE VeterinarianID = @VeterinarianID
          AND [Date] = @Date
          AND TimeSlot = @TimeSlot;
    END
    ELSE
    BEGIN
        INSERT dbo.Schedule(VeterinarianID, [Date], TimeSlot, [Status])
        VALUES (@VeterinarianID, @Date, @TimeSlot, @Status);
    END
END
GO

-- 6.2 Book an appointment for an available slot
CREATE PROCEDURE dbo.sp_BookAppointment
    @ScheduleID INT,
    @AnimalID   INT,
    @Reason     VARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Only allow booking if the slot is available
    IF EXISTS (SELECT 1 FROM dbo.Schedule 
               WHERE ScheduleID = @ScheduleID AND [Status] = 'available')
    BEGIN
        DECLARE @ApptDate DATE;
        DECLARE @ApptTime TIME(0);

        SELECT 
            @ApptDate = [Date],
            @ApptTime = TRY_CONVERT(TIME(0), LEFT(TimeSlot, 5))
        FROM dbo.Schedule
        WHERE ScheduleID = @ScheduleID;

        INSERT dbo.Appointment(ScheduleID, AnimalID, AppointmentDate, AppointmentTime, Reason, [Status])
        VALUES (@ScheduleID, @AnimalID, @ApptDate, @ApptTime, @Reason, 'scheduled');

        -- Mark the slot as unavailable after booking
        UPDATE dbo.Schedule
        SET [Status] = 'unavailable'
        WHERE ScheduleID = @ScheduleID;
    END
END
GO

-- 6.3 Record a visit and mark appointment as completed
CREATE PROCEDURE dbo.sp_RecordVisit
    @AppointmentID INT,
    @Diagnosis     VARCHAR(255) = NULL,
    @Treatment     VARCHAR(255) = NULL,
    @Prescription  VARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AnimalID INT;
    DECLARE @VetID    INT;
    DECLARE @Date     DATE;

    SELECT  
        @AnimalID = a.AnimalID,
        @VetID    = s.VeterinarianID,
        @Date     = a.AppointmentDate
    FROM dbo.Appointment a
    JOIN dbo.Schedule s ON s.ScheduleID = a.ScheduleID
    WHERE a.AppointmentID = @AppointmentID;

    IF @AnimalID IS NOT NULL
    BEGIN
        INSERT dbo.VisitHistory(AppointmentID, AnimalID, VeterinarianID, VisitDate,
                                Diagnosis, Treatment, Prescription)
        VALUES (@AppointmentID, @AnimalID, @VetID, @Date,
                @Diagnosis, @Treatment, @Prescription);

        UPDATE dbo.Appointment
        SET [Status] = 'completed'
        WHERE AppointmentID = @AppointmentID;
    END
END
GO

/* End of script */
