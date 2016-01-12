namespace BoxMistr
{
    public class objednavky
    {
        public bool Expoted { get; set; }
        public string CObjednavky { get; set; }
        public string Cena { get; set; }
        public string Dobirka { get; set; }
        public string Firma { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public string Adresa { get; set; }
        public string PSC { get; set; }
        public string Mesto { get; set; }
        public string VSymbol { get; set; }
        public string Telefon { get; set; }
        public string Emal { get; set; }
        public string Vaha { get; internal set; }
        public string Sluzby { get; internal set; }
        public string Doprava { get; internal set; }
        public string Vzkaz { get; internal set; }
        public bool Stav { get; set; }

        public string minus420(string telefon)
        {
            if (telefon.Substring(0, 3) == "420")
                telefon = telefon.Substring(3, telefon.Length - 3);
            else if (telefon.Substring(0, 4) == "+420")
                telefon = telefon.Substring(4, telefon.Length - 4);

            return telefon;
        }

        public string[] NameSurname(string readedData)
        {
            string[] OutData = new string[2];
            OutData[0] = readedData.Substring(0, readedData.IndexOf(" "));
            OutData[1] = readedData.Substring(readedData.IndexOf(" "), readedData.Length - readedData.IndexOf(" ")).Trim();
            if (OutData[1].IndexOf(',') > 0)
                OutData[1] = OutData[1].Substring(0, OutData[1].IndexOf(','));
            return OutData;
        }

        public string UlozenkaCode(string Ulozenka)
        {
            if (Ulozenka.Contains("Uloženka"))
            {
                Ulozenka = Ulozenka.Substring(Ulozenka.IndexOf('[') + 1, (Ulozenka.LastIndexOf(']') - Ulozenka.IndexOf('[') - 1));
            }
            return Ulozenka;
        }

    }

}