using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;


namespace DnD_2._0._0
{
    static class WriteRead
    {
        private static List<Effekt>[] stmeff;
        private static List<string>[] tegs;
        private static List<string> header;
        private static List<string[]>[] unserHeader;
        private enum Core { Hero, Effekt, Item, HinEff, HinIt };
        private const string tb = "  ";

        /////////////////////////////////////////////
        private static string[] uo = new string[] { "Weapon", "Weapon", "Armor" };
        /////////////////////////////////////////////
        // Save
        #region
        public static void Save(List<Human> put, List<Effekt>[] ef, List<Predmet>[] pr)
        {
            InitializeTegs();
            SaveCore("profile\\Heroes.xml", Core.Hero, put.Cast<CoreC>().ToList(), false, 0, true);

            for (int i = 0; i < put.Count; i++)
            {
                SaveCore("profile\\ItemsAndEffektsHeroes\\" + put[i].PersonParm("Name") + ".xml", Core.HinEff, put[i].efekt.Cast<CoreC>().ToList(), false, 0, false);
                SaveCore("profile\\ItemsAndEffektsHeroes\\" + put[i].PersonParm("Name") + ".xml", Core.HinIt, (put[i].eqip.Union(put[i].inven)).Cast<CoreC>().ToList(), true, 0, true);
            }

            //for (int i = 0; i < ef.Length - 1; i++)
            //{ SaveCore("profile\\Effects.xml", Core.Effekt, ef[i].Cast<CoreC>().ToList(), i != 0, i, i == ef.Length - 2); }
            //for (int i = 0; i < ef.Length - 1; i++)
            //{
            //    SaveCore("profile\\Items.xml", Core.Item, pr[i].Cast<CoreC>().ToList(), i != 0, i, i == ef.Length - 2);
            //    if (i < uo.Length)
            //    { tegs[2][0] = uo[i]; }
            //}
        }

        private static void SaveCore(string adres, Core clas, List<CoreC> core, bool pis, int nomermas, bool b_end)
        {
            int i = (int)clas;

            StreamWriter writ = new StreamWriter(adres, pis);
            if (!pis)
            {
                writ.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                writ.WriteLine();
                writ.WriteLine("<" + header[i] + ">"); // Главный заголовок
            }
            if (i != 0)
            {
                writ.WriteLine();
                writ.WriteLine(unserHeader[i][nomermas][0]);// Подзаголовок
            }
            foreach (CoreC item in core)
            {
                List<string>[] str = new List<string>[tegs[i].Count];
                for (int l = 0; l < str.Length; l++)
                { str[l] = new List<string>(); }
                item.Save(str);
                int j = 0;
                string shell = tegs[i][j];
                Write(writ, "<" + shell, str[j], ">"); // Тег оболочки открывающий
                //j++;
                //string op = i == 0 ? "<" + tegs[i][j] : "<" + tegs[i][j] + ">"; //?
                //string en = i == 0 ? "/>" : "</" + tegs[i][j] + ">"; //? Здесь определяется содержательный или бессодержательный тег
                //Write(writ, op, str[j], en);
                j++;
                for (; j < str.Length; j++)
                { Write(writ, "<" + tegs[i][j] + ">", str[j], "</" + tegs[i][j] + ">"); } // Наполнение
                writ.WriteLine("</" + shell + ">"); // Тег оболочки закрывающий
            }
            if (i != 0)
            { writ.WriteLine(unserHeader[i][nomermas][1]); }
            if (b_end)
            { writ.WriteLine("</" + header[i] + ">"); }
            writ.Close();
        }

        private static void Write(StreamWriter writ, string op, List<string> mstr, string en)
        {
            writ.WriteLine();
            writ.WriteLine(op);
            foreach (string str in mstr)
            { writ.WriteLine(str); }
            writ.WriteLine(en);
        }

        private static void InitializeTegs()
        {
            header = new List<string>() { "Heroes", "Effects", "ITEMS", "EandI", "EandI" };

            tegs = new List<string>[5];
            tegs[0] = new List<string>() { "Hero", "Ephekti", "Eqip", "Inventory" };
            tegs[1] = new List<string>() { "Effect", "Options", "Replacement" };
            tegs[2] = new List<string>() { "Item", "Effects", "Alternative" };
            tegs[3] = tegs[1];
            tegs[4] = tegs[2];
        }
        #endregion

        public static void Load(List<Human> put, ref List<Effekt>[] meff, ref List<Predmet>[] mpred, ref List<Esense>[] mmons)
        {
            unserHeader = new List<string[]>[5];
            unserHeader[3] = new List<string[]>() { new string[] { "<Effects>", "</Effects>" } };
            unserHeader[4] = new List<string[]>() { new string[] { "<Items>", "</Items>" } };
            
            meff = ReadEffekts("profile\\Effects.xml");
            stmeff = meff;
            mpred = ReadPredmets("profile\\Items.xml");
            mmons = ReadCreeps("profile\\Creeps.xml", Memori.modpers, meff, mpred);
            ReadPersons("profile\\Heroes.xml", Memori.modpers, put);
            ReadFormulas("profile\\Formulas.xml");
        }

        public static void ReadTypeArmor(string dmgAdr, string armAdr )
        {
            XmlNodeList masnode = Root(dmgAdr);
            foreach (XmlNode item in masnode)
            { Memori.Armor.Add(item.Attributes[0].Value, null); }
            masnode = Root(armAdr);
            foreach (XmlNode mas in masnode)
            {
                List<string> ls = new List<string>();
                foreach (XmlNode item in mas)
                { ls.Add(item.Attributes[0].Value); }
                Memori.Armor.Add(mas.Attributes[0].Value, ls);
            }
        }

        public static void ReadFormulas(string adres)
        {
            XmlNodeList masnode = Root(adres);
            foreach (XmlNode item in masnode)
            {
                Memori.Formulas.Add( item.Attributes[0].Value, new FormulaConten(item));
            }
        }

        public static void ReadProperty(string adres)
        {
            XmlNodeList masnode = Root(adres);
            XmlNodeList nodes = masnode[0].ChildNodes[0].ChildNodes;
            foreach (XmlNode node in nodes)
            { Memori.Labeltext.Add(node.Attributes[0].Value.Replace(" ", ""), node.InnerText); }
            nodes = masnode[6].ChildNodes;
            int i = 0;
            for (; i < 5; i++)
            { Memori.ModelTable.Add(bool.Parse(nodes[i].InnerText), nodes[i].Attributes[0].Value); }
            for (; i < nodes.Count; i++)
            { Memori.ModelTable.TableHero.Add(nodes[i].InnerText, nodes[i].Attributes[0].Value); }
        }

        private static List<Esense>[] ReadCreeps(string adres, Esense person, List<Effekt>[] meff, List<Predmet>[] mpred)
        {
            XmlNodeList masnode = Root(adres);
            List<Esense>[] mm = new List<Esense>[masnode.Count];
            int i = 0;
            foreach (XmlNode node in masnode)
            {
                mm[i] = new List<Esense>();
                switch (i)
                {
                    case 0:
                        foreach (XmlNode esen in node)
                        {
                            List<string> ms = new List<string>();
                            XmlNode xmlef = esen["Ephekti"];
                            mm[i].Add(new Esense(ReadUnit(esen, ms, person), xmlef, meff));
                            //добавить не стандартную модель, зависимости и регуляция
                        }
                        break;
                    case 1:
                        foreach (XmlNode esen in node)
                        {
                            List<string> ms = new List<string>();
                            XmlNode xmlef = esen["Ephekti"];
                            XmlNode xmleq = esen["Eqip"];
                            mm[i].Add(new Humanoid(ReadUnit(esen, ms, person), xmlef, xmleq, meff, mpred));
                            //добавить определение не стандартных слотов
                        }
                        break;
                    case 2:
                        foreach (XmlNode esen in node)
                        {
                            List<string> ms = new List<string>();
                            XmlNode xmlef = esen["Ephekti"];
                            XmlNode xmleq = esen["Eqip"];
                            XmlNode xmlin = esen["Inventory"];
                            mm[i].Add(new Human(ReadUnit(esen, ms, person), xmlef, xmleq, xmlin, meff, mpred));
                            //добавить определение не стандартных слотов
                        }
                        break;
                }
                i++;
            }
            return mm;
        }

        public static Esense ReadModelPers(string adres)
        {
            XmlNodeList masnode = Root(adres);
            Esense person = new Esense(masnode[0], masnode[1]);//???
            Memori.modpers = person;

            Memori.BaseSloti = Humanoid.XMLSL_Sloti(masnode[2]);

            return person;
        }

        private static Esense ReadUnit(XmlNode essense, List<string> ms, Esense person)
        {
            string[] mkey = person.GetParamrskey();
            for (int i = 0; i < mkey.Length; i++)
            {
                try { ms.Add(essense.Attributes[mkey[i]].Value); }
                catch (Exception e) { ms.Add(null); }
            }
            person.SetParamrsvalue(ms);
            person.CalcConnection();
            return person;
        }

        private static void ReadPersons(string adres, Esense person, List<Human> put)
        {
            List<string> valuelist = new List<string>();
            List<Esense> mess = new List<Esense>();
            XmlNodeList masnode = Root(adres);
            List<int> count = new List<int>();
            foreach (XmlNode hero in masnode)
            {
                mess.Add(new Esense(ReadUnit(hero, valuelist, person), hero.ChildNodes[0]));
                count.Add(hero.ChildNodes[1].ChildNodes.Count);
                valuelist.Clear();
            }
            int i = 0;
            foreach (Esense item in mess)
            {
                put.Add(new Human());
                put[put.Count - 1].Hero(item, count[i], true);
                i++;
            }
        }

        public static Effekt ReadEffekt(XmlNode xmleff)
        {
            Effekt eff = new Effekt();
            eff.name = xmleff.Attributes["Name"].Value;
            XmlAttribute atrb = xmleff.Attributes["Type"];
            if (atrb != null)
            { eff.type = atrb.Value; }
            else { eff.type = xmleff.ParentNode.Attributes[0].Value; }
            atrb = xmleff.Attributes["Specif"];
            if (atrb != null)
            { eff.opis = atrb.Value; }
            else { eff.opis = "НЕТ"; } // Исправить на ключевое слово
            atrb = xmleff.Attributes["TimAct"];
            if (atrb != null)
            { eff.mtime = atrb.Value; }
            else { eff.mtime = "-1"; }
            atrb = xmleff.Attributes["Time"];
            if (atrb != null)
            { eff.time = atrb.Value; }
            else { eff.time = eff.mtime; }
            atrb = xmleff.Attributes["Visible"];
            if (atrb != null)
            { eff.videm = bool.Parse(atrb.Value); }
            else { eff.videm = true; }
            atrb = xmleff.Attributes["Hidden"];
            if (atrb != null)
            { eff.hidden = bool.Parse(atrb.Value); }
            else { eff.hidden = false; }
            atrb = xmleff.Attributes["Period"];
            if (atrb != null)
            { eff.tikaet = bool.Parse(atrb.Value); }
            else { eff.tikaet = false; }
            if (eff.tikaet)
            {
                atrb = xmleff.Attributes["Tiki"];
                string[] ms;
                if (atrb != null)
                {
                    if (atrb.Value.Contains("ink"))
                    {
                        ms = Effekt.ParsTik(atrb.Value, eff.mtime);
                    }
                    else
                    {
                        string str = atrb.Value.Replace(" ", "");
                        ms = str.Split(',');
                    }
                }
                else { ms = Effekt.ParsTik("ink[1]", eff.mtime); }
                eff.tiki = ms;
            }

            if (xmleff.ChildNodes.Count > 0)
            {
                XmlNode opt = xmleff["Options"];
                if (opt != null)
                {
                    foreach (XmlNode item in opt)
                    {
                        eff.haracter.Add(item.Attributes[0].Value, item.InnerText.Replace(" ", ""));
                        eff.harK_Kusl.Add(item.Attributes[0].Value, "0");
                    }
                }
                opt = xmleff["Replacement"];
                if (opt != null)
                {
                    foreach (XmlNode item in opt)
                    {
                        int i = -1;
                        atrb = item.Attributes["SType"];
                        if (atrb != null)
                        {
                            if (!int.TryParse(atrb.Value, out i))
                            { i = Memori.VidEffekt.IndexOf(atrb.Value); }
                        }
                        eff.zamenam.Add(item.Attributes[0].Value, i);
                    }
                }
            }
            return eff;
        }

        private static List<Effekt>[] ReadEffekts(string adres)
        {
            XmlNodeList masnode = Root(adres);
            unserHeader[1] = new List<string[]>();
            foreach (XmlNode item in masnode)
            {
                Memori.VidEffekt.Add(item.Attributes[0].Value);
                unserHeader[1].Add(new string[] 
                { "<" + item.Name + " Name = \"" + item.Attributes[0].Value + "\">",
                "</" + item.Name + ">" });
            }
            
            List<Effekt>[] meff = new List<Effekt>[masnode.Count + 1];
            int i = 0;
            foreach (XmlNode items in masnode)
            {
                meff[i] = new List<Effekt>();
                foreach (XmlNode item in items)
                { meff[i].Add(ReadEffekt(item)); }
                i++;
            }
            meff[i] = new List<Effekt>();
            return meff;
        }

        public static Predmet ReadPredmet(XmlNode xmlpred)
        {
            Predmet pred = new Predmet();
            XmlAttribute atrb = xmlpred.Attributes["Damage"];
            if (atrb != null)
            {
                atrb = xmlpred.Attributes["Range"];
                if (atrb != null)
                { pred = new Dalwaffe(); }
                else { pred = new Waffe(); }
            }
            else
            {
                atrb = xmlpred.Attributes["Shield"];
                if (atrb != null)
                { pred = new Schutz(); }
            }
            pred.Load(xmlpred, stmeff);
            return pred;
        }

        private static List<Predmet>[] ReadPredmets(string adres)
        {
            XmlNodeList masnode = Root(adres);
            unserHeader[2] = new List<string[]>();
            foreach (XmlNode item in masnode)
            {
                Memori.VidPredmet.Add(item.Attributes[0].Value);
                unserHeader[2].Add(new string[] 
                { "<" + item.Name + " Name = \"" + item.Attributes[0].Value + "\">",
                "</" + item.Name + ">" });
            }

            List<Predmet>[] mpred = new List<Predmet>[masnode.Count + 1];
            int i = 0;
            foreach (XmlNode items in masnode)
            {
                mpred[i] = new List<Predmet>();
                foreach (XmlNode item in items)
                { mpred[i].Add(ReadPredmet(item)); }
                i++;
            }
            mpred[i] = new List<Predmet>();
            return mpred;
        }

        public static XmlNodeList Root(string adres)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(adres);
            return xmldoc.ChildNodes[1].ChildNodes;
        }

        public static void ReadTables(string adres)
        {
            XmlNodeList xmltabls = Root(adres);
            foreach (XmlNode xmltabl in xmltabls)
            {
                Dictionary<string, string> tabl = new Dictionary<string, string>();
                foreach (XmlNode item in xmltabl)
                { tabl.Add(item.Attributes[0].Value, item.Attributes[1].Value); }
                Memori.Tables.Add(xmltabl.Attributes[0].Value, tabl);
            }
        }
    }

    //
}
