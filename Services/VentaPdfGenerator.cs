using Pulperia.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Pulperia.Services;

public static class VentaPdfGenerator
{
    public static byte[] Generar(Venta venta, Empleado empleado)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        QuestPDF.Settings.EnableDebugging = true;

        // =========================
        // 1. CALCULAR ALTURA DINÁMICA
        // =========================

        const float headerHeight = 90;
        const float footerHeight = 40;
        const float totalsHeight = 90;
        const float margin = 20;
        const float lineHeight = 50;

        var detalleCount = venta.DetalleVentas?.Count ?? 0;

        var calculatedHeight =
            headerHeight +
            footerHeight +
            totalsHeight +
            margin +
            (detalleCount * lineHeight);

        if (calculatedHeight < 320)
            calculatedHeight = 320;

        if (calculatedHeight > 20000)
            calculatedHeight = 20000;

        var pageSize = new PageSize(226, calculatedHeight);

        // =========================
        // 2. DOCUMENTO
        // =========================

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(pageSize);
                page.Margin(8);
                page.DefaultTextStyle(x => x.FontSize(9));

                // =========================
                // HEADER (BRANDING ASOMECSA)
                // =========================
                page.Header()
                    .AlignCenter()
                    .Column(col =>
                    {
                        col.Item().Text("ASOMECSA")
                            .Bold()
                            .FontSize(14)
                            .FontColor(Colors.Black);

                        col.Item().Text("Pulpería")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken2);

                        col.Item().PaddingTop(3).LineHorizontal(1);

                        col.Item().Text($"Recibo #{venta.Id}")
                            .Bold();

                        col.Item().Text($"{venta.Fecha:dd/MM/yyyy HH:mm}");
                    });

                // =========================
                // CONTENT
                // =========================
                page.Content()
                    .Column(col =>
                    {
                        col.Spacing(5);

                        col.Item().Text($"Empleado: {empleado.Nombre} {empleado.Apellido}");
                        col.Item().Text($"Método de pago: {venta.MetodoPago}");

                        col.Item().PaddingVertical(3).LineHorizontal(1);

                        col.Item().Text("DETALLE DE COMPRA")
                            .Bold()
                            .FontSize(10);

                        foreach (var item in venta.DetalleVentas)
                        {
                            col.Item().Column(block =>
                            {
                                block.Item().Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text(item.Producto.Nombre)
                                        .Bold();

                                    row.ConstantItem(80)
                                        .AlignRight()
                                        .Text($"₡{item.Subtotal:N2}");
                                });

                                block.Item()
                                    .Text($"{item.Cantidad} x ₡{item.PrecioUnitarioHistorico:N2}")
                                    .FontSize(8)
                                    .FontColor(Colors.Grey.Darken1);
                            });
                        }

                        col.Item().PaddingVertical(3).LineHorizontal(1);

                        // =========================
                        // TOTALES (BLOQUE DESTACADO)
                        // =========================
                        col.Item()
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(6)
                            .Column(total =>
                            {
                                total.Item().Row(r =>
                                {
                                    r.RelativeItem().Text("TOTAL").Bold();
                                    r.ConstantItem(80).AlignRight()
                                        .Text($"₡{venta.Total:N2}")
                                        .Bold();
                                });

                                total.Item().Row(r =>
                                {
                                    r.RelativeItem().Text("Recibido");
                                    r.ConstantItem(80).AlignRight()
                                        .Text($"₡{venta.MontoRecibido:N2}");
                                });

                                total.Item().Row(r =>
                                {
                                    r.RelativeItem().Text("Cambio");
                                    r.ConstantItem(80).AlignRight()
                                        .Text($"₡{venta.Cambio:N2}");
                                });
                            });

                        if (!string.IsNullOrWhiteSpace(venta.Notas))
                        {
                            col.Item()
                                .PaddingTop(6)
                                .Text($"Notas: {venta.Notas}")
                                .FontSize(8)
                                .FontColor(Colors.Grey.Darken1);
                        }
                    });

                // =========================
                // FOOTER
                // =========================
                page.Footer()
                    .AlignCenter()
                    .Text("Gracias por su compra en ASOMECSA")
                    .FontSize(8)
                    .FontColor(Colors.Grey.Darken1);
            });
        }).GeneratePdf();
    }
}