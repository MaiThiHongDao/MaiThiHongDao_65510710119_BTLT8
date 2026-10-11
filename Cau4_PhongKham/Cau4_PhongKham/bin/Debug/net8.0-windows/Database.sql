CREATE DATABASE AnKhangClinicDB;
GO
USE AnKhangClinicDB;
GO
/* ---------- Câu 4: An Khang Clinic ---------- */
CREATE TABLE BacSi(
    MaBS INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    ChuyenKhoa NVARCHAR(100) NULL,
    SDT VARCHAR(15) NULL
);
CREATE TABLE LichKham(
    MaLich INT IDENTITY(1,1) PRIMARY KEY,
    TenBenhNhan NVARCHAR(100) NOT NULL,
    SDT VARCHAR(15) NULL,
    NgayKham DATE NULL,
    GioKham TIME(0) NULL,
    MaBS INT NULL REFERENCES BacSi(MaBS),
    TrangThai NVARCHAR(20) NULL     -- Chờ khám/Đã khám/Đã hủy
);
GO
INSERT INTO BacSi(HoTen,ChuyenKhoa,SDT) VALUES
(N'Nguyễn Văn A',N'Nội tổng quát','0901111111'),
(N'Nguyễn Văn B',N'Nhi khoa','0902222222'),
(N'Nguyễn Văn C',N'Da liễu','0903333333'),
(N'Nguyễn Văn D',N'Tai mũi họng','0904444444');
INSERT INTO LichKham(TenBenhNhan,SDT,NgayKham,GioKham,MaBS,TrangThai) VALUES
(N'Trần Minh Anh','09725667894','2026-10-20','09:00',1,N'Chờ khám'),
(N'Nguyễn Xinh','09725667897','2026-10-12','13:00',1,N'Đã khám'),
(N'Nguyễn Hạm','09725667899','2026-10-13','16:00',2,N'Đã hủy'),
(N'Nguyễn Tính','09725667830','2026-10-14','16:00',3,N'Chờ khám'),
(N'Nguyễn Hoan Anh','09725667899','2026-10-15','18:00',4,N'Chờ khám');
GO
