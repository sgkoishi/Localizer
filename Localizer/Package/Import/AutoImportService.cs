using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Package.Load;
using Ninject;

using PackageModel = Localizer.DataModel.Default.Package;

namespace Localizer.Package.Import
{
    public sealed class AutoImportService : Disposable
    {
        public bool Imported => _imported;

        private readonly IPackageManageService _packageManage;
        private readonly SourcePackageLoad<PackageModel> _sourcePackageLoad;
        private readonly PackedPackageLoad<PackageModel> _packedPackageLoad;
        private readonly IPackageImportService _packageImportPackageImport;
        private readonly IFileLoadService _fileLoad;

        private bool _imported = false;
        
        [Inject]
        public AutoImportService(IPackageManageService packageManage,
                                 SourcePackageLoad<PackageModel> sourcePackageLoad,
                                 PackedPackageLoad<PackageModel> packedPackageLoad,
                                 IPackageImportService packageImportPackageImport,
                                 IFileLoadService fileLoad)
        {
            _packageManage = packageManage ?? throw new ArgumentNullException(nameof(packageManage));
            _sourcePackageLoad = sourcePackageLoad ?? throw new ArgumentNullException(nameof(sourcePackageLoad));
            _packedPackageLoad = packedPackageLoad ?? throw new ArgumentNullException(nameof(packedPackageLoad));
            _packageImportPackageImport = packageImportPackageImport ?? throw new ArgumentNullException(nameof(packageImportPackageImport));
            _fileLoad = fileLoad ?? throw new ArgumentNullException(nameof(fileLoad));
            LoadPackages();
            
            Hooks.BeforeModCtor += OnBeforeModCtor;
            Hooks.PostSetupContent += OnPostSetupContent;
        }

        private void OnBeforeModCtor(object mod)
        {
            if(_imported)
                return;
            Utils.SafeWrap(() =>
            {
                if (Localizer.Config.AutoImport)
                {
                    var wrapped = new LoadedModWrapper(mod);
                    Utils.LogInfo($"Early auto import for mod: [{wrapped.Name}]");
                    Import(wrapped);
                }
            });
        }
        
        private void OnPostSetupContent()
        {
            if(_imported)
                return;
            
            try
            {
                if (Localizer.Config.AutoImport)
                {
                    Utils.LogInfo($"Late auto import begin.");
                    Import();
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e);
            }
            finally
            {
                _imported = true;
            }
        }

        private void LoadPackages()
        {
            void LoadPackedPackages()
            {
                foreach (var file in new DirectoryInfo(Localizer.DownloadPackageDirPath).GetFiles())
                {                
                    Utils.SafeWrap(() =>
                    {
                        var pack = _packedPackageLoad.Load(file.FullName, _fileLoad);
                        if (pack == null)
                        {
                            return;
                        }

                        _packageManage.AddPackage(pack);
                    });
                }
            }

            void LoadSourcePackages()
            {
                foreach (var dir in new DirectoryInfo(Localizer.SourcePackageDirPath).GetDirectories())
                {                
                    Utils.SafeWrap(() =>
                    {
                        var pack = _sourcePackageLoad.Load(dir.FullName, _fileLoad);
                        if (pack == null)
                        {
                            return;
                        }

                        _packageManage.AddPackage(pack);
                    });
                }
            }

            try
            {
                _packageManage.PackageGroups = new List<IPackageGroup>();
                
                var type = Localizer.Config.AutoImportType;
                if(type != AutoImportType.DownloadedOnly)
                    LoadSourcePackages();
                if(type != AutoImportType.SourceOnly)
                    LoadPackedPackages();

                _packageManage.LoadState();
            }
            catch (Exception e)
            {
                Utils.LogError(e);
            }
        }

        private void Import(IMod mod = null)
        {
            void QueuePackageGroup(IPackageGroup packageGroup)
            {
                if(packageGroup is null || !packageGroup.Mod.Enabled)
                    return;
                
                foreach (var p in packageGroup.Packages)
                {
                    if (p.Enabled)
                    {
                        _packageImportPackageImport.Queue(p);
                    }
                }
            }

            Utils.SafeWrap(() =>
            {
                _packageImportPackageImport.Clear();

                if (mod is null)
                {
                    foreach (var pg in _packageManage.PackageGroups)
                    {
                        QueuePackageGroup(pg);
                    }
                }
                else
                {
                    QueuePackageGroup(_packageManage.PackageGroups.FirstOrDefault(pg => pg.Mod.Name == mod.Name));
                }

                _packageImportPackageImport.Import(true);

                Localizer.RefreshLanguages();

                Utils.LogDebug("Auto import end.");
            });
        }

        protected override void DisposeUnmanaged()
        {
            Hooks.PostSetupContent -= OnPostSetupContent;
            Hooks.BeforeModCtor -= OnBeforeModCtor;
        }
    }
}
