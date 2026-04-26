namespace APPLICATION_BACKEND.DTOs
{
    public class DelivererDashboardStatsDto
    {
        /// <summary>COD totals you are holding on open runs (food + rider fee until delivery is completed).</summary>
        public int ActiveCodHoldPkr { get; set; }

        /// <summary>Rider fee only, lifetime (e.g. Rs. 50 × completed deliveries).</summary>
        public int TotalDeliveryEarningsPkr { get; set; }

        public int ActiveRunsCount { get; set; }

        public int CompletedDeliveriesCount { get; set; }

        /// <summary>Alias of <see cref="TotalDeliveryEarningsPkr"/> for older clients.</summary>
        public int TotalEarnings { get; set; }

        /// <summary>Alias of <see cref="ActiveRunsCount"/> for older clients.</summary>
        public int ActiveOrdersCount { get; set; }

        /// <summary>Alias of <see cref="CompletedDeliveriesCount"/> for older clients.</summary>
        public int CompletedOrdersCount { get; set; }
    }
}
