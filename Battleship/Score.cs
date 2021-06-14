using System;

namespace Battleship
{
    public class Score
    {
        public String Player { get; set; }
        public String Enemy { get; set; }
        public String Winner { get; set; }
        public int PlayerHits { get; set; }
        public int EnemyHits { get; set; }
        public int Rounds { get; set; }
    }
}
