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

        public override bool Equals(object obj)
        {
            return obj is Score score &&
                   Player == score.Player &&
                   Enemy == score.Enemy &&
                   Winner == score.Winner &&
                   PlayerHits == score.PlayerHits &&
                   EnemyHits == score.EnemyHits &&
                   Rounds == score.Rounds;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, Enemy, Winner, PlayerHits, EnemyHits, Rounds);
        }
    }
}
