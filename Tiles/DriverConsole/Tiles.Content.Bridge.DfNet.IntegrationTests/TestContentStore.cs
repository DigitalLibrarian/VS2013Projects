using DfNet.Raws;
using DfNet.Raws.Parsing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Bridge.DfNet.IntegrationTests
{
    public static class TestContentStore
    {
        private const string DirKey = @"DwarfFortressRawsDirectory";
        static TestContentStore()
        {
            var rawDirPath = ConfigurationManager.AppSettings.Get(DirKey);
            var raws = Directory.GetFiles(rawDirPath, "*", SearchOption.AllDirectories)
                .ToDictionary(
                    fileName => fileName,
                    fileName => File.ReadLines(fileName));
            var parser = new DfObjectParser(new DfTagParser());
            Store = new DfObjectStore(
                parser.Parse(raws.SelectMany(x => x.Value),
                DfTags.GetAllObjectTypes()));
        }

        private static IDfObjectStore Store { get; set; }

        internal static global::DfNet.Raws.IDfObjectStore Get()
        {
            return Store;
        }
    }
}
