using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class BudgetPerjawatanBAL
    {
        public IQueryable<BudgetPerjawatan> GetBudgetPerjawatans()
        {
            return new BudgetPerjawatanDAL().GetBudgetPerjawatans();
        }

        public IQueryable<JuncBgtPerjawatanSegDtl> GetPerjawatanSegDtls()
        {
            return new BudgetPerjawatanDAL().GetPerjawatanSegDtls();
        }

        public List<BudgetPerjawatan> GetBudgetPerjawatans(List<int> LstSegmentDetailIDs)
        {
            return new BudgetPerjawatanDAL().GetBudgetPerjawatans(LstSegmentDetailIDs);
        }

        public List<BudgetPerjawatan> GetBudgetPerjawatansWithTreeCalc(List<int> LstSegmentDetailIDs, ref bool CanEdit)
        {
            return new BudgetPerjawatanDAL().GetBudgetPerjawatansWithTreeCalc(LstSegmentDetailIDs, ref CanEdit);
        }

        public List<BudgetPerjawatan> GetBudgetPerjawatansStatus(Dictionary<int, int> SegmentAndDetailPair, ref bool CanEdit)
        {
            return new BudgetPerjawatanDAL().GetBudgetPerjawatansStatus(SegmentAndDetailPair, ref CanEdit);
        }

        public List<BudgetPerjawatanYearEnd> BudgetPerjawatanYearEnd(int Year)
        {
            return new BudgetPerjawatanDAL().BudgetPerjawatanYearEnd(Year);
        }

        public List<BudgetPerjawatan> GetBudgetPerjawatansForDashboard()
        {
            return new BudgetPerjawatanDAL().GetBudgetPerjawatansForDashboard();
        }

        public bool InsertBudgetPerjawatans(BudgetPerjawatan objSegment, List<JuncBgtPerjawatanSegDtl> lstBgtPerjawatansSegDtl)
        {
            try
            {
                return new BudgetPerjawatanDAL().InsertBudgetPerjawatan(objSegment, lstBgtPerjawatansSegDtl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateBudgetPerjawatans(BudgetPerjawatan objSegment, List<int> LstSegmentDetailIDs)
        {
            try
            {
                return new BudgetPerjawatanDAL().UpdateBudgetPerjawatan(objSegment, LstSegmentDetailIDs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateMultipleBudgetPerjawatans(List<int> LstSegmentDetailIDs, List<int> LstPeriodPerjawatansIDs, string FromStatus, string ToStatus, string Remarks, MasterUser User)
        {
            return new BudgetPerjawatanDAL().UpdateMultipleBudgetPerjawatans(LstSegmentDetailIDs, LstPeriodPerjawatansIDs, FromStatus, ToStatus, Remarks, User);
        }

        public List<int> GetBlockedPerjawatanYears()
        {
            try
            {
                return new BudgetPerjawatanDAL().GetBlockedPerjawatanYears();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<int> GetOpenPerjawatanYears()
        {
            try
            {
                return new BudgetPerjawatanDAL().GetOpenPerjawatanYears();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
