using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

namespace DnD_2._0._0
{
    public delegate void SelectedTabPageChangeEventHandler(Object sender, TabPageChangeEventArgs e);

    public class MyTab : TabControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public MyTab()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

        }


        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }


        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion

        #region Interop

        [StructLayout(LayoutKind.Sequential)]
            private struct NMHDR
        {
            public IntPtr HWND;
            public uint idFrom;
            public int code;
            public override String ToString()
            {
                return String.Format("Hwnd: {0}, ControlID: {1}, Code: {2}", HWND, idFrom, code);
            }
        }
        
        private const int TCN_FIRST = 0 - 550;                 
        private const int TCN_SELCHANGING = (TCN_FIRST - 2);
        
        private const int WM_USER = 0x400;
        private const int WM_NOTIFY = 0x4E;
        private const int WM_REFLECT = WM_USER + 0x1C00;
        
        #endregion

        #region BackColor Manipulation

        //As well as exposing the property to the Designer we want it to behave just like any other 
        //controls BackColor property so we need some clever manipulation.

        private Color m_Backcolor = Color.Empty;
        [Browsable(true),Description("The background color used to display text and graphics in a control.")]
        public override Color BackColor
        {
            get
            {
                if (m_Backcolor.Equals(Color.Empty))
                {
                    if (Parent == null)
                        return Control.DefaultBackColor;
                    else
                        return Parent.BackColor;
                }
                return m_Backcolor;
            }
            set
            {
                if (m_Backcolor.Equals(value)) return;
                m_Backcolor = value;
                Invalidate();
                //Let the Tabpages know that the backcolor has changed.
                base.OnBackColorChanged(EventArgs.Empty);
            }
        }
        public bool ShouldSerializeBackColor()
        {
            return !m_Backcolor.Equals(Color.Empty);
        }
        public override void ResetBackColor()
        {
            m_Backcolor = Color.Empty;
            Invalidate();
        }

        #endregion

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            base.OnParentBackColorChanged (e);
            Invalidate();
        }


        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged (e);
            Invalidate();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint (e);
            e.Graphics.Clear(Color.DarkOrange);
            Rectangle r = ClientRectangle;
            r = new Rectangle(r.X, r.Y, r.Width, GetTabRect(0).Height + 2);
            e.Graphics.FillRectangle(Brushes.OrangeRed, r);
            if (TabCount <= 0) return;
            //Draw a custom background for Transparent TabPages
            //r = SelectedTab.Bounds;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            //Font DrawFont = new Font(Font.FontFamily, 24, FontStyle.Regular, GraphicsUnit.Pixel);
            //e.Graphics.DrawString(tab.Controls[i].Text, new Font("Microsoft Sans Serif", 8.25f), Brushes.Black, new Rectangle(pnt, rect.Size));
            //ControlPaint.DrawStringDisabled(e.Graphics, "Micks Ownerdraw TabControl", DrawFont, BackColor, (RectangleF)r, sf);
            //DrawFont.Dispose();
            //Draw a border around TabPage
            
            //Draw the Tabs
            TabPage tp;
            LinearGradientBrush bruh = new LinearGradientBrush(new Rectangle(0, 0, 12, 12), Color.Tomato, Color.OrangeRed, LinearGradientMode.Vertical);
            for (int index = 0; index <= TabCount - 1; index++)
            {
                tp = TabPages[index];
                r = GetTabRect(index);
                r = new Rectangle(r.X + 2, r.Y + 2, r.Size.Width - 4, r.Size.Height - 3);
                e.Graphics.FillRectangle(bruh, r);
                r = new Rectangle(r.X - 1, r.Y - 1, r.Size.Width + 1, r.Size.Height + 1);
                e.Graphics.DrawRectangle(Pens.Aqua, r);
                r = new Rectangle(r.X - 1, r.Y - 1, r.Size.Width + 2, r.Size.Height + 1);
                e.Graphics.DrawRectangle(Pens.Silver, r);

                //Set up rotation for left and right aligned tabs
                if (Alignment == TabAlignment.Left || Alignment == TabAlignment.Right)
                {
                    float RotateAngle = 90;
                    if (Alignment == TabAlignment.Left) RotateAngle = 270;
                    PointF cp = new PointF(r.Left + (r.Width >> 1), r.Top + (r.Height >> 1));
                    e.Graphics.TranslateTransform(cp.X, cp.Y);
                    e.Graphics.RotateTransform(RotateAngle);
                    r = new Rectangle(-(r.Height >> 1), -(r.Width >> 1), r.Height, r.Width);
                }
                //Draw the Tab Text
                if (tp.Enabled)
                    e.Graphics.DrawString(tp.Text, Font, Brushes.Black, (RectangleF)r, sf);
                else
                    ControlPaint.DrawStringDisabled(e.Graphics, tp.Text, Font, Color.Black, (RectangleF)r, sf);
                
                e.Graphics.ResetTransform();
            }
            bruh.Dispose();
            tp = TabPages[SelectedIndex];
            r = GetTabRect(SelectedIndex);
            r = new Rectangle(r.X - 2, r.Y - 2, r.Size.Width + 3, r.Size.Height + 1);
            e.Graphics.DrawRectangle(Pens.Silver, r);
            r = new Rectangle(r.X + 1, r.Y + 1, r.Size.Width - 1, r.Size.Height);
            e.Graphics.FillRectangle(Brushes.DarkOrange, r);
            r = GetTabRect(SelectedIndex);
            e.Graphics.DrawString(tp.Text, Font, Brushes.White, (RectangleF)r, sf);
            //время
            r = new Rectangle(Bounds.Width - 100, 0, 100, 21);
            e.Graphics.DrawString("26.06.17 12:16", Font, Brushes.Black, (RectangleF)r, sf);
        }


        [Description("Occurs as a tab is being changed.")]
        public event SelectedTabPageChangeEventHandler SelectedIndexChanging;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (WM_REFLECT + WM_NOTIFY))
            {
                NMHDR hdr = (NMHDR)(Marshal.PtrToStructure(m.LParam, typeof(NMHDR)));
                if (hdr.code == TCN_SELCHANGING)
                {
                    TabPage tp = TestTab(PointToClient(Cursor.Position));
                    if (tp != null)
                    {
                        TabPageChangeEventArgs e = new TabPageChangeEventArgs(SelectedTab, tp);
                        if (SelectedIndexChanging != null)
                            SelectedIndexChanging(this, e);
                        if (e.Cancel || tp.Enabled == false)
                        {
                            m.Result = new IntPtr(1);
                            return;
                        }
                    }
                }
            }
            base.WndProc (ref m);
        }


        private TabPage TestTab(Point pt)
        {
            for (int index = 0; index <= TabCount - 1; index++)
            {
                if (GetTabRect(index).Contains(pt.X, pt.Y))
                    return TabPages[index];
            }
            return null;
        }
        
    }


    public class TabPageChangeEventArgs : EventArgs
    {
        private TabPage _Selected = null;
        private TabPage _PreSelected = null;
        public bool Cancel = false;

        public TabPage CurrentTab
        {
            get
            {
                return _Selected;
            }
        }


        public  TabPage NextTab
        {
            get
            {
                return _PreSelected;
            }
        }


        public TabPageChangeEventArgs(TabPage CurrentTab, TabPage NextTab)
        {
            _Selected = CurrentTab;
            _PreSelected = NextTab;
        }
    }

    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////
    /// </summary>

    partial class Kubic
    {
        public int n;
        public string v;
        public int m;

        public Kubic(int kolva, string vid, int modifik)
        {
            this.n = kolva;
            this.v = vid;
            this.m = modifik;
        }

        public Kubic(string s)
        {
            this.m = 0;
            this.n = int.Parse(s.Substring(0, s.IndexOf('Д')));
            if (s.Any<char>((char ch) => ch == '+'))
            { this.v = s.Substring(s.IndexOf('Д'), s.IndexOf('+') - s.IndexOf('Д')); this.m = int.Parse(s.Substring(s.IndexOf('+') + 1)); }
            else { this.v = s.Substring(s.IndexOf('Д')); }
        }

        public static void Redukt(List<Kubic> mas)
        {
            List<List<Kubic>> lm = new List<List<Kubic>>();
            while (mas.Count > 0)
            {
                lm.Add(new List<Kubic>());
                lm[lm.Count - 1] = mas.FindAll((Kubic x) => x.v == mas[0].v);
                mas.RemoveAll((Kubic x) => x.v == lm[lm.Count - 1][0].v);
            }
            foreach (List<Kubic> m in lm)
            {
                int i = 0;
                int j = 0;
                foreach (Kubic k in m)
                {
                    i += k.n;
                    j += k.m;
                }
                mas.Add(new Kubic(i, m[0].v, j));
            }
        }
    }

    static partial class Chasi
    {
        private static int chasi = 0;
        private static int minuti = 0;

        public static string Convert(int hod)
        {
            for (int i = 0; i < hod; i++)
            { minuti += 2; }
            while (minuti > 59)
            { minuti -= 60; chasi++; }
            if (chasi > 23)
            { chasi -= 24; }
            return Format(chasi, minuti);
        }

        public static string Time0(string time)
        {
            time = time.Replace(" ", "");
            string[] ch_min = time.Split(':');
            chasi = int.Parse(ch_min[0]);
            minuti = int.Parse(ch_min[1]);
            return Format(chasi, minuti);
        }

        private static string Format(int chasi, int minuti)
        {
            string ch = chasi.ToString();
            string mi = minuti.ToString();
            if (ch.Length < 2)
            { ch = "0" + ch; }
            if (mi.Length < 2)
            { mi = "0" + mi; }
            return ch + " : " + mi;
        }
    }

    //====================================================================================
    class GrafContenir
    {
        public GroupBox contenir;
        private Dictionary<string, Label> param;
        private List<PictureBox> image;
        private Dictionary<string, string> LabelText;

        private string maxSizeName;
        float[] fontstand = new float[] { 8.25F, 9.75F, 12F, 14.25F, 15.75F };

        public GrafContenir(Esense put, int n, Form1 f1, bool hero)
        {
            contenir = new GroupBox();
            FlowLayoutPanel flp = new FlowLayoutPanel();
            param = new Dictionary<string, Label>();
            image = new List<PictureBox>();
            string[] key = Memori.Labeltext.Keys.ToArray<string>();

            for (int i = 0; i < key.Length; i++)
            {
                Label label = new Label();
                label.AutoSize = true;
                label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));

                label.Text = pUpdata(put, key[i]);
                label.Click += new EventHandler(f1.GroupBox_Click);
                label.ContextMenuStrip = f1.contextMenuStrip1;
                flp.Controls.Add(label);
                param.Add(key[i], label);
            }

            for (int i = 0; i < 3; i++)
            {
                image.Add(new PictureBox());
                image[i].Size = new System.Drawing.Size(22, 22);
                image[i].Location = new System.Drawing.Point(6 + 30 * i, 135);
                image[i].Click += new EventHandler(f1.GroupBox_Click);
                image[i].Visible = false;
                contenir.Controls.Add(image[i]);
            }

            flp.Dock = DockStyle.Fill;
            flp.AutoScroll = true;
            flp.AutoSizeMode = AutoSizeMode.GrowOnly;
            flp.FlowDirection = FlowDirection.TopDown;
            flp.WrapContents = false;
            contenir.Controls.Add(flp);

            int h = 6;
            //contenir.Location = new System.Drawing.Point(6 + 101 * n, h); // Size.X = 95 + 6(отступ) = 101
            contenir.Dock = DockStyle.Fill;
            contenir.TabStop = false;
            contenir.Text = put.PersonParm("Name");
            contenir.BackColor = Color.White;
            contenir.ContextMenuStrip = f1.contextMenuStrip1;
            contenir.Click += new EventHandler(f1.GroupBox_Click);
            TableLayoutPanel pan = hero ? f1.tableLayoutPanel1 : f1.tableLayoutPanel2;
            int k = hero ? 1 : 2;
            if (pan.Controls.Count == pan.ColumnCount * k)
            {
                pan.ColumnCount++;
                pan.MinimumSize = new Size(pan.MinimumSize.Width + 132, pan.MinimumSize.Height);
                if (pan.MinimumSize.Width > f1.splitContainer3.Size.Width)
                {
                    pan.Dock = DockStyle.None;
                    pan.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
                }
                pan.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            }
            pan.Controls.Add(contenir);

            string[] lkey = param.Keys.ToArray();
            Label max = param[lkey[0]];
            maxSizeName = lkey[0];
            for (int i = 1; i < lkey.Length; i++)
            {
                if (param[key[i]].Size.Width > max.Size.Width)
                {
                    max = param[key[i]];
                    maxSizeName = lkey[i];
                }
            }
        }

        private string pUpdata(Esense put, string key)
        {
            string[] labText;
            if (key.Contains(','))
            { labText = key.Split(','); }
            else { labText = new string[] { key }; }
            string lt = Memori.Labeltext[key];
            foreach (string s in labText)
            { lt = lt.Replace("[" + s + "]", put.PersonParm(s)); }
            //lt = FormulParse.ParsF(lt);
            return lt;
        }

        public void Updata(Esense put, string key)
        {
            string[] keys = Memori.Labeltext.Keys.ToArray();
            foreach (string k in keys)
            {
                if (k.Contains(key))
                {
                    param[k].Text = pUpdata(put, k);
                    break;
                }
            }
        }

        public void AutoFont()
        {
            ((FlowLayoutPanel)contenir.Controls[3]).FlowDirection = FlowDirection.LeftToRight;
            Label lab = param[maxSizeName];
            if (lab.Size.Width < contenir.Size.Width)
            {
                int i = fontstand.ToList().FindIndex((float x) => x == lab.Font.Size) + 1;
                bool bl = true;
                for (; i < fontstand.Length; i++)
                {
                    lab.Font = new Font(lab.Font.Name, fontstand[i], lab.Font.Style, lab.Font.Unit, lab.Font.GdiCharSet);
                    if (lab.Size.Width > contenir.Size.Width)//=
                    {
                        i--;
                        bl = false;
                        break;
                    }
                }
                if (bl)
                { i--; }
                lab.Font = new Font(lab.Font.Name, fontstand[i], lab.Font.Style, lab.Font.Unit, lab.Font.GdiCharSet);
            }
            else if (lab.Size.Width > contenir.Size.Width)
            {
                int i = fontstand.ToList().FindIndex((float x) => x == lab.Font.Size) - 1;
                for (; i >= 0; i--)
                {
                    lab.Font = new Font(lab.Font.Name, fontstand[i], lab.Font.Style, lab.Font.Unit, lab.Font.GdiCharSet);
                    if (lab.Size.Width < contenir.Size.Width)//=
                    {
                        break;
                    }
                }
            }
            ((FlowLayoutPanel)contenir.Controls[3]).FlowDirection = FlowDirection.TopDown;

            Label[] ml = param.Values.ToArray();
            foreach (Label item in ml)
            { item.Font = new Font(lab.Font.Name, lab.Font.Size, lab.Font.Style, lab.Font.Unit, lab.Font.GdiCharSet); }
            contenir.Font = new Font(lab.Font.Name, lab.Font.Size, lab.Font.Style, lab.Font.Unit, lab.Font.GdiCharSet);
            contenir.Controls[3].Font = new Font(lab.Font.Name, lab.Font.Size, lab.Font.Style, lab.Font.Unit, lab.Font.GdiCharSet);
        }

        //public void SetParam(string p, int key)
        //{ param[key].Text = p; }

        //public void SetParam(string p, string key)
        //{
        //    for (int i = 0; i < param.Count; i++)
        //    {
        //        if (
        //    }
        //}
    }

    interface CoreC
    {
        void Save(List<string>[] str);
    }

    class Esense
    {
        private struct TablEf
        {
            public string value;
            public string limit;
            public string nameff;
            int st;
            public bool del;

            public TablEf(string name, string value, string nameff, string max)
            {
                this.nameff = nameff;
                this.limit = max;
                st = -1;
                del = true;
                if (limit != null)
                {
                    if (value.Contains('%'))
                    { this.value = "[" + name + "]*100" + "/" + "[" + max + "]" + value.Replace("%", ""); }
                    else { this.value = "[" + name + "]" + value; }
                }
                else { this.value = "[" + name + "]" + value; }
            }

            public TablEf(string name, string value, string nameff, string max, string styp)
                : this(name, value, nameff, max)
            {
                if (!int.TryParse(styp, out st))
                { st = Memori.VidEffekt.IndexOf(styp); }
            }

            public TablEf(string name, string value, string nameff, string max, string styp, string bol)
                : this(name, value, nameff, max, styp)
            { del = bool.Parse(bol); }
        }

        public string[] GetParamrskey()
        { return paramrs.Keys.ToArray(); }

        public void SetParamrsvalue(List<string> value)
        {
            string[] mkey = paramrs.Keys.ToArray();
            for (int i = 0; i < mkey.Length; i++)
            { paramrs[mkey[i]] = value[i]; }
        }

        public string PersonParm(string key)
        {
            return paramrs[key];
        }

        public void PersonParm(string key, string value)
        {
            int val = 0;
            int max = 1;
            if (parMaxRect.ContainsKey(key))
            {
                max = int.Parse(paramrs[parMaxRect[key]]);
                val = int.Parse(value);
            }
            if (max >= val)
            { paramrs[key] = value; }
            else { paramrs[key] = max.ToString(); }
            Grafics.Updata(this, key);
            //this.Display(Grafics. Udata DataGriedViev
        }

        public Dictionary<string, string> paramrs;
        public Dictionary<string, string> baseparms;

        protected Dictionary<string, string[]> connectpars;
        protected Dictionary<string, string> parsformcon;
        protected Dictionary<string, string> parMaxRect;
        private Dictionary<string, List<TablEf>> autoeffect;

        public Dictionary<string, string> armor;
        public List<List<Tostr>> damage;

        public bool ParamVal(string key, out string value)
        {
            if (paramrs.ContainsKey(key))
            {
                value = paramrs[key];
                return true;
            }
            value = "";
            return false;
        }

        public int deystvij;
        private bool market = false;
        public bool Market
        {
            get { return market; }
            set
            {
                market = !market;
                Grafics.contenir.BackColor = market ? Color.Red : Color.White;
            }
        }
        public bool weit = false;

        public GrafContenir Grafics { protected get; set; }

        public void AutoFont()
        { Grafics.AutoFont(); }

        public List<Effekt> efekt;

        public Esense()
        {
            paramrs = new Dictionary<string, string>();
            deystvij = 2;
            efekt = new List<Effekt>();

            connectpars = new Dictionary<string, string[]>();
            parsformcon = new Dictionary<string, string>();
            parMaxRect = new Dictionary<string, string>();
            autoeffect = new Dictionary<string, List<TablEf>>();

            armor = new Dictionary<string, string>();
            //damage = new List<Tostr>();
        }

        public Esense(Esense model)
        {
            deystvij = 2;
            efekt = new List<Effekt>();

            paramrs = new Dictionary<string, string>(model.paramrs);
            connectpars = new Dictionary<string, string[]>(model.connectpars);
            parsformcon = new Dictionary<string, string>(model.parsformcon);
            parMaxRect = new Dictionary<string, string>(model.parMaxRect);
            autoeffect = new Dictionary<string, List<TablEf>>(model.autoeffect);

            armor = new Dictionary<string, string>(model.armor);
        }

        public Esense(XmlNode xmlparam, XmlNode xmlTable)
            : this()
        {
            XmlNode attribut;
            foreach (XmlNode item in xmlparam)
            {
                paramrs.Add(item.InnerText, null);
                attribut = item.Attributes.GetNamedItem("Impact");
                if (attribut != null)
                {
                    string str = attribut.Value.Replace(" ", "");
                    string[] mstr = str.Split(',');
                    connectpars.Add(item.InnerText, mstr);
                }
                else
                {
                    attribut = item.Attributes.GetNamedItem("Formul");
                    if (attribut != null)
                    { parsformcon.Add(item.InnerText, attribut.Value); }
                }
                attribut = item.Attributes.GetNamedItem("Max");
                if (attribut != null)
                { parMaxRect.Add(item.InnerText, attribut.Value); }
            }
            foreach (XmlNode iTable in xmlTable)
            {
                List<TablEf> teff = new List<TablEf>();
                foreach (XmlNode item in iTable)
                {
                    attribut = item.Attributes.GetNamedItem("SType");
                    TablEf te;
                    if (attribut != null)
                    { te = new TablEf(iTable.Attributes[0].Value, item.Attributes["Value"].Value, item.Attributes["Effect"].Value, iTable.Attributes["Max"].Value, attribut.Value); }
                    else { te = new TablEf(iTable.Attributes[0].Value, item.Attributes["Value"].Value, item.Attributes["Effect"].Value, iTable.Attributes["Max"].Value); }
                    attribut = item.Attributes.GetNamedItem("Delete");
                    if (attribut != null)
                    { te.del = bool.Parse(attribut.Value); }
                    teff.Add(te);
                }
                autoeffect.Add(iTable.Attributes[0].Value, teff);
            }
        }

        public Esense(Esense es, XmlNode xmlmeff)
            : this()
        {
            this.paramrs = new Dictionary<string, string>(es.paramrs);
            baseparms = new Dictionary<string, string>(es.paramrs);
        }

        public Esense(Esense es, XmlNode xmlmeff, List<Effekt>[] meff)
            : this(es, xmlmeff)
        {
            if (xmlmeff != null)
            {
                foreach (XmlNode eff in xmlmeff)
                {
                    Effekt ef = new Effekt();
                    if (eff.InnerText != "")
                    {
                        for (int i = 0; i < meff.Length; i++)
                        {
                            ef = meff[i].Find((Effekt x) => x.name == eff.InnerText);
                            if (ef != null)
                            { break; }
                        }
                    }
                    else { ef = WriteRead.ReadEffekt(eff); }
                    AddEffekt(ef);
                }
            }
        }

        public void AddEffekt (Effekt effekt)
        {
            efekt.Add(effekt.Copi());
            if (!effekt.tikaet)
            { effekt.Allow(this, true); }
        }

        public void AddEffekt(List<Effekt> maseffekt, int n)
        {
            List<Effekt> me = new List<Effekt>();
            foreach (Effekt item in maseffekt)
            { me.Add(item.Copi()); }
            efekt.InsertRange(n, me);
            foreach (Effekt effekt in me)
            {
                if (!effekt.tikaet)
                { effekt.Allow(this, true); }
            }
        }

        public void AddEffekt(List<Effekt> maseffekt)
        {
            foreach (Effekt item in maseffekt)
            { AddEffekt(item); }
        }

        public virtual DataGridView Display(DataGridView dgv)
        {
            string name;
            if (!Memori.ModelTable.NameHeader(0, out name))
            {
                return dgv;
            }
            dgv.TopLeftHeaderCell.Value = name;
            for (int i = 0; i < 2; i++)
            { HeaderFill(dgv); }

            dgv.RowCount = Memori.ModelTable.TableHero.Count;
            string[] names = Memori.ModelTable.TableHero.Values.ToArray();
            string[] keys = Memori.ModelTable.TableHero.Keys.ToArray();
            Dictionary<string, string> allvalue = new Dictionary<string, string>(paramrs);

            for (int i = 0; i < dgv.RowCount; i++)
            {
                dgv.Rows[i].HeaderCell.Value = names[i];
                dgv.Rows[i].Cells[0].Value = allvalue[keys[i]];
            }

            if (dgv.ColumnCount > 1)
            { ColumnFill(dgv, efekt.Cast<ItemCore>().ToList()); }
            return dgv;
        }

        protected void ColumnFill(DataGridView dgv, List<ItemCore> item)
        {
            int c = dgv.ColumnCount - 1;
            List<string> itemnames = ItemCore.MasToString(item);
            for (int i = 0; i < itemnames.Count; i++)
            { dgv.Rows[i].Cells[c].Value = itemnames[i]; }
        }

        protected void HeaderFill(DataGridView dgv)
        {
            string name;
            int c = dgv.ColumnCount;
            if (Memori.ModelTable.NameHeader(c + 1, out name))
            {
                dgv.ColumnCount = c + 1;
                dgv.Columns[c].HeaderText = name;
                dgv.Columns[c].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        public virtual DataGridView DisplayItem(int nomer, string itname, DataGridView dgv)
        {
            Effekt ef = efekt.Find((Effekt x) => x.name == itname);
            dgv = ef.Display(dgv);
            return dgv;
        }

        //public string BaseValue(string key, bool one)
        //{ return MyValue(key, one, baseparms, basechartrs); }

        //public string ActualValue(string key, bool one)
        //{ return MyValue(key, one, paramrs, characters); }

        //private string MyValue(string key, bool one, Dictionary<string, string> par, Dictionary<string, string> chr)
        //{
        //    if (one)
        //    { return par[key]; }
        //    return chr[key];
        //}

        public void Next(List<Effekt>[] maseff)
        {
            UpdataEffekt();

            List<Effekt> masef = new List<Effekt>();
            for (int i = 0; i < maseff.Length - 1; i++)
            { masef.AddRange(maseff[i]); }

            bool[] flag = new bool[efekt.Count];
            Stack<List<string>> zamnams = new Stack<List<string>>();
            for (int i = 0; i < flag.Length; i++)
            {
                List<string> nams;
                flag[i] = efekt[i].Next(out nams);
                if (flag[i])
                { zamnams.Push(nams); }
            }
            for (int i = flag.Length - 1; i > -1; i--)
            {
                if (flag[i])
                {
                    List<string> nams = zamnams.Pop();
                    List<Effekt> meff = new List<Effekt>();
                    foreach (string item in nams)
                    { meff.Add(new Effekt(masef.Find((Effekt x) => x.name == item))); }
                    if (meff.Count != 0)
                    { AddEffekt(meff, i + 1); } // copii
                    RemoveEffekt(i);
                }
            }

            // этот блок обновляет графику после обработки эффектов
            string[] key = paramrs.Keys.ToArray();
            for (int i = 0; i < key.Length; i++)
            { PersonParm(key[i], paramrs[key[i]]); }
        }

        public void RemoveEffekt(Effekt eff)
        { RemoveEffekt(efekt.FindIndex((Effekt x) => x.name == eff.name)); }

        public void RemoveEffekt(List<Effekt> meff)
        {
            foreach (Effekt item in meff)
            { RemoveEffekt(item); }
        }

        public void RemoveEffekt(int n)
        {
            if (!efekt[n].tikaet)
            { efekt[n].Allow(this, false); }
            efekt.RemoveAt(n);
        }

        public void RemoveEffekt(List<int> mn)
        {
            foreach (int i in mn)
            { RemoveEffekt(i); }
        }

        public bool Box(GroupBox gb)
        { return Grafics.contenir == gb; }

        public void UpdataEffekt()// переназвать это тикание а не обновление
        {
            List<Effekt> peref = efekt.FindAll((Effekt x) => x.tikaet);
            foreach (Effekt item in peref)
            { item.Allow(this, true); }
        }

        public void CalcConnection()
        {
            string[] keyImp = connectpars.Keys.ToArray();
            string[][] valueKey = connectpars.Values.ToArray();
            Dictionary<string, string> soursformul = new Dictionary<string, string>(parsformcon);

            for (int i = 0; i < keyImp.Length; i++)
            {
                string[] key = valueKey[i];
                foreach (string item in key)
                { soursformul[item] = soursformul[item].Replace(keyImp[i], paramrs[keyImp[i]]); }
            }
            keyImp = soursformul.Keys.ToArray();
            foreach (string i in keyImp)
            { paramrs[i] = FormulParse.ParsF(soursformul[i]); }

            keyImp = parMaxRect.Keys.ToArray();
            foreach (string k in keyImp)
            { paramrs[k] = paramrs[parMaxRect[k]]; }// ошибка при загрузке только создание уместно
        }

        public Dictionary<string, string> ImpactPar
        {
            get
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string[] key = paramrs.Keys.ToArray();
                key = key.Except(parsformcon.Keys.Union(parMaxRect.Keys)).ToArray();
                foreach (string k in key)
                { dic.Add(k, ""); }
                return dic;
            }
            set
            {
                string[] key = value.Keys.ToArray();
                foreach (string k in key)
                { paramrs[k] = value[k]; }
            }
        }
    }

    class Humanoid : Esense
    {
        public List<Predmet> eqip;
        public enum whither { eqip, inven }
        public double Hefeqip { get { return eqip.Sum((Predmet x) => x.ves); } }
        protected Dictionary<string, SlotInfo> Sloti;

        public Humanoid()
        {

        }

        public Humanoid(Esense es, XmlNode eff, XmlNode eqip, List<Effekt>[] meff, List<Predmet>[] mpred)// добавить загрузку слотов
            : base(es, eff, meff)
        {
            this.eqip = new List<Predmet>();
            Sloti = new Dictionary<string, SlotInfo>(Memori.BaseSloti);
            if (eqip != null)
            { Things(eqip, mpred, this.eqip); }
        }

        public Humanoid(Esense es, XmlNode eff, XmlNode eqip, XmlNode xmlslots, List<Effekt>[] meff, List<Predmet>[] mpred)// добавить загрузку слотов
            : this(es, eff, eqip, meff, mpred)
        {
            //
        }

        public static Dictionary<string, SlotInfo> XMLSL_Sloti(XmlNode xmlsloti)
        {
            Dictionary<string, SlotInfo> dict = new Dictionary<string, SlotInfo>();
            foreach (XmlNode item in xmlsloti)
            {
                SlotInfo slin;
                XmlAttribute atrib = item.Attributes["Quantity"];
                if (atrib != null)
                {
                    atrib = item.Attributes["Master"];
                    if (atrib != null)
                    { slin = new SlotInfo(item.Attributes[1].Value, item.Attributes[2].Value, item.Attributes[3].Value); }
                    else { slin = new SlotInfo(item.Attributes[1].Value); }
                }
                else { slin = new SlotInfo("1"); }
                dict.Add(item.Attributes[0].Value, slin);
            }
            return dict;
        }

        public virtual bool AddPredmet(Predmet pred, whither whit)
        {
            SlotInfo slin = Sloti[pred.slot];
            int slp = int.Parse(pred.sltcount);
            if (slin.quant >= slp)
            {
                if (pred.effekt.Count > 0)
                { AddEffekt(pred.effekt); }

                Waffe wef = pred as Waffe;
                if (wef != null)
                {
                    damage.AddRange(wef.dem);
                }

                Schutz arm = pred as Schutz;
                if (arm != null)
                {
                    string[] key = arm.armor.Keys.ToArray();
                    foreach (string k in key)
                    {
                        armor[k] = (int.Parse(armor[k]) + int.Parse(arm.armor[k])).ToString();
                    }
                }

                Predmet p = pred.Copi();
                slin.Add(p, true);
                eqip.Add(p);
                this.PersonParm("hef", Hefeqip.ToString());
            }
            else
            {
                MessageBox.Show("Sloti Max: " + pred.name);
                return false;
            }
            return true;
        }

        //private void ArmorAdd(Dictionary<string, 

        public virtual void RemovePredmet(int j, whither whit)
        {
            // Вычетание
            Predmet pred = eqip[j];
            eqip.RemoveAt(j);
            SlotInfo slin = Sloti[pred.slot];
            slin.Remove(pred);
            RemoveEffekt(pred.effekt);
            this.PersonParm("hef", Hefeqip.ToString());
        }

        public virtual void RemovePredmet(List<int> mj, whither whit)
        {
            foreach (int i in mj)
            { RemovePredmet(i, whither.eqip); }
        }

        public void AddPredmet(List<Predmet> maspred)
        {
            foreach (Predmet item in maspred)
            { AddPredmet(item, whither.eqip); }
        }

        protected void Things(XmlNode xmlthin, List<Predmet>[] mp, List<Predmet> thin)
        {
            foreach (XmlNode pred in xmlthin)
            {
                Predmet p = new Predmet();
                if (pred.InnerText != "")
                {
                    for (int i = 0; i < mp.Length; i++)
                    {
                        p = mp[i].Find((Predmet x) => x.name == pred.InnerText);
                        if (p != null)
                        { break; }
                    }
                }
                else { p = WriteRead.ReadPredmet(pred); }
                thin.Add(p);
            }
        }

        public override DataGridView Display(DataGridView dgv)
        {
            base.Display(dgv);
            HeaderFill(dgv);
            ColumnFill(dgv, eqip.Cast<ItemCore>().ToList());
            return dgv;
        }

        public override DataGridView DisplayItem(int nomer, string itname, DataGridView dgv)
        {
            switch (nomer)
            {
                case 1:
                    dgv = base.DisplayItem(nomer, itname, dgv);
                    break;
                case 2:
                    Predmet pr = eqip.Find((Predmet x) => x.name == itname);
                    dgv = pr.Display(dgv);
                    break;
            }
            return dgv;
        }
    }

    class Human : Humanoid, CoreC
    {
        public DataGridView Table { get; protected set; }
        public SplitterPanel TableControl { get; protected set; }

        public List<Predmet> inven;
        public double Hefinvtn { get { return inven.Sum((Predmet x) => x.ves); } }
        public double HefTotal { get { return Hefeqip + Hefinvtn; } }

        public Human()
        {

        }

        public Human(Esense es, XmlNode eff, XmlNode eqip, XmlNode inv, List<Effekt>[] meff, List<Predmet>[] mpred)
            : base(es, eff, eqip, meff, mpred)
        {
            this.inven = new List<Predmet>();
            if (inv != null)
            { Things(inv, mpred, this.inven); }
        }

        public Human(Esense es, XmlNode eff, XmlNode eqip, XmlNode inv, XmlNode xmlslots, List<Effekt>[] meff, List<Predmet>[] mpred)
            : this(es, eff, eqip, inv, meff, mpred)
        {
            //
        }

        public void DisplaseInvenEqip(int innom)
        {
            Predmet pred = inven[innom];
            if (AddPredmet(pred, whither.eqip))
            { RemovePredmet(innom, whither.inven); }
        }

        public void DisplaseInvenEqip(List<int> innoms)
        {
            foreach (int i in innoms)
            { DisplaseInvenEqip(i); }
        }

        public void DisplaseEqipInven(int eqnom)
        {
            Predmet pred = eqip[eqnom];
            RemovePredmet(eqnom, whither.eqip);
            AddPredmet(pred, whither.inven);
        }

        public void DisplaseEqipInven(List<int> eqnoms)
        {
            foreach (int i in eqnoms)
            { DisplaseEqipInven(i); }
        }

        public override bool AddPredmet(Predmet pred, whither whit)
        {
            switch ((int)whit)
            {
                case 0:
                    return base.AddPredmet(pred, whit);
                case 1:
                    inven.Add(pred.Copi());
                    this.PersonParm("hef", HefTotal.ToString());
                    break;
            }
            return true;
        }

        public override void RemovePredmet(int j, Humanoid.whither whit)
        {
            switch ((int)whit)
            {
                case 0:
                    base.RemovePredmet(j, whit);
                    break;
                case 1:
                    inven.RemoveAt(j);
                    break;
            }
        }

        public override void RemovePredmet(List<int> mj, Humanoid.whither whit)
        {
            switch ((int)whit)
            {
                case 0:
                    base.RemovePredmet(mj, whit);
                    break;
                case 1:
                    foreach (int i in mj)
                    { inven.RemoveAt(i); }
                    break;
            }
        }

        public void AddPredmet(List<Predmet> maspred, Humanoid.whither writ)
        {
            foreach (Predmet item in maspred)
            { AddPredmet(item, writ); }
        } 

        public void Hero(Esense essense, int count, bool zagruz)
        {
            deystvij = 2;
            paramrs = new Dictionary<string, string>(essense.paramrs);
            baseparms = new Dictionary<string, string>(essense.paramrs);
            Sloti = new Dictionary<string, SlotInfo>(Memori.BaseSloti);
            //parMaxRect = new Dictionary<string,string>
            damage = new List<List<Tostr>>(); // исправить с загрузкой
            damage.Add(new List<Tostr>());
            damage[0].Add(new Tostr("рука", "1Д2"));

            eqip = new List<Predmet>();
            inven = new List<Predmet>();
            if (zagruz)
            {
                string adres = "profile\\ItemsAndEffektsHeroes\\" + paramrs["Name"] + ".xml";
                XmlNodeList masnode = WriteRead.Root(adres);
                XmlNode items = masnode[0];
                efekt = new List<Effekt>();
                foreach (XmlNode item in items)
                { AddEffekt(WriteRead.ReadEffekt(item)); }

                items = masnode[1];
                int i = 0;
                foreach (XmlNode item in items)
                {
                    if (i < count)
                    { eqip.Add(WriteRead.ReadPredmet(item)); } // еще не создан графикс что вызывает ошибку
                    else { inven.Add(WriteRead.ReadPredmet(item)); }
                    i++;
                }
            }
        }

        public void CreatHero(TabControl tab, Form1 f1)
        {
            // Создание новой TabPage с внешним видом
            TabPage tp = new TabPage(paramrs["Name"]);
            tp.BackColor = Color.DarkGray;
            tp.Padding = new System.Windows.Forms.Padding(3);
            SplitContainer sp1 = new SplitContainer();
            SplitContainer sp2 = new SplitContainer();
            sp1.Dock = DockStyle.Fill;
            sp1.Orientation = Orientation.Vertical;
            sp1.SplitterDistance = 90;
            int c = sp1.Margin.All;
            sp2.Dock = DockStyle.Fill;
            sp2.Orientation = Orientation.Horizontal;
            sp2.SplitterDistance = 59;
            sp2.BackColor = Color.DarkGray;
            sp1.Panel2.Controls.Add(sp2);
            sp1.Panel1.BackColor = Color.White;
            sp1.Panel2.BackColor = Color.White;
            sp2.Panel1.BackColor = Color.White;
            sp2.Panel2.BackColor = Color.White;

            DataGridView[] dg = new DataGridView[3];
            for (int i = 0; i < dg.Length; i++)
            {
                dg[i] = new DataGridView();

                dg[i].Dock = DockStyle.Fill;
                dg[i].Margin = new Padding(3);
                dg[i].Location = new Point(3, 3);
                dg[i].BackgroundColor = SystemColors.GradientInactiveCaption;
            }
            dg[0].CurrentCellChanged += new System.EventHandler(f1.dataGridView1_CurrentCellChanged);
            //dg[1].MouseClick += new MouseEventHandler(f1.dataGridView2_MouseClick);
            sp1.Panel1.Controls.Add(dg[0]);
            sp2.Panel1.Controls.Add(dg[1]);
            sp2.Panel2.Controls.Add(dg[2]);

            tp.Controls.Add(sp1);
            tab.Controls.Add(tp);
            //

            // Наполнение DataGriedViev
            TableControl = sp1.Panel1;
            Display(dg[0]);
            Table = dg[0];
            Table.ContextMenuStrip = f1.contextMenuStrip3;
            //
        }

        public void Save(List<string>[] str)
        {
            List<string> param = str[0];
            List<string> meff = str[1];
            List<string> eqip = str[2];
            List<string> inven = str[3];

            string tab = "  ";
            string[] mkey = paramrs.Keys.ToArray();
            string[] mvalue = paramrs.Values.ToArray();
            for (int i = 0; i < mkey.Length; i++)
            { param.Add(tab + mkey[i] + " = \"" + mvalue[i] + "\""); }

            meff.AddRange(this.efekt.Select<Effekt, string>((Effekt x) => x.ToString()).ToList<string>());
            eqip.AddRange(this.eqip.Select<Predmet, string>((Predmet x) => x.ToString()).ToList<string>());
            inven.AddRange(this.inven.Select<Predmet, string>((Predmet x) => x.ToString()).ToList<string>());
            Shell(meff, "E");
            Shell(eqip, "P");
            Shell(inven, "P");
        }

        private void Shell(List<string> ms, string V)
        {
            string op = "<" + V + ">";
            string en = "</" + V + ">";
            for (int i = 0; i < ms.Count; i++)
            { ms[i] = "  " + op + ms[i] + en; }
        }

        public override DataGridView Display(DataGridView dgv)
        {
            base.Display(dgv);
            HeaderFill(dgv);
            ColumnFill(dgv, inven.Cast<ItemCore>().ToList());
            return dgv;
        }

        public override DataGridView DisplayItem(int nomer, string itname, DataGridView dgv)
        {
            if (nomer < 3)
            { dgv = base.DisplayItem(nomer, itname, dgv); }
            else
            {
                Predmet pr = inven.Find((Predmet x) => x.name == itname);
                dgv = pr.Display(dgv);
            }
            return dgv;
        }

        public void DisplayUpdate()
        {
            Table.Columns.Clear();
            Table.Rows.Clear();
            Display(Table);
        }

        //public void Addeqip(List<Predmet> i)
        //{
        //    this.eqip.AddRange(i);
        //}

        //public void Addeqip(Predmet i)
        //{
        //    this.eqip.Add(i);
        //}
    }

    abstract class ItemCore : CoreC
    {
        public string name;
        public string type;
        public string opis;

        public ItemCore()
        {

        }

        public override string ToString()
        {
            return name;
        }

        public static List<string> MasToString(List<ItemCore> mas)
        {
            return mas.ConvertAll<string>((ItemCore x) => x.name);
        }

        public virtual void Save(List<string>[] str)
        {
            string tab = "  ";
            str[0].Add(tab + "Name = \"" + name + "\"");
            str[0].Add(tab + "Type = \"" + type + "\"");
            str[0].Add(tab + "Specif = \"" + opis + "\"");
        }
    }

    class Effekt : ItemCore
    {
        public string mtime;
        public string time;

        public Dictionary<string, string> haracter;
        public Dictionary<string, string> harK_Kusl;

        public bool tikaet;
        public bool videm;
        public bool hidden;

        public Dictionary<string, int> zamenam;
        public Dictionary<string, string> zuslov;
        public string[] tiki;

        public Effekt()
        {
            haracter = new Dictionary<string, string>();
            harK_Kusl = new Dictionary<string, string>();
            zamenam = new Dictionary<string, int>();
            zuslov = new Dictionary<string, string>();
            tiki = new string[0];
        }

        public Effekt(List<string> mstr)
            : this() { tiki = mstr.ToArray(); }

        public Effekt(Effekt ef)
        {
            name = ef.name;
            type = ef.type;
            opis = ef.opis;
            mtime = ef.mtime;
            time = ef.time;
            videm = ef.videm;
            tikaet = ef.tikaet;

            haracter = new Dictionary<string, string>(ef.haracter);
            harK_Kusl = new Dictionary<string, string>(ef.harK_Kusl);
            zamenam = new Dictionary<string, int>(ef.zamenam);
            zuslov = new Dictionary<string, string>(ef.zuslov);
            tiki = ef.tiki;
        }

        public bool Next(out List<string> names)
        {
            names = new List<string>();
            int tim = int.Parse(time);
            if (tim > 0)
            {
                time = (tim - 1).ToString();

                if (time == "0")
                {
                    //foreach (string item in zamenam)
                    //{ names.Add(item); }
                    return true;
                }
                return false;
            }
            return false;
        }

        public Effekt Copi()
        {
            return new Effekt(this);
        }

        //public bool Equals(Effekt ef)
        //{
        //    if (this.name != ef.name)
        //    { return false; }
        //    if
        //        (
        //    this.type == ef.type &&
        //    this.opis == ef.opis &&
        //    this.tikaet == ef.tikaet &&
        //    this.time == ef.time &&
        //    this.videm == ef.videm &&
        //    this.zamenam == ef.zamenam &&
        //    this.zuslov == ef.zuslov
        //        )
        //    {
        //        for (int i = 0; i < this.haracter.Count; i++)
        //        {
        //            //if (this.haracter[i] != ef.haracter[i])
        //            //{ return false; }
        //        }
        //        return true;
        //    }
        //    return false;
        //}

        public DataGridView Display(DataGridView dgv)
        {
            dgv.ColumnCount = 1;
            dgv.RowCount = 5;
            dgv.Columns[0].HeaderText = "Статы";
            dgv.Columns[0].Width = 75;
            dgv.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgv.RowHeadersWidth = 75;
            dgv.Rows[0].HeaderCell.Value = "Имя:";
            dgv.Rows[1].HeaderCell.Value = "Тип:";
            dgv.Rows[2].HeaderCell.Value = "Время:";
            dgv.Rows[3].HeaderCell.Value = "Видим:";
            dgv.Rows[4].HeaderCell.Value = "(Тп):";

            dgv.Rows[0].Cells[0].Value = this.name;
            dgv.Rows[1].Cells[0].Value = this.type;
            dgv.Rows[2].Cells[0].Value = this.time;
            dgv.Rows[3].Cells[0].Value = this.videm;
            dgv.Rows[4].Cells[0].Value = this.tikaet;
            return dgv;
        }

        public override void Save(List<string>[] str)
        {
            base.Save(str);
            string tab = "  ";
            str[0].Add(tab + "TimAct = \"" + mtime + "\"");
            str[0].Add(tab + "Time = \"" + time + "\"");
            str[0].Add(tab + "Visible = \"" + videm + "\"");
            str[0].Add(tab + "Hidden = \"" + hidden + "\"");
            str[0].Add(tab + "Period = \"" + tikaet + "\"");
            if (tikaet)
            {
                string ms = "";
                foreach (string s in tiki)
                { ms += s + ","; }
                ms = ms.Remove(ms.Length - 1);
                str[0].Add(tab + "Tiki = \"" + ms + "\"");
            }

            string[] key = haracter.Keys.ToArray();
            for (int i = 0; i < key.Length; i++)
            { str[1].Add("  <O Key = \"" + key[i] + "\">" + haracter[key[i]] + "</O>"); }

            //замена
        }

        public void Allow(Esense es, bool modif)
        {
            int m = modif ? 1 : -1;
            string[] key = haracter.Keys.ToArray();
            foreach (string i in key)
            {
                if (haracter[i].Contains("base="))
                {
                    //es.paramrs[i] = haracter[i]
                }
                else
                {
                    if (haracter[i].Contains("base"))
                    {
                        double val = int.Parse(haracter[i].Remove(0, 4)) / 100.0;
                        int rez = int.Parse(es.paramrs[i]);
                        int bez = int.Parse(es.baseparms[i]);
                        es.paramrs[i] = (Math.Round(rez + val * bez)).ToString();
                    }
                    else
                    {
                        if (tiki.Contains(time))
                        {
                            int val = int.Parse(haracter[i]);
                            int rez = int.Parse(es.paramrs[i]);
                            es.paramrs[i] = (rez + m * val).ToString();
                        }
                    }
                }
            }
        }

        public static string[] ParsTik(string tik , string max)
        {
            List<string> ls = new List<string>();
            bool bl = tik.Contains('t');
            string str = tik.Replace("ink[", "");
            string s = bl ? "]t" : "]";
            int ink = int.Parse(str.Replace(s, ""));
            int i = int.Parse(max);
            for (; i > 0; i -= ink)
            { ls.Add(i.ToString()); }
            if (bl)
            { ls.RemoveAt(0); }
            return ls.ToArray();
        }
    }

    class Predmet : ItemCore
    {
        public string slot;
        public string sltcount;
        public string stammax;
        public string stam;
        public string maxstk;
        public string kolich;
        public string price;
        public string metmis;
        public double ves;

        public List<Effekt> effekt;

        protected List<string> NameVariable;

        public Predmet()
        {
            effekt = new List<Effekt>();
            NameVariable = new List<string>();
        }

        public virtual void VaryValue(string name, string value)
        {
            int i = NameVariable.IndexOf(name);
            switch (i)
            {
                case 0:
                    name = value;
                    break;
                case 1:
                    type = value;
                    break;
                case 2:
                    opis = value;
                    break;
                case 3:
                    slot = value;
                    break;
                case 4:
                    sltcount = value;
                    break;
                case 5:
                    stammax = value;
                    break;
                case 6:
                    stam = value;
                    break;
                case 7:
                    price = value;
                    break;
                case 8:
                    metmis = value;
                    break;
                case 9:
                    maxstk = value;
                    break;
                case 10:
                    kolich = value;
                    break;
                case 11:
                    ves = double.Parse(value);
                    break;
            }
        }

        public virtual void Load(XmlNode xmlpred, List<Effekt>[] stmeff)
        {
            this.name = xmlpred.Attributes["Name"].Value;
            NameVariable.Add("Name");
            XmlAttribute atrb = xmlpred.Attributes["Type"];
            if (atrb != null)
            { this.type = atrb.Value; }
            else { this.type = xmlpred.ParentNode.Attributes[0].Value; }
            NameVariable.Add("Type");
            atrb = xmlpred.Attributes["Specif"];
            if (atrb != null)
            { this.opis = atrb.Value; }
            else { this.opis = "НЕТ"; } //aaaaaaaaa
            NameVariable.Add("Specif");
            atrb = xmlpred.Attributes["Slot"];
            if (atrb != null)
            {
                this.slot = atrb.Value;
                atrb = xmlpred.Attributes["SltCnt"];
                if (atrb != null)
                { this.sltcount = atrb.Value; }
                else { this.sltcount = "1"; }
            }
            else
            {
                this.slot = "НЕТ";
                this.sltcount = "0";
            }
            NameVariable.Add("Slot");
            NameVariable.Add("SltCnt");
            this.stammax = xmlpred.Attributes["Staminam"].Value;
            NameVariable.Add("Staminam");
            atrb = xmlpred.Attributes["Stamina"];
            if (atrb != null)
            { this.stam = atrb.Value; }
            else { this.stam = stammax; }
            NameVariable.Add("Stamina");
            this.price = xmlpred.Attributes["Price"].Value;
            NameVariable.Add("Price");
            atrb = xmlpred.Attributes["Basesharp"];
            if (atrb != null)
            { this.metmis = atrb.Value; }
            else { this.metmis = ""; }
            NameVariable.Add("Basesharp");
            atrb = xmlpred.Attributes["Countm"];
            if (atrb != null)
            {
                this.maxstk = atrb.Value;
                atrb = xmlpred.Attributes["Count"];
                if (atrb != null)
                { this.kolich = atrb.Value; }
                else { this.kolich = "1"; }
            }
            else
            {
                this.maxstk = "1";
                this.kolich = "1";
            }
            NameVariable.Add("Countm");
            NameVariable.Add("Count");
            atrb = xmlpred.Attributes["Heft"];
            if (atrb != null)
            { this.ves = double.Parse(atrb.Value); }
            else { this.ves = 0.0; }
            NameVariable.Add("Heft");
            //////////////////////////////////////
            XmlNode xmlpr = xmlpred["Effects"];
            if (xmlpr != null)
            {
                foreach (XmlNode xmleff in xmlpr)
                {
                    Effekt ef = new Effekt();
                    if (xmleff.InnerText != "")
                    {
                        for (int i = 0; i < stmeff.Length; i++)
                        {
                            ef = stmeff[i].Find((Effekt x) => x.name == xmleff.InnerText);
                            if (ef != null)
                            { break; }
                        }
                    }
                    else { ef = WriteRead.ReadEffekt(xmleff); }
                    ef.time = "-2";
                    this.effekt.Add(ef);
                }
            }
        }

        public override void Save(List<string>[] str)
        {
            base.Save(str);
            string tab = "  ";
            str[0].Add(tab + "Slot = \"" + slot + "\"");
            str[0].Add(tab + "SltCnt = \"" + sltcount + "\"");
            str[0].Add(tab + "Staminam = \"" + stammax + "\"");
            str[0].Add(tab + "Stamina = \"" + stam + "\"");
            str[0].Add(tab + "Price = \"" + price + "\"");
            str[0].Add(tab + "Basesharp = \"" + metmis + "\"");
            str[0].Add(tab + "Countm = \"" + maxstk + "\"");
            str[0].Add(tab + "Count = \"" + kolich + "\"");
            str[0].Add(tab + "Heft = \"" + ves + "\"");

            foreach (Effekt item in effekt)
            { str[1].Add(tab + "<E>" + item.ToString() + "</E>"); }
        }

        public DataGridView Display(DataGridView dgv)
        {
            dgv.ColumnCount = 1;
            dgv.RowCount = 14;

            dgv.Columns[0].HeaderText = "Статы";
            dgv.Columns[0].Width = 75;
            dgv.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgv.RowHeadersWidth = 75;
            dgv.Rows[0].HeaderCell.Value = "Имя:";
            dgv.Rows[1].HeaderCell.Value = "Тип:";
            dgv.Rows[2].HeaderCell.Value = "Слот:";
            dgv.Rows[3].HeaderCell.Value = "Прочм:";
            dgv.Rows[4].HeaderCell.Value = "Проч:";
            dgv.Rows[5].HeaderCell.Value = "Метк:";
            dgv.Rows[6].HeaderCell.Value = "Кол-м:";
            dgv.Rows[7].HeaderCell.Value = "Кол-в:";
            dgv.Rows[8].HeaderCell.Value = "Вес:";

            dgv.Rows[9].HeaderCell.Value = "Урон:";
            dgv.Rows[10].HeaderCell.Value = "Крит:";
            dgv.Rows[11].HeaderCell.Value = "Двур:";

            dgv.Rows[12].HeaderCell.Value = "Далн:";
            dgv.Rows[13].HeaderCell.Value = "Зард:";

            dgv.Rows[0].Cells[0].Value = this.name;
            dgv.Rows[1].Cells[0].Value = this.type;
            dgv.Rows[2].Cells[0].Value = this.slot;
            dgv.Rows[3].Cells[0].Value = this.stammax;
            dgv.Rows[4].Cells[0].Value = this.stam;
            dgv.Rows[5].Cells[0].Value = this.metmis;
            dgv.Rows[6].Cells[0].Value = this.maxstk;
            dgv.Rows[7].Cells[0].Value = this.kolich;
            dgv.Rows[8].Cells[0].Value = this.ves;

            return dgv;
        }

        public virtual Predmet Copi()
        {
            return Copi(0);
        }

        protected Predmet Copi(int i)
        {
            Predmet copi;
            switch (i)
            {
                case 1:
                    copi = new Waffe();
                    break;
                case 2:
                    copi = new Dalwaffe();
                    break;
                default:
                    copi = new Predmet();
                    break;
            }
            copi.name = this.name;
            copi.type = this.type;
            copi.opis = this.opis;
            copi.slot = this.slot;
            copi.sltcount = this.sltcount;
            copi.stammax = this.stammax;
            copi.stam = this.stam;
            copi.maxstk = this.maxstk;
            copi.kolich = this.kolich;
            copi.price = this.price;
            copi.metmis = this.metmis;
            copi.ves = this.ves;

            string[] ms = new string[this.NameVariable.Count];
            this.NameVariable.CopyTo(ms);
            copi.NameVariable = ms.ToList();

            List<Effekt> copief = new List<Effekt>();
            foreach (Effekt item in this.effekt)
            { copief.Add(new Effekt(item)); }
            copi.effekt = copief;
            return copi;
        }
    }

    class Waffe : Predmet
    {
        private int status; //показывает текущий режим
        List<string> ss = new List<string>();

        public List<List<Tostr>> dem;
        public string krit;

        public Waffe()
        {
            status = 0;
            dem = new List<List<Tostr>>();
            AlternativeWaffe = new List<Waffe>();
            AlternativeWaffe.Add(this);
        }

        public override void VaryValue(string name, string value)
        {
            base.VaryValue(name, value);
            int i = NameVariable.IndexOf(name);
            switch (i)
            {
                case 12:
                    krit = value;
                    break;
                case 13:
                    LD(value);
                    break;
            }
        }

        public override void Load(XmlNode xmlpred, List<Effekt>[] stmeff)
        {
            base.Load(xmlpred, stmeff);
            XmlAttribute atrb = xmlpred.Attributes["Crete"];
            if (atrb != null)
            { this.krit = atrb.Value; }
            else { this.krit = "1"; }

            NameVariable.Add("Crete");
            NameVariable.Add("Damage");
            string str = xmlpred.Attributes["Damage"].Value;
            LD(str);
            

            XmlNode xmlalt = xmlpred["Alternative"];
            if (xmlalt != null)
            {
                Waffe waf = (Waffe)this.Copi();
                XmlAttributeCollection masatrb = xmlalt.Attributes;
                foreach (XmlAttribute item in masatrb)
                { waf.VaryValue(item.Name, item.Value); }
                AlternativeWaffe.Add(waf);
            }
        }

        public List<List<Tostr>> ParserDamage(string damage)
        {
            LD(damage);
            return dem;
        }

        protected void LD(string str)
        {
            dem.Clear();
            str = str.Replace(" ", "");
            string[] ms;
            if (str.Contains("|"))
            { ms = str.Split('|'); }
            else
            {
                ms = new string[1];
                ms[0] = str;
            }
            string[][] mms = new string[ms.Length][];
            for (int i = 0; i < ms.Length; i++)
            { mms[i] = ms[i].Split('&'); }
            for (int i = 0; i < mms.Length; i++)
            {
                dem.Add(new List<Tostr>());
                for (int j = 0; j < mms[i].Length; j++)
                {
                    string[] keyval = mms[i][j].Split(':');
                    dem[i].Add(new Tostr(keyval[0], keyval[1]));
                }
            }
        }

        public override void Save(List<string>[] str)
        {
            base.Save(str);
            string tab = "  ";
            string ss = "";
            foreach (List<Tostr> item in dem)
            {
                string s = "";
                foreach (Tostr tstr in item)
                { s += tstr.key + ":" + tstr.formul + "&amp;"; }
                s = s.Remove(s.Length - 5);
                ss += s + "|";
            }
            ss = ss.Remove(ss.Length - 1);

            str[0].Add(tab + "Damage = \"" + ss + "\"");
            str[0].Add(tab + "Crete = \"" + krit + "\"");

            // Alternative
        }

        private List<Waffe> AlternativeWaffe;

        public Waffe Transmut(int i)
        {
            AlternativeWaffe[i].AlternativeWaffe = this.AlternativeWaffe;
            return AlternativeWaffe[i];
        }

        public override Predmet Copi()
        {
            Waffe waf = (Waffe)base.Copi(1);
            waf.dem = this.dem;
            waf.krit = this.krit;
            return waf;
        }
    }

    class Dalwaffe : Waffe
    {
        public string dal;
        public string zar;

        public override void Load(XmlNode xmlpred, List<Effekt>[] stmeff)
        {
            base.Load(xmlpred, stmeff);
            this.dal = xmlpred.Attributes["Range"].Value;
            NameVariable.Add("Range");
            XmlAttribute atrb = xmlpred.Attributes["Ammo"];
            if (atrb != null)
            { this.zar = atrb.Value; }
            else { this.zar = "НЕТ"; }
            NameVariable.Add("Ammo");
        }

        public override void VaryValue(string name, string value)
        {
            base.VaryValue(name, value);
            int i = NameVariable.IndexOf(name);
            switch (i)
            {
                case 14:
                    dal = value;
                    break;
                case 15:
                    zar = value;
                    break;
            }
        }

        public override void Save(List<string>[] str)
        {
            base.Save(str);
            string tab = "  ";
            str[0].Add(tab + "Range = \"" + dal + "\"");
            str[0].Add(tab + "Ammo = \"" + zar + "\"");
        }
        public override Predmet Copi()
        {
            Dalwaffe dwaf = (Dalwaffe)Copi(2);
            dwaf.dal = this.dal;
            dwaf.zar = this.zar;
            return dwaf;
        }
    }

    class Schutz : Predmet
    {
        public Dictionary<string, string> armor;

        public Schutz()
        {

        }

        public override void Load(XmlNode xmlpred, List<Effekt>[] stmeff)
        {
            base.Load(xmlpred, stmeff);
            string str = xmlpred.Attributes["Shield"].Value;
            string[] ms;
            string[][] mms;
            if (str.Contains("/"))
            {
                ms = str.Split('/');
                mms = new string[ms.Length][];
                for (int i = 0; i < mms.Length; i++)
                { mms[i] = ms[i].Split(':'); }
            }
            else
            {
                mms = new string[1][];
                mms[0] = str.Split(':');
            }
            armor = Instrument.ParserArmor(mms);
        }

        public override void Save(List<string>[] str)
        {
            base.Save(str);
            string sars = "";
            string[] key = armor.Keys.ToArray();
            foreach (string k in key)
            { sars += k + ":" + armor[k] + "/"; }
            sars.Remove(sars.Length - 1);
            str[0].Add("  Shield = \"" + sars + "\"");
        }
    }

    static class CursorHS
    {
        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
        [DllImport("user32.dll")]
        private static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        public static Cursor CreateCursorNoResize(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IntPtr ptr = bmp.GetHicon();
            IconInfo tmp = new IconInfo();
            GetIconInfo(ptr, ref tmp);
            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;
            ptr = CreateIconIndirect(ref tmp);
            return new Cursor(ptr);
        }
    }

    struct Tostr
    {
        public string key;
        public string formul;

        public Tostr(string key, string formul)
        {
            this.key = key;
            this.formul = formul;
        }
    }

    class FormulaConten
    {
        public int target;
        public string aktiv;
        private List<string> pasiv = new List<string>();
        private Dictionary<string, List<Tostr>> argum = new Dictionary<string, List<Tostr>>();

        public FormulaConten(XmlNode xmlform)
        {
            target = int.Parse(xmlform.Attributes[1].Value);
            XmlNodeList param = xmlform.ChildNodes;
            aktiv = param[0].InnerText;
            int max = target + 1;

            Dictionary<string, Queue<string>> marg = new Dictionary<string, Queue<string>>();
            for (int i = 1; i < max; i++)
            {
                string str = param[i].InnerText;
                if (!pasiv.Contains(param[i].InnerText))
                {
                    argum.Add(str, new List<Tostr>());
                    marg.Add(str, new Queue<string>());
                    foreach (XmlAttribute item in param[i].Attributes)
                    { marg[str].Enqueue(item.Value); }
                }
                pasiv.Add(str);
            }

            string strs = pasiv[0];
            int j = 1;
            for (int i = max; i < param.Count; i++)
            {
                if (marg[strs].Count == 0)
                { strs = pasiv[j]; j++; }
                argum[strs].Add(new Tostr(marg[strs].Dequeue(), param[i].InnerText));
            }
        }

        public List<Tostr> Datapas(int i, out string nampas)
        {
            nampas = pasiv[i];
            return argum[nampas];
        }
    }

    static class Memori
    {
        public static Esense modpers;
        public static Dictionary<string, Dictionary<string, string>> Tables = new Dictionary<string, Dictionary<string, string>>();
        private static Dictionary<string, SlotInfo> baseSloti;
        public static Dictionary<string, SlotInfo> BaseSloti
        {
            get
            {
                string[] key = baseSloti.Keys.ToArray();
                Dictionary<string, SlotInfo> bs = new Dictionary<string, SlotInfo>();
                foreach (string k in key)
                { bs.Add(k, baseSloti[k].Copi()); }
                return bs;
            }
            set { baseSloti = value; }
        }
        public static Dictionary<string, List<string>> Armor = new Dictionary<string, List<string>>();

        public static Dictionary<string, int> ItemVariable = new Dictionary<string, int> 
        { { "Эффекты", 1 }, { "Экипировка", 2 }, { "Инвентарь", 3 } };
        public static Dictionary<string, string> Labeltext = new Dictionary<string, string>();
        public static Dictionary<string, FormulaConten> Formulas = new Dictionary<string, FormulaConten>();
        public static List<string> VidEffekt = new List<string>();
        public static List<string> VidPredmet = new List<string>();
        
        public static class ModelTable
        {
            private static int i = 0;
            private static bool[] CreatHeader = new bool[5];
            private static string[] NameHeaders = new string[5];

            public static Dictionary<string, string> TableHero = new Dictionary<string, string>();

            public static bool NameHeader(int n, out string name)
            {
                name = NameHeaders[n];
                return CreatHeader[n];
            }

            public static void Add(bool create, string name)
            {
                if (i < 5)
                {
                    CreatHeader[i] = create;
                    NameHeaders[i] = name;
                    i++;
                }
            }
        }
    }
    //
}
