using DinkToPdf;
using Microsoft.AspNetCore.Mvc;
using markdown2.Shared;

namespace markdown2.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpPut]
    [Route("/download")]
    public IActionResult Download([FromBody] DownloadData downloadData)
    {
        return File(CreatePdfFromHtml(downloadData.Html), "application/octet-stream", "jacksPdf.pdf");
    }

    private byte[] CreatePdfFromHtml(string html)
    {
        var converter = new SynchronizedConverter(new PdfTools());
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
            },
            Objects = {
                new ObjectSettings() {
                    PagesCount = true,
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 },
                    FooterSettings = { FontSize = 9, Line = true, Spacing = 2.812, Left = "Printed: [date] [time]", Right = "Page [page] of [toPage]" }
                }
            }
        };
        byte[] pdfFromHtml = converter.Convert(doc);
        return pdfFromHtml;
    }
}

public record DownloadData(string Html);

