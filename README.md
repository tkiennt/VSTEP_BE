# VSTEP Writing System - Backend

Backend API built with ASP.NET Core 8.0 using Clean Architecture.

## Architecture

The project follows Clean Architecture principles with the following layers:

- **Domain**: Core entities and enums (no dependencies)
- **Application**: Business logic, interfaces, DTOs, and services
- **Infrastructure**: Data access and external service implementations
- **API**: Controllers and API configuration

## Authentication & Authorization (IAM)

- **JWT**: Access token để xác thực API (Bearer token trong header `Authorization`).
- **Password**: Mã hóa bằng **BCrypt** (hash + salt), không lưu plain text.
- **Forgot / Reset password**: Token reset lưu trong bảng `password_reset_tokens`, hết hạn sau 24h.

### Roles

- **Guest (0)**: Default role, limited access
- **User (1)**: Standard user access
- **Manager (2)**: Manager-level access
- **Admin (3)**: Full system access

### API Endpoints

#### Authentication
- `POST /api/auth/login` - Login (username, password) → JWT + user info
- `POST /api/auth/register` - Đăng ký (name, username, email, password, role)
- `POST /api/auth/validate` - Validate JWT (requires auth)
- `POST /api/auth/forgot-password` - Gửi yêu cầu đặt lại mật khẩu (body: email). Token lưu DB; production nên gửi link qua email.
- `POST /api/auth/reset-password` - Đặt lại mật khẩu bằng token (body: token, newPassword)
- `POST /api/auth/change-password` - Đổi mật khẩu khi đã đăng nhập (body: currentPassword, newPassword; requires auth)

#### Health & DB test (không cần auth)
- `GET /api/ping` - Ping API (backend đang chạy)
- `GET /api/health` - Trạng thái tổng: status, environment, database
- `GET /api/health/db` - Kiểm tra kết nối MySQL (elapsed ms, userCount)

#### User (requires auth)
- `GET /api/user/profile` - Lấy profile từ DB (theo JWT)
- `PUT /api/user/profile` - Cập nhật name, email, target_level_id
- `GET /api/user/user-only` - Policy UserOrAbove
- `GET /api/user/manager-only` - Policy ManagerOrAdmin
- `GET /api/user/admin-only` - Policy AdminOnly

#### Lookup (public)
- `GET /api/levels` - Danh sách levels
- `GET /api/levels/{id}` - Chi tiết level
- `GET /api/part-types` - Danh sách part types
- `GET /api/part-types/{id}` - Chi tiết part type
- `GET /api/practice-modes` - Danh sách practice modes
- `GET /api/practice-modes/{id}` - Chi tiết practice mode

#### Exam & Content (CRUD)
- `GET/POST/PUT/DELETE /api/exam-structures` - CRUD cấu trúc đề (POST/PUT/DELETE: Manager/Admin)
- `GET /api/parts/by-exam/{examStructureId}` - Parts theo exam structure
- `GET/POST/PUT/DELETE /api/parts/{id}` - CRUD part (POST/PUT: ManagerOrAdmin, DELETE: Admin)
- `GET /api/topics/by-part/{partId}` - Topics theo part
- `GET/POST/PUT/DELETE /api/topics/{id}` - CRUD topic (POST/PUT: ManagerOrAdmin, DELETE: Admin)

#### Practice (theo user đăng nhập)
- `GET /api/practice-sessions/my` - Danh sách session của user
- `GET /api/practice-sessions/{id}` - Chi tiết session (chỉ chủ session)
- `POST /api/practice-sessions` - Tạo session (body: modeId, isRandom)
- `GET /api/user-submissions/by-session/{sessionId}` - Submissions theo session (chỉ chủ session)
- `GET /api/user-submissions/{id}` - Chi tiết submission
- `POST /api/user-submissions` - Tạo submission (body: sessionId, topicId, partId, content, wordCount?, enableHint)

### Authorization Policies

- `AdminOnly`: Requires Admin role
- `ManagerOrAdmin`: Requires Manager or Admin role
- `UserOrAbove`: Requires User, Manager, or Admin role
- `Authenticated`: Requires any authenticated user

## Configuration

JWT settings are configured in `appsettings.json`:

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!ChangeThisInProduction!",
    "Issuer": "VSTEP_Writing_System",
    "Audience": "VSTEP_Writing_System",
    "ExpirationMinutes": "1440"
  }
}
```

**Important**: Change the `SecretKey` in production!

## Running the Application

1. Navigate to the API project:
   ```bash
   cd Backend/API
   ```

2. Restore packages:
   ```bash
   dotnet restore
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

The API will be available at:
- HTTP: `http://localhost:5268`
- HTTPS: `https://localhost:7061`
- Swagger UI: `http://localhost:5268/swagger`

## CORS

CORS is configured to allow requests from `http://localhost:3000` (Next.js frontend).

## Database

The project uses **MySQL** với schema 3NF (database **`vstep_writing`**). EF Core kết nối qua **Pomelo.EntityFrameworkCore.MySql**.

### 1. Tạo database bằng script SQL

Chạy script trong MySQL (hoặc MySQL Workbench / dòng lệnh) **trước khi chạy API**:

```bash
mysql -u root -p < Scripts/vstep_writing_3nf.sql
```

Hoặc mở file `Scripts/vstep_writing_3nf.sql` và chạy toàn bộ trong MySQL Client. Script sẽ:

- Tạo database **`vstep_writing`** (utf8mb4)
- Tạo đầy đủ bảng: levels, part_types, practice_modes, hint_types, prompt_purposes, sample_types, users, **password_reset_tokens**, practice_sessions, exam_structures, parts, topics, ...
- Bảng `users`: username, password_hash, role, updated_at, is_active (auth API).
- Seed: 1 level (B1), 2 part_types, 3 practice_modes.
- Nếu DB đã tạo trước đó và chưa có bảng reset password: chạy thêm `Scripts/add_password_reset_tokens.sql`.

### 2. Connection string

Cấu hình trong `API/appsettings.json` hoặc `API/appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=vstep_writing;Uid=root;Pwd=YOUR_PASSWORD;CharSet=utf8mb4;"
}
```

Sửa `Uid`, `Pwd` (và `Server`/`Port` nếu cần) cho đúng môi trường MySQL của bạn.

### 3. Chạy ứng dụng

Sau khi đã tạo database bằng script, chạy API như bình thường. Ứng dụng **không** dùng EF migrations; schema do script SQL quản lý.

### Cấu trúc bảng users (map với API auth)

- user_id, name, email, target_level_id, created_at, **username**, **password_hash**, **role**, updated_at, is_active

## Dependencies

- **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT authentication
- **BCrypt.Net-Next**: Password hashing
- **Swashbuckle.AspNetCore**: Swagger/OpenAPI documentation
- **Pomelo.EntityFrameworkCore.MySql**: MySQL provider cho EF Core

## Notes

- User data is stored in MySQL (database **vstep_writing**); tạo DB bằng script `Scripts/vstep_writing_3nf.sql`.
- **Mật khẩu**: BCrypt hash (không lưu plain text). **JWT** dùng cho access token (Bearer), hết hạn theo `Jwt:ExpirationMinutes` (mặc định 24h).
- Forgot password: token reset lưu trong `password_reset_tokens`, hết hạn 24h. Production nên tích hợp gửi email chứa link reset.

