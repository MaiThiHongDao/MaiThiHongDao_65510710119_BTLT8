CREATE DATABASE BaiTapEFCore;
GO
USE BaiTapEFCore;
GO
/* ---------- Câu 1: Tri Thức Books ---------- */
CREATE TABLE TheLoaiSach(
    MaTL INT IDENTITY(1,1) PRIMARY KEY,
    TenTheLoai NVARCHAR(100) NOT NULL UNIQUE,
    MoTa NVARCHAR(255) NULL,
    SoLuongSach INT NULL DEFAULT 0,
    NgayTao DATETIME NULL DEFAULT GETDATE()
);
GO
-- Bảng Sach (chỉ để minh họa lỗi ràng buộc khóa ngoại khi xóa thể loại)
CREATE TABLE Sach(
    MaSach INT IDENTITY(1,1) PRIMARY KEY,
    TenSach NVARCHAR(200) NOT NULL,
    MaTL INT NOT NULL REFERENCES TheLoaiSach(MaTL)
);
GO
INSERT INTO TheLoaiSach(TenTheLoai,MoTa,SoLuongSach) VALUES
(N'Tiểu thuyết',N'Thân tiểu thuyết',1),
(N'Kỹ năng sống',N'Kỹ năng sống năng sống',15),
(N'Thiếu nhi',N'Thiếu nhi củ cỏ',2),
(N'Sách giáo khoa',N'Sách giáo khoa chỉ đương phộp hơi vào tạo',2),
(N'Truyện tranh',N'Truyện tranh',1);
INSERT INTO Sach(TenSach,MaTL) VALUES (N'Dế Mèn phiêu lưu ký',1),(N'Đắc Nhân Tâm',2);
GO
/* ---------- Câu 2: Gym FitZone ---------- */
CREATE TABLE HoiVien(
    MaHV INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    GioiTinh BIT NULL,              -- 1=Nam, 0=Nữ
    NgaySinh DATE NULL,
    SDT VARCHAR(15) NULL,
    Email VARCHAR(100) NULL,
    HangThanhVien NVARCHAR(20) NULL, -- Basic/VIP/Premium
    NgayDangKy DATETIME NULL DEFAULT GETDATE(),
    TrangThai BIT NULL              -- 1=Đang hoạt động, 0=Tạm ngưng
);
GO
INSERT INTO HoiVien(HoTen,GioiTinh,NgaySinh,SDT,Email,HangThanhVien,TrangThai) VALUES
(N'Họ tên Vớn',1,'1999-11-07','07382735879','von@gmail.com',N'Basic',1),
(N'Nguyễn Ninh',1,'1999-11-10','07382731034','ninh@gmail.com',N'Premium',1),
(N'Nguyễn Bộ Dôn',1,'1999-12-29','07882773727','don@gmail.com',N'Basic',1),
(N'Nguyễn Xim',1,'1999-11-17','07375856597','xim@gmail.com',N'VIP',1),
(N'Nguyễn Tương',0,'1999-01-22','07887775931','tuong@gmail.com',N'Premium',1);
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
