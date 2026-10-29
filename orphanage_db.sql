CREATE DATABASE OrphanageDB;
GO

USE OrphanageDB;
GO


-- TABLE CREATION

CREATE TABLE Admin (
    A_ID INT IDENTITY(1,1) PRIMARY KEY,
    A_Pno NVARCHAR(20),
    A_name NVARCHAR(100)
);

CREATE TABLE Volunteer (
    V_ID INT IDENTITY(1,1) PRIMARY KEY,
    V_name NVARCHAR(100),
    V_hours INT
);

CREATE TABLE Orphan (
    O_ID INT IDENTITY(1,1) PRIMARY KEY,
    O_Fname NVARCHAR(100),
    O_Lname NVARCHAR(100),
    O_gender NVARCHAR(20),
    O_DoB DATE,
    A_ID INT,
    FOREIGN KEY (A_ID) REFERENCES Admin(A_ID)
);

CREATE TABLE Assisted_by (
    V_ID INT,
    O_ID INT,
    PRIMARY KEY (V_ID, O_ID),
    FOREIGN KEY (V_ID) REFERENCES Volunteer(V_ID),
    FOREIGN KEY (O_ID) REFERENCES Orphan(O_ID)
);

CREATE TABLE Sponsor (
    S_ID INT IDENTITY(1,1) PRIMARY KEY,
    S_address NVARCHAR(200),
    S_Fname NVARCHAR(100),
    S_Lname NVARCHAR(100),
    S_email NVARCHAR(100),
    total_donation DECIMAL(10,2)
);

CREATE TABLE Sponsored_by (
    S_ID INT,
    O_ID INT,
    PRIMARY KEY (S_ID, O_ID),
    FOREIGN KEY (S_ID) REFERENCES Sponsor(S_ID),
    FOREIGN KEY (O_ID) REFERENCES Orphan(O_ID)
);

CREATE TABLE School (
    Sch_ID INT IDENTITY(1,1) PRIMARY KEY,
    Sch_name NVARCHAR(200),
    Sch_address NVARCHAR(200),
    Sch_email NVARCHAR(100)
);

CREATE TABLE Education_record (
    R_ID INT IDENTITY(1,1) PRIMARY KEY,
    Date_acquired DATE,
    certificate NVARCHAR(200),
    O_ID INT,
    Sch_ID INT,
    FOREIGN KEY (O_ID) REFERENCES Orphan(O_ID),
    FOREIGN KEY (Sch_ID) REFERENCES School(Sch_ID)
);

CREATE TABLE Donations (
    D_ID INT IDENTITY(1,1) PRIMARY KEY,
    D_date DATE,
    S_ID INT,
    O_ID INT,
    A_ID INT,
    FOREIGN KEY (S_ID) REFERENCES Sponsor(S_ID),
    FOREIGN KEY (O_ID) REFERENCES Orphan(O_ID),
    FOREIGN KEY (A_ID) REFERENCES Admin(A_ID)
);

CREATE TABLE Item (
    I_ID INT IDENTITY(1,1) PRIMARY KEY,
    I_type NVARCHAR(100),
    I_name NVARCHAR(100),
    I_quantity INT,
    D_ID INT,
    FOREIGN KEY (D_ID) REFERENCES Donations(D_ID)
);

CREATE TABLE Events (
    E_ID INT IDENTITY(1,1) PRIMARY KEY,
    E_Date DATE,
    E_participants INT,
    E_Location NVARCHAR(200),
    A_ID INT,
    O_ID INT,
    FOREIGN KEY (A_ID) REFERENCES Admin(A_ID),
    FOREIGN KEY (O_ID) REFERENCES Orphan(O_ID)
);

CREATE TABLE Mealplan (
    MP_ID INT IDENTITY(1,1) PRIMARY KEY,
    MP_meal NVARCHAR(100),
    O_ID INT,
    FOREIGN KEY (O_ID) REFERENCES Orphan(O_ID)
);

CREATE TABLE Served_to (
    MP_ID INT,
    O_ID INT,
    PRIMARY KEY (MP_ID, O_ID),
    FOREIGN KEY (MP_ID) REFERENCES Mealplan(MP_ID),
    FOREIGN KEY (O_ID) REFERENCES Orphan(O_ID)
);



-- DATA INSERTION 



INSERT INTO Admin (A_Pno, A_name) VALUES
(N'01000000001', N'Admin 01'),
(N'01000000002', N'Admin 02'),
(N'01000000003', N'Admin 03'),
(N'01000000004', N'Admin 04'),
(N'01000000005', N'Admin 05'),
(N'01000000006', N'Admin 06'),
(N'01000000007', N'Admin 07'),
(N'01000000008', N'Admin 08'),
(N'01000000009', N'Admin 09'),
(N'01000000010', N'Admin 10'),
(N'01000000011', N'Admin 11'),
(N'01000000012', N'Admin 12'),
(N'01000000013', N'Admin 13'),
(N'01000000014', N'Admin 14'),
(N'01000000015', N'Admin 15'),
(N'01000000016', N'Admin 16'),
(N'01000000017', N'Admin 17'),
(N'01000000018', N'Admin 18'),
(N'01000000019', N'Admin 19'),
(N'01000000020', N'Admin 20'),
(N'01000000021', N'Admin 21'),
(N'01000000022', N'Admin 22'),
(N'01000000023', N'Admin 23'),
(N'01000000024', N'Admin 24'),
(N'01000000025', N'Admin 25'),
(N'01000000026', N'Admin 26'),
(N'01000000027', N'Admin 27'),
(N'01000000028', N'Admin 28'),
(N'01000000029', N'Admin 29'),
(N'01000000030', N'Admin 30');

INSERT INTO Volunteer (V_name, V_hours) VALUES

(N'Volunteer 04', 20),
(N'Volunteer 05', 6),
(N'Volunteer 06', 18),
(N'Volunteer 07', 10),
(N'Volunteer 08', 25),
(N'Volunteer 09', 14),
(N'Volunteer 10', 9),
(N'Volunteer 11', 16),
(N'Volunteer 12', 7),
(N'Volunteer 13', 22),
(N'Volunteer 14', 11),
(N'Volunteer 15', 19),
(N'Volunteer 16', 13),
(N'Volunteer 17', 21),
(N'Volunteer 18', 5),
(N'Volunteer 19', 24),
(N'Volunteer 20', 17),
(N'Volunteer 21', 26),
(N'Volunteer 22', 15),
(N'Volunteer 23', 12),
(N'Volunteer 24', 8),
(N'Volunteer 25', 14),
(N'Volunteer 26', 20),
(N'Volunteer 27', 9),
(N'Volunteer 28', 18),
(N'Volunteer 29', 6),
(N'Volunteer 30', 23);

INSERT INTO Orphan (O_Fname, O_Lname, O_gender, O_DoB, A_ID) VALUES
(N'Orphan01', N'Last01', N'Male',   '2015-01-01', 1),
(N'Orphan02', N'Last02', N'Female', '2016-02-02', 2),
(N'Orphan03', N'Last03', N'Male',   '2014-03-03', 3),
(N'Orphan04', N'Last04', N'Female', '2013-04-04', 4),
(N'Orphan05', N'Last05', N'Male',   '2012-05-05', 5),
(N'Orphan06', N'Last06', N'Female', '2011-06-06', 6),
(N'Orphan07', N'Last07', N'Male',   '2010-07-07', 7),
(N'Orphan08', N'Last08', N'Female', '2017-08-08', 8),
(N'Orphan09', N'Last09', N'Male',   '2018-09-09', 9),
(N'Orphan10', N'Last10', N'Female', '2016-10-10', 10),
(N'Orphan11', N'Last11', N'Male',   '2015-11-11', 11),
(N'Orphan12', N'Last12', N'Female', '2014-12-12', 12),
(N'Orphan13', N'Last13', N'Male',   '2013-01-13', 13),
(N'Orphan14', N'Last14', N'Female', '2012-02-14', 14),
(N'Orphan15', N'Last15', N'Male',   '2011-03-15', 15),
(N'Orphan16', N'Last16', N'Female', '2010-04-16', 16),
(N'Orphan17', N'Last17', N'Male',   '2017-05-17', 17),
(N'Orphan18', N'Last18', N'Female', '2018-06-18', 18),
(N'Orphan19', N'Last19', N'Male',   '2016-07-19', 19),
(N'Orphan20', N'Last20', N'Female', '2015-08-20', 20),
(N'Orphan21', N'Last21', N'Male',   '2014-09-21', 21),
(N'Orphan22', N'Last22', N'Female', '2013-10-22', 22),
(N'Orphan23', N'Last23', N'Male',   '2012-11-23', 23),
(N'Orphan24', N'Last24', N'Female', '2011-12-24', 24),
(N'Orphan25', N'Last25', N'Male',   '2010-01-25', 25),
(N'Orphan26', N'Last26', N'Female', '2017-02-26', 26),
(N'Orphan27', N'Last27', N'Male',   '2018-03-27', 27),
(N'Orphan28', N'Last28', N'Female', '2016-04-28', 28),
(N'Orphan29', N'Last29', N'Male',   '2015-05-29', 29),
(N'Orphan30', N'Last30', N'Female', '2014-06-30', 30);

INSERT INTO Assisted_by VALUES
(1,1), (2,2), (3,3), (4,4), (5,5),
(6,6), (7,7), (8,8), (9,9), (10,10),
(11,11), (12,12), (13,13), (14,14), (15,15),
(16,16), (17,17), (18,18), (19,19), (20,20),
(21,21), (22,22), (23,23), (24,24), (25,25),
(26,26), (27,27), (28,28), (29,29), (30,30);

INSERT INTO Sponsor (S_address, S_Fname, S_Lname, S_email, total_donation) VALUES
(N'City 01', N'Sponsor01', N'Family01', N'sponsor01@mail.com', 5000),
(N'City 02', N'Sponsor02', N'Family02', N'sponsor02@mail.com', 3000),
(N'City 03', N'Sponsor03', N'Family03', N'sponsor03@mail.com', 4500),
(N'City 04', N'Sponsor04', N'Family04', N'sponsor04@mail.com', 6200),
(N'City 05', N'Sponsor05', N'Family05', N'sponsor05@mail.com', 2100),
(N'City 06', N'Sponsor06', N'Family06', N'sponsor06@mail.com', 7800),
(N'City 07', N'Sponsor07', N'Family07', N'sponsor07@mail.com', 3300),
(N'City 08', N'Sponsor08', N'Family08', N'sponsor08@mail.com', 9100),
(N'City 09', N'Sponsor09', N'Family09', N'sponsor09@mail.com', 2700),
(N'City 10', N'Sponsor10', N'Family10', N'sponsor10@mail.com', 5400),
(N'City 11', N'Sponsor11', N'Family11', N'sponsor11@mail.com', 6100),
(N'City 12', N'Sponsor12', N'Family12', N'sponsor12@mail.com', 3500),
(N'City 13', N'Sponsor13', N'Family13', N'sponsor13@mail.com', 4800),
(N'City 14', N'Sponsor14', N'Family14', N'sponsor14@mail.com', 2600),
(N'City 15', N'Sponsor15', N'Family15', N'sponsor15@mail.com', 7200),
(N'City 16', N'Sponsor16', N'Family16', N'sponsor16@mail.com', 6900),
(N'City 17', N'Sponsor17', N'Family17', N'sponsor17@mail.com', 4100),
(N'City 18', N'Sponsor18', N'Family18', N'sponsor18@mail.com', 5200),
(N'City 19', N'Sponsor19', N'Family19', N'sponsor19@mail.com', 3900),
(N'City 20', N'Sponsor20', N'Family20', N'sponsor20@mail.com', 8600),
(N'City 21', N'Sponsor21', N'Family21', N'sponsor21@mail.com', 4400),
(N'City 22', N'Sponsor22', N'Family22', N'sponsor22@mail.com', 5700),
(N'City 23', N'Sponsor23', N'Family23', N'sponsor23@mail.com', 2900),
(N'City 24', N'Sponsor24', N'Family24', N'sponsor24@mail.com', 9600),
(N'City 25', N'Sponsor25', N'Family25', N'sponsor25@mail.com', 2500),
(N'City 26', N'Sponsor26', N'Family26', N'sponsor26@mail.com', 6700),
(N'City 27', N'Sponsor27', N'Family27', N'sponsor27@mail.com', 7300),
(N'City 28', N'Sponsor28', N'Family28', N'sponsor28@mail.com', 3100),
(N'City 29', N'Sponsor29', N'Family29', N'sponsor29@mail.com', 8400),
(N'City 30', N'Sponsor30', N'Family30', N'sponsor30@mail.com', 4600);

INSERT INTO Sponsored_by VALUES
(1,1), (2,2), (3,3), (4,4), (5,5),
(6,6), (7,7), (8,8), (9,9), (10,10),
(11,11), (12,12), (13,13), (14,14), (15,15),
(16,16), (17,17), (18,18), (19,19), (20,20),
(21,21), (22,22), (23,23), (24,24), (25,25),
(26,26), (27,27), (28,28), (29,29), (30,30);

INSERT INTO School (Sch_name, Sch_address, Sch_email) VALUES
(N'School 01', N'Area 01', N'school01@mail.com'),
(N'School 02', N'Area 02', N'school02@mail.com'),
(N'School 03', N'Area 03', N'school03@mail.com'),
(N'School 04', N'Area 04', N'school04@mail.com'),
(N'School 05', N'Area 05', N'school05@mail.com'),
(N'School 06', N'Area 06', N'school06@mail.com'),
(N'School 07', N'Area 07', N'school07@mail.com'),
(N'School 08', N'Area 08', N'school08@mail.com'),
(N'School 09', N'Area 09', N'school09@mail.com'),
(N'School 10', N'Area 10', N'school10@mail.com'),
(N'School 11', N'Area 11', N'school11@mail.com'),
(N'School 12', N'Area 12', N'school12@mail.com'),
(N'School 13', N'Area 13', N'school13@mail.com'),
(N'School 14', N'Area 14', N'school14@mail.com'),
(N'School 15', N'Area 15', N'school15@mail.com'),
(N'School 16', N'Area 16', N'school16@mail.com'),
(N'School 17', N'Area 17', N'school17@mail.com'),
(N'School 18', N'Area 18', N'school18@mail.com'),
(N'School 19', N'Area 19', N'school19@mail.com'),
(N'School 20', N'Area 20', N'school20@mail.com'),
(N'School 21', N'Area 21', N'school21@mail.com'),
(N'School 22', N'Area 22', N'school22@mail.com'),
(N'School 23', N'Area 23', N'school23@mail.com'),
(N'School 24', N'Area 24', N'school24@mail.com'),
(N'School 25', N'Area 25', N'school25@mail.com'),
(N'School 26', N'Area 26', N'school26@mail.com'),
(N'School 27', N'Area 27', N'school27@mail.com'),
(N'School 28', N'Area 28', N'school28@mail.com'),
(N'School 29', N'Area 29', N'school29@mail.com'),
(N'School 30', N'Area 30', N'school30@mail.com');

INSERT INTO Education_record (Date_acquired, certificate, O_ID, Sch_ID) VALUES
('2024-01-10', N'Certificate 01', 1, 1),
('2024-02-10', N'Certificate 02', 2, 2),
('2024-03-10', N'Certificate 03', 3, 3),
('2024-04-10', N'Certificate 04', 4, 4),
('2024-05-10', N'Certificate 05', 5, 5),
('2024-06-10', N'Certificate 06', 6, 6),
('2024-07-10', N'Certificate 07', 7, 7),
('2024-08-10', N'Certificate 08', 8, 8),
('2024-09-10', N'Certificate 09', 9, 9),
('2024-10-10', N'Certificate 10', 10, 10),
('2024-11-10', N'Certificate 11', 11, 11),
('2024-12-10', N'Certificate 12', 12, 12),
('2025-01-10', N'Certificate 13', 13, 13),
('2025-02-10', N'Certificate 14', 14, 14),
('2025-03-10', N'Certificate 15', 15, 15),
('2025-04-10', N'Certificate 16', 16, 16),
('2025-05-10', N'Certificate 17', 17, 17),
('2025-06-10', N'Certificate 18', 18, 18),
('2025-07-10', N'Certificate 19', 19, 19),
('2025-08-10', N'Certificate 20', 20, 20),
('2025-09-10', N'Certificate 21', 21, 21),
('2025-10-10', N'Certificate 22', 22, 22),
('2025-11-10', N'Certificate 23', 23, 23),
('2025-12-10', N'Certificate 24', 24, 24),
('2026-01-10', N'Certificate 25', 25, 25),
('2026-02-10', N'Certificate 26', 26, 26),
('2026-03-10', N'Certificate 27', 27, 27),
('2026-04-10', N'Certificate 28', 28, 28),
('2026-05-10', N'Certificate 29', 29, 29),
('2026-06-10', N'Certificate 30', 30, 30);


INSERT INTO Donations (D_date, S_ID, O_ID, A_ID) VALUES
('2024-01-01', 1, 1, 1),
('2024-01-15', 2, 2, 2),
('2024-02-01', 3, 3, 3),
('2024-02-15', 4, 4, 4),
('2024-03-01', 5, 5, 5),
('2024-03-15', 6, 6, 6),
('2024-04-01', 7, 7, 7),
('2024-04-15', 8, 8, 8),
('2024-05-01', 9, 9, 9),
('2024-05-15', 10, 10, 10),
('2024-06-01', 11, 11, 11),
('2024-06-15', 12, 12, 12),
('2024-07-01', 13, 13, 13),
('2024-07-15', 14, 14, 14),
('2024-08-01', 15, 15, 15),
('2024-08-15', 16, 16, 16),
('2024-09-01', 17, 17, 17),
('2024-09-15', 18, 18, 18),
('2024-10-01', 19, 19, 19),
('2024-10-15', 20, 20, 20),
('2024-11-01', 21, 21, 21),
('2024-11-15', 22, 22, 22),
('2024-12-01', 23, 23, 23),
('2024-12-15', 24, 24, 24),
('2025-01-01', 25, 25, 25),
('2025-01-15', 26, 26, 26),
('2025-02-01', 27, 27, 27),
('2025-02-15', 28, 28, 28),
('2025-03-01', 29, 29, 29),
('2025-03-15', 30, 30, 30);

INSERT INTO Item (I_type, I_name, I_quantity, D_ID) VALUES
(N'Clothes', N'Item 01', 10, 1),
(N'Food', N'Item 02', 5, 2),
(N'Books', N'Item 03', 7, 3),
(N'Toys', N'Item 04', 12, 4),
(N'Medicine', N'Item 05', 3, 5),
(N'Clothes', N'Item 06', 8, 6),
(N'Food', N'Item 07', 15, 7),
(N'Books', N'Item 08', 6, 8),
(N'Toys', N'Item 09', 9, 9),
(N'Medicine', N'Item 10', 4, 10),
(N'Clothes', N'Item 11', 11, 11),
(N'Food', N'Item 12', 7, 12),
(N'Books', N'Item 13', 10, 13),
(N'Toys', N'Item 14', 5, 14),
(N'Medicine', N'Item 15', 2, 15),
(N'Clothes', N'Item 16', 14, 16),
(N'Food', N'Item 17', 9, 17),
(N'Books', N'Item 18', 8, 18),
(N'Toys', N'Item 19', 13, 19),
(N'Medicine', N'Item 20', 6, 20),
(N'Clothes', N'Item 21', 7, 21),
(N'Food', N'Item 22', 12, 22),
(N'Books', N'Item 23', 4, 23),
(N'Toys', N'Item 24', 16, 24),
(N'Medicine', N'Item 25', 3, 25),
(N'Clothes', N'Item 26', 9, 26),
(N'Food', N'Item 27', 6, 27),
(N'Books', N'Item 28', 11, 28),
(N'Toys', N'Item 29', 8, 29),
(N'Medicine', N'Item 30', 5, 30);

INSERT INTO Events (E_Date, E_participants, E_Location, A_ID, O_ID) VALUES
('2024-01-10', 50, N'Location 01', 1, 1),
('2024-01-20', 80, N'Location 02', 2, 2),
('2024-02-05', 40, N'Location 03', 3, 3),
('2024-02-18', 65, N'Location 04', 4, 4),
('2024-03-03', 30, N'Location 05', 5, 5),
('2024-03-22', 90, N'Location 06', 6, 6),
('2024-04-06', 55, N'Location 07', 7, 7),
('2024-04-19', 70, N'Location 08', 8, 8),
('2024-05-02', 45, N'Location 09', 9, 9),
('2024-05-21', 85, N'Location 10', 10, 10),
('2024-06-04', 60, N'Location 11', 11, 11),
('2024-06-23', 35, N'Location 12', 12, 12),
('2024-07-07', 75, N'Location 13', 13, 13),
('2024-07-24', 50, N'Location 14', 14, 14),
('2024-08-08', 95, N'Location 15', 15, 15),
('2024-08-25', 42, N'Location 16', 16, 16),
('2024-09-09', 68, N'Location 17', 17, 17),
('2024-09-26', 58, N'Location 18', 18, 18),
('2024-10-10', 77, N'Location 19', 19, 19),
('2024-10-27', 33, N'Location 20', 20, 20),
('2024-11-11', 88, N'Location 21', 21, 21),
('2024-11-28', 46, N'Location 22', 22, 22),
('2024-12-12', 62, N'Location 23', 23, 23),
('2024-12-29', 53, N'Location 24', 24, 24),
('2025-01-13', 71, N'Location 25', 25, 25),
('2025-01-30', 49, N'Location 26', 26, 26),
('2025-02-14', 82, N'Location 27', 27, 27),
('2025-03-01', 57, N'Location 28', 28, 28),
('2025-03-16', 66, N'Location 29', 29, 29),
('2025-03-31', 44, N'Location 30', 30, 30);

INSERT INTO Mealplan (MP_meal, O_ID) VALUES
(N'Plan 01', 1),
(N'Plan 02', 2),
(N'Plan 03', 3),
(N'Plan 04', 4),
(N'Plan 05', 5),
(N'Plan 06', 6),
(N'Plan 07', 7),
(N'Plan 08', 8),
(N'Plan 09', 9),
(N'Plan 10', 10),
(N'Plan 11', 11),
(N'Plan 12', 12),
(N'Plan 13', 13),
(N'Plan 14', 14),
(N'Plan 15', 15),
(N'Plan 16', 16),
(N'Plan 17', 17),
(N'Plan 18', 18),
(N'Plan 19', 19),
(N'Plan 20', 20),
(N'Plan 21', 21),
(N'Plan 22', 22),
(N'Plan 23', 23),
(N'Plan 24', 24),
(N'Plan 25', 25),
(N'Plan 26', 26),
(N'Plan 27', 27),
(N'Plan 28', 28),
(N'Plan 29', 29),
(N'Plan 30', 30);

INSERT INTO Served_to VALUES
(1,1), (2,2), (3,3), (4,4), (5,5),
(6,6), (7,7), (8,8), (9,9), (10,10),
(11,11), (12,12), (13,13), (14,14), (15,15),
(16,16), (17,17), (18,18), (19,19), (20,20),
(21,21), (22,22), (23,23), (24,24), (25,25),
(26,26), (27,27), (28,28), (29,29), (30,30);


--- auth

ALTER TABLE Admin ADD apass NVARCHAR(50) NULL;
ALTER TABLE Sponsor ADD spass NVARCHAR(50) NULL;

UPDATE Admin
SET apass = CONCAT('a', CAST(A_ID AS varchar(12)))
WHERE apass IS NULL OR LTRIM(RTRIM(apass)) = '';

UPDATE Sponsor
SET spass = CONCAT('s', CAST(S_ID AS varchar(12)))
WHERE spass IS NULL OR LTRIM(RTRIM(spass)) = '';
