
using Tiles.Agents.Behaviors;
namespace Tiles.Agents
{
    public interface IAgentBehavior
    {
        IAgentCommandExecutionContext Context { get; }
        void Update(IGame game, IAgent agent);
    }

}
