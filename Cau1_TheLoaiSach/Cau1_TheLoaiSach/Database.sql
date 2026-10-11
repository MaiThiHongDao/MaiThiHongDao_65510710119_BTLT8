CREATE DATABASE TriThucBooksDB;
GO
USE TriThucBooksDB;
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
