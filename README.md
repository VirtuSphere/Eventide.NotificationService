# Eventide.NotificationService

[![.NET CI](https://github.com/VirtuSphere/Eventide.NotificationService/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/VirtuSphere/Eventide.NotificationService/actions/workflows/dotnet-ci.yml)

Сервис уведомлений для создания, хранения и управления уведомлениями пользователей.

Проект реализован на .NET 8 с PostgreSQL и Entity Framework Core и построен по принципам слоистой/чистой архитектуры.

## Быстрый старт

Требуется .NET SDK 8.0 и PostgreSQL.

```powershell
dotnet restore Eventide.NotificationService.sln
dotnet build Eventide.NotificationService.sln
dotnet run --project Presentation/NotificationService.WebHost/NotificationService.WebHost.csproj
```

После запуска Swagger UI доступен по адресу:

- `http://localhost:<порт>/swagger`
- точный порт определяется конфигурацией launch-профиля Visual Studio или выводом приложения

Строка подключения задаётся в `Presentation/NotificationService.WebHost/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ApplicationDbContext": "Host=localhost;Port=5433;Database=NotificationServiceDb;UserName=postgres;Password=1234;"
  }
}
```

При запуске приложение автоматически применяет миграции EF Core.

## Основные возможности

- CRUD-пользователи;
- CRUD-администраторы;
- CRUD-связанные сущности;
- CRUD-уведомления;
- фильтрация уведомлений по пользователю или администратору;
- выборка только непрочитанных уведомлений;
- подсчёт непрочитанных уведомлений;
- установка и снятие состояния «прочитано»;
- создание и применение миграций базы данных при старте.

## Документация

Подробная документация, схема данных, описание API и правила разработки находятся в [docs/PROJECT_DOCUMENTATION.md](docs/PROJECT_DOCUMENTATION.md).
