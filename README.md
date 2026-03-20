# Eventide.NotificationService
Сервис уведомлений
1. Установить Docker Desktop
2. В папке с **docker-compose.yml** открыть PowerShell или CMD
3. В терминале в папке с **docker-compose.yml** выполнить *docker compose up -d*

Это поднимет все сервисы:
  Postgres -- порт 5432
  RabbitMQ -- порты 5672, 15672
  Redis -- порт 6379

Проверить, что сервисы работают, можно через Docker Desktop → Containers

RabbitMQ UI открывается через **http://localhost:15672**

Логин: *guest*
Пароль: *guest*

4. Открыть проект NotificationService в Visual Studio или через терминал, введя *dotnet run*

5. При запуске автоматически поднимается Consumer для очереди *user-created-queue*
6. Publisher отправляет тестовое сообщение:"User created: test@example.com"

Если сообщение появляется в консоли → сервис работает, связь с RabbitMQ установлена


Фактически проверка сделана через тестовое сообщение (Publisher → RabbitMQ → Consumer).
Все сервисы видят друг друга через имена контейнеров (*rabbitmq*, *postgres*, *redis*). Проверка подтверждается тестовым сообщением из NotificationService.

7. Создать коллекцию *Auth & User*
Добавить три запроса:

Register: POST, JSON *{"email":"test@mail.com","password":"123456"}*
Login: POST, JSON *{"email":"test@mail.com","password":"123456"}*
Profile: GET, Body не нужен

Экспортировать коллекцию -- готово

