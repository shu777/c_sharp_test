using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ##### Queue
// ##### List
// ##### Array
// ##### Map(Dictionary)
/// <summary>
/// 
/// </summary>
namespace ConsoleApplication_MyLibs
{
    class MyClass_Dictionary
    {
        public static void dictionarySample()
        {
            // key value
            Dictionary<int, string> d = new Dictionary<int, string>();
            d.Add(1, "value");
            foreach(KeyValuePair<int, string> items in d)
            {
                Console.WriteLine("key: {0}", items.Key);
                Console.WriteLine("value {0}", items.Value);
            }
        }
    }
    class Myclass_Queue // 큐에 대한 샘플
    {
        public static void queueSample()
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue("first"); //추가
            queue.Enqueue("second"); //추가
            queue.Enqueue("third"); //추가
            Console.WriteLine("Q count: {0}", queue.Count); // 크기 확인
            foreach (string item in queue)
            {
                Console.WriteLine(item); // 아이템 출력
            }
            Console.WriteLine("Deque : '{0}'", queue.Dequeue()); //삭제
            Console.WriteLine("Peek : {0}", queue.Peek()); //조회
            Console.WriteLine("check Contains : {0}", queue.Contains("third")); // 확인
            queue.Clear();
        }

    }

    class Request
    {
        public string RequestId { get; set; }
        public string Type { get; set; }  // "P" or "A"
        public string Data { get; set; }
    }

    class MyClass_List_Array // 리스트와 배열간 변환 샘플
    {
        public List<int> CreatList () // 리스트 생성
        {
            List<int> sub_table2 = new List<int>();// 샘플 타입은 int.
            return sub_table2;
        }
        public void ClearList(List<int>input) // 리스트 초기화
        {
            input.Clear();
        }
        public void AddToList(List<int> input, int data) // 리스트에 추가
        {
            input.Add(data);
        }
        public void RemoveFromLIst(List<int> input, int data) // 리스트에서 삭제
        {
            input.Remove(data);
        }
        public void RemoveFromLIstWithIdx(List<int> input, int idx) // 리스트에서 인덱스로 삭제
        {
            input.Remove(input[idx]);
        }
        public void sortList(List <int>inputListData) // 리스트 소팅 정렬
        {
            // 기본 오름차순 정렬
            inputListData.Sort();

        }
        public void sortList2(List<int> inputListData) // 리스트 소팅 내림차순 정렬
        {
            // 내림차순 정렬
            inputListData.Sort(delegate (int x, int y)
            {
                return y.CompareTo(x); // 내림차순
            });

        }
        public void sortList3(List<int> inputListData) // 리스트 소팅 람다식 오름차순
        {
            // 오름차순 정렬
            inputListData.Sort((int x, int y) => x.CompareTo(y));
 
        }

        // 리스트를 그룹핑한느 샘플
        public static void listGroupingSample()
        {
            var input = new List<Request> // sample data
            {
                new Request { RequestId = "001", Type = "K2", Data = "0" },
                new Request { RequestId = "002", Type = "K2", Data = "1" },
                new Request { RequestId = "001", Type = "G1", Data = "0" },
                new Request { RequestId = "002", Type = "G1", Data = "0" },
                new Request { RequestId = "003", Type = "G1", Data = "1" }
            };

            // 특정 값으로 그룹핑해 dictionary로 변환
            var grouped = input
                .GroupBy(r => r.RequestId)
                .ToDictionary(g => g.Key, g => g.ToList());

            int totalRequests = 0;
            int matchingDataCount = 0;

            foreach (var kvp in grouped) // 그룹내에서 calc
            {
                var pair = kvp.Value;
                // 현재 requestid 그룹(pair)에서 Type이 "K2"인 요청을 찾음
                var PR = pair.FirstOrDefault(r => r.Type == "K2"); // 첫번째 매칭되는 값 찾음, 없으면 null
                // 현재 requestid 그룹(pair)에서 Type이 "A1"인 요청을 찾음
                var AN = pair.FirstOrDefault(r => r.Type == "G1"); // 첫번째 매칭되는 값 찾음, 없으면 null

                if (PR == null) continue;  // "K2" 타입이 없으면 유효한 요청x
                totalRequests++;

                if (AN != null && PR.Data == AN.Data)
                    matchingDataCount++;
            }
            Console.WriteLine($"request 수: {totalRequests}");
            Console.WriteLine($"data match: {matchingDataCount}");
        }
        public int GetMaxFromList(List<List<int>> temp)// 내림차순 정렬 후 첫번째 리스트 얻는다
        {               
            int result = temp.OrderByDescending(x => x.Count())
                        .FirstOrDefault<List<int>>()[0];
            return result;
        }
        public List<List<int>> ParseAndConvertExample(string inputData) // 리스트를 사용해 문자열 파싱하고 변환하는 샘플
        {
            {
                string[] arrInput = inputData.Split('#'); // 입력 데이터 파싱

                List<int> listInput = new List<int>(); // int 타입 리스트

                for (int i = 0; i < arrInput.Length; i++)
                {
                    listInput.Add(int.Parse(arrInput[i])); // 문자열을 int 타입 리스트로 변환
                }

                //오름차순 정렬
                listInput.Sort(); // 정렬

                //자리수별로 배치
                List<List<int>> temp = new List<List<int>>();

                //10의 자리수
                int dec = 0;
                List<int> temp2 = null;
                for (int i = 0; i < listInput.Count; i++)
                {
                    if (i == 0)
                    {
                        temp2 = new List<int>();

                        dec = listInput[i] / 10;

                        //첫번째 입력이면 십의 자리 넣고 일의 자리로 넣고
                        //두번째 부터는 1의 자리
                        if (temp2.Count == 0)
                        {
                            temp2.Add(dec);
                            temp2.Add(listInput[i] % 10);
                        }
                        else
                        {
                            temp2.Add(listInput[i] % 10);
                        }
                    }
                    else
                    {
                        //같은 자리수 인지 비교
                        if (dec == listInput[i] / 10)
                        {
                            temp2.Add(listInput[i] % 10);
                        }
                        else
                        {
                            //다를때 => 자리수가 변경
                            //지금까지 작업한 temp2 를 temp 에 추가
                            temp.Add(temp2);

                            //새로 temp2 시작
                            temp2 = new List<int>();
                            dec = listInput[i] / 10;
                            temp2.Add(dec);
                            temp2.Add(listInput[i] % 10);
                        }
                    }
                }

                temp.Add(temp2);

                return temp;
            }
        }


        public void test_class() // array를 list로 변환하여 처리.
        {

            // Array to List
            int[] ints = new[] { 10, 20, 10, 34, 113 };
            List<int> lst = ints.OfType<int>().ToList();
            lst.AddRange(new int[] { 10, 20, 10, 34, 113 }); // 리스트에 아이템들을 추가
            // List to Array
            int[] intsArray = lst.ToArray();

        }

    }
}
