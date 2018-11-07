using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FeserWard.Controls;
using TimeTracking.Database;

namespace TimeTracking.UI
{
    /// <summary>
    /// Interaction logic for NewEntry.xaml
    /// </summary>
    public partial class NewProject : INotifyPropertyChanged
    {
        private IIntelliboxResultsProvider _linqtoentitiesprovider;

        public IIntelliboxResultsProvider LinqToEntitiesProvider
        {
            get => _linqtoentitiesprovider;
            set
            {
                _linqtoentitiesprovider = value;
                OnPropertyChanged("_linqtoentitiesprovider");
                tbproject.DataProvider = LinqToEntitiesProvider;
            }
        }

        private TTProject _selectedProject;

        public TTProject SelectedProject
        {
            get { return _selectedProject; }
            set { _selectedProject = value; }
        }


        public NewProject()
        {
            InitializeComponent();
            SelectedProject = null;
            tbproject.KeyDown += (sender, e) =>
            {
                if (e.Key != Key.Return)
                {
                    return;
                }
                using (var context = new MSUtilityDBEntities())
                {
                    object selectedItem = ((Intellibox)sender).SelectedItem;
                    if (selectedItem == null)
                    {
                        //Item existierte noch nicht. Anlegen!
                        var stringitem = ((TextBox) ((Grid) ((Intellibox) sender).Content).Children[0]).Text;

                        TTProject project = new TTProject();
                        project.Project = stringitem;
                        project.KundeId = StateManager.CurrentInstance.CurrentSelectedTTKundeEntry.KundeID;
                        context.TTProject.Add(project);
                        SelectedProject = project;
                    }
                    else
                    {
                        SelectedProject = (TTProject)selectedItem;
                    }

                    if (SelectedProject != null)
                    {
                        var projectEntry =
                            (from c in context.TTProjectEntry
                                where c.ProjectId == SelectedProject.Id &&
                                      c.KundeEntryId == StateManager.CurrentInstance.CurrentSelectedTTKundeEntry.ID
                                select c).FirstOrDefault();
                        if (projectEntry == null)
                        {
                            projectEntry = new TTProjectEntry();
                            projectEntry.ProjectId = SelectedProject.Id;
                            projectEntry.KundeEntryId = StateManager.CurrentInstance.CurrentSelectedTTKundeEntry.ID;
                            context.TTProjectEntry.Add(projectEntry);
                        }
                        else
                        {
                            MainWindow.CurrentInstance.NewKundePopupOpen = false;
                            MainWindow.CurrentInstance.ShowGlobalMessage("Information",
                                "Projekteintrag existiert bereits!");
                        }
                    }
                    MainWindow.CurrentInstance.NewProjectPopupOpen = false;
                    ((Intellibox) sender).SelectedItem = null;
                    context.SaveChanges();
                    StateManager.CurrentInstance.RefreshData();
                    LinqToEntitiesProvider = new DBManager.LinqToEntititesResultsProviderProject(DBManager.GetProjects(StateManager.CurrentInstance.CurrentSelectedTTKundeEntry).ToList());
                }
            };
            tbproject.ResultsList.SelectionChanged += delegate
            {
                if (tbproject.SelectedItem == null)
                {
                    return;
                }
                SelectedProject = (TTProject)tbproject.SelectedValue;
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
