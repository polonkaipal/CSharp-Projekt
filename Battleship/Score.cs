using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    class Score
    {
        public String Player { get; set; }
        public String Enemy { get; set; }
        public String Winner { get; set; }
        public int PlayerHits { get; set; }
        public int EnemyHits { get; set; }
        public int Rounds { get; set; }
    }
}
