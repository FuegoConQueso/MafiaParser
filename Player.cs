using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaParser
{
    public class Player : IComparable<Player>
    {
        public string Name { get; set; }
        public List<string> Aliases { get; private set; }
        public int Votes { get; set; }
        public string CurrentVote { get; private set; }

        public Player(string name)
        {
            Name = name;
            CurrentVote = "";
        }

        public int CompareTo(Player other)
        {
            return Name.CompareTo(other.Name);
        }

        public void Vote(string target)
        {
            if(CurrentVote.Equals(""))
            {
                CurrentVote = target;
            }
            else
            {
                Console.WriteLine("{0} Voted without nulling.",
                                Name);
                CurrentVote = target;
            }
        }

        public void NullVote(string target)
        {
            if (!CurrentVote.Equals(""))
            {
                CurrentVote = "";
            }
            else
            {
                Console.WriteLine("{0} Nulled without an active vote.",
                                Name);
            }
        }

        public void NullVote()
        {
            if (!CurrentVote.Equals(""))
            {
                CurrentVote = "";
            }
            else
            {
                Console.WriteLine("{0} Nulled without an active vote.",
                                Name);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
