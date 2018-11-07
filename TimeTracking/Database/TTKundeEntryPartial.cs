using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracking.Logic;

namespace TimeTracking.Database
{
    public partial class TTKundeEntry
    {
        public RemoveCommand REmoveCommand { get; set; } = new RemoveCommand();
        public AddProjectCommand AddProjectCommand { get; set; } = new AddProjectCommand();
    }
}
