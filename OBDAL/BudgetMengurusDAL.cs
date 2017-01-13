using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class BudgetMengurusYearEnd
    {
        public List<string> ListSegmentDetails { get; set; }
        public string Prefix { get; set; }
        public string AccountCode { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string PeriodMengurus { get; set; }
    }

    public class BudgetMengurusDAL
    {
        OBEntities db = new OBEntities();

        public IQueryable<BudgetMenguru> GetBudgetMengurus()
        {
            return db.BudgetMengurus;        
        }

        public IQueryable<JuncBgtMengurusSegDtl> GetMengurusSegDtls()
        {
            return db.JuncBgtMengurusSegDtls;
        }

        public List<BudgetMenguru> GetBudgetMengurus(List<int> LstSegmentDetailIDs)
        {
            List<JuncBgtMengurusSegDtl> data = db.JuncBgtMengurusSegDtls.Where(x => LstSegmentDetailIDs.Contains(x.SegmentDetailID)).Select(x => x).ToList();
            foreach (int i in LstSegmentDetailIDs)
            {
                List<int> data1 = data.Where(y => y.SegmentDetailID == i).Select(y => y.BudgetMengurusID).ToList();
                data = data.Where(x => data1.Contains(x.BudgetMengurusID)).ToList();
            }

            List<int> budgetids = data.Select(y => y.BudgetMengurusID).Distinct().ToList();
            return db.BudgetMengurus.Where(x => budgetids.Contains(x.BudgetMengurusID)).Select(x => x).ToList();
        }

        public List<BudgetMenguru> GetBudgetMengurusWithTreeCalc(List<int> LstSegmentDetailIDs, ref bool CanEdit)
        {
            List<BudgetMenguru> BudgetData = new List<BudgetMenguru>();
            List<List<int>> lstIDs = new List<List<int>>();
            List<List<int>> resultcombi = new List<List<int>>();

            if (LstSegmentDetailIDs.Count > 0)
            {
                foreach (int id in LstSegmentDetailIDs)
                    lstIDs.Add(new SegmentDetailsDAL().AllLeafDetails(id));

                List<int> index = new List<int>();
                List<int> count = new List<int>();
                List<int> result = new List<int>();

                for (int i = 0; i < lstIDs.Count; i++)
                {
                    index.Add(0);
                    count.Add(lstIDs[i].Count);
                }

                while (index.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y) != count.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y))
                {
                    List<int> sdids = new List<int>();
                    for (int id = 0; id < index.Count; id++)
                        sdids.Add(lstIDs[id][index[id]]);

                    foreach (BudgetMenguru bm in GetBudgetMengurus(sdids))
                        BudgetData.Add(bm);

                    int i = index.Count - 1;
                    while (i >= 0 && index[i] < count[i])
                    {
                        index[i] = index[i] + 1;
                        i--;
                    }

                    if (index.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y) == count.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y))
                        continue;

                    for (int j = 0; j < index.Count; j++)
                    {
                        if (index[j] == count[j])
                        {
                            index[j] = 0;
                        }
                    }
                }
            }

            string s1 = lstIDs.Select(x => (x.Count) > 0 ? x[0] : 0).OrderBy(x => x).Select(x => x.ToString()).Aggregate((x, y) => x + y);
            string s2 = LstSegmentDetailIDs.OrderBy(x => x).Select(x => x.ToString()).Aggregate((x, y) => x + y);

            if (lstIDs.Where(x => x.Count > 1).Count() == 0)
            {
                if (s1 == s2)
                {
                    CanEdit = true;
                    return BudgetData;
                }
            }
                            
            CanEdit = false;
            return BudgetData
                .GroupBy(x => new
                {
                    x.AccountCode,
                    x.PeriodMengurusID,
                    x.Status
                })
                .Select(x => new BudgetMenguru()
                {
                    BudgetMengurusID = x.Count(),
                    PeriodMengurusID = x.Key.PeriodMengurusID,
                    AccountCode = x.Key.AccountCode,
                    Amount = x.Sum(y => y.Amount),
                    Status = x.Key.Status,
                    Remarks = string.Empty
                }).ToList();

            //else
            //{
            //    CanEdit = true;
            //    return BudgetData;
            //    //.Select(x => new BudgetMenguru()
            //    //{
            //    //    BudgetMengurusID = 1,
            //    //    PeriodMengurusID = x.PeriodMengurusID,
            //    //    AccountCode = x.AccountCode,
            //    //    Amount = x.Amount,
            //    //    Status = x.Status,
            //    //    Remarks = x.Remarks,
            //    //    CreatedBy = x.CreatedBy,
            //    //    CreatedTimeStamp = x.CreatedTimeStamp,
            //    //    ModifiedBy = x.ModifiedBy,
            //    //    ModifiedTimeStamp = x.ModifiedTimeStamp
            //    //}).ToList();
            //}
        }

        public List<BudgetMenguru> GetBudgetMengurusStatus(Dictionary<int, int> SegmentAndDetailPair, ref bool CanEdit)
        {
            List<BudgetMenguru> BudgetData = new List<BudgetMenguru>();

            List<int> segDtlIDs = new SegmentDetailsDAL().AllLeafDetails(SegmentAndDetailPair);
            List<int> budID = db.JuncBgtMengurusSegDtls.Where(x => segDtlIDs.Contains(x.SegmentDetailID))
                .GroupBy(x=>new {x.BudgetMengurusID})
                //.Select(x => new { BudgetMengurusID=x.Key.BudgetMengurusID, Count = x.Count() })
                .Where(x=>x.Count() == SegmentAndDetailPair.Count)
                .Select(x=>x.Key.BudgetMengurusID).Distinct().ToList();

            BudgetData = db.BudgetMengurus.Where(x => budID.Contains(x.BudgetMengurusID)).Select(x => x).ToList();

            //List<List<int>> lstIDs = new List<List<int>>();
            //CanEdit = true;

            ////if (SegmentAndDetailPair.Count != 0)
            ////{
            //    //foreach (int segid in db.Segments.Where(x => x.Status == "A").OrderBy(x => x.SegmentOrder).Select(x => x.SegmentID))
            //    //    lstIDs.Add(db.SegmentDetails.Where(x => x.ParentDetailID == 0 && x.Status == "A" && x.SegmentID == segid).Select(x => x.SegmentDetailID).ToList());

            //    foreach (KeyValuePair<int, int> pair in SegmentAndDetailPair)
            //    {
            //        if (pair.Value == 0)
            //            lstIDs.Add(db.SegmentDetails.Where(x => x.ParentDetailID == 0 && x.Status == "A" && x.SegmentID == pair.Key).Select(x => x.SegmentDetailID).ToList());
            //        else
            //            lstIDs.Add(new List<int>() { pair.Value });
            //    }
                    
            //    List<int> index = new List<int>();
            //    List<int> count = new List<int>();
            //    List<int> result = new List<int>();

            //    for (int i = 0; i < lstIDs.Count; i++)
            //    {
            //        index.Add(0);
            //        count.Add(lstIDs[i].Count);
            //    }

            //    while (index.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y) != count.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y))
            //    {
            //        List<int> sdids = new List<int>();
            //        for (int id = 0; id < index.Count; id++)
            //            sdids.Add(lstIDs[id][index[id]]);

            //        bool flag = true;
            //        foreach (BudgetMenguru bm in GetBudgetMengurusWithTreeCalc(sdids,ref flag))
            //            BudgetData.Add(bm);

            //        CanEdit = CanEdit && flag;

            //        int i = index.Count - 1;
            //        while (i >= 0 && index[i] < count[i])
            //        {
            //            index[i] = index[i] + 1;
            //            i--;
            //        }

            //        if (index.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y) == count.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y))
            //            continue;

            //        for (int j = 0; j < index.Count; j++)
            //        {
            //            if (index[j] == count[j])
            //            {
            //                index[j] = 0;
            //            }
            //        }
            //    }
            ////}

            //if (lstIDs.Where(x => x.Count > 1).Count() > 0)
            //{
            //    CanEdit = false;
            //}

            return BudgetData
                .GroupBy(x => new
                {
                    x.AccountCode,
                    x.PeriodMengurusID,
                    x.Status
                })
                .Select(x => new BudgetMenguru()
                {
                    BudgetMengurusID = x.Count(),
                    PeriodMengurusID = x.Key.PeriodMengurusID,
                    AccountCode = x.Key.AccountCode,
                    Amount = x.Sum(y => y.Amount),
                    Status = x.Key.Status,
                    Remarks = string.Empty
                }).ToList();
        }

        public List<BudgetMengurusYearEnd> BudgetMengurusYearEnd(int Year)
        {
            return db.BudgetMengurus.Where(x => x.Status == "A" && x.PeriodMenguru.MengurusYear == Year).ToList()
                .Select(x => new BudgetMengurusYearEnd()
                {
                    ListSegmentDetails = x.JuncBgtMengurusSegDtls.OrderBy(y => y.SegmentDetail.Segment.SegmentOrder)
                                        .Select(y => y.SegmentDetail.DetailCode + "-" + y.SegmentDetail.DetailDesc).ToList(),
                    Prefix = x.JuncBgtMengurusSegDtls.OrderBy(y => y.SegmentDetail.Segment.SegmentOrder)
                                        .Select(y => y.SegmentDetail.DetailCode).Aggregate((a, b) => a + "-" + b),
                    AccountCode = x.AccountCode,
                    Description = x.AccountCode1.AccountDesc,
                    Amount = x.Amount,
                    PeriodMengurus = x.PeriodMenguru.MengurusYear + "-" + x.PeriodMenguru.FieldMenguru.FieldMengurusDesc
                }).ToList();

            //List<BudgetMenguru> BudgetData = new List<BudgetMenguru>();
            //List<List<int>> lstIDs = new List<List<int>>();
            
            //List<int> lstparnids = db.SegmentDetails.Where(x => x.Status == "A").Select(x => Convert.ToInt32(x.ParentDetailID)).Distinct().ToList();
            //lstIDs = db.SegmentDetails
            //            .Where(x => !lstparnids.Contains(x.SegmentDetailID))
            //            .OrderBy(x => x.Segment.SegmentOrder)
            //            .GroupBy(x => x.SegmentID)
            //            .Select(x => x.Select(y => y.SegmentDetailID).ToList()).ToList();

            //List<int> index = new List<int>();
            //List<int> count = new List<int>();
            //List<int> result = new List<int>();

            //for (int i = 0; i < lstIDs.Count; i++)
            //{
            //    index.Add(0);
            //    count.Add(lstIDs[i].Count);
            //}

            //while (index.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y) != count.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y))
            //{
            //    List<int> sdids = new List<int>();
            //    for (int id = 0; id < index.Count; id++)
            //        sdids.Add(lstIDs[id][index[id]]);

            //    foreach (BudgetMenguru bm in GetBudgetMengurus(sdids))
            //        BudgetData.Add(bm);

            //    int i = index.Count - 1;
            //    while (i >= 0 && index[i] < count[i])
            //    {
            //        index[i] = index[i] + 1;
            //        i--;
            //    }

            //    if (index.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y) == count.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y))
            //        continue;

            //    for (int j = 0; j < index.Count; j++)
            //    {
            //        if (index[j] == count[j])
            //        {
            //            index[j] = 0;
            //        }
            //    }
            //}
        }

        public List<BudgetMenguru> GetBudgetMengurusForDashboard()
        {
            return db.BudgetMengurus.Where(x => x.Status == "A").ToList();
        }

        public bool InsertBudgetMenguru(BudgetMenguru objBudgetMenguru, List<JuncBgtMengurusSegDtl> lstBgtMengurusSegDtl)
        {
            PeriodMenguru per = db.PeriodMengurus.Where(x => x.PeriodMengurusID == objBudgetMenguru.PeriodMengurusID).FirstOrDefault();
            try
            {
                db.BudgetMengurus.Add(objBudgetMenguru);
                foreach (JuncBgtMengurusSegDtl obj in lstBgtMengurusSegDtl)
                {
                    JuncBgtMengurusSegDtl item = new JuncBgtMengurusSegDtl();
                    item.BudgetMenguru = objBudgetMenguru;
                    item.SegmentDetailID = obj.SegmentDetailID;
                    item.CreatedBy = objBudgetMenguru.CreatedBy;
                    item.CreatedTimeStamp = DateTime.Now;
                    item.ModifiedBy = objBudgetMenguru.CreatedBy;
                    item.ModifiedTimeStamp = DateTime.Now;

                    db.JuncBgtMengurusSegDtls.Add(item);
                }
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Budget Mengurus-" + per.MengurusYear + "-" + per.FieldMenguru.FieldMengurusDesc;
                bpe.ObjectName = GetAccountCodePrefix(lstBgtMengurusSegDtl) + "-" + objBudgetMenguru.AccountCode;
                bpe.ObjectChanges = "<tr><td>Status</td><td>O</td><td>S</td></tr> <tr><td>Amount</td><td>0.00</td><td>" + objBudgetMenguru.Amount.ToString("F") + "</td></tr>";
                bpe.EventMassage = "Success";
                bpe.Status = "I";
                bpe.CreatedBy = objBudgetMenguru.CreatedBy;
                bpe.CreatedTimeStamp = objBudgetMenguru.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Budget Mengurus-" + per.MengurusYear + "-" + per.FieldMenguru.FieldMengurusDesc;
                bpe.ObjectName = GetAccountCodePrefix(lstBgtMengurusSegDtl) + "-" + objBudgetMenguru.AccountCode;
                bpe.ObjectChanges = "<tr><td>Status</td><td>O</td><td>S</td></tr> <tr><td>Amount</td><td>0.00</td><td>" + objBudgetMenguru.Amount.ToString("F") + "</td></tr>";
                bpe.EventMassage = "Failure";
                bpe.Status = "I";
                bpe.CreatedBy = objBudgetMenguru.CreatedBy;
                bpe.CreatedTimeStamp = objBudgetMenguru.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdateBudgetMenguru(BudgetMenguru objBudgetMenguru, List<int> LstSegmentDetailIDs)
        {
            BudgetMenguru obj = GetBudgetMengurus(LstSegmentDetailIDs)
                .Where(x => x.PeriodMengurusID == objBudgetMenguru.PeriodMengurusID && x.AccountCode == objBudgetMenguru.AccountCode).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(obj, objBudgetMenguru);
            try
            {
                obj.Amount = objBudgetMenguru.Amount;
                obj.Status = objBudgetMenguru.Status;
                obj.Remarks = objBudgetMenguru.Remarks;
                obj.ModifiedBy = objBudgetMenguru.ModifiedBy;
                obj.ModifiedTimeStamp = objBudgetMenguru.ModifiedTimeStamp;
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Budget Mengurus-" + obj.PeriodMenguru.MengurusYear + "-" + obj.PeriodMenguru.FieldMenguru.FieldMengurusDesc;
                bpe.ObjectName = GetAccountCodePrefix(LstSegmentDetailIDs) + "-" + objBudgetMenguru.AccountCode;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Success";
                bpe.Status = "I";
                bpe.CreatedBy = objBudgetMenguru.ModifiedBy;
                bpe.CreatedTimeStamp = objBudgetMenguru.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Budget Mengurus-" + obj.PeriodMenguru.MengurusYear + "-" + obj.PeriodMenguru.FieldMenguru.FieldMengurusDesc;
                bpe.ObjectName = GetAccountCodePrefix(LstSegmentDetailIDs) + "-" + objBudgetMenguru.AccountCode;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Failure";
                bpe.Status = "I";
                bpe.CreatedBy = objBudgetMenguru.ModifiedBy;
                bpe.CreatedTimeStamp = objBudgetMenguru.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }
        
        public bool UpdateMultipleBudgetMengurus(List<int> LstSegmentDetailIDs, List<int> LstPeriodMengurusIDs, string FromStatus, string ToStatus, string Remarks, MasterUser User)
        {
            DateTime datestamp = DateTime.Now;
            List<BudgetMenguru> data = GetBudgetMengurus(LstSegmentDetailIDs).Where(x => LstPeriodMengurusIDs.Contains(x.PeriodMengurusID) && x.Status == FromStatus).ToList();
            
            if (data.Count > 0)
            {
                try
                {
                    foreach (BudgetMenguru obj in data)
                    {
                        string changes = string.Empty;
                        if (FromStatus != ToStatus)
                            changes = changes + "<tr><td>Status</td><td>" + FromStatus.Trim() + "</td><td>" + ToStatus.Trim() + "</td></tr>";
                        if (obj.Remarks != Remarks)
                            changes = changes + "<tr><td>Remarks</td><td>" + ((obj.Remarks == null) ? string.Empty : obj.Remarks.Trim()) + "</td><td>" + Remarks.Trim() + "</td></tr>";

                        obj.Status = ToStatus;
                        obj.Remarks = Remarks;
                        obj.ModifiedBy = User.UserID;
                        obj.ModifiedTimeStamp = datestamp;

                        BPEventLog bpe = new BPEventLog();
                        bpe.Object = "Budget Mengurus-" + obj.PeriodMenguru.MengurusYear + "-" + obj.PeriodMenguru.FieldMenguru.FieldMengurusDesc;
                        bpe.ObjectName = GetAccountCodePrefix(LstSegmentDetailIDs) + "-" + obj.AccountCode;
                        //bpe.ObjectChanges = "Batch status Changed From '" + FromStatus + "' To '" + ToStatus + "', IDs : "
                        //    + data.Select(x => x.BudgetMengurusID.ToString()).Aggregate((x, y) => x + "," + y);
                        bpe.ObjectChanges = changes;
                        bpe.EventMassage = "Success";
                        bpe.Status = "I";
                        bpe.CreatedBy = User.UserID;
                        bpe.CreatedTimeStamp = datestamp;
                        new EventLogDAL().AddEventLog(bpe);
                    }
                    db.SaveChanges();


                    return true;
                }
                catch (Exception ex)
                {
                    BPEventLog bpe = new BPEventLog();
                    bpe.Object = "Budget Mengurus-Batch Change";
                    bpe.ObjectName = GetAccountCodePrefix(LstSegmentDetailIDs);
                    //bpe.ObjectChanges = "Batch status Changed From '" + FromStatus + "' To '" + ToStatus + "', IDs : "
                    //    + data.Select(x => x.BudgetMengurusID.ToString()).Aggregate((x, y) => x + "," + y);
                    bpe.EventMassage = "Failure";
                    bpe.Status = "I";
                    bpe.CreatedBy = User.UserID;
                    bpe.CreatedTimeStamp = datestamp;
                    new EventLogDAL().AddEventLog(bpe);

                    throw ex;
                }
            }

            return true;
        }

        private string GetAccountCodePrefix(List<JuncBgtMengurusSegDtl> lstBgtMengurusSegDtl)
        {
            if (lstBgtMengurusSegDtl.Count > 0)
            {
                List<int> ids = lstBgtMengurusSegDtl.Select(x => x.SegmentDetailID).ToList();
                return GetAccountCodePrefix(ids);
            }
            return string.Empty;
        }

        private string GetAccountCodePrefix(List<int> LstSegmentDetailIDs)
        {
            List<SegmentDetail> lsd = db.SegmentDetails.Where(x => LstSegmentDetailIDs.Contains(x.SegmentDetailID)).OrderBy(x => x.Segment.SegmentOrder).ToList();
            if (lsd.Count > 0)
            {
                return lsd.Select(x => x.DetailCode).Aggregate((x, y) => x + "-" + y);
            }
            return string.Empty;
        }

        public List<int> GetBlockedMengurusYears()
        {
            return db.YearEnds.Where(x => x.BudgetType == "Mengurus" && x.Status == "D").Select(x => x.BudgetYear).ToList();
        }

        public List<int> GetOpenMengurusYears()
        {
            return db.YearEnds.Where(x => x.BudgetType == "Mengurus" && x.Status == "A").Select(x => x.BudgetYear).ToList();
        }
    }
}
