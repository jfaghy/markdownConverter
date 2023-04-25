using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace markdown2.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WordModController : ControllerBase
{
    [HttpPut]
    [Route("/word")]
    public IActionResult DownloadWord([FromBody] DownloadData downloadData) =>
        File(ModifyWordDoc(downloadData.Column1, downloadData.Column2), "application/octet-stream", "jacks.docx");

    /*private byte[] ModifyWordDoc(string data, string data2)
    {
        byte[] byteArray = System.IO.File.ReadAllBytes("E:\\doc.docx");
        using (var stream = new MemoryStream())
        {
            stream.Write(byteArray, 0, byteArray.Length);
            using (var doc = WordprocessingDocument.Open(stream, true))
            {
                // Get the main document part
                MainDocumentPart mainPart = doc.MainDocumentPart;

                // Get the document text
                string docText = mainPart.Document.Body.InnerText;

                // Replace the data tags with JSON data
                docText = docText.Replace("<DATATAG1>", JsonConvert.SerializeObject(data));
                docText = docText.Replace("<DATATAG2>", JsonConvert.SerializeObject(data2));

                // Update the document text
                mainPart.Document.Body.RemoveAllChildren();
                mainPart.Document.Body.Append(new Paragraph(new Run(new Text(docText))));

                // Save the changes
                mainPart.Document.Save(); // won't update the original file
            }
            // Save the file with the new name
            stream.Position = 0;
            //System.IO.File.WriteAllBytes("""C:\DataGlide\output.docx""", stream.ToArray());
            return stream.ToArray();
        }
    }*/

    private byte[] ModifyWordDoc(string data, string data2)
    {
        byte[] byteArray = System.IO.File.ReadAllBytes("E:\\doc.docx");
        using (var stream = new MemoryStream())
        {
            stream.Write(byteArray, 0, byteArray.Length);
            using (var doc = WordprocessingDocument.Open(stream, true))
            {
                MainDocumentPart mainPart = doc.MainDocumentPart;
                Body body = mainPart.Document.AppendChild(new Body());

                // Copy the contents of the input document to the output document
                foreach (var element in doc.MainDocumentPart.Document.Body.ChildElements)
                {
                    // Deep copy the element to preserve formatting
                    //var cloneElement = element.CloneNode(true);

                    // Replace the data tags with JSON data in the cloned element
                    foreach (var text in element.Descendants<Text>())
                    {
                        if (text.Text.Contains("<DATATAG1>"))
                        {
                            text.Text = text.Text.Replace("<DATATAG1>", JsonConvert.SerializeObject(data));
                        }
                        else if (text.Text.Contains("<DATATAG2>"))
                        {
                            text.Text = text.Text.Replace("<DATATAG2>", JsonConvert.SerializeObject(data2));
                        }
                    }
                }
            }

            // Save the file with the new name
            stream.Position = 0;
            //System.IO.File.WriteAllBytes("""C:\DataGlide\output.docx""", stream.ToArray());
            return stream.ToArray();
        }
    }
}
