using Microsoft.AspNetCore.Mvc;
using test.services;

namespace test.Controllers;

[ApiController]
    [Route("api/[controller]")]
    public class CaptchaController : ControllerBase
    {
        private readonly ICaptchaService _captchaService;
        private readonly Random _random;

        public CaptchaController(ICaptchaService captchaService)
        {
            _captchaService = captchaService;
            _random = new Random();
        }

        [HttpGet("generate")]
        public async Task<IActionResult> GenerateCaptcha()
        {
            var captchaText = GenerateRandomCaptchaText();
            var captchaImage = await _captchaService.GenerateCaptchaAsync(captchaText);

            // ذخیره‌سازی کپچا برای استفاده در اعتبارسنجی
            // این روش باید جایگزین با روش مناسب ذخیره‌سازی مانند cache یا session شود

            // برای مثال، با استفاده از Session (شما باید session را در برنامه فعال کنید)
            HttpContext.Session.SetString("CaptchaText", captchaText);

            return File(captchaImage, "image/png");
        }

        [HttpPost("validate")]
        public IActionResult ValidateCaptcha([FromBody] CaptchaValidationRequest request)
        {
            var captchaText = HttpContext.Session.GetString("CaptchaText");
            
            if (captchaText == null)
            {
                return BadRequest("Captcha not found.");
            }

            bool isValid = _captchaService.ValidateCaptcha(captchaText, request.UserInput);
            return Ok(new { IsValid = isValid });
        }

        private string GenerateRandomCaptchaText()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[6];
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[_random.Next(chars.Length)];
            }
            return new string(stringChars);
        }
    }

    public class CaptchaValidationRequest
    {
        public string UserInput { get; set; }
    }