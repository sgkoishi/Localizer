using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Localizer;
using Localizer.DataModel;
using Localizer.ServiceInterfaces.Network;
using Localizer.ServiceInterfaces.Package;
using Ninject;

namespace LocalizerWPF.ViewModel
{
    public class BrowserViewModel : ViewModelBase
    { 
        public ObservableCollection<IPackage> Packages { get; set; }

        public IPackage SelectedPackage
        {
            get => _selectedPackage;
            set
            {
                _selectedPackage = value;
                RaisePropertyChanged("SelectedPackage");
            }
        }

        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand<IPackage> DownloadCommand { get; set; }

        private IPackageBrowserService packageBrowserService;
        private IDownloadManagerService downloadManager;
        private IPackage _selectedPackage;

        public BrowserViewModel()
        {
            packageBrowserService = Localizer.Localizer.Kernel.Get<IPackageBrowserService>();
            downloadManager = Localizer.Localizer.Kernel.Get<IDownloadManagerService>();
            
            RefreshCommand = new RelayCommand(Refresh);
            DownloadCommand = new RelayCommand<IPackage>(Download);

            Packages = new ObservableCollection<IPackage>(packageBrowserService.GetList());
        }

        private void Refresh()
        {
            Utils.LogDebug($"Fetching package list");
            Packages = new ObservableCollection<IPackage>(packageBrowserService.GetList());
            Utils.LogDebug($"Package list fetched");
        }

        private void Download(IPackage pack)
        {
            Utils.LogDebug($"Requesting {pack.Name} download");
            var url = packageBrowserService.GetDownloadLinkOf(pack);
            
            var path = Utils.EscapePath(Path.Combine(Localizer.Localizer.DownloadPackageDirPath, pack.Name + pack.Author + ".locpack"));
            
            downloadManager.Download(url, path);
            
            Utils.LogDebug($"{pack.Name} is downloading");
        }
    }
}
