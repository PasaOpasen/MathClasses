using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace МатКлассы
{
    /// <summary>
    /// Класс графов
    /// </summary>
    public class Graphs
    {
        /// <summary>
        /// Тип графа
        /// </summary>
        public enum Type : byte { Full, Zero };
        /// <summary>
        /// Перечисление цветов
        /// </summary>
        private enum Color : byte { White, Gray, Black };

        private class ML
        {
            public Matrix M;
            public List<int> L;

            public ML(Matrix A) { this.M = new Matrix(A); L = new List<int>(); }
            public ML(Matrix A, List<int> Li) { this.M = new Matrix(A); L = new List<int>(Li); }
            public ML(ML A) { this.M = new Matrix(A.M); L = new List<int>(A.L); }

            /// <summary>
            /// Смежно ли ребро j вершине в конце списка цепи
            /// </summary>
            /// <param name="j"></param>
            /// <returns></returns>
            public bool HasEdge(int j)
            {
                int tmp = this.L[L.Count - 1];
                if (this.M[tmp, j] != 0)
                    for (int i = 0; i < this.M.n; i++)
                        if (this.M[i, j] != 0 && i != tmp)
                            return true;
                return false;
            }

            /// <summary>
            /// Произвести шаг в поиске цепи
            /// </summary>
            /// <param name="j"></param>
            /// <returns></returns>
            public ML Step(int j)
            {
                Matrix A = new Matrix();
                List<int> L = new List<int>(this.L);
                //if(L.Count==0) L.
                int tmp = this.L[L.Count - 1]/*, tmp2 = tmp */;
                //if (L.Count >= 2) tmp2 = this.L[L.Count - 2];

                for (int i = 0; i < this.M.n; i++)
                    if (this.M[i, j] != 0 && i != tmp /*&& i!=tmp2*/)
                    {
                        L.Add(i);
                        A = new Matrix(this.M.ColumnDelete(j));
                        return new ML(A, L);
                    }
                throw new Exception("Ввиду проверки на смежность до этого места не должно было дойти");
            }

            public void Show()
            {
                this.M.PrintMatrix();
                Console.Write("List: ");
                for (int i = 0; i < this.L.Count; i++) Console.Write((L[i] + 1) + " ");
                Console.WriteLine();
                Console.WriteLine();
            }

            public static void Test(Graphs g)
            {
                ML a = new ML(g.B);
                a.L.Add(0);
                Console.WriteLine(a.HasEdge(1));
            }

        }
        /// <summary>
        /// Класс вершин графа
        /// </summary>
        public class Vertex
        {
            public int x, y;//координаты вершины на плоскости
            public int color = 0;

            public Vertex(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        /// <summary>
        /// Класс рёбер графа
        /// </summary>
        public class Edge : IComparable
        {
            /// <summary>
            /// Какие вершины соединяет ребро (инцидентные ребру)
            /// </summary>
            public int v1, v2;
            /// <summary>
            /// Длина ребра
            /// </summary>
            public double length = 1;

            /// <summary>
            /// Создать ребро по инцидентным вершинам
            /// </summary>
            /// <param name="v1"></param>
            /// <param name="v2"></param>
            public Edge(int v1, int v2)
            {
                this.v1 = v1;//Math.Min(v1,v2);
                this.v2 = v2; //Math.Max(v1, v2);
            }
            /// <summary>
            /// Создать ребро по инцидентным вершинам и его длине
            /// </summary>
            /// <param name="v1"></param>
            /// <param name="v2"></param>
            /// <param name="s"></param>
            public Edge(int v1, int v2, double s) { this.v1 = v1/*Math.Min(v1, v2)*/; this.v2 = /*Math.Max(v1, v2)*/v2; this.length = s; }
            /// <summary>
            /// Конструктор копирования
            /// </summary>
            /// <param name="g"></param>
            public Edge(Edge g) { this.v1 = g.v1; this.v2 = g.v2; this.length = g.length; }

            public override string ToString()
            {
                return (this.v1 + 1).ToString() + "-" + (this.v2 + 1).ToString();
            }
            /// <summary>
            /// Вывести ребро на консоль
            /// </summary>
            public void Show()
            { Console.WriteLine(this.v1 + " - " + this.v2); }

            public int CompareTo(object obj)
            {
                Edge e = (Edge)obj;
                if (this < e) return -1;
                if (this == e) return 0;
                return 1;
            }

            //int IComparable.CompareTo(object obj)
            //{
            //    if (v1 < v2) return v1.CompareTo(obj);
            //    return v2.CompareTo(obj);
            //    throw new NotImplementedException();
            //}

            /// <summary>
            /// Совпадение рёбер. Рёбра считаются совпадающими, если они имеют одинаковую длину и одинаковые концевые вершины
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static bool operator ==(Edge a, Edge b)
            {
                return ((a.v1 == b.v1) && (a.v2 == b.v2) || (a.v1 == b.v2) && (a.v2 == b.v1)) && (a.length == b.length);
            }
            public static bool operator !=(Edge a, Edge b)
            { return !(a == b); }
            public static bool operator <(Edge a, Edge b)
            {
                Point ap = new Point(a.v1, a.v2);
                Point bp = new Point(b.v1, b.v2);
                return ap < bp;
            }
            public static bool operator >(Edge a, Edge b)
            {
                Point ap = new Point(a.v1, a.v2);
                Point bp = new Point(b.v1, b.v2);
                return ap > bp;
            }
        }

        /// <summary>
        /// Число вершин в графе
        /// </summary>
        private int p = 0;
        /// <summary>
        /// Число рёбер в графе
        /// </summary>
        private int? q = null;
        /// <summary>
        /// Матрица смежности
        /// </summary>
        private SqMatrix A;
        /// <summary>
        /// Список валетностей вершин
        /// </summary>
        private Vectors degrees = null;
        /// <summary>
        /// Тип графа
        /// </summary>
        private Type Prop;
        /// <summary>
        /// Каталог циклов
        /// </summary>
        private List<string> catalogCycles;
        /// <summary>
        /// Каталог вершин
        /// </summary>
        public List<Vertex> Ver;
        /// <summary>
        /// Каталог рёбер
        /// </summary>
        public List<Edge> Ed;
        /// <summary>
        /// Каталог простых цепей
        /// </summary>
        private List<string> Chains;
        /// <summary>
        /// Каталог маршрутов
        /// </summary>
        private List<string> Routes;
        /// <summary>
        /// Список независимых подмножеств вершин графа
        /// </summary>
        private List<Vectors> IndepSubsets;
        /// <summary>
        /// Список максимальных независимых вершин подмножества
        /// </summary>
        public List<Vectors> GreatestIndepSubsets;
        /// <summary>
        /// Список доминирующих подмножеств
        /// </summary>
        private List<Vectors> DominSubsets;
        /// <summary>
        /// Список минимальных доминирующих подмножеств
        /// </summary>
        public List<Vectors> MinimalDominSubsets;
        /// <summary>
        /// Число доминирования
        /// </summary>
        public int DominationNumber = 0;
        /// <summary>
        /// Число вершинного покрытия
        /// </summary>
        public int VCoatingNumber = 0;
        /// <summary>
        /// Число рёберного покрытия
        /// </summary>
        private int ECoatingNumber = 0;
        /// <summary>
        /// Число кликового покрытия
        /// </summary>
        public int CliquesNumber = 0;
        /// <summary>
        /// Число паросочетаний
        /// </summary>
        public int MatchingNumber = 0;
        /// <summary>
        /// Ядро графа
        /// </summary>
        private List<Vectors> Kernel;
        /// <summary>
        /// Список вершинных покрытий
        /// </summary>
        public List<Vectors> VCoatingSubsets;
        /// <summary>
        /// Список минимальных вершинных покрытий
        /// </summary>
        public List<Vectors> MinimalVCoatingSubsets;
        /// <summary>
        /// Список рёберных покрытий
        /// </summary>
        private List<List<Edge>> ECoatingSubsets;
        /// <summary>
        /// Список минимальных рёберных покрытий
        /// </summary>
        private List<List<Edge>> MinimalECoatingSubsets;
        /// <summary>
        /// Множества клик графа
        /// </summary>
        public List<Vectors> CliquesSubsets;
        /// <summary>
        /// Множество максимальных клик графа
        /// </summary>
        public List<Vectors> MaximalCliquesSubsets;
        /// <summary>
        /// Множество наибольших клик графа
        /// </summary>
        public List<Vectors> GreatestCliquesSubsets;
        /// <summary>
        /// Список паросочетаний
        /// </summary>
        private List<List<Edge>> MatchingSubsets;
        /// <summary>
        /// Список максимальных паросочетаний
        /// </summary>
        private List<List<Edge>> MaximalMatchingSubsets;
        /// <summary>
        /// Список наибольших паросочетаний
        /// </summary>
        private List<List<Edge>> GreatestMatchingSubsets;
        /// <summary>
        /// Эйлеровы циклы
        /// </summary>
        private List<string> EulerCycles;
        /// <summary>
        /// Эйлеровы цепи
        /// </summary>
        private List<string> EulerChains;

        /// <summary>
        /// Полный граф с 5 вершинами
        /// </summary>
        public static Graphs K5;
        /// <summary>
        /// Полный двудольный граф K(3,3)
        /// </summary>
        public static Graphs K3_3;

        //Конструкторы
        /// <summary>
        /// Пустой граф указанной размерности
        /// </summary>
        /// <param name="n"></param>
        public Graphs(int n) { this.A = new SqMatrix(n); this.p = n; this.q = 0; this.Prop = Type.Zero; GenerateCatalogs(); }
        /// <summary>
        /// Граф указанной размерности по типу
        /// </summary>
        /// <param name="n"></param>
        /// <param name="t"></param>
        public Graphs(int n, Graphs.Type t)
        {

            if (t == Graphs.Type.Zero) { this.A = new SqMatrix(n); this.p = n; this.Prop = Type.Zero; }
            if (t == Graphs.Type.Full)
            {
                this.A = new SqMatrix(n);
                for (int i = 0; i < n; i++)
                    for (int j = i + 1; j < n; j++)
                    {
                        A[i, j] = 1;
                        A[j, i] = 1;
                    }
                this.p = n; this.Prop = Type.Full;
                this.q = this.p * (this.p - 1) / 2;
            }

            GenerateCatalogs();//Console.WriteLine(12);
        }
        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="G"></param>
        public Graphs(Graphs G) { this.A = G.A; this.p = G.p; this.q = G.q; GenerateCatalogs(); }
        /// <summary>
        /// Конструктор по матрице смежности
        /// </summary>
        /// <param name="M"></param>
        public Graphs(SqMatrix M) { this.A = new SqMatrix(M); p = M.n; q = (M.n * M.n - M.NullValue) / 2; GenerateCatalogs(); }
        /// <summary>
        /// Конструктор по матрице смежности, расположенной в файле
        /// </summary>
        /// <param name="fs"></param>
        public Graphs(StreamReader fs) { SqMatrix M = new SqMatrix(fs); this.A = new SqMatrix(M); p = M.n; q = (M.n * M.n - M.NullValue) / 2; GenerateCatalogs(); }
        /// <summary>
        /// Создание графа по количеству вершин и набору рёбер
        /// </summary>
        /// <param name="n"></param>
        /// <param name="mas"></param>
        public Graphs(int n, Edge[] mas)
        {
            this.p = n;
            this.A = new SqMatrix(n);
            for (int i = 0; i < mas.Length; i++)
            {
                this.A[mas[i].v1, mas[i].v2] = mas[i].length;
                this.A[mas[i].v2, mas[i].v1] = mas[i].length;
            }
            GenerateCatalogs();
        }
        /// <summary>
        /// Создание графа по количеству вершин и списку рёбер
        /// </summary>
        /// <param name="n"></param>
        /// <param name="mas"></param>
        public Graphs(int n, List<Edge> mas)
        {
            this.p = n;
            this.A = new SqMatrix(n);
            for (int i = 0; i < mas.Count; i++)
            {
                this.A[mas[i].v1, mas[i].v2] = mas[i].length;
                this.A[mas[i].v2, mas[i].v1] = mas[i].length;
            }
            GenerateCatalogs();
        }
        /// <summary>
        /// Полный k-дольный граф
        /// </summary>
        /// <param name="k"></param>
        public Graphs(params int[] k)
        {
            int sum = 0;
            for (int i = 0; i < k.Length; i++) sum += k[i];
            Graphs g = new Graphs(sum);
            sum = 0;
            for (int i = 0; i < k.Length; i++)//по всем долям
            {
                for (int j = k[i] + sum; j < g.p; j++)//по нетронутым вершинам вне долей
                    for (int z = 0; z < k[i]; z++)//сделать смежную с каждой вершиной доли
                        g = g.IncludeEdges(new Edge(z, j));
                sum += k[i];
            }

            this.A = g.A; this.p = g.p; this.q = g.q; GenerateCatalogs();
        }

        static Graphs()
        {
            K5 = new Graphs(5, Graphs.Type.Full);
            K3_3 = new Graphs(3, 3);
        }

        //Cвойства
        /// <summary>
        /// Вектор валетностей графа
        /// </summary>
        public Vectors Deg
        {
            get
            {
                if (this.degrees == null)
                {
                    Vectors v = new Vectors(p);
                    for (int i = 0; i < p; i++)
                    {
                        int sum = 0;
                        for (int j = 0; j < p; j++)
                        {
                            sum += (int)A[i, j];
                        }
                        v[i] = sum;
                        //sum = 0;
                    }
                    this.degrees = v;
                    return v;
                }
                return this.degrees;
            }
        }
        /// <summary>
        /// Вектор передаточных чисел
        /// </summary>
        public Vectors P
        {
            get
            {
                Vectors v = new Vectors(this.p);
                SqMatrix M = new SqMatrix(this.Dist);
                for (int i = 0; i < this.p; i++)
                    for (int j = 0; j < this.p; j++)
                        v[i] += M[i, j];
                return v;
            }
        }
        /// <summary>
        /// Матрица Кирхгофа данного графа
        /// </summary>
        public SqMatrix Kirhg
        {
            get
            {
                SqMatrix D = new SqMatrix(this.p);
                for (int i = 0; i < this.p; i++) D[i, i] = this.Deg[i];
                return D - this.A;
            }
        }
        /// <summary>
        /// Число рёбер в графе
        /// </summary>
        public int Edges
        {
            get
            {
                if (/*this.Prop == Graphs.Type.Zero*/this.q != null) return (int)this.q;
                int s = 0;
                for (int i = 0; i < this.p; i++)
                    for (int j = i + 1; j < this.p; j++)
                        if (A[i, j] == 1) s++;
                this.q = s;
                return s;
            }
        }
        /// <summary>
        /// Дополнительный граф
        /// </summary>
        /// <returns></returns>
        public Graphs Addition { get { return Graphs.Additional(this); } }
        /// <summary>
        /// Матрица достижимости графа
        /// </summary>
        public SqMatrix Acces
        {
            get
            {
                int i = 0;
                SqMatrix Sum = this.A, Tmp = this.A;
                while ((Sum.NullValue > 0) && (i < 5))//5 итераций могут быть лишними...
                {
                    //Sum.PrintMatrix(); Console.WriteLine();
                    Tmp *= this.A;
                    Sum += Tmp;
                    i++;
                }

                for (int k = 0; k < this.p; k++)
                    for (int j = 0; j < this.p; j++) if (Sum[k, j] != 0) Sum[k, j] = 1;
                return Sum;
            }
        }
        /// <summary>
        /// Список рёбер графа
        /// </summary>
        public string SetOfEdges
        {
            get
            {
                string s = "( ";
                for (int i = 0; i < this.p; i++)
                    for (int j = i; j < this.p; j++)
                        if (this.A[i, j] != 0)
                        {
                            s += String.Format("{0}-{1} ", i + 1, j + 1);
                        }
                s += ")";
                return s;
            }
        }
        /// <summary>
        /// Матрица инцидентности графа
        /// </summary>
        public Matrix B
        {
            get
            {
                Matrix M = new Matrix(this.p, this.Edges);
                for (int i = 0; i < this.p; i++)
                    for (int j = 0; j < this.p; j++)
                        if (this.A[i, j] != 0)
                        {
                            if (j < i) M[i, this.NumberEd(j, i) - 1] = 1;
                            else M[i, this.NumberEd(i, j) - 1] = 1;
                        }

                return M;
            }
        }
        /// <summary>
        /// Матрица расстояний связного графа
        /// </summary>
        public SqMatrix Dist
        {
            get
            {
                if (!Graphs.Connectivity(this)) throw new Exception("Граф не является связным!");

                SqMatrix S = new SqMatrix(this.A);
                SqMatrix Tmp = new SqMatrix(S);
                SqMatrix M = this.A * this.A;
                int t = 2;
                while (Graphs.IsCvasFull(S))
                {
                    S = Graphs.MinDis(Tmp, M, t);
                    Tmp = new SqMatrix(S);//?
                                          //Console.WriteLine(t);
                                          //M.PrintMatrix();
                                          //Console.WriteLine();
                                          //S.PrintMatrix();
                                          //Console.WriteLine();
                                          //Console.WriteLine();
                    M *= this.A;
                    t++;
                }
                //for (int i = 0; i < S.Deg; i++) S[i, i] = 0;
                return S;
            }
        }
        private static SqMatrix MinDis(SqMatrix S, SqMatrix G, int t)
        {
            SqMatrix H = new SqMatrix(S.n);
            for (int i = 0; i < S.n; i++)
                for (int j = 0; j < S.n; j++)
                {
                    //if(S[i, j] > 0 && G[i,j]> 0) H[i, j] = Math.Min(G[i, j], S[i, j]);
                    //else H[i, j] = G[i, j];
                    if ((i != j) && (S[i, j] == 0) && (G[i, j] != 0)) H[i, j] = t;
                    else H[i, j] = S[i, j];
                }
            return H;
        }
        /// <summary>
        /// Содержит ли матрица нули где-то вне главной диагонали
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        private static bool IsCvasFull(SqMatrix S)
        {
            for (int i = 0; i < S.n; i++)
                for (int j = 0; j < S.n; j++)
                    if ((i != j) && (S[i, j] == 0)) return true;
            return false;
        }
        /// <summary>
        /// Расстояние между двумя вершинами
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public int Distance(int i, int j) { return (int)this.Dist[i, j]; }
        /// <summary>
        /// Вектор эксцентриситетов
        /// </summary>
        public Vectors Eccentricity
        {
            get
            {
                Vectors v = new Vectors(this.p);
                SqMatrix M = this.Dist;
                for (int i = 0; i < this.p; i++)
                {
                    int max = (int)M[i, 0];
                    for (int j = 1; j < this.p; j++)
                        if (M[i, j] > max) max = (int)M[i, j];
                    v[i] = max;
                }
                return v;
            }
        }
        /// <summary>
        /// Радиус графа
        /// </summary>
        public int Radius
        {
            get
            {
                Vectors v = this.Eccentricity;
                int min = (int)v[0];
                for (int i = 1; i < v.Deg; i++)
                    if (v[i] < min) min = (int)v[i];
                return min;
            }
        }
        /// <summary>
        /// Диаметр графа
        /// </summary>
        public int Diameter
        {
            get
            {
                Vectors v = this.Eccentricity;
                int max = (int)v[0];
                for (int i = 1; i < v.Deg; i++)
                    if (v[i] > max) max = (int)v[i];
                return max;
            }
        }
        /// <summary>
        /// Номер вершины, которую можно взять за центр графа
        /// </summary>
        public int Center
        {
            get
            {
                Vectors v = this.Eccentricity;
                int min = (int)v[0];
                int k = 0;
                for (int i = 1; i < v.Deg; i++)
                    if (v[i] < min) { min = (int)v[i]; k = i; }
                return k + 1;
            }
        }
        /// <summary>
        /// Обхват графа
        /// </summary>
        public int G
        {
            get
            {
                int k = 0, min = catalogCycles[k].Length;
                for (int i = 1; i < catalogCycles.Count; i++)
                    if (catalogCycles[i].Length < min)
                    {
                        min = catalogCycles[i].Length;
                        k = i;
                    }
                Console.WriteLine("Цикл длины {0}: {1}", (min - 1) / 2, catalogCycles[k]);
                return (min - 1) / 2;
            }
        }
        /// <summary>
        /// Окружение графа
        /// </summary>
        public int C
        {
            get
            {
                int k = 0, max = catalogCycles[k].Length;
                for (int i = 1; i < catalogCycles.Count; i++)
                    if (catalogCycles[i].Length > max)
                    {
                        max = catalogCycles[i].Length;
                        k = i;
                    }
                Console.WriteLine("Цикл длины {0}: {1}", (max - 1) / 2, catalogCycles[k]);
                return (max - 1) / 2;
            }
        }
        /// <summary>
        /// Периферии графа
        /// </summary>
        public Vectors Peripherys
        {
            get
            {
                Vectors v = this.Eccentricity;
                int k = 0;
                for (int i = 0; i < this.p; i++)
                    if (v[i] == this.Diameter) k++;
                Vectors r = new Vectors(k);
                k = 0;
                for (int i = 0; i < this.p; i++)
                    if (v[i] == this.Diameter)
                    {
                        r[k] = i;
                        k++;
                    }
                return r;
            }
        }
        /// <summary>
        /// Медианы графа
        /// </summary>
        public Vectors Medians
        {
            get
            {
                double min = sums(0);
                int k = 1;
                for (int i = 1; i < this.p; i++)
                    if (sums(i) < min)
                    {
                        min = sums(i);
                        k = 1;
                    }
                    else if (sums(i) == min) k++;

                Vectors v = new Vectors(k);
                k = 0;
                for (int i = 0; i < this.p; i++)
                    if (sums(i) == min)
                    {
                        v[k] = i;
                        k++;
                    }
                return v;
            }
        }
        /// <summary>
        /// Число компонент связности
        /// </summary>
        public int ComponCount
        {
            get
            {
                int[] used = new int[this.p];
                for (int i = 0; i < this.p; ++i) used[i] = 0;

                void dfs(int start, int s)
                {
                    used[start] = s;
                    for (int v = 0; v < this.p; ++v)
                        if (this.A[start, v] != 0 && used[v] == 0)
                            dfs(v, s);
                }
                int Ncomp = 0;
                for (int i = 0; i < this.p; ++i)
                    if (used[i] == 0)
                        dfs(i, ++Ncomp);
                return Ncomp;
            }
        }
        /// <summary>
        /// Цикломатическое число
        /// </summary>
        public int CyclomaticN
        {
            get { return this.Edges + this.ComponCount - this.p; }
        }
        /// <summary>
        /// Список мостов
        /// </summary>
        public List<Edge> Bridges
        {
            get
            {
                List<Edge> t = new List<Edge>();

                for (int i = 0; i < this.Ed.Count; i++)
                {
                    Graphs g = this.DeleteEdges(this.Ed[i]);
                    if (g.ComponCount > 1) t.Add(this.Ed[i]);
                }

                return t;
            }
        }
        /// <summary>
        /// Список точек сочленения
        /// </summary>
        public List<Vertex> JointPoints
        {
            get
            {
                List<Vertex> t = new List<Vertex>();

                for (int i = 0; i < this.Ver.Count; i++)
                {
                    Graphs g = this.DeleteVertexes(i + 1);
                    if (g.ComponCount > 1) t.Add(this.Ver[i]);
                }

                return t;
            }
        }
        /// <summary>
        /// Вектор точек сочленения
        /// </summary>
        public Vectors JointVect
        {
            get
            {
                List<Vertex> t = new List<Vertex>();
                Vectors v = new Vectors(this.JointPoints.Count);
                int k = 0;
                for (int i = 0; i < this.Ver.Count; i++)
                {
                    Graphs g = this.DeleteVertexes(i + 1);
                    if (g.ComponCount > 1)
                    {
                        t.Add(this.Ver[i]);
                        v[k] = i + 1;
                        k++;
                    }
                }

                return v;
            }
        }
        /// <summary>
        /// Реберная связность
        /// </summary>
        public int Lambda
        {
            get
            {
                double min = Double.PositiveInfinity;
                for (int i = 0; i < this.p; i++)
                    for (int j = i + 1; j < this.p; j++)
                    {
                        int k = this.ChainsNotCroosedEdges(i, j).Count;
                        min = Math.Min(min, k);
                    }

                return (int)min;
            }
        }
        /// <summary>
        /// Вершинная связность
        /// </summary>
        public int Kappa
        {
            get
            {
                double min = Double.PositiveInfinity;
                for (int i = 0; i < this.p; i++)
                    for (int j = i + 1; j < this.p; j++)
                    {
                        int k = this.ChainsNotCroosedVertex(i, j).Count;
                        min = Math.Min(min, k);
                    }

                return (int)min;
            }
        }
        /// <summary>
        /// Хроматическое число графа
        /// </summary>
        public int ChromaticNumber
        {
            get
            {
                Polynom p = this.Xpolymon();
                for (int i = 1; i < this.p; i++)
                {
                    double tmp = p.Value(i);
                    Console.WriteLine("\tP({0})={1}", i, tmp);
                    if (tmp > 0/*== (int)tmp*/) return i;
                }
                return this.p;
            }
        }
        /// <summary>
        /// Плотность графа
        /// </summary>
        public double Density
        {
            get
            {
                return (double)this.Edges * 2 / this.p / (this.p - 1);
            }
        }
        /// <summary>
        /// Матрица кликов графа
        /// </summary>
        public Matrix CliquesMatrix
        {
            get
            {
                Matrix M = new Matrix(this.CliquesSubsets.Count, this.p);
                for (int i = 0; i < M.n; i++)
                    for (int j = 0; j < this.CliquesSubsets[i].Deg; j++)
                        M[i, (int)this.CliquesSubsets[i].vector[j] - 1] = 1;
                return M;
            }
        }
        /// <summary>
        /// Граф клик исходного графа
        /// </summary>
        public Graphs CliquesGraph
        {
            get
            {
                int size = this.MaximalCliquesSubsets.Count;
                SqMatrix M = new SqMatrix(size);

                for (int i = 0; i < size; i++)
                    for (int j = i + 1; j < size; j++)
                        if (Vectors.ExistIntersection(this.MaximalCliquesSubsets[i], this.MaximalCliquesSubsets[j]))//если оба множества имеют непустое пересечение
                        {
                            M[i, j] = 1;
                            M[j, i] = 1;
                        }

                return new Graphs(M);
            }
        }




        //Методы
        /// <summary>
        /// Список непересекающихся по рёбрам путей из i в j
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        /// <remarks>Вершины в аргументах считаются с нуля</remarks>
        private List<string> ChainsNotCroosedEdges(int a, int b)
        {
            List<string> line = new List<string>(this.Chains);

            //for (int i = 0; i < line.Count; i++) Console.WriteLine(line[i]);

            //отсев по концам
            for (int i = 0; i < line.Count; i++)
            {
                string s = line[i];
                int q = (int)Char.GetNumericValue(s[0]) - 1, w = (int)Char.GetNumericValue(s[s.Length - 1]) - 1;
                if (q == a && w == b || q == b && w == a) ;
                else
                {
                    line.RemoveAt(i);
                    i--;
                }

            }
            //for (int i = 0; i < line.Count; i++) Console.WriteLine(line[i]);
            //отсев по повторениям рёбер
            for (int i = 0; i < line.Count; i++)
            {
                string s = line[i];
                for (int j = 0; j < s.Length - 2; j += 2)
                {
                    //выделение ребра
                    string s1 = s.Substring(j, 3);
                    char[] arr = s1.ToCharArray();
                    Array.Reverse(arr);
                    string s2 = new string(arr);
                    //отсев остальных цепей с таким ребром
                    for (int k = i + 1; k < line.Count; k++)
                        if (line[k].IndexOf(s1) > -1 || line[k].IndexOf(s2) > -1)
                        {
                            line.RemoveAt(k);
                            k--;
                        }
                }
            }
            //for (int i = 0; i < line.Count; i++) Console.WriteLine(line[i]);
            return line;
        }
        /// <summary>
        /// Список непересекающихся по вершинам путей из i в j
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        /// <remarks>Вершины в аргументах считаются с нуля</remarks>
        private List<string> ChainsNotCroosedVertex(int a, int b)
        {
            List<string> line = new List<string>(this.Chains);

            //for (int i = 0; i < line.Count; i++) Console.WriteLine(line[i]);

            //отсев по концам
            for (int i = 0; i < line.Count; i++)
            {
                string s = line[i];
                int q = (int)Char.GetNumericValue(s[0]) - 1, w = (int)Char.GetNumericValue(s[s.Length - 1]) - 1;
                if (q == a && w == b || q == b && w == a) ;
                else
                {
                    line.RemoveAt(i);
                    i--;
                }

            }
            //for (int i = 0; i < line.Count; i++) Console.WriteLine(line[i]);
            //отсев по повторениям вершин
            for (int i = 0; i < line.Count; i++)
            {
                string s = line[i];
                for (int j = 2; j < s.Length - 2; j += 2)
                {
                    //выделение ребра
                    string s1 = s.Substring(j, 1);
                    //отсев остальных цепей с таким ребром
                    for (int k = i + 1; k < line.Count; k++)
                        if (line[k].IndexOf(s1) > -1)
                        {
                            line.RemoveAt(k);
                            k--;
                        }
                }
            }
            //for (int i = 0; i < line.Count; i++) Console.WriteLine(line[i]);
            return line;
        }


        /// <summary>
        /// Заполнить каталоги
        /// </summary>
        private void GenerateCatalogs()
        {
            Chains = new List<string>();
            Routes = new List<string>();
            catalogCycles = new List<string>();
            Ver = new List<Vertex>();
            Ed = new List<Edge>();
            IndepSubsets = new List<Vectors>();
            GreatestIndepSubsets = new List<Vectors>();
            DominSubsets = new List<Vectors>();
            MinimalDominSubsets = new List<Vectors>();
            Kernel = new List<Vectors>();
            VCoatingSubsets = new List<Vectors>();
            MinimalVCoatingSubsets = new List<Vectors>();
            ECoatingSubsets = new List<List<Edge>>();
            MinimalECoatingSubsets = new List<List<Edge>>();
            CliquesSubsets = new List<Vectors>();
            GreatestCliquesSubsets = new List<Vectors>();
            MaximalCliquesSubsets = new List<Vectors>();
            MaximalMatchingSubsets = new List<List<Edge>>();
            MatchingSubsets = new List<List<Edge>>();
            GreatestMatchingSubsets = new List<List<Edge>>();

            int R = 220;

            double h = 2 * Math.PI / this.p;
            for (int i = 0; i < this.p; i++)
            {
                int x = (int)(Math.Cos(i * h) * R) + 300;
                int y = (int)(Math.Sin(i * h) * R) + 260;
                Ver.Add(new Vertex(x, y));
                for (int j = i; j < this.p; j++)
                    if (this.A[i, j] != 0)
                        Ed.Add(new Edge(i, j));
            }
            cyclesSearch();
            chainsSearch();
            IndSub();
            //DominSub();
            //this.CliquesSub();
        }

        private void routsSearch(int t)
        {
            int[] color = new int[Ver.Count];
            for (int i = 0; i < Ver.Count; i++)
                for (int j = 0; j < Ver.Count; j++)
                {
                    for (int k = 0; k < Ver.Count; k++)
                        color[k] = 1;
                    DFSrouts(i, j, Ed, color, (i + 1).ToString(), t);
                }

            for (int i = 0; i < Routes.Count; i++)
            {
                char[] arr = Routes[i].ToCharArray();
                Array.Reverse(arr);
                string b = new string(arr);
                for (int j = i + 1; j < Routes.Count; j++)
                    if (Routes[j] == Routes[i] || Routes[j] == b) { Routes.RemoveAt(j); j--; }
            }
        }
        private void DFSrouts(int u, int endV, List<Edge> Ed, int[] color, string s, int t)
        {

            if ((s.Length - 1) / 2 == t)
            {
                Routes.Add(s);
                return;
            }
            for (int w = 0; w < Ed.Count; w++)
            {
                if (color[Ed[w].v2] == 1 && Ed[w].v1 == u)
                {
                    DFSrouts(Ed[w].v2, endV, Ed, color, s + "-" + (Ed[w].v2 + 1).ToString(), t);
                    //color[Ed[w].v2] = 1;
                }
                else if (color[Ed[w].v1] == 1 && Ed[w].v2 == u)
                {
                    DFSrouts(Ed[w].v1, endV, Ed, color, s + "-" + (Ed[w].v1 + 1).ToString(), t);
                    //color[Ed[w].v1] = 1;
                }
            }
        }
        private void chainsSearch()
        {
            int[] color = new int[Ver.Count];
            for (int i = 0; i < Ver.Count - 1; i++)
                for (int j = i + 1; j < Ver.Count; j++)
                {
                    for (int k = 0; k < Ver.Count; k++)
                        color[k] = 1;
                    DFSchain(i, j, Ed, color, (i + 1).ToString());
                }
        }
        private void DFSchain(int u, int endV, List<Edge> Ed, int[] color, string s)
        {
            //вершину не следует перекрашивать, если u == endV (возможно в endV есть несколько путей)
            if (u != endV)
                color[u] = 2;
            else
            {
                Chains.Add(s);
                return;
            }
            for (int w = 0; w < Ed.Count; w++)
            {
                if (color[Ed[w].v2] == 1 && Ed[w].v1 == u)
                {
                    DFSchain(Ed[w].v2, endV, Ed, color, s + "-" + (Ed[w].v2 + 1).ToString());
                    color[Ed[w].v2] = 1;
                }
                else if (color[Ed[w].v1] == 1 && Ed[w].v2 == u)
                {
                    DFSchain(Ed[w].v1, endV, Ed, color, s + "-" + (Ed[w].v1 + 1).ToString());
                    color[Ed[w].v1] = 1;
                }
            }
        }
        private void cyclesSearch()
        {
            int[] color = new int[Ver.Count];
            for (int i = 0; i < Ver.Count; i++)
            {
                for (int k = 0; k < Ver.Count; k++)
                    color[k] = 1;
                List<int> cycle = new List<int>();
                cycle.Add(i + 1);
                DFScycle(i, i, Ed, color, -1, cycle);
            }
        }
        private void DFScycle(int u, int endV, List<Edge> E, int[] color, int unavailableEdge, List<int> cycle)
        {
            if (catalogCycles.Count == 40)
            {
                //Console.WriteLine("Число циклов достигло cорока! поиск прекращён!");
                return;
            }

            //если u == endV, то эту вершину перекрашивать не нужно, иначе мы в нее не вернемся, а вернуться необходимо
            if (u != endV)
                color[u] = 2;
            else if (cycle.Count >= 2)
            {
                cycle.Reverse();
                string s = cycle[0].ToString();
                for (int i = 1; i < cycle.Count; i++)
                    s += "-" + cycle[i].ToString();
                bool flag = false; //есть ли палиндром для этого цикла графа в List<string> catalogCycles?
                for (int i = 0; i < catalogCycles.Count; i++)
                    if (catalogCycles[i].ToString() == s)
                    {
                        flag = true;
                        break;
                    }
                if (!flag)
                {
                    cycle.Reverse();
                    s = cycle[0].ToString();
                    for (int i = 1; i < cycle.Count; i++)
                        s += "-" + cycle[i].ToString();
                    catalogCycles.Add(s);
                }
                return;
            }
            for (int w = 0; w < E.Count; w++)
            {
                if (w == unavailableEdge)
                    continue;
                if (color[E[w].v2] == 1 && E[w].v1 == u)
                {
                    List<int> cycleNEW = new List<int>(cycle);
                    cycleNEW.Add(E[w].v2 + 1);
                    DFScycle(E[w].v2, endV, E, color, w, cycleNEW);
                    color[E[w].v2] = 1;
                }
                else if (color[E[w].v1] == 1 && E[w].v2 == u)
                {
                    List<int> cycleNEW = new List<int>(cycle);
                    cycleNEW.Add(E[w].v1 + 1);
                    DFScycle(E[w].v1, endV, E, color, w, cycleNEW);
                    color[E[w].v1] = 1;
                }
            }
        }
        private double sums(int i)
        {
            double s = 0;
            for (int j = 0; j < this.p; j++) s += this.Dist[i, j];
            return s;
        }
        private void IndSub()
        {
            int k = 0;
            Vectors v = new Vectors();
            List<int> un = new List<int>(0);//список вершин этого множества
            for (int c = 0; c < this.p; c++)
            {
                un = new List<int>(0);//список вершин этого множества
                un.Add(c);
                for (int i = 0; i < this.p; i++)
                    if (i != c && this.IsNotRelated(un, i))//если вершина не смежная вершинам данного множества
                        un.Add(i);
                //if (un.Count > k)
                //{
                //k = un.Count;
                v = new Vectors(un.Count);
                for (int i = 0; i < v.Deg; i++) v[i] = un[i] + 1;
                IndepSubsets.Add(v);
                k = v.Deg;
                //}
            }
            k--;
            while (k > 0)
            {
                for (int c = 0; c < this.p; c++)
                {
                    un = new List<int>(0);//список вершин этого множества
                    un.Add(c);
                    for (int i = 0; i < this.p; i++)
                        if (i != c && this.IsNotRelated(un, i) && un.Count < k)//если вершина не смежная вершинам данного множества
                            un.Add(i);
                    v = new Vectors(un.Count);
                    for (int i = 0; i < v.Deg; i++) v[i] = un[i] + 1;
                    IndepSubsets.Add(v);
                }
                k--;
            }
            //отсеивание повторов
            for (int i = 0; i < IndepSubsets.Count - 1; i++)
                for (int j = i + 1; j < IndepSubsets.Count; j++)
                    if (IndepSubsets[i].Sort.Equals(IndepSubsets[j].Sort))
                    {
                        IndepSubsets.RemoveAt(j);
                        j--;
                    }
        }
        private static Vectors ToVectors(List<int> L)
        {
            Vectors v = new Vectors(L.Count);
            for (int i = 0; i < L.Count; i++) v[i] = L[i] + 1;
            return v;
        }
        /// <summary>
        /// Сгенерировать список доминирующих множеств
        /// </summary>
        public void DominSub()
        {
            List<List<int>> tmp = new List<List<int>>();//промежуточный список
            List<int> zero = new List<int>();
            for (int i = 0; i < this.p; i++) zero.Add(i);
            tmp.Add(zero);//добавить в список множество из всех вершин графа (оно считается доминирующим)

            while (tmp.Count > 0)//пока в промежуточном множестве есть элементы
            {
                List<int> R = new List<int>(tmp[0]);
                this.DominSubsets.Add(ToVectors(R));//занести это множество в список доминирующих
                int k = 0;
                for (int i = 0; i < R.Count; i++)//по очереди для каждой вершины
                {
                    List<int> Ri = new List<int>(R);
                    Ri.RemoveAt(i);//создать копию того множества, но без этой вершины
                    if (!this.IsDomination(Ri)) k++;//если получилось не доминирующее подмножество, запомнить это
                    else tmp.Add(Ri);//иначе занести в промежуточный список
                }
                if (k == R.Count) this.MinimalDominSubsets.Add(ToVectors(R));//если после удаления каждой вершины из множества получалось не доминирующее множество, добавить его в список минимальных
                tmp.RemoveAt(0);
            }
            //отсеивание повторов
            for (int i = 0; i < this.DominSubsets.Count - 1; i++)
                for (int j = i + 1; j < this.DominSubsets.Count; j++)
                    if (DominSubsets[j].Sort.Equals(DominSubsets[i].Sort))
                    {
                        DominSubsets.RemoveAt(j);
                        j--;
                    }
            for (int i = 0; i < this.MinimalDominSubsets.Count - 1; i++)
                for (int j = i + 1; j < this.MinimalDominSubsets.Count; j++)
                    if (MinimalDominSubsets[j].Sort.Equals(MinimalDominSubsets[i].Sort))
                    {
                        MinimalDominSubsets.RemoveAt(j);
                        j--;
                    }
            GenerateKernel();
        }
        private void GenerateKernel()
        {
            for (int i = 0; i < this.DominSubsets.Count; i++)//из доминирующих подмножеств выбираются независимые
            {
                bool tmp = true;
                for (int j = 0; j < this.DominSubsets[i].Deg - 1; j++)
                    for (int k = j + 1; k < this.DominSubsets[i].Deg; k++)
                        if (this.A[(int)DominSubsets[i].vector[j] - 1, (int)DominSubsets[i].vector[k] - 1] != 0)
                        {
                            tmp = false;
                            break;
                        }
                if (tmp) Kernel.Add(this.DominSubsets[i]);
            }
        }
        /// <summary>
        /// Сгенерировать список вершинных покрытий
        /// </summary>
        public void VCoatingSub()
        {
            List<List<int>> tmp = new List<List<int>>();//промежуточный список
            List<int> zero = new List<int>();
            for (int i = 0; i < this.p; i++) zero.Add(i);
            tmp.Add(zero);//добавить в список множество из всех вершин графа (оно считается вершинным покрытием)

            while (tmp.Count > 0)//пока в промежуточном множестве есть элементы
            {
                List<int> R = new List<int>(tmp[0]);
                this.VCoatingSubsets.Add(ToVectors(R));//занести это множество в список вершинных покрытий
                int k = 0;
                for (int i = 0; i < R.Count; i++)//по очереди для каждой вершины
                {
                    List<int> Ri = new List<int>(R);
                    Ri.RemoveAt(i);//создать копию того множества, но без этой вершины
                    if (!this.IsVertexCoating(Ri)) k++;//если получилось не вершинное покрытие, запомнить это
                    else tmp.Add(Ri);//иначе занести в промежуточный список
                }
                if (k == R.Count) this.MinimalVCoatingSubsets.Add(ToVectors(R));//если после удаления каждой вершины из множества получалось не вершинное покрытие, добавить его в список минимальных
                tmp.RemoveAt(0);
            }
            //отсеивание повторов
            for (int i = 0; i < this.VCoatingSubsets.Count - 1; i++)
                for (int j = i + 1; j < this.VCoatingSubsets.Count; j++)
                    if (VCoatingSubsets[j].Sort.Equals(VCoatingSubsets[i].Sort))
                    {
                        VCoatingSubsets.RemoveAt(j);
                        j--;
                    }
            for (int i = 0; i < this.MinimalVCoatingSubsets.Count - 1; i++)
                for (int j = i + 1; j < this.MinimalVCoatingSubsets.Count; j++)
                    if (MinimalVCoatingSubsets[j].Sort.Equals(MinimalVCoatingSubsets[i].Sort))
                    {
                        MinimalVCoatingSubsets.RemoveAt(j);
                        j--;
                    }
        }

        private static bool Equ(List<Edge> a, List<Edge> b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
                if (a[i] != b[i]) return false;
            return true;
        }
        private void ECoatingSub()
        {
            List<List<Edge>> tmp = new List<List<Edge>>();//промежуточный список
            List<Edge> zero = new List<Edge>();
            for (int i = 0; i < this.Edges; i++) zero.Add(this.Ed[i]);
            tmp.Add(zero);//добавить в список множество из всех рёбер графа (оно считается рёберным покрытием)

            try
            {
                while (tmp.Count > 0)//пока в промежуточном множестве есть элементы
                {
                    int g = tmp.Count - 1;
                    List<Edge> R = new List<Edge>(tmp[g]);
                    this.ECoatingSubsets.Add(R);//занести это множество в список рёберных покрытий
                    int k = 0/*,iter=0*/;

                    void EdCoat(int i)
                    {

                        //Console.WriteLine($"----------Начало итерации {i} при tmp.Count={g}");
                        List<Edge> Ri = new List<Edge>(R);
                        Ri.RemoveAt(i);//создать копию того множества, но без этого ребра
                        if (!this.IsEdgeCoating(Ri)) k++;//если получилось не рёберное покрытие, запомнить это
                        else tmp.Add(Ri);//иначе занести в промежуточный список
                                         //Console.WriteLine($"----------Конец итерации {i} при tmp.Count={g}");
                                         //iter++;
                    }
                    Parallel.For(0, R.Count - 1, EdCoat);
                    //if(iter==R.Count)
                    //      {
                    if (k == R.Count) this.MinimalECoatingSubsets.Add(R);//если после удаления каждого ребра из множества получалось не рёберное покрытие, добавить его в список минимальных
                    tmp.RemoveAt(g);
                    //}
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("РАСПАРАЛЛЕЛИВАНИЕ ПРИВЕЛО К ИСКЛЮЧЕНИЮ '" + e.Message + "'. ВОЗМОЖНА ПОТЕРЯ НЕКОТОРЫХ ДАННЫХ. ЕСЛИ ВАЖНО ИМЕТЬ ВСЕ ДАННЫЕ О РЁБЕРНЫХ ПОКРЫТИЯХ, ПОПРОБУЙТЕ ПЕРЕЗАПУСТИТЬ ПРИЛОЖЕНИЕ С ДРУГИМ РЕЖИМОМ ОПЕРАЦИИ");
                //ShowEdgeListofL(this.ECoatingSubsets);
            }

            //отсеивание повторов
            for (int i = 0; i < this.ECoatingSubsets.Count - 1; i++)
                for (int j = i + 1; j < this.ECoatingSubsets.Count; j++)
                {
                    List<Edge> a = new List<Edge>(ECoatingSubsets[j]);
                    List<Edge> b = new List<Edge>(ECoatingSubsets[i]);
                    a.Sort();
                    b.Sort();
                    if (Graphs.Equ(a, b))
                    {
                        ECoatingSubsets.RemoveAt(j);
                        j--;
                    }
                }

            for (int i = 0; i < this.MinimalECoatingSubsets.Count - 1; i++)
                for (int j = i + 1; j < this.MinimalECoatingSubsets.Count; j++)
                {
                    List<Edge> a = new List<Edge>(MinimalECoatingSubsets[j]);
                    List<Edge> b = new List<Edge>(MinimalECoatingSubsets[i]);
                    a.Sort();
                    b.Sort();
                    if (Graphs.Equ(a, b))
                    {
                        MinimalECoatingSubsets.RemoveAt(j);
                        j--;
                    }
                }
        }

        private void ECoatingSubFull()
        {
            List<List<Edge>> tmp = new List<List<Edge>>();//промежуточный список
            List<Edge> zero = new List<Edge>();
            for (int i = 0; i < this.Edges; i++) zero.Add(this.Ed[i]);
            tmp.Add(zero);//добавить в список множество из всех рёбер графа (оно считается рёберным покрытием)

            while (tmp.Count > 0)//пока в промежуточном множестве есть элементы
            {
                List<Edge> R = new List<Edge>(tmp[0]);
                this.ECoatingSubsets.Add(R);//занести это множество в список рёберных покрытий
                int k = 0;
                for (int i = 0; i < R.Count; i++)//по очереди для каждого ребра
                {
                    List<Edge> Ri = new List<Edge>(R);
                    Ri.RemoveAt(i);//создать копию того множества, но без этого ребра
                    if (!this.IsEdgeCoating(Ri)) k++;//если получилось не рёберное покрытие, запомнить это
                    else tmp.Add(Ri);//иначе занести в промежуточный список
                }

                if (k == R.Count) this.MinimalECoatingSubsets.Add(R);//если после удаления каждого ребра из множества получалось не рёберное покрытие, добавить его в список минимальных
                tmp.RemoveAt(0);
            }

            //отсеивание повторов
            for (int i = 0; i < this.ECoatingSubsets.Count - 1; i++)
                for (int j = i + 1; j < this.ECoatingSubsets.Count; j++)
                {
                    List<Edge> a = new List<Edge>(ECoatingSubsets[j]);
                    List<Edge> b = new List<Edge>(ECoatingSubsets[i]);
                    a.Sort();
                    b.Sort();
                    if (Graphs.Equ(a, b))
                    {
                        ECoatingSubsets.RemoveAt(j);
                        j--;
                    }
                }

            for (int i = 0; i < this.MinimalECoatingSubsets.Count - 1; i++)
                for (int j = i + 1; j < this.MinimalECoatingSubsets.Count; j++)
                {
                    List<Edge> a = new List<Edge>(MinimalECoatingSubsets[j]);
                    List<Edge> b = new List<Edge>(MinimalECoatingSubsets[i]);
                    a.Sort();
                    b.Sort();
                    if (Graphs.Equ(a, b))
                    {
                        MinimalECoatingSubsets.RemoveAt(j);
                        j--;
                    }
                }
        }
        //private void EdCoat(List<Edge> R,int k, List<List<Edge>> tmp,int i)
        //{
        //    List<Edge> Ri = new List<Edge>(R);
        //    Ri.RemoveAt(i);//создать копию того множества, но без этого ребра
        //    if (!this.IsEdgeCoating(Ri)) k++;//если получилось не рёберное покрытие, запомнить это
        //    else tmp.Add(Ri);//иначе занести в промежуточный список
        //}

        private List<List<int>> GetDCliques()
        {
            List<int> zero = new List<int>();
            List<List<int>> result = new List<List<int>>();
            for (int i = 0; i < this.p; i++)
                for (int j = i + 1; j < this.p; j++)
                    if (this.A[i, j] != 0)
                    {
                        zero.Add(i);
                        zero.Add(j);
                        result.Add(zero);
                        //Console.WriteLine(zero[0] + " " + zero[1]);
                        zero.Clear();

                    }
            //for (int i = 0; i < result.Count; i++)
            //{
            //    for (int j = 0; j < result[i].Count; j++)
            //        Console.Write(result[i].ElementAt(j) + "+");
            //    Console.WriteLine();
            //}
            return result;
        }
        /// <summary>
        /// Сгенерировать список кликов графа
        /// </summary>
        public void CliquesSub()
        {
            List<List<int>> tmp = new List<List<int>>(this.GetDCliques());//промежуточный список, изначально состоящий из всех пар смежных вершин

            while (tmp.Count > 0)//пока в промежуточном множестве есть элементы
            {
                //if(tmp[0].Count>=1)
                //{
                List<int> R = new List<int>(tmp[0]); //Console.WriteLine(R[0] + " " + R[1]);
                Vectors v = Graphs.ToVectors(R);
                if (v.Deg >= 2) this.CliquesSubsets.Add(v);//занести это множество в список клик; из-за какого-то бага сначала появляются векторы с менее чем двумя компонентами
                int k = 0;

                for (int i = 0; i < this.p; i++)//по очереди для каждой вершины в графе
                    if (!v.Contain(i + 1))//если эта вершина ещё не содержится в клике
                    {
                        List<int> Ri = new List<int>(R);
                        Ri.Add(i);//создать копию того множества, но c новой вершиной
                        Vectors c = Graphs.ToVectors(Ri);
                        Graphs g = this.SubGraph(c);

                        if (!Graphs.IsFull(g)) k++;//если получился не клик, запомнить это
                        else tmp.Add(Ri);//иначе занести в промежуточный список
                    }

                if (k + R.Count == this.p) this.MaximalCliquesSubsets.Add(v);//если после добавления каждой вершины вне множества получался не клик, добавить его в список максимальных
                                                                             //}

                tmp.RemoveAt(0);
            }

            //отсеивание повторов
            for (int i = 0; i < this.CliquesSubsets.Count - 1; i++)
                for (int j = i + 1; j < this.CliquesSubsets.Count; j++)
                    if (CliquesSubsets[j].Sort.Equals(CliquesSubsets[i].Sort))
                    {
                        CliquesSubsets.RemoveAt(j);
                        j--;
                    }
            for (int i = 0; i < this.MaximalCliquesSubsets.Count - 1; i++)
                for (int j = i + 1; j < this.MaximalCliquesSubsets.Count; j++)
                    if (MaximalCliquesSubsets[j].Sort.Equals(MaximalCliquesSubsets[i].Sort))
                    {
                        MaximalCliquesSubsets.RemoveAt(j);
                        j--;
                    }

            int t = 0;
            //выделить наибольшие клики
            for (int i = 0; i < this.MaximalCliquesSubsets.Count; i++)
                if (MaximalCliquesSubsets[i].Deg > t) t = MaximalCliquesSubsets[i].Deg;
            this.CliquesNumber = t;
            for (int i = 0; i < this.MaximalCliquesSubsets.Count; i++)
                if (MaximalCliquesSubsets[i].Deg == t) GreatestCliquesSubsets.Add(MaximalCliquesSubsets[i]);
        }

        private static bool ContainEdge(Edge e, List<Edge> L)
        {
            for (int i = 0; i < L.Count; i++)
                if (L[i] == e) return true;
            return false;
        }
        private static bool IsMatching(List<Edge> L)
        {
            for (int i = 0; i < L.Count - 1; i++)
                for (int j = i + 1; j < L.Count; j++)
                    if (L[i].v1 == L[j].v1 || L[i].v1 == L[j].v2 || L[i].v2 == L[j].v1 || L[i].v2 == L[j].v2)
                        return false;
            return true;
        }

        /// <summary>
        /// Сгенерировать список паросочетаний
        /// </summary>
        public void MatchingSub()
        {
            List<List<Edge>> tmp = new List<List<Edge>>();//промежуточный список
            List<Edge> zero = new List<Edge>();
            for (int i = 0; i < this.Edges; i++)
            {
                zero.Add(this.Ed[i]);
                tmp.Add(zero);//добавить в список по каждому ребру (ребро считается паросочетанием)
                zero.Clear();
            }


            while (tmp.Count > 0)//пока в промежуточном множестве есть элементы
            {
                List<Edge> R = new List<Edge>(tmp[0]);
                if (R.Count > 0) this.MatchingSubsets.Add(R);//занести это множество в список паросочетаний
                int k = 0;
                for (int i = 0; i < this.Edges; i++)//по очереди для каждого ребра
                    if (!Graphs.ContainEdge(this.Ed[i], R))
                    {
                        List<Edge> Ri = new List<Edge>(R);
                        Ri.Add(this.Ed[i]);//создать копию того множества, но с новым ребром
                        if (!Graphs.IsMatching(Ri)) k++;//если получилось не паросочетание, запомнить это
                        else tmp.Add(Ri);//иначе занести в промежуточный список
                    }
                if (k + R.Count == this.Edges) this.MaximalMatchingSubsets.Add(R);//если после добавления каждого ребра из множества получалось не паросочетание, добавить его в список максимальных
                tmp.RemoveAt(0);
            }
            //отсеивание повторов
            for (int i = 0; i < this.MatchingSubsets.Count - 1; i++)
                for (int j = i + 1; j < this.MatchingSubsets.Count; j++)
                {
                    List<Edge> a = new List<Edge>(MatchingSubsets[j]);
                    List<Edge> b = new List<Edge>(MatchingSubsets[i]);
                    a.Sort();
                    b.Sort();
                    if (Graphs.Equ(a, b))
                    {
                        MatchingSubsets.RemoveAt(j);
                        j--;
                    }
                }

            for (int i = 0; i < this.MaximalMatchingSubsets.Count - 1; i++)
                for (int j = i + 1; j < this.MaximalMatchingSubsets.Count; j++)
                {
                    List<Edge> a = new List<Edge>(MaximalMatchingSubsets[j]);
                    List<Edge> b = new List<Edge>(MaximalMatchingSubsets[i]);
                    a.Sort();
                    b.Sort();
                    if (Graphs.Equ(a, b))
                    {
                        MaximalMatchingSubsets.RemoveAt(j);
                        j--;
                    }
                }

            int t = 0;
            //выделить наибольшие паросочетания
            for (int i = 0; i < this.MaximalMatchingSubsets.Count; i++)
                if (MaximalMatchingSubsets[i].Count > t) t = MaximalMatchingSubsets[i].Count;
            this.MatchingNumber = t;
            for (int i = 0; i < this.MaximalMatchingSubsets.Count; i++)
                if (MaximalMatchingSubsets[i].Count == t) GreatestMatchingSubsets.Add(MaximalMatchingSubsets[i]);
        }


        /// <summary>
        /// Номер ребра, соответствующего заданному элементу матрицы смежности
        /// </summary>
        /// <param name="ii"></param>
        /// <param name="jj"></param>
        /// <returns></returns>
        private int NumberEd(int ii, int jj)
        {
            int z = 0;
            for (int i = 0; i < this.p; i++)
                for (int j = i; j < this.p; j++)
                    if (this.A[i, j] != 0)
                    {
                        z++;
                        if ((i == ii) && (j == jj)) return z;
                    }
            throw new Exception("Элемент не обозначает ребро или не существует");
        }

        /// <summary>
        /// Проверка на изоморфизм графов
        /// </summary>
        /// <param name="Q"></param>
        /// <param name="W"></param>
        /// <returns>True, если граф связен</returns>
        public static bool Isomorphism(Graphs Q, Graphs W) { return (Q.A.CharactPol == W.A.CharactPol); }//совпадение характеристических многочленов матриц
                                                                                                         /// <summary>
                                                                                                         /// Проверка на связность графа
                                                                                                         /// </summary>
                                                                                                         /// <param name="Q"></param>
                                                                                                         /// <returns></returns>
        public static bool Connectivity(Graphs Q)//Проверка через сумму ряда степений матрицы смежности
        {
            if (Q.A.NullValue == 0) return true;
            //Q.Acces.PrintMatrix();
            if (Q.Acces.NullValue == 0) return true;
            return false;
        }
        /// <summary>
        /// Число вершин графа с нечётными степенями
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        private static int CountN(Graphs g)
        {
            int k = 0;
            Vectors v = g.Deg;
            for (int i = 0; i < v.Deg; i++)
                if (v[i] % 2 != 0) k++;
            return k;
        }
        /// <summary>
        /// Гомеоморфизм графов
        /// </summary>
        /// <param name="Q"></param>
        /// <param name="W"></param>
        /// <returns></returns>
        public static bool Gomeomorphism(Graphs Q, Graphs W) { return (Graphs.CountN(Q) == Graphs.CountN(W)); }
        /// <summary>
        /// Пример графа, гомеоморфного данному (посередине ребра поставлена вершина степени 2)
        /// </summary>
        /// <returns></returns>
        public Graphs GomeoExample()
        {
            if (this.Edges < 1) throw new Exception("Нельзя постоить граф, гомеоморфный пустому!");
            Graphs g = new Graphs(this.p + 1, this.Ed);
            //int a = g.Ed[0].v1, b = g.Ed[0].v2;
            //g.Ed.RemoveAt(0);
            Edge w = new Edge(g.Ed[0]);
            g = g.DeleteEdges(w);
            //g.Ed.Add(new Edge(a, g.p - 1));
            //g.Ed.Add(new Edge(b, g.p - 1));
            Edge r = new Edge(g.p - 1, w.v1);
            Edge t = new Edge(g.p - 1, w.v2);
            //r.Show();
            //t.Show();
            g = g.IncludeEdges(r, t);

            return g;
        }
        /// <summary>
        /// Пример первообразного графа (вершина степени 2 заменена ребром)
        /// </summary>
        public Graphs OrigExample()
        {
            if (this.Edges < 2) throw new Exception("Нельзя постоить граф, первообразный от графа без вершин валентности 2!");
            Vectors v = this.Deg;
            int k = 0;
            bool f = true;
            int[] s = new int[2];

            while (f)
            {
                while ((int)v[k] != 2)
                {
                    k++;
                    if (k == v.Deg)
                    {
                        Console.WriteLine("Нельзя построить первообразный граф, так как исходный граф не содержит вершин степени 2 либо для всякой вершины степени 2 обе смежные ей смежны и друг другу.");
                        Console.WriteLine("В этом случае выводится исходных граф.");
                        return this;
                    }
                }

                int t = 0;
                for (int i = 0; i < this.p; i++)
                    if (this.A[k, i] != 0) { s[t] = i; t++; }
                if (A[s[0], s[1]] == 0) f = false;
                else k++;
            }
            //Console.WriteLine("{0} {1} {2}", k + 1, s[0] + 1, s[1] + 1);
            Graphs g = new Graphs(this);
            g = g.IncludeEdges(new Edge(s[0], s[1]));
            g = g.DeleteVertexes(k + 1);
            return g;
        }

        /// <summary>
        /// Полнота графа
        /// </summary>
        /// <param name="E"></param>
        /// <returns></returns>
        public static bool IsFull(Graphs E)
        {
            //if (E.p <= 1) return false;
            Vectors r = E.Deg;
            for (int i = 0; i < r.Deg; i++)
                if (r[i] != E.p - 1) return false;
            return true;
        }
        /// <summary>
        /// Регулярность графа
        /// </summary>
        /// <param name="E"></param>
        /// <returns></returns>
        public static bool IsRegular(Graphs E)
        {
            Vectors r = E.Deg;
            int t = (int)r[0];
            for (int i = 1; i < r.Deg; i++)
                if (r[i] != t) return false;
            return true;
        }
        /// <summary>
        /// Самодополнительность графа
        /// </summary>
        /// <param name="E"></param>
        /// <returns></returns>
        public static bool IsSelfAdditional(Graphs E)
        {
            Vectors one = new Vectors(E.Deg);
            Vectors two = new Vectors(E.Addition.Deg);
            Array.Sort(one.vector);
            Array.Sort(two.vector);
            for (int i = 0; i < one.Deg; i++)
                if (one[i] != two[i])
                {
                    Console.WriteLine("Граф не самодополнительный, так как не выполняется необходимое условие: список валентностей его вершин не совпадает со списком валентности вершин дополнительного графа");
                    return false;
                }
            return Graphs.Isomorphism(E, E.Addition);
        }

        /// <summary>
        /// Вывести кратчайшую цепь между двумя вершинами (Алгоритм Дейкстры)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns>Вектор как перечисление вершин в цикле, но отчёт вершин начинается с первой</returns>
        public Vectors Chain(int s, int t)
        {
            //заполнение начальных данных
            Vectors r = new Vectors(this.p);//вектор расстояний
            Vectors pr = new Vectors(this.p);//вектор предшественников
            bool[] l = new bool[this.p];//поставлена ли пометка
            for (int i = 0; i < this.p; i++)
            {
                r[i] = Double.PositiveInfinity;
                pr[i] = -1;
                l[i] = false;
            }
            r[s] = 0; l[s] = true;
            int p = s;
            int k = 0;

            //цикл выполняется, пока не найдётся цепь от s до t
            while (!l[t])
            {
                double tmp = Double.PositiveInfinity;
                //помечать вершины и находить минимальное значение
                for (int i = 0; i < this.p; i++)
                    if (!l[i] && this.A[i, p] != 0)
                        if (r[i] > r[p] + this.A[i, p])
                        {
                            r[i] = r[p] + this.A[i, p];
                            pr[i] = p;

                        }

                for (int i = 0; i < this.p; i++) if (r[i] < tmp && !l[i]) tmp = r[i];


                //найти минимальную вершину среди всех помеченных
                for (int i = 0; i < this.p; i++)
                    if (!l[i] && r[i] == tmp /*&& this.A[i, p] != 0*/)
                    {
                        l[i] = true;//пометить вершину
                        p = i;//перейти на эту вершину
                        break;
                    }
                k++;//счётчик по циклам 
                    //r.Show();
                    //pr.Show();
                    //    Console.WriteLine();
            }


            //преобразовать полученные данные в цепь
            k = 0;
            int e = -1; int q = t;
            while (q != s)
            {
                e = (int)pr[q];
                q = e;
                k++;
            }

            double[] m = new double[k + 1];
            m[k] = t + 1;
            q = t;
            for (int i = k - 1; i >= 0; i--)
            { m[i] = pr[q] + 1; q = (int)(m[i] - 1); }

            return new Vectors(m);
        }
        /// <summary>
        /// Является ли маршрут цепью
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool IsChain(string s)
        {
            for (int i = 0; i < s.Length - 2; i += 2)
            {
                string a = s.Substring(i, 3);
                char[] arr = a.ToCharArray();
                Array.Reverse(arr);
                string b = new string(arr);

                //Console.WriteLine(a + " " + b);
                if (s.IndexOf(a, i + 1) > -1 || s.IndexOf(b, i + 1) > -1) return false;
            }

            return true;
        }
        /// <summary>
        /// Является ли маршрут простой цепью
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool IsSimpleChain(string s)
        {
            if (IsChain(s))
            {
                string[] st = s.Split('-');
                int n = st.Length;
                double[] vector = new double[n];
                for (int i = 0; i < n; i++) vector[i] = Convert.ToDouble(st[i]);
                Array.Sort(vector);
                for (int i = 0; i < n - 1; i++)
                    if (vector[i] == vector[i + 1]) return false;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Является ли маршрут циклом
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool IsCycle(string s)
        {
            if (IsChain(s))
            {
                string[] st = s.Split('-');
                int n = st.Length;
                double[] vector = new double[n];
                for (int i = 0; i < n; i++) vector[i] = Convert.ToDouble(st[i]);
                if (vector[0] != vector[n - 1]) return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Вывести все простые циклы в графе
        /// </summary>
        public void ShowAllCycles()
        {
            string[] s = new string[catalogCycles.Count];
            catalogCycles.CopyTo(s);
            for (int i = 0; i < s.Length; i++) Console.WriteLine(s[i]);
        }
        private bool ContainCycle()
        {
            //Vectors stack = new Vectors(this.p);
            //Vectors stack2 = new Vectors(this.p);
            //Color[] color = new Color[this.p];
            //int sp = 0, sp2 = 0, cc = 0;

            //for (int i = 0; i < this.p; i++) color[i] = Color.White;
            //bool rm = false, Found = false;
            //DFS(0, ref color, ref stack, ref stack2, ref Found, ref sp, ref rm, ref cc, ref sp2);
            //stack.Show();
            //stack2.Show();
            //return Found;
            return this.catalogCycles.Count > 0;
        }
        /// <summary>
        /// Является ли граф ациклическим
        /// </summary>
        /// <returns></returns>
        public bool IsAcyclic() { return !this.ContainCycle(); }
        /// <summary>
        /// Вывести один цикл указанной длины
        /// </summary>
        /// <param name="t"></param>
        /// <returns>Цикл указанной длины или нулевой вектор в случае отсутствия такого цикла</returns>
        public string GetCycleExample(int t)
        {
            int k = 0;
            while ((catalogCycles[k].Length - 1) / 2 != t) k++;
            return catalogCycles[k];
        }

        /// <summary>
        /// Вывести на консоль все простые циклы указанной длины либо сообщение об их отсутствии
        /// </summary>
        /// <param name="t"></param>
        public void ShowAllCycles(int t)
        {
            if ((t <= 2) || (t > this.p))
            {
                Console.WriteLine("Простых циклов указанной длины не может существовать");
                return;
            }
            int cn = 0;//число найденных циклов

            //for (int i = 0; i < this.p; i++)
            //    for (int j = i + 1; j < this.p; j++)//Проход по всевозможным неповторяющимся парам вершин в графе
            //    {
            //        Vectors ij = this.Chain(i, j);//зафиксировать кратчайшую цепь между этими вершинами
            //        //ij.Show();
            //        if ((ij.Deg - 1) + 2 <= t)//если цепь ij имеет не настолько большую длину, чтобы цикла не могло быть
            //            for (int k = j + 1; k < this.p; k++)//по всем третьим вершинам
            //                if (!ij.Contain(k + 1))//если цепь ij не проходит через k (в Chain отчёт ведётся от 1, а не от 0) 
            //                {
            //                    Vectors jk = this.Chain(j, k);
            //                    Vectors ki = this.Chain(k, i);
            //                    if (ij.Deg - 1 + jk.Deg - 1 + ki.Deg - 1 == t)//если число рёбер в их объединении равно указанной длине цикла
            //                    {
            //                        Vectors v = Vectors.Merge(ij, jk, ki);//создать цикл
            //                        //v.ShowPlusOne();
            //                        if (Vectors.IsSimpleCycle(v))//если получился простой цикл
            //                        {
            //                            v.Show();
            //                            cn++;
            //                        }
            //                    }
            //                }

            //    }
            for (int k = 0; k < catalogCycles.Count; k++)
                if ((catalogCycles[k].Length - 1) / 2 == t)
                {
                    Console.WriteLine(catalogCycles[k]);
                    cn++;
                }

            if (cn == 0) Console.WriteLine("Простых циклов указанной длины не существует");
            //----------------------------------------
            ////Vectors v = new Vectors(t+1);//вектор, отображающий цикл
            //Vectors color = new Vectors(this.p);//"цвета" вершин графа
            //Vectors r = new Vectors(this.p);//Вектор предшественников
            //for (int i = 0; i < r.Deg; i++) r[i] = -1;//Изначально вершины не имеют предшественников

            //for (int i = 0; i < this.p; i++)//цикл начинается по каждой вершине
            //{
            //    int k = 0;//длина цикла в графе
            //    if (this.Deg[i] >= 2)//если эта вершина соединена хотя бы с двумя другими
            //    {
            //        color[i] = 1;//отметить вершину как корневую

            //        NewIt(ref color, ref r);//отметить вершины, смежные корневой вершине
            //        k++;

            //        //color.Show();
            //        //r.Show();
            //        //Console.WriteLine("----------i= {0}, k= {1}",i,k);

            //        while (k <= t - 1)//пока число рёбер меньше, чем должно быть в цикле
            //        {
            //            NewIt(ref color, ref r);//отметить вершины, смежные крайним вершинам
            //            k++;

            //            //color.Show();
            //            //r.Show();
            //            //Console.WriteLine("------i= {0}, k= {1}", i, k);

            //            if (!ExistC(color)) break;//если все вершины уже отмеченные, покинуть while{}
            //        }
            //        ShowCycles(color, r);
            //        for (int e = 0; e < this.p; e++) { color[e] = 0; r[e] = -1; }
            //    }

            //}
        }
        private void NewIt(ref Vectors c, ref Vectors r)
        {
            int max = (int)c[0];
            for (int i = 1; i < this.p; i++)
                if (c[i] > max) max = (int)c[i];//найти максимальный элемент в массиве

            for (int i = 0; i < this.p; i++)
                if (c[i] == max)//если найден крайний элемент
                {
                    for (int j = 0; j < this.p; j++)
                        if (this.A[i, j] == 1 && c[j] == 0)//если элемент j смежный крайнему и ещё не был в цепи
                        {
                            c[j] = max + 1;//отметить смежную вершину
                            r[j] = i;//отметить предшественника
                        }
                }
        }
        private bool ExistC(Vectors c)
        {
            for (int i = 0; i < c.Deg; i++)
                if (c[i] == 0) return true;
            return false;
        }
        /// <summary>
        /// Вывести все циклы, исходящие из массива цветов
        /// </summary>
        /// <param name="c"></param>
        /// <param name="r"></param>
        private void ShowCycles(Vectors c, Vectors r)
        {
            int max = (int)c[0], k = 0;
            for (int i = 1; i < c.Deg; i++)
            {
                if (c[i] > max) max = (int)c[i];//найти значение максимума в массиве цветов
                if (c[i] == 1) k = i;//найти индекс корневого элемента
            }
            for (int i = 0; i < c.Deg; i++)
                if (c[i] == max && Related(i, k))//если вершина - самая крайняя и смежна с корневой (то цикл есть)
                {
                    Vectors v = new Vectors(c.Deg + 1);
                    v[0] = v[c.Deg] = k;
                    v[c.Deg - 1] = i;
                    for (int j = c.Deg - 2; j > 0; j--)
                        v[j] = r[(int)v[j + 1]];//восстановить вектор по массиву предшественников
                    v.Show();
                }

        }
        private bool Related(int i, int j) { return !(this.A[i, j] == 0); }
        /// <summary>
        /// Является ли граф двудольным
        /// </summary>
        /// <returns></returns>
        public bool IsBichromatic(out Vectors v)
        {
            int[] color = new int[this.p];
            for (int i = 0; i < this.p; ++i) color[i] = 0;
            bool Two = false;
            SqMatrix g = new SqMatrix(this.A);

            void dfs(int start)
            {
                for (int u = 0; u < this.p; ++u)
                    if (g[start, u] != 0)
                    {
                        if (color[u] == 0)
                        {
                            color[u] = 3 - color[start];
                            dfs(u);
                        }
                        else if (color[u] == color[start])
                            Two = false;
                    }
            }
            Two = true;
            for (int i = 0; i < this.p; ++i)
                if (color[i] == 0)
                {
                    color[i] = 1;
                    dfs(i);
                }
            v = new Vectors(color);
            return Two;
        }
        /// <summary>
        /// Результат теоремы Кёнига
        /// </summary>
        public void KoenigTheorem()
        {
            for (int i = 0; i < catalogCycles.Count; i++)
            {
                int k = (catalogCycles[i].Length - 1) / 2;
                if (k % 2 == 1)
                {
                    Console.WriteLine("Так как граф содержит цикл нечётной длины, он не является двудольным. Пример такого цикла: " + catalogCycles[i]);
                    return;
                }
            }
            Console.WriteLine("Так как граф не содержит циклов нечётной длины, он является двудольным");
        }

        /// <summary>
        /// Число компонент связности
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public int CompCount(out Vectors vec)
        {
            int[] used = new int[this.p];
            for (int i = 0; i < this.p; ++i) used[i] = 0;

            void dfs(int start, int s)
            {
                used[start] = s;
                for (int v = 0; v < this.p; ++v)
                    if (this.A[start, v] != 0 && used[v] == 0)
                        dfs(v, s);
            }

            int Ncomp = 0;
            for (int i = 0; i < this.p; ++i)
                if (used[i] == 0)
                    dfs(i, ++Ncomp);


            vec = new Vectors(used);
            return Ncomp;
        }

        /// <summary>
        /// Дополнение графа
        /// </summary>
        /// <param name="E"></param>
        /// <returns></returns>
        public static Graphs Additional(Graphs E)
        {
            Graphs F = new Graphs(E.p, Graphs.Type.Full);
            SqMatrix M = F.A - E.A;
            for (int i = 0; i < E.p; i++) M[i, i] = 0;
            return new Graphs(M);
        }
        /// <summary>
        /// Граф, порождённый вершинами из массива k
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public Graphs SubGraph(params int[] k)
        {
            return new Graphs(this.A.SubMatrix(k));
        }
        /// <summary>
        /// Граф, порождённый вершинами вектора
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Graphs SubGraph(Vectors v)
        {
            int[] k = new int[v.Deg];
            for (int i = 0; i < v.Deg; i++) k[i] = (int)v[i];
            return new Graphs(this.A.SubMatrix(k));
        }

        /// <summary>
        /// Подграф, порождённый удалением заданных вершин
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public Graphs DeleteVertexes(params int[] k)
        {
            int[] s = new int[this.p - k.Length];
            int tmp1 = 0, tmp2 = 0;
            Array.Sort(k);
            for (int i = 0; i < this.p; i++)
                if (tmp1 < k.Length && k[tmp1] == i + 1) tmp1++;//!!!!!!!!!!!!!!!!!Тут сначала проверка условия на возможность обратиться к элементу массива
                else { s[tmp2] = i + 1; tmp2++; }
            return this.SubGraph(s);
        }
        /// <summary>
        /// Создать подграф, порождённый удалением заданных рёбер
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Graphs DeleteEdges(params Edge[] s)
        {
            SqMatrix M = new SqMatrix(this.A);
            for (int i = 0; i < s.Length; i++)
            {
                M[s[i].v1, s[i].v2] = 0;
                M[s[i].v2, s[i].v1] = 0;
            }
            return new Graphs(M);
        }
        /// <summary>
        /// Создать граф добавлением заданных рёбер
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Graphs IncludeEdges(params Edge[] s)
        {
            SqMatrix M = new SqMatrix(this.A);
            for (int i = 0; i < s.Length; i++)
            {
                M[s[i].v1, s[i].v2] = 1;
                M[s[i].v2, s[i].v1] = 1;
            }
            return new Graphs(M);
        }
        /// <summary>
        /// Пример остова в графе (создаётся разрушением циклов)
        /// </summary>
        /// <returns></returns>
        public Graphs GetSpanningTree()
        {
            Graphs g = new Graphs(this);
            while (g.ContainCycle())
            {
                string s = g.catalogCycles[0];
                //Edge e = new Edge(Convert.ToInt16(s[0])-1, Convert.ToInt16(s[2])-1);
                Edge e = new Edge((int)Char.GetNumericValue(s[0]) - 1, (int)Char.GetNumericValue(s[2]) - 1);
                g = g.DeleteEdges(e);

                //g.A.PrintMatrix();e.Show();//Console.WriteLine(s);
            }
            return g;
        }
        /// <summary>
        /// Является ли граф деревом
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static bool IsTree(Graphs g)
        {
            return (g.IsAcyclic() && (g.ComponCount == 1));
        }
        /// <summary>
        /// Код Прюфера для дерева
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static Vectors Pryufer(Graphs g)
        {
            if (!Graphs.IsTree(g)) throw new Exception("Граф не является деревом!");
            Vectors v = new Vectors(g.p - 2);
            int ind = 0;
            while (ind < g.p - 2)
            {
                int k = g.p, j = k;
                for (int i = 0; i < g.p; i++)
                    if (g.Deg[i] == 1 && i < k) k = i; //выбирается лист с минимальным номером
                for (int i = 0; i < g.p; i++)
                    if (g.A[i, k] != 0) { j = i; break; }//Находится смежная ему вершина
                Edge e = new Edge(k, j);
                Console.WriteLine($"----Удаление ребра {k + 1}-{j + 1}");
                g = g.DeleteEdges(e);//ребро удаляется
                Console.WriteLine($"----Добавление в массив вершины {j + 1}");
                v[ind] = j + 1;
                ind++;
            }
            return v;
        }
        /// <summary>
        /// Распаковка кода Прюфера
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Graphs PryuferUnpacking(Vectors v)
        {
            Vectors count = new Vectors(v.Deg + 2);//массив вершин
            for (int i = 0; i < count.Deg; i++) count[i] = i;
            Edge[] edges = new Edge[v.Deg + 1];//массив рёбер

            //распаковка вектора
            int ind = 0;
            Console.WriteLine($"В пустой граф из {v.Deg + 2} вершин добавляются рёбра.");
            Console.WriteLine($"Конец ребра - первая слева вершина в коде, ещё не бывшая концом ребра (не удалённая).");
            Console.WriteLine($"Начало ребра - минимальная по номеру вершина, не упоминающаяся в коде и ещё не бывшая началом ребра (не удалённая).");
            while (ind < v.Deg)
            {
                int k = (v.Deg + 3);
                for (int i = 0; i < count.Deg; i++)
                    if (!v.Contain(count[i] + 1) && count[i] < k) { k = (int)count[i]; break; }
                Console.Write($"Список доступных вершин ({3 * v.Deg + 1} - не доступна): "); count.ShowPlusOne();
                Console.Write($"Список неиспользованных вершин в коде ({-1} - использованная): "); v.Show();

                Console.WriteLine($"----Добавление ребра {k + 1}-{(int)v[ind]}");
                edges[ind] = new Edge(k, (int)v[ind] - 1);
                count[k] = 3 * v.Deg;
                v[ind] = -1;
                ind++;

                //count.ShowPlusOne();
                //v.Show();
                //Console.WriteLine(k);
            }

            //добавление последнего ребра
            int t = 0;
            Vectors m = new Vectors(2);
            for (int i = 0; i < count.Deg; i++)
                if (count[i] != 3 * v.Deg) { m[t] = count[i]; t++; }
            //m.ShowPlusOne();
            Console.Write($"Список доступных вершин ({3 * v.Deg + 1} - не доступна): "); count.ShowPlusOne();
            Console.WriteLine($"----Добавление последнего ребра {(int)m[0] + 1}-{(int)m[1] + 1} (ребро между двумя доступными вершинами)");
            edges[ind] = new Edge((int)m[0], (int)m[1]);

            return new Graphs(count.Deg, edges);
        }
        /// <summary>
        /// Сожержится ли в графе заданное ребро
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool ContainThisEdge(Edge e)
        {
            return (this.A[e.v1, e.v2] != 0);
        }
        /// <summary>
        /// Выдать номер ребра в каталоге рёбер
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private int GetNumderOfEdge(Edge e)
        {
            int k = 0;
            while (this.Ed[k] != e) k++;
            return k;
        }

        /// <summary>
        /// Матрица фундаментальных циклов относительно выбранного остова
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public Matrix FundamentalCycles(Graphs g)
        {
            if (!Graphs.IsTree(g)) throw new Exception("Аргумент не является остовом!!!");

            Matrix R = new Matrix(this.CyclomaticN, this.Edges);//объявление матрицы
            int k = 0;
            int[,] p = new int[2, this.CyclomaticN];//перестановка для замены столбцов в конце

            for (int i = 0; i < this.Edges; i++)//проход по всем рёбрам
                if (!g.ContainThisEdge(this.Ed[i]))//если остов не содержит это ребро
                {
                    p[0, k] = i + 1;
                    Console.WriteLine($"-Добавить в остов ребро с концевыми вершинами {this.Ed[i].v1 + 1} и {this.Ed[i].v2 + 1}");
                    Graphs G = g.IncludeEdges(this.Ed[i]);//добавить в него ребро
                    string s = G.catalogCycles[0];
                    Console.WriteLine($"---Добавить в цикл {k + 1} ");
                    for (int j = 0; j < s.Length - 2; j += 2)
                    {
                        int u = (int)Char.GetNumericValue(s[j]), v = (int)Char.GetNumericValue(s[j + 2]);//зафиксировать концы рёбер
                        Edge e = new Edge(u - 1, v - 1);//создать ребро
                        R[k, this.GetNumderOfEdge(e)] = 1;//в строке этго цикла поставить 1 в столбце ребра, входящего в циклы
                                                          //if (p[1, k] == 0 && e!= this.Ed[i]) p[1, k] = this.GetNumderOfEdge(e) + 1;
                        Console.WriteLine($"-------Добавлено ребро с концевыми вершинами {u} и {v} (место {k + 1} {this.GetNumderOfEdge(e) + 1})");
                    }
                    k++;//перейти к следующему циклу
                }
            Console.WriteLine("Полученная матрица:"); R.PrintMatrix();

            Console.WriteLine("Если последовательно переставить местами столбцы:");
            //for(int i=0;i<this.CyclomaticN;i++)
            //{
            //for (int t = 0; t < this.Edges; t++)//проход по всем рёбрам
            //    if (g.ContainThisEdge(this.Ed[t]))
            //    {
            //        p[1, i] = t + 1;
            //        break;
            //    }
            int w = 0;
            for (int t = 0; t < this.CyclomaticN; t++)
                if (t + 1 == p[0, t]) { p[1, t] = t + 1; w++; }
                else
                {
                    p[1, t] = w + 1;
                    Console.WriteLine(p[0, t] + " c " + p[1, t]);
                    R.ColumnSwap(p[0, t] - 1, p[1, t] - 1);
                    p[0, t] = w + 1;
                    w++;
                }
            //}
            //Console.WriteLine();
            Console.WriteLine("Получим матрицу в ^красивом^ виде");

            return R;
        }
        /// <summary>
        /// Матрица фундаментальных разрезов
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        private Matrix BasisSection(Matrix M)
        {
            Matrix R = new Matrix(this.Edges - this.CyclomaticN, this.Edges);

            for (int i = 0; i < R.n; i++)
                for (int j = 0; j < this.CyclomaticN; j++)
                    R[i, j] = M[j, i + this.CyclomaticN];
            for (int i = 0; i < R.n; i++) R[i, i + this.CyclomaticN] = 1;

            return R;
        }
        /// <summary>
        /// Содержит ли цепь/цикл ребро
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static bool ContainEdge(string s, Edge e)
        {
            string s1 = (e.v1 + 1).ToString() + "-" + (e.v2 + 1).ToString();
            string s2 = (e.v2 + 1).ToString() + "-" + (e.v1 + 1).ToString();
            int ind1 = s.IndexOf(s1), ind2 = s.IndexOf(s2);
            if (ind1 > -1 || ind2 > -1) return true;
            return false;
        }
        /// <summary>
        /// Содержит ли цепь/цикл вершину
        /// </summary>
        /// <param name="s"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private static bool ContainVer(string s, int k)
        {
            int ind1 = s.IndexOf((k + 1).ToString());
            if (ind1 > -1) return true;
            return false;
        }
        /// <summary>
        /// Содержит ли цепь/цикл ребро только единожды
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static bool ContainEdgeUnique(string s, Edge e)
        {
            bool result = false;
            string s1 = (e.v1 + 1).ToString() + "-" + (e.v2 + 1).ToString();
            string s2 = (e.v2 + 1).ToString() + "-" + (e.v1 + 1).ToString();
            int ind1 = s.IndexOf(s1), ind2 = s.IndexOf(s2);//Console.WriteLine(s1 + " " + s2+" "+ind1.ToString()+" "+ind2.ToString());
            if (ind1 > -1 || ind2 > -1)//если ребро есть
                if (ind1 == -1 || ind2 == -1)//если ребро пока одно
                {
                    int ind11 = s.IndexOf(s1, Math.Max(ind1, ind2) + 1);
                    int ind22 = s.IndexOf(s2, Math.Max(ind1, ind2) + 1);
                    //Console.WriteLine(s1 + " " + s2+" "+ind11.ToString()+" "+ind22.ToString());
                    if (ind11 < 0 && ind22 < 0) result = true;//если дальше этого ребра нет
                }

            return result;
        }
        /// <summary>
        /// Содержит ли цепь/цикл вершину только единожды
        /// </summary>
        /// <param name="s"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private static bool ContainVerUnique(string s, int k)
        {
            bool result = false;
            int ind1 = s.IndexOf((k + 1).ToString());
            if (ind1 > -1)//если вершина есть
            {
                int ind11 = s.IndexOf((k + 1).ToString(), ind1 + 1);
                if (ind11 == -1)//если вершина встретилась только один раз
                    result = true;
            }

            return result;
        }
        /// <summary>
        /// Объединение мостов-блоков графа (исходный граф без мостов)
        /// </summary>
        /// <returns></returns>
        public Graphs BridgeBlocks()
        {
            Graphs g = new Graphs(this);
            Edge[] e = new Edge[this.Bridges.Count];
            for (int i = 0; i < e.Length; i++) e[i] = new Edge(this.Bridges[i]);
            g = g.DeleteEdges(e);
            return g;
        }
        /// <summary>
        /// Граф блоков-точек сочленения (максимальный подграф без точек сочленения)
        /// </summary>
        /// <returns></returns>
        public Graphs JointBlock()
        {
            if (this.JointVect.Deg == 0) return this;
            Graphs g = new Graphs(this);
            Vectors v = new Vectors(g.JointVect);
            //for (int i = 0; i < v.Deg; i++) v[i] = (int)g.JointVect[i];
            for (int i = 0; i < g.p; i++)
                for (int j = 0; j < v.Deg; j++)
                    g = g.DeleteEdges(new Edge((int)v[j] - 1, i));//удалить рёбра из точек сочленения
                                                                  //g = g.DeleteVertexes(v);
            Vectors s;
            int tmp = g.CompCount(out s);//узнать распределение по компонентам связности
                                         //v.Show();
                                         //s.Show();
            int max = 0;
            double maxval = 0;
            //найти наибольшую компоненту
            for (int i = 0; i < s.Deg - 1; i++)
            {
                int k = 1;
                for (int j = i + 1; j < s.Deg; j++)
                    if (s[j] == s[i]) k++;
                if (k > max)
                {
                    max = k;
                    maxval = s[i];
                }
            }
            //Console.WriteLine(max + " " + maxval);
            //найти вершины, которые не входят в наибольшую компоненту и не являются точками сочленения
            int[] r = new int[this.p - v.Deg - max];
            int t = 0;
            for (int i = 0; i < g.p; i++)
                if (s[i] != maxval && !v.Contain(i + 1))
                {
                    r[t] = i + 1;
                    //Console.WriteLine((r[t]+1) + " ");
                    t++;
                }

            Graphs w = this.DeleteVertexes(r);
            if (w.JointVect.Deg == 0) return w;//если теперь нет точек сочленения, вернуть ответ
            return w.JointBlock();//иначе рекурсия
        }


        /// <summary>
        /// Исследование графа на планарность
        /// </summary>
        public void AboutPlanarity()
        {
            if (this.p > 3)
            {
                Console.WriteLine("Пусть f - число граней графа. Тогда имеет место эйлерова характеристика для плоскости:");
                Console.WriteLine("\tДля связного планарного графа справедливо: p-q+f=2");
                Console.WriteLine("Отсюда получается необходимых признак планарности:");
                Console.WriteLine("\tДля связного планарного графа с р>3: q<=3p-6.");
                if (this.Edges > 3 * this.p - 6)
                    Console.WriteLine("Так как для этого графа q>3p-6 ({0}>{1}), то граф не планарный.", this.Edges, 3 * this.p - 6);
                else
                {
                    Console.WriteLine("Так как для этого графа q<=3p-6 ({0}<={1}), граф может быть планарным.", this.Edges, 3 * this.p - 6);
                    Console.WriteLine("И его потенциальное число граней f=2-p+q=2-{0}+{1}={2}", this.p, this.Edges, 2 - this.p + this.Edges);
                }

                Console.WriteLine("По теореме Понтрягина-Куратовского: ");
                if (this.p >= 5)
                {
                    for (int a = 0; a < this.p; a++)
                        for (int b = a + 1; b < this.p; b++)
                            for (int c = b + 1; c < this.p; c++)
                                for (int d = c + 1; d < this.p; d++)
                                    for (int e = d + 1; e < this.p; e++)
                                    {
                                        Graphs g = this.SubGraph(a + 1, b + 1, c + 1, d + 1, e + 1);
                                        if (Graphs.Isomorphism(g, Graphs.K5))
                                        {
                                            Console.WriteLine("\t Так как исходный граф содержит подграф, изоморвный К5 (на вершинах {0}, {1}, {2}, {3}, {4})", a + 1, b + 1, c + 1, d + 1, e + 1);
                                            Console.WriteLine("\t Исходный граф не планарен.");
                                            return;
                                        }
                                        if (this.p >= 6)
                                            for (int f = e + 1; f < this.p; f++)
                                            {
                                                Graphs y = this.SubGraph(a + 1, b + 1, c + 1, d + 1, e + 1, f + 1);
                                                if (Graphs.Isomorphism(y, Graphs.K3_3))
                                                {
                                                    Console.WriteLine("\t Так как исходный граф содержит подграф, изоморвный К(3,3) (на вершинах {0}, {1}, {2}, {3}, {4}, {5})", a + 1, b + 1, c + 1, d + 1, e + 1, f + 1);
                                                    Console.WriteLine("\t Исходный граф не планарен.");
                                                    return;
                                                }
                                            }
                                    }
                    Console.WriteLine("\t Так как граф не содержит подграфов, изоморфных K5, K(3,3), он планарен.");
                }
                else
                    Console.WriteLine("\t Так как в графе меньше 5 вершин, он не содержит подграфов, изоморфных K5, K(3,3), поэтому планарен.");
            }

        }

        /// <summary>
        /// Сгенерировать все эйлеровы циклы и цепи
        /// </summary>
        private void FillEuler()
        {
            //ML.Test(this);

            List<ML> tmp = new List<ML>();
            tmp.Add(new ML(this.B));//добавить в промежуточное множество матрицу инцидентности
            tmp[0].L.Add(0);//добавить первую вершину

            int t = 0;
            while (tmp.Count > 0)//пока в этом множестве есть элементы
            {
                ML R = new ML(tmp[0]);//взять первый
                t++;
                //if (t % 1000 == 0) { R.M.PrintMatrix(); Console.WriteLine(tmp.Count); }
                //R.Show();

                for (int j = 0; j < R.M.m; j++)//для каждого ребра
                {
                    if (R.HasEdge(j))
                    {
                        ML Rj = new ML(R.Step(j));//удалить ребро и поместить в цикл
                        if (Rj.M.m == 0)//если матрица не имеет столбцов, сгенерировать цепь
                        {
                            string s = "";
                            for (int i = 0; i < Rj.L.Count - 1; i++) s += String.Format("{0}-", Rj.L[i] + 1);
                            s += String.Format("{0}", Rj.L[Rj.L.Count - 1] + 1);
                            double a = Char.GetNumericValue(s[0]), b = Char.GetNumericValue(s[s.Length - 1]);
                            if (a == b) this.EulerCycles.Add(s);
                            else this.EulerChains.Add(s);
                        }
                        else
                        {
                            int k = 0;
                            for (int i = 0; i < Rj.M.m; i++)
                                if (!Rj.HasEdge(i)) k++;//если нельзя идти дальше, запомнить
                            if (k != Rj.M.m)//если с матрицей можно дальше работать, занести её в список
                                tmp.Add(Rj);
                        }
                    }
                }
                tmp.RemoveAt(0);
            }
        }

        /// <summary>
        /// Проверка графа на наличие эйлерова цикла/цепи с выводом
        /// </summary>
        public void IsEuler()
        {
            List<string> Z = new List<string>();
            for (int i = 0; i < this.catalogCycles.Count; i++)
            {
                int k = 0;
                string s = this.catalogCycles[i];
                for (int j = 0; j < this.Ed.Count; j++)
                    if (Graphs.ContainEdgeUnique(s, this.Ed[j]))
                        k++;//если ребро входит в цикл единожды
                if (k == this.Edges) Z.Add(this.catalogCycles[i]);//если каждое ребро входит в цикл единожды
            }

            if (Z.Count > 0)
            {
                Console.WriteLine("Граф является эйлеровым. Его эйлеровы (вдобавок простые) циклы:");
                for (int i = 0; i < Z.Count; i++) Console.WriteLine(Z[i]);
            }
            else
            {

                Vectors v = new Vectors(this.Deg);
                int sum = 0, ind = 0;
                for (int i = 0; i < v.Deg; i++) { sum += (int)v[i] % 2; ind++; }
                if (sum == 0)
                {
                    this.EulerChains = new List<string>();
                    this.EulerCycles = new List<string>();
                    FillEuler();

                    Console.WriteLine("Граф является эйлеровым. Его эйлеровы циклы:");
                    for (int i = 0; i < EulerCycles.Count; i++) Console.WriteLine(EulerCycles[i]);
                }
                else
                {
                    Console.WriteLine("Граф не является эйлеровым, так как содержит вершину нечётной степени.");

                    if (ind > 2) Console.WriteLine("...и не содержит эйлеровы цепи (не является полуэйлеровым, поскольку имеет более двух ({0}) вершин с нечётными степенями).", 1);
                    //List<string> C = new List<string>();
                    //for (int i = 0; i < this.Chains.Count; i++)
                    //{
                    //    int k = 0;
                    //    string s = this.Chains[i];
                    //    for (int j = 0; j < this.Ed.Count; j++)
                    //        if (Graphs.ContainEdgeUnique(s, this.Ed[j])) k++;//если ребро входит в цепь единожды
                    //    if (k == this.Edges) C.Add(this.Chains[i]);//если каждое ребро входит в цикл единожды
                    //}
                    else
                    {
                        Console.WriteLine("...но содержит эйлеровы цепи (является полуэйлеровым):");
                        for (int i = 0; i < EulerChains.Count; i++) Console.WriteLine(EulerChains[i]);
                    }

                }
            }
        }
        /// <summary>
        /// Проверка графа на наличие гамильтонова цикла/цепи с выводом
        /// </summary>
        public void IsHamilton()
        {
            List<string> Z = new List<string>();
            for (int i = 0; i < this.catalogCycles.Count; i++)
            {
                int k = 0;
                string s = this.catalogCycles[i].Substring(1);
                for (int j = 0; j < this.p; j++)
                    if (Graphs.ContainVerUnique(s, j))
                        k++;//если вершина входит в цикл единожды
                if (k == this.p) Z.Add(this.catalogCycles[i]);//если каждая вершина входит в цикл единожды
            }

            if (Z.Count > 0)
            {
                Console.WriteLine("Граф является гамильтоновым. Его гамильтоновы циклы:");
                for (int i = 0; i < Z.Count; i++) Console.WriteLine(Z[i]);
            }
            else
            {
                Console.WriteLine("Граф не является гамильтоновым.");

                List<string> C = new List<string>();
                for (int i = 0; i < this.Chains.Count; i++)
                {
                    int k = 0;
                    string s = this.Chains[i];
                    for (int j = 0; j < this.p; j++)
                        if (Graphs.ContainVerUnique(s, j)) k++;//если вершина входит в цепь единожды
                    if (k == this.p) C.Add(this.Chains[i]);//если каждая вершина входит в цепь единожды
                }
                if (C.Count > 0)
                {
                    Console.WriteLine("...но содержит гамильтоновы простые цепи:");
                    for (int i = 0; i < C.Count; i++) Console.WriteLine(C[i]);
                }
                else
                    Console.WriteLine("...и не содержит гамильтоновы простые цепи.");
            }
        }

        private bool IsNotRelated(List<int> l, int p)
        {
            for (int i = 0; i < l.Count; i++)
                if (this.A[l[i], p] != 0) return false;
            return true;
        }
        private bool IsNotRelated(Vectors l, int p)
        {
            for (int i = 0; i < l.Deg; i++)
                if (this.A[(int)l[i] - 1, p] != 0) return false;
            return true;
        }

        /// <summary>
        /// Число независимости графа
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int IndependenceNumber(out Vectors v)
        {
            int k = 0;
            v = new Vectors();
            List<int> un = new List<int>(0);//список вершин этого множества
            for (int c = 0; c < this.p; c++)
            {
                un = new List<int>(0);//список вершин этого множества
                un.Add(c);
                for (int i = 0; i < this.p; i++)
                    if (i != c && this.IsNotRelated(un, i))//если вершина не смежная вершинам данного множества
                        un.Add(i);
                if (un.Count > k)
                {
                    k = un.Count;
                    v = new Vectors(un.Count);
                    for (int i = 0; i < v.Deg; i++) v[i] = un[i] + 1;
                }
            }
            return v.Deg;
        }
        /// <summary>
        /// Вывести все независимые подмножетсва вершин
        /// </summary>
        public void ShowIndepSubSets()
        {
            for (int i = 0; i < this.IndepSubsets.Count; i++) IndepSubsets[i].Show();
        }
        /// <summary>
        /// Вывести все наибольшие независимые подмножества
        /// </summary>
        public void ShowGreatestIndepSubSets()
        {
            for (int i = 0; i < this.IndepSubsets.Count; i++)
            {
                bool tmp = true;
                for (int j = 0; j < this.p; j++)
                    if (IndepSubsets[i].Contain(j + 1) || !this.IsNotRelated(IndepSubsets[i], j)) ;
                    else { tmp = false; break; }//если вершина не входит в подмножество и не смежна ни одной вершине из подмножества
                if (tmp)
                {
                    IndepSubsets[i].Show();
                    GreatestIndepSubsets.Add(IndepSubsets[i]);
                }
            }
        }

        /// <summary>
        /// Выдать вектор раскраски графа
        /// </summary>
        /// <returns></returns>
        public Vectors GetColouring()
        {
            int k = 0;//число цветов
            Vectors color = new Vectors(this.p);//результат
                                                //List<int> uncol = new List<int>(this.p);//список нераскрашенных вершин
                                                //for (int i = 0; i < uncol.Count; i++) uncol[i] = i;

            while (color.Contain(0))//пока есть нераскрашенные вершины
            {
                k++;
                int c = 0;//номер нераскашенной вершины
                while (color[c] > 0) c++;
                color[c] = k;
                //List<int> unsm = new List<int>(0);//списко не смежных и не раскрашенных
                List<int> un = new List<int>(0);//список вершин этого цвета
                un.Add(c);
                for (int i = 0; i < this.p; i++)
                    if (color[i] == 0 && this.IsNotRelated(un, i))//если вершина не смежная вершинам данного цвета и не раскрашенная
                    {
                        color[i] = k;
                        un.Add(i);
                    }
                //unsm.Add(i);//сгенерировать список
                //for(int i=unsm.Count-1;i>=0;i--)
                //{
                //    color[unsm[i]] = k;
                //}
            }

            return color;
        }
        /// <summary>
        /// Отсортировать вершины по невозврастанию их валентностей
        /// </summary>
        /// <param name="l"></param>
        private void SortDeg(ref List<int> l)
        {
            Vectors v = new Vectors(this.Deg);
            for (int i = 0; i < l.Count; i++)
                for (int j = 0; j < l.Count - i - 1; j++)
                    if (v[l[j]] < v[l[j + 1]])//один шаг степени
                    {
                        int tmp = l[j];
                        l[j] = l[j + 1];
                        l[j + 1] = tmp;
                    }
                    else if (v[l[j]] == v[l[j + 1]])//второй шаг степени
                    {
                        double v1 = 0, v2 = 0;
                        for (int k = 0; k < l.Count; k++)
                        {
                            if (this.A[l[j], l[k]] != 0) v1 += v[l[k]];
                            if (this.A[l[j + 1], l[k]] != 0) v2 += v[l[k]];
                        }
                        if (v1 < v2)
                        {
                            int tmp = l[j];
                            l[j] = l[j + 1];
                            l[j + 1] = tmp;
                        }
                    }
        }
        /// <summary>
        /// Выдать вектор (предположительно) минимальной раскраски графа (алгоритм с двухшаговыми степенями)
        /// </summary>
        /// <returns></returns>
        public Vectors GetModifColouring()
        {
            int k = 0;//число цветов
            Vectors color = new Vectors(this.p);//результат
            List<int> uncol = new List<int>();//список нераскрашенных вершин
            for (int i = 0; i < this.p; i++) uncol.Add(i);
            //for (int i = 0; i < uncol.Count; i++)
            //    Console.WriteLine(uncol[i] + " " + this.Deg[uncol[i]]);
            //Console.WriteLine();
            SortDeg(ref uncol);
            //for (int i = 0; i < uncol.Count; i++)
            //    Console.WriteLine(uncol[i] + " " + this.Deg[uncol[i]]);


            while (/*uncol.Count > 0*/color.Contain(0))//пока есть нераскрашенные вершины
            {
                k++;
                int c = uncol[0];//номер нераскашенной вершины
                color[c] = k;
                List<int> un = new List<int>(0);//список вершин этого цвета
                un.Add(c);
                uncol.RemoveAt(0);
                for (int i = 0; i < uncol.Count; i++)
                    if (color[uncol[i]] == 0 && this.IsNotRelated(un, uncol[i]))//если вершина не смежная вершинам данного цвета и не раскрашенная
                    {
                        color[uncol[i]] = k;
                        un.Add(uncol[i]);
                        uncol.RemoveAt(i);
                        i--;
                    }
                SortDeg(ref uncol);
            }

            return color;
        }

        /// <summary>
        /// Дать оценки хроматическому числу
        /// </summary>
        public void EstimateChromaticNumder()
        {
            Console.WriteLine("Обозначим хроматическое число за X. Тогда:");
            double min = (double)this.p * this.p / (this.p * this.p - 2 * this.Edges);
            Console.WriteLine("\t1.1) Х имеет нижнюю оценку: p*p/(p*p-2q), т. е. X >= {0}/({0}-{1}) = {2}", this.p * this.p, 2 * this.Edges, min);

            Console.WriteLine("\t1.2) Х всегда => 1");
            min = Math.Max(1, min);

            Vectors e;
            int b = this.IndependenceNumber(out e);
            Console.WriteLine("\t1.3) Х имеет нижнюю оценку (через число независимости): p/b, т. е. X >= {0}/{1} = {2}", this.p, b, (double)this.p / b);
            min = Math.Max(b, min);

            if (!this.A.Nulle())
            {
                Console.WriteLine("\t1.4) Так как граф не пустой (значит, в нём есть смежные вершины), то Х => 2");
                min = Math.Max(2, min);
            }

            for (int i = 0; i < this.catalogCycles.Count; i++)
                if (((this.catalogCycles[i].Length - 1) / 2) % 2 == 1)
                {
                    Console.WriteLine("\t1.5) Так как граф имеет цикл нечётной длины, то Х > 2");
                    min = Math.Max(3, min);
                    break;
                }


            double max = this.p;
            Console.WriteLine("\t2.1) Х не превышает числа вершин, т. е. X <= " + max);
            Console.WriteLine("\t2.2) Х имеет верхнюю оценку (по числу независимости): p-b+1, т. е. X <= " + (this.p - b + 1));
            max = Math.Min(this.p - b + 1, max);

            Vectors v = this.Deg;
            if (!Graphs.IsFull(this) && this.p >= 3)
            {
                Console.WriteLine("\t2.3) Раз граф не полный и p>=3 ({0}>=3), то по теореме Брукса Х не превышает наибольшую валентность, т. е. X <= {1}", this.p, v.Max);
                max = Math.Min(max, v.Max);
            }
            else
            {
                Console.WriteLine("\t2.3) Х не превышает наибольшую валентность в графе более чем на 1, т. е. X <= {0} + 1 = {1}", v.Max, v.Max + 1);
                max = Math.Min(max, v.Max + 1);
            }

            Graphs ее = new Graphs(this.BridgeBlocks());
            v = ее.GetModifColouring();
            Console.Write("\t Раскрасим граф рёберных блоков (можно брать и вершинные блоки): "); v.Show();
            Console.WriteLine("\t2.4) Так как граф рёберных блоков {0}-раскрашиваем, то исходный граф {0}-раскрашиваем и X<={0}", v.Max);
            max = Math.Min(max, v.Max);

            if (this.ComponCount == 1 && ((this.p - 1) <= this.Edges) && (this.Edges <= ((double)this.p / 2 * (this.p - 1))))
            {
                Console.WriteLine("\t Так как граф связен и в нём выполнено условие: (p-1)<=q<=p(p-1)/2 ({0}<={1}<={2}),", this.p - 1, this.Edges, (double)this.p / 2 * (this.p - 1));
                double t = (3 + Math.Sqrt(9 + 8 * (this.Edges - this.p))) / 2;
                Console.WriteLine("\t2.5) Верхняя оценка для Х: (3+(9+8(q-p))^0.5)/2, т. е. X <= {0}", t);
                max = Math.Min(max, t);
            }

            v = new Vectors((int)(max - min) + 1);
            for (int i = 0; i < v.Deg; i++) v[i] = (int)(min + i);
            Console.Write("\t3.1) Тогда возможные значения Х: "); v.Show();

            if (v.Deg == 1) Console.WriteLine("\t3.2) Значит, хроматическое число равно " + v[0]);
            else
            {
                Vectors m = this.GetModifColouring();
                Console.WriteLine("\t3.2) Предположительно (поскольку не существует универсального алгоритма минимальной раскраски: ), хроматическое число равно " + m.Max);
            }
        }

        internal static Polynom Degree(int n)
        {
            Polynom p = new Polynom((int)n);
            p.coef[n] = 1;
            return p;
        }
        private static Polynom FactorialDeg(int n)
        {
            Vectors v = new Vectors(n);
            for (int i = 0; i < n; i++) v[i] = i;
            return new Polynom(1, v);
        }
        private void GetRel(out int i, out int j)
        {
            i = 0; j = 0;
            //Vectors v = new Vectors(this.Deg);
            //while (v[i] != v.Max) { i++;j++; }

            for (int n = 0; n < this.p; n++)
                for (int m = 0; m < this.p; m++)
                    if (this.A[n, m] != 0 /*&&v[m]<v[j]Math.Min(v[m],v[n])<Math.Min(v[i],v[j])*/)
                    {
                        i = n;
                        j = m;
                        return;
                    }
        }
        private Graphs Delete(int i, int j)
        {
            Edge e = new Edge(i, j);
            return new Graphs(this.DeleteEdges(e));
        }
        private Graphs PullTogether(int i, int j)
        {
            Graphs g = new Graphs(this);
            List<Edge> v = new List<Edge>(0);
            for (int k = 0; k < this.p; k++)
                if (k != i && g.A[k, j] != 0)
                    v.Add(new Edge(i, k));//к i добавляются рёбра от смежных с j
            Edge[] e = new Edge[v.Count];
            for (int k = 0; k < e.Length; k++) e[k] = v[k];
            g = g.IncludeEdges(e);
            g = g.DeleteVertexes(j + 1);//j удаляется
                                        //g.A.PrintMatrix();
            return g;
        }
        /// <summary>
        /// Является ли граф пустым
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static bool IsNulle(Graphs g)
        {
            return g.A.Nulle();
        }

        /// <summary>
        /// Хроматический полином графа
        /// </summary>
        /// <returns></returns>
        public Polynom Xpolymon()
        {
            if (Graphs.IsFull(this)) return Graphs.FactorialDeg(this.p);
            if (Graphs.IsNulle(this)) return Graphs.Degree(this.p);

            Polynom pol1 = new Polynom(this.p);
            Polynom pol2 = new Polynom(this.p);
            List<Graphs> Positive = new List<Graphs>();//графы, которые дадут сумму
            List<Graphs> Negative = new List<Graphs>();//графы, которые дадут разность
                                                       //разбить исходный граф на разность
            int a, b;
            this.GetRel(out a, out b);
            Positive.Add(this.Delete(a, b));
            Negative.Add(this.PullTogether(a, b));
            //Positive[0].A.PrintMatrix();
            //Negative[0].A.PrintMatrix();
            //Graphs.FactorialDeg(3).Show();

            //выполнять разбиение и заполнение полинома
            while (Positive.Count > 0 || Negative.Count > 0)
            {
                //Console.WriteLine(Positive.Count + " " + Negative.Count);
                for (int i = 0; i < Positive.Count; i++)
                {
                    if (Graphs.IsFull(Positive[i]))
                    { pol1 += Graphs.FactorialDeg(Positive[i].p); PolCount++; }
                    else if (Graphs.IsNulle(Positive[i]))
                    { pol1 += Graphs.Degree(Positive[i].p); PolCount++; }
                    else
                    {
                        Positive[i].GetRel(out a, out b);
                        Positive.Add(Positive[i].Delete(a, b));
                        Negative.Add(Positive[i].PullTogether(a, b));
                    }
                    Positive.RemoveAt(i); i--;
                }
                //Console.WriteLine(Positive.Count + " " + Negative.Count);
                for (int i = 0; i < Negative.Count; i++)
                {
                    if (Graphs.IsFull(Negative[i]))
                    { pol2 += Graphs.FactorialDeg(Negative[i].p); PolCount++; }
                    else if (Graphs.IsNulle(Negative[i]))
                    { pol2 += Graphs.Degree(Negative[i].p); PolCount++; }
                    else
                    {
                        Negative[i].GetRel(out a, out b);
                        Negative.Add(Negative[i].Delete(a, b));
                        Positive.Add(Negative[i].PullTogether(a, b));
                    }
                    Negative.RemoveAt(i); i--;
                }
            }
            //pol1.Show();
            //pol2.Show();

            return pol1 - pol2;
        }
        private int PolCount = 0;

        private bool IsDomination(List<int> L)
        {
            Vectors v = new Vectors(L.Count);
            for (int i = 0; i < v.Deg; i++) v[i] = L[i];
            //проверка для всех вершин
            for (int i = 0; i < this.p; i++)
                if (!v.Contain(i))//если вершина не входит в множество
                {
                    bool ret = false;
                    for (int j = 0; j < v.Deg; j++)
                        if (this.A[i, (int)v[j]] != 0)
                        {
                            ret = true;//вершина смежна какой-то вершине из множества
                            break;//можно прекратить проверку для этой вершины
                        }
                    if (!ret) return false;//если какая-то вершина не смежна всем вершинам множества, это не доминирующее множество
                }
            return true;
        }
        /// <summary>
        /// Показать доминирующие подмножества вершин с шагом step
        /// </summary>
        /// <param name="step"></param>
        public void ShowDominSub(int step)
        {
            for (int i = 0; i < this.DominSubsets.Count; i += step)
                DominSubsets[i].Show();
        }
        /// <summary>
        /// Показать минимальные доминирующие подмножества вершин
        /// </summary>
        public void ShowMinDominSub()
        {
            for (int i = 0; i < this.MinimalDominSubsets.Count; i++)
                MinimalDominSubsets[i].Show();
        }
        /// <summary>
        /// Показать наименьшие доминирующие подмножества
        /// </summary>
        public void ShowSmallestDominSub()
        {
            int k = this.MinimalDominSubsets[0].Deg;
            for (int i = 1; i < MinimalDominSubsets.Count; i++)
                if (k > MinimalDominSubsets[i].Deg)
                    k = MinimalDominSubsets[i].Deg;
            this.DominationNumber = k;
            for (int i = 0; i < this.MinimalDominSubsets.Count; i++)
                if (MinimalDominSubsets[i].Deg == k)
                    MinimalDominSubsets[i].Show();
        }

        /// <summary>
        /// Показать список векторов
        /// </summary>
        /// <param name="L"></param>
        public static void ShowVectorsList(List<Vectors> L)
        {
            for (int i = 0; i < L.Count; i++)
                L[i].Show();
        }
        /// <summary>
        /// Показать список векторов с шагом по списку
        /// </summary>
        /// <param name="L"></param>
        /// <param name="k"></param>
        public static void ShowVectorsList(List<Vectors> L, int k)
        {
            for (int i = 0; i < L.Count; i += k)
                L[i].Show();
        }
        /// <summary>
        /// Показать список рёбер с шагом по списку
        /// </summary>
        /// <param name="L"></param>
        /// <param name="k"></param>
        public static void ShowEdgeListofL(List<List<Edge>> L, int k = 1)
        {
            for (int i = 0; i < L.Count; i += k)
            {
                for (int j = 0; j < L[i].Count; j++) Console.Write(L[i][j].ToString() + " ");
                Console.WriteLine();
            }

        }


        /// <summary>
        /// Показать наименьшие вершинные покрытия
        /// </summary>
        public void ShowSmallestVCoatingSub()
        {
            int k = this.MinimalVCoatingSubsets[0].Deg;
            for (int i = 1; i < MinimalVCoatingSubsets.Count; i++)
                if (k > MinimalVCoatingSubsets[i].Deg)
                    k = MinimalVCoatingSubsets[i].Deg;
            this.VCoatingNumber = k;
            for (int i = 0; i < this.MinimalVCoatingSubsets.Count; i++)
                if (MinimalVCoatingSubsets[i].Deg == k)
                    MinimalVCoatingSubsets[i].Show();
        }
        /// <summary>
        /// Показать наименьшие рёберные покрытия
        /// </summary>
        public void ShowSmallestECoatingSub()
        {
            if (this.MinimalECoatingSubsets != null && this.MinimalECoatingSubsets.Count != 0)
            {
                int k = this.MinimalECoatingSubsets[0].Count;
                for (int i = 1; i < MinimalECoatingSubsets.Count; i++)
                    if (k > MinimalECoatingSubsets[i].Count)
                        k = MinimalECoatingSubsets[i].Count;
                this.ECoatingNumber = k;
                for (int i = 0; i < this.MinimalECoatingSubsets.Count; i++)
                    if (MinimalECoatingSubsets[i].Count == k)
                    {
                        for (int j = 0; j < MinimalECoatingSubsets[i].Count; j++) Console.Write(MinimalECoatingSubsets[i][j].ToString() + " ");
                        Console.WriteLine();
                    }
            }
        }

        /// <summary>
        /// Являются ли ребро и вершина инцидентными друг другу
        /// </summary>
        /// <param name="e"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool IsIncidental(Edge e, int k)
        {
            return (e.v1 == k || e.v2 == k);
        }

        /// <summary>
        /// Образует ли множество вершинное покрытие
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private bool IsVertexCoating(List<int> v)
        {
            for (int i = 0; i < this.Ed.Count; i++)//проход по всем рёбрам
            {
                int k = 0;
                for (int j = 0; j < v.Count; j++)//по всем вершинам в множестве
                    if (!this.IsIncidental(this.Ed[i], v[j]))/*(this.Ed[i].v1 != v[j] && this.Ed[i].v2 != v[j])*/ k++;//если ребро не инцидентно этой вершине, запомнить
                if (k == v.Count) return false;//если ребро не инцидентно всем вершинам, то множество не образует вершинное покрытие
            }
            return true;
        }
        /// <summary>
        /// Образует ли множество рёберное покрытие
        /// </summary>
        /// <param name="L"></param>
        /// <returns></returns>
        private bool IsEdgeCoating(List<Edge> L)
        {
            for (int i = 0; i < this.p; i++)//проход по всем вершинам
            {
                int k = 0;
                for (int j = 0; j < L.Count; j++)//по всем рёбрам в множестве
                    if (!this.IsIncidental(L[j], i))/*(this.Ed[i].v1 != v[j] && this.Ed[i].v2 != v[j])*/ k++;//если вершина не инцидентна этому ребру, запомнить
                if (k == L.Count) return false;//если вершина не инцидентна ни одному из рёбер, то множество не образует рёберное покрытие
            }
            return true;
        }
        private void Lemma()
        {
            Console.WriteLine("Вершины с нечётными степенями:");
            int k = 0;
            for (int i = 0; i < this.p; i++)
                if (this.Deg[i] % 2 == 1)
                {
                    k++;
                    Console.WriteLine("Вершина {0} (степень {1});", i + 1, this.Deg[i]);
                }
            Console.WriteLine("Всего вершин с нечётными степенями {0}, то есть чётное число", k);
        }
        private void EulerTeo()
        {
            Console.WriteLine("Теорема Эйлера: сумма степеней вершин равна удвоенному числу рёбер. Действительно:");
            Vectors v = this.Deg;
            string s = v[0].ToString();
            double sum = v[0];
            if (v.Deg > 1)
                for (int i = 1; i < v.Deg; i++)
                {
                    s += String.Format(" + {0}", v[i]);
                    sum += v[i];
                }
            Console.WriteLine(s + " = " + sum + " = 2q = 2*" + this.Edges + " = " + 2 * this.Edges);
        }

        public void ShowCheck0()
        {
            Console.WriteLine("ДЕМОНСТРАЦИЯ РАБОТЫ БИБЛИОТЕКИ ГРАФОВ (Дм. ПА.). ВЕРШИНЫ ГРАФА НУМЕРУЮТСЯ, НАЧИНАЯ С 1");

            Console.WriteLine("Текущие дата и время: " + DateTime.Now);
            Console.WriteLine("ОБОЗНАЧЕНИЯ: p - число вершин, q - число рёбер, k - число компонент связности");
            Console.WriteLine();
            Console.WriteLine();
        }
        public void ShowCheck1()
        {
            Console.WriteLine("Матрица смежности графа:"); this.A.PrintMatrix();
            Console.WriteLine("Число вершин графа равно {0}, число рёбер равно {1}", this.p, this.Edges);
            Console.Write("Список валентностей вершин:"); this.Deg.Show();
            this.Lemma();
            this.EulerTeo();
            if (Graphs.IsFull(this)) Console.WriteLine("Граф полный");
            else if (Graphs.IsRegular(this)) Console.WriteLine("Граф не полный, но регулярный");
            else Console.WriteLine("Граф нерегулярный");

            Console.WriteLine();
            if (!(Graphs.IsFull(this)))
            {
                Console.WriteLine("Дополнение графа содержит p(p-1)/2-q={1}*{2}/2-{3}={0} рёбер", this.Addition.Edges, this.Deg.Deg, this.Deg.Deg - 1, this.Edges);
                Console.Write("Список валентностей вершин дополнения:"); this.Addition.Deg.Show();
                Console.WriteLine("Матрица смежности дополнения:"); this.Addition.A.PrintMatrix();
            }
            if (Graphs.IsSelfAdditional(this)) Console.WriteLine("Граф является самодополнительным");
            else Console.WriteLine("Граф не является самодополнительным");

            Console.WriteLine();
            Console.WriteLine("Перечисление рёбер графа: " + this.SetOfEdges);
            Console.WriteLine("Соответствующая матрица инцидентности графа:"); this.B.PrintMatrix();
            Console.WriteLine();
            Console.WriteLine("СВЯЗЬ МЕЖДУ МАТРИЦАМИ. Матрица инцидентности выводится по верхнему/нижнему треугольнику матрицы смежности. ");
            Console.WriteLine("Матрица смежности выводится из матрицы инцидентности: ");
            Console.WriteLine("\tA = B*B.Transpose - Deg (диагональная матрица с валентностями вершин по диагонали).");
            Console.WriteLine("\tДействительно. B*B.Transpose:");
            Matrix M = this.B * this.B.Transpose(); M.PrintMatrix();
            Console.WriteLine("\tB*B.Transpose - Deg:");
            Vectors v = new Vectors(this.Deg);
            for (int i = 0; i < this.p; i++)
                M[i, i] -= v[i];
            M.PrintMatrix();

        }
        public void ShowCheck2()
        {
            Console.WriteLine();
            Console.WriteLine("Матрица смежности одного из гомеоморфных графов:"); this.GomeoExample().A.PrintMatrix();
            Console.WriteLine("Матрица смежности одного из первообразных графов:"); this.OrigExample().A.PrintMatrix();
            Console.WriteLine();
        }
        public void ShowCheck3()
        {
            if (this.ContainCycle())
            {
                Console.WriteLine("Все циклы графа длины {0}:", this.p / 2); this.ShowAllCycles(this.p / 2);
                Console.WriteLine($"Обхват графа = {this.G}; окружение графа: {this.C}");
            }
            else Console.WriteLine("Граф ациклический");
        }
        public void ShowCheck4()
        {
            Vectors p;
            Console.WriteLine();
            if (this.IsBichromatic(out p))
            {
                Console.WriteLine("Граф является двудольным, распределение вершин по долям: ");
                p.Show();
            }
            else Console.WriteLine("Граф не является двудольным");
            Console.WriteLine("Результат теоремы Кёнига:"); this.KoenigTheorem();
        }
        public void ShowCheck5()
        {
            Vectors p;
            Console.WriteLine();
            Console.WriteLine($"Число компонент связности в графе = {this.CompCount(out p)}");
            if (this.CompCount(out p) > 1)
            {
                Console.WriteLine("Распределение вершин по компонентам связности: ");
                p.Show();
                Console.WriteLine("Метрические характеристики в несвязном графе не рассматриваются.");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Матрица достижимости графа:"); this.Acces.PrintMatrix();
                Console.WriteLine();
                Console.WriteLine("Матрица расстояний графа:"); this.Dist.PrintMatrix();
                Console.WriteLine();
                Console.WriteLine("Массив эксцентриситетов:"); this.Eccentricity.Show();
                Console.WriteLine("Передаточные числа: "); this.P.Show();
                Console.WriteLine("Радиус графа = {0}; диаметр графа = {1}; один из центров графа: {2}", this.Radius, this.Diameter, this.Center);
                Console.WriteLine("Периферии графа (вершины с эксцентриситетом, равным диаметру графа): "); this.Peripherys.ShowPlusOne();
                Console.WriteLine("Медианы графа (вершины с эксцентриситетом, равным радиусу графа): "); this.Medians.ShowPlusOne();
                Console.WriteLine();


                Console.WriteLine("Пример кратчайшей цепи между двумя случайными вершинами:");
                Random rnd = new Random(); int z = rnd.Next(0, this.p); int x = -1; while (x == -1 || x == z) x = rnd.Next(0, this.p);
                this.Chain(z, x).Show();
            }
        }
        public void ShowCheck6()
        {
            Console.WriteLine();
            Graphs r = this.SubGraph(1, 2, 3, 4);
            Console.WriteLine("Матрица смежности подграфа, порождённого вершинами 1, 2, 3, 4:"); r.A.PrintMatrix();
            SqMatrix M = new SqMatrix(r.A), MM = new SqMatrix(M);
            Console.WriteLine("Квадрат матрицы смежности подграфа, порождённого вершинами 1, 2, 3, 4:");
            MM *= M; MM.PrintMatrix();
            Console.WriteLine("Куб матрицы смежности подграфа, порождённого вершинами 1, 2, 3, 4:");
            MM *= M; MM.PrintMatrix();

            int sum = 0;
            for (int i = 0; i < 4; i++)
            {
                sum += (int)MM[i, i];
                for (int j = i + 1; j < 4; j++)
                    sum += 2 * (int)MM[i, j];
            }
            Console.WriteLine("Полусумма всех элементов в кубе той матрицы смежности (число маршрутов длины 3): " + sum / 2);

            r.routsSearch(3);
            Console.WriteLine("Все маршруты длины {0} (всего {1}) подграфа, порождённого вершинами 1, 2, 3, 4:", 3, r.Routes.Count);
            for (int i = 0; i < r.Routes.Count; i++) Console.WriteLine(r.Routes[i]);
            Console.WriteLine("Из них - цепи (нет повторов рёбер):");
            for (int i = 0; i < r.Routes.Count; i++)
                if (Graphs.IsChain(r.Routes[i])) Console.WriteLine(r.Routes[i]);
            Console.WriteLine("Из них - простые цепи (нет повторов рёбер и вершин):");
            for (int i = 0; i < r.Routes.Count; i++)
                if (Graphs.IsSimpleChain(r.Routes[i])) Console.WriteLine(r.Routes[i]);
            Console.WriteLine("Из них - циклы (цепи с одинаковыми концами):");
            for (int i = 0; i < r.Routes.Count; i++)
                if (Graphs.IsCycle(r.Routes[i])) Console.WriteLine(r.Routes[i]);
        }
        public void ShowCheck7()
        {
            Console.WriteLine();
            Console.WriteLine($"Цикломатическое число графа (число рёбер с количеством компонент связности без числа вершин) = {this.CyclomaticN}");
            Vectors p;
            Console.WriteLine();
            Console.WriteLine();
            if (this.ComponCount == 1)
            {
                Console.WriteLine("Матрица Кирхгофа для данного графа:");
                this.Kirhg.PrintMatrix();
                Console.WriteLine("Количество остовов = {0}", this.Kirhg.Minor(1, 1));
                Console.WriteLine("Матрица смежности одного из них (полученного разрушением циклов):");
                this.GetSpanningTree().A.PrintMatrix();
                Console.WriteLine();
                Console.WriteLine("Его код Прюфера (генерация с итогом): ");
                p = Graphs.Pryufer(this.GetSpanningTree());
                p.Show();
                Console.WriteLine();
                Console.WriteLine("Матрица смежности распаковки (генерация рёбер с итогом):");
                Graphs.PryuferUnpacking(p).A.PrintMatrix();

                Console.WriteLine();
                Console.WriteLine("Матрица фундаментальных циклов графа (с выводом):");
                Matrix M = this.FundamentalCycles(this.GetSpanningTree());
                M.PrintMatrix();
                Console.WriteLine();
                Console.WriteLine("Матрица базисных разрезов графа:");
                this.BasisSection(M).PrintMatrix();
            }
            else Console.WriteLine("Циклы в несвязном графе не рассматриваются.");
        }
        public void ShowCheck8()
        {
            Vectors p;
            if (this.ComponCount == 1)
            {
                Console.Write("Точки сочленения: ");
                if (this.JointPoints.Count == 0) Console.WriteLine("таких точек нет");
                else { for (int i = 0; i < this.JointPoints.Count; i++) Console.Write(this.JointVect[i] + " "); Console.WriteLine(); }
                Console.Write("Мосты графа: ");
                if (this.Bridges.Count == 0) Console.WriteLine("отсутствуют");
                else { Console.WriteLine(); for (int i = 0; i < this.Bridges.Count; i++) Console.WriteLine((this.Bridges[i].v1 + 1) + " " + (this.Bridges[i].v2 + 1)); }
                Console.WriteLine();
                Console.WriteLine($"Рёберная связность графа = {this.Lambda} (граф рёберно-{this.Lambda}-связный)");
                Console.WriteLine($"Вершинная связность графа = {this.Kappa} (граф {this.Kappa}-связный)");
                Console.WriteLine("Матрица смежности объединения мостовых блоков (исходный граф без мостов):"); this.BridgeBlocks().A.PrintMatrix();
                Console.WriteLine("Матрица смежности сочленённого блока (максимальный подграф в исходном графе без точек сочленения):");
                this.JointBlock().A.PrintMatrix();
            }
            else
                Console.WriteLine("Характеристики связности в несвязном графе не рассматриваются.");
        }
        public void ShowCheck9()
        {
            Console.WriteLine();
            this.AboutPlanarity();
        }
        public void ShowCheck10()
        {
            Console.WriteLine();
            this.IsEuler();
            this.IsHamilton();
        }
        public void ShowCheck11()
        {
            Vectors p;
            Console.WriteLine();
            //Console.WriteLine("Независимые (внутренне устойчивые) подмножества вершин графа (подмножества наибольшей длины - максимальные, любые подмножества этих подмножеств - тоже независимые подмножества):"); this.ShowIndepSubSets();
            //Console.WriteLine("Наибольшие независимые подмножества (^наибольшие^ значит, что каждая вершина графа вне этого подмножества смежна вершине в подмножестве):"); this.ShowGreatestIndepSubSets();
            //Console.Write("-----------> Число независимости графа = {0}. Вершины максимального множества: ", this.IndependenceNumber(out p));
            int t = this.IndependenceNumber(out p);
            //p.Show();
            p = this.GetColouring();
            Console.WriteLine();
            Console.Write("Одна из раскрасок графа:"); p.Show();
            p = this.GetModifColouring();
            Console.Write("Модифицированная раскраска графа:"); p.Show();
            this.EstimateChromaticNumder();
            Polynom pol = this.Xpolymon();
            Console.Write("Хроматический полином получен по теореме: P(G) = P(G-uv)-P(G, где u~v).");
            Console.WriteLine("Таким образом, полином графа получается из длинной суммы полиномов более простых графов, учитывая то, что полиномы пустых и полных графов известны.");
            Console.Write("Хроматический полином (окончательно как сумма {0} полиномов): ", this.PolCount); pol.Show();
            Console.WriteLine("-----------> Это значит, что хроматическое число X = {0}", this.ChromaticNumber);
        }
        public void ShowCheck12()
        {
            Vectors p;
            Console.WriteLine();
            Console.WriteLine("Независимые (внутренне устойчивые) подмножества вершин графа (подмножества наибольшей длины - максимальные, любые подмножества этих подмножеств - тоже независимые подмножества):"); this.ShowIndepSubSets();
            Console.WriteLine("Наибольшие независимые подмножества (^наибольшие^ значит, что каждая вершина графа вне этого подмножества смежна вершине в подмножестве):"); this.ShowGreatestIndepSubSets();
            Console.Write("-----------> Число независимости графа = {0}. Вершины максимального множества: ", this.IndependenceNumber(out p));
            p.Show();
            Console.WriteLine();
            this.DominSub();
            Console.WriteLine("Доминирующие (внешне устойчивые) множества (записано каждое третье):"); this.ShowDominSub(3);
            Console.WriteLine("Минимальные (не содержащие в себе других) доминирующие множества:"); this.ShowMinDominSub();
            Console.WriteLine("Наименьшие (по мощности) доминирующие  множества:"); this.ShowSmallestDominSub();
            Console.WriteLine("-----------> Число доминирования равно {0}", this.DominationNumber);
            Console.WriteLine();
            Console.WriteLine("Ядро графа (множества вершин, одновременно внутренне и внешне устойчивые):"); Graphs.ShowVectorsList(this.Kernel);
        }
        public void ShowCheck13()
        {
            Console.WriteLine();
            this.VCoatingSub();
            Console.WriteLine("Вершинные покрытия графа (записано каждое третье):"); Graphs.ShowVectorsList(this.VCoatingSubsets, 3);
            Console.WriteLine("Минимальные (не содержащие в себе других) вершинные покрытия:"); Graphs.ShowVectorsList(this.MinimalVCoatingSubsets);
            Console.WriteLine("Наименьшие (по мощности) вершинные покрытия:"); this.ShowSmallestVCoatingSub();
            Console.WriteLine("-----------> Число вершинного покрытия равно {0}", this.VCoatingNumber);
            Console.WriteLine();
            this.ECoatingSub();
            Console.WriteLine("Рёберные покрытия графа (записано каждое пятое):"); Graphs.ShowEdgeListofL(this.ECoatingSubsets, 5);
            Console.WriteLine("Минимальные (не содержащие в себе других) рёберные покрытия (записано каждое третье):"); Graphs.ShowEdgeListofL(this.MinimalECoatingSubsets, 3);
            Console.WriteLine("Наименьшие (по мощности) рёберные покрытия:"); this.ShowSmallestECoatingSub();
            Console.WriteLine("-----------> Число рёберного покрытия равно {0}", this.ECoatingNumber);
            Console.WriteLine();
        }
        public void ShowCheck13Full()
        {
            Console.WriteLine();
            this.VCoatingSub();
            Console.WriteLine("Вершинные покрытия графа (записано каждое третье):"); Graphs.ShowVectorsList(this.VCoatingSubsets, 3);
            Console.WriteLine("Минимальные (не содержащие в себе других) вершинные покрытия:"); Graphs.ShowVectorsList(this.MinimalVCoatingSubsets);
            Console.WriteLine("Наименьшие (по мощности) вершинные покрытия:"); this.ShowSmallestVCoatingSub();
            Console.WriteLine("-----------> Число вершинного покрытия равно {0}", this.VCoatingNumber);
            Console.WriteLine();
            this.ECoatingSubFull();
            Console.WriteLine("Рёберные покрытия графа (записано каждое пятое):"); Graphs.ShowEdgeListofL(this.ECoatingSubsets, 5);
            Console.WriteLine("Минимальные (не содержащие в себе других) рёберные покрытия (записано каждое третье):"); Graphs.ShowEdgeListofL(this.MinimalECoatingSubsets, 3);
            Console.WriteLine("Наименьшие (по мощности) рёберные покрытия:"); this.ShowSmallestECoatingSub();
            Console.WriteLine("-----------> Число рёберного покрытия равно {0}", this.ECoatingNumber);
            Console.WriteLine();
        }
        public void ShowCheck14()
        {
            Console.WriteLine();
            this.CliquesSub();
            Console.WriteLine("Клики графа (вершины, дающие полные подграфы; записан каждый второй):"); Graphs.ShowVectorsList(this.CliquesSubsets, 2);
            Console.WriteLine("Макcимальные (не содержащиеся в других) клики:"); Graphs.ShowVectorsList(this.MaximalCliquesSubsets);
            Console.WriteLine("Наибольшие (по мощности) клики:"); Graphs.ShowVectorsList(this.GreatestCliquesSubsets);
            Console.WriteLine("-----------> Число кликового покрытия равно {0}", this.CliquesNumber);
            Console.WriteLine("-----------> Рёберная плотность графа равна 2q/(p(p-1) = 2*{0}/({1}*{2}) = {3}", this.Edges, this.p, this.p - 1, this.Density);
            Console.WriteLine("Матрица кликов графа:"); this.CliquesMatrix.PrintMatrix();
            Console.WriteLine("-----------> Плотность графа (размерность графа клик) = {0} ", this.CliquesGraph.A.ColCount);
            Console.WriteLine("Матрица смежности графа клик:"); this.CliquesGraph.A.PrintMatrix();

            Console.WriteLine();
            this.MatchingSub();
            Console.WriteLine("Паросочетания графа (записано каждое второе):"); Graphs.ShowEdgeListofL(this.MatchingSubsets, 2);
            Console.WriteLine("Макcимальные (не содержащиеся в других) паросочетания:"); Graphs.ShowEdgeListofL(this.MaximalMatchingSubsets);
            Console.WriteLine("Наибольшие (по мощности) паросочетания:"); Graphs.ShowEdgeListofL(this.GreatestMatchingSubsets);
            Console.WriteLine("-----------> Число паросочетания равно {0}", this.MatchingNumber);
        }

        /// <summary>
        /// Показать информацию о графе в консоли
        /// </summary>
        public void ShowInfoConsole()
        {
            ShowCheck0();
            ShowCheck1();
            ShowCheck2();
            ShowCheck3();
            ShowCheck4();
            ShowCheck5();
            ShowCheck6();
            ShowCheck7();
            ShowCheck8();
            ShowCheck9();
            ShowCheck10();
            ShowCheck11();
            ShowCheck12();
            ShowCheck13();
            ShowCheck14();
        }
        /// <summary>
        /// Вывести информацию о графе в файл
        /// </summary>
        public void ShowInfoFile()
        {
            FileStream fs = new FileStream("Информация о графе.txt", FileMode.Create);
            TextWriter tmp = Console.Out;
            StreamWriter sw = new StreamWriter(fs);
            Console.SetOut(sw);
            this.ShowInfoConsole();
            sw.Close();
            Console.SetOut(tmp);
            Console.WriteLine("Запись завершена!");
        }
    }
}

