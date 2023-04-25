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
    [Route("/downloadfakehappy")]
    public IActionResult DinkDownload([FromBody] DownloadData downloadData)
    {
	    string html = CreateHtml(downloadData.Column1, downloadData.Column2);
	    return File(CreatePdfFromHtml(html), "application/octet-stream", "jacksPdf.pdf");
    }

    [HttpPut]
    [Route("/downloaddink")]
    public IActionResult Download([FromBody] DownloadData downloadData)
    {
        string html = CreateHtml2(downloadData.Column1, downloadData.Column2);
        return File(CreatePdfFromHtml(html), "application/octet-stream", "jacksPdf.pdf");
    }

    [HttpPut]
    [Route("/downloaddinkjs")]
    public IActionResult DownloadJs([FromBody] DownloadData downloadData)
    {
        string html = CreateHtmlJs(downloadData.Column1, downloadData.Column2);
        return File(CreatePdfFromHtml(html), "application/octet-stream", "jacksPdf.pdf");
    }

    [HttpPut]
    [Route("/downloadnreco")]
    public IActionResult NRecoDownload([FromBody] DownloadData downloadData)
    {
	    string html = CreateNRecoHtml(downloadData.Column1, downloadData.Column2);
	    return File(NRecoPdf(html), "application/octet-stream", "jacksPdf.pdf");
    }

    private string CreateHtml(string column1, string column2)
    {
	    string htmlTemplate = GetHtmlTemplate();
	    htmlTemplate = htmlTemplate.Replace("<!-- Content for the first column -->", column1);
	    htmlTemplate = htmlTemplate.Replace("<!-- Content for the second column -->", column2);
	    Console.WriteLine(htmlTemplate);
	    return htmlTemplate;
    }

    private string CreateHtmlJs(string column1, string column2)
    {
	    string htmlTemplate = GetHtmlTemplateWithJS();
	    StringBuilder sb = new();
	    sb.Append(column1);
	    sb.Append(column2);
	    string s = sb.ToString();
	    htmlTemplate = htmlTemplate.Replace("<!-- Content --!>", s);
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

    private string CreateNRecoHtml(string column1, string column2)
    {
	    string htmlTemplate = GetHtmlTemplateWithStyle();
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
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = """C:\DataGlide\markdown2\Server\css\format.css""", EnableJavascript = true},
                    HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 },
                    FooterSettings = { FontSize = 9, Line = true, Spacing = 2.812, Left = "Printed: [date] [time]", Right = "Page [page] of [toPage]" }
                }
            }
        };
        byte[] pdfFromHtml = converter.Convert(doc);
        return pdfFromHtml;
    }

    private byte[] NRecoPdf(string html)
    {
	    var pdfBytes = (new NReco.PdfGenerator.HtmlToPdfConverter()).GeneratePdf(html);
	    return pdfBytes;
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

    private string GetHtmlTemplateWithJS() => """
		<!DOCTYPE html>
		<html>
		<head>
			<meta name="viewport" content="width=device-width, initial-scale=1">
			<script>
				const textContainer = document.getElementById("text-container");
				const availableHeight = textContainer.clientHeight;

				if (textContainer.scrollHeight > availableHeight) {
				  textContainer.classList.add("three-columns");
				}
			</script>
		</head>
		<body>
			<div id="text-container">
					<!-- Content --!>
			</div>
		</body>
		</html>
		""";

    private string GetHtmlTemplate2() => """
		<!DOCTYPE html>
		<html>
		<body>
			<div class="two-columns">
				<!-- Content -->
			</div>
		</body>
		</html>
		""";

    private string GetHtmlTemplateWithStyle() => """
		<!DOCTYPE html>
		<html>
			<head>
				<style>
				.two-columns {
				    column-count: 2;
				    column-gap: 20px;
				    column-fill: auto;
				    height: 1000px;
				}
				body {
				    font-family: Arial, sans-serif;
				}

				/* Make headers red */
				h1, h2, h3, h4, h5, h6 {
				    color: blueviolet;
				}
				</style>
			</head>
		<body>
			<div class="two-columns">
				<!-- Content -->
			</div>
		</body>
		</html>
		""";
}

