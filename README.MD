# CAPTCHA Service

## Overview

This project is an ASP.NET Core Web API designed for generating and validating CAPTCHAs. It utilizes the `ImageSharp` library for CAPTCHA image generation and stores CAPTCHA texts in memory cache. The project is containerized using Docker for easier deployment and management.

## Features

- Generate CAPTCHAs with random text.
- Validate user input against the generated CAPTCHAs.
- Base64-encoded CAPTCHA image for easy embedding in web applications.
- Docker setup for building and running the application.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Installation

### 1. Clone the Repository

To get started, clone the repository:

```bash
git clone https://github.com/your-repo/captcha-service.git
cd captcha-service
```
### 2. Build and Run with Docker
   Ensure Docker and Docker Compose are installed on your system. Build and run the Docker containers using:
```bash
docker-compose build
docker-compose up
```
This command builds the Docker images and starts the containers as specified in the docker-compose.yml file.


### 3. Running Locally without Docker
   If you prefer to run the application locally without Docker, make sure you have .NET 8 SDK installed, then run:
   ```bash
dotnet restore
dotnet run
```


The application will start and listen on http://localhost:5000 by default.



## Usage
Generate CAPTCHA
To generate a new CAPTCHA, send a GET request to the /api/captcha/generate endpoint:

Request:

```bash
GET /api/captcha/generate
```

Response:

```bash
{
  "captchaId": "unique-captcha-id",
  "captchaImage": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUg..."
}
```
captchaId: A unique identifier for the CAPTCHA.
captchaImage: Base64-encoded CAPTCHA image, which can be embedded directly in HTML.
Validate CAPTCHA
To validate user input against a CAPTCHA, send a POST request to the /api/captcha/validate endpoint with the CAPTCHA ID and user input:

Request:
```bash
POST /api/captcha/validate
Content-Type: application/json

{
  "captchaId": "unique-captcha-id",
  "userInput": "user-entered-text"
}
```
Response:

```bash
{
  "isValid": true
}
```
isValid: A boolean indicating whether the user input matches the CAPTCHA text.
## Error Handling
400 Bad Request: "Captcha not found or expired." – The provided CAPTCHA ID is either invalid or has expired.
500 Internal Server Error: An unexpected error occurred. Please check the server logs for more details.
Docker Configuration
## Dockerfile
The Dockerfile defines the steps to build the application image. It uses the official .NET SDK and ASP.NET runtime images to build and run the application.

## docker-compose.yml
The docker-compose.yml file sets up the services needed for the application, including the web application container, and configures networking between services.

## Contributing
We welcome contributions to the project. To contribute:

##Fork the repository.
Create a new branch for your feature or bug fix.
Commit your changes and push to your forked repository.
Submit a pull request detailing your changes.
License
This project is licensed under the MIT License. See the LICENSE file for more details.

## Contact
For questions or support, please contact alirezang7575@gmial.com.


### توضیحات

- **Overview**: توضیحاتی درباره‌ی پروژه و ویژگی‌های اصلی آن.
- **Prerequisites**: نیازمندی‌های لازم برای اجرای پروژه.
- **Installation**: راهنمای نصب و راه‌اندازی پروژه، شامل اجرای Docker و بدون Docker.
- **Usage**: نحوه استفاده از API برای تولید و اعتبارسنجی کپچا.
- **Error Handling**: مدیریت خطاهای متداول.
- **Docker Configuration**: توضیحات درباره‌ی فایل‌های Docker برای پیکربندی پروژه.
- **Contributing**: نحوه مشارکت در پروژه.
- **License**: اطلاعات مربوط به لایسنس.
- **Contact**: اطلاعات تماس برای پشتیبانی.

این ساختار به شما کمک می‌کند تا مستندات پروژه را به طور کامل و قابل فهم برای سایرین ارائه دهید.
