CREATE DATABASE FitZoneDB;
GO
USE FitZoneDB;
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
