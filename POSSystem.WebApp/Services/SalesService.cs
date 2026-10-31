using POSSystem.Infrastructure.Repositories;
using POSSystem.WebApp.Models;
using System.Security.Cryptography.X509Certificates;

namespace POSSystem.WebApp.Services
{
    public class SalesService
    {
        private readonly SaleRepository _saleRepository;

        public SalesService(SaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SalesSummary GetSalesSummary()
        {
            DateTime now = DateTime.Now;

            var summary = new SalesSummary();


            //今日の売り上げと昨日の売り上げ
            DateTime todayStart = now.Date;
            DateTime yesterdayStart = todayStart.AddDays(-1);

            summary.TodaySales = _saleRepository.GetTotalSales(todayStart, now.AddSeconds(1));

            summary.YesterdaySales = _saleRepository.GetTotalSales(yesterdayStart, now.AddDays(-1).AddSeconds(1));

            summary.TodayGrowthRate = CalculateGrowthRate(summary.TodaySales, summary.YesterdaySales);


            //今月の売り上げと先月の売上
            DateTime monthStart = new DateTime(now.Year, now.Month, 1);
            DateTime previousMonthStart = monthStart.AddMonths(-1);

            int previousMonthDay = Math.Min(now.Day, DateTime.DaysInMonth(previousMonthStart.Year, previousMonthStart.Month));

            DateTime previousMonthEnd = new DateTime(
                previousMonthStart.Year,
                previousMonthStart.Month,
                previousMonthDay,
                now.Hour,
                now.Minute,
                now.Second).AddSeconds(1);

            summary.MonthlySales = _saleRepository.GetTotalSales(monthStart, now.AddSeconds(1));

            summary.PreviousMonthSales = _saleRepository.GetTotalSales(previousMonthStart, previousMonthEnd);

            summary.MonthlyGrowthRate = CalculateGrowthRate(summary.MonthlySales, summary.PreviousMonthSales);


            //今年度の開始日
            DateTime fiscalYearStart = now.Month >= 4 ? new DateTime(now.Year, 4, 1) : new DateTime(now.Year - 1, 4, 1);

            //前年度の開始日
            DateTime previousFiscalYearStart = fiscalYearStart.AddYears(-1);

            //前年度の同じ経過時間まで
            DateTime previousFiscalYearEnd = previousFiscalYearStart.Add(now - fiscalYearStart)
                                                                    .AddSeconds(1);

            summary.FiscalYearSales = _saleRepository.GetTotalSales(fiscalYearStart, now.AddSeconds(1));

            summary.PreviousFiacalYearSales = _saleRepository.GetTotalSales(previousFiscalYearStart, previousFiscalYearEnd);

            summary.FiscalYearGrowthRate = CalculateGrowthRate(summary.FiscalYearSales, summary.PreviousFiacalYearSales);

            return summary;
        }

        private decimal CalculateGrowthRate(decimal currentSales, decimal previousSales)
        {
            if (previousSales == 0)
            {
                return 0;
            }

            return Math.Round((currentSales - previousSales) / previousSales * 100, 1);
        }
    }
}
