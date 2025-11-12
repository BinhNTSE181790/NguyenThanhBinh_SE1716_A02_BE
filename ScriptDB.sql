IF DB_ID('FUNewsManagementSystemDB') IS NOT NULL
    DROP DATABASE FUNewsManagementSystemDB;
GO

CREATE DATABASE FUNewsManagementSystemDB;
GO

USE FUNewsManagementSystemDB;
GO

-- =======================================================
-- 1️⃣ BẢNG SYSTEMACCOUNT
-- =======================================================
CREATE TABLE SystemAccount (
    AccountID INT PRIMARY KEY IDENTITY(1,1),
    AccountName NVARCHAR(100) NOT NULL,
    AccountEmail VARCHAR(100) UNIQUE NOT NULL,
    AccountRole INT NOT NULL,               -- 1=Staff, 2=Lecturer, Admin lấy từ cấu hình hệ thống
    AccountPassword VARCHAR(255) NOT NULL
);
GO

-- =======================================================
-- 2️⃣ BẢNG CATEGORY
-- =======================================================
CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    CategoryDesciption NVARCHAR(500),
    ParentCategoryID INT NULL,
    Status INT NOT NULL DEFAULT 1,          -- 1 = Active, 0 = Inactive

    CONSTRAINT FK_Category_ParentCategory FOREIGN KEY (ParentCategoryID)
        REFERENCES Category (CategoryID)
);
GO

-- =======================================================
-- 3️⃣ BẢNG NEWSARTICLE
-- =======================================================
CREATE TABLE NewsArticle (
    NewsArticleID INT PRIMARY KEY IDENTITY(1,1),
    NewsTitle NVARCHAR(200) NOT NULL,
    Headline NVARCHAR(500),
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    NewsContent NTEXT NOT NULL,
    NewsSource NVARCHAR(200),
    CategoryID INT NOT NULL,
    NewsStatus INT NOT NULL DEFAULT 0,      -- 1 = Published, 0 = Draft
    CreatedByID INT NOT NULL,
    UpdatedByID INT NULL,
    ModifiedDate DATETIME NULL,

    CONSTRAINT FK_NewsArticle_Category FOREIGN KEY (CategoryID)
        REFERENCES Category (CategoryID),

    CONSTRAINT FK_NewsArticle_CreatedBy FOREIGN KEY (CreatedByID)
        REFERENCES SystemAccount (AccountID),

    CONSTRAINT FK_NewsArticle_UpdatedBy FOREIGN KEY (UpdatedByID)
        REFERENCES SystemAccount (AccountID)
);
GO

-- =======================================================
-- 4️⃣ BẢNG TAG
-- =======================================================
CREATE TABLE Tag (
    TagID INT PRIMARY KEY IDENTITY(1,1),
    TagName NVARCHAR(50) UNIQUE NOT NULL,
    Note NVARCHAR(255)
);
GO

-- =======================================================
-- 5️⃣ BẢNG NEWSTAG (QUAN HỆ N-N)
-- =======================================================
CREATE TABLE NewsTag (
    NewsArticleID INT NOT NULL,
    TagID INT NOT NULL,
    PRIMARY KEY (NewsArticleID, TagID),

    CONSTRAINT FK_NewsTag_NewsArticle FOREIGN KEY (NewsArticleID)
        REFERENCES NewsArticle (NewsArticleID)
        ON DELETE CASCADE,

    CONSTRAINT FK_NewsTag_Tag FOREIGN KEY (TagID)
        REFERENCES Tag (TagID)
        ON DELETE CASCADE
);
GO

-- =======================================================
-- 6️⃣ DỮ LIỆU MẪU (CHUẨN HÓA STATUS)
-- =======================================================

-- SystemAccount
SET IDENTITY_INSERT SystemAccount ON;
INSERT INTO SystemAccount (AccountID, AccountName, AccountEmail, AccountRole, AccountPassword)
VALUES
(2, N'Staff 1', 'staff1@fpt.edu.vn', 1, 'staff123'),
(3, N'Lecturer A', 'lecturerA@fpt.edu.vn', 2, 'lecturer123'),
(4, N'Staff 2', 'staff2@fpt.edu.vn', 1, 'staff456');
SET IDENTITY_INSERT SystemAccount OFF;
GO

-- Category
SET IDENTITY_INSERT Category ON;
INSERT INTO Category (CategoryID, CategoryName, CategoryDesciption, ParentCategoryID, Status)
VALUES
(1, N'Tuyển Sinh', N'Thông tin tuyển sinh', NULL, 1),
(2, N'Học Thuật', N'Thông tin về học tập, nghiên cứu', NULL, 1),
(3, N'Hoạt Động Sinh Viên', N'Các hoạt động ngoại khóa', NULL, 1),
(4, N'Thông báo chung', N'Thông báo nội bộ trường', 2, 1),
(5, N'Học Bổng', N'Thông tin các loại học bổng', 2, 1);
SET IDENTITY_INSERT Category OFF;
GO

-- Tag
SET IDENTITY_INSERT Tag ON;
INSERT INTO Tag (TagID, TagName, Note)
VALUES
(1, N'Tuyển sinh', N'Tin tức, thông báo về tuyển sinh đại học, cao học'),
(2, N'Học bổng', N'Thông tin về các loại học bổng, hỗ trợ tài chính cho sinh viên'),
(3, N'Sự kiện', N'Hội thảo, workshop, lễ hội, lễ khai giảng, tốt nghiệp...'),
(4, N'Nghiên cứu', N'Tin tức về hoạt động nghiên cứu khoa học, seminar học thuật'),
(5, N'Thể thao', N'Các giải đấu, hoạt động thể thao của sinh viên, cán bộ'),
(6, N'Tình nguyện', N'Các chương trình thiện nguyện, hoạt động cộng đồng'),
(7, N'Thông báo', N'Thông báo học vụ, lịch thi, nghỉ học, quy định mới'),
(8, N'Cựu sinh viên', N'Tin tức và sự kiện liên quan đến cựu sinh viên'),
(9, N'Công nghệ', N'Bài viết, hội thảo, tin tức về lĩnh vực công nghệ, IT'),
(10, N'Tuyển dụng', N'Cơ hội việc làm, thực tập cho sinh viên và cựu sinh viên');
SET IDENTITY_INSERT Tag OFF;
GO

-- NewsArticle
SET IDENTITY_INSERT NewsArticle ON;
INSERT INTO NewsArticle (
    NewsArticleID, NewsTitle, Headline, CreatedDate, NewsContent, NewsSource,
    CategoryID, NewsStatus, CreatedByID, UpdatedByID, ModifiedDate
)
VALUES
(1, N'Khai giảng Khóa 18', N'Thông báo khai giảng chính thức cho Khóa 18',
 '2025-10-25 09:00:00', N'Chi tiết về lễ khai giảng...', N'FPTU Website',
 1, 1, 2, NULL, NULL),

(2, N'Quy chế bảo vệ đồ án', N'Các quy định mới về bảo vệ đồ án tốt nghiệp',
 '2025-10-26 10:30:00', N'Bao gồm các tiêu chí đánh giá...', N'Phòng Đào tạo',
 2, 1, 3, NULL, NULL),

(3, N'Giải bóng đá sinh viên 2025', N'Thông báo tổ chức giải bóng đá thường niên',
 '2025-10-27 15:00:00', N'Đăng ký tại phòng Công tác Sinh viên...', N'Đoàn trường',
 3, 0, 4, 4, '2025-10-28 11:00:00'),

(4, N'Cảnh báo học vụ', N'Danh sách sinh viên bị cảnh báo học vụ',
 '2025-10-28 08:00:00', N'Vui lòng kiểm tra email để biết chi tiết...', N'Phòng Đào tạo',
 4, 1, 2, NULL, NULL);
SET IDENTITY_INSERT NewsArticle OFF;
GO

-- NewsTag
INSERT INTO NewsTag (NewsArticleID, TagID)
VALUES
(1, 2),  -- Khai giảng ↔ Sự kiện
(2, 1),  -- Quy chế đồ án ↔ Kỳ thi
(3, 2),  -- Giải bóng đá ↔ Sự kiện
(3, 3);  -- Giải bóng đá ↔ Bóng đá
GO

-- =======================================================
-- KIỂM TRA KẾT QUẢ
-- =======================================================
SELECT * FROM SystemAccount;
SELECT * FROM Category;
SELECT * FROM Tag;
SELECT * FROM NewsArticle;
SELECT * FROM NewsTag;
GO

--------------------------------------------------------
-- 1️⃣  THÊM 3 CATEGORY MỚI
--------------------------------------------------------
INSERT INTO Category (CategoryName, CategoryDesciption, ParentCategoryID) VALUES
(N'Cựu Sinh Viên', N'Tin tức, sự kiện và kết nối với cựu sinh viên', NULL),
(N'Tuyển Dụng', N'Cơ hội việc làm, thực tập cho sinh viên và cựu sinh viên', NULL),
(N'Nghiên Cứu', N'Các hoạt động nghiên cứu khoa học và seminar học thuật', 2); -- Con của Học Thuật
GO

--------------------------------------------------------
-- 2️⃣  THÊM 10 BÀI VIẾT MỚI VÀO BẢNG NEWSARTICLE
--------------------------------------------------------
INSERT INTO NewsArticle
(NewsTitle, Headline, CreatedDate, NewsContent, NewsSource, CategoryID, NewsStatus, CreatedByID, UpdatedByID, ModifiedDate)
VALUES
(N'Workshop Kỹ năng phỏng vấn thành công', N'Hướng dẫn sinh viên chuẩn bị phỏng vấn tuyển dụng', GETDATE(), 
 N'Buổi workshop chia sẻ kỹ năng phỏng vấn với nhà tuyển dụng FPT Software.', 
 N'Trung tâm Hỗ trợ việc làm', 7, 1, 2, NULL, NULL),

(N'Cựu sinh viên FPTU chia sẻ kinh nghiệm khởi nghiệp', N'Câu chuyện khởi nghiệp từ cựu sinh viên ngành CNTT', GETDATE(), 
 N'Anh Nguyễn Văn B – CEO của Startup ABC – chia sẻ hành trình khởi nghiệp sau khi tốt nghiệp FPTU.', 
 N'Phòng Truyền thông', 6, 1, 3, NULL, NULL),

(N'Thông báo lịch nghỉ Tết Dương lịch 2026', N'Lịch nghỉ và kế hoạch học bù cho sinh viên toàn trường', GETDATE(),
 N'Sinh viên được nghỉ từ ngày 31/12/2025 đến hết 2/1/2026, học bù bắt đầu từ 5/1.', 
 N'Phòng Đào tạo', 4, 1, 4, NULL, NULL),

(N'Chung kết cuộc thi Lập trình Sinh viên 2025', N'Các đội tranh tài tại vòng chung kết lập trình cấp trường', GETDATE(),
 N'Sự kiện diễn ra tại hội trường Innovation Hub với sự tham gia của 20 đội.', 
 N'Đoàn trường', 2, 1, 2, NULL, NULL),

(N'FPTU đạt giải thưởng quốc tế về đổi mới sáng tạo', N'Thành tích nổi bật trong lĩnh vực giáo dục sáng tạo', GETDATE(),
 N'Trường Đại học FPT được vinh danh tại giải thưởng Giáo dục sáng tạo khu vực châu Á.', 
 N'FPTU News', 2, 1, 3, NULL, NULL),

(N'Giải chạy vì cộng đồng – FPTU Run 2025', N'Sự kiện thể thao gây quỹ từ thiện', GETDATE(),
 N'Hơn 1000 sinh viên tham gia giải chạy gây quỹ “Vì Trái Tim Xanh”.', 
 N'Phòng Công tác sinh viên', 3, 1, 4, NULL, NULL),

(N'Thông báo học bổng học kỳ Spring 2026', N'Dành cho sinh viên có thành tích học tập xuất sắc', GETDATE(),
 N'Sinh viên đạt GPA trên 3.6 có thể nộp hồ sơ xét học bổng học kỳ mới.', 
 N'Phòng Tài chính', 5, 1, 2, NULL, NULL),

(N'Hội thảo nghiên cứu khoa học 2025', N'Cơ hội cho sinh viên trình bày đề tài nghiên cứu', GETDATE(),
 N'Hội thảo sẽ diễn ra vào 12/12/2025 với sự góp mặt của nhiều giảng viên.', 
 N'Phòng Nghiên cứu khoa học', 8, 1, 3, NULL, NULL),

(N'Tuyển sinh chương trình Thạc sĩ CNTT 2026', N'Mở rộng cơ hội học tập sau đại học cho sinh viên', GETDATE(),
 N'Chương trình Thạc sĩ CNTT sẽ tuyển sinh đợt đầu vào tháng 3/2026.', 
 N'Phòng Sau đại học', 1, 1, 4, NULL, NULL),

(N'Tuần lễ Văn hóa Sinh viên 2025', N'Chuỗi hoạt động văn hóa – nghệ thuật cho sinh viên', GETDATE(),
 N'Sự kiện kéo dài 5 ngày với các hoạt động văn nghệ, hội trại và giao lưu văn hóa.', 
 N'Đoàn Thanh niên', 3, 1, 2, NULL, NULL);
GO

--------------------------------------------------------
-- 3️⃣  GẮN TAG CHO 10 BÀI VIẾT (NewsTag)
--------------------------------------------------------
-- Giả sử TagID:
-- 1: Tuyển sinh | 2: Học bổng | 3: Sự kiện | 4: Nghiên cứu | 5: Thể thao
-- 6: Tình nguyện | 7: Thông báo | 8: Cựu sinh viên | 9: Công nghệ | 10: Tuyển dụng

DECLARE @StartID INT = (SELECT MAX(NewsArticleID) - 9 FROM NewsArticle); -- Lấy ID bắt đầu của 10 bài mới

INSERT INTO NewsTag (NewsArticleID, TagID) VALUES
(@StartID, 10),     -- Workshop phỏng vấn → Tuyển dụng
(@StartID + 1, 8),  -- Cựu sinh viên → Cựu sinh viên
(@StartID + 2, 7),  -- Lịch nghỉ → Thông báo
(@StartID + 3, 9),  -- Cuộc thi lập trình → Công nghệ
(@StartID + 4, 4),  -- Giải thưởng đổi mới → Nghiên cứu
(@StartID + 5, 5),  -- Giải chạy → Thể thao
(@StartID + 6, 2),  -- Học bổng → Học bổng
(@StartID + 7, 4),  -- Hội thảo nghiên cứu → Nghiên cứu
(@StartID + 8, 1),  -- Tuyển sinh Thạc sĩ → Tuyển sinh
(@StartID + 9, 3);  -- Tuần lễ Văn hóa → Sự kiện
GO

--------------------------------------------------------
-- 4️⃣  KIỂM TRA DỮ LIỆU
--------------------------------------------------------
SELECT * FROM Category ORDER BY CategoryID DESC;
SELECT * FROM NewsArticle ORDER BY NewsArticleID DESC;
SELECT * FROM NewsTag ORDER BY NewsArticleID, TagID;
GO
	