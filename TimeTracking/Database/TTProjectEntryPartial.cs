using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracking.Logic;

namespace TimeTracking.Database
{
    public partial class TTProjectEntry
    {
        public EditProjectIdCommand EditProjectID { get; set; } = new EditProjectIdCommand();
        public OpenBrowserCommand OpenBrowserComm { get; set; } = new OpenBrowserCommand();
    }
}
