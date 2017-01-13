using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetPrep
{
    public class Helper
    {
        public int IndentPixels { get; set; }

        public Helper()
        {
            IndentPixels = 30;
        }

        public enum PageMode
        {
            New,
            Edit
        }

        public enum ItemStatus
        {
            Active = 'A',
            Inactive = 'D'
        }

        public enum BudgetItemStatusNew
        {
            Active = 'A',
            Inactive = 'D',
            Open = 'O',
            Prepared = 'P',
            Reviewed = 'R'
        }

        public enum BudgetItemStatus
        {
            Open = 'O',
            Saved = 'S',
            Prepared = 'P',
            Reviewed = 'R',
            Approved = 'A',
            ReviewerRejected = 'X',
            ApproverRejected = 'Y'
        }

        public System.Drawing.Color GetColorForStatus(BudgetItemStatus Status)
        {
            switch(Status)
            {
                case BudgetItemStatus.Open: return System.Drawing.Color.FromName("#ffffff");
                case BudgetItemStatus.Saved: return System.Drawing.Color.FromName("#999999");
                case BudgetItemStatus.Prepared: return System.Drawing.Color.FromName("#ffff00");
                case BudgetItemStatus.Reviewed: return System.Drawing.Color.FromName("#00ffff");
                case BudgetItemStatus.Approved: return System.Drawing.Color.FromName("#00ff00");
                case BudgetItemStatus.ReviewerRejected: return System.Drawing.Color.FromName("#ff00ff");
                case BudgetItemStatus.ApproverRejected: return System.Drawing.Color.FromName("#ff0000");
            }

            return System.Drawing.Color.FromName("#ffffff");
        }

        public string GetItemStatusEnumName(char strValue)
        {
            return Enum.GetName(typeof(ItemStatus), strValue);
        }

        public string GetBudgetStatusEnumName(char strValue)
        {
            return Enum.GetName(typeof(BudgetItemStatus), strValue);
        }

        public string GetItemStatusEnumValue(ItemStatus objItemStaus)
        {
            return Convert.ToChar(objItemStaus).ToString();
        }

        public string GetItemStatusEnumValueByName(string strName)
        {
            ItemStatus status;
            Enum.TryParse<ItemStatus>(strName, out status);
            return GetItemStatusEnumValue(status);
        }
        
        public System.Drawing.Color GetColorByStatusValue(char strValue)
        {
            BudgetItemStatus objEnum = (BudgetItemStatus)Enum.ToObject(typeof(BudgetItemStatus), strValue);
            return GetColorForStatus(objEnum);
        }
        
        public string GetBudgetItemStatusEnumValue(BudgetItemStatus objItemStaus)
        {
            return Convert.ToChar(objItemStaus).ToString();
        }

        public string GenerateRandomPassword()
        {
            int length = 10;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string EmailClipper(string strEmail)
        {
            string[] strarr = strEmail.Split('@');
            string email = strarr[0].Substring(0, 2).PadRight(strarr[0].Length, '*') + "@" + strarr[1];
            return email;
        }

        public string GetLevelString(string MainString,int Level)
        {
            for (int i = 0; i < Level; i++)
                MainString = "    " + MainString;
            return MainString;
        }
    }
}