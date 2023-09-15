using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;
using Dice;
using MoreLinq;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.PixelFormats;
using Path = System.IO.Path;

namespace ChatBeet.Services;

public class GraphicsService
{
    private readonly string _webRootPath;
    private const string FontPath = "font/impact.woff2";

    public GraphicsService(IWebHostEnvironment environment)
    {
        _webRootPath = environment.WebRootPath;
    }

    public async Task<Stream> BuildHighGroundImageAsync(string anakin, string obiWan)
    {
        using var image = await Image.LoadAsync(Path.Join(_webRootPath, "img/high_ground.webp"));
        var fontCollection = new FontCollection();
        var fontFamily = fontCollection.Add(Path.Join(_webRootPath, FontPath));
        var font = fontFamily.CreateFont(100);
        var fill = Brushes.Solid(Color.White);
        var outline = Pens.Solid(Color.Black, 3);

        RichTextOptions anakinOptions = new(font)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Origin = new(424, 391)
        };
        image.Mutate(x => x.DrawText(anakinOptions, anakin.ToUpper(), fill, outline));

        RichTextOptions obiWanOptions = new(font)
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

    public async Task<Stream> BuildDiceRollImageAsync(RollResult result, Color? color = null)
    {
        const int diceTileSize = 48;
        const int polygonRadius = 20;
        const int fontSize = 14;

        var valuesToShow = result.Values
            .Where(v => v.DieType == DieType.Normal)
            .ToList();

        using var image = new Image<Rgba32>(diceTileSize * valuesToShow.Count, diceTileSize);
        var backgroundColor = color ?? Color.DarkGray;
        var foregroundColor = backgroundColor.ToPixel<L8>().PackedValue > byte.MaxValue / 2
            ? Color.Black
            : Color.White;

        var fontCollection = new FontCollection();
        var fontFamily = fontCollection.Add(Path.Join(_webRootPath, FontPath));
        var font = fontFamily.CreateFont(fontSize);

        valuesToShow.ForEach((die, index) => DrawDie(index, die));
        
        var ms = new MemoryStream();
        var encoder = new WebpEncoder();
        await image.SaveAsync(ms, encoder);
        ms.Position = 0;
        return ms;

        void DrawDie(int index, DieResult die)
        {
            var shapeOrigin = new PointF((float)(0.5 * diceTileSize + index * diceTileSize), (float)(0.5 * diceTileSize));
            var polygon = new RegularPolygon(shapeOrigin, GetDieVertexCount(die.NumSides), polygonRadius, (float)(Random.Shared.NextSingle() * Math.PI * 2));
            image.Mutate(i => i.Fill(backgroundColor, polygon));
            var textOrigin = new PointF(shapeOrigin.X, shapeOrigin.Y - fontSize / 2);
            var textOptions = new RichTextOptions(font)
            {
                Origin = textOrigin,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            image.Mutate(i => i.DrawText(textOptions, die.Value.ToString(CultureInfo.InvariantCulture), foregroundColor));
        }

        int GetDieVertexCount(int dieSides) => dieSides switch
        {
            4 => 3, // tetrahedron
            6 => 4, // cube
            8 => 3, // octahedron
            10 => 5, // pentagonal trapezohedron
            12 => 5, // dodecahedron
            20 => 6, // icosahedron (looks like hexagon from front)
            _ => 12 // idfk
        };
    }
}