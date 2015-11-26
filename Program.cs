using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubleDescription
{
    class Program
    {
        struct minInLine
        {
            public float a;
            public int pos;
            public void Print()
            {
                Console.WriteLine("Element: {0} Pos: {1}", a, pos);
            }
        }
        struct Otvet
        {
            public float V;
            public float[] X;

            public Otvet(float v, params float[] x)
            {
                if (v < 0)
                    V = -v;
                else
                    V = v;
                X = x;                
            }
            public void Print()
            {
                Console.Write("Цена игры равна: {0}\nСтратегия: (", V);
                for (int i = 0; i < X.Length - 1; i++)
                    Console.Write("{0} , ",X[i]);
                Console.WriteLine("{0})",X[X.Length - 1]);
            }
        }
        class LineOfTabl
        {
            protected float[] dot;
            protected float z, d;
            protected bool[] p, a;

            public LineOfTabl (LineOfTabl other)
            {
                this.dot = new float[other.dot.Length];
                for (int i = 0; i < other.dot.Length; i++)
                    this.dot = other.dot;
                this.z = other.z;
                this.d = other.d;
                this.p = new bool[other.p.Length];
                for (int i = 0; i < other.p.Length; i++)
                    this.p = other.p;
                this.a = new bool[other.a.Length];
                for (int i = 0; i < other.a.Length; i++)
                    this.a = other.a;
            }
            public LineOfTabl(float[][] matrix)
            {
                dot = new float[matrix.Length];
                p = new bool[matrix.Length];
                a = new bool[matrix[0].Length];
            }
            public void DotSet(params float[] val)
            {
                for (int i = 0; i < val.Length; i++)
                    dot[i] = val[i];
            }
            public float[] GetDot()
            {
                return dot;
            }
            public float GetZ()
            {
                return z;
            }
            public float GetD()
            {
                return d;
            }
            public bool[] GetP()
            {
                return p;
            }
            public bool[] GetA()
            {
                return a;
            }
            public virtual void CalculateZ(float[][] matrix, int k)
            {
                float count = 0;
                for (int i = 0; i < matrix.Length; i++)
                    count += dot[i] * matrix[i][k];
                z = count;
            }
            public virtual void Cross(int k)
            {
                int count = 1;
                for (int i = 0; i < dot.Length; i++)
                    if (dot[i] == 0)
                    {
                        count++;
                        p[i] = true;
                    }
                    else p[i] = false;
                for (int i = 0; i < k; i++)
                    a[i] = false;
                for (int i = k - 1; (i >= 0) && (count < p.Length); i--)
                {
                    count++;
                    a[i] = true;
                }
                a[k] = true;
            }
            public void CalculateD(float[][] matrix, int k)
            {
                float count = 0;
                if (k < matrix[0].Length - 1)
                    for (int i = 0; i < dot.Length; i++)
                        count += dot[i] * matrix[i][k + 1];
                else
                    for (int i = 0; i < dot.Length; i++)
                        count += dot[i] * matrix[i][matrix[i].Length - 1];
                d = count - GetZ();
            }
            public void DoAllRight(float[][] matrix, int k)
            {
                CalculateZ(matrix,k);
                Cross(k);
                CalculateD(matrix, k);
            }
            public virtual void Print(int n,int k)
            {
                Console.Write("| {0} |", n);
                for (int i = 0; i < dot.Length; i++)
                    Console.Write("{0:0.00} ",dot[i]);
                Console.Write("| {0:0.00} ", z);
                for (int i = 0; i < p.Length; i++)
                    if (p[i] == true)
                        Console.Write("| x ");
                    else
                        Console.Write("|   ");
                for (int i = 0; i < a.Length; i++)
                    if (a[i] == true)
                        Console.Write("| x ");
                    else
                        Console.Write("|   ");
                Console.Write("| {0:0.00} |", d);
            }
            
        }
        class LineOfTablCross: LineOfTabl
        {
            private LineOfTabl one, too;
            private int Pos1,Pos2;
            public LineOfTablCross(float[][] matrix, List<LineOfTabl> Table, int A, int B)
                :base (matrix)
            {
                one = new LineOfTabl(Table[A]);
                too = new LineOfTabl(Table[B]);
                Pos1 = A;
                Pos2 = B;
                for (int i = 0; i < dot.Length; i++)
                    dot[i] = Formul(one.GetD(), too.GetD(), one.GetDot()[i], too.GetDot()[i]);
            }
            static float Formul(float d1, float d2,float p1, float p2)
            {
                return (-d2 * p1 + d1 * p2) / (d1 - d2);
            }
            public override void CalculateZ(float[][] matrix, int k)
            {
                if (d >= 0)
                    z = Formul(one.GetD(), too.GetD(), one.GetZ(), too.GetZ());
                else
                {
                    float count = 0;
                    for (int i = 0; i < matrix.Length; i++)
                        count += dot[i] * matrix[i][k];
                    z = count;
                }
                    
            }
            public override void Cross(int k)
            {
                int count = 1;
                for (int i = 0; i < p.Length; i++)
                    if (one.GetP()[i] == too.GetP()[i] && one.GetP()[i] == true)
                    {
                        p[i] = one.GetP()[i];
                        count++;
                    }
                for (int i = 0; i < k - 1; i++)
                    a[i] = false;
                for (int i = k - 1; (i >= 0) && (count < p.Length); i--)
                    if (one.GetA()[i] == too.GetA()[i] && one.GetA()[i] == true)
                    {
                        a[i] = one.GetA()[i];
                        count++;
                    }
                a[k] = true;
            }
            public void SetPos()
            {
                Pos1 = Pos2;
            }
            public override void Print(int n, int k)
            {
                base.Print(n, k);
                if(Pos1 != Pos2)
                    Console.Write(" ({0} - {1}) ", Pos2 + 1, Pos1 + 1);
            }
        }
        static float[][] ReadMatrix(string FileName)
        {
            string[] sInput = File.ReadAllLines(FileName);
            float[][] matrix = new float[sInput.Length][];
            int cur = 0;
            foreach (string s in sInput)
            {
                matrix[cur] = s.Split(' ').Select(x => float.Parse(x)).ToArray();
                cur++;
            }
            MatrixNormalize(matrix);
            return matrix;
        }
        static void MatrixNormalize(float[][] matrix)
        {
            minInLine[] ArrayOfMin = new minInLine[matrix.Length];
            for (int i = 0; i < matrix.Length; i++)
            {
                ArrayOfMin[i].a = matrix[i].Min();
                ArrayOfMin[i].pos = Search(matrix[i], ArrayOfMin[i].a);
            }
            for (int k = 0; k < ArrayOfMin.Length - 1; k++)
                for (int i = 0; i < ArrayOfMin.Length - k - 1; i++)
                    if (ArrayOfMin[i].a > ArrayOfMin[i + 1].a)
                        Swap(ref ArrayOfMin[i], ref ArrayOfMin[i + 1]);
            for (int i = 0; i < ArrayOfMin.Length; i++)
                SwapColloms(matrix, i, ArrayOfMin[i].pos);
        }
        static float[][] ChangeMatrix(float[][] matrix)
        {
            float[][] matrixSecond = new float[matrix[0].Length][];
            for (int i = 0; i < matrixSecond.Length; i++)
                matrixSecond[i] = new float[matrix.Length];
            for (int i = 0; i < matrix[0].Length; i++)
                for (int j = 0; j < matrix.Length; j++)
                    matrixSecond[i][j] = (-1)*matrix[j][i];
            return matrixSecond;
        }
        static void PrintMatrix(float[][] matrix)
        {
            foreach (float[] a in matrix)
            {
                foreach (float i in a)
                    Console.Write("{0} ", i);
                Console.WriteLine();
            }
        }
        static void PrintHeader(float[][] matrix)
        {
            Console.Write("| N |");
            for (int i = 0; i < matrix.Length; i++)
                Console.Write("x{0}   ",i + 1);
            Console.Write("|  Z   ");
            for (int i = 0; i < matrix.Length; i++)
                Console.Write("|П{0} ",i + 1);
            for (int i = 0; i < (matrix[0].Length); i++)
                Console.Write("|a{0} ",i + 1);
            Console.Write("|  D   |\n");
        }
        static void SwapColloms(float[][] matrix, int k, int l)
        {
            for (int i = 0; i < matrix.Length; i++)
                Swap(ref matrix[i][k], ref matrix[i][l]);
        }
        static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
        static int Search(float[] arr, float a)
        {
            for (int i = 0; i < arr.Length; i++)
                if (a == arr[i])
                    return i;
            return -1;
        }
        static void Standart(float[][] matrix, List<LineOfTabl> Table)
        {
            float[] DotSt = new float[matrix.Length];
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < DotSt.Length; j++)
                    if (j != i)
                        DotSt[j] = 0;
                    else
                        DotSt[j] = 1;
                Table.Add(new LineOfTabl(matrix));
                Table[i].DotSet(DotSt);
                Table[i].DoAllRight(matrix, 0);
                Table[i].Print(i, 0);
                Console.WriteLine();
            }
        }

        static bool CheckD(List<LineOfTabl> Table)
        {
            foreach (LineOfTabl Line in Table)
                if (Line.GetD() < 0)
                    return false;
            return true;            
        }
        static bool CheckCross(List<LineOfTabl> Table, int a, int b)
        {
            int count = 0;
            for (int i = 0; i < Table[a].GetP().Length; i++)
                if ((Table[a].GetP()[i] == Table[b].GetP()[i]) && (Table[a].GetP()[i] == true))
                    count++;
            for (int i = 0; i < Table[a].GetA().Length; i++)
                if ((Table[a].GetA()[i] == Table[b].GetA()[i]) && (Table[a].GetA()[i] == true))
                    count++;
            if (count < Table[a].GetP().Length - 1)
                return false;
            else
                return true;
        }
        static Otvet DoubleDescription(float[][] matrix)
        {
            Otvet otvet;
            int TableLen;
            Console.WriteLine();
            //Begin
            PrintHeader(matrix);
            //Step 1
            List<LineOfTabl> Table = new List<LineOfTabl>();
            Standart(matrix, Table);
            //Step next
            for (int k = 1; (k < matrix[0].Length) && !(CheckD(Table)); k++)
            {
                TableLen = Table.Count;
                Console.WriteLine("***********************Step next**************************");
                for (int i = 0; i < TableLen; i++)
                    if (Table[i].GetD() < 0)
                        for (int j = 0; j < TableLen; j++)
                            if (Table[j].GetD() >= 0 && CheckCross(Table, j, i))
                                Table.Add(new LineOfTablCross(matrix, Table, j, i));
                for (int i = TableLen; i < Table.Count; i++)
                    Table[i].DoAllRight(matrix, k);
                for (int i = 0; i < TableLen; i++)
                {
                    LineOfTablCross SPos = Table[i] as LineOfTablCross;
                    if (SPos != null)
                        SPos.SetPos();

                    if (Table[i].GetD() < 0)
                    {
                        Table[i].DoAllRight(matrix, k);
                        Table[i].Print(i + 1, k);
                        Console.WriteLine();
                    }
                    else
                    {
                        if (Table[i] is LineOfTablCross)
                            Table[i].CalculateZ(matrix, k);
                        Table[i].CalculateD(matrix, k);
                        Table[i].Print(i + 1, k);
                        Console.WriteLine();
                    }
                }
                for (int i = TableLen; i < Table.Count; i++)
                {
                    Table[i].Print(i + 1, k);
                    Console.WriteLine();
                }

            }
            int max = 0;
            for (int i = 1; i < Table.Count; i++)
                if (Table[max].GetZ() < Table[i].GetZ())
                    max = i;
            return otvet = new Otvet(Table[max].GetZ(), Table[max].GetDot());
        }
        static void Main(string[] args)
        {
            string InputFile = @"C:\MyData\input.txt";
            float[][] matrix = ReadMatrix(InputFile);
            Otvet First;
            PrintMatrix(matrix);
            First = DoubleDescription(matrix);
            First.Print();
            
            Console.WriteLine("Решение для второго игрока:");
            float[][] matrixSecond = ChangeMatrix(matrix);
            Otvet Second;
            PrintMatrix(matrixSecond);
            Second = DoubleDescription(matrixSecond);
            Second.Print();
            

            Console.Read();
        }
     }  
}
