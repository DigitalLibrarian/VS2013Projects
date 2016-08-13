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
using Tiles.Agents.CommandInterpreters;
using Tiles.Items.Outfits;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class AgentFactory
    {
        static IAgentCommandInterpreter CommandInterpreter = new DefaultAgentCommandInterpreter();
        static BodyFactory BodyFactory = new BodyFactory();

        public IRandom Random { get; set; }
        public AgentFactory(IRandom random)
        {
            Random = random;
        }

        static class ZombieClaw
        {
            public static WeaponClass WeaponClass = new WeaponClass(
                    name: "zombie claws",
                    sprite: null,
                    slots: new WeaponSlot[] { WeaponSlot.Main},
                    attackMoveClasses: new IAttackMoveClass[] { 
                       new AttackMoveClass(
                           name: "Scratch",
                           meleeVerb: new Verb(
                            new Dictionary<VerbConjugation, string>()
                            {
                                { VerbConjugation.FirstPerson, "scratch"},
                                { VerbConjugation.SecondPerson, "scratch"},
                                { VerbConjugation.ThirdPerson, "scratches"},
                            }, true),
                           damage: new DamageVector(
                                    new Dictionary<DamageType,uint>{
                                        { DamageType.Slash, 12 },
                                        { DamageType.Pierce, 2},
                                    }
                               )
                           ),
                       new AttackMoveClass(
                           name: "Rake",
                           meleeVerb: new Verb(
                            new Dictionary<VerbConjugation, string>()
                            {
                                { VerbConjugation.FirstPerson, "rake"},
                                { VerbConjugation.SecondPerson, "rake"},
                                { VerbConjugation.ThirdPerson, "rakes"},
                            }, true),
                           damage: new DamageVector(
                                    new Dictionary<DamageType,uint>{
                                        { DamageType.Slash, 18 },
                                        { DamageType.Pierce, 5},
                                    }
                               )
                           )
                    }
                    );

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

            zombie.Outfit.Wield(new Item { Name = ZombieClaw.WeaponClass.Name,  WeaponClass = ZombieClaw.WeaponClass });

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
            return new CommandAgentBehavior(planner, new AgentCommandExecutionContext(CommandInterpreter));
        }
    }   
}
