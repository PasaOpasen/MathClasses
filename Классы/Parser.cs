using System;

namespace МатКлассы
{
    /// <summary>
    /// Парсер для перевода формулы функции в делегат
    /// </summary>
    public class Parser
    {
        private string term = "";
        private static string formula = "";
        /// <summary>
        /// Последняя отредактированная формула, по которой построился делегат
        /// </summary>
        public static string FORMULA => formula;
        private double nam, arg = 0;
        /// <summary>
        /// Конструктор для выражений без переменных (переменная считается нулевой)
        /// </summary>
        /// <param name="str">Строка формулы</param>
        public Parser(string str)
        {
            Clean(str);
            //arg = 0.0;
            nam = Product(term);
        }

        private void Clean(string str)
        {
            //Обработка входной строки
            foreach (char ch in str)//убрать пробелы и всякие знаки, которые не должны быть в формулах
            {
                if (Char.IsLetterOrDigit(ch) || ch == '^' || ch == '*' || ch == '/' || ch == '+' || ch == '-' || ch == ',' || ch == '(' || ch == ')' || ch == '.')
                {
                    term += ch;
                    if (ch == '.') term = term.Substring(0, term.Length - 1) + ',';
                }
            }

            //убрать лишние знаки операций
            DeleteOperators();

            //свести все лишние слова к одной переменной
            ToX();

            //убрать лишние знаки операций в конце
            char chh = term[term.Length - 1];
            while (chh == '^' || chh == '*' || chh == '/' || chh == '+' || chh == '-' || chh == ',' || chh == '(' || chh == ',')
            {
                term = term.Substring(0, term.Length - 1);
                chh = term[term.Length - 1];
            }

            formula = term;
        }

        private void DeleteOperators()
        {
            for (int i = 0; i < term.Length - 1; i++)
            {
                char chh = term[i];
                if (chh == '^' || chh == '*' || chh == '/' || chh == '+' || chh == '-' || chh == ',' || chh == ',')
                {
                    char ch = term[i + 1];
                    while (ch == '^' || ch == '*' || ch == '/' || ch == '+' || ch == '-' || ch == ',' || ch == ',')
                    {
                        if (i + 2 < term.Length)
                            term = term.Substring(0, i + 1) + term.Substring(i + 2, term.Length - i - 2);
                        else
                            term = term.Substring(0, i + 1);
                        if (i + 1 != term.Length) ch = term[i + 1];
                        else ch = ' ';
                    }
                }
                if (chh == ')')
                    if (Char.IsDigit(term[i + 1]))
                        term = term.Substring(0, i + 1) + "*" + term.Substring(i + 1, term.Length - i - 1);

            }
        }
        private void ToX()
        {
            //char[] c=new char[10];
            //for (int i = 0; i < 10; i++)
            //    c[i] = Convert.ToChar(i);

            string[] st = term.Split('+', '-', '^', '*', '/', ',', '(', ')');
            Array.Sort(st);
            st.Show();

            for (int i = 0; i < st.Length; i++)
            {
                string el = st[i];
                if (el.Length != 0)
                {
                    bool b = false;
                    for (int j = 0; j < el.Length; j++)
                        if (!Char.IsDigit(el[j]))
                        {
                            b = true;
                            break;
                        }
                    if (b)
                        if (el != "sin" && el != "cos" && el != "tan" && el != "acos" && el != "asin"
            && el != "atan" && el != "exp" && el != "log" && el != "abs" && el != "pi"
            && el != "sqrt" && el != "sqr" && el != "cube" && el != "x")
                            term = term.Replace(el, "x");
                }

            }
        }

        private static string info = "Для задания собственной функции требуется ввести строкой её формулу (аналитическое выражение). Выражения должны вводиться в точности так же, как если бы это была часть кода на C#; единственное отличие состоит в том, что вместо записей вида Math.Sin(x) используется sin(x). В качестве аргумента должен исльзоваться 'x'; доступные функции: sin, cos, tan, acos, asin, atan, exp, log, abs, sqrt, sqr, cube. Пример записи готовой функции: cos(x)+sin(x/2)-exp(x*log(x))^3. В случае некорректного ввода программа либо исправит формулу, либо вместо исключения будет использовать нулевую функцию.";

        /// <summary>
        /// Информация о том, какие функции считываются при парсинге и как и пользоваться
        /// </summary>
        public static string INFORMATION => info;

        /// <summary>
        /// Конструктор для выражений с переменными (переменная x)
        /// </summary>
        /// <param name="x">Значение переменной</param>
        /// <param name="str">Строка формулы</param>
        public Parser(double x, string str)
        {
            Clean(str);
            arg = x;
            ShowT();
            nam = Product(term);
            ShowT();
        }
        /// <summary>
        /// Конструктор для выражений с переменными (переменная x)
        /// </summary>
        /// <param name="x">Значение переменной</param>
        /// <param name="str">Строка формулы</param>
        public Parser(string s, double x) : this(x, s) { }

        private void ShowT()
        {
            var t = new Tuple<string, double, double>(term, nam, arg);
            Console.WriteLine(t.Item1 + " " + t.Item2 + " " + t.Item3);
        }

        //Метод обработки функций и присваивания значения переменной
        protected double Func(string s)
        {
            double element = 0.0;
            string el = "";
            foreach (char ch in s)
            {
                if (!Char.IsLetter(ch) /*|| ch!='x'*/) break;
                el += ch;
            }
            if (el == "sin") element = Math.Sin(Convert.ToDouble(s.Substring(el.Length)));
            if (el == "cos") element = Math.Cos(Convert.ToDouble(s.Substring(el.Length)));
            if (el == "tan") element = Math.Tan(Convert.ToDouble(s.Substring(el.Length)));
            if (el == "asin") element = Math.Asin(Convert.ToDouble(s.Substring(el.Length)));
            if (el == "acos") element = Math.Acos(Convert.ToDouble(s.Substring(el.Length)));
            if (el == "atan") element = Math.Atan(Convert.ToDouble(s.Substring(el.Length)));
            if (el == "exp") element = Math.Exp(Convert.ToDouble(s.Substring(el.Length)));
            if (el == "ln") element = Math.Log(Convert.ToDouble(s.Substring(el.Length)));
            if (el == "abs") element = Math.Abs(Convert.ToDouble(s.Substring(el.Length)));
            if (el == "sqrt") element = Math.Sqrt(Convert.ToDouble(s.Substring(el.Length)));
            if (el == "sqr") element = Math.Pow(Convert.ToDouble(s.Substring(el.Length)), 2);
            if (el == "cube") element = Math.Pow(Convert.ToDouble(s.Substring(el.Length)), 3);
            if (el == "pi") element = Math.PI;
            if (el != "sin" && el != "cos" && el != "tan" && el != "acos" && el != "asin"
                && el != "atan" && el != "exp" && el != "log" && el != "abs" && el != "pi"
                && el != "sqrt" && el != "sqr" && el != "cube") element = arg;
            return element;
        }
        //Метод возведения в степень
        protected double Power(string s)
        {
            double element;
            string el = "";
            foreach (char ch in s)
            {
                if (ch == '^') break;
                el += ch;
            }
            if (Char.IsLetter(el[0])) element = arg;
            else element = Convert.ToDouble(el);
            if (s.Substring(el.Length + 1) != "")
            {
                element = Math.Pow(element, Element(s.Substring(el.Length + 1)));
            }

            return element;
        }
        //Расчет умножения/деления
        protected double Element(string s)
        {
            double element;
            string el = "";
            foreach (char ch in s)
            {
                if (ch == '*' || ch == '/') break;
                el += ch;
            }
            if (Char.IsLetter(el[0]) && el.IndexOf('^') == -1) element = Func(el);
            else
            {
                if (el.IndexOf('^') == -1) element = Convert.ToDouble(el);
                else element = Power(el);
            }
            if (el.Length < s.Length - 1)
            {
                if (s[el.Length] == '*') element *= Element(s.Substring(el.Length + 1));
                if (s[el.Length] == '/') element /= Element(s.Substring(el.Length + 1));
            }
            return element;
        }
        //Входная точка. Выделение элементов
        protected double Product(string s)
        {
            int co = 0;
            string el = "";
            double element;
            string s1 = s;
            string sstr;
            if (s != "" && (s[0] == '+' || s[0] == '-')) co++;
            for (int i = co; i < s.Length; i++)
            {
                if (s[i] == '(')
                {
                    el += Breckets(s.Substring(el.Length + co), out sstr);
                    s = el + sstr;
                    co = 0;
                    i = el.Length;
                    if (sstr == "") break;
                }
                if (s[i] == '+' || s[i] == '-') break;
                el += s[i];
            }
            if (s1[0] == '-') element = -Element(el);
            else element = Element(el);
            if (el.Length < s.Length - 1) element += Product(s.Substring(el.Length + co));

            return element;
        }
        //Обработка выражений в скобках
        protected string Breckets(string s, out string sstr)
        {
            int co = 1;
            int open = 1;
            int quit = 0;
            string el = "";
            double element;
            while (open != quit)
            {
                if (s[co] == '(')
                {
                    open++;
                }
                if (s[co] == ')') quit++;
                if (open == quit) break;
                el += s[co];
                co++;
            }
            if (co < s.Length - 1) sstr = s.Substring(co + 1);
            else
            {
                sstr = "";
            }
            element = Product(el);

            return element.ToString();
        }
        //Результат
        private double Nam => nam;
        /// <summary>
        /// Возвращает функцию по формуле этой функции, где переменной является x
        /// </summary>
        /// <param name="s">Формула функции</param>
        /// <returns></returns>
        public static Func<double,double> GetDelegate(string s)
        {
            Parser p = new Parser(s);

            Func<double,double> f = (double x) =>
            {
                p.arg = x;
                p.nam = p.Product(p.term);
                return p.Nam;
            };
            return f;
        }
    }
}

