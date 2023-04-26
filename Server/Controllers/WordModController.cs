using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace markdown2.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WordModController : ControllerBase
{
    [HttpPut]
    [Route("/word")]
    public IActionResult DownloadWord([FromBody] DownloadData downloadData) =>
        //File(ReplaceDataTagsInWordDocument(downloadData.Column1, downloadData.Column2), "application/octet-stream", "jacks.docx");
        File(ModifyWordDoc(downloadData.Column1, downloadData.Column2), "application/octet-stream", "jacks.docx");

    public byte[]  ReplaceDataTagsInWordDocument(string data, string data2)
    {
        byte[] byteArray = System.IO.File.ReadAllBytes("C:\\DataGlide\\template.docx");
        using (MemoryStream stream = new MemoryStream())
        {
            stream.Write(byteArray, 0, byteArray.Length);
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
            {
                /*// Find all text in the Word document
                    IEnumerable<Text> textElements = wordDoc.MainDocumentPart.Document.Descendants<Text>();
                    foreach (Text textElement in textElements)
                    {
                        // Check if the current text element contains the old data tag
                        if (textElement.Text.Contains("<DATATAG1>"))
                        {
                            // Replace the old data tag with the new data tag
                            textElement.Text = textElement.Text.Replace("<DATATAG1>", data);
                        }
                    }*/
                

                /*
                foreach (Text textElement in textElements)
                {
                    // Check if the current text element contains the old data tag
                    if (textElement.Text.Contains("<DATATAG1>"))
                    {
                        // Replace the old data tag with the new data tag
                        textElement.Text = textElement.Text.Replace("<DATATAG1>", data);
                    }
                }*/

                // Find all content controls in the Word document
                IEnumerable<SdtElement> contentControls = wordDoc.MainDocumentPart.Document.Descendants<SdtElement>();

                foreach (SdtElement cc in contentControls)
                {
                    // Check if the current content control contains the old data tag
                    Text textElement = cc.Descendants<Text>().FirstOrDefault();
                    if (textElement != null && textElement.Text.Contains("<DATATAG1>"))
                    {
                        // Replace the old data tag with the new data tag
                        textElement.Text = textElement.Text.Replace("<DATATAG1>", data);
                    }
                }
                
                // Save the updated Word document
                wordDoc.MainDocumentPart.Document.Save();
            }

            // Save the file with the new name
            stream.Position = 0;
            //System.IO.File.WriteAllBytes("""C:\DataGlide\output.docx""", stream.ToArray());
            return stream.ToArray();
        }
    }

    private byte[] ModifyWordDoc(string data, string data2)
    {
        byte[] byteArray = System.IO.File.ReadAllBytes("C:\\DataGlide\\templatecc.docx");
        using (MemoryStream stream = new ())
        {
            stream.Write(byteArray, 0, byteArray.Length);
            using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, true))
            {
                MainDocumentPart mainPart = doc.MainDocumentPart;

                foreach (OpenXmlElement element in mainPart.Document.Body.ChildElements)
                {
                    // Replace the data tags with JSON data in the cloned element
                    if (element.GetType() == typeof(Paragraph))
                    {
                        foreach (Text text in element.Descendants<Text>())
                        {
                            if (text.Text.Contains("<DATA1>"))
                            {
                                text.Text = text.Text.Replace("<DATA1>", JsonConvert.SerializeObject(data));
                            }
                            else if (text.Text.Contains("<DATATAG2>"))
                            {
                                text.Text = text.Text.Replace("<DATATAG2>", JsonConvert.SerializeObject(data2));
                            }
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
