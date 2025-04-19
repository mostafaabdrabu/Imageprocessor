# 🖼️ Image Processor API

A clean-architecture structured, production-ready .NET 8 Web API for image upload, resizing, metadata extraction, and downloading.

---

## 📦 Features

- Upload multiple images (`JPG`, `PNG`, `WebP`) with a max size of 2MB each.
- Automatically converts images to `WebP` and resizes them for:
  - 📱 Phones
  - 💻 Tablets
  - 🖥️ Desktops
- Extracts EXIF metadata (Geo info, Camera Make & Model) and stores it as JSON.
- Download resized images via unique IDs.
- Retrieve image metadata via unique IDs.
- Built-in:
  - ✅ API Versioning.
  - 🔐 Basic Authentication.
  - 🧢 Rate Limiting (5 requests per IP per minute).
  - 🐳 Dockerized for easy deployment.

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Docker (optional for containerized deployment)

---

### 💻 Run Locally

```bash
git clone https://github.com/mostafaabdrabu/Imageprocessor
cd ImageProcessor
dotnet build
dotnet run --project src/WebApi/WebApi.csproj
```

#### 🧪 Using Swagger

once running, open your browser:

```bash
https://localhost:7200/swagger
```
or 
```bash
http://localhost:5200/swagger
```

* Requires **Basic Authentication** to try endpoints.
* Default credentials are stored in `appsettings.json`

```json
  "Authentication": {
    "BasicAuth": {
      "Username": "admin",
      "Password": "password123"
    }
  }
```
#### 🐳 Docker Build & Run

Build the Docker image:

```bash
  docker build -t image-processor-api
```

Run the container:
```bash
  docker run -d -p 5000:5000 -p 5001:5001 --name image-api image-processor-api
```

---
### 🔐 Security
* Basic Auth: configured in `appsettings.json`.
* Rate Limiting: 5 requests per IP per 60 seconds (HTTP `429` returned if exceeded).

---
### 📂 Project Structure

```css
src/
├── Application/
├── Domain/
├── Infrastructure/
├── Presentation/
├── WebApi/
```
Following Clean Architecture principles:
* `Application`: interfaces and use cases.
* `Domain`: core entities and value objects.
* `Infrastructure`: concrete implementations (image processing, storage, metadata).
* `Presentation`: API controllers.
* `Web`: program entry, configuration, dependency injection.

---
### 📌 Endpoints Overview
| Method | Endpoint             | Description     |
|-------:|---------------------:|----------------:|
| POST   | `/api/v1/image/upload` | 	Upload images.|
| GET   | `/api/v1/image/{imageId}/download/{size}` | 	Download resized image (`phone`, `tablet`, `desktop`).|
| GET   | `/api/v1/image/{imageId}/metadata` | 	metadata	Get image metadata.|
