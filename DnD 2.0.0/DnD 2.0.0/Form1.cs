using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace DnD_2._0._0
{
    public partial class Form1 : Form
    {
        public List<ContextMenuStrip> masContMenStr;
        public enum HeroColumnsNumer { para, effe, eqip, inve }

        Esense origin;
        List<Esense> pasiv;
        FormulaConten formulas; // источник действия

        List<Human> putesh;
        List<Esense> opponent;
        List<Effekt>[] maseffet;
        List<Predmet>[] maspredmet;
        List<Esense>[] masmonstr;

        public Form1()
        {
            InitializeComponent();
            WriteRead.ReadProperty("profile\\Form.xml");
            WriteRead.ReadModelPers("profile\\ModelPerson.xml");
            WriteRead.ReadTypeArmor("profile\\Damage.xml", "profile\\Armor.xml");
            WriteRead.ReadTables("profile\\Tables.xml");
            for (int i = 0; i < tabControl1.Controls.Count; i++)
            { tabControl1.Controls[i].BackColor = Color.DarkOrange; }
            Chasi.Time0("12 : 04");
            putesh = new List<Human>();
            opponent = new List<Esense>();
            сохранитьToolStripMenuItem.Enabled = false;

            masContMenStr = new List<ContextMenuStrip>();
        }

        private List<Esense> EsenseMas()
        {
            List<Esense> mases = new List<Esense>();
            mases.AddRange(opponent);
            mases.AddRange(putesh.Cast<Esense>().ToList());
            return mases;
        }

        public void GroupBox_Click(object sender, EventArgs e)
        {
            GroupBox box = sender as GroupBox;
            if (box == null)
            { box = ((Control)sender).Parent as GroupBox; }
            EsenseMas().Find((Esense x) => x.Box(box)).Market = true;
            //qqqqq
            if (origin != null)
            {
                int n = EsenseMas().Count((Esense x) => x.Market);
                if (formulas.target + 1 == n)
                {
                    pasiv = EsenseMas().FindAll((Esense x) => x.Market);
                    pasiv.Remove(origin);
                    Event(origin, pasiv, formulas);

                    origin.Market = true;
                    foreach (Esense item in pasiv)
                    { item.Market = true; }
                    this.Cursor = Cursors.Arrow;
                }
            }
            else { this.Cursor = Cursors.Arrow; }
        }

        private void загрузитьToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            WriteRead.Load(putesh, ref maseffet, ref maspredmet, ref masmonstr);
            //
            opponent.Add(masmonstr[0][0]);
            opponent.Add(masmonstr[2][0]);
            opponent.Add(masmonstr[2][1]);
            opponent.Add(masmonstr[2][2]);
            opponent.Add(masmonstr[2][3]);
            //
            for (int i = 0; i < putesh.Count; i++)
            {
                putesh[i].Grafics = new GrafContenir(putesh[i], i, this, true);
                putesh[i].CreatHero(tabControl2, this);

                ToolStripMenuItem[] tsmi = new ToolStripMenuItem[3];
                for (int j = 0; j < tsmi.Length; j++)
                {
                    tsmi[j] = new ToolStripMenuItem();
                    tsmi[j].Click += new System.EventHandler(contextMenuStrip_mas_Click);
                }
                tsmi[0].Text = "Сделка!";
                tsmi[1].Text = "Удалить";
                tsmi[2].Text = "Эффекты";

                masContMenStr.Add(new ContextMenuStrip());
                masContMenStr[i].Items.AddRange(tsmi);
                masContMenStr[i].Size = new System.Drawing.Size(153, 48);
                //masContMenStr[i].Click += new System.EventHandler(this.contextMenuStrip_mas_Click);
                
            }

            for (int i = 0; i < opponent.Count; i++)
            {
                opponent[i].Grafics = new GrafContenir(opponent[i], i, this, false);
            }

            сохранитьToolStripMenuItem.Enabled = true;
            загрузитьToolStripMenuItem.Enabled = false;

            string[] key = Memori.Formulas.Keys.ToArray();
            ToolStripMenuItem[] tlspitem = new ToolStripMenuItem[key.Length];
            for (int i = 0; i < key.Length; i++)
            {
                tlspitem[i] = new ToolStripMenuItem();
                tlspitem[i].Text = key[i];
                tlspitem[i].Click += new EventHandler(ToolStripMenuItem2_Click);
            }
            contextMenuStrip1.Items.AddRange(tlspitem);

            Market.Creat(putesh.Count, this, putesh, maspredmet, maseffet);
            Market.Show(tabPage5);
        }

        private void сохранитьToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            WriteRead.Save(putesh, maseffet, maspredmet);
        }

        private void геройToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Esense es = new Esense(Memori.modpers);
            Dictionary<string, string> dic = es.ImpactPar;
            Form3 f3 = new Form3(dic);
            f3.ShowDialog();
            es.ImpactPar = dic;
            es.CalcConnection();
            Human hm = new Human();
            putesh.Add(hm);
            hm.Hero(es, 0, false);
            hm.Grafics = new GrafContenir(hm, putesh.Count - 1, this, true);
            hm.CreatHero(tabControl2, this);

            //hero.Hero()
            //putesh.Add
        }

        private void предметToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap("cursors\\atac.png");
            this.Cursor = CursorHS.CreateCursorNoResize(bmp, 7, 3);

            string str = ((ToolStripMenuItem)sender).Text;
            formulas = Memori.Formulas[str];
            origin = EsenseMas().Find((Esense x) => x.Market);
        }

        public void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGridView dgv1 = (DataGridView)sender;
            if (dgv1.SelectedCells.Count > 0)
            {
                string columnHeader = dgv1.SelectedCells[0].OwningColumn.HeaderText;
                if (Memori.ItemVariable.ContainsKey(columnHeader))
                {
                    if (dgv1.SelectedCells[0].Value != null)
                    {
                        string value = dgv1.SelectedCells[0].Value.ToString();
                        SplitContainer unserp = (SplitContainer)dgv1.Parent.Parent;
                        SplitContainer unterp = (SplitContainer)unserp.Panel2.Controls[0];
                        int i = Memori.ItemVariable[columnHeader];
                        DataGridView dgv2 = i > 1 ? (DataGridView)unterp.Panel1.Controls[0] : (DataGridView)unterp.Panel2.Controls[0];
                        putesh[tabControl2.SelectedIndex].DisplayItem(i, value, dgv2);
                    }
                }
            }
        }

        private void ходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tik(putesh.Cast<Esense>().ToList());
            //Tik(masmonstr);
        }

        private void Tik(List<Esense> mases)
        {
            foreach (Esense item in mases)
            { item.Next(maseffet); }
        }

        private void contextMenuStrip_mas_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            ContextMenuStrip ms = (ContextMenuStrip)tsmi.GetCurrentParent();
            int j = masContMenStr.IndexOf(ms);

            int i = ms.Items.IndexOf(tsmi);
            switch (i)
            {
                case 0: // Сделка
                    Market.Add(putesh[j], j);
                    break;
                case 1:// Удалить
                    Market.Delete(j);
                    break;
                case 2:
                    bool bol = Market.Markt(masContMenStr.IndexOf(ms));
                    ms.Items[2].Text = bol ? "Эффекты" : "Магазин";
                    break;
            }
        }

        private void tabControl2_Click(object sender, EventArgs e)
        {
            int i = tabControl2.SelectedIndex;
            putesh[i].DisplayUpdate();
        }

        private new List<DataGridViewCell>[] SelectDataGridViev(object sender)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            ContextMenuStrip ms = (ContextMenuStrip)tsmi.GetCurrentParent();
            DataGridView dgv = (DataGridView)ms.SourceControl;
            DataGridViewSelectedCellCollection mascell = dgv.SelectedCells;
            List<DataGridViewCell> listcell = new List<DataGridViewCell>();
            foreach (DataGridViewCell item in mascell)
            {
                if (item.Value != null)
                { listcell.Add(item); }
            }
            listcell.RemoveAll((DataGridViewCell x) => x.Value.ToString().Replace(" ","") == "");
            List<DataGridViewCell> lpar = listcell.FindAll((DataGridViewCell x) => x.ColumnIndex == (int)HeroColumnsNumer.para);
            List<DataGridViewCell> lefft = listcell.FindAll((DataGridViewCell x) => x.ColumnIndex == (int)HeroColumnsNumer.effe);
            List<DataGridViewCell> leqip = listcell.FindAll((DataGridViewCell x) => x.ColumnIndex == (int)HeroColumnsNumer.eqip);
            List<DataGridViewCell> linvn = listcell.FindAll((DataGridViewCell x) => x.ColumnIndex == (int)HeroColumnsNumer.inve);
            return new List<DataGridViewCell>[] { lpar, lefft, leqip, linvn };
        }

        private List<int>[] SDGVnumer(object sender)
        {
            List<DataGridViewCell>[] arr = SelectDataGridViev(sender);
            List<int>[] mi = new List<int>[arr.Length];
            for (int i = 0; i < mi.Length; i++)
            {
                mi[i] = new List<int>();
                foreach (DataGridViewCell item in arr[i])
                { mi[i].Add(item.RowIndex); }
            }
            return mi;
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int>[] delnom = SDGVnumer(sender);
            int j = tabControl2.SelectedIndex;
            putesh[j].RemoveEffekt(delnom[(int)HeroColumnsNumer.effe]);
            putesh[j].RemovePredmet(delnom[(int)HeroColumnsNumer.eqip], Humanoid.whither.eqip);
            putesh[j].RemovePredmet(delnom[(int)HeroColumnsNumer.inve], Humanoid.whither.inven);
        }

        private void переместитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int>[] delnom = SDGVnumer(sender);
            int j = tabControl2.SelectedIndex;
            putesh[j].DisplaseEqipInven(delnom[(int)HeroColumnsNumer.eqip]);
            putesh[j].DisplaseInvenEqip(delnom[(int)HeroColumnsNumer.inve]);
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int j = tabControl2.SelectedIndex;
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            ContextMenuStrip ms = (ContextMenuStrip)tsmi.GetCurrentParent();
            DataGridView dgv = (DataGridView)ms.SourceControl;
            string[] key = Memori.ModelTable.TableHero.Keys.ToArray();
            dgv.SelectAll();
            DataGridViewSelectedCellCollection arrcell = dgv.SelectedCells;
            dgv.ClearSelection();
            List<DataGridViewCell> lpar = new List<DataGridViewCell>();
            foreach (DataGridViewCell item in arrcell)
            {
                if (item.ColumnIndex == (int)HeroColumnsNumer.para && item.Value != "")
                { lpar.Add(item); }
            }
            int max = key.Length;
            for (int i = 0; i < max; i++)
            {
                putesh[j].PersonParm(key[i], lpar[max - 1 - i].Value.ToString());
            }
            putesh[j].CalcConnection();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            foreach (Human put in putesh)
            { put.AutoFont(); }
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {
            foreach (Esense op in opponent)
            { op.AutoFont(); }
        }
    }
}

//private void button1_Click(object sender, EventArgs e)
//{
//Thread thread1 = new Thread(mythread1);

//thread1.Start();
//}

//void mythread1()
//{
//this.textBox1.BeginInvoke((MethodInvoker)(() => this.textBox1.Text = “text”));
//}

//На WPF используйте такое выражение:

//Dispatcher.BeginInvoke((Action)(() => this.textBox1.Text = “text”));
//количество процессоров можно получить через свойство Environment.ProcessorCount

//Highest - самый высокий
//AboveNormal - выше среднего
//Normal - стандартный
//BelowNormal - ниже среднего
//Lowest - самый низкий

//Инструмент манипуляции заднего фона tabControls
//RectangleF tf = new RectangleF(-5,-5, tab.Size.Width+5, tab.Size.Height+5);
//            Brush b = parent_backcolor; //Цвет
 
//            e.Graphics.FillRectangle(b, tf);
//C:\Server\bin\Apache24\htdocs