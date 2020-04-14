using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DnD_2._0._0
{
    public partial class Form3 : Form
    {
        Dictionary<string, string> dicout;

        public Form3(Dictionary<string, string> dicup)
        {
            InitializeComponent();
            DataGridView dgv = dataGridView1;
            dgv = dataGridView1;
            dgv.ColumnCount = 1;
            dgv.RowCount = dicup.Count;
            string[] key = dicup.Keys.ToArray();
            for (int i = 0; i < key.Length; i++)
            {
                dgv.Rows[i].HeaderCell.Value = key[i];
                dgv.Rows[i].Cells[0].Value = dicup[key[i]];
            }
            dicout = dicup;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] key = dicout.Keys.ToArray();
            for (int i = 0; i < key.Length; i++)
            { dicout[key[i]] = dataGridView1.Rows[i].Cells[0].Value.ToString(); }
            this.Close();
        }
    }
}
