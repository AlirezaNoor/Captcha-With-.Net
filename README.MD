# CaptchaService

## Overview

`CaptchaService` is a simple service to generate CAPTCHA images using the `SixLabors.ImageSharp` library. This service includes functionality to create CAPTCHA images with random noise and validate user input against the generated CAPTCHA code.

## Features

- **Generate CAPTCHA Images:** Generates CAPTCHA images with random noise to enhance security.
- **Validate User Input:** Compares user input against the generated CAPTCHA code.

## Prerequisites

- .NET 6.0 or later
- NuGet packages:
    - `SixLabors.ImageSharp`
    - `SixLabors.Fonts`

## Installation

1. **Create a new ASP.NET Core Web API project** (if you don't have one):

    ```bash
    dotnet new webapi -n CaptchaDemo
    cd CaptchaDemo
    ```

2. **Install the required NuGet packages**:

    ```bash
    dotnet add package SixLabors.ImageSharp
    dotnet add package SixLabors.Fonts
    ```

3. **Add your font file**:
   Place your font file (e.g., `ArizonaBold.ttf`) in a folder named `fonts` inside the `wwwroot` directory of your project.

4. **Add the CAPTCHA service code**:
   Create a folder named `Services` and add the following `CaptchaService.cs` file:

    ```csharp
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Drawing.Processing;
    using SixLabors.ImageSharp.Formats.Png;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using SixLabors.Fonts;
    using Microsoft.Extensions.Hosting;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    namespace CaptchaDemo.Services
    {
        public interface ICaptchaService
        {
            Task<byte[]> GenerateCaptchaAsync(string captchaText);
            bool ValidateCaptcha(string captchaText, string userInput);
        }

        public class CaptchaService : ICaptchaService
        {
            private readonly Random _random = new Random();
            private readonly string _backupFolder;

            public CaptchaService(IWebHostEnvironment environment)
            {
                _backupFolder = Path.Combine(environment.WebRootPath, "fonts");
            }

            public async Task<byte[]> GenerateCaptchaAsync(string captchaText)
            {
                using (var image = new Image<Rgba32>(200, 100))
                {
                    var filePath = Path.Combine(_backupFolder, "ArizonaBold.ttf");

                    if (!File.Exists(filePath)) return new byte[2];

                    var collection = new FontCollection();
                    var family = collection.Add(filePath);
                    var font = family.CreateFont(38, FontStyle.Italic);

                    image.Mutate(x => x.Fill(Color.White));
                    image.Mutate(x => x.DrawText(captchaText, font, Color.Black, new PointF(13, 19)));

                    AddNoise(image);
                    AddLines(image);

                    using (var ms = new MemoryStream())
                    {
                        await image.SaveAsync(ms, new PngEncoder());
                        return ms.ToArray();
                    }
                }
            }

            public bool ValidateCaptcha(string captchaText, string userInput)
            {
                return captchaText.Equals(userInput, StringComparison.OrdinalIgnoreCase);
            }

            private void AddNoise(Image<Rgba32> image)
            {
                var random = new Random();
                int width = image.Width;
                int height = image.Height;
                int noisePoints = (width * height) / 100;

                for (int i = 0; i < noisePoints; i++)
                {
                    int x = random.Next(width);
                    int y = random.Next(height);
                    var color = new Rgba32((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(100, 255));
                    image[x, y] = color;
                }
            }

            private void AddLines(Image<Rgba32> image)
            {
                var random = new Random();
                int width = image.Width;
                int height = image.Height;
                int lineCount = (width * height) / 500;

                for (int i = 0; i < lineCount; i++)
                {
                    var x1 = random.Next(width);
                    var y1 = random.Next(height);
                    var x2 = random.Next(width);
                    var y2 = random.Next(height);
                    var color = new Rgba32((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(100, 255));
                    image.Mutate(ctx => ctx.DrawLines(color, 1, new PointF(x1, y1), new PointF(x2, y2)));
                }
            }
        }
    }
    ```

5. **Register the CAPTCHA service in `Program.cs` or `Startup.cs`**:

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddScoped<ICaptchaService, CaptchaService>();
        services.AddSingleton<IWebHostEnvironment>(provider => provider.GetRequiredService<IWebHostEnvironment>());
    }
    ```

6. **Create a CAPTCHA controller**:

    ```csharp
    using CaptchaDemo.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    namespace CaptchaDemo.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class CaptchaController : ControllerBase
        {
            private readonly ICaptchaService _captchaService;

            public CaptchaController(ICaptchaService captchaService)
            {
                _captchaService = captchaService;
            }

            [HttpGet]
            public async Task<IActionResult> GetCaptcha([FromQuery] string captchaText)
            {
                var captchaImage = await _captchaService.GenerateCaptchaAsync(captchaText);
                return File(captchaImage, "image/png");
            }

            [HttpPost("verify")]
            public IActionResult VerifyCaptcha([FromBody] CaptchaValidationRequest request)
            {
                bool isValid = _captchaService.ValidateCaptcha(request.CaptchaText, request.UserInput);
                return Ok(isValid ? "Captcha verification succeeded." : "Captcha verification failed.");
            }
        }

        public class CaptchaValidationRequest
        {
            public string CaptchaText { get; set; }
            public string UserInput { get; set; }
        }
    }
    ```

## Usage

### Generating CAPTCHA

Send a `GET` request to `/api/captcha` with a query parameter for `captchaText` to receive a CAPTCHA image.

Example request:

GET /api/captcha?captchaText=ABCD

### Verifying CAPTCHA

Send a `POST` request to `/api/captcha/verify` with a JSON body containing the `CaptchaText` and `UserInput` to verify the CAPTCHA.

### توضیحات

- **Overview**: بخش کلی درباره سرویس.
- **Features**: ویژگی‌های اصلی سرویس.
- **Prerequisites**: پیش‌نیازها برای اجرای سرویس.
- **Installation**: مراحل نصب و پیکربندی سرویس.
- **Usage**: نحوه استفاده از سرویس برای تولید و تأیید CAPTCHA.
- **Notes**: نکات مهم و توصیه‌ها.
- **License**: اطلاعات لایسنس.

این مستندات به کاربران و توسعه‌دهندگان کمک می‌کند تا به راحتی سرویس CAPTCHA را در پروژه‌های خود پیاده‌سازی کنند و از آن استفاده کنند.

 
