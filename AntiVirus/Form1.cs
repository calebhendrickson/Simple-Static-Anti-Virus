using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace AntiVirus
{
    public partial class Form1 : Form
    {
        int virusesDetected;
        List<string> files = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }

        // BROWSE
        private void button1_Click(object sender, EventArgs e)
        {
            // Select a folder to scan
            // folder label
            folderBrowserDialog1.ShowDialog();
            label2.Text = folderBrowserDialog1.SelectedPath;

            // virus label
            virusesDetected = 0;
            label1.Text = "Viruses Detected:" + virusesDetected.ToString();
            progressBar1.Value = 0;
            listBox1.Items.Clear();

        }

        // SCAN
        private void button2_Click(object sender, EventArgs e)
        {

            // searchResults contains a list of files
            // List<string> searchResults = Directory.GetFiles(@folderBrowserDialog1.SelectedPath, "*.*").ToList();
            GetFilesRecursive(@folderBrowserDialog1.SelectedPath);

            List<string> searchResults = files;

            progressBar1.Maximum = searchResults.Count;

            // get md5 signatures from virus db
            var md5Signatures = File.ReadAllLines("md5virusdb.txt");

            // for each file in searchResult list
            foreach (string item in searchResults)
            {
                var fileSignature = GetMD5FromFile(item);

                if (md5Signatures.Contains(fileSignature))
                {
                    // virus + 1 
                    virusesDetected += 1;
                    label1.Text = "Viruses Detected:" + virusesDetected.ToString();
                    listBox1.Items.Add(item);
                    progressBar1.Increment(1);
                }
                else
                {
                    // no virus
                    progressBar1.Increment(1);
                }

            }
  
        }

        // HELPER FOR BROWSE
        // RECURSIVE FILE SEARCH
        private void GetFilesRecursive(string sDir)
        {
            foreach (string dir in Directory.GetDirectories(sDir))
            {
                try
                {
                    foreach (string file in Directory.GetFiles(dir, "*.*"))
                    {
                        //string currentFile = Path.GetFileName(file);
                        string currentFile = Path.GetFullPath(file);
                        files.Add(currentFile);
                        Console.WriteLine(currentFile);
                    }

                    GetFilesRecursive(dir);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

        }

        // HELPER FOR SCAN
        public string GetMD5FromFile(string filePath)
        {
            // instantiating md5 class
            using (var md5 = MD5.Create())
            {
                // reading one file
                using(var stream = File.OpenRead(filePath))
                {
                    // converts base data types to an array of bytes,
                    // and an array of bytes to base data types.

                    // computes hash for the read file
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
                }
            }
        }
    }
}
