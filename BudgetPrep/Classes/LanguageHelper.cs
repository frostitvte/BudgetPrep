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
    public class LangHelp
    {
        public string ControlName { get; set; }
        public string English { get; set; }
        public string Foreign { get; set; }
        public string Status { get; set; }
    }
    public class LanguageHelper
    {
        List<LangHelp> lstBPLanguageLocal;

        public LanguageHelper()
        {
            lstBPLanguageLocal = new List<LangHelp>();
        }

        public void ChangeLanguage(MasterUser AuthUser,List<LangHelp> lstBPLanguage, List<Control> ControlsCollection)
        {
            lstBPLanguageLocal = lstBPLanguage;
            foreach (Control c in ControlsCollection)
            {
                switch (c.GetType().Name)
                {
                    case "Label": LanguageLabel((Label)c);
                        break;
                    case "LinkButton": LanguageButton((LinkButton)c);
                        break;
                    case "DropDownList": LanguageDropDownList((DropDownList)c);
                        break;
                    case "GridView": LanguageGridView((GridView)c);
                        break;
                }
            }
        }

        private void LanguageLabel(Label Lbl)
        {
            LangHelp item = lstBPLanguageLocal.Where(x => x.Status == "C" && x.ControlName.Trim() == Lbl.ID.Trim()).FirstOrDefault();
            if (item != null && item.Foreign != string.Empty)
                Lbl.Text = item.Foreign;
        }

        private void LanguageButton(LinkButton Btn)
        {
            LangHelp item = lstBPLanguageLocal.Where(x => x.Status == "C" && x.ControlName.Trim() == Btn.ID.Trim()).FirstOrDefault();
            if (item != null && item.Foreign != string.Empty)
                Btn.Text = item.Foreign;
        }

        private void LanguageDropDownList(DropDownList DDL)
        {
            foreach (ListItem li in DDL.Items)
            {
                LangHelp item = lstBPLanguageLocal.Where(x => x.Status == "D" && x.ControlName.Trim() == li.Text.Trim()).FirstOrDefault();
                if (item != null && item.Foreign != string.Empty)
                    li.Text = item.Foreign;
            }
        }

        private void LanguageGridView(GridView GV)
        {
            foreach (DataControlField DCF in GV.Columns)
            {
                LangHelp item = lstBPLanguageLocal.Where(x => x.Status == "G" && x.ControlName.Trim() == DCF.HeaderText.Trim()).FirstOrDefault();
                if (item != null && item.Foreign != string.Empty)
                    DCF.HeaderText = item.Foreign;

                item = lstBPLanguageLocal.Where(x => x.Status == "G" && x.ControlName.Trim() == DCF.FooterText.Trim()).FirstOrDefault();
                if (item != null && item.Foreign != string.Empty)
                    DCF.FooterText = item.Foreign;
            }
        }

        public List<string> GetLanguages()
        {
            List<string> lstLanguages = new BPLanguage().GetType().GetProperties().Where(x => !x.GetMethod.IsVirtual).Select(x=>x.Name).ToList();
            lstLanguages.Remove("LanguageID");
            lstLanguages.Remove("ControlName");
            lstLanguages.Remove("Status");

            return lstLanguages;
        }
    }
}