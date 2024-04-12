using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Xml.Linq;
using System.Text;

namespace QRCodeGeneratorAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QRCodeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(string data)
        {
            try
            {
                if (string.IsNullOrEmpty(data))
                {
                    return BadRequest("Por favor, proporciona datos para generar el código QR.");
                }

                // Crear instancia del generador de códigos QR
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                // Generar un código único
                string codigoUnico = Guid.NewGuid().ToString();

                // Crear instancia del renderer SVG
                using (MemoryStream ms = new MemoryStream())
                {
                    // Obtener el código QR como imagen Bitmap
                    using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                    {
                        // Guardar la imagen en un MemoryStream
                        qrCodeImage.Save(ms, ImageFormat.Png);
                    }

                    // Convertir la imagen a SVG usando XDocument
                    ms.Position = 0; // Resetear la posición del MemoryStream
                    XDocument svgDocument = ConvertToSvg(ms, codigoUnico);

                    // Convertir el documento SVG a una cadena
                    string svgString = svgDocument.ToString();

                    // Generar el HTML que muestra el SVG
                    StringBuilder htmlBuilder = new StringBuilder();

                    htmlBuilder.Append("<!DOCTYPE html>");
                    htmlBuilder.Append("<html>");
                    htmlBuilder.Append("<head>");
                    htmlBuilder.Append("<title>Generación de Código QR</title>");
                    htmlBuilder.Append("<link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css\">");
                    htmlBuilder.Append("<style>");
                    htmlBuilder.Append(".container { display: flex; justify-content: center; align-items: center; height: 100vh; flex-direction: column; }"); // Cambiar la dirección del contenedor a columna
                    htmlBuilder.Append(".qr-code { margin: 20px auto; }"); // Agregar margen alrededor del código QR
                    htmlBuilder.Append("</style>");
                    htmlBuilder.Append("</head>");
                    htmlBuilder.Append("<body>");
                    htmlBuilder.Append("<div class=\"container\">");
                    htmlBuilder.Append("<div class=\"text-center\">");
                    htmlBuilder.Append("</div>");
                    htmlBuilder.Append("<div class=\"qr-code\">"); // Div para el código QR
                    htmlBuilder.Append(svgString); // Agregar el SVG al cuerpo del HTML
                    htmlBuilder.Append("</div>");
                    htmlBuilder.Append($"<div>Codigo unico: {codigoUnico}</div>"); // Agregar el código único en una línea separada
                    htmlBuilder.Append("</div>");
                    htmlBuilder.Append("</body>");
                    htmlBuilder.Append("</html>");


                    // Devolver la página HTML como contenido del texto
                    return Content(htmlBuilder.ToString(), "text/html");
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción y devolver un código de estado 500 con un mensaje de error
                return StatusCode(500, $"Ha ocurrido un error al generar el código QR: {ex.Message}");
            }
        }

        private XDocument ConvertToSvg(Stream imageStream, string codigoUnico)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convertir la imagen PNG a SVG usando QRCoder
                using (var qrCodeImage = new Bitmap(imageStream))
                {
                    // Guardar la imagen en un MemoryStream
                    qrCodeImage.Save(ms, ImageFormat.Png);
                }

                // Resetear la posición del MemoryStream para leer desde el inicio
                ms.Position = 0;

                // Convertir la imagen PNG a SVG utilizando XDocument
                var svgDocument = new XDocument(
                    new XElement(XName.Get("svg", "http://www.w3.org/2000/svg"),
                        new XAttribute("version", "1.1"),
                        new XElement("image",
                            new XAttribute("xlink", "http://www.w3.org/1999/xlink"),
                            new XAttribute("x", "0"),
                            new XAttribute("y", "0"),
                            new XAttribute("width", "100%"),
                            new XAttribute("height", "100%"),
                            new XAttribute("href", $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}")
                        ),
                        new XElement("text", codigoUnico, new XAttribute("x", "50%"), new XAttribute("y", "95%"), new XAttribute("fill", "black"), new XAttribute("font-size", "12px"), new XAttribute("text-anchor", "middle"))
                    )
                );

                return svgDocument;
            }
        }
    }
}
