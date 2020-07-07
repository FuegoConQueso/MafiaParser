using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaParser
{
    public class Day
    {
        public List<Comment> Comments { get; private set; }
        public int DayNumber { get; set; }

        public Day() { }

        public Day(List<Comment> comments)
        {
            Comments = comments;
            Comments.Sort();
        }

        public Day(int dayNumber, List<Comment> comments)
        {
            this.DayNumber = dayNumber;
            this.Comments = comments;
        }

        /**public List<Tuple<Player, int>> VoteCount(PlayerList playerList)
        {
            playerList.ClearVotes();
            for(int i = 0; i < Comments.Count; i++)
            {

            }
        }*/

        public List<Comment> GetComments(string playerName)
        {
            List<Comment> output = new List<Comment>();
            foreach(Comment comment in Comments)
            {
                if (comment.Name.Equals(playerName))
                {
                    output.Add(comment);
                }
            }
            return output;
        }

        public void CommentBrowser(int startIndex = 0)
        {
            
            bool flag = true;
            int centeredIndex = startIndex;
            Console.Clear();
            while (flag)
            {
                Console.Clear();
                Console.WriteLine("--Comment Browser--");
                int topIndex = centeredIndex - 2;
                if (topIndex < 0)
                {
                    topIndex = 0;
                }
                int bottomIndex = topIndex + 5;
                if (bottomIndex >= Comments.Count)
                {
                    bottomIndex = Comments.Count - 1;
                }

                for (int i = topIndex; i < bottomIndex; i++)
                {
                    string border = "";
                    if (i == centeredIndex)
                    {
                        border = "--------";
                    }

                    Console.WriteLine(border);
                    Console.Write("{0}/{1}: ", i + 1, Comments.Count);
                    Comments[i].Print();
                    Console.Write(border);
                }
                Console.WriteLine("\nPress ^ or v arrow keys to scroll comments; press Enter to exit comment browser.");
                ConsoleKeyInfo key = Console.ReadKey(false);
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        flag = false;
                        break;

                    case ConsoleKey.UpArrow:
                        centeredIndex--;
                        if (centeredIndex < 0)
                        {
                            centeredIndex = 0;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        centeredIndex++;
                        if (centeredIndex >= Comments.Count)
                        {
                            centeredIndex = Comments.Count - 1;
                        }
                        break;
                }
            }
        }
    }
}
