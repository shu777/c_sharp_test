using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using System.Security.Cryptography;// 암호화
//using System.Collections;
//using System.Diagnostics;
using System.Text.RegularExpressions;//정규표현식
using System.Diagnostics;

//1.MyClass_Strings :  문자열 처리하는 클래스
//2.Address_struct : 전화번호부 관리 예

namespace ConsoleApplication_MyLibs
{
    class Address_struct // 구조체 샘플
    {
        public string name { get; set; }
        public List<string> phone { get; set; }
        public List<string> email { get; set; }

    }
    /// <summary>
    /// 문자열 처리 클래스
    /// 
    /// </summary>

    class MyClass_Strings
    {
        public MyClass_Strings()
        {
            // 생성자 없음.. method 들은 static 으로 구현
        }

       
        // 중복되지 않는 첫번째 문자 구해서 return하는 method
        public static char GetFirstChar(char[] S)
        {
            Dictionary<char, int> ht = new Dictionary<char, int>();

            foreach (char ch in S)
            {
                if (!ht.ContainsKey(ch))
                    ht[ch] = 1;
                else
                    ht[ch] += 1;
            }

            foreach (char ch in S)
            {
                if (ht[ch] == 1)
                    return ch;
            }
            return '\0';
        }
        public static void getStringCombination(string s)
        {            
            StringBuilder sb = new StringBuilder();//append 기능 위해 stringbuilder 사용
            StringCombination(s, sb, 0); // 재귀적으로 실행하여 모든 조합을 찾는다.
        }

        private static void StringCombination(string s, StringBuilder sb, int index)
        {
            for (int i = index; i < s.Length; i++)
            {
                // 1) 한 문자 추가
                sb.Append(s[i]);

                // 2) 구한 문자조합 출력
                Debug.WriteLine(sb.ToString());
                // 3) 나머지 문자들에 대한 조합 구하기
                StringCombination(s, sb, i + 1);

                // 위의 1에서 추가한 문자 삭제 
                sb.Remove(sb.Length - 1, 1);
            }
        }
        
        /// <summary>
        /// src string 내의 문자들이 target string 내에 순서와 상관없이 포함되어 있는지 확인 // coffee -> foecofee
        /// </summary>
        /// <param name="src"></param>
        /// <param name="target"></param>
        /// <param name="result"></param>
        /// <returns>0 매치됨, else 타겟에 몇개 문자 더 있음, -1 타겟에 문자 부족</returns>
        public static int checkCharsInString(string src, string target, ref string result)
        {
            int res = 0;
            string src_tmp = src;
            string target_tmp = target;
            foreach (char c in src) // 각 문자를 순서대로 비교
            {
                int index = target_tmp.IndexOf(c); // 일치하는 문자 위치 찾음
                if (index != -1)
                {
                    Debug.WriteLine("match index:{0}", index);
                    target_tmp = target_tmp.Remove(index, 1); // 일치하는 문자 제거
                }
                else
                {
                    // not found char!!!!
                    return -1; // 문자가 포함되지 않는다면 error return
                }
                //if (target_tmp.Contains(c))
                {

                }

            }
            result = target_tmp; // 제거하고 남은 문자열 return
            if (target_tmp.Length != 0) // 남아있는 문자가 있다면 남은 문자 갯수 return
                res = target_tmp.Length;

            return res; // 문자 수가 일치하면 return 0
        }
        /// <summary>
        ///  문자열의 배열 내에서 중복되는 문자열 제거
        /// </summary>
        /// <param name="myList"></param>
        /// <returns></returns>
        public static string[] RemoveDuplicates(string[] myList)
        {
            System.Collections.ArrayList newList = new System.Collections.ArrayList();

            foreach (string str in myList)
                if (!newList.Contains(str))
                    newList.Add(str);
            return (string[])newList.ToArray(typeof(string));
        }

        // 스트링 리스트 오름차순 정렬
        public string[] strOrderUP(string[] words)
        {
            //string[] words = inputData.Split('#');
            // 정렬
            var asc = from s in words
                   orderby s ascending
                   select s;
            string[] result = asc.ToArray();
            return result;
        }
        // 스트링 리스트 내림차순 정렬
        public string[] strOrderDOWN(string[] words)
        {
            //string[] words = inputData.Split('#');
            // 정렬
            var desc = from s in words
                      orderby s descending
                      select s;
            string[] result = desc.ToArray();
            return result;
        }
        // 입력된 string이 전화번호 형식에 맞는지 확인
        public Boolean strPhoneNumberValidation(string inputStr)
        {
            //check phone num or email 
            Regex regex = new Regex(@"010-[0-9]{4}-[0-9]{4}");
            Match m = regex.Match(inputStr);
            if (m.Success)
            {
                // 휴대전화 번호임
                return true;
            }
            else
            {
                return false;
            }
        }
        // 입력된 string이 이메일 형식인지 체크
        public Boolean strEmailValidation(string inputStr)
        {
            // check email
            bool valid = Regex.IsMatch(inputStr, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
            if (valid)
            {
                // email
                return true;
            }
            else
            {
                // invalid
                // valid_res = false;
                return false;
            }
        }
        // 입력된 string이 a~z 정규식 조건에 맞는지 확인
        public Boolean strNameValidation(string inputStr)
        {
            {
                // name only
                Regex regex = new Regex(@"[^a-z@_\.]");
                Match m = regex.Match(inputStr);
                if (m.Success)
                {
                    // invalid name
                    return false; // skip...
                }
                else
                {
                    return true;
                    // valid name

                }
            }

        }

        // 전화번호 구조체 내에 해당 이름을 가진 data가 있는지 체크
        public Boolean checkNameExists(List<Address_struct> inputData, string inputName)
        {
            if (inputData.Exists(x => x.name == inputName)) // already added
            {
                // exists
                return true;
            }
            else
            {
                return false;
            }
        }
        // 전화번호 구조체 내 멤버의 중복데이터 제거
        public void removeDuplication(List<Address_struct> inputData)
        {
            foreach (Address_struct tmp in inputData)
                tmp.phone.Distinct();
        }
        /// <summary>
        ///  지정한 위치에서 부터 지정한 길이까지 스트링을 자름 
        ///  Substring(9); // 오프셋 9자리부터 끝까지
        ///  Substring(9, 5); // 오프셋 9부터 5길이 만큼 자르기
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string strDevideWithLength(string input, int length)
        {
            string result = input.Substring(0, length);
            return result;
        }
        /// <summary>
        ///  string to char array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static char[] strToCharArray(string str)
        {
            return str.ToCharArray();
        }

        // 스트링을 byte array로 바꾼다.
        public static byte[] strToByteArray(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
            // test
            // foreach (var b_data in ret)
            //     Console.Write(b + " ");
        }
        // byte array 를 string으로 바꾼다.
        public static string byteArrayToString(byte[] inputBytes)
        {
            return System.Text.Encoding.UTF8.GetString(inputBytes); 
        }
        /// <summary>
        /// string to int
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int strToInt(string number)
        {
            return Convert.ToInt32(number);
        }
        /// <summary>
        /// char to int convert
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int charToInt(char c)
        {
            //return (int)(c - '0');
            //return Convert.ToInt32(new string(c, 1));
            return Convert.ToInt32(c);
        }
        // 구분자로 string을 나눈다.
        public string[] StringSplit(string src, char delimiter)
        {
            string[] words = src.Split(delimiter);
            return words;
        }
        // 문자열에 해당 문자열이 포함되어 있는지를 체크한다.
        public bool checkContains(string src, string value)
        {
            return src.Contains(value);
        }

        // ex입력된 숫자를 10진수 4자리로 출력
        //    string.Format("{0:D4}", 123);
        // ex입력된 숫자를 16진수 출력
        //     string.Format("{0:X2}", 123); // --> 대문자 hex
        //     string.Format("{0:X2}", {1:x2}, 123, 123); // ->> 대문자 hex, 소문자 hex
        public static string pad_and_int (int N, int P) // integer값을 string으로 변환시 P로 지정한 자릿수 형식에 맞게 변환 앞은 0으로 채운다.
        {
            string s = "{0:";
            for(int i = 0; i<P; i++)
            {
                s += "0";
            }
            s += "}";

            // use of string.Format() method
            string value = string.Format(s, N);

            return value;
        }

        /// <summary>
        /// 스트링 내 해당 문자열 매치 갯수 구한다.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static int countMatches(string src, string pattern)
        {
            int countResult = Regex.Matches(src, pattern).Count;
            return countResult;
        }
        // 로그 내 type 갯수 추출
        public int countingAllTypes(string[] src, char delimiter, int field_number)
        {
            List<string> types = new List<string>();
            // scan types
            foreach (string s in src)
            {
                string[] data = this.StringSplit(s, delimiter);
                if (types.Any(data[1].Contains)) // 겹치는 타입이 있는 경우는 skip
                {
                    // skip
                }
                else
                {
                    types.Add(data[1]); // new type Add
                }
            }
            return types.Count(); // type count 전달
        }
        // 로그 내 type 종류 추출
        public string[] getAllTypes(string[] src, char delimiter, int field_number)
        {
            //string[] types = new string[];
            List<string> types = new List<string>();
            // scan types
            foreach (string s in src)
            {
                string[] data = this.StringSplit(s, delimiter);
                //Class_Strings.
                if (types.Any(data[1].Contains)) // types라는 [스트링 타입을 나열하는] 문자열 리스트상에 파싱한 type이 이미 존재하는지 확인하고,
                {
                    // 중복되는 경우는 skip
                }
                else
                {
                    types.Add(data[1]); // 없으면 [스트링 타입을 나열하는] 문자열 리스트에 추가.
                }
            }
            //types.Count
            return types.ToArray(); // [스트링 타입을 나열하는] 문자열 리스트를 배열로 변환하여 출력
        }
        public string[] getAllTypes_n(string[] src, char delimiter, int field_number)
        {
            //string[] types = new string[];
            List<string> types = new List<string>();
            // scan types
            foreach (string s in src)
            {
                string[] data = this.StringSplit(s, delimiter);
              
                types.Add(data[1]); // 없으면 [스트링 타입을 나열하는] 문자열 리스트에 추가.
               
            }
            return types.Distinct().ToArray(); // 중복 제거 후 [스트링 타입을 나열하는] 문자열 리스트를 배열로 변환하여 출력
        }

        // 로그 내 특정 type의 발생 갯수 counting
        public int getCountOfType(string[] src, char delimiter, int field_number, string type)
        {
            int total = 0;
            List<string> types = new List<string>(); // 문자열 리스트

            foreach (string s in src) // 문자열 배열 소스
            {
                string[] data = this.StringSplit(s, delimiter); // 문자열 파싱 분리
                if (data[field_number].Equals(type)) // 샘플이 1번 필드가 type이어서 비교해서 카운팅 했음. // 범용으로 바꿔야함
                    total++;
            }
            return total;
        }
        // 로그 내 특정 타입의 문자열들 추출
        public string[] getLinesOfType(string[] src, char delimiter, int field_number, string type)
        {
            int total = 0;
            List<string> strings = new List<string>();

            foreach (string s in src)
            {
                string[] data = this.StringSplit(s, delimiter);
                //Class_Strings.
                if (data[field_number].Equals(type))
                    //total++;
                    strings.Add(s);
            }
            return strings.ToArray();
        }

        /// <summary>
        /// 스트링을 입력받아 분리하고, data field의 내용을 외부 컨버터를 사용해 변환 후 해당 스트링에 값을 swap하여 반환
        /// </summary>
        /// <param name="src"></param>
        /// <param name="delimiter"></param>
        /// <param name="field_number"></param>
        /// <returns></returns>
        public string convertStringMsg(string src, char delimiter, int field_number)
        {
            string[] data = this.StringSplit(src, delimiter); // 구분자로 나눠서 문자열 분리
            //MyClass_Program ttest = new MyClass_Program();
            string tttt = MyClass_Program.runExternalProgram("CODECONV.EXE", data[field_number]);
            return src.Replace(data[field_number], tttt);// 문자열 내에서 매칭되는 문자를 target문자로 replace한다.
        }
        ~MyClass_Strings()
        {

        }
        public static string makeNewFilenameWithVersion(string version)
        {
            return string.Format("libtest_{0}.dll", version); // 버전명을 추가한 스트링 형식 리턴
        }
        public static void testClass()
        {
            MyClass_Strings tmp = new MyClass_Strings();

            // 문자열 내 문자로 가능한 조합 구하기
            MyClass_Strings.getStringCombination("ABC");
            // string 내 패턴 스트링 카운팅
            string sampleData = "asdfkk;lkasldfajsdlkfj999kajsdlfkjasdlfkj9999kljflaskdflkajsdlfkjasdlkfj999lkajsdlfkjasldfkja999";
            // 정규식 매칭 확인 + count
            //int countResult = System.Text.RegularExpressions.Regex.Matches(sampleData, "999").Count;
            int countResult = MyClass_Strings.countMatches(sampleData, "999");
            //Match matchRes = Regex.Match(sampleData, "999");

            makeNewFilenameWithVersion("1.1.1");

            string res = pad_and_int(123, 6); // 숫자리 정해진 자릿수 형식으로 문자변환
            Debug.Print(res);
        }
    }
    /// <summary>
    /// 
}
