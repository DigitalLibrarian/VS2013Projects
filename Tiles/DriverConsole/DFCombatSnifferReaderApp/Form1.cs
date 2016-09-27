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
        const string CheatPath = @"D:\git\VS2013Projects\Tiles\CombatSnifferLogs\combat-sniffer-log-1.txt";

        static class ViewerTabs
        {
            public const int ReportText = 0;
            public const int Strike = 1;
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

        IEnumerable<string> FileData { get; set; }

        Dictionary<TreeNode, SnifferNode> TreeToSnif = new Dictionary<TreeNode, SnifferNode>();
        Dictionary<TreeNode, AttackStrike> TreeToStrike = new Dictionary<TreeNode, AttackStrike>();
        Dictionary<AttackStrike, TreeNode> StrikeToTree = new Dictionary<AttackStrike, TreeNode>();
        Dictionary<int, AttackStrike> ReportTextToStrike = new Dictionary<int, AttackStrike>();

        private void LoadFile(string path)
        {
            FileData = ReadLines(path).ToList();
            LoadFileData();
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
            comboBox1.DataSource = data.Sessions.Select(s =>
                {

                    var o = new SessionComboItem() { Value = sessionCount, Display = string.Format("Session #{0}", sessionCount+1) };
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
            treeView1.Nodes.Clear();
            reportLogListView.Items.Clear();
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

                    var strikeNode = new TreeNode(strikeReportText, woundNodes.ToArray());
                    strikeNodes.Add(strikeNode);
                    TreeToSnif[strikeNode] = strike;

                    TreeToStrike[strikeNode] = strike;
                    StrikeToTree[strike] = strikeNode;

                    if (strike.ReportTextIndex != -1)
                    {
                        ReportTextToStrike[strike.ReportTextIndex] = strike;
                    }

                    treeView1.Nodes.Add(strikeNode);
                }
            }

            int index = 0;
            foreach (var reportText in session.ReportTexts)
            {
                var lvi = new ListViewItem(reportText);
                if (session.Strikes.Any(strike => strike.ReportTextIndex == index))
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
                index++;
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
            ReloadSession(regex);
            treeView1.Select();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = (int)comboBox1.SelectedValue;
            CurrentSession = Data.Sessions[id];
            ReloadSession(textBox1.Text);
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

                tabControl1.SelectTab(ViewerTabs.Strike);
                var treeNode = StrikeToTree[strike];
                treeView1.SelectedNode = treeNode;
                treeNode.EnsureVisible();
                treeNode.ExpandAll();
            }
        }

        private void reportLogListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                HyperLinkSelectedReportText();
            }
        }
    }
}
