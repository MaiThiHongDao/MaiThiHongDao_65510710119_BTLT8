CREATE DATABASE SunriseHomestayDB;
GO
USE SunriseHomestayDB;
GO
/* ---------- Câu 3: Sunrise Homestay ---------- */
CREATE TABLE LoaiPhong(
    MaLoai INT IDENTITY(1,1) PRIMARY KEY,
    TenLoai NVARCHAR(100) NOT NULL,
    GiaMoiDem DECIMAL(18,2) NULL,
    MoTa NVARCHAR(255) NULL
);
CREATE TABLE Phong(
    MaPhong INT IDENTITY(1,1) PRIMARY KEY,
    SoPhong VARCHAR(10) NOT NULL,
    TangSo INT NULL,
    TinhTrang NVARCHAR(20) NULL,    -- Trống/Đang ở/Đang dọn
    HinhAnh NVARCHAR(255) NULL,     -- chỉ lưu tên file
    MaLoai INT NULL REFERENCES LoaiPhong(MaLoai)
);
GO
INSERT INTO LoaiPhong(TenLoai,GiaMoiDem,MoTa) VALUES
(N'Phòng Đơn',350000,N'1 giường đơn'),
(N'Phòng Đôi',500000,N'1 giường đôi'),
(N'Phòng VIP',900000,N'Phòng cao cấp, view đẹp');
INSERT INTO Phong(SoPhong,TangSo,TinhTrang,HinhAnh,MaLoai) VALUES
('101',1,N'Trống','phong1.png',1),
('102',1,N'Đang ở','phong2.png',2),
('201',2,N'Đang dọn','phong3.png',1),
('202',2,N'Đang dọn','phong2.png',2),
('301',3,N'Đang dọn','phong4.png',3);
GO
