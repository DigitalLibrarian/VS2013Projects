using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DfCombatSnifferReaderApp
{

    public partial class Form1 : Form
    {
        const string CheatPath = @"D:\git\VS2013Projects\Tiles\CombatSnifferLogs\combat-sniffer-log-3.txt";

        static class ViewerTabs
        {
            public const int ReportText = 0;
            public const int Strike = 1;

            public const int Units = 2;
        }

        ISnifferLogParser Parser { get; set; }

        public Form1()
        {
            InitializeComponent();
            
            Parser = new SnifferLogParser();
            LoadFile(CheatPath);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog();

            if(result == System.Windows.Forms.DialogResult.OK)
            {
                if (openFileDialog1.CheckFileExists)
                {
                    LoadFile(openFileDialog1.FileName);
                }
            }
        }

        string LoadedFile { get; set; }
        IEnumerable<string> FileData { get; set; }

        Dictionary<TreeNode, SnifferNode> TreeToSnif = new Dictionary<TreeNode, SnifferNode>();
        Dictionary<TreeNode, AttackStrike> TreeToStrike = new Dictionary<TreeNode, AttackStrike>();
        Dictionary<AttackStrike, TreeNode> StrikeToTree = new Dictionary<AttackStrike, TreeNode>();
        Dictionary<int, AttackStrike> ReportTextToStrike = new Dictionary<int, AttackStrike>();

        Dictionary<Unit, TreeNode> UnitNodes = new Dictionary<Unit, TreeNode>();
        Dictionary<TreeNode, Unit> UnitHyperLinks = new Dictionary<TreeNode, Unit>();

        private void LoadFile(string path)
        {
            FileData = ReadLines(path).ToList();
            LoadFileData();
            LoadedFile = path;
        }

        class SessionComboItem
        {

            public string Display { get; set; }
            public int Value { get; set; }
        }

        ISnifferLogData Data { get; set; }
        SnifferSession CurrentSession { get; set; }
        private void LoadFileData()
        {
            var data = Parser.Parse(FileData);
            Data = data;

            int sessionCount = 0;
            comboBox1.DisplayMember = "Display";
            comboBox1.ValueMember = "Value";
            comboBox1.DataSource = data.Sessions
                .OrderBy(s => s.Id)
                .Select(s =>
                {
                    var sessionName = string.Format("Session #{0}", s.Id);
                    if (s.Name != null)
                    {
                        sessionName = s.Name;
                    }
                    var o = new SessionComboItem() { Value = sessionCount, Display = sessionName };
                    sessionCount++;
                    return o;
                }).ToList();

            CurrentSession = Data.Sessions.First();

            ReloadSession();
        }

        private void ReloadSession(string filterRegex = ".")
        {
            LoadSession(CurrentSession, filterRegex);
        }

        private void LoadSession(SnifferSession session, string filterRegex)
        {
            StrikeTree.Nodes.Clear();
            StrikesNodeDisplayListView.Clear();

            UnitsTree.Nodes.Clear();
            UnitsNodeDisplayListView.Clear();

            reportLogListView.Items.Clear();

            UnitNodes.Clear();
            UnitHyperLinks.Clear();
            TreeToSnif.Clear();
            TreeToStrike.Clear();
            StrikeToTree.Clear();
            ReportTextToStrike.Clear();

            TreeNode treeNode = null;
            var strikeNodes = new List<TreeNode>();
            foreach (var strike in session.Strikes)
            {
                var strikeReportText = session.GetReportText(strike);
                if (strikeReportText == null)
                {
                    string killHint = strike.KeyValues[SnifferTags.WoundId] == "-1" ? "(Looks like kill)" : "";
                    strikeReportText = string.Format("{0} vs {1} {2}", strike.KeyValues[SnifferTags.AttackerName], strike.KeyValues[SnifferTags.DefenderName], killHint);
                }
                if (Regex.IsMatch(strikeReportText, filterRegex))
                {
                    var attackerUnit = session.Units.First(x => x.Name.Equals(strike.AttackerName));
                    var defenderUnit = session.Units.First(x => x.Name.Equals(strike.DefenderName));




                    int woundCount = 1;
                    var woundNodes = new List<TreeNode>();
                    foreach (var wound in strike.Wounds)
                    {
                        var wbpNodes = new List<TreeNode>();
                        foreach (var wbp in wound.Parts)
                        {
                            var layerNodes = new List<TreeNode>();
                            foreach (var tl in wbp.Layers)
                            {
                                treeNode = new TreeNode(tl.KeyValues[SnifferTags.TissueLayerName]);
                                layerNodes.Add(treeNode);
                                TreeToSnif[treeNode] = tl;
                            }

                            var wbpNodeName = wbp.KeyValues[SnifferTags.BodyPartNameSingular];
                            treeNode = new TreeNode(wbpNodeName, layerNodes.ToArray());
                            wbpNodes.Add(treeNode);
                            TreeToSnif[treeNode] = wbp;
                        }
                        treeNode = new TreeNode(string.Format("Wound #{0}", woundCount++), wbpNodes.ToArray());
                        woundNodes.Add(treeNode);
                        TreeToSnif[treeNode] = wound;
                    }


                    var attackerHyperNode = new TreeNode(string.Format("Attacker: {0}", attackerUnit.Name));
                    UnitHyperLinks[attackerHyperNode] = attackerUnit;
                    TreeToSnif[attackerHyperNode] = attackerUnit;
                    var defenderHyperNode = new TreeNode(string.Format("Defender: {0}", defenderUnit.Name));
                    UnitHyperLinks[defenderHyperNode] = defenderUnit;
                    TreeToSnif[defenderHyperNode] = defenderUnit;

                    var strikeNode = new TreeNode(strikeReportText, 
                        new TreeNode[]{ 
                            attackerHyperNode, 
                            defenderHyperNode
                        }.Concat(woundNodes).ToArray());

                    strikeNodes.Add(strikeNode);
                    TreeToSnif[strikeNode] = strike;

                    TreeToStrike[strikeNode] = strike;
                    StrikeToTree[strike] = strikeNode;

                    if (strike.ReportTextIndex != -1)
                    {
                        ReportTextToStrike[strike.ReportTextIndex] = strike;
                    }

                    StrikeTree.Nodes.Add(strikeNode);
                }
            }

            int reportTextIndex = 0;
            foreach (var reportText in session.ReportTexts)
            {
                var lvi = new ListViewItem(reportText);
                if (session.Strikes.Any(strike => strike.ReportTextIndex == reportTextIndex))
                {
                    lvi.BackColor = Color.LightGreen;
                }
                else
                {
                    if(Parser.IsCombatText(reportText))
                    {
                        lvi.BackColor = Color.LightPink;
                        lvi.ToolTipText = "Looks like a missing attack event went with this report text.";
                    }
                }
                reportLogListView.Items.Add(lvi);
                reportTextIndex++;
            }


            foreach(var unit in session.Units.OrderBy(u => u.Id))
            {
                var partNodes = new List<TreeNode>();
                foreach(var bp in unit.Body.BodyParts)
                {
                    var layerNodes = new List<TreeNode>();
                    foreach(var layer in bp.Layers)
                    {
                        var layerNode = new TreeNode(layer.Name);
                        TreeToSnif[layerNode] = layer;
                        layerNodes.Add(layerNode);
                    }

                    var partNode = new TreeNode(bp.Name, layerNodes.ToArray());
                    TreeToSnif[partNode] = bp;
                    partNodes.Add(partNode);
                }
                var bodyPartsNode = new TreeNode("Body Parts", partNodes.ToArray());

                var bpAttackNodes = new List<TreeNode>();
                foreach(var bpAttack in unit.BodyPartAttacks)
                {
                    var bpAttackNode = new TreeNode(bpAttack.Name);
                    TreeToSnif[bpAttackNode] = bpAttack;
                    bpAttackNodes.Add(bpAttackNode);
                }
                var bpAttacksNode = new TreeNode("Body Part Attacks", bpAttackNodes.ToArray());

                var armorNodes = new List<TreeNode>();
                foreach(var armor in unit.Armors)
                {
                    var armorNode = new TreeNode(armor.Name);
                    TreeToSnif[armorNode] = armor;
                    armorNodes.Add(armorNode);
                }
                var armorsNode = new TreeNode("Armor", armorNodes.ToArray());

                var weaponNodes = new List<TreeNode>();
                foreach(var weapon in unit.Weapons)
                {
                    var attackNodes = new List<TreeNode>();
                    foreach(var attack in weapon.Attacks)
                    {
                        var attackNode = new TreeNode(attack.Name);
                        TreeToSnif[attackNode] = attack;
                        attackNodes.Add(attackNode);
                    }
                    var weaponNode = new TreeNode(weapon.Name, attackNodes.ToArray());
                    TreeToSnif[weaponNode] = weapon;
                    weaponNodes.Add(weaponNode);
                }
                var weaponsNode = new TreeNode("Weapons", weaponNodes.ToArray());

                var bodyNode = new TreeNode("Body", new TreeNode[]{
                    bodyPartsNode,
                    bpAttacksNode,
                });

                TreeToSnif[bodyNode] = unit.Body;

                var unitNode = new TreeNode(unit.Name, new TreeNode[]{
                    bodyNode,
                    armorsNode,
                    weaponsNode,
                });

                TreeToSnif[unitNode] = unit;
                UnitNodes[unit] = unitNode;

                UnitsTree.Nodes.Add(unitNode);
            }
        }

        private IEnumerable<string> ReadLines(string path)
        {
            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            KeyPreview = true;
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = (int)comboBox1.SelectedValue;
            CurrentSession = Data.Sessions[id];
            ReloadSession();
        }

        private void reportLogListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            HyperLinkSelectedReportText();
        }

        private void HyperLinkSelectedReportText()
        {
             var index = reportLogListView.SelectedIndices[0];
             if (ReportTextToStrike.ContainsKey(index))
             {
                 var strike = ReportTextToStrike[index];
                 var treeNode = StrikeToTree[strike];
                 HyperLink(ViewerTabs.Strike, StrikeTree, treeNode);
             }
        }

        private void HyperLinkSelectedUnit()
        {
            var node = StrikeTree.SelectedNode;
            if (UnitHyperLinks.ContainsKey(node))
            {
                DoHyperLinkUnit(node);
            }

        }

        private void HyperLink(int tabId, TreeView treeView, TreeNode treeNode)
        {
            var index = reportLogListView.SelectedIndices[0];
            var strike = ReportTextToStrike[index];

            TabControl.SelectTab(tabId);

            treeView.SelectedNode = treeNode;
            treeNode.Expand();
            treeNode.EnsureVisible();
        }

        private void reportLogListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                HyperLinkSelectedReportText();
            }
        }

        private void UnitsTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SnifferNodeTree_AfterSelect(UnitsTree, UnitsNodeDisplayListView);
        }


        void StrikeTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SnifferNodeTree_AfterSelect(StrikeTree, StrikesNodeDisplayListView);
        }

        void SnifferNodeTree_AfterSelect(TreeView treeView, ListView nodeDisplayView)
        {
            nodeDisplayView.Clear();
            if (TreeToSnif.ContainsKey(treeView.SelectedNode))
            {
                var asset = TreeToSnif[treeView.SelectedNode];

                foreach (var key in asset.KeyValues.Keys)
                {
                    nodeDisplayView.Items.Add(string.Format("{0} : {1}", key, asset.KeyValues[key]));
                }
            }
        }

        private void StrikeTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var node = e.Node;

            DoHyperLinkUnit(node);
        }

        void DoHyperLinkUnit(TreeNode node)
        {
            if (UnitHyperLinks.ContainsKey(node))
            {
                var unit = UnitHyperLinks[node];
                var unitNode = UnitNodes[unit];
                HyperLink(ViewerTabs.Units, UnitsTree, unitNode);
            }
        }

        private void StrikeTree_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                HyperLinkSelectedUnit();
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F5)
            {
                LoadFile(LoadedFile);
            }
        }

    }
}
