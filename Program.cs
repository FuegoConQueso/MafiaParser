using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaParser
{
    class Program
    {

        const string inpath = "Game4Day2.txt";
        const string version = "0.1.0";
        const string copyright = "(c)2020 John Arachtingi";

        static void Main(string[] args)
        {
            Game game = null;
            Console.WriteLine("Welcome to Mafia Parser Version " + version + ".");
            Console.WriteLine(copyright);
            while(true)
            {
                Console.WriteLine("\n-----");
                Console.WriteLine("Choose from the following options (type the option name and press Enter):");
                if (game == null) //if game is uninitialized
                {
                    //Console.WriteLine("'New': create a new game.");
                    //Console.WriteLine("'Load': load an existing game.");
                    Console.WriteLine("'ParseCount': parse a day thread's html, and provides a vote count.");
                    Console.WriteLine("'LoadCount': load a previously parsed file, and provides a vote count.");
                    Console.WriteLine("'Help': this function doesn't actually exist. Go to the group post I made for instructions.");
                    Console.WriteLine("'Exit': exit the program.");
                    switch(Console.ReadLine().ToLower())
                    {
                        case "parsecount":
                            ParseAndVoteCount();
                            break;
                        case "loadcount":
                            LoadAndVoteCount();
                            break;
                        case "new":
                            game = NewGame();
                            break;
                        case "load":
                            game = LoadGame();
                            break;
                        case "exit":
                            return;
                        case "test":
                            test();
                            break;
                        default:
                            Console.WriteLine("Please select a valid option.");
                            break;
                    }
                }
                else //if a game is already initialized
                {
                    Console.WriteLine("'New': create a new game, and open that game.");
                    Console.WriteLine("'Load': load a different existing game.");
                    Console.WriteLine("'Parse': parse a day thread's html, adding or updating it in the game.");
                    Console.WriteLine("'Exit': exit the program.");
                    switch (Console.ReadLine().ToLower())
                    {
                        case "new":
                            game = NewGame();
                            break;
                        case "load":
                            game = LoadGame();
                            break;
                        case "parse":
                            ParseDay(ref game);
                            break;
                        case "exit":
                            return;
                        case "test":
                            test();
                            break;
                        default:
                            Console.WriteLine("Please select a valid option.");
                            break;
                    }
                }

            }
        }

        static void test()
        {

            PlayerList players = new PlayerList();
            Day day = Parser.Parse(inpath, ref players);
            IO.saveDayFile(day, "Out.txt");
            Parser.TallyVotes(ref day, ref players);
            players = new PlayerList();
            day = IO.loadDayFile("Out.txt");
            Parser.TallyVotes(ref day, ref players);

            //day.CommentBrowser();

            Console.ReadLine();
        }

        static Game NewGame()
        {
            Console.WriteLine("Enter name for new game.");
            Game output = new Game(Console.ReadLine());
            Console.WriteLine("Enter the full name of each player, exactly how they appear on their facebook profile.");
            Console.WriteLine("When you are finished, type \"Done\"");
            string name = Console.ReadLine();
            while (!name.Equals("Done", StringComparison.OrdinalIgnoreCase))
            {
                output.Players.Add(new Player(name));
                name = Console.ReadLine();
            }

            return output;
        }

        static Game LoadGame()
        {
            Console.WriteLine("Enter game file name to load (including .txt extension).");
            try
            {
            return IO.loadGameFile(Console.ReadLine());

            }
            catch (Exception e)
            {
                Console.WriteLine("Sorry, there was an error loading that file. Please try again.");
                return null;
            }
        }

        static void ParseAndVoteCount()
        {
            Day day;
            PlayerList players;
            Console.WriteLine("Enter unparsed day file name to parse (including .txt extension).");
            try
            {
                players = new PlayerList();
                day = Parser.Parse(Console.ReadLine(), ref players);

            }
            catch (Exception e)
            {
                Console.WriteLine("Sorry, there was an error loading that file. Please try again.");
                return;
            }

            Console.WriteLine("Enter a unique name for saving this file. (Recommended: {GameName}Day# )");
            IO.saveDayFile(day, Console.ReadLine() + ".txt");
            Console.WriteLine("File Saved.");
            Console.WriteLine();
            Parser.TallyVotes(ref day, ref players);
        }

        static void LoadAndVoteCount()
        {
            Day day;
            PlayerList players;
            Console.WriteLine("Enter parsed day file name to load (including .txt extension).");
            try
            {
                players = new PlayerList();
                day = IO.loadDayFile(Console.ReadLine());

            }
            catch (Exception e)
            {
                Console.WriteLine("Sorry, there was an error loading that file. Please try again.");
                return;
            }
            Console.WriteLine("File Loaded.");
            Console.WriteLine();
            Parser.TallyVotes(ref day, ref players);
        }

        static void ParseDay(ref Game game)
        {

        }

        static void PrintPlayerComments(ref Day day, string name)
        {
            foreach(Comment comment in day.GetComments(name))
                {
                    comment.Print();
                }
        }

        static void PrintLastComment(ref Day day)
        {
            Console.WriteLine("\nLast comment read:\n{0} commented: {1}",
                              day.Comments.Last().Name,
                              day.Comments.Last().Message
                              );
        }

    }
}
