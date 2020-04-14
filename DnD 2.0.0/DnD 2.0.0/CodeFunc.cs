using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace DnD_2._0._0
{
    static class Instrument
    {
        public static Dictionary<string, string> ParserArmor(string[][] masstr)
        {
            Dictionary<string, string> dss = new Dictionary<string, string>();
            foreach (string[] item in masstr)
            {
                List<string> ls = PA(item[0]);
                foreach (string name in ls)
                {
                    if (dss.ContainsKey(name))
                    { dss[name] = item[1]; }
                    else { dss.Add(name, item[1]); }
                }
            }
            return dss;
        }

        private static List<string> PA(string str)
        {
            List<string> ls = Memori.Armor[str];
            if (ls != null)
            {
                for (int i = 0; i < ls.Count; )
                {
                    List<string> lss = PA(ls[i]);
                    if (lss.Count > 1)
                    {
                        ls.RemoveAt(i);
                        ls.InsertRange(i, lss);
                    }
                    else { i++; }
                }
            }
            else
            { ls = new List<string>() { str }; }
            return ls;
        }
    }

    public partial class Form1
    {
        private void Event(Esense orig, List<Esense> pas, FormulaConten fc)
        {
            for (int i = 0; i < pas.Count; i++)
            {
                string nampas;
                List<Tostr> valformul = fc.Datapas(i, out nampas);
                foreach (Tostr item in valformul)
                {
                    string strform = FormulParse.ReplaceStrVal(item.formul, fc.aktiv, orig.paramrs.Keys.ToArray(), orig.paramrs.Values.ToArray());
                    strform = FormulParse.ReplaceStrVal(strform, nampas, pas[i].paramrs.Keys.ToArray(), pas[i].paramrs.Values.ToArray());
                    int result = ParserRecursiv(0, strform);
                    if (result != -121)
                    {
                        pas[i].PersonParm(item.key, result.ToString());
                    }
                }
            }
        }

        private int ParserRecursiv(int i, string str)
        {
            string push = "push" + i;
            if (str.Contains(push))
            {
                Form2 f2 = new Form2();
                f2.ShowDialog();
                string valpush = f2.numericUpDown1.Value.ToString();
                str = str.Replace(push, valpush);
            }
            str = FormulParse.ParsF(str);
            if (str != "end")
            {
                int result;
                if (!int.TryParse(str, out result))
                { result = ParserRecursiv(i + 1, str); }
                return result;
            }
            return -121;
        }
    }
}
