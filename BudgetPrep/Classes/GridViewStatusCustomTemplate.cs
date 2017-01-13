using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BudgetPrep.Classes
{
    public delegate void CustomStatusClickedEventHandler(object sender, CustomEvenArgs e);

    public class GridViewStatusCustomTemplate : Control, System.Web.UI.ITemplate
    {
        int LocalPeriodID;
        int LocalColumnOption;
        string LocalColumnName;

        public event CustomStatusClickedEventHandler OnCustomStatusClicked;

        public GridViewStatusCustomTemplate(int ColumnOption,string ColumnName, int PeriodMenguruID)
        {
            LocalColumnOption = ColumnOption;
            LocalPeriodID = PeriodMenguruID;
            LocalColumnName = ColumnName;
        }

        public void InstantiateIn(System.Web.UI.Control container)
        {
            if (LocalColumnOption == 0)
            {
                Label lblPeriodMenguruID = new Label();
                lblPeriodMenguruID.ID = "lbl_PeriodMenguruID_" + LocalPeriodID.ToString();
                lblPeriodMenguruID.Text = LocalPeriodID.ToString();
                lblPeriodMenguruID.Visible = false;
                container.Controls.Add(lblPeriodMenguruID);

                Label lbl = new Label();
                lbl.ID = "lbl_" + LocalPeriodID.ToString();
                container.Controls.Add(lbl);

                LinkButton btnstatus = new LinkButton();
                btnstatus.ID = "btnSaved_" + LocalPeriodID.ToString();
                btnstatus.CssClass = "btn btn-primary";
                btnstatus.Style.Add("background-color", new Helper().GetColorByStatusValue('S').Name);
                btnstatus.Click += btnstatus_Click;
                container.Controls.Add(btnstatus);

                btnstatus = new LinkButton();
                btnstatus.ID = "btnPrepared_" + LocalPeriodID.ToString();
                btnstatus.CssClass = "btn btn-primary";
                btnstatus.Style.Add("background-color", new Helper().GetColorByStatusValue('P').Name);
                btnstatus.Click += btnstatus_Click;
                container.Controls.Add(btnstatus);

                btnstatus = new LinkButton();
                btnstatus.ID = "btnReviewed_" + LocalPeriodID.ToString();
                btnstatus.CssClass = "btn btn-primary";
                btnstatus.Style.Add("background-color", new Helper().GetColorByStatusValue('R').Name);
                btnstatus.Click += btnstatus_Click;
                container.Controls.Add(btnstatus);

                btnstatus = new LinkButton();
                btnstatus.ID = "btnApproved_" + LocalPeriodID.ToString();
                btnstatus.CssClass = "btn btn-primary";
                btnstatus.Style.Add("background-color", new Helper().GetColorByStatusValue('A').Name);
                btnstatus.Click += btnstatus_Click;
                container.Controls.Add(btnstatus);

                btnstatus = new LinkButton();
                btnstatus.ID = "btnRevRej_" + LocalPeriodID.ToString();
                btnstatus.CssClass = "btn btn-primary";
                btnstatus.Style.Add("background-color", new Helper().GetColorByStatusValue('X').Name);
                btnstatus.Click += btnstatus_Click;
                container.Controls.Add(btnstatus);

                btnstatus = new LinkButton();
                btnstatus.ID = "btnAprRej_" + LocalPeriodID.ToString();
                btnstatus.CssClass = "btn btn-primary";
                btnstatus.Style.Add("background-color", new Helper().GetColorByStatusValue('Y').Name);
                btnstatus.Click += btnstatus_Click;
                container.Controls.Add(btnstatus);
            }
        }

        public void btnstatus_Click(object sender, EventArgs e)
        {
            string strAccountCode = ((GridView)((LinkButton)sender).Parent.Parent.Parent.Parent).DataKeys[((GridViewRow)((LinkButton)sender).Parent.Parent).RowIndex][0].ToString();
            CustomEvenArgs eventargs = new CustomEvenArgs();
            eventargs.Code = strAccountCode;
            eventargs.PeriodID = LocalPeriodID;
            eventargs.Period = LocalColumnName;

            this.OnCustomStatusClicked(sender, eventargs);
        }
    }
}