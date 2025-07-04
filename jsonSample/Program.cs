﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;


namespace jsonSample
{
    internal class Program
    {
        public const String inputJSONPATH = ".\\json.txt";
        
        static void Main(string[] args)
        {
            var dictSample = new Dictionary<string, string>();
            dictSample.Add("id", "root");
            dictSample.Add("passwd", "1234");
            dictSample.Add("token", "asdfg1234567");

            var dictSampleMain = new Dictionary<string, object>();
            dictSampleMain.Add("sample1", dictSample);

            var json_from_dict = JObject.FromObject(dictSample);
            //Console.WriteLine(json_from_dict.ToString());
            Console.WriteLine(dictSampleMain.ToString());

            // jobject 
            var json = new JObject(); /// dictionary 처럼 사용하는 jobject
            json.Add("id", "Luna");
            json.Add("name", "Silver");
            json.Add("age", 19);
            Console.WriteLine(json.ToString());

            var json2 = JObject.Parse("{ id : \"Luna\" , name : \"Silver\" , age : 19 }");
            json2.Add("blog", "devluna.blogspot.kr");
            Console.WriteLine(json2.ToString());

            var json4 = JObject.FromObject(new { id = "J01", name = "June", age = 23 });
            Console.WriteLine(json4.ToString());

            var json5 = JObject.Parse("{ id : \"sjy\" , name : \"seok-joon\" , age : 27 }");
            json5.Add("friend1", json);
            json5.Add("friend2", json2);
            json5.Add("friend4", json4);
            Console.WriteLine(json5.ToString());

            // jarray
            var jarray = new JArray();   // 간단한 string array 샘플
            jarray.Add(1);
            jarray.Add("Luna");
            jarray.Add(DateTime.Now);

            Console.WriteLine(jarray.ToString());

            var jFriends = new JArray(); // jobject의 jarray 샘플
            jFriends.Add(json);
            jFriends.Add(json2);
            // jFriends.Add(json3);
            jFriends.Add(json4);
            Console.WriteLine(jFriends.ToString());

            json2.Add("Friends", jFriends);
            Console.WriteLine(json2.ToString());

            ////////////////////////////읽어오기 ////////////////
            string test = json2.GetValue("blog").ToString();  // jobject내 value 가져오기 샘플

            foreach (JObject item in jFriends) // jarray 값 foreach로 가져오기 샘픔
            {
                string id = item.GetValue("id").ToString();
                string name = item.GetValue("name").ToString();
                // ...
            }

            ///////////////// jobject 정렬 //////////////
            string voteJson = File.ReadAllText("vote.json"); // file 에서 json 읽기
            JObject voteObj = JObject.Parse(voteJson); // json Object로 파싱
            var sortedObj = new JObject(
                voteObj.Properties().OrderByDescending(p => (int)p.Value) //값으로 내림차순 정렬해서 새로 저장
            );
            string output = sortedObj.ToString();
            JProperty firstProp = sortedObj.Properties().First(); // 제일 큰 값을 get
            Console.WriteLine("Winner: " + firstProp.Name + " (" + firstProp.Value + " votes)");


            //https://json2csharp.com/  json -> c# class online converter TOOL
            string jsonStructureSampleString = @"{
    ""Class1"":{
        ""id"":4,
        ""user_id"":""user_id_value"",
        ""awesomeobject"":{
        ""SomeProps1"":1,
        ""SomeProps2"":""test""
    },
    ""created_at"":""2015-06-02 23:33:90"",
    ""updated_at"":""2015-06-02 23:33:90"",
    ""users"":[
        {
        ""id"":""6"",
        ""name"":""Test Child 1"",
        ""created_at"":""2015-06-02 23:33:90"",
        ""updated_at"":""2015-06-02 23:33:90"",
        ""email"":""test@gmail.com""
        },
        {
        ""id"":""6"",
        ""name"":""Test Child 1"",
        ""created_at"":""2015-06-02 23:33:90"",
        ""updated_at"":""2015-06-02 23:33:90"",
        ""email"":""test@gmail.com"",
        ""testanadditionalfield"":""tet""
        } ]
    },
    ""Class2"":{
    ""SomePropertyOfClass2"":""SomeValueOfClass2""
    }
}";
            // 1. json format string을 jobject로 변경
            var jobjectSample = JObject.Parse(jsonStructureSampleString);
            // 2. jobject를 c# class structure로 변경
            TEST_Class_Root myDeserializedClass = JsonConvert.DeserializeObject<TEST_Class_Root>(jobjectSample.ToString());
            //TEST_Class_Root myDeserializedClass2 = JsonSerializer.Deserialize<TEST_Class_Root>(jobjectSample.ToString()); //using System.Text.Json;
            TEST_Class_Root myDeserializedClass2 = JsonConvert.DeserializeObject<TEST_Class_Root>(jsonStructureSampleString);
            var cteated_at = myDeserializedClass.Class1.created_at;//.Class1



            // c# class structure를 json format string으로 변경
            string convertedJson = JsonConvert.SerializeObject(myDeserializedClass2);
            // json string을 jobject로 변경
            var jobjectSample2 = JObject.Parse(convertedJson);



            //////SAMPLE2 start
            /*
            {
                "stringVal":"testTitle",
                "arrayVal": ["srray1", "array2", "array3"           ],
                "intVal":1000
            }
            */
            //https://jsontostring.com/

            // JSON -> CLASS 정형화된 구조의 JSON을 class structure로 변환
            string inputJsonDataSample2 = "{\"stringVal\":\"testTitle\",\"arrayVal\":[\"srray1\",\"array2\",\"array3\"],\"intVal\":1000}";
            Root_SAMPLE2 myDeserializedClassSample2 = JsonConvert.DeserializeObject<Root_SAMPLE2>(inputJsonDataSample2);

            // 비정형화된 형식의 JSON을 c# Dietionary로 변환, 서로다른 크기의 배열을 갖는 경우
            string jsonInput = "{ \"key1\": [\"value1\", \"value2\", \"value3\"], \"key2\": [\"value1\",], \"someOtherKey\": [\"value2\", \"value3\", \"value4\"] }";
            Dictionary<string, List<string>> data = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonInput);
            foreach (var kvp in data)
            {
                Console.WriteLine($"Key: {kvp.Key}, Values: {string.Join(", ", kvp.Value)}");
                // 해당 value에 포함여부 체크 루틴
                if (kvp.Value.Contains("value2"))
                {
                    Console.WriteLine($"match Key: {kvp.Key}, value2");
                }
            }

            int CalculateSquare(int x)
            {
                int Square(int n) => n * n;  // 인라인 로컬 함수
                return Square(x);
            }

            // CLASS -> JSON
            Root_SAMPLE2 resultSample2 = new Root_SAMPLE2();
            resultSample2.stringVal = "TEST";
            List<string> resArray = new List<string>();
            resArray.Add("Array1");
            resArray.Add("Array2");
            resArray.Add("Array2");
            resultSample2.arrayVal = resArray;
            resultSample2.intVal = 9999;

            //  c# class structure를 json format string으로 변경
            string convertedJsonSample2 = JsonConvert.SerializeObject(resultSample2);
            var jobjectSample22 = JObject.Parse(convertedJsonSample2); // json obj
            int testttt = 0;

            //  read from file
            //  inputJSONPATH
            string text = System.IO.File.ReadAllText(inputJSONPATH);
            Console.WriteLine("readAllText : {0} ", text);
            Root_SAMPLE2 myDeserializedClassSample3 = JsonConvert.DeserializeObject<Root_SAMPLE2>(text);
            Console.WriteLine("readAllText : {0} ", myDeserializedClassSample3.intVal);
        }
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root_SAMPLE2>(myJsonResponse);
    //https://json2csharp.com/
    public class Root_SAMPLE2
    {
        public string stringVal { get; set; }
        public List<string> arrayVal { get; set; }
        public int intVal { get; set; }
    }


    /// <summary>
    /// SAMPLE 1
    /// </summary>
    public class Awesomeobject // depth3 class 구조체
    {
        public int SomeProps1 { get; set; }
        public string SomeProps2 { get; set; }
    }

    public class User // depth3 class 구조체
    {
        public string id { get; set; }
        public string name { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string email { get; set; }
        public string testanadditionalfield { get; set; }
    }
    public class Class1 // depth 1 class // 서브구조체
    {
        public int id { get; set; } // int 타입
        public string user_id { get; set; } // string 타입
        public Awesomeobject awesomeobject { get; set; } // depth3 class 서브 구조체
        public string created_at { get; set; } // string 타입
        public string updated_at { get; set; } // string 타입
        public List<User> users { get; set; } // depth3 array  서브 구조체
    }

    public class Class2 // depth 1 class  서브구조체
    {
        public string SomePropertyOfClass2 { get; set; } // depth2 class  서브의 서브 구조체
    }

    public class TEST_Class_Root //Root Main path  메인 구조체
    {
        public Class1 Class1 { get; set; } // depth1 class 서브 구조체
        public Class2 Class2 { get; set; } // depth1 class 서브 구조체
    }



}
