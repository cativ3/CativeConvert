using CativeConvert.Commands.Abstractions;
using DinkToPdf;
using HtmlAgilityPack;
using System.Reflection;

namespace CativeConvert.Commands
{
    internal class ConvertToPdfCommand<T> : IConvertToPdfCommand<T>
    {
        private const string TITLE_VAR = "[[title]]";

        private const string PDF_THEMES_DIR = @"PdfThemes\LightTheme.html";
        private const string CSS_DIR = @"PdfThemes\css\bootstrap.min.css";

        private string GetTableHeadColumnNode (string value) => $"<th scope='col'>{value}</th>";
        private string GetTableBodyFirstColumnNode (object value) => $"<th scope='row'>{value}</th>";
        private string GetTableBodyOtherColumnNode (object value) => $"<td>{value}</td>";

        public byte[] Convert(List<T> list)
        {
            var modelType = typeof(T);

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), PDF_THEMES_DIR);
            var html = File.ReadAllText(path);

            var htmlDoc = GenerateHtmlDocument(list, html);

            var memoryStream = ConvertToPdfFile(htmlDoc);

            return memoryStream.ToArray();
        }

        private HtmlDocument GenerateHtmlDocument(List<T> list, string html)
        {
            var modelType = typeof(T);

            html = html.Replace(TITLE_VAR, modelType.Name);

            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(html);

            modelType.GetProperties().ToList().ForEach(x =>
            {
                var th = HtmlNode.CreateNode(GetTableHeadColumnNode(x.Name));
                var headrow = htmlDoc.GetElementbyId("head-row");
                headrow.AppendChild(th);
            });

            list.ForEach(x =>
            {
                var values = modelType.GetProperties().Select(propertyInfo => propertyInfo.GetValue(x, null)).ToList();

                var tablebody = htmlDoc.GetElementbyId("table-body");
                var tr = HtmlNode.CreateNode($"<tr>");

                for (int i = 0; i < values.Count; i++)
                {
                    if (i == 0)
                    {
                        tr.AppendChild(HtmlNode.CreateNode(GetTableBodyFirstColumnNode(values[i])));
                    }
                    else
                    {
                        tr.AppendChild(HtmlNode.CreateNode(GetTableBodyOtherColumnNode(values[i])));
                    }
                }
                tablebody.AppendChild(tr);
            });

            return htmlDoc;
        }

        private MemoryStream ConvertToPdfFile(HtmlDocument document)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = document.DocumentNode.InnerHtml,
                        WebSettings = {
                            DefaultEncoding = "utf-8",
                            UserStyleSheet = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), CSS_DIR)
                        },
                        HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 },
                    }
                }
            };

            var converter = new SynchronizedConverter(new PdfTools());

            var memoryStream = new MemoryStream(converter.Convert(doc));

            return memoryStream;
        }
    }
}
