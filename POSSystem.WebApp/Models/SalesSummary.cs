//売上集計モデル

namespace POSSystem.WebApp.Models
{
    public class SalesSummary
    {
        public decimal TodaySales { get; set; }
        public decimal YesterdaySales { get; set; }
        public decimal TodayGrowthRate { get; set; }

        public decimal MonthlySales { get; set; }
        public decimal PreviousMonthSales { get; set; }
        public decimal MonthlyGrowthRate { get; set; }

        public decimal FiscalYearSales { get; set; }
        public decimal PreviousFiacalYearSales { get; set; }
        public decimal FiscalYearGrowthRate { get; set; }
    }
}
