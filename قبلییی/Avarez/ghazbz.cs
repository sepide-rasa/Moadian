using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace Avarez
{
    public class ghabz
    {
        string _shghabz, _shpardakht;
        public ghabz(int parvande, int codeanformatik, int codekhedmat, int mablagh, int sal, int dore)
        {
            codeanformatik = codeanformatik % 1000;
            codekhedmat = codekhedmat % 10;
            sal = sal % 10;
            dore = dore % 100;
            mablagh = mablagh / 1000;
            parvande = parvande % 100000000;
            _shghabz = string.Format("{0}{1:000}{2}", parvande, codeanformatik, codekhedmat);
            int k = GetControl(_shghabz);
            _shghabz = _shghabz + k.ToString();
            _shpardakht = string.Format("{0}{1}{2:00}", mablagh, sal, dore);
            k = GetControl(_shpardakht);
            _shpardakht = _shpardakht + k.ToString();
            string temp = _shghabz + _shpardakht;
            k = GetControl(temp);
            _shpardakht = _shpardakht + k.ToString();
            string temp1 = Reverse(_shghabz);
            int l = temp1.Length;
            for (int i = l; i < 13; i++)
                temp1 = temp1 + "0";
            string temp2 = Reverse(_shpardakht);
            l = temp2.Length;
            for (int i = l; i < 13; i++)
                temp2 = temp2 + "0";
            string strbar = Reverse(temp1) + Reverse(temp2);

        }

        public string ShGhabz
        {
            get
            {
                return _shghabz;
            }
        }

        public string BarcodeText
        {
            get
            {
                return BarcodeShenase(_shghabz) + BarcodeShenase(_shpardakht);
            }
        }

        private string BarcodeShenase(string s)
        {
            for (int i = s.Length + 1; i <= 13; i++)
            {
                s = "0" + s;
            }
            return s;
        }

        public string ShPardakht
        {
            get
            {
                return _shpardakht;
            }
        }

        private int GetControl(string str)
        {
            string sr = Reverse(str);
            int[] a = new int[] { 2, 3, 4, 5, 6, 7 };
            int l, i, k, d, s = 0;
            l = str.Length;
            for (i = 0; i < l; i++)
            {
                k = a[i % 6];
                d = Convert.ToInt32(sr[i]) - 48;
                s = s + k * d;

            }
            int f = s % 11;
            if (f == 0 || f == 1)
            {
                s = 0;

            }
            else
            {
                s = 11 - (f);
            }
            return s;

        }

        private string Reverse(string s)
        {
            string temp = "";
            int l = s.Length, k = 0;
            for (int i = l - 1; i >= 0; i--)
            {
                temp = temp + s[i];
                k++;
            }
            return temp;
        }
    }
}
