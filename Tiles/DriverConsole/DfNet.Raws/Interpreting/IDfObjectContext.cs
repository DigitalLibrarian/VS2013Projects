using System;
using System.Collections.Generic;
namespace DfNet.Raws.Interpreting
{
    public interface IDfObjectContext
    {
        IEnumerable<DfTag> WorkingSet { get; }

        DfObject Source { get; }
        int Cursor { get; }

        void GoToEnd();
        void GoToStart();
        bool GoToTag(string name);
        int RemoveTagsByName(string tagName);
        bool ReplaceTag(DfTag original, DfTag newTag);

        DfObject Create();

        void InsertTags(params DfTag[] newTags);

        void StartPass();
        void EndPass();

        void CopyTagsFrom(DfObject creatureDf);

        void Remove(params DfTag[] dfTag);
    }
}
