﻿using System;
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

        private void LoadFile(string path)
        {
            var lines = ReadLines(path).ToList();

            var data = Parser.Parse(lines);

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
    }
}