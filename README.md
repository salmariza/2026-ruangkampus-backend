# RuangKampus Backend

Backend API Sistem Peminjaman Ruangan Kampus.

## Tech Stack
- ASP.NET Web API (.NET 10)
- Entity Framework Core
- SQLite
- Swagger

## Features
- Room CRUD
- RoomBooking CRUD
- Booking status: Pending/Approved/Rejected
- Filter booking: by room, status, date
- Conflict validation: prevent overlapping approved bookings
- Seed initial rooms via EF Core migration

## Setup
```bash
dotnet restore
dotnet ef database update
dotnet run
