using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class BudgetMengurusBAL
    {
        public IQueryable<BudgetMenguru> GetBudgetMengurus()
        {
            return new BudgetMengurusDAL().GetBudgetMengurus();
        }

        public IQueryable<JuncBgtMengurusSegDtl> GetMengurusSegDtls()
        {
            return new BudgetMengurusDAL().GetMengurusSegDtls();
        }

        public List<BudgetMenguru> GetBudgetMengurus(List<int> LstSegmentDetailIDs)
        {
            return new BudgetMengurusDAL().GetBudgetMengurus(LstSegmentDetailIDs);
        }

        public List<BudgetMenguru> GetBudgetMengurusWithTreeCalc(List<int> LstSegmentDetailIDs, ref bool CanEdit)
        {
            return new BudgetMengurusDAL().GetBudgetMengurusWithTreeCalc(LstSegmentDetailIDs, ref CanEdit);
        }

        public List<BudgetMenguru> GetBudgetMengurusStatus(Dictionary<int, int> SegmentAndDetailPair, ref bool CanEdit)
        {
            return new BudgetMengurusDAL().GetBudgetMengurusStatus(SegmentAndDetailPair, ref CanEdit);
        }

        public List<BudgetMengurusYearEnd> BudgetMengurusYearEnd(int Year)
        {
            return new BudgetMengurusDAL().BudgetMengurusYearEnd(Year);
        }

        public List<BudgetMenguru> GetBudgetMengurusForDashboard()
        {
            return new BudgetMengurusDAL().GetBudgetMengurusForDashboard();
        }

        public bool InsertBudgetMengurus(BudgetMenguru objSegment, List<JuncBgtMengurusSegDtl> lstBgtMengurusSegDtl)
        {
            try
            {
                return new BudgetMengurusDAL().InsertBudgetMenguru(objSegment, lstBgtMengurusSegDtl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateBudgetMengurus(BudgetMenguru objSegment, List<int> LstSegmentDetailIDs)
        {
            try
            {
                return new BudgetMengurusDAL().UpdateBudgetMenguru(objSegment, LstSegmentDetailIDs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateMultipleBudgetMengurus(List<int> LstSegmentDetailIDs, List<int> LstPeriodMengurusIDs, string FromStatus, string ToStatus, string Remarks, MasterUser User)
        {
            return new BudgetMengurusDAL().UpdateMultipleBudgetMengurus(LstSegmentDetailIDs, LstPeriodMengurusIDs, FromStatus, ToStatus, Remarks, User);
        }

        public List<int> GetBlockedMagurusYears()
        {
            try
            {
                return new BudgetMengurusDAL().GetBlockedMengurusYears();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<int> GetOpenMengurusYears()
        {
            try
            {
                return new BudgetMengurusDAL().GetOpenMengurusYears();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
