using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Battleship
{
    public class ScoreResult
    {
        private static List<Score> _result;
        public static List<Score> ReadResult(string file)
        {
            if (File.Exists(file))
            {
                using (StreamReader r = new(file))
                {
                    string json = r.ReadToEnd();
                    _result = JsonConvert.DeserializeObject<List<Score>>(json);
                }
                return _result;
            }
            return null;
        }

        public static void WriteResult(List<Score> result, string file)
        {
            using StreamWriter w = new(file);
            string json = JsonConvert.SerializeObject(result);
            w.WriteLine(json);
        }
    }
}
