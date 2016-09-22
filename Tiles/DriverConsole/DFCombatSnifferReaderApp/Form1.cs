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

        IEnumerable<string> FileData { get; set; }

        Dictionary<TreeNode, SnifferNode> TreeToSnif = new Dictionary<TreeNode, SnifferNode>();
        Dictionary<TreeNode, AttackStrike> TreeToStrike = new Dictionary<TreeNode, AttackStrike>();
        Dictionary<AttackStrike, TreeNode> StrikeToTree = new Dictionary<AttackStrike, TreeNode>();

        private void LoadFile(string path)
        {
            FileData = ReadLines(path).ToList();
            LoadFileData();
        }

        private void LoadFileData(string filterRegex = ".")
        {
            treeView1.Nodes.Clear();
            TreeToSnif.Clear();
            TreeToStrike.Clear();
            StrikeToTree.Clear();

            var data = Parser.Parse(FileData);

            TreeNode treeNode = null;
            int sessionCount = 1;
            foreach (var session in data.Sessions)
            {
                var strikeNodes = new List<TreeNode>();
                foreach (var strike in session.Strikes)
                {
                    if (Regex.IsMatch(strike.ReportText, filterRegex))
                    {
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

                        var strikeNode = new TreeNode(strike.ReportText, woundNodes.ToArray());
                        strikeNodes.Add(strikeNode);
                        TreeToSnif[strikeNode] = strike;

                        TreeToStrike[strikeNode] = strike;
                        StrikeToTree[strike] = strikeNode;
                    }
                }

                var sessionNode = new TreeNode(string.Format("Session #{0}", sessionCount++), strikeNodes.ToArray());
                treeView1.Nodes.Add(sessionNode);

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
            treeView1.AfterSelect += treeView1_AfterSelect;
        }

        void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(TreeToSnif.ContainsKey(treeView1.SelectedNode))
            {
                var asset = TreeToSnif[treeView1.SelectedNode];

                listView1.Clear();
                foreach (var key in asset.KeyValues.Keys)
                {
                    listView1.Items.Add(string.Format("{0} : {1}", key, asset.KeyValues[key]));
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var regex = textBox1.Text;
            LoadFileData(regex);
            foreach (var topNode in treeView1.Nodes)
            {
                (topNode as TreeNode).Expand();
            }
            treeView1.Select();
        }
    }
}
