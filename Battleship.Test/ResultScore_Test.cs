using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace Battleship.Test
{
    [TestClass]
    public class ResultScore_Test
    {
        readonly private string _filename = "unit_test.json";
        readonly private string _filename2 = "unit_test2.json";

        [TestMethod]
        public void ReadResult_ScoreFileDoesntExist_ReturnNull()
        {
            // Arrange + Act
            List<Score> scores = ScoreResult.ReadResult(_filename);

            // Assert
            Assert.IsNull(scores);
        }

        [TestMethod]
        public void WriteResult_NewScorePutsScoreFile_ScoreFileContainsNewScore()
        {
            // Arrange
            Score s = new() { 
                Enemy = "AI",
                EnemyHits = 3,
                Player = "My",
                PlayerHits = 15,
                Rounds = 5,
                Winner = "My"
            };

            List<Score> scores = new List<Score>();
            scores.Add(s);

            // Act
            ScoreResult.WriteResult(scores, _filename2);

            // Assert
            List<Score> tmp = ScoreResult.ReadResult(_filename2);
            if (!tmp.Contains(s))
            {
                Assert.Fail();
            }
        }
    }
}
