using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaParser
{
    public class Game
    {
        public string Name {get; private set; }
        public List<Day> Days { get; set; }
        public List<Player> Players { get; set; }

        public Game() { }
        public Game(string Name)
        {
            this.Name = Name;
            Players = new List<Player>();
            Days = new List<Day>();
        }
    }
}
