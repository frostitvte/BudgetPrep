using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBDAL;
using System.Diagnostics;
using OBBAL;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using BudgetPrep.Classes;
using System.Web.UI.HtmlControls;

namespace BudgetPrep
{
    public class PageMenuHelper
    {
        public int PageID { get; set; }
        public string PageName { get; set; }
        public string PagePath { get; set; }
        public int ParentPageID { get; set; }
        public int PageOrder { get; set; }
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public string MenuIcon { get; set; }
        public int MenuOrder { get; set; }
    }

    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        MasterUser AuthUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthUser = (MasterUser)Session["UserData"];
            lblLanguage.Text = AuthUser.Language;

            if (Request.QueryString["HideSideBar"] != null && Request.QueryString["HideSideBar"].ToString() == "1")
            {
                btnCapture.Visible = false;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "onrefLoad", "RefreshSession();HideSideBar();", true);                
            }
            else
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "onrefLoad", "RefreshSession();", true);

            if (!IsPostBack)
            {
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "onLoad", "DisplaySessionTimeout();", true);

                BuildMenu();
                GetLanguage();
            }

            string str = string.Empty;
            foreach (string s in new LanguageHelper().GetLanguages())
            {
                HtmlGenericControl li = new HtmlGenericControl("li");
                LinkButton lb = new LinkButton();
                lb.ID = s;
                lb.Text = "<i class=\"fa fa-hand-o-right\"></i> <span>" + s + "</span>";
                lb.Click += btnLang_Click;
                li.Controls.Add(lb);

                language.Controls.Add(li);
            }

            if (!IsPostBack)
            {
                ChangeLanguage();
            }
        }

        private void GetLanguage()
        {
            List<LangHelp> lstBPLanguage = new LanguageDAL().GetLanguages().AsEnumerable()
                    .Select(x => new LangHelp()
                    {
                        ControlName = x.ControlName,
                        English = x.English,
                        Foreign = x.GetType().GetProperty(AuthUser.Language.Trim()).GetValue(x, null).ToString(),
                        Status = x.Status
                    }).ToList();

            Session["Language"] = lstBPLanguage;
        }
        
        public void ChangeLanguage()
        {
            List<LangHelp> lstBPLanguage = (List<LangHelp>)Session["Language"];

            List<System.Web.UI.Control> controlCollection = new List<System.Web.UI.Control>();
            List<System.Web.UI.Control> controls = Page.Controls.Cast<System.Web.UI.Control>().ToList();
            List<System.Web.UI.Control> refcontrols = new List<System.Web.UI.Control>();
            while (controls.Count > 0)
            {
                refcontrols.Clear();
                foreach (System.Web.UI.Control c in controls)
                {
                    controlCollection.Add(c);
                    if (c.HasControls())
                    {
                        foreach (System.Web.UI.Control c1 in c.Controls)
                        {
                            refcontrols.Add(c1);
                        }
                    }
                }
                controls.Clear();
                foreach (System.Web.UI.Control c in refcontrols)
                {
                    controls.Add(c);
                }
            }

            new LanguageHelper().ChangeLanguage(AuthUser, lstBPLanguage, controlCollection);
        }

        protected void btnLang_Click(object sender, EventArgs e)
        {
            if (AuthUser.Language != ((LinkButton)sender).ID)
            {
                if (new UserBAL().ChangeLanguage(AuthUser, ((LinkButton)sender).ID))
                {
                    AuthUser.Language = ((LinkButton)sender).ID;
                    Session["UserData"] = AuthUser;

                    lblLanguage.Text = AuthUser.Language;

                    ShowMessage("Success", "User Language changed successfully");

                    GetLanguage();
                    ChangeLanguage();
                }
                else
                {
                    ((SiteMaster)this.Master).ShowMessage("Failure", "An error occurred while changing User Language");
                }
            }

            ChangeLanguage();
        }
        
        private void BuildMenu()
        {
            List<PageMenuHelper> lstPages = (List<PageMenuHelper>)Session["ListPages"];
            string Home = "<li><a href=\"Default.aspx\"><i class=\"fa fa-home\"></i><span class=\"hidden-xs\">Home</span></a></li>";
            string strinbox = (AuthUser.JuncUserRoles.First().RoleID == 1) ? "Event Log" : "Inbox"; //<span class=\"badge\">10</span>
            string Inbox = "<li><a href=\"MailInBox.aspx\"><i class=\"fa fa-envelope-o\"></i><span class=\"hidden-xs\">" + strinbox + "</span></a></li>";

            Inbox = (AuthUser.JuncUserRoles.Where(x => x.RoleID == 5).Count() > 0) ? string.Empty : Inbox;

            string strmenu = string.Empty;
            foreach (int menuid in lstPages.Where(x => x.ParentPageID == 0).OrderBy(x => x.MenuOrder).Select(x => x.MenuID).Distinct())
            {
                PageMenuHelper mh = lstPages.Where(x => x.MenuID == menuid).FirstOrDefault();
                strmenu = strmenu + "<li class=\"dropdown\"><a href=\"#\" class=\"dropdown-toggle\"><i class=\"" + mh.MenuIcon + "\"></i>";
                strmenu = strmenu + "<span class=\"hidden-xs\">" + mh.MenuName + "</span></a><ul class=\"dropdown-menu\">";
                foreach (PageMenuHelper pmh in lstPages.Where(x => x.ParentPageID == 0 && x.MenuID == menuid).OrderBy(x => x.PageOrder))
                {
                    string cssclass = string.Empty;
                    if (pmh.PagePath.Contains(Request.Path))
                        cssclass = "class=\"active\"";
                    strmenu = strmenu + "<li><a " + cssclass + " href=\"" + pmh.PagePath + "\">" + pmh.PageName + "</a></li>";
                }
                strmenu = strmenu + "</ul></li>";
            }
            divMenu.InnerHtml = "<ul class=\"nav main-menu\">" + Home + Inbox + strmenu + "</ul>";
        }

        protected void btnCapture_Click(object sender, EventArgs e)
        {
            string url = Request.Url.ToString();
            Thread thread = new Thread(delegate()
            {
                using (WebBrowser browser = new WebBrowser())
                {
                    browser.ScrollBarsEnabled = false;
                    browser.AllowNavigation = true;
                    browser.Navigate(url);
                    browser.Width = 1024;
                    browser.Height = 768;
                    browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(DocumentCompleted);
                    while (browser.ReadyState != WebBrowserReadyState.Complete)
                    {
                        System.Windows.Forms.Application.DoEvents();
                    }
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser browser = sender as WebBrowser;
            using (Bitmap bitmap = new Bitmap(browser.Width, browser.Height))
            {
                browser.DrawToBitmap(bitmap, new Rectangle(0, 0, browser.Width, browser.Height));

                Response.ContentType = "image/jpeg";
                Response.AppendHeader("Content-Disposition", "attachment; filename=Capture.jpg");

                bitmap.Save(Response.OutputStream, ImageFormat.Jpeg);

                //using (MemoryStream stream = new MemoryStream())
                //{
                //    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                //    byte[] bytes = stream.ToArray();
                //    imgScreenShot.Visible = true;
                //    imgScreenShot.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(bytes);
                //}
            }
        }

        public void ShowMessage(string Title, string Body)
        {
            //((Label)this.Master.FindControl("lblModalTitle")).Text = Title;
            //((Label)this.Master.FindControl("lblModalBody")).Text = Body;
            //((System.Web.UI.HtmlControls.HtmlGenericControl)this.Master.FindControl("divModalDetail")).InnerHtml = string.Empty;

            List<LangHelp> lstBPLanguage = ((List<LangHelp>)Session["Language"]).Where(x => x.Status == "M").ToList();

            LangHelp objtitle = lstBPLanguage.Where(x => x.English.Trim().ToUpper() == Title.Trim().ToUpper()).FirstOrDefault();
            LangHelp objBody = lstBPLanguage.Where(x => x.English.Trim().ToUpper() == Body.Trim().ToUpper()).FirstOrDefault();

            lblModalTitle.Text = (objtitle != null && objtitle.Foreign != string.Empty) ? objtitle.Foreign : Title;
            lblModalBody.Text = (objBody != null && objBody.Foreign != string.Empty) ? objBody.Foreign : Body;
            divModalDetail.InnerHtml = string.Empty;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myMsgModal", "$('#myMsgModal').modal();", true);
        }

        public void ShowMessage(string Title, string Body, Exception Exception, bool LogError)
        {
            List<LangHelp> lstBPLanguage = ((List<LangHelp>)Session["Language"]).Where(x => x.Status == "M").ToList();

            LangHelp objtitle = lstBPLanguage.Where(x => x.English.Trim().ToUpper() == Title.Trim().ToUpper()).FirstOrDefault();
            LangHelp objBody = lstBPLanguage.Where(x => x.English.Trim().ToUpper() == Body.Trim().ToUpper()).FirstOrDefault();

            lblModalTitle.Text = (objtitle != null && objtitle.Foreign != string.Empty) ? objtitle.Foreign : Title;
            lblModalBody.Text = (objBody != null && objBody.Foreign != string.Empty) ? objBody.Foreign : Body;

            //lblModalTitle.Text = Title;
            //lblModalBody.Text = Body;
            divModalDetail.InnerHtml = Exception.Message;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myMsgModal", "$('#myMsgModal').modal();", true);

            if (LogError && !Exception.Message.Contains("Thread was being aborted"))
            {
                if (AuthUser == null)
                    AuthUser = (MasterUser)Session["UserData"];

                StackTrace t = new StackTrace();
                System.Reflection.MethodBase mb = t.GetFrame(1).GetMethod();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = mb.ReflectedType.Name;
                bpe.ObjectName = mb.Name;
                bpe.ObjectChanges = Exception.Message;
                bpe.EventMassage = string.Empty;
                bpe.Status = "E";
                bpe.CreatedBy = AuthUser.UserID;
                bpe.CreatedTimeStamp = DateTime.Now;
                new EventLogBAL().AddEventLog(bpe);
            }
        }
    }
}
