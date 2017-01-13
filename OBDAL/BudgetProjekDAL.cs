using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class BudgetProjekYearEnd
    {
        public List<string> ListSegmentDetails { get; set; }
        public string Prefix { get; set; }
        public string AccountCode { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string PeriodMengurus { get; set; }
    }

    public class BudgetProjekDAL
    {
        OBEntities db = new OBEntities();

        public IQueryable<BudgetProjek> GetBudgetProjek()
        {
            return db.BudgetProjeks;        
        }

        public IQueryable<JuncBgtProjekSegDtl> GetMengurusSegDtls()
        {
            return db.JuncBgtProjekSegDtls;
        }

        public List<BudgetProjek> GetBudgetProjek(List<int> LstSegmentDetailIDs)
        {
            List<JuncBgtProjekSegDtl> data = db.JuncBgtProjekSegDtls.Where(x => LstSegmentDetailIDs.Contains(x.SegmentDetailID)).Select(x => x).ToList();
            foreach (int i in LstSegmentDetailIDs)
            {
                List<int> data1 = data.Where(y => y.SegmentDetailID == i).Select(y => y.BudgetProjekID).ToList();
                data = data.Where(x => data1.Contains(x.BudgetProjekID)).ToList();
            }

            List<int> budgetids = data.Select(y => y.BudgetProjekID).Distinct().ToList();
            return db.BudgetProjeks.Where(x => budgetids.Contains(x.BudgetProjekID)).Select(x => x).ToList();
        }

        public List<BudgetProjek> GetBudgetProjekWithTreeCalc(List<int> LstSegmentDetailIDs, ref bool CanEdit)
        {
            List<BudgetProjek> BudgetData = new List<BudgetProjek>();
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

                    foreach (BudgetProjek bm in GetBudgetProjek(sdids))
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
                .Select(x => new BudgetProjek()
                {
                    BudgetProjekID = x.Count(),
                    PeriodMengurusID = x.Key.PeriodMengurusID,
                    AccountCode = x.Key.AccountCode,
                    Amount = x.Sum(y => y.Amount),
                    Status = x.Key.Status,
                    Remarks = string.Empty
                }).ToList();
        }

        public List<BudgetProjek> GetBudgetProjekStatus(Dictionary<int, int> SegmentAndDetailPair, ref bool CanEdit)
        {
            List<BudgetProjek> BudgetData = new List<BudgetProjek>();

            List<int> segDtlIDs = new SegmentDetailsDAL().AllLeafDetails(SegmentAndDetailPair);
            List<int> budID = db.JuncBgtProjekSegDtls.Where(x => segDtlIDs.Contains(x.SegmentDetailID))
                .GroupBy(x=>new {x.BudgetProjekID})
                .Where(x=>x.Count() == SegmentAndDetailPair.Count)
                .Select(x=>x.Key.BudgetProjekID).Distinct().ToList();

            BudgetData = db.BudgetProjeks.Where(x => budID.Contains(x.BudgetProjekID)).Select(x => x).ToList();

            return BudgetData
                .GroupBy(x => new
                {
                    x.AccountCode,
                    x.PeriodMengurusID,
                    x.Status
                })
                .Select(x => new BudgetProjek()
                {
                    BudgetProjekID = x.Count(),
                    PeriodMengurusID = x.Key.PeriodMengurusID,
                    AccountCode = x.Key.AccountCode,
                    Amount = x.Sum(y => y.Amount),
                    Status = x.Key.Status,
                    Remarks = string.Empty
                }).ToList();
        }

        public List<BudgetProjekYearEnd> BudgetProjekYearEnd(int Year)
        {
            return db.BudgetProjeks.Where(x => x.Status == "A" && x.PeriodMenguru.MengurusYear == Year).ToList()
                .Select(x => new BudgetProjekYearEnd()
                {
                    ListSegmentDetails = x.JuncBgtProjekSegDtls.OrderBy(y => y.SegmentDetail.Segment.SegmentOrder)
                                        .Select(y => y.SegmentDetail.DetailCode + "-" + y.SegmentDetail.DetailDesc).ToList(),
                    Prefix = x.JuncBgtProjekSegDtls.OrderBy(y => y.SegmentDetail.Segment.SegmentOrder)
                                        .Select(y => y.SegmentDetail.DetailCode).Aggregate((a, b) => a + "-" + b),
                    AccountCode = x.AccountCode,
                    Description = x.AccountCode1.AccountDesc,
                    Amount = x.Amount,
                    PeriodMengurus = x.PeriodMenguru.MengurusYear + "-" + x.PeriodMenguru.FieldMenguru.FieldMengurusDesc
                }).ToList();
        }

        public List<BudgetProjek> GetBudgetProjekForDashboard()
        {
            return db.BudgetProjeks.Where(x => x.Status == "A").ToList();
        }

        public bool InsertBudgetProjek(BudgetProjek objBudgetProjek, List<JuncBgtProjekSegDtl> lstBgtProjekSegDtl)
        {
            PeriodMenguru per = db.PeriodMengurus.Where(x => x.PeriodMengurusID == objBudgetProjek.PeriodMengurusID).FirstOrDefault();
            try
            {
                db.BudgetProjeks.Add(objBudgetProjek);
                foreach (JuncBgtProjekSegDtl obj in lstBgtProjekSegDtl)
                {
                    JuncBgtProjekSegDtl item = new JuncBgtProjekSegDtl();
                    item.BudgetProjek = objBudgetProjek;
                    item.SegmentDetailID = obj.SegmentDetailID;
                    item.CreatedBy = objBudgetProjek.CreatedBy;
                    item.CreatedTimeStamp = DateTime.Now;
                    item.ModifiedBy = objBudgetProjek.CreatedBy;
                    item.ModifiedTimeStamp = DateTime.Now;

                    db.JuncBgtProjekSegDtls.Add(item);
                }
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Budget Projek-" + per.MengurusYear + "-" + per.FieldMenguru.FieldMengurusDesc;
                bpe.ObjectName = GetAccountCodePrefix(lstBgtProjekSegDtl) + "-" + objBudgetProjek.AccountCode;
                bpe.ObjectChanges = "<tr><td>Status</td><td>O</td><td>S</td></tr> <tr><td>Amount</td><td>0.00</td><td>" + objBudgetProjek.Amount.ToString("F") + "</td></tr>";
                bpe.EventMassage = "Success";
                bpe.Status = "I";
                bpe.CreatedBy = objBudgetProjek.CreatedBy;
                bpe.CreatedTimeStamp = objBudgetProjek.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Budget Projek-" + per.MengurusYear + "-" + per.FieldMenguru.FieldMengurusDesc;
                bpe.ObjectName = GetAccountCodePrefix(lstBgtProjekSegDtl) + "-" + objBudgetProjek.AccountCode;
                bpe.ObjectChanges = "<tr><td>Status</td><td>O</td><td>S</td></tr> <tr><td>Amount</td><td>0.00</td><td>" + objBudgetProjek.Amount.ToString("F") + "</td></tr>";
                bpe.EventMassage = "Failure";
                bpe.Status = "I";
                bpe.CreatedBy = objBudgetProjek.CreatedBy;
                bpe.CreatedTimeStamp = objBudgetProjek.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdateBudgetProjek(BudgetProjek objBudgetProjek, List<int> LstSegmentDetailIDs)
        {
            BudgetProjek obj = GetBudgetProjek(LstSegmentDetailIDs)
                .Where(x => x.PeriodMengurusID == objBudgetProjek.PeriodMengurusID && x.AccountCode == objBudgetProjek.AccountCode).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(obj, objBudgetProjek);
            try
            {
                obj.Amount = objBudgetProjek.Amount;
                obj.Status = objBudgetProjek.Status;
                obj.Remarks = objBudgetProjek.Remarks;
                obj.ModifiedBy = objBudgetProjek.ModifiedBy;
                obj.ModifiedTimeStamp = objBudgetProjek.ModifiedTimeStamp;
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Budget Projek-" + obj.PeriodMenguru.MengurusYear + "-" + obj.PeriodMenguru.FieldMenguru.FieldMengurusDesc;
                bpe.ObjectName = GetAccountCodePrefix(LstSegmentDetailIDs) + "-" + objBudgetProjek.AccountCode;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Success";
                bpe.Status = "I";
                bpe.CreatedBy = objBudgetProjek.ModifiedBy;
                bpe.CreatedTimeStamp = objBudgetProjek.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Budget Projek-" + obj.PeriodMenguru.MengurusYear + "-" + obj.PeriodMenguru.FieldMenguru.FieldMengurusDesc;
                bpe.ObjectName = GetAccountCodePrefix(LstSegmentDetailIDs) + "-" + objBudgetProjek.AccountCode;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Failure";
                bpe.Status = "I";
                bpe.CreatedBy = objBudgetProjek.ModifiedBy;
                bpe.CreatedTimeStamp = objBudgetProjek.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }
        
        public bool UpdateMultipleBudgetProjek(List<int> LstSegmentDetailIDs, List<int> LstPeriodMengurusIDs, string FromStatus, string ToStatus, string Remarks, MasterUser User)
        {
            DateTime datestamp = DateTime.Now;
            List<BudgetProjek> data = GetBudgetProjek(LstSegmentDetailIDs).Where(x => LstPeriodMengurusIDs.Contains(x.PeriodMengurusID) && x.Status == FromStatus).ToList();
            
            if (data.Count > 0)
            {
                try
                {
                    foreach (BudgetProjek obj in data)
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
                        bpe.Object = "Budget Projek-" + obj.PeriodMenguru.MengurusYear + "-" + obj.PeriodMenguru.FieldMenguru.FieldMengurusDesc;
                        bpe.ObjectName = GetAccountCodePrefix(LstSegmentDetailIDs) + "-" + obj.AccountCode;
                        //bpe.ObjectChanges = "Batch status Changed From '" + FromStatus + "' To '" + ToStatus + "', IDs : "
                        //    + data.Select(x => x.BudgetProjekID.ToString()).Aggregate((x, y) => x + "," + y);
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
                    bpe.Object = "Budget Projek-Batch Change";
                    bpe.ObjectName = GetAccountCodePrefix(LstSegmentDetailIDs);
                    //bpe.ObjectChanges = "Batch status Changed From '" + FromStatus + "' To '" + ToStatus + "', IDs : "
                    //    + data.Select(x => x.BudgetProjekID.ToString()).Aggregate((x, y) => x + "," + y);
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

        private string GetAccountCodePrefix(List<JuncBgtProjekSegDtl> lstBgtProjekSegDtl)
        {
            if (lstBgtProjekSegDtl.Count > 0)
            {
                List<int> ids = lstBgtProjekSegDtl.Select(x => x.SegmentDetailID).ToList();
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

        public List<int> GetBlockedProjekYears()
        {
            return db.YearEnds.Where(x => x.BudgetType == "Projek" && x.Status == "D").Select(x => x.BudgetYear).ToList();
        }

        public List<int> GetOpenProjekYears()
        {
            return db.YearEnds.Where(x => x.BudgetType == "Projek" && x.Status == "A").Select(x => x.BudgetYear).ToList();
        }
    }
}
