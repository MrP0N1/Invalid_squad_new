# Платформа для обмена конспектами (Backend на C# / ASP.NET Core)

Этот проект — простой backend-сервер для платформы, где студенты могут делиться своими конспектами и файлами (PDF, фото).

Функционал:

- Авторизация / регистрация
  - `POST /api/auth/register` — регистрация
  - `POST /api/auth/login` — вход по email + пароль, получение JWT токена
- Заметки (конспекты)
  - `GET /api/notes` — список своих конспектов
  - `POST /api/notes` — создание
  - `PUT /api/notes/{id}` — редактирование
  - `DELETE /api/notes/{id}` — удаление
  - `POST /api/notes/{id}/files` — загрузка файлов (pdf / фото) к заметке
- Профиль пользователя
  - `GET /api/profile/me` — просмотр профиля
  - `PUT /api/profile/me` — редактирование профиля
  - `GET /api/profile/me/notes` — список своих конспектов (короткий формат)

## 1. Что нужно установить

1. **.NET SDK 8**
   - Зайди на официальный сайт Microsoft и скачай **.NET SDK 8** под свою ОС.
   - После установки проверь:
     ```bash
     dotnet --version
     ```
     Должно вывести что‑то вроде `8.0.x`.

2. **Visual Studio Code**
   - Скачай и установи VS Code.
   - Поставь расширения:
     - **C# Dev Kit** (или просто C# от Microsoft)
     - (опционально) **REST Client** или используй Postman/Insomnia для тестов API.

## 2. Структура проекта

После распаковки архива у тебя будет папка:

```text
notes-platform-backend/
  README.md
  NotesPlatformBackend.sln
  NotesPlatformBackend/
    NotesPlatformBackend.csproj
    Program.cs
    appsettings.json
    Controllers/
      AuthController.cs
      NotesController.cs
      ProfileController.cs
    Models/
      User.cs
      Note.cs
      NoteFile.cs
    Data/
      AppDbContext.cs
    DTOs/
      AuthDtos.cs
      NoteDtos.cs
      ProfileDtos.cs
    Services/
      JwtOptions.cs
      JwtService.cs
    wwwroot/
      (сюда будут сохраняться файлы)
```

## 3. Как открыть и запустить в VS Code

1. Открой VS Code.
2. `File` → `Open Folder...` → выбери папку **`notes-platform-backend`**.
3. VS Code предложит установить расширения для C# (если ещё нет) — согласись.
4. Открой встроенный терминал:
   - `Terminal` → `New Terminal`.
5. Перейди в папку проекта API:
   ```bash
   cd NotesPlatformBackend
   ```
6. Восстанови зависимости (скачать нужные библиотеки NuGet):
   ```bash
   dotnet restore
   ```
7. Запусти проект:
   ```bash
   dotnet run
   ```
8. В терминале появится что-то вроде:
   ```text
   Now listening on: http://localhost:5099
   Now listening on: https://localhost:7243
   ```
   Значит сервер запущен.

9. Открой браузер и перейди по адресу:
   - `https://localhost:7243/swagger` (или порт, который покажет `dotnet run`)

   Там будет автоматическая документация Swagger, где можно тестировать запросы.

## 4. База данных

Используется **SQLite** — это обычный файл `notes.db` в корне проекта `NotesPlatformBackend`.

При первом запуске `dotnet run` база создаётся автоматически благодаря миграции `db.Database.Migrate()` в `Program.cs`.

Никаких дополнительных установок не нужно.

## 5. Настройка JWT (токены)

В файле `NotesPlatformBackend/appsettings.json` лежат настройки токена:

```json
"Jwt": {
  "Key": "SUPER_SECRET_KEY_CHANGE_ME",
  "Issuer": "NotesPlatform",
  "Audience": "NotesPlatformUsers",
  "ExpiresMinutes": 60
}
```

Для реального проекта **обязательно** поменяй `Key` на длинную случайную строку.

## 6. Основные эндпоинты и пример использования

### 6.1. Регистрация

**POST** `/api/auth/register`  
Тело (JSON):

```json
{
  "username": "student1",
  "email": "student1@example.com",
  "password": "StrongPassword123"
}
```

В ответ придёт объект с токеном:

```json
{
  "token": "JWT_ТОКЕН",
  "username": "student1",
  "email": "student1@example.com"
}
```

### 6.2. Логин

**POST** `/api/auth/login`

```json
{
  "email": "student1@example.com",
  "password": "StrongPassword123"
}
```

Ответ аналогичный — с `token`.  
Этот `token` нужно указывать в заголовке всех защищённых запросов:

```http
Authorization: Bearer JWT_ТОКЕН_ОТ_СЕРВЕРА
```

### 6.3. Создание заметки

**POST** `/api/notes` (требуется авторизация)

Тело:

```json
{
  "title": "Конспект по математике",
  "content": "Здесь текст конспекта..."
}
```

### 6.4. Получение списка своих заметок

**GET** `/api/notes` (с токеном)  
Вернёт массив заметок с прикреплёнными файлами.

### 6.5. Редактирование заметки

**PUT** `/api/notes/{id}`

```json
{
  "title": "Обновлённый конспект по математике",
  "content": "Новый текст..."
}
```

### 6.6. Удаление заметки

**DELETE** `/api/notes/{id}`

### 6.7. Загрузка файлов (pdf / фото)

**POST** `/api/notes/{id}/files`  
Тип запроса: `multipart/form-data`  
Поле: `file` — сам файл.

В Swagger это можно выбрать через кнопку **"Add file"**.

Файл сохранится в папку `wwwroot/uploads`, а в базе — путь к файлу.

### 6.8. Профиль

**GET** `/api/profile/me` — вернуть данные профиля  
**PUT** `/api/profile/me` — изменить `FullName` и `Bio`

Тело для обновления:

```json
{
  "fullName": "Иван Иванов",
  "bio": "Студент 2 курса, люблю матан :)"
}
```

**GET** `/api/profile/me/notes` — список своих конспектов (id + заголовок + даты).

## 7. Как протестировать без фронтенда

Варианты:

1. **Swagger**  
   - Открыть `https://localhost:.../swagger`
   - Сначала вызвать `/api/auth/register` / `/api/auth/login`
   - Скопировать токен, нажать **Authorize** вверху, вставить `Bearer <токен>`
   - После этого все защищённые методы будут работать.

2. **Postman / Insomnia**
   - Создать запросы к тем же URL.
   - Вкладка **Authorization** → тип **Bearer Token** → вставить токен.

## 8. Как поменять порт

По умолчанию порт задаётся в `launchSettings.json` (его можно добавить, если нужно)  
или через переменную среды `ASPNETCORE_URLS`, либо аргументом:

```bash
dotnet run --urls "http://localhost:5000"
```

---

Этот backend — базовый каркас для курса/дипломного проекта.  
К нему легко добавить:

- общедоступные конспекты (шаринг между студентами),
- лайки/комментарии,
- поиск по предмету/ключевым словам,
- роли (админ, модератор и т.д.).
