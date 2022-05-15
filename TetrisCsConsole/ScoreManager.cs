using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TetrisCsConsole
{
    public class ScoreManager
    {
        private readonly string fileName;
        private readonly int[] lineScores;

        public ScoreManager(string fileName)
        {
            this.fileName = fileName;

            this.lineScores = new int[] { 1, 40, 100, 300, 1200 };
            this.HighScore = this.GetHighScore();
        }

        public int Score { get; private set; }

        public int HighScore { get; private set; }

        public void AddScore(int lines, int level)
        {
            this.Score += lineScores[lines] * level;
            if (this.Score > this.HighScore)
            {
                this.HighScore = this.Score;
            }
        }

        private int GetHighScore()
        {
            int highScore = 0;

            if (File.Exists(fileName))
            {
                string[] scores = File.ReadAllLines(fileName);

                foreach (string score in scores)
                {
                    Match scorePattern = Regex.Match(score, @" => (?<score>[0-9]+)");
                    highScore = Math.Max(highScore, int.Parse(scorePattern.Groups["score"].Value));
                }
            }

            return highScore;
        }

        public void PostHighScore()
        {
            File.AppendAllLines(fileName, new List<string>
            {
                $"[{DateTime.Now}] {Environment.UserName} => {this.Score}"
            });
        }
    }
}
