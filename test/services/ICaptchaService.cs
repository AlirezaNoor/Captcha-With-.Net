using System.Drawing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;
using PointF = SixLabors.ImageSharp.PointF;
using SixLabors.Fonts;
using Font = SixLabors.Fonts.Font;
using FontFamily = SixLabors.Fonts.FontFamily;
using FontStyle = SixLabors.Fonts.FontStyle;

namespace test.services;
public interface ICaptchaService
{
    Task<byte[]> GenerateCaptchaAsync(string captchaText);
    bool ValidateCaptcha(string captchaText, string userInput);
}

public class CaptchaService : ICaptchaService
{
    private readonly Random _random = new Random();
    private readonly string _backupFolder = Path.Combine(Directory.GetCurrentDirectory(), "font");
    public async Task<byte[]> GenerateCaptchaAsync(string captchaText)
    {

        using (var image = new Image<Rgba32>(200, 100))
        {
            var filePath = Path.Combine(_backupFolder, "ArizonaBold.ttf");

            // بررسی وجود فایل
            if (!System.IO.File.Exists(filePath)) return new byte[2];

            // خواندن محتوای فایل
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            FontCollection collection = new();
            FontFamily family = collection.Add(filePath);
            Font font = family.CreateFont(38, FontStyle.Italic);
            // پس‌زمینه سفید و متن مشکی را اضافه می‌کنیم
            image.Mutate(x => x.Fill(Color.White)); // تنظیم پس‌زمینه سفید
            image.Mutate(x => x.DrawText(captchaText, font, Color.Black, new PointF(13, 19)));

            // اضافه کردن نویز
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

        // Define the number of noise points
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

    // Define the number of noise lines
    int lineCount = (width * height) / 500;

    for (int i = 0; i < lineCount; i++)
    {
        var x1 = random.Next(width);
        var y1 = random.Next(height);
        var x2 = random.Next(width);
        var y2 = random.Next(height);
        var color = new Rgba32((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(100, 255));
        image.Mutate(ctx => ctx.DrawLine(color,1,new PointF(x1, y1), new PointF(x2, y2)));
    }
}

}