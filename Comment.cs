using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaParser
{
    public class Comment : IComparable<Comment>
    {
        public enum CommentType
        {
            Vote,
            NullVote,
            Reference,
            Discussion
        }
        public static CommentType ParseCommentType(string type)
        {
            switch (type)
            {
                case "Vote":
                    return CommentType.Vote;
                case "NullVote":
                    return CommentType.NullVote;
                case "Reference":
                    return CommentType.Reference;
                case "Discussion":
                    return CommentType.Discussion;
                default:
                    throw new Exception("Can't interpret comment type: \"" + type + "\"");
            }
        }

        public string Name { get; private set; }
        public string Message { get; private set; }
        public CommentType Type { get; private set; }
        public string Target { get; private set; }
        public Player TargetPlayer { get; private set; }
        public DateTime Time { get; private set; }
        public bool Flagged { get; set; }


        public Comment(string name, string message, string utimeseconds, CommentType type, string target)
        {
            Name = name;
            Message = message;
            Type = type;
            Target = target;
            Flagged = false;

            DateTimeOffset dTO = DateTimeOffset.FromUnixTimeSeconds(long.Parse(utimeseconds));
            Time = dTO.UtcDateTime;
        }

        public Comment(string name, string message, string utime)
        {
            Name = name;
            Message = message;
            Time = DateTime.Parse(utime);

            string target;
            Type = Parser.EvaluateType(message, out target);
            Target = target;

            Flagged = false;
        }

        public void SetTargetPlayer(Player targetplayer)
        {
            TargetPlayer = targetplayer;
        }

        public void Print()
        {
            if (Type == CommentType.NullVote || Type == CommentType.Vote)
            {

                Console.WriteLine("{0} commented: \"{1}\"\nAt {2}\nType: {3} targeting {4}",
                                        Name,
                                        Message,
                                        Time.ToString(),
                                        Type.ToString(),
                                        Target.ToString()
                                        );
            }
            else
            {
                Console.WriteLine("{0} commented: \"{1}\"\nAt {2}\nType: {3}",
                                        Name,
                                        Message,
                                        Time.ToLocalTime().ToString(),
                                        Type.ToString()
                                        );

            }
        }

        public int CompareTo(Comment other)
        {
            return Time.CompareTo(other.Time);
        }

        override
        public string ToString()
        {
            
            return String.Format("{0} [{1}]: {2}", Name, Time.ToString(), Message);
        }
    }
}
