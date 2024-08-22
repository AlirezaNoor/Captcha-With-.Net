using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using test.services;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaptchaController : ControllerBase
    {
        private readonly ICaptchaService _captchaService;
        private readonly Random _random;
        private readonly IMemoryCache _cache;

        public CaptchaController(ICaptchaService captchaService, IMemoryCache cache)
        {
            _captchaService = captchaService;
            _cache = cache;
            _random = new Random();
        }

        [HttpGet("generate")]
        public async Task<IActionResult> GenerateCaptcha()
        {
            try
            {
                var captchaText = GenerateRandomCaptchaText();
                var captchaId = Guid.NewGuid().ToString();

                // ذخیره در کش به مدت 10 دقیقه
                _cache.Set(captchaId, captchaText, TimeSpan.FromMinutes(4));

                var captchaImage = await _captchaService.GenerateCaptchaAsync(captchaText);
                if (captchaImage == null || captchaImage.Length == 0)
                {
                    return StatusCode(500, "Failed to generate CAPTCHA image.");
                }

                var base64Image = Convert.ToBase64String(captchaImage);

                return Ok(new
                {
                    captchaId = captchaId,
                    captchaImage = $"data:image/png;base64,{base64Image}"
                });
            }
            catch (Exception ex)
            {
                // ثبت خطا و بازگشت پاسخ خطا
                // LogException(ex); // این متد باید برای ثبت خطا پیاده‌سازی شود
                return StatusCode(500, "An error occurred while generating CAPTCHA.");
            }
        }

        [HttpPost("validate")]
        public IActionResult ValidateCaptcha([FromBody] CaptchaValidationRequest request)
        {
            try
            {
                if (_cache.TryGetValue(request.CaptchaId, out string storedCaptchaText))
                {
                    bool isValid = _captchaService.ValidateCaptcha(storedCaptchaText, request.UserInput);
                    return Ok(new { IsValid = isValid });
                }
                else
                {
                    // لاگ کردن شناسه کپچا و وضعیت کش
                    Console.WriteLine($"Captcha not found or expired. CaptchaId: {request.CaptchaId}");
                    return BadRequest("Captcha not found or expired.");
                }
            }
            catch (Exception ex)
            {
                // ثبت خطا و بازگشت پاسخ خطا
                // LogException(ex); // این متد باید برای ثبت خطا پیاده‌سازی شود
                return StatusCode(500, "An error occurred while validating CAPTCHA.");
            }
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
        public string CaptchaId { get; set; }
    }
}
