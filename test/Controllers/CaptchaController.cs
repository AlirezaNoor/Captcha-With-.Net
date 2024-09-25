using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
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
        private readonly IDatabase _redis;

        public CaptchaController(ICaptchaService captchaService, IConnectionMultiplexer redis)
        {
            _captchaService = captchaService;
            _random = new Random();
            _redis = redis.GetDatabase();
        }

        [HttpGet("generate")]
        public async Task<IActionResult> GenerateCaptcha()
        {
            try
            {
                var captchaText = GenerateRandomCaptchaText();
                var captchaId = Guid.NewGuid().ToString();

                // ذخیره در ردیس به مدت 4 دقیقه
                await _redis.StringSetAsync(captchaId, captchaText, TimeSpan.FromMinutes(4));

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
                return StatusCode(500, "An error occurred while generating CAPTCHA.");
            }
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateCaptcha([FromBody] CaptchaValidationRequest request)
        {
            try
            {
                // بازیابی متن کپچا از ردیس
                var storedCaptchaText = await _redis.StringGetAsync(request.CaptchaId);

                if (!storedCaptchaText.IsNullOrEmpty)
                {
                    bool isValid = _captchaService.ValidateCaptcha(storedCaptchaText, request.UserInput);
                    return Ok(new { IsValid = isValid });
                }
                else
                {
                    // لاگ کردن شناسه کپچا و وضعیت ردیس
                    Console.WriteLine($"Captcha not found or expired. CaptchaId: {request.CaptchaId}");
                    return BadRequest("Captcha not found or expired.");
                }
            }
            catch (Exception ex)
            {
                // ثبت خطا و بازگشت پاسخ خطا
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
