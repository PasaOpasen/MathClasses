using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace МатКлассы
{
    /// <summary>
    /// Класс кодирования-декодирования
    /// </summary>
    public static class Coding
    {
        static Coding()
        {
            TempList = new List<string>();
            HofTable = new Turn();
            DoubleCharTable = new List<Tuple<double, char>>();
        }

        /// <summary>
        /// Набор промежуточной информации
        /// </summary>
        public static List<string> TempList = new List<string>();

        /// <summary>
        /// Таблица сопоставления символам двоичных чисел
        /// </summary>
        public static Turn HofTable;

        /// <summary>
        /// Набор возможных символов
        /// </summary>
        public static string abc = "1234567890-+qwertyuiopasdfghjkl; zxcvbnm,.йцукенгшщзхъфывапролджэячсмитьбюЦУКЕНГШЩЗФВАПРОЛДЯЧСМИТБЮХЖЭQWERTYUIOPASDFGHJKLZXCVBNM";
        //public static string abc = "1234567890-+=qwertyuiopasdfghjkl; zxcvbnm,.ёйцукенгшщзхъфывапролджэячсмитьбюЁЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬБЮХЪЖЭQWERTYUIOPASDFGHJKLZXCVBNM";

        /// <summary>
        /// Рандомизированный алфавит
        /// </summary>
        public static string randabc;
        /// <summary>
        /// Таблица для шифра Полибия
        /// </summary>
        public static char[,] TablePolib;
        /// <summary>
        /// Таблица для шифра Плейфера
        /// </summary>
        public static char[,] TablePlay;
        /// <summary>
        /// Таблица для маршрутной перестановки
        /// </summary>
        public static char[,] TableVert;
        /// <summary>
        /// Массив принятых частот символов
        /// </summary>
        private static CharFreq[] FreqMas = new CharFreq[34];

        /// <summary>
        /// Попытка взлома по частоте символов
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Hacking(string s)
        {
            //массив принятых частот (отсортированный в порядке убывания)
            FreqMas[1] = new CharFreq('о', 0.10983);
            FreqMas[2] = new CharFreq('е', 0.08483);
            FreqMas[3] = new CharFreq('а', 0.07998);
            FreqMas[4] = new CharFreq('и', 0.07367);
            FreqMas[5] = new CharFreq('н', 0.067);
            FreqMas[6] = new CharFreq('т', 0.06318);
            FreqMas[7] = new CharFreq('с', 0.05473);
            FreqMas[8] = new CharFreq('р', 0.04746);
            FreqMas[9] = new CharFreq('в', 0.04533);
            FreqMas[10] = new CharFreq('л', 0.04343);
            FreqMas[11] = new CharFreq('к', 0.03486);
            FreqMas[12] = new CharFreq('м', 0.03203);
            FreqMas[13] = new CharFreq('д', 0.02977);
            FreqMas[14] = new CharFreq('п', 0.02804);
            FreqMas[15] = new CharFreq('у', 0.02615);
            FreqMas[16] = new CharFreq('я', 0.02001);
            FreqMas[17] = new CharFreq('ы', 0.01898);
            FreqMas[18] = new CharFreq('ь', 0.01735);
            FreqMas[19] = new CharFreq('г', 0.01687);
            FreqMas[20] = new CharFreq('з', 0.01641);
            FreqMas[21] = new CharFreq('б', 0.01592);
            FreqMas[22] = new CharFreq('ч', 0.0145);
            FreqMas[23] = new CharFreq('й', 0.01208);
            FreqMas[24] = new CharFreq('х', 0.00966);
            FreqMas[25] = new CharFreq('ж', 0.0094);
            FreqMas[26] = new CharFreq('ш', 0.00718);
            FreqMas[27] = new CharFreq('ю', 0.00639);
            FreqMas[28] = new CharFreq('ц', 0.00486);
            FreqMas[29] = new CharFreq('щ', 0.00361);
            FreqMas[30] = new CharFreq('э', 0.00331);
            FreqMas[31] = new CharFreq('ф', 0.00267);
            FreqMas[32] = new CharFreq('ъ', 0.00037);
            FreqMas[33] = new CharFreq('ё', 0.00013);


            s = new string(s.Where(n => n != ' ' && n != ',' && n != '.' && n != ';' && n != ':' && n != '(' && n != ')' && n != '!' && n != '?' && n != '-' && n != '—' && n != '…' && n != '\n' && n != '\t').ToArray()).ToLower();//убрать из строки все пробелы, точки, запятые, все буквы заменить на маленькие
            TempList.Add($"Исправленная исходная строка: {s}");

            var smas = CharFreq.GetMas(s);
            //smas=smas.Reverse().ToArray();//реверсировать массив (чтоб было по убыванию)

            TempList.Add("----------------Сортированный массив частот в тексте:");
            for (int i = 0; i < smas.Length; i++)
                TempList.Add($"\tsmas[{i}] = {smas[i].simbol} \t{smas[i].frequency}");

            string tmp = "";
            for (int i = 0; i < smas.Length; i++)//перевести отсортированный массив в строку
                tmp += smas[i].simbol;
            tmp = new string(tmp.Reverse().ToArray());//tmp.Show();
            TempList.Add("----------------Строка символов текста по убыванию частоты: " + tmp);

            TempList.Add($"----------------Перевод символов строки:");
            string res = "";
            for (int i = 0; i < s.Length; i++)//перевести каждый символ в строке, зная частоту
                try
                {
                    int k = tmp.IndexOf(s[i]);
                    res += FreqMas[k + 1].simbol;
                    TempList.Add($"\ts[{i}] = {s[i]} \t(частота {smas[smas.Length - 1 - k].frequency}) \t------> \t{FreqMas[k + 1].simbol} (принятая частота {FreqMas[k + 1].frequency})");
                }
                catch { throw new Exception("Походу неизвестный символ"); }

            return res;
        }

        private class CharFreq : IComparable
        {
            public char simbol;
            public double frequency = 0;
            public CharFreq(char c) { simbol = c; }
            public CharFreq(char c, double v) : this(c) { frequency = v; }

            public int CompareTo(object obj)
            {
                CharFreq t = (CharFreq)obj;
                return frequency.CompareTo(t.frequency);
            }

            public static CharFreq[] GetMas(string s)
            {
                var mas = s.Distinct().ToArray();//взять массив разных символов
                CharFreq[] smas = new CharFreq[mas.Length];
                for (int i = 0; i < mas.Length; i++)//перевести массив символов в массив объектов класса
                {
                    smas[i] = new CharFreq(mas[i]);
                    double c = 0;
                    for (int j = 0; j < s.Length; j++)
                        if (s[j] == mas[i])
                            c++;
                    smas[i].frequency = c / s.Length;
                }
                Array.Sort(smas);//сортировать по частоте
                return smas;
            }
        }

        /// <summary>
        /// Шифр цезаря
        /// </summary>
        /// <param name="s"></param>
        /// <param name="key"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string Zesar(string s, bool encode = true, int key = 4)
        {
            TempList.Add($"--------------Исходный алфавит: {abc}");
            TempList.Add($"--------------Ключ: {key}");

            string res = "";
            for (int i = 0; i < s.Length; i++)
            {
                int k = abc.IndexOf(s[i]);
                if (k >= 0)
                {
                    k += (encode) ? key : -key;
                    if (k > abc.Length) k -= abc.Length;
                    if (k < 0) k += abc.Length;
                    res += abc[k];
                }
            }
            return res;
        }

        private static void GenerateRandabc()
        {
            randabc = new string(abc.ToArray());
            for (int i = 0; i < abc.Length; i++)
            {
                Random r = new Random();
                int a = (r.Next(abc.Length) + i) % abc.Length;
                //r = new Random();
                int b = r.Next(i) % abc.Length;
                // a.Show();b.Show();"".Show();
                randabc = randabc.Swap(randabc[a], randabc[b]);
            }
        }
        /// <summary>
        /// Простая замена
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Simple(string s, bool encode = true, string key = "")
        {
            //если ключ нулевой и нужна кодировка, сгенерировать случайный ключ
            if (key.Length == 0)
                if (encode)
                {
                    GenerateRandabc();
                    key = randabc;
                    //key.Show();
                }
                else//если нужна декодировка, просто присвоить ключу сгенерированный алфавит
                {
                    key = randabc;
                    //key.Show();
                }
            //abc.Show();
            TempList.Add($"--------------Ключ: {key}");

            string res = "";
            if (encode)
                for (int i = 0; i < s.Length; i++)
                {
                    int k = abc.IndexOf(s[i]);
                    if (k >= 0) res += key[k];
                }
            else
                for (int i = 0; i < s.Length; i++)
                {
                    int k = key.IndexOf(s[i]);
                    if (k >= 0) res += abc[k];
                }

            return res;
        }

        private static void GenerateTable(string key)
        {
            TablePolib = new char[19, 10];//таблица -9 -8 -7 ... -1 0 1 ... 9 на 0 ... 9
            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 10; j++)
                    if (i * 10 + j < abc.Length)
                        TablePolib[i, j] = key[i * 10 + j];

        }
        private static Point IndexOf(char[,] table, char k)
        {
            for (int i = 0; i < table.GetLength(0); i++)
                for (int j = 0; j < table.GetLength(1); j++)
                    if (table[i, j] == k)
                        return new Point(i, j);
            return new Point(0, 0);
        }
        /// <summary>
        /// Шифр Полибия
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Polib(string s, bool encode = true, string key = "")
        {
            //если ключ нулевой и нужна кодировка, сгенерировать случайный ключ
            if (key.Length == 0)
                if (encode)
                {
                    GenerateRandabc();
                    key = randabc;
                    //key.Show();
                }
                else//если нужна декодировка, просто присвоить ключу сгенерированный алфавит
                {
                    key = randabc;
                    //key.Show();
                }
            TempList.Add($"--------------Ключ: {key}");
            GenerateTable(key);//Сгенерировать таблицу по ключу

            TempList.Add($"--------------Таблица шифра Полибия:");
            for (int i = 0; i < 19; i++)
            {
                string tmp = "";
                for (int j = 0; j < 10; j++)
                    tmp += TablePolib[i, j].ToString();
                TempList.Add(tmp);
            }

            string res = "";
            if (encode)//кодировка
                for (int i = 0; i < s.Length; i++)
                {
                    Point p = IndexOf(TablePolib, s[i]);
                    res += (p.x - 9).ToString() + p.y.ToString() + ' ';
                }
            else//декодировка
            {
                string[] mas = s.Split(' ');
                mas = mas.Where(n => n.Length > 0).ToArray();
                //for (int i = 0; i < mas.Length; i++) Console.Write(mas[i] + "   ");
                for (int i = 0; i < mas.Length; i++)
                    if (mas[i].Length == 2)
                    {
                        int a = Convert.ToInt32(mas[i][0].ToString()), b = Convert.ToInt32(mas[i][1].ToString());
                        res += TablePolib[a + 9, b];
                        //res.Show();
                    }
                    else//=3
                    {
                        int a = Convert.ToInt32(mas[i][1].ToString()), b = Convert.ToInt32(mas[i][2].ToString());
                        res += TablePolib[-a + 9, b];
                        //res.Show();
                    }
            }
            return res;
        }

        private static void GenerateTablePlay(string key)
        {
            TablePlay = new char[(int)Math.Ceiling((decimal)abc.Length / key.Length), key.Length];
            int l = key.Length;
            key += new string(abc.Where(n => key.IndexOf(n) < 0).ToArray());//key.Show();
            for (int i = 0; i < (int)Math.Ceiling((decimal)abc.Length / l); i++)
                for (int j = 0; j < l; j++)
                    if (i * l + j < key.Length) TablePlay[i, j] = key[i * l + j];
                    else TablePlay[i, j] = '`';

        }
        /// <summary>
        /// Шифр Плейфера
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Playfer(string s, bool encode = true, string key = "abcd")
        {
            key = new string(key.Distinct().ToArray());//убрать из ключа повторы
            if (key.Length == 0) key = "abcd";//страховка при нулeвом ключе
                                              //key.Show();
            TempList.Add($"--------------Ключ: {key}");
            GenerateTablePlay(key);//сгенерировать таблицу по ключу
            TempList.Add("--------------Таблица шифра Плейфера");
            for (int i = 0; i < TablePlay.GetLength(0); i++)
            {
                string tmp = "";
                for (int j = 0; j < TablePlay.GetLength(1); j++)
                    tmp += TablePlay[i, j].ToString();
                TempList.Add(tmp);
            }

            string res = "";
            if (encode)
            {
                for (int i = 0; i < s.Length - 1; i++)
                    if (s[i] == s[i + 1] && i % 2 == 0)
                        s = s.Insert(i + 1, "+");//вставить особый символ между повторами символов в строке
                if (s.Length % 2 != 0) s += "+";//если не хватает пар, добавить символ в конец
                                                //s.Show();
                TempList.Add($"--------------Исправленные исходный текст: {s}");

                for (int i = 0; i < s.Length; i += 2)
                {
                    Point a = IndexOf(TablePlay, s[i]);
                    Point b = IndexOf(TablePlay, s[i + 1]);
                    if (a.x == b.x)
                    {
                        try { res += TablePlay[(int)a.x, (int)a.y + 1]; }
                        catch { res += TablePlay[(int)a.x, 0]; }
                        try { res += TablePlay[(int)b.x, (int)b.y + 1]; }
                        catch { res += TablePlay[(int)b.x, 0]; }
                    }
                    else if (a.y == b.y)
                    {
                        try { res += TablePlay[(int)a.x + 1, (int)a.y]; }
                        catch { res += TablePlay[0, (int)a.y]; }
                        try { res += TablePlay[(int)b.x + 1, (int)b.y]; }
                        catch { res += TablePlay[0, (int)b.y]; }
                    }
                    else
                    {
                        res += TablePlay[(int)a.x, (int)b.y];
                        res += TablePlay[(int)b.x, (int)a.y];
                    }
                }
            }

            else
            {
                for (int i = 0; i < s.Length; i += 2)
                {
                    Point a = IndexOf(TablePlay, s[i]);
                    Point b = IndexOf(TablePlay, s[i + 1]);
                    if (a.x == b.x)
                    {
                        try { res += TablePlay[(int)a.x, (int)a.y - 1]; }
                        catch { res += TablePlay[(int)a.x, TablePlay.GetLength(1) - 1]; }
                        try { res += TablePlay[(int)b.x, (int)b.y - 1]; }
                        catch { res += TablePlay[(int)b.x, TablePlay.GetLength(1) - 1]; }
                    }
                    else if (a.y == b.y)
                    {
                        try { res += TablePlay[(int)a.x - 1, (int)a.y]; }
                        catch { res += TablePlay[TablePlay.GetLength(0) - 1, (int)a.y]; }
                        try { res += TablePlay[(int)b.x - 1, (int)b.y]; }
                        catch { res += TablePlay[TablePlay.GetLength(0) - 1, (int)b.y]; }
                    }
                    else
                    {
                        res += TablePlay[(int)a.x, (int)b.y];
                        res += TablePlay[(int)b.x, (int)a.y];
                    }
                }
                res = new string(res.ToCharArray().Where(n => n != '+').ToArray());
            }

            return res;
        }

        private static string UseKey(string key, bool end = false)
        {
            //перевести строку в массив чисел
            int[] mas = new int[key.Length];
            for (int i = 0; i < mas.Length; i++)
                mas[i] = Convert.ToInt32(key[i].ToString()) - 1;

            //заполнить новую таблицу по ключу
            char[,] table = new char[TableVert.GetLength(0), TableVert.GetLength(1)];
            for (int j = 0; j < mas.Length; j++)
                for (int i = 0; i < TableVert.GetLength(0); i++)
                    table[i, mas[j]] = TableVert[i, j];

            //перевести таблицу в фразу
            string res = "";
            for (int i = 0; i < TableVert.GetLength(0); i++)
                for (int j = 0; j < TableVert.GetLength(1); j++)
                    res += table[i, j];

            if (end) return new string(res.Where(n => n != '`').ToArray());
            return res;
        }
        private static string ModKey(string key)
        {
            char[] res = new char[key.Length];
            for (int i = 0; i < key.Length; i++)
            {
                int tmp = Convert.ToInt32(key[i].ToString()) - 1;
                res[tmp] = (i + 1).ToString()[0];
            }
            //res.Show();
            return new string(res);
        }
        /// <summary>
        /// Маршрутная (вертикальная) перестановка
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Vert(string s, bool encode = true, string key = "34125")
        {
            //if (key.Length == 0) key = new string(randabc.ToCharArray());
            TableVert = new char[(int)Math.Ceiling((decimal)s.Length / key.Length), key.Length];
            for (int i = 0; i < (int)Math.Ceiling((decimal)s.Length / key.Length); i++)
                for (int j = 0; j < key.Length; j++)
                    if (i * key.Length + j < s.Length) TableVert[i, j] = s[i * key.Length + j];
                    else TableVert[i, j] = '`';//сгенерировать таблицу по фразе

            TempList.Add($"--------------Таблица маршрутной перестановки");
            for (int i = 0; i < TableVert.GetLength(0); i++)
            {
                string tmp = "";
                for (int j = 0; j < TableVert.GetLength(1); j++)
                    tmp += TableVert[i, j];
                TempList.Add(tmp);
            }

            if (encode)
                return UseKey(key, false);//кодировать в другую фразу

            string k = ModKey(key);//иначе изменить ключ и декодировать
            return UseKey(k, true);
        }


        public class Turn : IComparable
        {
            public List<Tuple<char, string>> SymbolList;
            public double value;
            public Turn()
            {
                this.SymbolList = null;
                this.value = 0;
            }
            public Turn(char c, string s, double v)
            {
                this.SymbolList = new List<Tuple<char, string>>();
                SymbolList.Add(new Tuple<char, string>(c, s));
                this.value = v;
            }
            public int CompareTo(object obj)
            {
                Turn t = (Turn)obj;
                return t.value.CompareTo(this.value);

            }
            public int Count => SymbolList.Count;

            private void AddInString(int a, int b)
            {
                for (int i = 0; i < a; i++)
                    this.SymbolList[i] = new Tuple<char, string>(this.SymbolList[i].Item1, String.Concat("0", this.SymbolList[i].Item2));
                for (int i = a; i < a + b; i++)
                    this.SymbolList[i] = new Tuple<char, string>(this.SymbolList[i].Item1, String.Concat("1", this.SymbolList[i].Item2));
            }
            public static Turn operator +(Turn t1, Turn t2)
            {
                List<Tuple<char, string>> list = new List<Tuple<char, string>>();
                list.AddRange(t1.SymbolList);
                list.AddRange(t2.SymbolList);
                Turn res = new Turn();
                res.SymbolList = list;
                res.value = t1.value + t2.value;
                res.AddInString(t1.SymbolList.Count, t2.SymbolList.Count);
                return res;
            }
        }

        /// <summary>
        /// Таблица сопоставления действительным числам из кодировки символа Unicode
        /// </summary>
        public static List<Tuple<double, char>> DoubleCharTable = new List<Tuple<double, char>>();
        /// <summary>
        /// Перевод массива чисел в строку символов Unicode (поддерживает около 130 000 разных символов) c параллельным заполнением таблицы соответствия
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static string DoubleMasToUnicodeString(double[] mas)
        {
            var tmas = mas.Distinct().ToArray();//убрать из массива повторы
            Array.Sort(tmas);
            string res = "";
            //заполнить таблицу
            DoubleCharTable = new List<Tuple<double, char>>();

            int y = 0;
            for (int i = 0; i < tmas.Length; i++)
            {
                char tmp = Convert.ToChar(y++);
                while (/*tmp.Equals(null)||Char.IsSeparator(tmp)||Char.IsWhiteSpace(tmp)*/!Char.IsLetterOrDigit(tmp)) tmp = Convert.ToChar(y++);
                DoubleCharTable.Add(new Tuple<double, char>(tmas[i], tmp));
            }


            //записать результат
            for (int i = 0; i < mas.Length; i++)
            {
                int k = Array.IndexOf(tmas, mas[i]);
                res += DoubleCharTable[k].Item2;
            }

            return res;
        }
        /// <summary>
        /// Перевод строки в массив чисел с помощью таблицы соответствия
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double[] StringToDoubleMas(string s)
        {
            double[] res = new double[s.Length];
            //взять из таблицы массив символов
            char[] mas = new char[DoubleCharTable.Count];
            for (int i = 0; i < mas.Length; i++) mas[i] = DoubleCharTable[i].Item2;
            //записать результат
            for (int i = 0; i < res.Length; i++)
            {
                int k = Array.IndexOf(mas, s[i]);
                res[i] = DoubleCharTable[k].Item1;
            }
            return res;
        }
        ///// <summary>
        ///// Сортировщик очереди
        ///// </summary>
        ///// <param name="s"></param>
        ///// <param name="r"></param>
        ///// <returns></returns>
        //private static int comparer(this Tuple<List<char>, string, double> s, Tuple<List<char>, string, double> r)
        //{
        //    return s.Item3.CompareTo(r.Item3);
        //}
        /// <summary>
        /// Кодирование по алгоритму Хоффмана
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string Hoffman(string s, bool encode = true)
        {
            string res = "";

            if (encode)
            {
                var constmas = CharFreq.GetMas(s);//набор символов с частотой
                constmas.Reverse();
                var mas = new List<Turn>();//список пар нескольких символов и их суммарной частоты
                TempList.Add("-----Исходный набор символов:");
                for (int i = 0; i < constmas.Length; i++)
                {
                    mas.Add(new Turn(constmas[i].simbol, "", constmas[i].frequency));
                    TempList.Add($"symbol[{i}] = {constmas[i].simbol} \t{constmas[i].frequency}");
                }

                //генерируется таблица
                while (mas.Count > 1)
                {
                    mas.Sort();
                    mas.Reverse();
                    for (int i = 0; i < mas.Count; i++)
                    {
                        string t = "";
                        for (int j = 0; j < mas[i].SymbolList.Count; j++)
                            t += $"{mas[i].SymbolList[j].Item1}({mas[i].SymbolList[j].Item2})";
                        TempList.Add($"{t} \t{mas[i].value}");
                    }

                    mas[0] += mas[1];
                    mas.RemoveAt(1);
                    TempList.Add("");
                    //вдруг ускорит поиск
                    mas.Sort();
                    mas.Reverse();
                }

                HofTable = mas[0];

                //генерируется вывод
                string tmp = "";
                for (int i = 0; i < HofTable.Count; i++)
                    tmp += HofTable.SymbolList[i].Item1;
                for (int i = 0; i < s.Length; i++)
                    res += HofTable.SymbolList[tmp.IndexOf(s[i])].Item2 + " ";
                //res += HofTable.SymbolList[Array.BinarySearch(tmp.ToArray(),s[i])].Item2 + " ";
            }
            else
            {
                //генерируется вывод
                var tmp = s.Split(' ').Where(n => n.Length > 0).ToArray();
                TempList.Add($"Декодируемая строка без пробелов:");
                for (int i = 0; i < tmp.Length; i++)
                {
                    TempList.Add($"tmp[{i}] = {tmp[i]}");
                    for (int j = 0; j < HofTable.Count; j++)
                        if (tmp[i] == HofTable.SymbolList[j].Item2)
                        {
                            res += HofTable.SymbolList[j].Item1;
                            break;
                        }
                }

            }
            return res;
        }
        /// <summary>
        /// Кодировани набора чисел в набор двоичных чисел в два этапа (массив чисел -> строка символов -> строка двоичных чисел)
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static string HoffmanNumberEncode(double[] mas) => Hoffman(DoubleMasToUnicodeString(mas), true);
        /// <summary>
        /// Перевод набора двоичных чисел в массив строк в два этапа
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double[] HoffmanNunderDecode(string s) => StringToDoubleMas(Hoffman(s, false));

        static char[] characters => abc.ToArray();
        //= new char[] { 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И',
        //                                            'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С',
        //                                            'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ь', 'Ы', 'Ъ',
        //                                            'Э', 'Ю', 'Я', ' ', '1', '2', '3', '4', '5', '6', '7',
        //                                            '8', '9', '0' };

        private static int N => characters.Length; //длина алфавита
                                                   /// <summary>
                                                   /// Кодирование шифром Виженера
                                                   /// </summary>
                                                   /// <param name="input"></param>
                                                   /// <param name="keyword"></param>
                                                   /// <returns></returns>
        public static string VigenereEncode(string input, string keyword)
        {
            //input = input.ToUpper();
            //keyword = keyword.ToUpper();

            string result = "";

            int keyword_index = 0, i = 1;
            TempList.Add($"Ключ = {keyword}");
            foreach (char symbol in input)
            {
                int c = (Array.IndexOf(characters, symbol) +
                    Array.IndexOf(characters, keyword[keyword_index])) % N;

                result += characters[c];

                TempList.Add($"(p[{i}] + k[{i}]) mod {N} = ({Array.IndexOf(characters, symbol)} + {Array.IndexOf(characters, keyword[keyword_index])}) % {N} = {characters[c]}");

                keyword_index++; i++;

                if ((keyword_index + 1) == keyword.Length)
                    keyword_index = 0;
            }

            return result;
        }
        /// <summary>
        /// Декодирование шифром Виженера
        /// </summary>
        /// <param name="input"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static string VigenereDecode(string input, string keyword)
        {
            //input = input.ToUpper();
            //keyword = keyword.ToUpper();

            string result = "";

            int keyword_index = 0, i = 1;
            TempList.Add($"Ключ = {keyword}");
            foreach (char symbol in input)
            {
                int p = (Array.IndexOf(characters, symbol) + N -
                    Array.IndexOf(characters, keyword[keyword_index])) % N;

                result += characters[p];

                TempList.Add($"(p[{i}] + {N} - k[{i}]) mod {N} = ({Array.IndexOf(characters, symbol)} + {N} - {Array.IndexOf(characters, keyword[keyword_index])}) % {N} = {characters[p]}");

                keyword_index++; i++;

                if ((keyword_index + 1) == keyword.Length)
                    keyword_index = 0;
            }

            return result;
        }
        /// <summary>
        /// Генерация случайного шифра по длине и спец. числу
        /// </summary>
        /// <param name="lenght"></param>
        /// <param name="startSeed"></param>
        /// <returns></returns>
        public static string Generate_Pseudorandom_KeyWord(int lenght, int startSeed)
        {
            Random rand = new Random(startSeed);

            string result = "";

            for (int i = 0; i < lenght; i++)
                result += characters[rand.Next(0, characters.Length)];

            return result;
        }

        static List<Tuple<char, BitArray>> AlphaBit = null;//массив битовых кодировок алфавита
        static int bitlenth => (int)Math.Ceiling(Math.Log(N, 2));//нужное число бит
        static void GetAlphaBit()//заполнение алфавита
        {
            AlphaBit = new List<Tuple<char, BitArray>>();
            for (int i = 0; i < abc.Length; i++)
            {
                BitArray arr = new BitArray(bitlenth, false);//пустой массив
                string code = Convert.ToString(i, 2);//двоичный код числа
                string nol = new string('0', bitlenth - code.Length);
                code = String.Concat(nol, code);
                for (int j = 0; j < bitlenth; j++)
                    if (code[code.Length - 1 - j] == '1')//перевод строки кода в массив битов
                        arr[bitlenth - 1 - j] = true;
                AlphaBit.Add(new Tuple<char, BitArray>(abc[i], arr));
            }
        }
        static BitArray[] bitkey;//ключ
        static void GetBitArr(int length)//генерация ключа
        {
            Random r = new Random();
            bitkey = new BitArray[length];
            for (int i = 0; i < length; i++)
            {
                bool[] mas = new bool[bitlenth];
                for (int j = 0; j < mas.Length; j++)
                    mas[j] = (r.Next() % 2 == 1) ? true : false;
                bitkey[i] = new BitArray(mas);
            }
        }
        static BitArray[] WordToBitMas(string s)
        {
            BitArray[] res = new BitArray[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                int ind = abc.IndexOf(s[i]);
                res[i] = new BitArray(AlphaBit[ind].Item2);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            }
            return res;
        }
        static string BitToString(BitArray t) => BitMasToSting(new BitArray[] { t });
        static string BitMasToWord(BitArray[] arr)
        {
            string res = "";
            for (int i = 0; i < arr.Length; i++)
                for (int j = 0; j < AlphaBit.Count; j++)
                    if (BitToString(arr[i]) == BitToString(AlphaBit[j].Item2))
                    {
                        res += AlphaBit[j].Item1;
                        //Console.WriteLine(AlphaBit[j].Item1.ToString() + " = " + BitMasToSting(new BitArray[] { AlphaBit[j].Item2 })+" = "+ BitMasToSting(new BitArray[] { arr[i] }));
                        //res.Show();
                        break;
                    }
            return res;
        }
        public static string BitMasToSting(BitArray[] arr)//перевод массива бит в строку из 0 и 1
        {
            string res = "";
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr[i].Length; j++)
                    if (arr[i][j])
                        res += "1";
                    else res += "0";
                res += " ";
            }
            return res;
        }
        public static BitArray[] StringToBitMas(string s)//перевод строки из 0 и 1 в массив бит
        {
            string[] st = s.Split(' ').Where(n => n.Length != 0).ToArray();
            BitArray[] res = new BitArray[st.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = new BitArray(st[i].Length);
                for (int j = 0; j < st[i].Length; j++)
                    if (st[i][j] == '1')
                        res[i][j] = true;
                    else res[i][j] = false;
            }
            return res;
        }
        static BitArray[] XORBitMas(BitArray[] a, BitArray[] b)
        {
            BitArray[] c = new BitArray[a.Length];
            for (int i = 0; i < a.Length; i++)
                c[i] = a[i].Xor(b[i]);
            return c;
        }
        /// <summary>
        /// Шифр Вернама
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string Vernam(string s, bool encode = true)
        {
            if (AlphaBit == null) GetAlphaBit();
            TempList.Add("Алфавит:");
            for (int i = 0; i < AlphaBit.Count; i++)
                TempList.Add('\t' + AlphaBit[i].Item1.ToString() + " = " + BitMasToSting(new BitArray[] { AlphaBit[i].Item2 }));

            if (encode)
            {
                TempList.Add($"Исходное слово:\t {s}");
                GetBitArr(s.Length); TempList.Add($"Ключ:\t {BitMasToSting(bitkey)}");
                BitArray[] wordToBit = WordToBitMas(s); TempList.Add($"Слово в битах:\t {BitMasToSting(wordToBit)}");
                BitArray[] newWordTiBit = XORBitMas(wordToBit, bitkey); TempList.Add($"Шифр в битах:\t {BitMasToSting(newWordTiBit)}"); //BitMasToSting(newWordTiBit).Show();
                return BitMasToWord(newWordTiBit);
            }
            else
            {
                TempList.Add($"Исходное слово:\t {s}");
                BitArray[] code = WordToBitMas(s); TempList.Add($"Слово (шифр) в битах:\t {BitMasToSting(code)}");
                BitArray[] newcode = XORBitMas(code, bitkey); TempList.Add($"(Искомое) слово в битах:\t {BitMasToSting(newcode)}");
                return BitMasToWord(newcode);
            }
        }
    }
}

