## Flujo del Código QR Generator API

1. **Entrada de Datos**: El método `Get` del controlador `QRCodeController` espera recibir datos a través de una solicitud HTTP GET. Los datos se proporcionan como parte de la URL.

2. **Validación de Datos**: Se comprueba si los datos proporcionados son válidos. Si los datos están vacíos o nulos, se devuelve una respuesta de error BadRequest.

3. **Generación del Código QR**:
   - Se crea una instancia del generador de códigos QR (`QRCodeGenerator`).
   - Se utiliza el generador para crear un objeto `QRCodeData` a partir de los datos proporcionados.
   - Se crea un objeto `QRCode` a partir de los datos del código QR.
   - Se genera un código único utilizando `Guid.NewGuid().ToString()`.

4. **Generación del SVG**:
   - Se crea un `MemoryStream` para almacenar la imagen del código QR en formato PNG.
   - Se obtiene la imagen del código QR como un objeto `Bitmap` y se guarda en el `MemoryStream`.
   - Se convierte la imagen PNG en un documento SVG utilizando el método `ConvertToSvg`.
   - El código único se inserta en el SVG como un texto visible en la parte inferior de la imagen.

5. **Generación de HTML**:
   - Se construye una página HTML dinámica que contiene el SVG del código QR y el código único generado.
   - Se utiliza Bootstrap para aplicar estilos al HTML generado, centrando el contenido verticalmente en la página.

6. **Respuesta de la API**:
   - La página HTML generada se devuelve como contenido de texto con un tipo de contenido "text/html".
   - Si ocurre algún error durante el proceso, se captura la excepción y se devuelve una respuesta de error con un código de estado 500 y un mensaje de error.

