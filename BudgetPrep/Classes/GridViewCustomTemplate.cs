using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBBAL;
using OBDAL;

namespace BudgetPrep.Classes
{
    public delegate void CustomTextChangedEventHandler(object sender, CustomEvenArgs e);
    public delegate void CustomClickedEventHandler(object sender, CustomEvenArgs e);
    public delegate void CustomKetPenTextChangedEventHandler(object sender, CustomKetPenEvenArgs e);
    
    public class CustomEvenArgs : EventArgs
    {
        public string Code { get; set; }
        public int PeriodID { get; set; }
        public string Period { get; set; }
        public decimal Amount { get; set; }
    }

    public class CustomKetPenEvenArgs : EventArgs
    {
        public string AccountCode { get; set; }
        public string KetorPen { get; set; }
    }

    public class GridViewCustomTemplate : Control, System.Web.UI.ITemplate
    {   
        string LocalColumnName;
        int LocalPeriodID;
        int LocalColumnOption;        

        public GridViewCustomTemplate(int ColumnOption, string ColumnName, int PeriodMenguruID)
        {
            LocalColumnName = ColumnName;
            LocalPeriodID = PeriodMenguruID;
            LocalColumnOption = ColumnOption; 
            //tbGridCustomColumn = new TextBox();
        }

        //public event EventHandler OnCustomTextChanged
        //{
        //    add { tbGridCustomColumn.TextChanged += value; }
        //    remove { tbGridCustomColumn.TextChanged -= value; }
        //}

        public event CustomTextChangedEventHandler OnCustomTextChanged;
        public event CustomClickedEventHandler OnCustomClicked;
        public event CustomKetPenTextChangedEventHandler OnCustomKetPenTextChanged;

        public void InstantiateIn(System.Web.UI.Control container)
        {
            if (LocalColumnOption == 0)
            {
                LinkButton lb = new LinkButton();
                lb.ID = "btnExpand";
                lb.CommandName = "Expand";
                lb.CommandArgument = "CommandArgument";
                //lb.Attributes["data-toggle"] = "tooltip";
                //lb.Attributes["data-placement"] = "bottom";
                //lb.Attributes["data-trigger"] = "hover";
                //lb.Attributes["data-original-title"] = "Tooltip";

                container.Controls.Add(lb);
            }
            else if (LocalColumnOption == 1)
            {
                Label lblPeriodMenguruID = new Label();
                lblPeriodMenguruID.ID = "lbl_PeriodMenguruID_" + LocalPeriodID.ToString();
                lblPeriodMenguruID.Text = LocalPeriodID.ToString();
                lblPeriodMenguruID.Visible = false;

                Label lblGridCustomColumn = new Label();
                lblGridCustomColumn.ID = "lbl_" + LocalPeriodID.ToString();

                TextBox tbGridCustomColumn = new TextBox();
                tbGridCustomColumn.ID = "tb_" + LocalPeriodID.ToString();
                tbGridCustomColumn.AutoPostBack = true;
                tbGridCustomColumn.Width = Unit.Percentage(100);
                tbGridCustomColumn.Attributes.Add("onkeypress", "return IsNumberKey(event,this);");
                tbGridCustomColumn.TextChanged += tbGridCustomColumn_TextChanged;

                Button btnDecision = new Button();
                btnDecision.ID = "btn_" + LocalPeriodID.ToString();
                //btnDecision.Text = Server.HtmlDecode("&#9635;");
                btnDecision.CssClass = "btn btn-info";
                //cbDecision.AutoPostBack = true;
                btnDecision.Click += btnDecision_Click;

                container.Controls.Add(lblPeriodMenguruID);
                container.Controls.Add(lblGridCustomColumn);
                container.Controls.Add(tbGridCustomColumn);
                container.Controls.Add(btnDecision);
            }
            else if (LocalColumnOption == 2)
            {
                TextBox tbGridKetPenColumn = new TextBox();
                tbGridKetPenColumn.ID = "tb_" + LocalColumnName;
                tbGridKetPenColumn.AutoPostBack = true;
                tbGridKetPenColumn.Width = Unit.Percentage(100);
                tbGridKetPenColumn.TextChanged += tbGridKetPenColumn_TextChanged;

                container.Controls.Add(tbGridKetPenColumn);
            }
        }
        
        public void tbGridCustomColumn_TextChanged(object sender, EventArgs e)
        {
            string strAccountCode = ((GridView)((TextBox)sender).Parent.Parent.Parent.Parent).DataKeys[((GridViewRow)((TextBox)sender).Parent.Parent).RowIndex][0].ToString();

            //BudgetMenguru newBudgetMenguru = new BudgetMenguru();
            //newBudgetMenguru.AccountCode = strAccountCode;
            //newBudgetMenguru.PeriodMengurusID = LocalPeriodMenguruID;
            //newBudgetMenguru.Amount = Convert.ToDecimal(((TextBox)sender).Text.Trim());
            //newBudgetMenguru.Status = "A";
            //new BudgetMengurusBAL().UpdateBudgetMengurus(newBudgetMenguru);

            CustomEvenArgs eventargs = new CustomEvenArgs();
            eventargs.Code = strAccountCode;
            eventargs.PeriodID = LocalPeriodID;

            string stramnt = ((TextBox)sender).Text.Trim().Replace(",", "");
            decimal d = 0;
            bool flag = decimal.TryParse(stramnt,out d);
            eventargs.Amount = (flag) ? d : 0;
            //eventargs.Amount = (stramnt == string.Empty) ? 0 : Convert.ToDecimal(stramnt);

            this.OnCustomTextChanged(sender, eventargs);
        }

        public void tbGridKetPenColumn_TextChanged(object sender, EventArgs e)
        {
            string strAccountCode = ((GridView)((TextBox)sender).Parent.Parent.Parent.Parent).DataKeys[((GridViewRow)((TextBox)sender).Parent.Parent).RowIndex][0].ToString();
            CustomKetPenEvenArgs eventargs = new CustomKetPenEvenArgs();
            eventargs.AccountCode = strAccountCode;
            eventargs.KetorPen = LocalColumnName;
            this.OnCustomKetPenTextChanged(sender, eventargs);
        }

        public void btnDecision_Click(object sender, EventArgs e)
        {
            string strAccountCode = ((GridView)((Button)sender).Parent.Parent.Parent.Parent).DataKeys[((GridViewRow)((Button)sender).Parent.Parent).RowIndex][0].ToString();
            //string strPeriod = ((GridView)((Button)sender).Parent.Parent.Parent.Parent).HeaderRow.Cells[((GridViewRow)((Button)sender).Parent.Parent).RowIndex].Text;

            CustomEvenArgs eventargs = new CustomEvenArgs();
            eventargs.Code = strAccountCode;
            eventargs.PeriodID = LocalPeriodID;
            eventargs.Period = LocalColumnName;
            eventargs.Amount = Convert.ToDecimal(((TextBox)((Button)sender).Parent.FindControl("tb_" + LocalPeriodID.ToString())).Text.Trim());

            this.OnCustomClicked(sender, eventargs);            
        }
    }
}