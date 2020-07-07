using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MafiaParser
{
    class Parser
    {

        public static Day Parse(string inPath, string outPath, ref PlayerList players)
        {
            List<Comment> comments;
            

            inPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), inPath);
            outPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), outPath);
            if (File.Exists(inPath))
            {

                Console.Write("Loading input file... ");
                string rawtext = File.ReadAllText(inPath);
                Console.WriteLine("Loaded.");

                comments = Process(in rawtext, ref players);
                

                //using (StreamWriter outStream = File.CreateText(outPath))
                {

                }
                return new Day(comments);
            }
            else
            {
                Console.WriteLine("File not Found");
                throw new Exception("Unable to load file");
            }
        }

        public static bool TallyVotes(ref Day dayThread, ref PlayerList players, bool verbose = false)
        {
            bool flaggedNames = false;
            for (int i = 0; i < dayThread.Comments.Count; i++)
            {
                Comment comment = dayThread.Comments[i];

                if (comment.Type == Comment.CommentType.Vote ||
                    comment.Type == Comment.CommentType.NullVote)
                {
                    string guessedName = "";
                    int distance = -1;
                    
                    if (comment.Type == Comment.CommentType.NullVote)
                    {
                        players.SearchByName(comment.Name).NullVote(comment.Target);
                    }
                    if (comment.Target != "")
                    {
                        if (comment.Type == Comment.CommentType.Vote)
                        {
                            players.SearchByName(comment.Name).Vote(comment.Target);
                        }
                        guessedName = players.GuessNickname(ref comment, out distance);
                    }
                    if (verbose) { 
                    Console.WriteLine("{0,21} {1,-9} \"{2}\"\n[{3}] (Guess: {4} - [{5}])\n",
                        comment.Name,
                        comment.Type,
                        comment.Target,
                        comment.Time,
                        guessedName,
                        distance
                        );
                    }
                }
            }
            if (flaggedNames)
            {
                Console.WriteLine("Votes unclear: please resolve flagged comments to get final tally.");
            }
            else
            {
                Console.WriteLine(players.PrintVotes());
            }
            return flaggedNames;
        }

        public static void ResolveFlaggedComments(ref Day dayThread, ref PlayerList players)
        {
            for (int i = 0; i < dayThread.Comments.Count; i++)
            {
                if (dayThread.Comments[i].Flagged)
                {
                    Comment comment = dayThread.Comments[i]; 
                    /**try
                    {
                        foreach(string player in players.RegisteredAliases[comment.Target])
                        {

                        }
                    }**/
                }
                dayThread.CommentBrowser();
            }
        }

        static List<Comment> Process(in string rawtext, ref PlayerList players)
        {
            List < Comment > comments = new List<Comment>();

            string nameBeg = Regex.Escape("a class=\"_6qw4\"");
            string nameEnd = Regex.Escape("</a>");
            string nameReg = nameBeg + ".*?>(?<name>.*?)(?=" + nameEnd + ")";

            string commentBeg = Regex.Escape("span class=\"_3l3x\">");
            string commentEnd = Regex.Escape("</div>");
            string commentReg = commentBeg + "(?<message>.*?)(?=" + commentEnd + ")";

            string timeReg = Regex.Escape("data-utime=\"") + "(?<time>.*?)\"";


            string pattern = nameReg + ".*?" + commentReg + ".*?" + timeReg;

            Regex rx = new Regex(pattern,
                                 RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            MatchCollection matches = rx.Matches(rawtext);
            foreach (Match match in matches)
            {
                GroupCollection groups = CleanMatch(match.Groups, rx);
                
                string name = groups["name"].Value;
                string message = ProcessMessage(groups["message"].Value);
                string target = "";
                string time = groups["time"].Value;
                Comment.CommentType type = EvaluateType(message, out target);
                players.TryAdd(name);
                comments.Add(new Comment(name, message, time, type, target));
            }
            Console.WriteLine("There were {0} comments.",
                              comments.Count);
            return comments;
        }

        static GroupCollection CleanMatch(GroupCollection match, Regex rx)
        {
            string badMarker = @"<\s*?div\s*?class=" + "\"_2h2j\"";
            Regex rxBad = new Regex(badMarker, RegexOptions.IgnoreCase);

            Match badMatch = rxBad.Match(match[0].Value);
            if (badMatch.Success)
            {
                string[] newMatches = rxBad.Split(match[0].Value);
                return rx.Match(newMatches.Last()).Groups;
            }
            else
            {
                return match;
            }
        }

        static string ProcessMessage(string rawMessage)
        {
            Regex rx = new Regex("<.*?>");
            return rx.Replace(rawMessage, "");
        }
        
        public static Comment.CommentType EvaluateType(string message, out string target)
        {
            //null vote: name
            string pattern = @"\s*?null\s+?vote\s*?\:\s*(?<name>.*?)\s*";
            if (TypeCheck(pattern, message, out target))
            {
                return Comment.CommentType.NullVote;
            }

            //vote null: name
            pattern = @"\s*?vote\s+?null\s*?\:\s*(?<name>.*?)\s*";
            if (TypeCheck(pattern, message, out target))
            {
                return Comment.CommentType.NullVote;
            }

            //null: name
            pattern = @"\s*?null\s*?\:\s*(?<name>.*)\s*";
            if (TypeCheck(pattern, message, out target))
            {
                return Comment.CommentType.NullVote;
            }

            //vote: name
            pattern = @"\s*?vote\s*?\:\s*(?<name>.*)\s*";
            if (TypeCheck(pattern, message, out target))
            {
                return Comment.CommentType.Vote;
            }

            target = "";
            return Comment.CommentType.Discussion;
        }

        static bool TypeCheck(string regex, string message, out string target)
        {
            Regex rx = new Regex(regex, RegexOptions.IgnoreCase);
            Match match = rx.Match(message);
            if (match.Success)
            {
                target = match.Groups["name"].Value;
                return true;
            }
            target = "";
            return false;
        } 
    }
}



