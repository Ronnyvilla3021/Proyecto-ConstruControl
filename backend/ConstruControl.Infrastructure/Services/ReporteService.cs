using ClosedXML.Excel;
using ConstruControl.Application.DTOs.Reportes;
using ConstruControl.Application.Interfaces;
using ConstruControl.Domain.Entities;
using ConstruControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ConstruControl.Infrastructure.Services;

public class ReporteService : IReporteService
{
    private readonly ConstruControlDbContext _context;

    public ReporteService(ConstruControlDbContext context)
    {
        _context = context;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerarExcelComprasConsumosAsync(ReporteObraRequestDto filtro)
    {
        var obra = await _context.Obras.FirstOrDefaultAsync(o => o.Id == filtro.ObraId)
            ?? throw new InvalidOperationException($"No existe una obra con id {filtro.ObraId}.");

        var comprasQuery = _context.Compras
            .Include(c => c.Proveedor)
            .Include(c => c.Detalles).ThenInclude(d => d.Material)
            .Where(c => c.ObraId == filtro.ObraId);

        var consumosQuery = _context.Consumos
            .Include(c => c.Material)
            .Include(c => c.Responsable)
            .Where(c => c.ObraId == filtro.ObraId);

        if (filtro.FechaDesde.HasValue)
        {
            comprasQuery = comprasQuery.Where(c => c.Fecha >= filtro.FechaDesde.Value);
            consumosQuery = consumosQuery.Where(c => c.Fecha >= filtro.FechaDesde.Value);
        }
        if (filtro.FechaHasta.HasValue)
        {
            comprasQuery = comprasQuery.Where(c => c.Fecha <= filtro.FechaHasta.Value);
            consumosQuery = consumosQuery.Where(c => c.Fecha <= filtro.FechaHasta.Value);
        }

        var compras = await comprasQuery.OrderBy(c => c.Fecha).ToListAsync();
        var consumos = await consumosQuery.OrderBy(c => c.Fecha).ToListAsync();

        using var workbook = new XLWorkbook();

        var hojaCompras = workbook.Worksheets.Add("Compras");
        hojaCompras.Cell(1, 1).Value = $"Compras - {obra.Nombre}";
        hojaCompras.Range(1, 1, 1, 6).Merge().Style.Font.SetBold().Font.SetFontSize(14);

        string[] encabezadosCompras = { "Fecha", "Proveedor", "Material", "Cantidad", "Precio Unit.", "Subtotal" };
        for (int i = 0; i < encabezadosCompras.Length; i++)
        {
            hojaCompras.Cell(3, i + 1).Value = encabezadosCompras[i];
            hojaCompras.Cell(3, i + 1).Style.Font.SetBold();
        }

        int fila = 4;
        foreach (var compra in compras)
        {
            foreach (var detalle in compra.Detalles)
            {
                hojaCompras.Cell(fila, 1).Value = compra.Fecha.ToString("yyyy-MM-dd");
                hojaCompras.Cell(fila, 2).Value = compra.Proveedor.Nombre;
                hojaCompras.Cell(fila, 3).Value = detalle.Material.Nombre;
                hojaCompras.Cell(fila, 4).Value = (double)detalle.Cantidad;
                hojaCompras.Cell(fila, 5).Value = (double)detalle.PrecioUnitario;
                hojaCompras.Cell(fila, 6).Value = (double)detalle.Subtotal;
                fila++;
            }
        }
        hojaCompras.Columns().AdjustToContents();

        var hojaConsumos = workbook.Worksheets.Add("Consumos");
        hojaConsumos.Cell(1, 1).Value = $"Consumos - {obra.Nombre}";
        hojaConsumos.Range(1, 1, 1, 4).Merge().Style.Font.SetBold().Font.SetFontSize(14);

        string[] encabezadosConsumos = { "Fecha", "Material", "Cantidad", "Responsable" };
        for (int i = 0; i < encabezadosConsumos.Length; i++)
        {
            hojaConsumos.Cell(3, i + 1).Value = encabezadosConsumos[i];
            hojaConsumos.Cell(3, i + 1).Style.Font.SetBold();
        }

        fila = 4;
        foreach (var consumo in consumos)
        {
            hojaConsumos.Cell(fila, 1).Value = consumo.Fecha.ToString("yyyy-MM-dd");
            hojaConsumos.Cell(fila, 2).Value = consumo.Material.Nombre;
            hojaConsumos.Cell(fila, 3).Value = (double)consumo.Cantidad;
            hojaConsumos.Cell(fila, 4).Value = consumo.Responsable.NombreCompleto;
            fila++;
        }
        hojaConsumos.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> GenerarPdfIndicadoresObraAsync(int obraId)
    {
        var obra = await _context.Obras.FirstOrDefaultAsync(o => o.Id == obraId)
            ?? throw new InvalidOperationException($"No existe una obra con id {obraId}.");

        var gastoTotal = await _context.Compras
            .Where(c => c.ObraId == obraId && c.Estado == EstadoCompra.Recibida)
            .SumAsync(c => c.Total);

        var porcentajeUsado = obra.Presupuesto > 0 ? gastoTotal / obra.Presupuesto : 0;
        var diasTranscurridos = Math.Max(1, (DateTime.UtcNow.Date - obra.FechaInicio.Date).Days);
        var costoDiarioPromedio = gastoTotal / diasTranscurridos;

        var totalCompras = await _context.Compras.CountAsync(c => c.ObraId == obraId);
        var totalConsumos = await _context.Consumos.CountAsync(c => c.ObraId == obraId);

        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Text($"Reporte de Indicadores - {obra.Nombre}")
                    .SemiBold().FontSize(18);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text($"Ubicacion: {obra.Ubicacion}");
                        col.Item().Text($"Estado: {obra.Estado}");
                        col.Item().Text($"Fecha de inicio: {obra.FechaInicio:yyyy-MM-dd}");
                        col.Item().Text($"Dias transcurridos: {diasTranscurridos}");

                        col.Item().PaddingTop(15).Text("Indicadores financieros").Bold().FontSize(14);
                        col.Item().Text($"Presupuesto total: ${obra.Presupuesto:N2}");
                        col.Item().Text($"Gasto total: ${gastoTotal:N2}");
                        col.Item().Text($"Porcentaje de presupuesto usado: {porcentajeUsado:P1}");
                        col.Item().Text($"Costo diario promedio: ${costoDiarioPromedio:N2}");

                        col.Item().PaddingTop(15).Text("Actividad").Bold().FontSize(14);
                        col.Item().Text($"Ordenes de compra registradas: {totalCompras}");
                        col.Item().Text($"Consumos registrados: {totalConsumos}");
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generado el ");
                        x.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm")).SemiBold();
                        x.Span(" - ConstruControl");
                    });
            });
        });

        return documento.GeneratePdf();
    }
}
