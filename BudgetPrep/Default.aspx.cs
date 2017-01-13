using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using BudgetPrep.Classes;
using OBBAL;
using OBDAL;
using System.Web.Script.Serialization;

namespace BudgetPrep
{
    public class GetAnggaranDipohonByYear
    {
        public int Year { get; set; }
        public string SumBudget { get; set; }
    }

    public class GetAPvsAE
    {
        public string Name { get; set; }
        public int Year { get; set; }
        public double SumBudget { get; set; }
    }

    public partial class _Default : PageHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ((Label)this.Master.FindControl("lblbreadcrumbMenu")).Text = "";
                ((Label)this.Master.FindControl("lblbreadcrumbScreen")).Text = "Home";
            }
            colors = new List<string>();
        }

        private static List<string> colors;
        private static string GetRandomColor()
        {
            Random rand = new Random();
            //Color color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
            //return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
            string color = string.Empty;
            do
            {
                color = string.Format("#{0:X2}{1:X2}{2:X2}", rand.Next(256), rand.Next(256), rand.Next(256));
            }
            while (colors.IndexOf(color) > -1);

            colors.Add(color);
            return color;
        }

        #region Mengurus

        [WebMethod]
        public static string GetMengurusYearData()
        {
            string str = String.Empty;
            List<GetAnggaranDipohonByYear> lstobj = new List<GetAnggaranDipohonByYear>();

            for (int i = DateTime.Now.Year - 5; i <= DateTime.Now.Year+2; i++)
            {
                List<BudgetMenguru> fndyr = new BudgetMengurusBAL().GetBudgetMengurusForDashboard().Where(x => x.PeriodMenguru.MengurusYear.Equals(i)).ToList();

                if (fndyr.Count() > 0)
                {
                    lstobj.Add(new GetAnggaranDipohonByYear
                                {
                                    Year = fndyr.First().PeriodMenguru.MengurusYear,
					                SumBudget = fndyr.Sum(y => y.Amount).ToString("F")
                                });
                }
                else
                {
                    lstobj.Add(new GetAnggaranDipohonByYear { Year = i, SumBudget = "0.00" });
                }
            }

            if (lstobj.Count() > 0)
            {
                str = lstobj.Select(x => x.Year == Convert.ToInt32(DateTime.Now.Year) ? "{y:" + x.SumBudget + ", label: \"" + x.Year + "\", color:\""
                    + GetRandomColor() + "\"}" : "{y:" + x.SumBudget + ", label: \"" + x.Year + "\", color:\"#6AA6D6\"}").Aggregate((x, y) => x + "," + y);
            }
            return "[" + str + "]";
        }

        [WebMethod]
        public static string GetMengurusFieldData(string Year)
        {
            string str = string.Empty;

            str = new BudgetMengurusBAL().GetBudgetMengurusForDashboard().Where(x => x.PeriodMenguru.MengurusYear.ToString().Equals(Year))
                .GroupBy(x => x.PeriodMenguru.FieldMenguru.FieldMengurusDesc)
                .Select(x => new
                {
                    FieldDesc = x.First().PeriodMenguru.FieldMenguru.FieldMengurusDesc,
                    SumBudget = x.Sum(y => y.Amount).ToString("F")
                })
                .OrderBy(x => x.FieldDesc)
                .Select(x => "{y:" + x.SumBudget + ", legendText:\"" + x.FieldDesc + "\", indexLabel: \"" + x.FieldDesc + "\", color:\"" + GetRandomColor() + "\"}")
                .Aggregate((x, y) => x + "," + y);

            return "[" + str + "]";
        }

        [WebMethod]
        public static string GetMengurusAcountData(string Year, string Field)
        {
            string str = string.Empty;

            List<string> parentAccCode = new AccountCodeBAL().GetAccountCodes().Select(x => x.ParentAccountCode).Distinct().ToList();
            List<string> AccCode = new AccountCodeBAL().GetAccountCodes().Where(x => !parentAccCode.Contains(x.AccountCode1)).Select(x => x.AccountCode1).Distinct().ToList();

            var data = new BudgetMengurusBAL().GetBudgetMengurusForDashboard()
                .Where(x => x.PeriodMenguru.MengurusYear.ToString().Equals(Year) && x.PeriodMenguru.FieldMenguru.FieldMengurusDesc == Field && AccCode.Contains(x.AccountCode))
                .Select(x => new
                {
                    Account = GetRootAccountCode(x.AccountCode1),
                    SumBudget = x.Amount
                });
            var data1 = data
                .GroupBy(x => x.Account)
                .Select(x => new
                {
                    Account = x.First().Account,
                    SumBudget = x.Sum(y => y.SumBudget).ToString("F")
                })
                .OrderBy(x => x.Account);
            str = data1.Select(x => "{y:" + x.SumBudget + ", legendText:\"" + x.Account + "\", label: \"" + x.Account + "\", color:\"" + GetRandomColor() + "\"}")
                .Aggregate((x, y) => x + "," + y);

            return "[" + str + "]";
        }

        [WebMethod]
        public static List<object> GetMengurusAPvsAE()
        {
            List<object> ser1 = new List<object>();
            List<object> ser2 = new List<object>();
            List<object> obj = new List<object>();

            ser1 = new List<object>
                { 
                    new GetAPvsAE { Year = DateTime.Now.Year, SumBudget = 100.00 },
                    new GetAPvsAE { Year = DateTime.Now.Year + 1, SumBudget = 666.00 }
                };

            ser2 = new List<object>
                { 
                    new GetAPvsAE { Year = DateTime.Now.Year + 1, SumBudget = 22.00 },
                    new GetAPvsAE { Year = DateTime.Now.Year, SumBudget = 88.00 }
                };
            
            obj.Add(ser1);
            obj.Add(ser2);

            return obj;
        }

        [WebMethod]
        public static List<object> GetMengurusAPvsAB()
        {
            List<object> obj = new List<object>();

            List<GetAPvsAE> ser1 = new List<GetAPvsAE>();
            List<GetAPvsAE> ser2 = new List<GetAPvsAE>();
            List<GetAPvsAE> ser3 = new List<GetAPvsAE>();

            ser1 = new List<GetAPvsAE>
                { 
                    new GetAPvsAE { Year = DateTime.Now.Year, SumBudget = 50 },
                    new GetAPvsAE { Year = DateTime.Now.Year + 1, SumBudget = 20 }
                };

            ser2 = new List<GetAPvsAE>
                { 
                    new GetAPvsAE { Year = DateTime.Now.Year + 1, SumBudget = 40 },
                    new GetAPvsAE { Year = DateTime.Now.Year, SumBudget = 10 }
                };

            //to find Q2
            ser3 = new List<GetAPvsAE>
                { 
                    new GetAPvsAE { Year = DateTime.Now.Year + 1, SumBudget = 40 },
                    new GetAPvsAE { Year = DateTime.Now.Year, SumBudget = 20 }
                };
            //foreach (var ser1yr in ser1)
            //{
            //    foreach (var ser2yr in ser2)
            //    {
            //        double sum = 0.0;

            //        if (ser1yr.Year == ser2yr.Year)
            //        {
            //            sum = 100 - (Convert.ToDouble(ser1yr.SumBudget) + Convert.ToDouble(ser2yr.SumBudget));

            //            ser3.Add(new GetAPvsAE { Year = ser1yr.Year, SumBudget = sum });
            //        }
            //    }
            //}

            obj.Add(ser1);
            obj.Add(ser2);
            obj.Add(ser3);

            return obj;
        }
        #endregion

        #region Projek

        [WebMethod]
        public static string GetProjekYearData()
        {
            string str = String.Empty;
            List<GetAnggaranDipohonByYear> lstobj = new List<GetAnggaranDipohonByYear>();

            for (int i = DateTime.Now.Year - 5; i <= DateTime.Now.Year+2; i++)
            {
                List<BudgetProjek> fndyr = new BudgetProjekBAL().GetBudgetProjekForDashboard().Where(x => x.PeriodMenguru.MengurusYear.Equals(i)).ToList();

                if (fndyr.Count() > 0)
                {
                    lstobj.Add(new GetAnggaranDipohonByYear
                    {
                        Year = fndyr.First().PeriodMenguru.MengurusYear,
                        SumBudget = fndyr.Sum(y => y.Amount).ToString("F")
                    });
                }
                else
                {
                    lstobj.Add(new GetAnggaranDipohonByYear { Year = i, SumBudget = "0.00" });
                }
            }

            if (lstobj.Count() > 0)
            {
                str = lstobj.Select(x => x.Year == Convert.ToInt32(DateTime.Now.Year) ? "{y:" + x.SumBudget + ", label: \"" + x.Year + "\", color:\""
                    + GetRandomColor() + "\"}" : "{y:" + x.SumBudget + ", label: \"" + x.Year + "\", color:\"#6AA6D6\"}").Aggregate((x, y) => x + "," + y);
            }
            return "[" + str + "]";
        }

        #endregion

        private static string GetRootAccountCode(AccountCode accountcode)
        {
            while (accountcode.ParentAccountCode != string.Empty)
                accountcode = new AccountCodeBAL().GetParentAccountCode(accountcode);

            return accountcode.AccountCode1;
        }
    }
}
