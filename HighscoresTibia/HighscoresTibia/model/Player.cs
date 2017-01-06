using System.Collections.Generic;

namespace HighscoresTibia.model
{
    public class Player
    {
        public string world             { get; set; }
        public string relativePosition  { get; set; }
        public string name              { get; set; }
        public string vocation          { get; set; }
        public string level             { get; set; }
        public string experience        { get; set; }
    }

    public class AllPlayers
    {
        public List<Player> Players { get; set; }
    }
}

