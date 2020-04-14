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
using System.Xml;

namespace DnD_2._0._0
{
    class SlotInfo
    {
        private int i = 0;

        public int quant { get; private set; }
        private int mast;
        public int Mast
        {
            get { return mast; }
        }
        public string flow;

        private Predmet[] SM;

        public SlotInfo(string quant)
        {
            this.quant = int.Parse(quant);
            mast = this.quant;
            SM = new Predmet[mast];
            flow = "Nat";
        }

        public SlotInfo(string quant, string mast, string flow)
            : this(quant)
        {
            this.mast = int.Parse(mast);
            SM = new Predmet[this.mast];
            this.flow = flow;
        }

        public bool WhatSlot(Predmet pred, out string flow)
        {
            flow = this.flow;
            if (SM.Contains(pred))
            { return true; }
            return false;
        }

        public void Add(Predmet pred, bool msdef)
        {
            int consl = int.Parse(pred.sltcount);
            if (msdef && mast > 0)
            {
                mast--;
                quant--;
                consl--;
                SM[i] = pred;
                i++;
            }
            quant -= consl;
        }

        public void Remove(Predmet pred)
        {
            int consl = int.Parse(pred.sltcount);
            if (SM.Contains(pred))
            {
                mast++;
                quant++;
                consl--;
                int n = Array.IndexOf(SM, pred);
                int l = Array.FindLastIndex(SM, (Predmet x) => x != null);
                if (n != l)
                {
                    Predmet p = SM[n];
                    SM[n] = SM[l];
                    SM[l] = p;
                }
                SM[l] = null;
                i = Array.FindIndex(SM, (Predmet x) => x == null);
            }
            quant += consl;
        }

        public SlotInfo Copi()
        { return new SlotInfo(quant.ToString(), mast.ToString(), flow); }
    }

    static class FormulParse
    {
        public static string ParsF(string str)
        {
            if (!str.Contains('$'))
            {
                //
                Type scriptType = Type.GetTypeFromCLSID(Guid.Parse("0E59F1D5-1FBE-11D0-8FF2-00A0D10038BC"));
                dynamic obj = Activator.CreateInstance(scriptType, false);
                obj.Language = "javascript";
                // позволяет выполнять код написанный в виде строки синтаксис JS, obj.Eval
                var s = obj.Eval(str);
                string ss;
                try
                {
                    ss = s;
                }
                catch { ss = s.ToString(); }
                return ss;
            }
            else
            {
                str = str.Replace("$", "");
                str = str.Replace("]", "");
                string[] nameVal = str.Split('[');
                return Memori.Tables[nameVal[0]][nameVal[1]];
            }
        }

        public static void ParsFullCh(string str)
        {
            if (str.Contains('@'))
            {
                //string farm = Rege
                // посмотреть регулярные выражения с#
            }
        }

        public static string ReplaceStrVal(string str, string name, string[] key, string[] value)
        {
            name += ".";
            for (int i = 0; i < key.Length; i++)
            { str = str.Replace(name + key[i], value[i]); }
            return str;
        }
    }

    static class Market
    {
        static List<DataGridView[]> rows;
        static List<DataGridView> tabl;
        static List<Predmet>[] masprv;
        static List<Effekt>[] masefv;
        static List<Human> mput;
        static List<List<Predmet>> pulpred;
        static List<List<Effekt>> pulefft;
        static List<int> num = new List<int>();
        static List<int> sumpris = new List<int>();
        static List<bool> market = new List<bool>();

        static string money;

        static string[] pred = new string[] { "Покупка", "Продажа" };
        static string[] efft = new string[] { "Добавить", "Удалить" };
        //ooooooooooooooo
        //public static int NumerTable(DataGridView dgv)
        //{
        //    int i = -1;
        //    i = tabl.IndexOf(dgv);
        //    return i;
        //}
        //oooooooooooooo
        private static void Clear(int j)
        {
            rows[j][1].Rows[0].Cells[0].Value = null;
            rows[j][2].Rows[0].Cells[0].Value = null;
            for (int i = tabl[j].Rows.Count - 1; i > 0; i--)
            { tabl[j].Rows.RemoveAt(i); }
            num[j] = 0;
            sumpris[j] = 0;
            pulpred[j].Clear();
        }

        public static bool Markt(int i)
        {
            market[i] = !market[i];
            Clear(i);
            if (market[i])
            { MKT(i, pred, Memori.VidPredmet); }
            else { MKT(i, efft, Memori.VidEffekt); }
            return market[i];
        }

        private static void MKT(int i, string[] masstr, List<string> masvid)
        {
            DataGridViewComboBoxCell comcell = (DataGridViewComboBoxCell)rows[i][0].Rows[0].Cells[1];
            comcell.Items.Clear();
            comcell.Items.AddRange(masstr);
            comcell = (DataGridViewComboBoxCell)rows[i][1].Rows[0].Cells[0];
            comcell.Items.Clear();
            comcell.Items.AddRange(masvid.ToArray());
        }

        public static void Show(TabPage parent)
        {
            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < rows[i].Length; j++)
                { parent.Controls.Add(rows[i][j]); }
                parent.Controls.Add(tabl[i]);

                rows[i][0].Rows[0].Cells[0].Value = mput[i].PersonParm("Name");
                Markt(i);
                rows[i][1].CellValueChanged += Market_CellValueChanged1;
                rows[i][2].CellValueChanged += Market_CellValueChanged2;
            }
        }

        private static void Market_CellValueChanged1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                DataGridView dgv = (DataGridView)sender;
                if (dgv.Rows[0].Cells[0].Value != null)
                {
                    string str = dgv.Rows[0].Cells[0].Value.ToString();
                    int i = rows.FindIndex((DataGridView[] x) => x[1] == dgv);

                    if (market[i])
                    { MCVC1(i, masprv[Memori.VidPredmet.FindIndex((string x) => x == str)].Cast<ItemCore>().ToList()); }
                    else { MCVC1(i, masefv[Memori.VidEffekt.FindIndex((string x) => x == str)].Cast<ItemCore>().ToList()); }
                }
            }
        }

        private static void MCVC1(int i, List<ItemCore> mas)
        {
            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)rows[i][2].Rows[0].Cells[0];
            cell.Items.Clear();
            cell.Items.AddRange(ItemCore.MasToString(mas).ToArray());
        }

        private static void Market_CellValueChanged2(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.Rows[0].Cells[0].Value != null)
            {
                int i = rows.FindIndex((DataGridView[] x) => x[2] == dgv);
                string str = dgv.Rows[0].Cells[0].Value.ToString();
                if (market[i])
                {
                    string vid = rows[i][1].Rows[0].Cells[0].Value.ToString();
                    int l = Memori.VidPredmet.FindIndex((string x) => x == vid);
                    pulpred[i].Add(masprv[l].Find((Predmet x) => x.name == str));
                }
                else
                {
                    string vid = rows[i][1].Rows[0].Cells[0].Value.ToString();
                    int l = Memori.VidEffekt.FindIndex((string x) => x == vid);
                    pulefft[i].Add(masefv[l].Find((Effekt x) => x.name == str));
                }
                tabl[i].Rows.Add();
                int j = tabl[i].RowCount - 1;
                num[i]++;
                tabl[i].Rows[j].Cells[0].Value = num[i].ToString();
                tabl[i].Rows[j].Cells[1].Value = str;
                tabl[i].Rows[j].Cells[2].Value = "50";
                tabl[i].Rows[j].Cells[3].Value = rows[i][1].Rows[0].Cells[1].Value;
                sumpris[i] += int.Parse(tabl[i].Rows[j].Cells[2].Value.ToString());
                tabl[i].Rows[0].Cells[2].Value = sumpris[i].ToString();
            }
        }

        public static void Creat(int n, Form1 f1, List<Human> puts, List<Predmet>[] maspr, List<Effekt>[] masef)
        {
            masprv = maspr;
            masefv = masef;
            mput = puts;

            pulefft = new List<List<Effekt>>();
            pulpred = new List<List<Predmet>>();

            rows = new List<DataGridView[]>();
            tabl = new List<DataGridView>();

            for (int i = 0; i < n; i++)
            {
                market.Add(false);

                pulefft.Add(new List<Effekt>());
                pulpred.Add(new List<Predmet>());

                rows.Add(new DataGridView[3]);
                tabl.Add(new DataGridView());

                for (int j = 0; j < 2; j++)
                {
                    rows[i][j] = InizialElement(rows[i][j], false);
                    rows[i][j].ColumnCount = 2;
                }
                rows[i][2] = InizialElement(rows[i][2], false);
                rows[i][2].ColumnCount = 1;
                rows[i][2].Columns[0].Width = 225;
                foreach (DataGridView item in rows[i])
                { item.RowCount = 1; }
                rows[i][0].Rows[0].Cells[1] = new DataGridViewComboBoxCell();
                rows[i][1].Rows[0].Cells[0] = new DataGridViewComboBoxCell();
                rows[i][1].Rows[0].Cells[1].Value = "100";
                rows[i][2].Rows[0].Cells[0] = new DataGridViewComboBoxCell();
                for (int j = 0; j < rows[i].Length; j++)
                {
                    rows[i][j].Location = new Point(228 * i, 25 * j);
                    InizialProp(rows[i][j]);
                }
            }

            for (int i = 0; i < tabl.Count; i++)
            {
                tabl[i] = InizialElement(tabl[i], true);
                tabl[i].Location = new Point(228 * i, 75);
                tabl[i].ColumnCount = 4; 
                tabl[i].Columns[0].HeaderText = "№";
                tabl[i].Columns[0].Width = 30;
                tabl[i].Columns[1].HeaderText = "Название";
                tabl[i].Columns[1].Width = 105;
                tabl[i].Columns[2].HeaderText = "Цена";
                tabl[i].Columns[2].Width = 45;
                tabl[i].Columns[3].HeaderText = "%";
                tabl[i].Columns[3].Width = 45;
                tabl[i].Rows.Add();
                tabl[i].Rows[0].Cells[0].Value = "0";
                num.Add(0);
                sumpris.Add(0);
                tabl[i].Rows[0].Cells[1].Value = "Сумма:";
                tabl[i].Rows[0].Cells[2].Value = "0";
                //tabl[i].Rows[0].Cells[3].Value = "0";

                InizialProp(tabl[i]);
                tabl[i].ContextMenuStrip = f1.masContMenStr[i];
            }
        }

        private static DataGridView InizialElement(DataGridView tab, bool head)
        {
            tab = new DataGridView();
            tab.RowHeadersVisible = false;
            tab.ColumnHeadersVisible = head;
            tab.AllowUserToAddRows = false;

            tab.Size = new Size(228, head ? 500 : 25);
            return tab;
        }

        private static void InizialProp(DataGridView tab)
        {
            tab.AllowUserToResizeColumns = false;
            tab.AllowUserToResizeRows = false;
            tab.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            for (int j = 0; j < tab.ColumnCount; j++)
            {
                tab.Columns[j].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tab.Columns[j].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        public static void Delete(int i)
        {
            ///////
        }

        public static void Add(Human put, int i)
        {
            if (market[i])
            {
                put.AddPredmet(pulpred[i], Humanoid.whither.inven);
                Clear(i);
            }

        }
    }
}
