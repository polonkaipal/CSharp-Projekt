using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class ScoreResult
    {
        private static List<Score> _result;
        public static List<Score> ReadResult(string file)
        {
            using (StreamReader r = new StreamReader(file))
            {
                string json = r.ReadToEnd();
                _result = JsonConvert.DeserializeObject<List<Score>>(json);
            }

            return _result;
        }

        public static void WriteResult(List<Score> result, string file)
        {
            using (StreamWriter w = new StreamWriter(file))
            {
                string json = JsonConvert.SerializeObject(result);
                w.WriteLine(json);
            }
        }
    }
}
