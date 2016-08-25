﻿using System;
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
using Tiles.Bodies;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class HardCodedAgentFactory
    {
        static IAgentCommandInterpreter CommandInterpreter = new DefaultAgentCommandInterpreter();
        static IBodyFactory BodyFactory = new BodyFactory(new TissueFactory());
        static IBodyClassFactory BodyClassFactory = new BodyClassFactory();

        static IItemClass ZombieClawClass = new ItemClass(
            name: ZombieClaw.WeaponClass.Name,
            sprite: null,
            weaponClass: ZombieClaw.WeaponClass,
            armorClass: null,
            material: null
            );
        static IItemClass ZombieTeethClass = new ItemClass ( 
            name:  ZombieTeeth.WeaponClass.Name, 
            sprite: null,
            weaponClass:  ZombieTeeth.WeaponClass,
            armorClass: null,
            material: null
            );

        public IRandom Random { get; set; }
        public HardCodedAgentFactory() : this(new RandomWrapper(new System.Random()))
        {

        }

        public HardCodedAgentFactory(IRandom random)
        {
            Random = random;
        }

        IItemFactory ItemFactory = new ItemFactory();
        public IAgent CreateZombieAgent(IAtlas atlas, Vector3 worldPos)
        {
            var bodyClass = BodyClassFactory.CreateFeralHumanoid();
            var body = BodyFactory.Create(bodyClass);
            var zombie = new Agent(
                atlas,
                new AgentClass(
                    "Shambler",
                    new Sprite(
                            symbol: Symbol.Zombie,
                            foregroundColor: Color.DarkGreen,
                            backgroundColor: Color.Black
                            ),
                    bodyClass),
                worldPos,
                body,
                new Inventory(),
                new Outfit(body, new OutfitLayerFactory()),
                new AgentCommandQueue()
                );

            zombie.Outfit.Wield(ItemFactory.Create(ZombieClawClass));
            zombie.Outfit.Wield(ItemFactory.Create(ZombieClawClass));
            zombie.Outfit.Wield(ItemFactory.Create(ZombieTeethClass));

            zombie.IsUndead = true;

            zombie.AgentBehavior = CreateBehavior(new ZombieAgentCommandPlanner(Random, new AgentCommandFactory()));
            return zombie;
        }


        public IPlayer CreatePlayer(IAtlas atlas, Vector3 worldPos)
        {
            var bodyClass = BodyClassFactory.CreateHumanoid();
            var body = BodyFactory.Create(bodyClass);
            var planner = new DoNothingAgentCommandPlanner(new AgentCommandFactory());
            var player = new Player(
                atlas,
                new AgentClass(
                    "Player",
                    new Sprite(
                            symbol: Symbol.Player,
                            foregroundColor: Color.Cyan,
                            backgroundColor: Color.Black
                            ),
                    bodyClass
                    ),
                worldPos,
                body,
                new Inventory(),
                new Outfit(body, new OutfitLayerFactory()),
                new AgentCommandQueue()
            );
            player.IsUndead = false;
            player.Agent.AgentBehavior = CreateBehavior(planner);
            return player;
        }


        public IAgent CreateSurvivor(IAtlas atlas, Vector3 worldPos)
        {
            var bodyClass = BodyClassFactory.CreateHumanoid();
            var body = BodyFactory.Create(bodyClass);
            var survivor = new Agent(atlas,
                new AgentClass(
                    "Survivor",
                    new Sprite(
                            symbol: Symbol.Survivor,
                            foregroundColor: Color.DarkRed,
                            backgroundColor: Color.Black
                            ),
                    bodyClass),
                worldPos,
                body,
                new Inventory(),
                new Outfit(body, new OutfitLayerFactory()),
                new AgentCommandQueue()
                );

            survivor.AgentBehavior = CreateBehavior(new SurvivorAgentCommandPlanner(Random, new AgentCommandFactory()));
            survivor.IsUndead = false;
            return survivor;
        }

        IAgentBehavior CreateBehavior(IAgentCommandPlanner planner)
        {
            return new CommandAgentBehavior(planner, new AgentCommandExecutionContext(CommandInterpreter));
        }

        #region Claws and Teeth
        static class ZombieClaw
        {
            public static WeaponClass WeaponClass = new WeaponClass(
                    name: "zombie claws",
                    sprite: null,
                    slots: new WeaponSlot[] { WeaponSlot.Claw },
                    attackMoveClasses: new ICombatMoveClass[] { 
                       new CombatMoveClass(
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
                               ),
                          prepTime: 1,
                          recoveryTime: 1
                           )
                           {
                               IsMartialArts = true,
                               IsStrike = true,
                               IsDefenderPartSpecific = true,
                               IsItem = true
                           },
                       new CombatMoveClass(
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
                               ),
                          prepTime: 1,
                          recoveryTime: 1
                           )
                           {
                               IsMartialArts = true,
                               IsStrike = true,
                               IsDefenderPartSpecific = true,
                               IsItem = true
                           }
                    }
                    );

        }
        static class ZombieTeeth
        {
            public static WeaponClass WeaponClass = new WeaponClass(
                    name: "zombie teeth",
                    sprite: null,
                    slots: new WeaponSlot[] { WeaponSlot.Teeth },
                    attackMoveClasses: new ICombatMoveClass[] { 
                       new CombatMoveClass(
                           name: "Bite",
                           meleeVerb: new Verb(
                            new Dictionary<VerbConjugation, string>()
                            {
                                { VerbConjugation.FirstPerson, "bite"},
                                { VerbConjugation.SecondPerson, "bite"},
                                { VerbConjugation.ThirdPerson, "bites"},
                            }, true),
                           damage: new DamageVector(
                                    new Dictionary<DamageType,uint>{
                                        { DamageType.Slash, 12 },
                                        { DamageType.Pierce, 2},
                                    }
                               ),
                          prepTime: 1,
                          recoveryTime: 1
                           )
                           {
                               IsMartialArts = true,
                               IsStrike = true,
                               IsDefenderPartSpecific = true,
                               IsItem = true
                           },
                    }
                    );

        }
        #endregion
    }   
}