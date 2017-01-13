using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class BudgetProjekBAL
    {
        public IQueryable<BudgetProjek> GetBudgetProjek()
        {
            return new BudgetProjekDAL().GetBudgetProjek();
        }

        public IQueryable<JuncBgtProjekSegDtl> GetMengurusSegDtls()
        {
            return new BudgetProjekDAL().GetMengurusSegDtls();
        }

        public List<BudgetProjek> GetBudgetProjek(List<int> LstSegmentDetailIDs)
        {
            return new BudgetProjekDAL().GetBudgetProjek(LstSegmentDetailIDs);
        }

        public List<BudgetProjek> GetBudgetProjekWithTreeCalc(List<int> LstSegmentDetailIDs, ref bool CanEdit)
        {
            return new BudgetProjekDAL().GetBudgetProjekWithTreeCalc(LstSegmentDetailIDs, ref CanEdit);
        }

        public List<BudgetProjek> GetBudgetProjekStatus(Dictionary<int, int> SegmentAndDetailPair, ref bool CanEdit)
        {
            return new BudgetProjekDAL().GetBudgetProjekStatus(SegmentAndDetailPair, ref CanEdit);
        }

        public List<BudgetProjekYearEnd> BudgetProjekYearEnd(int Year)
        {
            return new BudgetProjekDAL().BudgetProjekYearEnd(Year);
        }

        public List<BudgetProjek> GetBudgetProjekForDashboard()
        {
            return new BudgetProjekDAL().GetBudgetProjekForDashboard();
        }

        public bool InsertBudgetProjek(BudgetProjek objSegment, List<JuncBgtProjekSegDtl> lstBgtProjekSegDtl)
        {
            try
            {
                return new BudgetProjekDAL().InsertBudgetProjek(objSegment, lstBgtProjekSegDtl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateBudgetProjek(BudgetProjek objSegment, List<int> LstSegmentDetailIDs)
        {
            try
            {
                return new BudgetProjekDAL().UpdateBudgetProjek(objSegment, LstSegmentDetailIDs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateMultipleBudgetProjek(List<int> LstSegmentDetailIDs, List<int> LstPeriodMengurusIDs, string FromStatus, string ToStatus, string Remarks, MasterUser User)
        {
            return new BudgetProjekDAL().UpdateMultipleBudgetProjek(LstSegmentDetailIDs, LstPeriodMengurusIDs, FromStatus, ToStatus, Remarks, User);
        }

        public List<int> GetBlockedProjekYears()
        {
            try
            {
                return new BudgetProjekDAL().GetBlockedProjekYears();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<int> GetOpenProjekYears()
        {
            try
            {
                return new BudgetProjekDAL().GetOpenProjekYears();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
