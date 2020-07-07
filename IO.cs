using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace MafiaParser
{
    class IO
    {
        public static Game loadGameFile(string filename)
        {
            string path = buildPath(filename);
            if (File.Exists(path))
            {
                string[] filetext = File.ReadAllLines(path);
                switch (filetext[0])
                {
                    case "MafiaParser Game File v1":
                        return loadGameFilev1(filetext);
                    default:
                        throw new Exception("Improperly formatted Game file: " + filename);
                        break;
                }
            }
            else
            {
                Console.WriteLine("File not Found");
                throw new Exception("Unable to load Game file: " + filename);
            }
        }

        private static Game loadGameFilev1(string[] filetext)
        {
            //load Name
            Game output = new Game(filetext[1]);

            //load Player List
            int line = 3; // skip <Players> line
            while(filetext[line] != "</Players>")
            {
                output.Players.Add(new Player(filetext[line]));
                line++;
            }

            //load Days
            line += 2; // skip </Players> and <Days> lines
            while(filetext[line] != "</Days>")
            {
                output.Days.Add(loadDayFile(filetext[line] + ".txt"));
                line++;
            }

            return output;
        }

        public static void saveGameFile(Game game)
        {
            if (File.Exists(game.Name + ".txt"))
            {
                Console.WriteLine("Game File Exists. Overwrite? (y to continue)");
                if ( !Console.ReadLine().Equals("y") ) {
                    return;
                }
            }

            using (StreamWriter outStream = File.CreateText(game.Name + ".txt"))
            {
                outStream.WriteLine("MafiaParser Game File v1");
                outStream.WriteLine(game.Name);

                outStream.WriteLine("<Players>");
                for(int i = 0; i < game.Players.Count; i++)
                {
                    outStream.WriteLine(game.Players[i]);
                }
                outStream.WriteLine("</Players>");


                outStream.WriteLine("<Days>");
                outStream.WriteLine("</Days>");

            }
        }

        public static Day loadDayFile(string filename)
        {
            string path = buildPath(filename);
            if (File.Exists(path))
            {
                string[] filetext = File.ReadAllLines(path);
                switch (filetext[0])
                {
                    case "MafiaParser Day File v1":
                        return loadDayFilev1(filetext);
                    default:
                        throw new Exception("Improperly formatted Day file: " + filename);
                        break;
                }
            }
            else
            {
                Console.WriteLine("File not Found");
                throw new Exception("Unable to load Game file: " + filename);
            }
        }

        private static Day loadDayFilev1(string[] filetext)
        {
            int dayNumber = int.Parse(filetext[1].Split(' ')[1]);

            List<Comment> comments = new List<Comment>();
            int line = 3;
            while (line < filetext.Length)
            {
                //parse 1st line of comment
                string[] splitters = { " [", "]: " };
                string[] lineA = filetext[line].Split(splitters, StringSplitOptions.RemoveEmptyEntries);
                string name = lineA[0];
                string utime = lineA[1];
                string message = lineA[2];

                comments.Add(new Comment(name, message, utime));
                PlayerList.UniversalPlayerList.TryAdd(name);

                line++;
            }

            return new Day(dayNumber, comments);
        }

        public static void saveDayFile(Day day, string filepath)
        {
            using (StreamWriter outStream = File.CreateText(filepath))
            {
                outStream.WriteLine("MafiaParser Day File v1");
                outStream.WriteLine("Day " + day.DayNumber.ToString());
                outStream.WriteLine("<Comments>");
                for(int i = 0; i < day.Comments.Count; i++)
                {
                    outStream.WriteLine(day.Comments[i].ToString());
                }
            }
        }

        public static string buildPath(string filename)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filename);
        }
    }
}
