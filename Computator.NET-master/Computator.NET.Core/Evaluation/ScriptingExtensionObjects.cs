﻿// ReSharper disable RedundantNameQualifier
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable UseStringInterpolation

namespace Computator.NET.Core.Evaluation
{
    //TODO: refactor this shit
    public class TypeDeducer
    {
        public static System.Func<TR> Func<TR>(System.Func<TR> f)
        {
            return f;
        }

        public static System.Func<T1, TR> Func<T1, TR>(System.Func<T1, TR> f)
        {
            return f;
        }

        public static System.Func<T1, T2, TR> Func<T1, T2, TR>(System.Func<T1, T2, TR> f)
        {
            return f;
        }

        public static System.Func<T1, T2, T3, TR> Func<T1, T2, T3, TR>(System.Func<T1, T2, T3, TR> f)
        {
            return f;
        }

        public static System.Func<T1, T2, T3, T4, TR> Func<T1, T2, T3, T4, TR>(System.Func<T1, T2, T3, T4, TR> f)
        {
            return f;
        }

        public static System.Func<T1, T2, T3, T4, T5, TR> Func<T1, T2, T3, T4, T5, TR>(
            System.Func<T1, T2, T3, T4, T5, TR> f)
        {
            return f;
        }

        public static System.Func<T1, T2, T3, T4, T5, T6, TR> Func<T1, T2, T3, T4, T5, T6, TR>(
            System.Func<T1, T2, T3, T4, T5, T6, TR> f)
        {
            return f;
        }

        public static System.Func<T1, T2, T3, T4, T5, T6, T7, TR> Func<T1, T2, T3, T4, T5, T6, T7, TR>(
            System.Func<T1, T2, T3, T4, T5, T6, T7, TR> f)
        {
            return f;
        }

        public static System.Func<T1, T2, T3, T4, T5, T6, T7, T8, TR> Func<T1, T2, T3, T4, T5, T6, T7, T8, TR>(
            System.Func<T1, T2, T3, T4, T5, T6, T7, T8, TR> f)
        {
            return f;
        }
    }


    public class File
    {
        private readonly string path;
        private System.IO.StreamReader sr;
        private System.IO.StreamWriter sw;

        public File(string path)
        {
            this.path = path;
            reOpen();
        }

        private void reOpen()
        {
            var oStream = new System.IO.FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write,
                System.IO.FileShare.Read);
            var iStream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read,
                System.IO.FileShare.ReadWrite);

            sw = new System.IO.StreamWriter(oStream);
            sr = new System.IO.StreamReader(iStream);
        }

        public void clear()
        {
            close();
            System.IO.File.WriteAllText(path, string.Empty);
            reOpen();
        }

        public void close()
        {
            sr.Close();
            sw.Close();
        }

        public string readln()
        {
            return sr.ReadLine();
        }

        public string readAll()
        {
            return sr.ReadToEnd();
        }

        public void write(string s)
        {
            sw.Write(s);
            sw.Flush();
        }

        public void writeln(string s)
        {
            sw.WriteLine(s);
            sw.Flush();
        }
    }

    public static class ScriptingExtensions
    {
        public static int size<T>(this MathNet.Numerics.LinearAlgebra.Vector<T> vector)
            where T : struct, System.IEquatable<T>, System.IFormattable
        {
            return vector.Count;
        }

        public static int[] size<T>(this MathNet.Numerics.LinearAlgebra.Matrix<T> matrix)
            where T : struct, System.IEquatable<T>, System.IFormattable
        {
            return new[] { matrix.RowCount, matrix.ColumnCount };
        }

        private const string _Exponents = "⁰¹²³⁴⁵⁶⁷⁸⁹⁻";
        private const string _ToReplace = "0123456789-";
        public static double FromMathString(this string mathString)
        {
            double value = 0;
            if (mathString.Contains("E") || mathString.Contains("e"))
            {
                value = double.Parse(mathString, System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (mathString.Contains(Computator.NET.DataTypes.Text.SpecialSymbols.DotSymbol + ""))
            {
                var parts1 = mathString.Split(Computator.NET.DataTypes.Text.SpecialSymbols.DotSymbol);
                if (parts1.Length == 2)
                {
                    if (System.Linq.Enumerable.Any(parts1[1], c => System.Linq.Enumerable.Contains(_Exponents, c)))
                        value = double.Parse(parts1[0], System.Globalization.CultureInfo.InvariantCulture) *
                                EngineeringToValue(parts1[1]);
                    else
                        value = double.Parse(parts1[0], System.Globalization.CultureInfo.InvariantCulture) *
                                double.Parse(parts1[1], System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (parts1.Length == 1 && System.Linq.Enumerable.Any(parts1[0], c => System.Linq.Enumerable.Contains(_Exponents, c)))
                {
                    value = EngineeringToValue(parts1[0]);
                }
            }
            else
            {
                value = double.Parse(mathString, System.Globalization.CultureInfo.InvariantCulture);
            }
            return value;
        }
        private static double EngineeringToValue(string v)
        {
            var sb = new System.Text.StringBuilder(v);

            if (sb[0] == '1' && sb[1] == '0')
            {
                sb[1] = 'E';
            }

            for (var i = 0; i < sb.Length; i++)
                for (var j = 0; j < _Exponents.Length; j++)
                    if (sb[i] == _Exponents[j])
                        sb[i] = _ToReplace[j];

            return double.Parse(sb.ToString(), System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string ToMathString(this System.Numerics.Complex z)
        {
            //if (double.IsNaN(z.Real) && double.IsNaN(z.Imaginary))
            //return "NaN";

            switch (Properties.Settings.Default.NumericalOutputNotation)
            {
                case Computator.NET.DataTypes.SettingsTypes.NumericalOutputNotationType.MathematicalNotation:
                    if (z.Real == 0)
                    {
                        if (z.Imaginary == 1.0)
                            return "i";

                        if (z.Imaginary == -1.0)
                            return "-i";

                        return z.Imaginary != 0
                            ? string.Format("{0}{1}i", z.Imaginary.ToMathString(), Computator.NET.DataTypes.Text.SpecialSymbols.DotSymbol)
                            : "0";
                    }

                    if (z.Imaginary == 0)
                        return z.Real.ToMathString();
                    return (z.Imaginary > 0 || double.IsNaN(z.Imaginary))
                        ? string.Format("{0}+{1}{2}i", z.Real.ToMathString(), z.Imaginary.ToMathString(),
                            Computator.NET.DataTypes.Text.SpecialSymbols.DotSymbol)
                        : string.Format("{0}{1}{2}i", z.Real.ToMathString(), z.Imaginary.ToMathString(),
                            Computator.NET.DataTypes.Text.SpecialSymbols.DotSymbol);
                default:
                    //case NumericalOutputNotationType.EngineeringNotation:
                    return z.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public static string ToMathString(this double x)
        {
            if (x == double.PositiveInfinity)
                return Computator.NET.DataTypes.Text.SpecialSymbols.Infinity;
            if (x == double.NegativeInfinity)
                return string.Format("-{0}", Computator.NET.DataTypes.Text.SpecialSymbols.Infinity);
            return x.ToString(System.Globalization.CultureInfo.InvariantCulture).ToMathString();
        }

        /* Note: Do NOT create special overload for decimal, because decimal is crazy:
            (1e-23).ToString()
            "1E-23"
            ((decimal)(1e-23)).ToString()
            "0.00000000000000000000001"
         */
        public static string ToMathString(this decimal x)
        {
            return ((double)x).ToMathString();
        }

        public static string ToMathString(this string str)
        {
            switch (Properties.Settings.Default.NumericalOutputNotation)
            {
                case Computator.NET.DataTypes.SettingsTypes.NumericalOutputNotationType.MathematicalNotation:
                    if (!str.Contains("E") && !str.Contains("e"))
                        return str;
                    var chunks = str.Split('E', 'e');
                    var ret = string.Format("{0}{1}10{2}", chunks[0], Computator.NET.DataTypes.Text.SpecialSymbols.DotSymbol,
                        Computator.NET.DataTypes.Text.SpecialSymbols.AsciiToSuperscript(double.Parse(chunks[1]).ToString(System.Globalization.CultureInfo.InvariantCulture)));//we parse first then apply ToString() again to get rid of notations from engineering notation like eg +019 (when original walue was 1E+19) simple hack :)
                    return ret;
                case Computator.NET.DataTypes.SettingsTypes.NumericalOutputNotationType.EngineeringNotation:
                    return str;
                default:
                    return str;
            }
        }

        public static int size<T>(this System.Collections.Generic.List<T> list)
        {
            return list.Count;
        }

        public static int size(this System.Array array)
        {
            return array.Length;
        }

        public static void Add<T>(this T[] array, T element)
        {
            var narray = new T[array.Length + 1];
            narray[array.Length] = element;
            array = narray;
        }
    }

    internal static class ScriptingExtensionObjects
    {
        public const string ToCode = Computator.NET.Core.Functions.ReadForm.ToCode +
                                     @"
     public class TypeDeducer
    {
        public static System.Func<TR> Func<TR>(System.Func<TR> f)
        {
            return f;
        }

        public static System.Func<T1, TR> Func<T1, TR>(System.Func<T1, TR> f)
        {
            return f;
        }

        public static System.Func<T1, T2, TR> Func<T1, T2, TR>(System.Func<T1, T2, TR> f)
        {
            return f;
        }

        public static System.Func<T1, T2, T3, TR> Func<T1, T2, T3, TR>(System.Func<T1, T2, T3, TR> f)
        {
            return f;
        }

        public static System.Func<T1, T2, T3, T4, TR> Func<T1, T2, T3, T4, TR>(System.Func<T1, T2, T3, T4, TR> f)
        {
            return f;
        }

        public static System.Func<T1, T2, T3, T4, T5, TR> Func<T1, T2, T3, T4, T5, TR>(
            System.Func<T1, T2, T3, T4, T5, TR> f)
        {
            return f;
        }

        public static System.Func<T1, T2, T3, T4, T5, T6, TR> Func<T1, T2, T3, T4, T5, T6, TR>(
            System.Func<T1, T2, T3, T4, T5, T6, TR> f)
        {
            return f;
        }

        public static System.Func<T1, T2, T3, T4, T5, T6, T7, TR> Func<T1, T2, T3, T4, T5, T6, T7, TR>(
            System.Func<T1, T2, T3, T4, T5, T6, T7, TR> f)
        {
            return f;
        }

        public static System.Func<T1, T2, T3, T4, T5, T6, T7, T8, TR> Func<T1, T2, T3, T4, T5, T6, T7, T8, TR>(
            System.Func<T1, T2, T3, T4, T5, T6, T7, T8, TR> f)
        {
            return f;
        }
    }


    public class File
    {
        private readonly string path;
        private System.IO.StreamReader sr;
        private System.IO.StreamWriter sw;

        public File(string path)
        {
            this.path = path;
            reOpen();
        }

        private void reOpen()
        {
            var oStream = new System.IO.FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write,
                System.IO.FileShare.Read);
            var iStream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read,
                System.IO.FileShare.ReadWrite);

            sw = new System.IO.StreamWriter(oStream);
            sr = new System.IO.StreamReader(iStream);
        }

        public void clear()
        {
            close();
            System.IO.File.WriteAllText(path, string.Empty);
            reOpen();
        }

        public void close()
        {
            sr.Close();
            sw.Close();
        }

        public string readln()
        {
            return sr.ReadLine();
        }

        public string readAll()
        {
            return sr.ReadToEnd();
        }

        public void write(string s)
        {
            sw.Write(s);
            sw.Flush();
        }

        public void writeln(string s)
        {
            sw.WriteLine(s);
            sw.Flush();
        }
    }

    public static class ScriptingExtensions
    {
        public static int size<T>(this MathNet.Numerics.LinearAlgebra.Vector<T> vector)
            where T : struct, System.IEquatable<T>, System.IFormattable
        {
            return vector.Count;
        }

        public static int[] size<T>(this MathNet.Numerics.LinearAlgebra.Matrix<T> matrix)
            where T : struct, System.IEquatable<T>, System.IFormattable
        {
            return new[] { matrix.RowCount, matrix.ColumnCount };
        }

        private const string _Exponents = ""⁰¹²³⁴⁵⁶⁷⁸⁹⁻"";
        private const string _ToReplace = ""0123456789-"";
        public static double FromMathString(this string mathString)
        {
            double value = 0;
            if (mathString.Contains(""E"") || mathString.Contains(""e""))
            {
                value = double.Parse(mathString, System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (mathString.Contains(Computator.NET.DataTypes.Text.SpecialSymbols.DotSymbol + """"))
            {
                var parts1 = mathString.Split(Computator.NET.DataTypes.Text.SpecialSymbols.DotSymbol);
                if (parts1.Length == 2)
                {
                    if (System.Linq.Enumerable.Any(parts1[1], c => System.Linq.Enumerable.Contains(_Exponents, c)))
                        value = double.Parse(parts1[0], System.Globalization.CultureInfo.InvariantCulture) *
                                EngineeringToValue(parts1[1]);
                    else
                        value = double.Parse(parts1[0], System.Globalization.CultureInfo.InvariantCulture) *
                                double.Parse(parts1[1], System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (parts1.Length == 1 && System.Linq.Enumerable.Any(parts1[0], c => System.Linq.Enumerable.Contains(_Exponents, c)))
                {
                    value = EngineeringToValue(parts1[0]);
                }
            }
            else
            {
                value = double.Parse(mathString, System.Globalization.CultureInfo.InvariantCulture);
            }
            return value;
        }
        private static double EngineeringToValue(string v)
        {
            var sb = new System.Text.StringBuilder(v);

            if (sb[0] == '1' && sb[1] == '0')
            {
                sb[1] = 'E';
            }

            for (var i = 0; i < sb.Length; i++)
                for (var j = 0; j < _Exponents.Length; j++)
                    if (sb[i] == _Exponents[j])
                        sb[i] = _ToReplace[j];

            return double.Parse(sb.ToString(), System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string ToMathString(this System.Numerics.Complex z)
        {
            //if (double.IsNaN(z.Real) && double.IsNaN(z.Imaginary))
            //return ""NaN"";

            switch (Properties.Settings.Default.NumericalOutputNotation)
            {
                case Computator.NET.DataTypes.SettingsTypes.NumericalOutputNotationType.MathematicalNotation:
                    if (z.Real == 0)
                    {
                        if (z.Imaginary == 1.0)
                            return ""i"";

                        if (z.Imaginary == -1.0)
                            return ""-i"";

                        return z.Imaginary != 0
                            ? string.Format(""{0}{1}i"", z.Imaginary.ToMathString(), Computator.NET.DataTypes.Text.SpecialSymbols.DotSymbol)
                            : ""0"";
                    }

                    if (z.Imaginary == 0)
                        return z.Real.ToMathString();
                    return (z.Imaginary > 0 || double.IsNaN(z.Imaginary))
                        ? string.Format(""{0}+{1}{2}i"", z.Real.ToMathString(), z.Imaginary.ToMathString(),
                            Computator.NET.DataTypes.Text.SpecialSymbols.DotSymbol)
                        : string.Format(""{0}{1}{2}i"", z.Real.ToMathString(), z.Imaginary.ToMathString(),
                            Computator.NET.DataTypes.Text.SpecialSymbols.DotSymbol);
                default:
                    //case NumericalOutputNotationType.EngineeringNotation:
                    return z.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public static string ToMathString(this double x)
        {
            if (x == double.PositiveInfinity)
                return Computator.NET.DataTypes.Text.SpecialSymbols.Infinity;
            if (x == double.NegativeInfinity)
                return string.Format(""-{0}"", Computator.NET.DataTypes.Text.SpecialSymbols.Infinity);
            return x.ToString(System.Globalization.CultureInfo.InvariantCulture).ToMathString();
        }

        /* Note: Do NOT create special overload for decimal, because decimal is crazy:
            (1e-23).ToString()
            ""1E-23""
            ((decimal)(1e-23)).ToString()
            ""0.00000000000000000000001""
         */
        public static string ToMathString(this decimal x)
        {
            return ((double)x).ToMathString();
        }

        public static string ToMathString(this string str)
        {
            switch (Properties.Settings.Default.NumericalOutputNotation)
            {
                case Computator.NET.DataTypes.SettingsTypes.NumericalOutputNotationType.MathematicalNotation:
                    if (!str.Contains(""E"") && !str.Contains(""e""))
                        return str;
                    var chunks = str.Split('E', 'e');
                    var ret = string.Format(""{0}{1}10{2}"", chunks[0], Computator.NET.DataTypes.Text.SpecialSymbols.DotSymbol,
                        Computator.NET.DataTypes.Text.SpecialSymbols.AsciiToSuperscript(double.Parse(chunks[1]).ToString(System.Globalization.CultureInfo.InvariantCulture)));//we parse first then apply ToString() again to get rid of notations from engineering notation like eg +019 (when original walue was 1E+19) simple hack :)
                    return ret;
                case Computator.NET.DataTypes.SettingsTypes.NumericalOutputNotationType.EngineeringNotation:
                    return str;
                default:
                    return str;
            }
        }

        public static int size<T>(this System.Collections.Generic.List<T> list)
        {
            return list.Count;
        }

        public static int size(this System.Array array)
        {
            return array.Length;
        }

        public static void Add<T>(this T[] array, T element)
        {
            var narray = new T[array.Length + 1];
            narray[array.Length] = element;
            array = narray;
        }
    }

        ";
    }
}