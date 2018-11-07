using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeserWard.Controls;

namespace TimeTracking.Database
{
    public static class DBManager
    {
        public static TTDates GetDateDataFromDate(DateTime date)
        {
            using (var context = new MSUtilityDBEntities())
            {
                var result = context.TTDates
                    .Include("TTKundeEntry")
                    .Include("TTKundeEntry.TTKunde")
                    .Include("TTKundeEntry.TTProjectEntry")
                    .Include("TTKundeEntry.TTProjectEntry.TTProject")
                    .Include("TTKundeEntry.TTProjectEntry.TTEntryData").FirstOrDefault(x => x.Day == date.Date);
                return result;
            }
        }

        public static List<DateTime> GetCalenderEntries()
        {
            using (var context = new MSUtilityDBEntities())
            {
                return (from c in context.TTDates where c.Day.Month == StateManager.CurrentInstance.CurrentSelectedDate.Month && c.Day.Year == StateManager.CurrentInstance.CurrentSelectedDate.Year select c).Include("TTKundeEntry").ToList().Where(x=> x.TTKundeEntry.Count != 0).Select(x=> x.Day).ToList();
            }
        }

        public static List<TTKunde> GetKunden()
        {
            using (var context = new MSUtilityDBEntities())
            {
                return (from c in context.TTKunde select c).Distinct().ToList();
            }
        }

        public static List<TTProject> GetProjects(TTKundeEntry currentInstanceSelectedKundeEntry)
        {
            if (currentInstanceSelectedKundeEntry == null) return new List<TTProject>();
            
            using (var context = new MSUtilityDBEntities())
            {
                return (from c in context.TTProject where c.KundeId == currentInstanceSelectedKundeEntry.KundeID select c).Distinct().ToList();
            }
        }

        

        //public static List<TTDates> GetDateData(int currentMonth, int currentYear)
        //{
        //    using (var context = new MSUtilityDBEntities())
        //    {
        //        var result = context.TTDates.Include("TTKundeEntry")
        //            .Include("TTKundeEntry.TTKunde")
        //            .Include("TTKundeEntry.TTProjectEntry")
        //            .Include("TTKundeEntry.TTProjectEntry.TTProject")
        //            .Include("TTKundeEntry.TTProjectEntry.TTEntryData")
        //            //.Include(x=> x.TTKundeEntry.Select(e => e.TTKunde))
        //            //.Include(x => x.TTKundeEntry.Select(e => e.TTProjectEntry))
        //            //.Include("TTKunde.TTProjectEntry")
        //            .Where(x=> x.Day.Month == currentMonth && x.Day.Year == currentYear).ToList();
        //        return result;
        //    }
        //}


        public class LinqToEntititesResultsProviderKunde : IIntelliboxResultsProvider
        {
            public List<TTKunde> SearchData;

            public LinqToEntititesResultsProviderKunde(List<TTKunde> searchdata)
            {
                SearchData = searchdata;
            }
            IEnumerable IIntelliboxResultsProvider.DoSearch(string searchTerm, int maxResults, object extraInfo)
            {
                var data = SearchData.Where(i => i.Kunde.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase)).Distinct();
                return data;
            }
        }

        public class LinqToEntititesResultsProviderProject : IIntelliboxResultsProvider
        {
            public List<TTProject> SearchData;

            public LinqToEntititesResultsProviderProject(List<TTProject> searchdata)
            {
                SearchData = searchdata;
            }
            IEnumerable IIntelliboxResultsProvider.DoSearch(string searchTerm, int maxResults, object extraInfo)
            {
                var data = SearchData.Where(i => i.Project.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase)).Distinct();
                return data;
            }
        }
    }
}
