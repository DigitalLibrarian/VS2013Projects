using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;
using Tiles.Control;
using Tiles.Items;
using Tiles.Render;
using Tiles.Math;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Agents.Behaviors;
using Tiles.Random;
using Tiles.Structures;
using DfNet.Raws;
using Tiles.ScreensImpl.SiteFactories;
using Tiles.Ecs;
using Tiles.Content.Map;



namespace Tiles.ScreensImpl.ContentFactories
{
    public class GameFactory
    {
        IDfObjectStore DfStore { get; set; }
        DfTagsFascade Df { get; set; }
        IRandom Random { get; set; }
        IEntityManager EntityManager { get; set; }

        public GameFactory()
        {
            var dfRawDir = System.Configuration.ConfigurationManager.AppSettings.Get(@"DwarfFortressRawsDirectory");
            EntityManager = new EntityManager();
            Random = new RandomWrapper(new System.Random(42));
            DfStore = DfObjectStore.CreateFromDirectory(dfRawDir);
            Df = new DfTagsFascade(DfStore, EntityManager, Random);
        }

        public IGame SetupGenericZombieWorld()
        {

            Vector3 siteSize = new Vector3(64, 64, 64);
            var siteFactory = new ZombieSiteFactory(EntityManager, Random);
            return Setup(EntityManager, siteFactory, siteSize, Random);
        }

        public IGame SetupArenaWorld()
        {
            var siteSize = new Vector3(64, 64, 64);

            var weaponMats = new string[]{
                "COPPER", "SILVER", "STEEL", "IRON"
            };
            var weaponTypes = new string[]{
                DfTags.MiscTags.ITEM_WEAPON_MACE,
                DfTags.MiscTags.ITEM_WEAPON_SWORD_SHORT,
                DfTags.MiscTags.ITEM_WEAPON_SPEAR,
                DfTags.MiscTags.ITEM_WEAPON_HAMMER_WAR,
                DfTags.MiscTags.ITEM_WEAPON_AXE_BATTLE,
                DfTags.MiscTags.ITEM_WEAPON_WHIP,
                DfTags.MiscTags.ITEM_WEAPON_PICK
            };
            List<IItem> invItems = new List<IItem>();
            foreach (var weaponType in weaponTypes)
            {
                foreach (var weaponMat in weaponMats)
                {
                    invItems.Add(Df.CreateInorganicWeapon(weaponType, weaponMat));
                }
                invItems.Add(Df.CreateMaterialTemplateWeapon(weaponType, "WOOD_TEMPLATE"));
            }

            var siteFactory = new ArenaSiteFactory(DfStore, EntityManager, Random);
            return Setup(EntityManager, siteFactory, siteSize, Random, invItems.ToArray());
        }

        public IGame SetupDfTestWorld()
        {

            var siteSize = new Vector3(64, 64, 64);
            var siteFactory = new DfTestSiteFactory(DfStore, EntityManager, Random);
            return Setup(EntityManager, siteFactory, siteSize, Random);

        }

        private Game Setup(IEntityManager entityManager,
            ISiteFactory siteFactory, Vector3 siteSize, IRandom random, 
            params IItem[] invItems)
        {
            var atlas = new Atlas(siteFactory, siteSize);

            var actionLog = new ActionLog();
            
            var spawnBox = new Box3(
                Vector3.Zero,
                new Vector3(siteSize.X, siteSize.Y, 1)
                );
            
            var spawnPos = FindSpawnLocation(atlas, random, spawnBox);
            var player = CreatePlayer(random, entityManager, atlas, spawnPos);

            foreach (var item in invItems)
            {
                player.Inventory.AddItem(item);
            }

            var camera = new Camera(spawnPos);

            return new Game(entityManager, atlas, player, camera, actionLog, random);
        }

        private IPlayer CreatePlayer(IRandom random, IEntityManager entityManager, IAtlas atlas, Vector3 spawnPos)
        {
            var agentClass = Df.CreateCreatureAgentClass(atlas, "DWARF", "MALE", spawnPos);
            return new HardCodedAgentFactory(entityManager, random).CreatePlayer(atlas, agentClass, spawnPos);
        }

        Vector3 FindSpawnLocation(IAtlas atlas, IRandom random, Box3 spawnBox)
        {
            return random.FindRandomInBox(spawnBox, test =>
            {
                var tile = atlas.GetTileAtPos(test);
                return tile.HasRoomForAgent;
            }).Value;
        }

    }
}
