using System.Text;
using DinkToPdf;
using Microsoft.AspNetCore.Mvc;

namespace markdown2.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class PdfGeneratorController : ControllerBase
{

    private readonly ILogger<PdfGeneratorController> _logger;

    public PdfGeneratorController(ILogger<PdfGeneratorController> logger)
    {
        _logger = logger;
    }

    [HttpPut]
    [Route("/download")]
    public IActionResult Download([FromBody] DownloadData downloadData)
    {
        string html = CreateHtml(downloadData.Column1, downloadData.Column2);
        //string html = CreateHtml2(downloadData.Column1, downloadData.Column2);
        return File(CreatePdfFromHtml(html), "application/octet-stream", "jacksPdf.pdf");
    }

    private string CreateHtml(string column1, string column2)
    {
	    string htmlTemplate = GetHtmlTemplate();
	    htmlTemplate = htmlTemplate.Replace("<!-- Content for the first column -->", column1);
	    htmlTemplate = htmlTemplate.Replace("<!-- Content for the second column -->", column2);
	    Console.WriteLine(htmlTemplate);
	    return htmlTemplate;
    }

    private string CreateHtml2(string column1, string column2)
    {
	    string htmlTemplate = GetHtmlTemplate2();
	    StringBuilder sb = new();
	    sb.Append(column1);
	    sb.Append(column2);
	    string s = sb.ToString();
	    htmlTemplate = htmlTemplate.Replace("<!-- Content -->", s);
	    Console.WriteLine(htmlTemplate);
	    return htmlTemplate;
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
                Margins = new MarginSettings() { Top = 10 },
            },
            Objects = {
                new ObjectSettings() {
                    PagesCount = true,
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = """C:\DataGlide\markdown2\Server\css\format.css""" },
                    HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 },
                    FooterSettings = { FontSize = 9, Line = true, Spacing = 2.812, Left = "Printed: [date] [time]", Right = "Page [page] of [toPage]" }
                }
            }
        };
        byte[] pdfFromHtml = converter.Convert(doc);
        return pdfFromHtml;
    }

    private string GetHtmlTemplate() => """
		<!DOCTYPE html>
		<html>
		<head>
			<meta name="viewport" content="width=device-width, initial-scale=1">
		</head>
		<body>
			<div class="columns">
				<div class="left-column" style="background-color:#aaa;">
					<!-- Content for the first column -->
				</div>
				<div class="right-column" style="background-color:#eee;">
					<!-- Content for the second column -->
				</div>
			</div>
		</body>
		</html>
		""";

    private string GetHtmlTemplate2() => """
		<!DOCTYPE html>
		<html>
		<head>
			<meta name="viewport" content="width=device-width, initial-scale=1">
		</head>
		<body>
			<div class="columns">
				 <div class="content">
					<!-- Content -->
				</div>
			</div>
		</body>
		</html>
		""";
}

public record DownloadData(string Column1, string Column2);

