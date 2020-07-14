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



// 1. MyClass_CircularBuffer : 링버퍼 데이터 구조 클래스
// 2. MyClass_linkedList : 링크드 리스트 데이터 구조 클래스

namespace ConsoleApplication_MyLibs
{
    public class MyClass_RandomData
    {
        public MyClass_RandomData()
        {

        }
        public byte[] GenerateRandomData(int length)
        {
            var data = new byte[length];
            rnd.NextBytes(data);
            return data;
        }
        public Random rnd;
    }
    /// <summary> 
    /// 링버퍼
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyClass_CircularBuffer<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable
    {
        private int capacity;
        private int size;
        private int head;
        private int tail;
        private T[] buffer;

        [NonSerialized()]
        private object syncRoot;

        public MyClass_CircularBuffer(int capacity)
            : this(capacity, false)
        {
        }

        public MyClass_CircularBuffer(int capacity, bool allowOverflow)
        {
            if (capacity < 0)
                throw new ArgumentException("The buffer capacity must be greater than or equal to zero.", "capacity");

            this.capacity = capacity;
            size = 0;
            head = 0;
            tail = 0;
            buffer = new T[capacity];
            AllowOverflow = allowOverflow;
        }

        public bool AllowOverflow
        {
            get;
            set;
        }

        public int Capacity
        {
            get { return capacity; }
            set
            {
                if (value == capacity)
                    return;

                if (value < size)
                    throw new ArgumentOutOfRangeException("value", "The new capacity must be greater than or equal to the buffer size.");

                var dst = new T[value];
                if (size > 0)
                    CopyTo(dst);
                buffer = dst;

                capacity = value;
            }
        }

        public int Size
        {
            get { return size; }
        }

        public bool Contains(T item)
        {
            int bufferIndex = head;
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < size; i++, bufferIndex++)
            {
                if (bufferIndex == capacity)
                    bufferIndex = 0;

                if (item == null && buffer[bufferIndex] == null)
                    return true;
                else if ((buffer[bufferIndex] != null) &&
                    comparer.Equals(buffer[bufferIndex], item))
                    return true;
            }

            return false;
        }

        public void Clear()
        {
            size = 0;
            head = 0;
            tail = 0;
        }

        public int Put(T[] src)
        {
            return Put(src, 0, src.Length);
        }

        public int Put(T[] src, int offset, int count)
        {
            if (!AllowOverflow && count > capacity - size)
                throw new InvalidOperationException("The buffer does not have sufficient capacity to put new items.");

            int srcIndex = offset;
            for (int i = 0; i < count; i++, tail++, srcIndex++)
            {
                if (tail == capacity)
                    tail = 0;
                buffer[tail] = src[srcIndex];
            }
            size = Math.Min(size + count, capacity);
            return count;
        }

        public void Put(T item)
        {
            if (!AllowOverflow && size == capacity)
                throw new InvalidOperationException("The buffer does not have sufficient capacity to put new items.");

            buffer[tail] = item;
            if (++tail == capacity)
                tail = 0;
            size++;
        }

        public void Skip(int count)
        {
            head += count;
            if (head >= capacity)
                head -= capacity;
        }

        public T[] Get(int count)
        {
            var dst = new T[count];
            Get(dst);
            return dst;
        }

        public int Get(T[] dst)
        {
            return Get(dst, 0, dst.Length);
        }

        public int Get(T[] dst, int offset, int count)
        {
            int realCount = Math.Min(count, size);
            int dstIndex = offset;
            for (int i = 0; i < realCount; i++, head++, dstIndex++)
            {
                if (head == capacity)
                    head = 0;
                dst[dstIndex] = buffer[head];
            }
            size -= realCount;
            return realCount;
        }

        public T Get()
        {
            if (size == 0)
                throw new InvalidOperationException("The buffer is empty.");

            var item = buffer[head];
            if (++head == capacity)
                head = 0;
            size--;
            return item;
        }

        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            CopyTo(0, array, arrayIndex, size);
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            if (count > size)
                throw new ArgumentOutOfRangeException("count", "The read count cannot be greater than the buffer size.");

            int bufferIndex = head;
            for (int i = 0; i < count; i++, bufferIndex++, arrayIndex++)
            {
                if (bufferIndex == capacity)
                    bufferIndex = 0;
                array[arrayIndex] = buffer[bufferIndex];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            int bufferIndex = head;
            for (int i = 0; i < size; i++, bufferIndex++)
            {
                if (bufferIndex == capacity)
                    bufferIndex = 0;

                yield return buffer[bufferIndex];
            }
        }

        public T[] GetBuffer()
        {
            return buffer;
        }

        public T[] ToArray()
        {
            var dst = new T[size];
            CopyTo(dst);
            return dst;
        }

        #region ICollection<T> Members

        int ICollection<T>.Count
        {
            get { return Size; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        void ICollection<T>.Add(T item)
        {
            Put(item);
        }

        bool ICollection<T>.Remove(T item)
        {
            if (size == 0)
                return false;

            Get();
            return true;
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICollection Members

        int ICollection.Count
        {
            get { return Size; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (syncRoot == null)
                    Interlocked.CompareExchange(ref syncRoot, new object(), null);
                return syncRoot;
            }
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            CopyTo((T[])array, arrayIndex);
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        #endregion
    }

    // C#의 리스트를 활용한 클래스 리스트 sorting
    public class Myclass_listData
    {
        public int nID; // 멤버변수 숫자타입
        string strName; // 멤버변수 문자열타입
        public Myclass_listData() // 생성자
        {

        }
        public Myclass_listData(int _nID, string _strName)// argument가진 생성자
        {
            nID = _nID;
            strName = _strName;
        }
        public void Print() // 디버깅용 메서드
        {
            Debug.Print("ID : " + nID.ToString() + ", Name : " + strName);
        }

        public static void testClass()
        {
            List<Myclass_listData> IData = new List<Myclass_listData>(); // 클래스 리스트 // 구조체 리스트 보다 더 쉽게 사용

            // 정렬할 데이터 샘플 램덤으로 5개 생성/입력
            Random r = new Random();
            for(int nIndex = 0; nIndex < 5; nIndex++)
            {                
                r.Next();
                int nID = r.Next(0, 101);//System.Random.(0, 101);
                IData.Add(new Myclass_listData(nID, nID.ToString())); // 리스트에 추가
            }

            Debug.Print("<color=red>정렬 전</color>");
            for(int nIndex = 0; nIndex<IData.Count; nIndex++)
            {
                IData[nIndex].Print();
            }
            // 정렬 예제

            IData.Sort(delegate (Myclass_listData A, Myclass_listData B) // 1. 오름차순 문자 정렬 예제
            {
                return A.strName.CompareTo(B.strName);
                
            });
            
            IData.Sort(delegate(Myclass_listData A, Myclass_listData B) // 2. 오름차순 숫자 정렬 예제
            {
                if (A.nID > B.nID) return 1;
                else if (A.nID < B.nID) return -1;
                return 0;
            });
            
            Debug.Print("<color=red>정렬 후</color>");
            for (int nIndex = 0; nIndex < IData.Count; nIndex++)
            {
                IData[nIndex].Print();
            }
        }
    }


   

    /// <summary>
    /// 링크드 리스트의 예
    /// </summary>
    public class MyClass_linkedList
    {

        public static void test()
        {
            LinkedList<string> list = new LinkedList<string>();
            list.AddLast("ZApple");
            list.AddLast("CBanana");
            list.AddLast("Lemon");

            LinkedListNode<string> node = list.Find("Banana");
            LinkedListNode<string> newNode = new LinkedListNode<string>("Grape");

            // 새 Grape 노드를 Banana 노드 뒤에 추가
            if (node != null)
                list.AddAfter(node, newNode);

            // 리스트 출력
            list.ToList().ForEach(p => Debug.WriteLine(p));
            // Enumerator를 이용한 리스트 출력
            foreach (var m in list)
            {
                Debug.WriteLine(m);
            }
        }
    }

}
