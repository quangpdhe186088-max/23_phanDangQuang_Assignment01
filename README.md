# 23_phanDangQuang_Assignment01

FUNewsManagementSystem — bài tập PRN232, ASP.NET Core MVC Frontend và Web API Backend (.NET 8, SQL Server, OData).

## Chạy Frontend và Backend cùng lúc trong Visual Studio

1. Mở `23_phanDangQuang_Assignment01.sln` bằng Visual Studio 2022 17.11 trở lên.
2. Nếu chưa có profile nhiều project, vào **Tools > Options > Environment > Preview Features**, bật **Enable Multi-Project Launch Profiles**. Đóng và mở lại solution nếu cần.
3. Chọn **Frontend + Backend** trong danh sách cạnh nút Start. Profile có sẵn trong `23_phanDangQuang_Assignment01.slnLaunch` và chạy cả hai project bằng profile `http`.
4. Bấm **Start / F5**. Backend khởi động trước, Frontend khởi động cùng phiên debug; Stop kết thúc cả hai.

Nếu danh sách chưa hiện profile, nhấp phải solution > **Configure Startup Projects** > **Multiple startup projects**; đặt cả hai project **Start**, Debug Target **http**, Backend nằm trước Frontend.

- Frontend: http://localhost:5259/
- Backend Swagger: http://localhost:5278/swagger
- Frontend Development gọi Backend tại http://localhost:5278/.

Hướng dẫn Microsoft: https://learn.microsoft.com/en-us/visualstudio/ide/how-to-set-multiple-startup-projects

## Thiết lập lần đầu trên máy khác

- Cài .NET 8 SDK hoặc workload ASP.NET and web development trong Visual Studio.
- Chuẩn bị database `FUNewsManagement` theo SQL được cung cấp trong đề bài. Repository không kèm database hoặc tự tạo/migration database bài tập.
- Đặt `ConnectionStrings:FUNewsManagement` phù hợp máy hiện tại. Chuỗi hiện có dùng Windows Authentication, không chứa mật khẩu SQL.
- Tạo khóa JWT riêng bằng User Secrets, không đưa khóa lên Git. Có thể chạy trong PowerShell tại thư mục gốc dự án:

```powershell
$jwtBytes = New-Object byte[] 48
[System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($jwtBytes)
dotnet user-secrets set 'Jwt:SigningKey' ([Convert]::ToBase64String($jwtBytes)) --project .\23_phanDangQuang_Assignment01_BackEnd
```

Khóa User Secrets trên máy đang phát triển được giữ ngoài source. `AdminAccount` trong Backend appsettings.json chứa tài khoản mẫu theo đề bài: `admin@FUNewsManagementSystem.org`, mật khẩu `@@abc123@@`. Tài khoản database: role 0 = Admin, role 1 = Staff, role 2 = Lecturer.

## Chạy bằng terminal

Mở hai terminal tại thư mục gốc:

```powershell
dotnet run --project .\23_phanDangQuang_Assignment01_BackEnd --launch-profile http
```

```powershell
dotnet run --project .\23_phanDangQuang_Assignment01_FrontEnd --launch-profile http
```

## Chức năng

- Đăng nhập, đăng xuất, phân quyền bằng JWT Backend và cookie/session MVC.
- Admin quản lý tài khoản và báo cáo theo khoảng ngày tạo bài.
- Staff quản lý danh mục, bài viết, tag; sửa hồ sơ và xem lịch sử bài mình tạo.
- Lecturer xem, sửa, xóa bài do mình tạo.
- Khách đọc tin active không cần đăng nhập.
- Validation, tìm kiếm/OData, phân trang và điều kiện chặn xóa theo dữ liệu liên quan.

## Kiểm tra build

```powershell
dotnet build .\23_phanDangQuang_Assignment01.sln
```

Các thư mục `tests` và `TestResults` đã được loại khỏi bản source hiện tại. `.gitignore` loại thư mục build, dữ liệu Visual Studio, cấu hình người dùng và khóa riêng khỏi Git.
