using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Terraria.ModLoader;

namespace Localizer.Services.File
{
    public sealed class BasicFileExportService<T> : IFileExportService where T : IFile
    {
        public void Export(IPackage package, IExportConfig config)
        {
            var modName = package.ModName;
            var mod = Utils.GetModByName(modName);

            if (mod == null)
            {
                return;
            }

            var file = Activator.CreateInstance(typeof(T)) as IFile;

            foreach (var prop in typeof(T).GetTModLocalizeFieldPropInfos())
            {
                var fieldName = prop.GetTModLocalizeFieldName();

                dynamic field = typeof(Mod).GetFieldDirectly(mod, fieldName);

                var entryType = prop.PropertyType.GenericTypeArguments
                                    .FirstOrDefault(g => g.GetInterfaces().Contains(typeof(IEntry)));

                var entries = CreateEntries(field, entryType, package.Language, config.WithTranslation);

                dynamic entriesOfFile = prop.GetValue(file);
                foreach (var e in entries)
                {
                    entriesOfFile.Add(e.Key, e.Value);
                }
            }

            package.AddFile(file);
        }

        private Dictionary<string, object> CreateEntries(dynamic localizeOwners, Type entryType, CultureInfo lang,
                                                         bool withTranslation)
        {
            var entries = new Dictionary<string, object>();

            var mappings = Utils.CreateEntryMappings(entryType);

            foreach (var pair in localizeOwners)
            {
                var entry = Activator.CreateInstance(entryType);

                foreach (var mapping in mappings)
                {
                    object owner = pair.Value;
                    var localizeTrans = owner?.GetType().GetProperty(mapping.Key)?.GetValue(owner) as ModTranslation;

                    mapping.Value.SetValue(entry, new BaseEntry
                    {
                        Origin = localizeTrans.DefaultOrEmpty(),
                        Translation = withTranslation ? localizeTrans?.GetTranslation(lang) : ""
                    });
                }

                entries.Add(pair.Key, entry);
            }

            return entries;
        }
    }
}
