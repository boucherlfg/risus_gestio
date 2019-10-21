using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace risus_gestio
{
    class Program
    {
        static GWrap g;
        static void Main(string[] args)
        {
            g = new GWrap();
            while(true)
            {
                Console.Write(" > ");
                g.Switch(Console.ReadLine());
            }
        }
    }
    
    class GWrap
    {
        private GESTIO g;
        public GWrap()
        {
            g = new GESTIO();
        }
        private string Command(string line)
        {
            string sw = line.Split(' ').First();
            string arg = string.Join(" ", line.Split(' ').Skip(1));

            switch (sw)
            {
                case "add":
                    return Add(arg);
                case "del":
                    return Del(arg);
                case "has":
                    return Has(arg);
                case "get":
                    return Get(arg);
                case "select":
                    return Select(arg);
                default:
                    throw new Exception("invalid command");
            }
        }
        public void Switch(string line)
        {
            foreach(string or in line.Split('|'))
            {
                foreach (string and in or.Split('&'))
                {
                    try
                    {
                        Console.WriteLine(Command(and.Trim()));
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                }
            }
        }

        private string Add(string line)
        {
            g.AddELEM(line.Split(' ')[0], line.Split(' ')[1], line.Split(' ')[2]);
            return "element added successfully";
        }
        private string Del(string line)
        {
            if (g.DelELEM(line.Split(' ')[0], line.Split(' ')[1], line.Split(' ')[2]))
            {
                return "element deleted successfully";
            }
            else throw new Exception("couldn't delete element");
        }
        private string Has(string line)
        {
            line = line.Replace("*", "");
            return "" + g.ContainELEM(line.Split(' ')[0], line.Split(' ')[1], line.Split(' ')[2]);
        }
        private string Get(string line)
        {
            return g.GetELEM(line.Split(' ')[0], line.Split(' ')[1], line.Split(' ')[2]);
        }
        private string Select(string line)
        {
            line = line.Replace("*", "");
            return g.SelectElem(line.Split(' ')[0], line.Split(' ')[1], line.Split(' ')[2]);
        }
        private string Help()
        {
            return "add del has get select help";
        }
    }
    class GESTIO
    {
        private static Random r = new Random();
        private List<ELEM> elem;

        public GESTIO()
        {
            elem = new List<ELEM>();
        }
        public static implicit operator GESTIO(string line)
        {
            GESTIO g = new GESTIO();
            while (line.Contains("</elem>"))
            {
                g.elem.Add(line.Substring(line.IndexOf("<elem>") + "<elem>".Length).Substring(0, line.IndexOf("</elem>")));
                line = line.Substring(line.IndexOf("</elem>") + "</elem>".Length);
            }
            return g;
        }
        public override string ToString()
        {
            string ret = "";
            elem.ForEach(x =>
            {
                ret += "<elem>" + x.ToString() + "</elem>\n";
            });
            return ret.Trim();
        }


        public string SelectElem(string type, string id, string value)
        {
            GESTIO g = new GESTIO();
            g.elem = elem.FindAll(x => x.GetELEMType().Contains(type));
            g.elem = g.elem.FindAll(x => x.GetELEMId().Contains(id));
            g.elem = g.elem.FindAll(x => x.GetELEMValue().Contains(value));
            return g.ToString();
        }
        public string GetELEM(string type, string id, string value)
        {
            try
            {
                return elem.Find(x => x.GetELEMType() == type && x.GetELEMId() == id && x.GetELEMValue() == value).ToString();
            }
            catch
            {
                return "";
            }
        }
        public bool ContainELEM(string type, string id, string value)
        {
            return elem.Exists(x => x.GetELEMType().Contains(type) && x.GetELEMId().Contains(id) && x.GetELEMValue().Contains(value));
        }
        public void AddELEM(string type, string id, string value)
        {
            elem.Add(new ELEM(type, id, value));
        }
        public bool DelELEM(string type, string id, string value)
        {
            if(elem.Exists(x => x.GetELEMType() == type && x.GetELEMId() == id && x.GetELEMValue() == value))
            {
                elem.RemoveAll(x => x.GetELEMType() == type && x.GetELEMId() == id && x.GetELEMValue() == value);
                return true;
            }
            return false;
        }
    }
    class ELEM
    {
        private string type;
        private string id;
        private string value;

        public ELEM() { }
        public ELEM(string type, string id, string value)
        {
            this.type = type;
            this.id = id;
            this.value = value;
        }

        public string GetELEMType()
        {
            return type;
        }
        public string GetELEMId()
        {
            return id;
        }
        public string GetELEMValue()
        {
            return value;
        }

        public static implicit operator ELEM(string line)
        {
            ELEM ret = new ELEM();
            ret.type = line.Substring(line.IndexOf("<type>") + "<type>".Length).Substring(0, line.IndexOf("</type>"));
            ret.id = line.Substring(line.IndexOf("<id>") + "<id>".Length).Substring(0, line.IndexOf("</id>"));
            ret.value = line.Substring(line.IndexOf("<value>") + "<value>".Length).Substring(0, line.IndexOf("</value>"));
            return ret;
        }
        public override string ToString()
        {
            return "<type>" + type + "</type><id>" + id + "</id><value>" + value + "</value>";
        }
    }
}
