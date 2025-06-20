using CreditApp.Application.DTOs;
using CreditApp.Domain.Enums;
using CreditApp.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.Data;

namespace CreditApp.Application.Handlers;

public record ExportCreditRequestsQuery(ExportCreditRequestsRequest Request) : IRequest<byte[]>;

public class ExportCreditRequestsHandler : IRequestHandler<ExportCreditRequestsQuery, byte[]>
{
    private readonly ApplicationDbContext _context;

    public ExportCreditRequestsHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> Handle(ExportCreditRequestsQuery request, CancellationToken cancellationToken)
    {
        // Construir query con filtros
        var query = _context.CreditRequests
            .Include(cr => cr.User)
            .AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrEmpty(request.Request.Status))
        {
            query = query.Where(cr => cr.Status == request.Request.Status);
        }

        // Obtener datos
        var creditRequests = await query
            .OrderByDescending(cr => cr.CreatedAt)
            .Select(cr => new CreditRequestExportDTO
            {
                Id = cr.Id,
                Username = cr.User.Username,
                Email = cr.User.Email,
                Amount = cr.Amount.Amount,
                MonthlyIncome = cr.MonthlyIncome.Amount,
                TermInMonths = cr.TermInMonths,
                WorkSeniorityYears = cr.WorkSeniorityYears,
                Purpose = cr.Purpose,
                Status = cr.Status,
                RejectionReason = cr.RejectionReason,
                ApprovedBy = cr.ApprovedBy,
                CreatedAt = cr.CreatedAt,
                ApprovedAt = cr.ApprovedAt,
                UpdatedAt = cr.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        // Generar Excel con ClosedXML
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Solicitudes de Crédito");

        // Configurar encabezados
        var headers = new[]
        {
            "ID", "Usuario", "Email", "Monto", "Ingreso Mensual", "Plazo (meses)", 
            "Antigüedad Laboral", "Propósito", "Estado", "Motivo Rechazo", 
            "Aprobado Por", "Fecha Creación", "Fecha Aprobación", "Última Actualización"
        };

        // Escribir encabezados
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // Llenar datos
        for (int row = 0; row < creditRequests.Count; row++)
        {
            var cr = creditRequests[row];
            var excelRow = row + 2;

            worksheet.Cell(excelRow, 1).Value = cr.Id.ToString();
            worksheet.Cell(excelRow, 2).Value = cr.Username;
            worksheet.Cell(excelRow, 3).Value = cr.Email;
            worksheet.Cell(excelRow, 4).Value = cr.Amount;
            worksheet.Cell(excelRow, 5).Value = cr.MonthlyIncome;
            worksheet.Cell(excelRow, 6).Value = cr.TermInMonths;
            worksheet.Cell(excelRow, 7).Value = cr.WorkSeniorityYears;
            worksheet.Cell(excelRow, 8).Value = cr.Purpose;
            worksheet.Cell(excelRow, 9).Value = cr.Status;
            worksheet.Cell(excelRow, 10).Value = cr.RejectionReason ?? "";
            worksheet.Cell(excelRow, 11).Value = cr.ApprovedBy ?? "";
            worksheet.Cell(excelRow, 12).Value = cr.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            worksheet.Cell(excelRow, 13).Value = cr.ApprovedAt?.ToString("dd/MM/yyyy HH:mm") ?? "";
            worksheet.Cell(excelRow, 14).Value = cr.UpdatedAt.HasValue ? cr.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm") : "";
        }

        // Autoajustar columnas
        worksheet.Columns().AdjustToContents();

        // Agregar filtros
        var range = worksheet.Range(1, 1, 1, headers.Length);
        range.SetAutoFilter();

        // Agregar información del reporte
        var infoRow = creditRequests.Count + 3;
        var infoCell = worksheet.Cell(infoRow, 1);
        infoCell.Value = $"Reporte generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
        infoCell.Style.Font.Bold = true;

        // Convertir a bytes
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
} 