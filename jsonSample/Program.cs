using System;
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
            // json format string을 jobject로 변경
            var jobjectSample = JObject.Parse(jsonStructureSampleString);

            // jobject를 c# class structure로 변경
            TEST_Class_Root myDeserializedClass = JsonConvert.DeserializeObject<TEST_Class_Root>(jobjectSample.ToString());
            //TEST_Class_Root myDeserializedClass2 = JsonSerializer.Deserialize<TEST_Class_Root>(jobjectSample.ToString()); //using System.Text.Json;
            TEST_Class_Root myDeserializedClass2 = JsonConvert.DeserializeObject<TEST_Class_Root>(jsonStructureSampleString);
            var cteated_at = myDeserializedClass.Class1.created_at;//.Class1

            // c# class structure를 json format string으로 변경
            string convertedJson = JsonConvert.SerializeObject(myDeserializedClass2);
            // json string을 jobject로 변경
            var jobjectSample2 = JObject.Parse(convertedJson);
        }
    }

    public class Awesomeobject
    {
        public int SomeProps1 { get; set; }
        public string SomeProps2 { get; set; }
    }

    public class Class1
    {
        public int id { get; set; }
        public string user_id { get; set; }
        public Awesomeobject awesomeobject { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public List<User> users { get; set; }
    }

    public class Class2
    {
        public string SomePropertyOfClass2 { get; set; }
    }

    public class TEST_Class_Root //Root
    {
        public Class1 Class1 { get; set; }
        public Class2 Class2 { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public string name { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string email { get; set; }
        public string testanadditionalfield { get; set; }
    }


}
