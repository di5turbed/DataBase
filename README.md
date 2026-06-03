Инструкция по развертыванию и запуску
1. Подготовка базы данных
Разверните СУБД PostgreSQL.

Создайте базу данных (например, esports_db).

Выполните SQL-скрипты для инициализации структуры 12 таблиц (убедитесь, что таблица игроков называется players, а таблица результатов — match_result со столбцами winner_team, tournament_id и total_prize_money).

2. Настройка сервера (ServerApi)
Перейдите в папку Server/ServerApi.

Откройте файл appsettings.json (или appsettings.Development.json) и укажите вашу строку подключения к PostgreSQL в секции ConnectionStrings.DefaultConnection:

JSON
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=esports_db;Username=esports_admin;Password=YOUR_PASSWORD"
}
Запустите проект сервера через Visual Studio или из консоли:

Bash
dotnet run --project ServerApi.csproj
Сервер запустится на портах по умолчанию (например, http://localhost:5007). Вы можете проверить работоспособность, перейдя по адресу http://localhost:5007/swagger, где откроется интерактивная документация Swagger UI.

3. Первичная регистрация администратора
Так как пароли хэшируются с помощью BCrypt, напрямую вписать пароль текстом в БД нельзя (сервер не пропустит авторизацию).

Откройте файл ServerApi.http в Visual Studio при запущенном сервере.

Выполните POST-запрос на регистрацию пользователя:

HTTP
POST http://localhost:5007/api/auth/register
Content-Type: application/json

{
  "username": "admin",
  "password": "12345"
}
Сервер зашифрует пароль и создаст валидную учетную запись администратора в таблице users.

4. Запуск клиента (DesktopClient)
Перейдите в папку Client/DesktopClient.

Убедитесь, что в файле ApiClient.cs базовый адрес BaseAddress соответствует адресу запущенного API сервера (http://localhost:5007/).

Запустите десктопное приложение.

В окне авторизации введите логин и пароль.

После успешной валидации и получения JWT-токена откроется главная рабочая панель приложения.
