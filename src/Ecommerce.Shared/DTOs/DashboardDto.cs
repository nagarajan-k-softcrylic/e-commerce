namespace Ecommerce.Shared.DTOs;

public record DashboardSummaryDto
{
    public int TotalProducts { get; init; }
    public int TotalOrders { get; init; }
    public int PendingOrders { get; init; }
    public int ConfirmedOrders { get; init; }
    public int FailedOrders { get; init; }
    public decimal TotalRevenue { get; init; }
}

