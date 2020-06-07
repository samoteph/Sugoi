using GameJolt;
using GameJolt.Objects;
using Sugoi.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrazyZone
{
    public class Leaderboard
    {
        GameJoltApi gameJolt;
        Machine machine;

        List<LeaderboardItem> items = new List<LeaderboardItem>(100);

        /// <summary>
        /// Items chargés
        /// </summary>

        public List<LeaderboardItem> Items
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="machine"></param>

        public Leaderboard(Machine machine)
        {
            this.machine = machine;
            this.gameJolt = new GameJoltApi(501513, "64c8855b6c591e8fadd8dc4589fb531e");
        }

        /// <summary>
        /// Chargement 
        /// </summary>

        public async Task<bool> LoadScoresAsync()
        {
            Response<Score[]> response = null;

            try
            {
                response = await gameJolt.Scores.FetchAsync(limit: 100);
            }
            catch(Exception ex)
            {
                return false;
            }

            if (response.Success)
            {
                List<LeaderboardItem> items = new List<LeaderboardItem>(100);

                foreach (var data in response.Data)
                {
                    var item = new LeaderboardItem();

                    item.Name = data.GuestName;
                    item.Score = data.Value;

                    items.Add(item);
                }

                this.items = items;

                this.SaveLocalScores();
            }

            return true;
        }

        /// <summary>
        /// Sauvegarde un score
        /// </summary>
        /// <param name="score"></param>
        /// <param name="text"></param>

        public void SaveScore(string name, int score, Action<Response> completed = null)
        {
            gameJolt.Scores.Add(name, score, score.ToString(), callback: completed);
        }

        public async Task<bool> SaveScoreAsync(string name, int score)
        {
            try
            {
                var scoreString = score.ToString();
                await gameJolt.Scores.AddAsync(name, score, scoreString, Encode(name, scoreString));
                return true;
            }
            catch
            {
                return false; 
            }
        }

        private string Encode(string name, string scoreString)
        {
            string base64 = EncodeTo64(name + "/" + scoreString);

            return base64[1] + base64[0] + base64.Substring(2);                        
        }

        private string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes

                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);

            string returnValue

                  = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;
        }

        /// <summary>
        /// Sauvegarde
        /// </summary>

        private void SaveLocalScores()
        {
            //this.machine.
        }
    }

    /// <summary>
    /// Item
    /// </summary>

    public class LeaderboardItem
    {
        public string Name
        {
            get;
            set;
        }

        public int Score
        {
            get;
            set;
        }
    }
}
