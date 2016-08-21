using System;
using System.Collections.Generic;
namespace DfNet.Raws.Interpreting
{
    public interface IDfObjectContext
    {
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
    }
}
