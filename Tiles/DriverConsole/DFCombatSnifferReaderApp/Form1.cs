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

namespace DfCombatSnifferReaderApp
{
    public partial class Form1 : Form
    {
        const string CheatPath = @"D:\git\VS2013Projects\Tiles\DfHackScripts\combat-sniffer-log.txt";

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

        Dictionary<TreeNode, SnifferNode> TreeToSnif = new Dictionary<TreeNode, SnifferNode>();

        private void LoadFile(string path)
        {
            TreeToSnif.Clear();

            var lines = ReadLines(path).ToList();

            var data = Parser.Parse(lines);

            TreeNode treeNode = null;
            int sessionCount = 1;
            foreach (var session in data.Sessions)
            {
                var strikeNodes = new List<TreeNode>();
                foreach (var strike in session.Strikes)
                {
                    //if (strike.ReportText.StartsWith("Dwarf 3 "))
                    {
                        int woundCount = 1;
                        var woundNodes = new List<TreeNode>();
                        foreach (var wound in strike.Wounds)
                        {
                            var wbpNodes = new List<TreeNode>();
                            foreach (var wbp in wound.Parts)
                            {
                                treeNode = new TreeNode(wbp.KeyValues[SnifferTags.BodyPartName]);
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
    }
}
