using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;// 암호화
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;//정규표현식

namespace ConsoleApplication_MyLibs
{
    /// <summary>
    /// 
    /// 로그를 파싱 하는 test sample. 파싱 구분자 # 으로 고정
    /// </summary>
    class MyClass_Parse_Log
    {
        private char delimiter;
        public MyClass_Parse_Log()
        {
            delimiter = '#'; // 로그 파싱 할 구분자
        }
        public MyClass_Parse_Log(char _delimiter)
        {
            delimiter = _delimiter; // 로그 파싱 할 구분자
        }
        /// <summary>
        /// 샘플 1 솔루션
        /// </summary>
        /// <param name="inputLog"></param>
        /// <param name="outputTxt"></param>
        public void MyClass_Parse_And_Report1(string inputLog, string outputTxt)
        {
            //////////////////////////////////////////////////////
            // log 포맷 시간(19)#타입(2)#메시지코드(9)
            // 입력받은 text log 파일을 열어, #으로 split 한 후 각 type 별로 분리하고 카운팅한 결과를 output text에 저장
            MyClass_Files_Reader srcFile = new MyClass_Files_Reader(inputLog);
            MyClass_Strings strClass = new MyClass_Strings();
            string res = srcFile.readLine(); // 1 line read
            string[] line = srcFile.readLines(inputLog); // read all lines
            int num = line.Count(); // total count
#if DEBUG_PRINT_ENABLE
            MyClass_Dprint.debugPrint("read lines:" + num);
#endif
            ///////////////////////////////////////////////////////
            int typeCount = strClass.countingAllTypes(line, delimiter, 1);
            string[] logTypes = { "EE", "WW", "SS" }; // 세개의 로그타입
            ////////////////////////////////////////////////////////
            // Example #4: Append new text to an existing file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            using (System.IO.StreamWriter outFile = new System.IO.StreamWriter(outputTxt, true))
            {
                // scan types
                foreach (string s in logTypes)
                {
                    //string[] data = strClass.StringSplit(s, delimiter);
                    int cnt = strClass.getCountOfType(line, delimiter, 1, s);
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("type:" + s + " total num:" + cnt);
#endif
                    outFile.WriteLine(s + "#" + cnt);
                }
            }
        }
        public void MyClass_Parse_And_Report2(string inputLog, string outputTxt)
        {
            // log 포맷 시간(19)#타입(2)#메시지코드(9)
            // 입력받은 text log 파일을 열어, #으로 split 한 후 각 type 별로 분리하고 카운팅한 결과를 output text에 저장
            MyClass_Files_Reader srcFile = new MyClass_Files_Reader(inputLog);
            MyClass_Strings strClass = new MyClass_Strings();
            string res = srcFile.readLine(); // 1 line read
            string[] line = srcFile.readLines(inputLog); // read all lines
            int num = line.Count(); // total count
#if DEBUG_PRINT_ENABLE
            MyClass_Dprint.debugPrint("read lines:" + num);
#endif
            int typeCount = strClass.countingAllTypes(line, delimiter, 1);
            string[] logTypes = strClass.getAllTypes(line, delimiter, 1);

            // Example #4: Append new text to an existing file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            using (System.IO.StreamWriter outFile =
                new System.IO.StreamWriter(outputTxt, true))
            {
                //outFile.WriteLine("Fourth line");
                // scan types
                foreach (string s in logTypes)
                {
                    //string[] data = strClass.StringSplit(s, delimiter);
                    int cnt = strClass.getCountOfType(line, delimiter, 1, s);
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("type:" + s + " total num:" + cnt);
#endif
                    outFile.WriteLine(s + "#" + cnt);
                }
            }
        }
        public void MyClass_Parse_And_Report3(string inputLog, string outputTxt)
        {
            // log 포맷 시간(19)#타입(2)#메시지코드(9)
            // 입력받은 text log 파일을 열어, #으로 split 한 후 각 type 별로 분리하고 카운팅한 결과를 output text에 저장
            MyClass_Files_Reader srcFile = new MyClass_Files_Reader(inputLog);
            MyClass_Strings strClass = new MyClass_Strings();
            string res = srcFile.readLine(); // 1 line read
            string[] line = srcFile.readLines(inputLog); // read all lines
            int num = line.Count(); // total count
#if DEBUG_PRINT_ENABLE
            MyClass_Dprint.debugPrint("read lines:" + num);
#endif
            int typeCount = strClass.countingAllTypes(line, delimiter, 1);
            string[] logTypes = strClass.getAllTypes(line, delimiter, 1);
            MyClass_Thread myThread = new MyClass_Thread();
            // Example #4: Append new text to an existing file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            using (System.IO.StreamWriter outFile =
                new System.IO.StreamWriter(outputTxt, false))
            {

                foreach (string s in logTypes)
                {
                    //string[] data = strClass.StringSplit(s, delimiter);
                    int cnt = strClass.getCountOfType(line, delimiter, 1, s);

                    string[] strResult = strClass.getLinesOfType(line, delimiter, 1, s);// 타입별 로그를 추출한다.
                    foreach (string _line in strResult)
                    {

                        // convert and write to file
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter("TYPELOG_3_" + s + ".TXT", false))
                        {
                            string convertedStr = strClass.convertStringMsg(_line, delimiter, 2);
                            {
                                file.WriteLine(convertedStr);
                            }
                        }

                    }
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("type:" + s + " total num:" + cnt);
#endif
                    outFile.WriteLine(s + "#" + cnt);
                }
            }
        }
        public void MyClass_Parse_And_Report4(string inputLog, string outputTxt)
        {
            // log 포맷 시간(19)#타입(2)#메시지코드(9)
            // 입력받은 text log 파일을 열어, #으로 split 한 후 각 type 별로 분리하고 카운팅한 결과를 output text에 저장
            MyClass_Files_Reader srcFile = new MyClass_Files_Reader(inputLog);
            MyClass_Strings strClass = new MyClass_Strings();
            string res = srcFile.readLine(); // 1 line read
            string[] line = srcFile.readLines(inputLog); // read all lines
            int num = line.Count(); // total count
#if DEBUG_PRINT_ENABLE
            MyClass_Dprint.debugPrint("read lines:" + num);
#endif
            int typeCount = strClass.countingAllTypes(line, delimiter, 1);
            string[] logTypes = strClass.getAllTypes(line, delimiter, 1);
            MyClass_Thread myThread = new MyClass_Thread();
            // Example #4: Append new text to an existing file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            using (System.IO.StreamWriter outFile = new System.IO.StreamWriter(outputTxt, false))
            {
                foreach (string s in logTypes)
                {
                    //string[] data = strClass.StringSplit(s, delimiter);
                    int cnt = strClass.getCountOfType(line, delimiter, 1, s);
                    // 타입별 출력 파일 생성
                    MyClass_Files_Writer fileWriter = new MyClass_Files_Writer("TYPELOG_4_" + s + ".TXT");
                    // file writer 클래스에 thread safty하도록 lock 추가. (여러 thread에서 동시에 write의 경우 처리)


                    string[] strResult = strClass.getLinesOfType(line, delimiter, 1, s);// 입력된 타입에 해당되는 로그 만 추출한다.
                    foreach (string _line in strResult)
                    {
                        // 이부분 multi thread로 바꾼다.
#if DEBUG_PRINT_ENABLE
                        MyClass_Dprint.debugPrint("type: " + s);
                        MyClass_Dprint.debugPrint("line: " + _line);
#endif
                        myThread.StartConvertAndWrite(fileWriter, _line, delimiter, 2);
                    }
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("type:" + s + " total num:" + cnt);
#endif
                    outFile.WriteLine(s + "#" + cnt);
                }
            }
        }
    }

    
    /// <summary>
    /// 1원, 10원, 50원, 100원짜리 동전이 제한없이 있다고 가정했을 때, 총 90원을 만드는 방법의 수를 구하는 함수
    /// 리커시브 함수를 사용하여 경우의 수 구한다.
    /// </summary>
    public class MyClass_coinChangeCount
    {
        public void Test()
        {
            int[] coins = { 1, 10, 50, 100 };

            // CoinChangeCount c = new CoinChangeCount();
            // 1.  Recursive 방식 해결
            int ans = this.Count(coins, 4, 90);
#if DEBUG_PRINT_ENABLE
            MyClass_Dprint.debugPrint("Count={0}", ans);
#endif
            // 2. 중간 결과를 저장(Memoization)하여 Lookup하는 방식 해결
            Dictionary<Tuple<int, int>, int> hash = new Dictionary<Tuple<int, int>, int>();
            ans = this.DPCount(coins, 4, 90, hash);
#if DEBUG_PRINT_ENABLE
            MyClass_Dprint.debugPrint("DPCount={0}", ans);
#endif
        }

        public int Count(int[] coins, int m, int n)
        {
            if (n == 0) return 1;
            if (n < 0) return 0;
            if (m <= 0) return 0;
#if DEBUG_PRINT_ENABLE
            MyClass_Dprint.debugPrint("m:{0}, n:{1}", m, n);
#endif
            return Count(coins, m - 1, n) + Count(coins, m, n - coins[m - 1]);
        }

        public int DPCount(int[] coins, int m, int n, Dictionary<Tuple<int, int>, int> hash)
        {
            if (n == 0) return 1;
            if (n < 0) return 0;
            if (m <= 0) return 0;

            Tuple<int, int> pair = new Tuple<int, int>(m, n);
            if (!hash.ContainsKey(pair))
            {
                int result = DPCount(coins, m - 1, n, hash) + DPCount(coins, m, n - coins[m - 1], hash);
                hash.Add(pair, result);
            }
            return hash[pair];
        }
    }
    // 해시 테이블 예제
    public class SimpleHashTable
    {
        private const int INITIAL_SIZE = 16;
        private int size;
        private Node[] buckets;

        public SimpleHashTable()
        {
            this.size = INITIAL_SIZE;
            this.buckets = new Node[size];
        }

        public SimpleHashTable(int capacity)
        {
            this.size = capacity;
            this.buckets = new Node[size];
        }

        public void Put(object key, object value)
        {
            int index = HashFunction(key);
            if (buckets[index] == null)
            {
                buckets[index] = new Node(key, value);
            }
            else
            {
                Node newNode = new Node(key, value);
                newNode.Next = buckets[index];
                buckets[index] = newNode;
            }
        }

        public object Get(object key)
        {
            int index = HashFunction(key);

            if (buckets[index] != null)
            {
                for (Node n = buckets[index]; n != null; n = n.Next)
                {
                    if (n.Key == key)
                    {
                        return n.Value;
                    }
                }
            }
            return null;
        }

        public bool Contains(object key)
        {
            int index = HashFunction(key);
            if (buckets[index] != null)
            {
                for (Node n = buckets[index]; n != null; n = n.Next)
                {
                    if (n.Key == key)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual int HashFunction(object key)
        {
            return Math.Abs(key.GetHashCode() + 1 +
                (((key.GetHashCode() >> 5) + 1) % (size))) % size;
        }

        private class Node
        {
            public object Key { get; set; }
            public object Value { get; set; }
            public Node Next { get; set; }

            public Node(object key, object value)
            {
                this.Key = key;
                this.Value = value;
                this.Next = null;
            }
        }
    }
    /// <summary>
    /// 해시 테이블 사용
    /// </summary>
    public class MyClass_hashMap
    {
        struct Gameharacter
        {
            //Gameharacter(){ }
            public Gameharacter(int CharCd, int Level, int Money)
            {
                _CharCd = CharCd;
                _Level = Level;
                _Money = Money;
            }
            public int _CharCd;
            public int _Level;
            public int _Money;
        };
        /*
        //create comparer
        internal class PersonComparer : IComparer<Gameharacter>
        {
            public int Compare(Gameharacter x, Gameharacter y)
            {
                //first by age
                int result = x._CharCd.CompareTo(y._CharCd);

                //then name
                if (result == 0)
                    result = x._Level.CompareTo(y._Level);

                //a third sort
                if (result == 0)
                    result = x._Money.CompareTo(y._Money);

                return result;
            }
        }
        */
        public void test()
        {
            Hashtable ht = new Hashtable();
            ht.Add("irina", "Irina SP");
            ht.Add("tom", "Tom Cr");

            if (ht.Contains("tom"))
            {
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint(ht["tom"]);
#endif
            }
            ///////////////////////////////

            Hashtable htt = new Hashtable(); // 해시 테이블 생성
            Gameharacter Character1 = new Gameharacter(12, 7, 1000); // 구조체1
            htt.Add(12, Character1); // 추가
            Gameharacter Character2 = new Gameharacter(5, 200, 111000); // 구조체  2
            htt.Add(15, Character2); // 추가
            Gameharacter Character3 = new Gameharacter(200, 34, 3345000); // 구조체 3
            htt.Add(200, Character3); // 추가
#if DEBUG_PRINT_ENABLE
            MyClass_Dprint.debugPrint("before");
#endif
            foreach (DictionaryEntry entry in htt) // 상태 출력
            {
                Gameharacter getStr = (Gameharacter)entry.Value;
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint(entry.Key + ":" + getStr._CharCd);
#endif
            }
            htt.Remove(200); // key 200 제거
#if DEBUG_PRINT_ENABLE
            MyClass_Dprint.debugPrint("after");
#endif
            foreach (DictionaryEntry entry in htt) // 상태 출력
            {
                Gameharacter getStr = (Gameharacter)entry.Value;
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint(entry.Key + ":" + getStr._CharCd);
#endif
            }
            // IDictionary<string, Gameharacter> d = htt; // your hash table
            //var ordered = d.OrderBy(p => p.Key).ToList();
            // foreach (var p in ordered)
            // {
#if DEBUG_PRINT_ENABLE
            //    MyClass_Dprint.debugPrint("Key: {0} Value: {1}", p.Key, p.Value);
#endif
            //  }
            SortedList sorter2 = new SortedList(htt);
            foreach (DictionaryEntry entry in htt) // 상태 출력
            {
                Gameharacter getStr = (Gameharacter)entry.Value;
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint(entry.Key + ":" + getStr._CharCd + "|" + getStr._Level + ":"+getStr._Money);
#endif
            }


            int test = 0;
        }
    }
  
    /// <summary>
    /// 
    /// array를 list로 변환하여 핸들링
    /// </summary>
    //public class MyClass_list_sort<T>// : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable
    public class MyClass_list_sort
    {
        class Product_struct
        {
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public decimal UnitPrice { get; set; }

        }       
        static public void test()
        {
            //Product_struct[] Product_structArray = new[24] Product_struct;
            List<Product_struct> Product = new List<Product_struct>();//정해지지 않은 길이로 생성
            var tmp = new Product_struct();
            tmp.ProductID = 001;
            tmp.ProductName = "바둑이";
            tmp.UnitPrice = 0.0003M; 
            Product.Add(tmp);
            var tmp1 = new Product_struct();
            tmp1.ProductID = 001;
            tmp1.ProductName = "바둑이2";
            tmp1.UnitPrice = 0.0001M;
            Product.Add(tmp1);
            var tmp2 = new Product_struct();
            tmp2.ProductID = 010;
            tmp2.ProductName = "두더쥐";
            tmp2.UnitPrice = 0.0013M;
            Product.Add(tmp2);
            var tmp22 = new Product_struct();
            tmp22.ProductID = 010;
            tmp22.ProductName = "다람쥐";
            tmp22.UnitPrice = 0.1003M;
            Product.Add(tmp22);
            var tmp33 = new Product_struct();
            tmp33.ProductID = 011;
            tmp33.ProductName = "고양이";
            tmp33.UnitPrice = 0.00M;
            Product.Add(tmp33);

            /// SORT
            //람다식 sort // 이름을 비교 - 가나다 오름차순 정렬
            Product.Sort((p1, p2) => p1.ProductName.CompareTo(p2.ProductName)); 
            //람다식 sort2 // productID를 비교 - 1 2 3 오름차순 정렬
            Product.Sort((p1, p2) => p1.ProductID.CompareTo(p2.ProductID));

            // OrderBy 방식의 sort // 지정한 structure member를 기준으로 sorting
            List<Product_struct> _Product = new List<Product_struct>();
            _Product = Product.OrderBy(order => order.ProductName).ToList(); // IEnumerable을 반환하니 list로 변환

            // delegate 방식으로 정렬 // 오름차순 정렬
            Product.Sort(delegate(Product_struct A, Product_struct B)
            {
                // 복합적인 조건으로 compare해서 sort할때 적당하다.
                if (A.UnitPrice > B.UnitPrice) // 뒤
                    return 1;
                else if(A.UnitPrice < B.UnitPrice) // 앞
                    return -1;

                return 0; // 동일
            });
            /// 중복제거
            // list내 중복 제거 // !!!전체가 완전히 같은 경우만 제거되는 문제 있음
            List<Product_struct> _Product_struct1 = new List<Product_struct>(); // 중복 제거한 결과 저장할 리스트 생성
            _Product_struct1 = Product.Distinct().ToList();

            // list내 중복 제거 with ramda // 지정한 structure member를 기준으로 람다식으로 중복확인/제거 후 새로운 struct에 저장
            List<Product_struct> _Product_struct2 = new List<Product_struct>();
            _Product_struct2 = Product.GroupBy(c => c.ProductID).Select(grp => grp.First()).ToList();

            /// 검색
            // list 내 검색 방법 // 람다식으로 검색
            Product_struct result = _Product.Find(x => x.ProductID == 001);

            /// 검색 & count // list내에서 다람쥐로 시작하는 data 갯수 구함
            int count = Product.FindAll(x => x.ProductName.StartsWith("다람쥐")).Count;

            // list 내 검색 + 제거 
            _Product.RemoveAll(x => x.ProductID == 001);
            int test_break = 0;
        }


    }


        
    /// <summary>
    /// 2차원 배열을 사용한 sort 
    /// </summary>
    public class MyClass_array_sort
    {
        // 2차원 배열 input 받아 sorting 후 2차원 배열로 output
        public static int[][]sortedArray(int [][]inData)
        {
            int ln_length = inData.GetLength(0); // 배열 크기
            int ln_length2 = inData[0].GetLength(0); // 배열 크기

            int[][] outArr = new int[ln_length][];
            for (int i = 0; i < inData.GetLength(0); i++)
            {
                outArr[i] = (int[])inData[i].Clone(); // array 복제 (반환되는 object를 array 타입케스팅하여 사용한다.)
            }

            Comparer<int> comparer = Comparer<int>.Default;
            // 람다식을 이용한 IComparer 구현
            Array.Sort<int[]>(inData, (x, y) => comparer.Compare(x[1], y[1])); // 1번째 배열로 오름차순 정렬
            //Array.Sort<int[]>(inData, (x, y) => comparer.Compare(x[0], y[0])); // 0번째 배열로 오름차순 정렬
            Array.Reverse(inData); // 내림차순 정렬로 변환

            // 이중 for문으로 접근
            for(int i=0;i<inData.GetLength(0); i++)
            {
                for(int ii=0; ii< inData[0].GetLength(0); ii++)
                {
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("sorted array>" + inData[i][ii]);
#endif
                }
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint("###");
#endif
            }
            return inData;
        }
        
        /// <summary>
        /// array sorting 예제
        /// </summary>
        public static void test()
        {
            // could just as easily be string...
            int[][] data = new int[][] {
                new int[] {10,100},
                new int[] {2,30},
                new int[] {3,500},
                new int[] {4,70}
            };
            int[][] outData = MyClass_array_sort.sortedArray(data);
            int test = 0;
        }
    }

    // 도서관리 프로그램 샘플
    public class MyClass_Book
    {
        public int BNum
        {
            get;
            private set;
        }
        public string Title
        {
            get;
            private set;
        }
        public string Author
        {
            get;
            private set;
        }
        public string Publisher
        {
            get;
            private set;
        }
        public int Price
        {
            get;
            private set;
        }
        public MyClass_Book(int bnum, string title, string author, string pub, int price)
        {
            BNum = bnum;
            Title = title;
            Author = author;
            Publisher = pub;
            Price = price;
        }
        public override string ToString()
        {
            return Title;
        }
    }

    // 도서관리 프로그램 샘플
    class MyClass_Books
    {
        MyClass_Book[] books = new MyClass_Book[100];
        public MyClass_Book this[int bnum]
        {
            get
            {
                int i = 0;
                for (i = 0; i < books.Length; i++)
                {
                    if (books[i] == null)
                    {
                        break;
                    }
                    if (books[i].BNum == bnum)
                    {
                        return books[i];
                    }
                }
                return null;
            }
            set
            {
                int i = 0;
                for (i = 0; i < books.Length; i++)
                {
                    if ((books[i] == null) || (books[i].BNum == bnum))
                    {
                        break;
                    }
                }
                if (i == books.Length)
                {
                    return;
                }
                books[i] = value;
            }
        }

        internal void ViewAll()
        {
            for (int i = 0; i < books.Length; i++)
            {
                if (books[i] == null)
                {
                    break;
                }
                ViewBook(books[i]);
            }
        }

        private void ViewBook(MyClass_Book book)
        {
            Console.WriteLine("<{0}>", book.BNum);
            Console.WriteLine("\t제목:{0}", book.Title);
            Console.WriteLine("\t출판사:{0}", book.Publisher);
            Console.WriteLine("\t저자:{0}", book.Author);
            Console.WriteLine("\t가격:{0}", book.Price);
        }
    }
    public class MyClass_BookManager
    {
        MyClass_Books books = new MyClass_Books();
        internal void Run()
        {
            ConsoleKey key = ConsoleKey.NoName;
            while ((key = SelectMenu()) != ConsoleKey.Escape)//반복(메뉴 선택 한 것이 ESC가 아니라면)
            {
                switch (key)//선택한 메뉴에 따라
                {
                    case ConsoleKey.F1: Insert(); break;//F1이면 추가
                    case ConsoleKey.F2: Delete(); break;//F2이면 삭제
                    case ConsoleKey.F3: Search(); break;//F3이면 조회
                    case ConsoleKey.F4: books.ViewAll(); break;//F4이면 전체 보기
                    default: Console.WriteLine("잘못 선택하였습니다."); break;
                }
                Console.WriteLine("아무키나 누르세요.");
                Console.ReadKey(true);
            }

        }



        private void ViewBook(MyClass_Book book)
        {
            Console.WriteLine("<{0}>", book.BNum);
            Console.WriteLine("\t제목:{0}", book.Title);
            Console.WriteLine("\t출판사:{0}", book.Publisher);
            Console.WriteLine("\t저자:{0}", book.Author);
            Console.WriteLine("\t가격:{0}", book.Price);
        }

        private void Search()
        {
            //조회할 번호 입력
            int num = 0;
            Console.WriteLine("조회할 도서 번호를 입력:");
            num = int.Parse(Console.ReadLine());

            if (books[num] == null)
            {
                Console.WriteLine("{0}번: 입력하지 않음", num);
            }
            else
            {
                ViewBook(books[num]);
            }
        }

        private void Delete()
        {
            //삭제할 번호 입력
            int num = 0;
            Console.WriteLine("삭제할 도서 번호를 입력:");
            num = int.Parse(Console.ReadLine());

            if (books[num] != null)
            {
                books[num] = null;
            }
        }

        private void Insert()
        {
            //추가할 번호 입력
            int num = 0;
            Console.WriteLine("추가할 도서 번호를 입력:");
            num = int.Parse(Console.ReadLine());

            if (books[num] != null)
            {
                Console.WriteLine("이미 존재합니다.");
                return;
            }
            string title;
            Console.WriteLine("제목");
            title = Console.ReadLine();
            string author;
            Console.WriteLine("저자");
            author = Console.ReadLine();
            string publisher;
            Console.WriteLine("출판사");
            publisher = Console.ReadLine();
            int price = 0;
            Console.WriteLine("가격");
            int.TryParse(Console.ReadLine(), out price);
            books[num] = new MyClass_Book(num, title, author, publisher, price);
        }

        private ConsoleKey SelectMenu()
        {
            Console.Clear();
            Console.WriteLine("도서 관리 프로그램(인덱서 실습) 메뉴");
            Console.WriteLine("F1: 도서 추가 F2:도서 삭제 F3:도서 조회 F4:전체 보기");
            Console.WriteLine("ESC: 프로그램 종료");
            return Console.ReadKey(true).Key;
        }


    }


    // 테스트 샘플들
    public class MyTest
    {
        public MyTest()
        {

        }
        ~MyTest() { }

        
        public void run_Test()
        {
#if false
            // 도서관리 sample
            MyClass_BookManager bm = new MyClass_BookManager();
            bm.Run();
#endif
#if false
            // 100의 bitcoins 전송 sample //
            List<MyClassBlockChain> blockchain = new List<MyClassBlockChain>();
            // Genesis block
            string[] transactions = { "Jone Sent 100 Bitcoins to Bob." };
            // 최초 노드 : genesisBlock
            myClassBlockChainHeader blockheader = new myClassBlockChainHeader(null, transactions);
            MyClassBlockChain genesisBlock = new MyClassBlockChain(blockheader, transactions);
            Console.WriteLine("Block Hash : {0}", genesisBlock.getBlockHash());

            Stopwatch stopw = new Stopwatch();//시간 측정 클래스
            MyClassBlockChain previousBlock = genesisBlock;
            for (int i = 0; i < 5; i++)
            {
                myClassBlockChainHeader secondBlockheader = new myClassBlockChainHeader(Encoding.UTF8.GetBytes(previousBlock.getBlockHash()), transactions);
                MyClassBlockChain nextBlock = new MyClassBlockChain(secondBlockheader, transactions);
                stopw.Start();
                int count = secondBlockheader.ProofOfWorkCount();
                stopw.Stop();
                Console.WriteLine("{0} th Block Hash : {1}", i.ToString(), nextBlock.getBlockHash());
                Console.WriteLine("   └ COUNT of Proof of Work : {0} th loop", count);
                Console.WriteLine("   └ Delay : {0} millisecond", stopw.ElapsedMilliseconds);
                previousBlock = nextBlock;
                stopw.Reset();
            }
#endif

            // Array to List
            int[] ints = new[] { 10, 20, 10, 34, 113 };
            List<int> lst = ints.OfType<int>().ToList();
            lst.AddRange(new int[] { 10, 20, 10, 34, 113 });
            // List to Array
            int [] intsArray = lst.ToArray();

            //중복되지 않는 첫번째 문자 구하기 // dictionary 사용
            string s = "abcabdefe";
            char ch = MyClass_Strings.GetFirstChar(s.ToCharArray());
#if DEBUG_PRINT_ENABLE
            MyClass_Dprint.debugPrint(ch);
#endif

            // 문자열 내 문자로 가능한 조합 구하기
            MyClass_Strings.getStringCombination("ABC");

            // string 내 패턴 스트링 카운팅
            string sampleData = "asdfkk;lkasldfajsdlkfj999kajsdlfkjasdlfkj9999kljflaskdflkajsdlfkjasdlkfj999lkajsdlfkjasldfkja999";
            // 정규식 매칭 확인 + count
            //int countResult = System.Text.RegularExpressions.Regex.Matches(sampleData, "999").Count;
            int countResult = MyClass_Strings.countMatches(sampleData, "999");
            Match matchRes = Regex.Match(sampleData, "999");
            // recursive하게 sub folder에서 유일한 file 찾기 예
            string fullPath = MyClass_Files.findFileFromSubFolder(".", "test.txt"); 

            // 스트링내 캐릭터 조합 비교 예 
            string res = string.Empty;
            int ress = MyClass_Strings.checkCharsInString("coffee", "fofeefac", ref res);

            // 라인브레이크 사용자설정 예  // append mode on/off 예
            MyClass_Files_Writer fileWriter = new MyClass_Files_Writer("fileWrite.TXT", false);
            fileWriter.customNewLine = "\r\n";
            fileWriter.WriteToFile("asdfasdfasdfasdf");
            fileWriter.WriteToFile("asdfasdfasdfasdf");
            fileWriter.WriteToFile("asdfasdfasdfasdf");
            // 소스의 라인브레이크 디텍션 예    
            MyClass_Files_Reader reader = new MyClass_Files_Reader("fileWrite.TXT");
            string getNewLineChar = reader.getNewLine(); // newLine 형식 읽어와서 file writer의 customNewLine에도 맞춰주면 된다.

            // 암호화 예
            string str_test = "3#ABCDEFGHIJKLMNOPQRSTUVWXYZ";            
            string str_Enc = MyClass_Security.CaesarCipherEncrypt(str_test, 20);
            string str_test2 = "21#abcdefghijklmnopqrstuvwxyz";
            string str_Enc2 = MyClass_Security.CaesarCipherEncrypt(str_test2, 20);

            //


            // 메서드로 바꿀것
            // ABC 부품을 갖고 A7 B7 C7이 순서대로 오면 제품 조립 ok 
            // 총 완성 제품 counting
            string str_Product_struct = "A2B3A7B7C7B2C7A9B4A9B8C7A2B7C9";
            char[] array_Product_struct = str_Product_struct.ToCharArray();
            int status = 0; //1:A ok, 2:B ok, 3:C ok
            int totalProductCount = 0;
            for(int i = 0; i<array_Product_struct.Length; i=i+2)
            {
                string num = string.Empty;
                    num += array_Product_struct[i + 1];
                int part_count = Convert.ToInt32(num);
                //int level2 = Convert.ToInt32("-1024");
                //int level = (int)Char.GetNumericValue(array_Product_struct[i + 1]);//Convert.ToInt32(array_Product_struct[i + 1]);
                if (array_Product_struct[i] == 'A')
                {
                    if (part_count >= 7)
                    {
                        status = 1;
                    }
                    else
                    {
                        status = 0;
                    }
                }
                if (array_Product_struct[i] == 'B' && status == 1)
                {
                    if(part_count >= 7 )
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 0;
                    }

                }
                if (array_Product_struct[i] == 'C' && status == 2)
                {
                    if (part_count >= 7)
                    {
                        //status = 3;
                        // count up 
                        status = 0;
                        totalProductCount++;
                    }
                    else
                    {
                        status = 0;
                    }
                }
            }
#if DEBUG_PRINT_ENABLE
            MyClass_Dprint.debugPrint("totalProductCount: " + totalProductCount);
#endif

            // 구조체 타입 list의 정렬 예
            //MyClass_list_sort<int> lSortTest = new MyClass_list_sort<int>();
            MyClass_list_sort.test();
            //MyClass_array_sort sortTest = new MyClass_array_sort();
            MyClass_array_sort.test();

            // 헤시맵을 이용한 정렬 TODO
            MyClass_hashMap hashTest = new MyClass_hashMap();  // TODO...
            hashTest.test();

            // 링크드리스트 사용 예
            //MyClass_linkedList list = new MyClass_linkedList();
            MyClass_linkedList.test();

            // 동전바꿈 예
            MyClass_coinChangeCount cnt = new MyClass_coinChangeCount();
            cnt.Test();

            
            // MyClass_Parse_Log parse = new MyClass_Parse_Log('#'); // delimitor
            // 문제 sample //
            // parse.MyClass_Parse_And_Report1("LOGFILE_A.TXT", "REPORT_1.TXT");            // 문제 2
            //parse.MyClass_Parse_And_Report2("LOGFILE_B.TXT", "REPORT_2.TXT");
            //parse.MyClass_Parse_And_Report3("LOGFILE_B.TXT", "REPORT_3.TXT");
            //parse.MyClass_Parse_And_Report4("LOGFILE_B.TXT", "REPORT_4.TXT");


            // 암호/복호화 sample //
            String originalText = "plain text";
            String key = "key";
            String en = MyClass_Security.Encrypt(originalText, key);
            String de = MyClass_Security.Decrypt(en, key);
#if DEBUG_PRINT_ENABLE
            MyClass_Dprint.debugPrint("Original Text is " + originalText);
            MyClass_Dprint.debugPrint("Encrypted Text is " + en);
            MyClass_Dprint.debugPrint("Decrypted Text is " + de);
#endif
            MyClass_Networks.StartServerWithThread();
            //string testSend = MyClass_Networks.StartClientSync("1111111");
            string testSend2 = MyClass_Networks.StartClientSync("send data 111111 ", false);
            //testSend2 = MyClass_Networks.StartClientSync("222222 ", false);
            testSend2 = MyClass_Networks.StartClientSync("test <EOF>", true);
#if DEBUG_PRINT_ENABLE
            // MyClass_Dprint.debugPrint("start StartAsyncClient");
#endif
            //MyClass_Networks.StartAsyncClient("test <EOF>");
            //MyClass_Networks.StartAsyncClient("<EOF>");
#if DEBUG_PRINT_ENABLE
            MyClass_Dprint.debugPrint("abort socket server thread");
#endif

            MyClass_Networks.StopServerWithThread();

            // 폴더 스캔 샘플 //
            MyClass_Files.TreeScan(".\\..");
            // 지정한 확장자를 가진 파일을 폴더내에서 찾아 list로 update
            string[] resultt = MyClass_Files.scanFolderAndUpdate_Filelists(".", "cs");

            // 로그파일 쓰기 예 //
            MyClass_Logger log = new MyClass_Logger("restLog.txt");
            log.WriteEntry("TTTTTTTTTTTTTTTTTTTTTTTTT");

            // 링버퍼 샘플 //
            MyClass_RandomData rndData = new MyClass_RandomData();
            rndData.rnd = new Random(); // random 초기화..
            var data = rndData.GenerateRandomData(10); // 100개의 random 데이터 생성
            // MyClass_CircularBuffer<타입>(갯수)
            var buffer = new MyClass_CircularBuffer<byte>(100); // 링버퍼 생성
            buffer.Put(data); // 10 byte push
            data = rndData.GenerateRandomData(10);
            buffer.Put(data);
            //TestTools.UnitTesting.CollectionAssert.AreEqual(data, buffer.ToArray());
            var ret = new byte[10];//[buffer.Size];
            buffer.Get(ret);
            buffer.Get(ret); // buffer size만큼 get

            //CollectionAssert.AreEqual(data, ret);
            //Assert.IsTrue(buffer.Size == 0);

            int test = 2;
        }

    }// end class
}// end namespace

