# Eventide.NotificationService: подробная документация

## 1. Назначение проекта

`Eventide.NotificationService` — REST-сервис для хранения и управления уведомлениями. Сервис связывает уведомление с получателем-пользователем, необязательным отправителем-администратором и необязательной связанной сущностью внешней подсистемы.

Типичные сценарии:

- создание уведомления для конкретного пользователя;
- получение всех уведомлений или выборка только непрочитанных;
- получение уведомлений, связанных с администраторами;
- отметка уведомления прочитанным или непрочитанным;
- подсчёт непрочитанных уведомлений;
- создание справочных сущностей и привязка их к уведомлению через идентификатор.

## 2. Технологический стек

| Компонент | Технология |
|---|---|
| Платформа | .NET 8 / ASP.NET Core |
| API | ASP.NET Core Controllers |
| База данных | PostgreSQL |
| ORM | Entity Framework Core 8 |
| Провайдер БД | Npgsql.EntityFrameworkCore.PostgreSQL 8.0.8 |
| Маппинг DTO | AutoMapper 17.0.0 |
| Документация API | Swashbuckle / Swagger UI |
| Тестирование | xUnit, Moq, coverlet |
| CI | GitHub Actions |

## 3. Структура решения

```text
Eventide.NotificationService/
├── Application/
│   ├── NotificationService.Application.Models/
│   │   ├── Administrator/
│   │   ├── Notification/
│   │   ├── RelaredEntity/
│   │   └── User/
│   └── NotificationService.Application.Services/
│       ├── Abstractions/
│       └── Mapping/
├── Domain/
│   ├── NotificationService.Domain/
│   ├── NotificationService.Domain.Repositories.Abstractions/
│   └── NotificationService.ValueObjects/
├── Infrastructure/
│   └── NotificationService.Infrastructure.EntityFramework/
├── Presentation/
│   └── NotificationService.WebHost/
├── Tests/
└── .github/workflows/dotnet-ci.yml
```

### 3.1 Domain

Содержит доменные сущности и бизнес-инварианты:

- `User` — пользователь с уникальным именем;
- `Administrator` — администратор с уникальным именем;
- `RelatedEntity` — сущность внешней подсистемы;
- `Notification` — уведомление, состояние чтения и временные метки;
- value objects: `Username`, `Title`, `Body`, `ExternalUrl`, `RelatedEntityDisplayName`, `RelatedEntityTypeName`;
- абстракции репозиториев.

Доменные сущности не зависят от ASP.NET и базы данных.

### 3.2 Application

Содержит модели запросов/ответов, интерфейсы application-сервисов и их реализации.

Application-сервисы:

- проверяют существование связанных сущностей;
- обеспечивают уникальность имён пользователей и администраторов;
- создают доменные объекты;
- преобразуют доменные сущности в DTO через AutoMapper;
- вызывают репозитории для чтения и сохранения данных.

### 3.3 Infrastructure

Реализует хранение через Entity Framework Core и PostgreSQL:

- `ApplicationDbContext`;
- конфигурации EF для таблиц и отношений;
- репозитории;
- миграции;
- первичную индексацию часто используемых колонок.

### 3.4 Presentation

Содержит:

- ASP.NET Core host;
- контроллеры REST API;
- настройку dependency injection;
- Swagger;
- `MigrationManager`, применяющий миграции при старте.

### 3.5 Tests

Проект `NotificationService.Tests` содержит unit-тесты доменной и прикладной логики.

Используются:

- xUnit для тестовых сценариев;
- Moq для имитации репозиториев;
- coverlet для сбора покрытия кода.

## 4. Архитектура и поток запроса

```text
HTTP-клиент
    |
    v
Controller
    |
    v
ApplicationService
    |
    +--> Mapper (Domain <-> DTO)
    |
    v
Repository
    |
    v
Entity Framework Core
    |
    v
PostgreSQL
```

Контроллеры не обращаются к `DbContext` напрямую. Сервисы используют интерфейсы репозиториев, а инфраструктурная реализация скрывает детали EF и PostgreSQL.

## 5. Модель данных

### 5.1 Users

| Поле | Тип | Ограничения |
|---|---|---|
| `Id` | UUID | первичный ключ |
| `Username` | varchar(30) | обязательное, уникальное, 3–30 символов |

### 5.2 Administrators

| Поле | Тип | Ограничения |
|---|---|---|
| `Id` | UUID | первичный ключ |
| `Username` | varchar(30) | обязательное, уникальное, 3–30 символов |

### 5.3 RelatedEntities

| Поле | Тип | Ограничения |
|---|---|---|
| `Id` | UUID | первичный ключ |
| `DisplayName` | varchar(200) | необязательное |
| `ExternalUrl` | varchar(2000) | необязательное, корректный URI |

### 5.4 Notifications

| Поле | Тип | Ограничения |
|---|---|---|
| `Id` | UUID | первичный ключ |
| `RecipientUserId` | UUID | обязательная FK на `Users`, удаление запрещено |
| `AdministratorId` | UUID? | необязательная FK на `Administrators`, при удалении становится `null` |
| `RelatedEntityId` | UUID? | необязательная FK на `RelatedEntities`, при удалении становится `null` |
| `Title` | varchar(30) | обязательное, 3–30 символов |
| `Body` | varchar(1000) | обязательное, 1–1000 символов |
| `Type` | integer/enum | `Info`, `Warning`, `Success`, `Error` |
| `IsRead` | boolean | обязательное, по умолчанию `false` |
| `ReadAt` | timestamptz? | устанавливается при прочтении и очищается при возврате в непрочитанное |
| `RelatedEntityType` | varchar/text | необязательное, 2–100 символов |
| `CreatedAt` | timestamptz | обязательное, в UTC |

Индексы созданы для `CreatedAt`, `IsRead`, `RecipientUserId` и `AdministratorId`. Пользовательские и административные имена уникальны.

## 6. Запуск и настройка

### 6.1 Предварительные требования

- Windows, Linux или macOS;
- .NET SDK 8.0;
- PostgreSQL;
- Visual Studio 2022 либо другой совместимый редактор/IDE;
- для работы EF Tools при создании миграций — глобальный или локальный `dotnet ef`.

Установка EF CLI при необходимости:

```powershell
dotnet tool install --global dotnet-ef
```

### 6.2 Подготовка PostgreSQL

Создайте базу данных:

```sql
CREATE DATABASE "NotificationServiceDb";
```

Проверьте строку подключения в `Presentation/NotificationService.WebHost/appsettings.json`:

```json
"ApplicationDbContext": "Host=localhost;Port=5433;Database=NotificationServiceDb;UserName=postgres;Password=<пароль>;"
```

Рекомендуется хранить учётные данные вне репозитория и переопределять строку подключения через переменные окружения или секрет-хранилище.

Пример переопределения для .NET-конфигурации:

```powershell
$env:ConnectionStrings__ApplicationDbContext = "Host=localhost;Port=5433;Database=NotificationServiceDb;UserName=postgres;Password=<пароль>"
```

### 6.3 Сборка и запуск

```powershell
dotnet restore Eventide.NotificationService.sln
dotnet build Eventide.NotificationService.sln --configuration Release
dotnet run --project Presentation/NotificationService.WebHost/NotificationService.WebHost.csproj
```

При старте:

1. загружается конфигурация приложения;
2. читается `ConnectionStrings:ApplicationDbContext`;
3. регистрируются `DbContext`, репозитории, application-сервисы и AutoMapper;
4. создаётся scope приложения;
5. вызывается `Database.Migrate()` для применяемых миграций;
6. запускаются контроллеры и Swagger.

Если строка подключения отсутствует, приложение завершается исключением `InvalidOperationException`.

### 6.4 Swagger

Swagger включён для всех сред и доступен по адресу `/swagger`.

Пример JSON запроса для создания уведомления:

```json
{
  "recipientUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "administratorId": null,
  "title": "Новая задача",
  "body": "Вам назначена новая задача.",
  "type": "Info",
  "relatedEntityType": "Task",
  "relatedEntityId": null
}
```

Enum `NotificationType` может передаваться именем или числовым значением JSON-сериализатора. Рекомендуется использовать строки: `Info`, `Warning`, `Success`, `Error`.

## 7. REST API

Базовые маршруты:

- `/api/users`
- `/api/administrators`
- `/api/related-entities`
- `/api/notifications`

### 7.1 Пользователи

| Метод | Маршрут | Описание | Основные ответы |
|---|---|---|---|
| `GET` | `/api/users` | Получить всех пользователей | `200` |
| `GET` | `/api/users/{id}` | Получить пользователя | `200`, `404` |
| `POST` | `/api/users` | Создать пользователя | `201`, `409` при занятом имени |
| `PUT` | `/api/users/{id}` | Обновить пользователя | `204`, `400` при несовпадении ID, `409` |
| `DELETE` | `/api/users/{id}` | Удалить пользователя | `204`, `404` |

Тело создания:

```json
{
  "username": "alex"
}
```

Тело обновления соответствует `UserModel`:

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "username": "alex-new"
}
```

Имя пользователя должно иметь длину от 3 до 30 символов.

### 7.2 Администраторы

| Метод | Маршрут | Описание | Основные ответы |
|---|---|---|---|
| `GET` | `/api/administrators` | Получить всех администраторов | `200` |
| `GET` | `/api/administrators/{id}` | Получить администратора | `200`, `404` |
| `POST` | `/api/administrators` | Создать администратора | `201`, `409` |
| `PUT` | `/api/administrators/{id}` | Обновить администратора | `204`, `400`, `409` |
| `DELETE` | `/api/administrators/{id}` | Удалить администратора | `204`, `404` |

Тело создания:

```json
{
  "username": "moderator"
}
```

Имя администратора должно иметь длину от 3 до 30 символов.

### 7.3 Связанные сущности

| Метод | Маршрут | Описание | Основные ответы |
|---|---|---|---|
| `GET` | `/api/related-entities` | Получить все связанные сущности | `200` |
| `GET` | `/api/related-entities/{id}` | Получить сущность | `200`, `404` |
| `POST` | `/api/related-entities` | Создать сущность | `201`, `400` |
| `PUT` | `/api/related-entities/{id}` | Обновить сущность | `204`, `400`, `404` |
| `DELETE` | `/api/related-entities/{id}` | Удалить сущность | `204`, `404` |

Тело создания:

```json
{
  "displayName": "Task #42",
  "externalUrl": "https://example.com/tasks/42"
}
```

Оба поля необязательны в DTO, однако непустое значение проверяется:

- `DisplayName`: 1–200 символов;
- `ExternalUrl`: не более 2000 символов и корректный URI.

### 7.4 Уведомления

| Метод | Маршрут | Описание | Основные ответы |
|---|---|---|---|
| `GET` | `/api/notifications` | Получить уведомления с необязательными фильтрами | `200` |
| `GET` | `/api/notifications/{id}` | Получить уведомление | `200`, `404` |
| `GET` | `/api/notifications/unread-count?userId={id}` | Получить количество непрочитанных уведомлений | `200` |
| `POST` | `/api/notifications` | Создать уведомление | `201`, `400` |
| `PUT` | `/api/notifications/{id}` | Обновить уведомление | `204`, `404` |
| `DELETE` | `/api/notifications/{id}` | Удалить уведомление | `204`, `404` |
| `PATCH` | `/api/notifications/{id}/read` | Отметить прочитанным | `204`, `404` |
| `PATCH` | `/api/notifications/{id}/unread` | Отметить непрочитанным | `204`, `404` |

Фильтры `GET /api/notifications`:

- `userId` — фильтр получателя;
- `administratorId` — фильтр автора/администратора;
- `onlyUnread=true` — только непрочитанные записи.

Приоритет фильтрации в application-сервисе:

1. если указан `administratorId`, используется выборка по администратору;
2. иначе, если указан `userId`, используется выборка по пользователю;
3. иначе возвращаются все уведомления.

Для выборки по пользователю параметр `onlyUnread` учитывается. Для выборки по администратору он игнорируется. Уведомления сортируются по убыванию `CreatedAt`.

Тело создания/обновления:

```json
{
  "recipientUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "administratorId": "9ca4b8dc-b8ec-4de5-8b21-fdeb5ebc5b92",
  "title": "Новая задача",
  "body": "Вам назначена новая задача.",
  "type": "Info",
  "relatedEntityType": "Task",
  "relatedEntityId": "b8ed1d6f-2914-4f7e-8a4e-4765b4c39dc2"
}
```

Создание проверяет существование получателя, администратора и связанной сущности. При отсутствии хотя бы одного обязательного для запроса идентификатора возвращается `400`.

Обновление не меняет `CreatedAt`. Администратор при обновлении не обновляется и сохраняет значение исходного уведомления.

Уведомление может принимать следующие типы:

| Значение | Описание |
|---|---|
| `Info` | информационное уведомление |
| `Warning` | предупреждение |
| `Success` | успешный результат |
| `Error` | ошибка |

## 8. Валидация

| Объект | Минимум | Максимум |
|---|---:|---:|
| `Username` | 3 символа | 30 символов |
| `Title` | 3 символа | 30 символов |
| `Body` | 1 символ | 1000 символов |
| `RelatedEntityDisplayName` | 1 символ | 200 символов |
| `RelatedEntityTypeName` | 2 символа | 100 символов |
| `ExternalUrl` | — | 2000 символов |

Пустые строки и строки, состоящие из пробельных символов, считаются невалидными там, где поле обязательное.

Валидация доменных value objects выбрасывает типизированные исключения. На HTTP-уровне контроллеры в основном сопоставляют отсутствующие сущности и конфликт имён с результатами; глобальный обработчик исключений не настроен.

## 9. Миграции базы данных

Файлы миграций находятся в `Infrastructure/NotificationService.Infrastructure.EntityFramework/Migrations`.

Просмотр миграций:

```powershell
dotnet ef migrations list `
  --project Infrastructure/NotificationService.Infrastructure.EntityFramework `
  --startup-project Presentation/NotificationService.WebHost
```

Создание новой миграции:

```powershell
dotnet ef migrations add <Name> `
  --project Infrastructure/NotificationService.Infrastructure.EntityFramework `
  --startup-project Presentation/NotificationService.WebHost
```

Применение миграций вручную:

```powershell
dotnet ef database update `
  --project Infrastructure/NotificationService.Infrastructure.EntityFramework `
  --startup-project Presentation/NotificationService.WebHost
```

Откат к конкретной миграции:

```powershell
dotnet ef database update <MigrationName> `
  --project Infrastructure/NotificationService.Infrastructure.EntityFramework `
  --startup-project Presentation/NotificationService.WebHost
```

При старте `MigrationManager` вызывает `Database.Migrate()` автоматически. Для продакшена предпочтительно валидировать изменения миграций и применять их в рамках контролируемого deployment-процесса.

## 10. Тестирование

Запуск всех тестов:

```powershell
dotnet test Eventide.NotificationService.sln --configuration Release
```

Запуск тестового проекта с покрытием:

```powershell
dotnet test Tests/NotificationService.Tests.csproj `
  --configuration Release `
  --collect:"XPlat Code Coverage"
```

Покрытие сохраняется в `Tests/TestResults/**/coverage.cobertura.xml`.

### HTML-отчёт покрытия

Для просмотра покрытия в браузере используйте ReportGenerator. Установка глобального инструмента выполняется один раз для текущего пользователя:

```powershell
dotnet tool install --global dotnet-reportgenerator-globaltool
```

Затем из корня репозитория запустите тесты с генерацией Cobertura-файла:

```powershell
dotnet test Tests/NotificationService.Tests.csproj `
  --configuration Release `
  --collect:"XPlat Code Coverage"
```

Постройте HTML-отчёт из созданного файла покрытия:

```powershell
reportgenerator `
  "-reports:Tests/**/coverage.cobertura.xml" `
  "-targetdir:coverage-report"
```

Откройте отчёт в браузере:

```powershell
Invoke-Item .\coverage-report\index.html
```

ReportGenerator находит Cobertura-отчёты под `Tests`, включая вложенные каталоги `TestResults/<идентификатор запуска>`. Каждый запуск тестов обычно создаёт новый такой каталог; маска `Tests/**/coverage.cobertura.xml` позволяет обработать доступные отчёты. Чтобы строить отчёт только по самому свежему запуску, удалите старые каталоги результатов тестирования перед запуском тестов либо укажите конкретный путь файла в параметре `-reports`.

Если после установки команда `reportgenerator` не распознаётся, перезапустите терминал, чтобы обновился `PATH`, либо вызовите установленный глобальный инструмент через каталог инструментов .NET пользователя.

Текущий набор тестов охватывает:

- создание и изменение domain-сущностей;
- переходы состояния прочтения;
- валидацию value objects;
- уникальность имён;
- создание пользователей, администраторов и связанных сущностей;
- CRUD application-сервисов;
- фильтрацию уведомлений;
- подсчёт непрочитанных уведомлений;
- обработку отсутствующих связанных сущностей.

## 11. CI/CD

GitHub Actions workflow `.github/workflows/dotnet-ci.yml` запускается:

- при push в `main` или `master`;
- при pull request в `main` или `master`;
- вручную через `workflow_dispatch`.

Pipeline выполняет:

1. checkout;
2. установку .NET 8;
3. restore solution;
4. build в Release-конфигурации;
5. запуск тестов с coverlet;
6. проверку line coverage не ниже 80%;
7. загрузку отчёта покрытия как артефакт.

CI падает, если Cobertura-отчёт не создан или его `line-rate` ниже `0.80`.

## 12. Безопасность и эксплуатационные ограничения

Перед использованием в prod рекомендуется исправить или проверить следующие моменты:

- вынести строку подключения и пароль PostgreSQL из конфигурации и исходного кода;
- включить аутентификацию и авторизацию: на текущий момент контроллеры используют `UseAuthorization`, но нет схемы аутентификации и политики доступа;
- добавить HTTPS redirect и действительную TLS-конфигурацию;
- ограничить Swagger по окружению или закрыть его за авторизацией в prod;
- отключить `EnableSensitiveDataLogging()`;
- добавить глобальную обработку исключений и единый формат ошибок;
- добавить ограничения размера запросов и пагинацию для выборок;
- добавить health checks и телеметрию;
- определить стратегию миграций для production-выпуска;
- обеспечить резервное копирование PostgreSQL.

## 13. Добавление новой функциональности

Рекомендуемый порядок изменений:

1. добавить или изменить entity/value object в Domain;
2. добавить валидацию или доменное правило;
3. обновить интерфейс репозитория при необходимости;
4. реализовать репозиторий в Infrastructure;
5. добавить application-модель и application-сервис;
6. обновить `ApplicationProfile`;
7. добавить контроллер или маршрут;
8. создать EF-конфигурацию или миграцию;
9. покрыть изменения unit-тестами;
10. обновить Swagger-документацию или `PROJECT_DOCUMENTATION.md`.

## 14. Подробная карта зависимостей проектов

Решение организовано как набор независимых .NET-проектов. Зависимости направлены в сторону доменной логики:

```text
NotificationService.WebHost
    ├── NotificationService.Application.Models
    ├── NotificationService.Application.Services
    └── NotificationService.Infrastructure.EntityFramework
            ├── NotificationService.Domain
            └── NotificationService.Domain.Repositories.Abstractions
                    └── NotificationService.Domain

NotificationService.Application.Services
    ├── NotificationService.Application.Models
    ├── NotificationService.Domain
    └── NotificationService.Domain.Repositories.Abstractions

NotificationService.Application.Models
    └── NotificationService.Domain (для NotificationType)

NotificationService.Domain
    └── NotificationService.ValueObjects

NotificationService.Tests
    ├── Application Services
    ├── Application Models
    ├── Domain
    ├── ValueObjects
    └── Repository Abstractions
```

Практическое правило зависимостей:

- Domain не должен ссылаться на Application, Infrastructure или Presentation;
- Application не должен ссылаться на конкретный EF Core-контекст;
- Infrastructure реализует абстракции, объявленные в Domain;
- Controllers не создают SQL-запросы и не должны управлять `DbContext`;
- только WebHost знает о конкретной БД и библиотеке Swagger.

## 15. Ключевые интерфейсы и контракт репозиториев

Базовый репозиторий `IRepository<TEntity, TId>` предоставляет:

```csharp
Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken);
Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken);
```

Специализированные интерфейсы:

- `IUserRepository.GetUserByUsernameAsync`;
- `IAdministratorRepository.GetAdministratorByUsernameAsync`;
- `IRelatedEntityRepository` использует базовый репозиторий без дополнительных методов;
- `INotificationRepository.GetByUserAsync`;
- `INotificationRepository.GetByAdministratorAsync`;
- `INotificationRepository.GetUnreadCountAsync`.

Общие application-интерфейсы следуют CRUD-контракту:

```csharp
Task<IEnumerable<TModel>> GetModelsAsync(CancellationToken cancellationToken = default);
Task<TModel?> GetModelByIdAsync(TId id, CancellationToken cancellationToken = default);
Task<TModel?> CreateModelAsync(TCreateModel model, CancellationToken cancellationToken = default);
Task<bool> UpdateModelAsync(TModel model, CancellationToken cancellationToken = default);
Task<bool> DeleteModelAsync(TId id, CancellationToken cancellationToken = default);
```

Уведомления имеют дополнительный контракт для фильтрации, чтения и подсчёта непрочитанных записей.

## 16. Полная последовательность создания уведомления

```text
POST /api/notifications
  |
  v
NotificationsController.Create()
  |
  v
NotificationApplicationService.CreateModelAsync()
  |
  +-- INotificationRepository (проверка выполняется позднее при сохранении)
  +-- IUserRepository.GetByIdAsync(recipientUserId)
  +-- IAdministratorRepository.GetByIdAsync(administratorId), если id задан
  +-- IRelatedEntityRepository.GetByIdAsync(relatedEntityId), если id задан
  |
  +-- новый Title(value)
  +-- новый Body(value)
  +-- новый RelatedEntityTypeName(value), если поле задано
  |
  v
new Notification(...)
  |
  v
INotificationRepository.AddAsync(...)
  |
  v
EfRepository.AddAsync()
  |
  v
DbContext.AddAsync() + SaveChangesAsync()
  |
  v
ApplicationProfile -> NotificationModel
  |
  v
201 Created + Location
```

При отсутствии получателя либо необязательной, но указанной в запросе сущности сервис возвращает `null`. Контроллер преобразует это в `400 Bad Request`.

Администратор не является обязательным получателем или отправителем для хранения уведомления и может отсутствовать.

## 17. Жизненный цикл состояний уведомления

Начальное состояние:

```text
IsRead = false
ReadAt = null
```

Отметка прочитанным:

```text
IsRead == false
  -> MarkAsRead(UtcNow)
  -> IsRead = true
  -> ReadAt = текущее UTC-время
```

Повторная отметка прочитанным:

```text
IsRead == true
  -> false
  -> состояние не изменяется
  -> метод контроллера возвращает 404
```

Отметка непрочитанным:

```text
IsRead == true
  -> MarkAsUnread()
  -> IsRead = false
  -> ReadAt = null
```

Повторная отметка непрочитанным:

```text
IsRead == false
  -> false
  -> состояние не изменяется
  -> метод контроллера возвращает 404
```

В случае отсутствующего уведомления оба метода состояния возвращают `404 Not Found`.

## 18. Детали преобразования Domain в API-модели

`ApplicationProfile` выполняет следующие преобразования:

| Domain-поле | API-поле |
|---|---|
| `Administrator.Id` | `AdministratorModel.Id` |
| `Administrator.Username.Value` | `AdministratorModel.Username` |
| `User.Id` | `UserModel.Id` |
| `User.Username.Value` | `UserModel.Username` |
| `RelatedEntity.Id` | `RelatedEntityModel.Id` |
| `RelatedEntity.DisplayName.Value` | `DisplayName` |
| `RelatedEntity.ExternalUrl.Value` | `ExternalUrl` |
| `Notification.RecipientUser.Id` | `RecipientUserId` |
| `Notification.Administrator.Id` | `AdministratorId` или `null` |
| `Notification.Title.Value` | `Title` |
| `Notification.Body.Value` | `Body` |
| `Notification.RelatedEntityType.Value` | `RelatedEntityType` или `null` |
| `Notification.RelatedEntity.Id` | `RelatedEntityId` или `null` |

API-модели являются positional record с неизменяемыми свойствами. Это упрощает передачу данных и снижает риск случайной мутации DTO.

## 19. Полный набор доменных исключений

### Исключения value objects

| Исключение | Причина |
|---|---|
| `ArgumentNullOrWhiteSpaceException` | строка `null`, пустая или состоит из пробелов |
| `ArgumentShortValueException` | значение короче минимальной длины |
| `ArgumentLongValueException` | значение превышает максимальную длину |
| `ValidatorNullException` | в base-класс передан отсутствующий валидатор |
| `ArgumentException` | URL не является корректным URI |

### Исключения сущности Notification

В проекте объявлены специализированные исключения:

- `ArgumentNullValueException`;
- `NotificationBodyNullException`;
- `NotificationCreatedAtInvalidException`;
- `NotificationNotFoundException`;
- `NotificationReadStateInvalidException`;
- `NotificationRecipientUserIdEmptyException`;
- `NotificationRelatedEntityReferenceInvalidException`;
- `NotificationTitleNullException`.

Часть специализированных исключений не используется каждым контроллером напрямую. Основная runtime-проверка данных в сервисах строится через value objects и явные проверки репозиториев.

## 20. Конфигурация DI и жизненный scope

В `Program.cs` все application-сервисы и репозитории регистрируются как `Scoped`:

```csharp
builder.Services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IAdministratorRepository, EfAdministratorRepository>();
builder.Services.AddScoped<IRelatedEntityRepository, EfRelatedEntityRepository>();
builder.Services.AddScoped<INotificationRepository, EfNotificationRepository>();

builder.Services.AddScoped<IUserApplicationService, UserApplicationService>();
builder.Services.AddScoped<IAdministratorApplicationService, AdministratorApplicationService>();
builder.Services.AddScoped<IRelatedEntityApplicationService, RelatedEntityApplicationService>();
builder.Services.AddScoped<INotificationApplicationService, NotificationApplicationService>();
```

`DbContext` и сервисы существуют в одном HTTP-scope запроса. Автоматическое применение миграций происходит до обработки запросов на уровне запуска приложения.

Содержимое HTTP-response не кэшируется и политика кэширования не настроена.

## 21. Настройка среды разработки

### Visual Studio

1. Откройте `Eventide.NotificationService.sln`.
2. Установите `NotificationService.WebHost` как Startup Project.
3. Проверьте профиль запуска в свойствах проекта.
4. Запустите PostgreSQL и создайте базу.
5. Запустите проект с профилем `https` либо `http`.

### Visual Studio Code

```powershell
code .
dotnet restore Eventide.NotificationService.sln
dotnet run --project Presentation/NotificationService.WebHost/NotificationService.WebHost.csproj
```

Полезные launch-параметры .NET:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://localhost:5180"
dotnet run --project Presentation/NotificationService.WebHost/NotificationService.WebHost.csproj
```

Комбинированная проверка сборки и тестов:

```powershell
dotnet build Eventide.NotificationService.sln --configuration Release
dotnet test Tests/NotificationService.Tests.csproj --configuration Release
```

## 22. Примеры вызовов API через curl

Создание пользователя:

```bash
curl -X POST "http://localhost:5180/api/users" \
  -H "Content-Type: application/json" \
  -d '{"username":"alex"}'
```

Получение пользователя:

```bash
curl "http://localhost:5180/api/users/<user-id>"
```

Создание связанной сущности:

```bash
curl -X POST "http://localhost:5180/api/related-entities" \
  -H "Content-Type: application/json" \
  -d '{"displayName":"Task #42","externalUrl":"https://example.com/tasks/42"}'
```

Создание уведомления:

```bash
curl -X POST "http://localhost:5180/api/notifications" \
  -H "Content-Type: application/json" \
  -d '{
    "recipientUserId":"<user-id>",
    "administratorId":"<administrator-id>",
    "title":"Новая задача",
    "body":"Вам назначена новая задача.",
    "type":"Info",
    "relatedEntityType":"Task",
    "relatedEntityId":"<related-entity-id>"
  }'
```

Получение непрочитанных уведомлений пользователя:

```bash
curl "http://localhost:5180/api/notifications?userId=<user-id>&onlyUnread=true"
```

Получение количества непрочитанных:

```bash
curl "http://localhost:5180/api/notifications/unread-count?userId=<user-id>"
```

Отметка прочитанным:

```bash
curl -X PATCH "http://localhost:5180/api/notifications/<notification-id>/read"
```

## 23. Формирование ошибок API

Текущая реализация не содержит централизованного middleware или `ProblemDetails`.

| Ситуация | Ответ |
|---|---|
| Неверный формат GUID маршрута | MVC binding/`400` |
| Несовпадение id маршрута и payload | `400` с текстом |
| Некорректное/поврежденное JSON-тело | автоматически `400` через `[ApiController]` |
| Объект не найден | `404` |
| Дублирующееся имя пользователя/администратора | `409` |
| Отсутствует получатель или связанная сущность уведомления | `400` |
| Повторная операция mark read/unread | `404` при логическом отсутствии перехода |
| Исключение доменной валидации | пока может стать `500`, если не перехвачено middleware |

Рекомендуемый формат будущих ошибок:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110",
  "title": "Bad Request",
  "status": 400,
  "detail": "RecipientUserId was not found.",
  "traceId": "<request-id>"
}
```

## 24. Рекомендации по тестам

Domain-тесты должны проверять:

- неизменность base Entity;
- уникальность значений value objects;
- граничные значения длины;
- переходы read/unread;
- операции администратора над уведомлениями;
- сравнение и хеширование value objects.

Application-тесты должны проверять:

- существование и отсутствия всех связей;
- уникальность имен;
- вызовы репозитория;
- отсутствие сохранения при ошибке;
- фильтрацию и сортировку данных;
- отсутствие повторного mark read/unread;
- сопоставление DTO.

Интеграционные тесты должны:

- запускать PostgreSQL через Testcontainers или CI-сервис;
- применять все миграции;
- проверять уникальные индексы;
- проверять каскадные и restrict/delete-поведения;
- использовать реальный HTTP-host, а не один только controller method.

## 25. Рекомендованный deployment-процесс

1. Собрать Release-образ или publish-пакет.
2. Подготовить PostgreSQL и секреты.
3. Проверить миграции на копии prod-базы.
4. Применить миграции отдельной release-job.
5. Запустить application на первом instance.
6. Проверить Swagger и API health endpoint.
7. Запустить нагрузочный smoke-тест.
8. Постепенно перевести traffic.

Пример publish-команды:

```powershell
dotnet publish Presentation/NotificationService.WebHost/NotificationService.WebHost.csproj `
  --configuration Release `
  --runtime linux-x64 `
  --self-contained false `
  --output ./publish
```

В release-окружении следует отключить автоматическую миграцию при старте и перенести `Database.Migrate()` в контролируемую миграционную задачу, если версия базы разворачивается несколькими инстансами.

## 26. Типовые проблемы и диагностика

### Не найдена строка подключения

Симптом:

```text
InvalidOperationException: Connection string for NotificationServiceDbContext is not configured.
```

Проверить:

```powershell
dotnet run --project Presentation/NotificationService.WebHost/NotificationService.WebHost.csproj
```

и наличие ключа `ConnectionStrings:ApplicationDbContext`.

### PostgreSQL недоступен

Проверить порт и доступность:

```powershell
Test-NetConnection localhost -Port 5433
```

В обычной установке PostgreSQL стандартный порт может быть `5432`, а проект настроен на `5433`.

### Автоматическая миграция падает при старте

Проверить права пользователя PostgreSQL на создание и изменение таблиц:

- CREATE;
- ALTER;
- создание индексов;
- чтение и запись системных таблиц миграций `__EFMigrationsHistory`.

### Пользователь или администратор не создаётся

Причины:

- имя имеет меньше 3 или больше 30 символов;
- имя уже занято;
- имя состоит только из пробелов.

### Swagger не открывается

Проверить адрес из вывода `dotnet run`, затем открыть `http://<host>:<port>/swagger`.

Приложение использует `app.UseSwagger()` и `app.UseSwaggerUI()` без ограничения окружения.

### Сборка проходит, тесты падают

Запустить тестовый проект с подробным выводом:

```powershell
dotnet test Tests/NotificationService.Tests.csproj --configuration Release --logger "console;verbosity=detailed"
```

Проверить, что все output-проекты закрыты или не используются одновременно двумя процессами сборки.
