using System.Data;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;

namespace GestionProveedores.Reportes
{
    // Un solo generador sirve para los 5 reportes: recibe el título, el filtro aplicado,
    // el usuario que lo genera y la tabla de datos.
    public static class GeneradorPdf
    {
        public static void Generar(string rutaArchivo, string titulo, string filtro,
                                   string usuario, DataTable datos)
        {
            // En Windows se usan las fuentes instaladas (Arial, etc.)
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;

            var documento = new Document();
            documento.Info.Title = titulo;

            Style normal = documento.Styles[StyleNames.Normal]!;
            normal.Font.Name = "Arial";
            normal.Font.Size = 10;

            Section seccion = documento.AddSection();
            seccion.PageSetup.PageFormat = PageFormat.A4;
            seccion.PageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Landscape;
            seccion.PageSetup.LeftMargin = Unit.FromCentimeter(1.5);
            seccion.PageSetup.RightMargin = Unit.FromCentimeter(1.5);
            seccion.PageSetup.TopMargin = Unit.FromCentimeter(1.5);
            seccion.PageSetup.BottomMargin = Unit.FromCentimeter(1.5);

            // Encabezado del reporte
            Paragraph pTitulo = seccion.AddParagraph(titulo);
            pTitulo.Format.Font.Size = 16;
            pTitulo.Format.Font.Bold = true;
            pTitulo.Format.SpaceAfter = Unit.FromCentimeter(0.3);

            seccion.AddParagraph("Filtro: " + filtro);

            Paragraph pDatos = seccion.AddParagraph(
                $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}   ·   Usuario: {usuario}");
            pDatos.Format.SpaceAfter = Unit.FromCentimeter(0.5);

            // Tabla con los datos
            Table tabla = seccion.AddTable();
            tabla.Borders.Width = 0.5;
            tabla.Borders.Color = Colors.Gray;
            tabla.Format.Font.Size = 9;

            double anchoUtil = 26.7;   // A4 horizontal (29.7 cm) menos los márgenes
            double anchoColumna = anchoUtil / datos.Columns.Count;
            foreach (DataColumn columna in datos.Columns)
            {
                tabla.AddColumn(Unit.FromCentimeter(anchoColumna));
            }

            Row encabezado = tabla.AddRow();
            encabezado.HeadingFormat = true;    // se repite en cada página
            encabezado.Shading.Color = Colors.LightGray;
            encabezado.Format.Font.Bold = true;
            for (int i = 0; i < datos.Columns.Count; i++)
            {
                encabezado.Cells[i].AddParagraph(datos.Columns[i].ColumnName);
            }

            foreach (DataRow fila in datos.Rows)
            {
                Row nueva = tabla.AddRow();
                for (int i = 0; i < datos.Columns.Count; i++)
                {
                    nueva.Cells[i].AddParagraph(Texto(fila[i]));
                }
            }

            // Total de filas
            Paragraph pTotal = seccion.AddParagraph($"Total de filas: {datos.Rows.Count}");
            pTotal.Format.SpaceBefore = Unit.FromCentimeter(0.4);
            pTotal.Format.Font.Bold = true;

            // Pie de página con el número de página
            Paragraph pie = seccion.Footers.Primary.AddParagraph();
            pie.Format.Alignment = ParagraphAlignment.Center;
            pie.Format.Font.Size = 8;
            pie.AddText("Página ");
            pie.AddPageField();
            pie.AddText(" de ");
            pie.AddNumPagesField();

            var generador = new PdfDocumentRenderer { Document = documento };
            generador.RenderDocument();
            generador.PdfDocument.Save(rutaArchivo);
        }

        private static string Texto(object valor)
        {
            return valor switch
            {
                DBNull => string.Empty,
                DateTime fecha => fecha.ToString("dd/MM/yyyy"),
                _ => Convert.ToString(valor) ?? string.Empty
            };
        }
    }
}