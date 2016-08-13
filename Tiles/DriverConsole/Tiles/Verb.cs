using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Math;

namespace Tiles
{
    public abstract class ConjugatedWord
    {
        IDictionary<VerbConjugation, string> Conjugations { get; set; }

        public ConjugatedWord(IDictionary<VerbConjugation, string> conjugations)
        {
            Conjugations = conjugations;
        }

        public string Conjugate(VerbConjugation vc)
        {
            if (Conjugations.ContainsKey(vc))
            {
                return Conjugations[vc];
            }
            throw new MissingVerbConjugationException(vc);
        }
    }

    public class Verb : ConjugatedWord, IVerb
    {
        public bool IsTransitive { get; private set; }

        public Verb(IDictionary<VerbConjugation, string> conjugationMap, bool isTransitive)
            : base(conjugationMap)
        {
            IsTransitive = isTransitive;
        }
    }

    public class MissingVerbConjugationException : Exception
    {
        public VerbConjugation VerbConjugation { get; private set; }

        public MissingVerbConjugationException(VerbConjugation vc)
            : base(string.Format("Missing verb conjugation {0}.", vc))
        {
            VerbConjugation = vc;
        }
    }

    public class Noun : ConjugatedWord, INoun
    {
        public bool IsProper { get; set; }
        
        public Noun(IDictionary<VerbConjugation, string> conjugationMap, bool isProper)
            : base(conjugationMap)
        {
            IsProper = isProper;
        }
    }


    public interface INounFactory
    {
        INoun FirstPerson();
        INoun SecondPerson(IAgent agent);

        INoun AgentBodyPart(IAgent agent, IBodyPart part);
        INoun AgentItem(IAgent agent, IItem item);
        INoun Agent(IAgent agent);

        INoun Item(IItem item);
    }

 


    public interface IActionReportMessageFormatter
    {
        string ReportAgentSubVerbObject(Vector2 actionWorldPos, INoun subject, IVerb verb, INoun obj, string punctuation, VerbConjugation conjugation);
    }

    public class ActionReportMessageFormatter : IActionReportMessageFormatter
    {
        public string ReportAgentSubVerbObject(Vector2 actionWorldPos, INoun subject, IVerb verb, INoun obj, string punctuation, VerbConjugation conjugation)
        {
            if (!verb.IsTransitive) throw new GrammaticalException();

            string subjectStr = SubjectConjugation(subject, conjugation);
            string verbStr = verb.Conjugate(conjugation);
            string objStr = SubjectConjugation(obj, conjugation);
            string articleStr = obj.IsProper ? "" : "the ";

            string message = string.Format("{0} {1} {2}{3}{4}",
                subjectStr,
                verbStr,
                articleStr,
                objStr,
                punctuation
                );
            return message;
        }

        string SubjectConjugation(INoun noun, VerbConjugation conj)
        {
            var nounStr = noun.Conjugate(conj);
            if (conj == VerbConjugation.ThirdPerson && !noun.IsProper)
            {
                return string.Format("The {0}", nounStr);
            }
            return nounStr;
        }
    }

    public class GrammaticalException : Exception { }
}
