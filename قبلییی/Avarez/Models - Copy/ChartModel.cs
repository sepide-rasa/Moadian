using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Avarez.Models
{
    public class ChartModel
    {
        public string Name
        {
            get;
            set;
        }

        public double Data1
        {
            get;
            set;
        }

        public double Data2
        {
            get;
            set;
        }

        public double Data3
        {
            get;
            set;
        }

        public double Data4
        {
            get;
            set;
        }

        public double Data5
        {
            get;
            set;
        }

        public double Data6
        {
            get;
            set;
        }

        public double Data7
        {
            get;
            set;
        }

        public double Data8
        {
            get;
            set;
        }

        public double Data9
        {
            get;
            set;
        }
        public static List<sp_RptMonthlyUser_CountWithDate> GenerateFishData(int usermnu,DateTime start,DateTime end)
        {
            Models.cartaxEntities car = new cartaxEntities();
            
            var q = car.sp_RptMonthlyUser_CountWithDate(start,
                end, usermnu).ToList();

            return q;
        }
        public static List<sp_RptChart> GeneratePieData(int usermnu, DateTime start, DateTime end)
        {
            Models.cartaxEntities car = new cartaxEntities();
            string date=MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now);
            var q = car.sp_RptChart(start, end, usermnu).ToList();
            List<sp_RptChart> sum = new List<sp_RptChart>();
            foreach (var item in q)
            {
                var t = sum.Where(k => k.CarModelTip == item.CarModelTip).FirstOrDefault();
                if (t != null)
                    t.fldPrice += item.fldPrice;
                else
                    sum.Add(item);
            }
            return sum;
        }

        public static List<ChartModel> GenerateData()
        {
            return ChartModel.GenerateData(12, 20);
        }

        public static List<ChartModel> GenerateData(int n)
        {
            return ChartModel.GenerateData(n, 20);
        }

        public static List<ChartModel> GenerateData(int n, int floor)
        {
            List<ChartModel> data = new List<ChartModel>(n);
            Random random = new Random();
            double p = (random.NextDouble() * 11) + 1;

            for (int i = 0; i < n; i++)
            {
                data.Add(new ChartModel
                {
                    Name = CultureInfo.InvariantCulture.DateTimeFormat.MonthNames[i % 12],
                    Data1 = Math.Floor(Math.Max(random.NextDouble() * 100, floor)),
                    Data2 = Math.Floor(Math.Max(random.NextDouble() * 100, floor)),
                    Data3 = Math.Floor(Math.Max(random.NextDouble() * 100, floor)),
                    Data4 = Math.Floor(Math.Max(random.NextDouble() * 100, floor)),
                    Data5 = Math.Floor(Math.Max(random.NextDouble() * 100, floor)),
                    Data6 = Math.Floor(Math.Max(random.NextDouble() * 100, floor)),
                    Data7 = Math.Floor(Math.Max(random.NextDouble() * 100, floor)),
                    Data8 = Math.Floor(Math.Max(random.NextDouble() * 100, floor)),
                    Data9 = Math.Floor(Math.Max(random.NextDouble() * 100, floor))
                });
            }

            return data;
        }

        public static List<ChartModel> GenerateDataNegative()
        {
            return ChartModel.GenerateDataNegative(12, 20);
        }

        public static List<ChartModel> GenerateDataNegative(int n, int floor)
        {
            List<ChartModel> data = new List<ChartModel>(n);
            Random random = new Random();
            double p = (random.NextDouble() * 11) + 1;

            for (int i = 0; i < n; i++)
            {
                data.Add(new ChartModel
                {
                    Name = CultureInfo.InvariantCulture.DateTimeFormat.MonthNames[i % 12],
                    Data1 = Math.Floor(Math.Max((random.NextDouble() - 0.5) * 100, floor)),
                    Data2 = Math.Floor(Math.Max((random.NextDouble() - 0.5) * 100, floor)),
                    Data3 = Math.Floor(Math.Max((random.NextDouble() - 0.5) * 100, floor)),
                    Data4 = Math.Floor(Math.Max((random.NextDouble() - 0.5) * 100, floor)),
                    Data5 = Math.Floor(Math.Max((random.NextDouble() - 0.5) * 100, floor)),
                    Data6 = Math.Floor(Math.Max((random.NextDouble() - 0.5) * 100, floor)),
                    Data7 = Math.Floor(Math.Max((random.NextDouble() - 0.5) * 100, floor)),
                    Data8 = Math.Floor(Math.Max((random.NextDouble() - 0.5) * 100, floor)),
                    Data9 = Math.Floor(Math.Max((random.NextDouble() - 0.5) * 100, floor))
                });
            }

            return data;
        }
    }
}