using System;

namespace gameStoreAPI.Models
{
    class GameException : ApplicationException 
    {
        public string correction {get;set;}
        public GameException(string correction)
        {
            this.correction = correction;
        }
    }

    public class Game
    {
        public string gameId { get; }

        private string _gameName;
        public string gameName
        {
            get
            {
                return this._gameName;
            }
            set
            {
                if (value == null || value == "") 
                {
                    throw new UserException("Invalid gameName");
                }
                else if (value.Length > 20)
                {
                    throw new UserException("gameName too long.");
                }
                this._gameName = value;
            }
        }

        private string _publisher;
        public string publisher
        {
            get
            {
                return this._publisher;
            }
            set
            {
                if (value == null || value == "") 
                {
                    throw new UserException("Invalid publisher");
                }
                this._publisher = value;
            }
        }

        private string _developer;
        public string developer
        {
            get
            {
                return this._developer;
            }
            set
            {
                if (value == null || value == "") 
                {
                    throw new UserException("Invalid developer");
                }
                this._developer = value;
            }
        }
        public int launchYear { get; }
        public List<string> gameItemIds { get;set; }

        public Game()
        {
           this.gameItemIds = new List<string>();
        }
        public Game(string gameId, string gameName, string publisher, string developer, int launchYear, List<string>? gameItemIds = null)
        {
            this.gameId = gameId;
            this.gameName = gameName;
            this.publisher = publisher;
            this.developer = developer;
            this.launchYear = launchYear;
            this.gameItemIds = gameItemIds ?? new List<string>();
        }
    }
}