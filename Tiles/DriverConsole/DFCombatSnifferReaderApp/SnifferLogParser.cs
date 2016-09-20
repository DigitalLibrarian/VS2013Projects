using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfCombatSnifferReaderApp
{
    public interface ISnifferLogData
    {
        List<SnifferSession> Sessions { get; }
    }

    public class SnifferLogData : ISnifferLogData
    {
        public List<SnifferSession> Sessions { get; private set; }
        public SnifferLogData()
        {
            Sessions = new List<SnifferSession>();
        }
    }

    public class SnifferSession
    {
        public List<string> ReportTexts { get; set; }
        public SnifferSession()
        {
            ReportTexts = new List<string>();
        }
    }

    public interface ISnifferLogParser
    {
        ISnifferLogData Parse(IEnumerable<string> lines);
    }

    class ParserContext
    {
        public SnifferLogData Data { get; set; }
        public SnifferSession Session { get; set; }
        public ParserContext()
        {
            Data = new SnifferLogData();
        }
    }

    public class SnifferLogParser : ISnifferLogParser
    {
        public ISnifferLogData Parse(IEnumerable<string> lines)
        {
            var context = new ParserContext();
            var enumerator = lines.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var line = enumerator.Current;
                if (line.Trim().Equals("")) continue;
                switch (line)
                {
                    case SnifferTags.SessionStart:
                        HandleSessionStart(context, line, enumerator);
                        break;
                    case SnifferTags.AttackStart:
                        HandleAttackStart(context, line, enumerator);
                        break;
                    default:
                        if (IsKeyValue(SnifferTags.ReportText, line))
                        {
                            HandleReportText(context, line, enumerator);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        break;
                }
                
            }

            return context.Data;
        }

        private void HandleAttackStart(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            while (enumerator.MoveNext())
            {
                line = enumerator.Current;
                if (line.Equals(SnifferTags.AttackEnd)) break;
            }
        }


        private const char KeyValueSeparator = ':';
        private bool IsKeyValue(string key, string line)
        {
            if (!line.Contains(KeyValueSeparator)) return false;

            var lineKey = ParseKey(line);
            return lineKey.Equals(key);
        }

        private string ParseKey(string keyValue)
        {
            return keyValue.Split(KeyValueSeparator).First();
        }
        private string ParseValue(string keyValue)
        {
            return keyValue.Split(KeyValueSeparator).Skip(1).First();
        }

        private void HandleReportText(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            var v = ParseValue(line);
            context.Session.ReportTexts.Add(v);
        }

        private void HandleSessionStart(ParserContext context, string line, IEnumerator<string> enumerator)
        {
            context.Session = new SnifferSession();
            context.Data.Sessions.Add(context.Session);
        }
    }
}
