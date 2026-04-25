namespace APPLICATION_BACKEND.DTOs
{
    public class DelivererDashboardStatsDto
    {
        /// <summary>Total delivery fees earned (Rs. 50 per completed delivery).</summary>
        public int TotalEarnings { get; set; }

        public int ActiveOrdersCount { get; set; }

        public int CompletedOrdersCount { get; set; }
    }
}
