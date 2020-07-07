using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaParser
{
    public class PlayerList
    {
        public List<Player> Players { get; private set; }
        public Dictionary<string, List<string>> RegisteredAliases { get; set; }
        public static PlayerList UniversalPlayerList;


        public PlayerList()
        {
            Players = new List<Player>();
            UniversalPlayerList = this;
        }

        public Player TryAdd(string name)
        {
            Player player = new Player(name);
            int index = Players.BinarySearch(player);

            if (index < 0)
            {
                index = ~index;
                Players.Insert(index, player);
            }

            return Players[index];
        }
        
        public Player SearchByName(string name)
        {
            int index = Players.BinarySearch(new Player(name));
            return Players[index];
        }

        public string PrintVotes()
        {
            string result = "Vote Call:";
            foreach (Player player in Players)
            {
                result += "\n" + player + "'s current vote is: " + player.CurrentVote;
            }
            return result;
        }

        public override string ToString()
        {
            string result = "Player List:";
            foreach(Player player in Players)
            {
                result += "\n" + player;
            }
            return result;
        }

        public Player DetermineTarget(ref Comment comment)
        {
            try
            {
                List<string> possibleNames = RegisteredAliases[comment.Target];
                if (possibleNames.Count == 1)
                {
                    return SearchByName(possibleNames[0]);
                }
                else {
                    comment.Flagged = true;
                    return null;
                }
            }
            catch (KeyNotFoundException e)
            {
                comment.Flagged = true;
                return null;
            }
        }

        public string GuessNickname(ref Comment comment, out int distance)
        {
            string nickname = comment.Target;
            List<int> distances = new List<int>();
            foreach (Player player in Players)
            {
                string[] NameParts = player.Name.Split(' ');
                string[] NickParts = nickname.Split(' ');
                distances.Add(LevenshteinDistance.Compute(NickParts[0].ToLower(), NameParts[0].ToLower()));
            }
            int minDistance = int.MaxValue;
            int index = 0;

            for (int i = 0; i < distances.Count; i++)
            {
                if (distances[i] < minDistance)
                {
                    index = i;
                    minDistance = distances[i];
                }
                else if (distances[i] == minDistance)
                {
                    string[] NickParts = nickname.Split(' ');
                    if (NickParts.Length > 1)
                    {
                        string[] NamePartsA = Players[i].Name.Split(' ');
                        string[] NamePartsB = Players[index].Name.Split(' ');
                        int distanceA = LevenshteinDistance.Compute(NickParts.Last().ToLower(), NamePartsA.Last().ToLower());
                        int distanceB = LevenshteinDistance.Compute(NickParts.Last().ToLower(), NamePartsB.Last().ToLower());
                        if (distanceA < distanceB)
                        {
                            index = i;
                        }
                    }
                    else if (distances[i] == 0)
                    {
                        Console.WriteLine("Warning: Indeterminate Name Detected: {0}",
                            nickname);
                    }

                }

            }

            distance = distances[index];
            return Players[index].Name;
        }

        public void ClearVotes()
        {
            foreach (Player player in Players)
            {

            }
        }
    }
}
