using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracking
{
    public static class StaticHelper
    {
        /// <summary>
        /// DoEvents() von WinForm | Quelle: http://proggie.rosenbohm.org/2012/07/23/das-leidige-wpf-doevents-problem/
        /// </summary>
        internal static void DoEvents()
        {
            System.Windows.Forms.Application.DoEvents();
        }
    }
}
