using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Behaviors;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Items;
using Tiles.Math;
using Tiles.Random;
using Tiles.Agents.Combat.CommandInterpreters;
using Tiles.Items.Outfits;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class AgentFactory
    {
        static IAgentCommandInterpreter CommandInterpreter = new DefaultAgentCommandInterpreter();
        static BodyFactory BodyFactory = new BodyFactory();

        public IRandom Random { get; set; }

        IAgentContextStatusObserver StatusObserver { get; set; }
        public AgentFactory(IRandom random, IAgentContextStatusObserver statusObserver)
        {
            Random = random;
            StatusObserver = statusObserver;
        }

        class ZombieClaw : IWeapon
        {
            public ZombieClaw()
            {
                WeaponClass = new WeaponClass(
                    "zombie claws",
                    null,
                    new DamageVector(new Dictionary<DamageType, uint>
                    {
                        { DamageType.Slash, 12},
                        { DamageType.Pierce, 2},
                    }),
                    "mauls",
                    WeaponSlot.None);
            }

            public IWeaponClass WeaponClass { get; private set; }

            public uint GetBaseTypeDamage(DamageType damageType)
            {
                return WeaponClass.DamageVector.GetComponent(damageType);
            }
        }
        public IAgent CreateZombieAgent(IAtlas atlas, Vector2 worldPos)
        {
            var body = BodyFactory.CreateHumanoid();
            var zombie = new Agent(atlas,
                new Sprite(
                        symbol: Symbol.Zombie,
                        foregroundColor: Color.DarkGreen,
                        backgroundColor: Color.Black
                        ),
                worldPos,
                body,
                "Shambler",
                new Inventory(),
                new Outfit(body, new OutfitLayerFactory())
                );

            zombie.Outfit.Wield(new Item { Weapon = new ZombieClaw() });

            zombie.IsUndead = true;

            zombie.AgentBehavior = CreateBehavior(new ZombieAgentCommandPlanner(Random, new AgentCommandFactory()));
            return zombie;
        }


        public IPlayer CreatePlayer(IAtlas atlas, Vector2 worldPos)
        {
            var body = BodyFactory.CreateHumanoid();
            var planner = new QueueAgentCommandPlanner(Random, new AgentCommandFactory());
            var player = new Player(
                atlas,
                new Sprite(
                        symbol: Symbol.Player,
                        foregroundColor: Color.Cyan,
                        backgroundColor: Color.Black
                        ),
                worldPos,
                body,
                new Inventory(),
                new Outfit(body, new OutfitLayerFactory()),
                planner
            );
            player.IsUndead = false;
            player.Agent.AgentBehavior = CreateBehavior(planner);
            return player;
        }


        public IAgent CreateSurvivor(IAtlas atlas, Vector2 worldPos)
        {
            var body = BodyFactory.CreateHumanoid();
            var planner = new QueueAgentCommandPlanner(Random, new AgentCommandFactory());
            var survivor = new Agent(atlas,
                new Sprite(
                        symbol: Symbol.Survivor,
                        foregroundColor: Color.DarkRed,
                        backgroundColor: Color.Black
                        ),
                worldPos,
                body,
                "Survivor",
                new Inventory(),
                new Outfit(body, new OutfitLayerFactory())
                );

            survivor.AgentBehavior = CreateBehavior(new SurvivorAgentCommandPlanner(Random, new AgentCommandFactory()));
            survivor.IsUndead = false;
            return survivor;
        }

        IAgentBehavior CreateBehavior(IAgentCommandPlanner planner)
        {
            return new CommandAgentBehavior(planner, new AgentCommandExecutionContext(StatusObserver, planner, CommandInterpreter));
        }
    }   
}
