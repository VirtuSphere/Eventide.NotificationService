# Сценарий видео о NotificationService

**Длительность:** около 4 минут 55 секунд.

## Подготовка к записи

- Запустить PostgreSQL и WebHost.
- Открыть Swagger по адресу `/swagger`.
- Открыть окно с базой данных и таблицей `Notifications`.
- Заранее создать пользователя и скопировать его ID для запроса создания уведомления.
- Подготовить HTML-отчёт покрытия `coverage-report/index.html`.

## Слайд 1 — Что делает сервис · 0:00–0:15

**На экране:** титульный слайд с названием проекта.

**Говорить:**

> «Это NotificationService — REST API для создания и хранения уведомлений пользователей. Покажу устройство приложения, доменную модель, тесты, а затем прослежу создание уведомления от запроса в Swagger до записи в PostgreSQL».

## Слайд 2 — Технологии, архитектура и миграции · 0:15–1:00

**На экране:** Solution Explorer, затем [Program.cs](../Presentation/NotificationService.WebHost/Program.cs) и [начальная миграция](../Infrastructure/NotificationService.Infrastructure.EntityFramework/Migrations/20260527112601_init2.cs).

**Говорить:**

> «Проект написан на C# и .NET 8. API реализован на ASP.NET Core, доступ к данным — через Entity Framework Core и PostgreSQL с провайдером Npgsql. AutoMapper преобразует доменные объекты в модели API, а Swagger предоставляет интерактивное описание запросов.
>
> Это одно REST API-приложение с несколькими слоями, а не набор отдельных развёрнутых микросервисов. В `Domain` находятся сущности и правила, в `Application` — сценарии и модели запросов, в `Infrastructure` — EF Core и репозитории, а `WebHost` принимает HTTP-запросы.
>
> При запуске WebHost вызывается `MigrateDatabase`, который применяет ожидающие миграции. Например, начальная миграция создаёт таблицы пользователей, администраторов, связанных сущностей и уведомлений».

## Слайд 3 — Схема PostgreSQL · 1:00–1:35

**На экране:** ER-схема или таблицы PostgreSQL: `Users`, `Administrators`, `RelatedEntities`, `Notifications`.

**Говорить:**

> «Главная таблица — `Notifications`. В ней хранится получатель, заголовок, текст, тип и состояние прочтения. Уведомление обязательно связано с пользователем-получателем. Администратор и связанная сущность необязательны.
>
> При удалении администратора или связанной сущности уведомление сохраняется, а соответствующая ссылка очищается. Удалить пользователя-получателя, пока на него ссылаются уведомления, нельзя. Для ускорения выборок по получателю, администратору и дате в схеме предусмотрены индексы».

## Слайд 4 — Доменный слой: сущности и правила · 1:35–2:40

**На экране:** [Notification.cs](../Domain/NotificationService.Domain/Notification.cs), [Administrator.cs](../Domain/NotificationService.Domain/Administrator.cs), [User.cs](../Domain/NotificationService.Domain/User.cs), [RelatedEntity.cs](../Domain/NotificationService.Domain/RelatedEntity.cs).

**Говорить:**

> «Доменный слой содержит бизнес-модель и не отвечает за HTTP или SQL.
>
> Центральный класс — `Notification`. У него есть получатель, заголовок, текст, тип уведомления, дата создания и состояние прочтения. Методы `MarkAsRead` и `MarkAsUnread` меняют состояние и дату `ReadAt`: при повторном переходе метод сообщает, что изменений не было. `Update` меняет данные уведомления.
>
> Типы уведомлений описаны enum `NotificationType`: `Info`, `Warning`, `Success` и `Error`. В текущем JSON они сериализуются числом: например, `1` означает `Warning`.
>
> `User` хранит имя пользователя и умеет отмечать уведомление прочитанным или непрочитанным. `Administrator` может создать уведомление, а также управляет связанными с ним уведомлениями.
>
> `RelatedEntity` добавляет контекст — например, название задачи и ссылку на неё. Это позволяет связать уведомление с внешним объектом, не помещая всю информацию о задаче в сам сервис уведомлений.
>
> Для значений используются Value Objects: `Username`, `Title`, `Body`, `ExternalUrl` и тип связанной сущности. Они проверяют данные, например допустимую длину имени и заголовка».

## Слайд 5 — Тесты, покрытие и GitHub Actions · 2:40–3:25

**На экране:** [DomainTests.cs](../Tests/DomainTests.cs), [ApplicationServicesTests.cs](../Tests/ApplicationServicesTests.cs), [ApplicationProfileTests.cs](../Tests/ApplicationProfileTests.cs), затем `coverage-report/index.html` и [workflow CI](../.github/workflows/dotnet-ci.yml).

**Говорить:**

> «Сейчас тестовый проект содержит 39 тестов, и последний запуск прошёл успешно. Проверяются доменные правила, валидация Value Objects, application-сервисы и преобразование моделей. По последнему Cobertura-отчёту line coverage составляет 95,83 процента — 414 из 432 измеряемых строк.
>
> GitHub Actions запускает сборку и тесты при push или pull request в `main` и `master`, а также вручную. В workflow задан порог покрытия 80 процентов. CI загружает Cobertura-файл как artifact. HTML-отчёт я сформировал локально с помощью ReportGenerator; сам workflow HTML сейчас не генерирует».

## Слайд 6 — Swagger → Application → PostgreSQL · 3:25–4:45

**На экране:** Swagger `/swagger`; [NotificationsController.cs](../Presentation/NotificationService.WebHost/Controllers/NotificationsController.cs); [NotificationApplicationService.cs](../Application/NotificationService.Application.Services/NotificationApplicationService.cs); [NotificationRepository.cs](../Infrastructure/NotificationService.Infrastructure.EntityFramework/RepositoriesEF/NotificationRepository.cs); таблица `Notifications`.

**Действие:** выполните `POST /api/notifications` с существующим `recipientUserId`; затем покажите ответ и запись в БД. Получите её через `GET /api/notifications`.

**Говорить:**

> «В Swagger отправляю `POST /api/notifications`. В запросе указываю ID существующего пользователя, заголовок, текст, тип и при необходимости администратора или связанную сущность.
>
> Контроллер принимает JSON и вызывает `NotificationApplicationService`. Сервис проверяет получателя и указанные связи, создаёт доменное уведомление и передаёт его репозиторию. Репозиторий сохраняет объект через Entity Framework Core в PostgreSQL. При успехе API возвращает `201 Created` и модель созданного уведомления.
>
> Теперь показываю строку в таблице `Notifications`. Затем вызываю `GET /api/notifications?userId=...`: сервис возвращает уведомления пользователя, а репозиторий загружает связанные данные, чтобы в ответе были корректные ID получателя и связанных объектов.
>
> Для управления состоянием есть отдельные запросы `PATCH` на `/read` и `/unread`, а для подсчёта непрочитанных — `GET /api/notifications/unread-count`».

## Завершение · 4:45–4:55

**На экране:** схема `Swagger → Controller → Application → Repository → PostgreSQL`.

**Говорить:**

> «Итак, запрос проходит через контроллер и application-сервис, проверяется доменной моделью и сохраняется репозиторием в PostgreSQL. Слои и тесты помогают поддерживать эту логику и проверять её при изменениях».

## Проверка перед записью

- Убедиться, что ID пользователя существует в базе данных. Иначе API не сможет создать уведомление.
- В запросе создания использовать заголовок длиной от 3 до 30 символов и непустое тело длиной до 1000 символов.
- Для презентации можно выполнить запрос:

```http
POST /api/notifications
Content-Type: application/json

{
  "recipientUserId": "<существующий-user-id>",
  "administratorId": null,
  "title": "Новая задача",
  "body": "Вам назначена новая задача.",
  "type": "Info",
  "relatedEntityType": null,
  "relatedEntityId": null
}
```

- После успешного `POST` показать ответ `201 Created`, затем запись в таблице `Notifications` и результат `GET /api/notifications?userId=<существующий-user-id>`.
