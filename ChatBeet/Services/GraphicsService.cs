using Microsoft.AspNetCore.Hosting;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;

namespace ChatBeet.Services;
public class GraphicsService
{
    private readonly string WebRootPath;

    public GraphicsService(IWebHostEnvironment environment)
    {
        WebRootPath = environment.WebRootPath;
    }

    public async Task<Stream> BuildHighGroundImageAsync(string anakin, string obiWan)
    {
        using var image = await Image.LoadAsync(Path.Join(WebRootPath, "img/high_ground.webp"));
        var fontCollection = new FontCollection();
        var fontFamily = fontCollection.Add(Path.Join(WebRootPath, "font/impact.woff2"));
        var font = fontFamily.CreateFont(100);
        var fill = Brushes.Solid(Color.White);
        var outline = Pens.Solid(Color.Black, 3);

        TextOptions anakinOptions = new(font)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Origin = new(424, 391)
        };
        image.Mutate(x => x.DrawText(anakinOptions, anakin.ToUpper(), fill, outline));

        TextOptions obiWanOptions = new(font)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Origin = new(1411, 261)
        };
        image.Mutate(x => x.DrawText(obiWanOptions, obiWan.ToUpper(), fill, outline));


        var ms = new MemoryStream();
        var encoder = new WebpEncoder();
        await image.SaveAsync(ms, encoder);
        ms.Position = 0;
        return ms;
    }
}
