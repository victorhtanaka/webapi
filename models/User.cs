using System;
using Microsoft.AspNetCore.Http.HttpResults;

namespace gameStoreAPI.Models
{
    class UserException : ApplicationException 
    {
        public string correction {get;set;}
        public UserException(string correction)
        {
            this.correction = correction;
        }
    }

    public class User
    {
        private string _userName;
        public string userId { get; }
        public string userName
        {
            get
            {
                return this._userName;
            }
            set 
            {
                if (value == null || value == "") 
                {
                    throw new UserException("Invalid userName");
                }
                else if (value.Length > 15)
                {
                    throw new UserException("Username too long.");
                }
                this._userName = value;
            }
        }
        public DateTime creationDate { get; }
        public List<string> userGameIds {get; set;}

        public User()
        {
            this.userGameIds = new List<string>();
        }
        public User(string userId, string userName, DateTime creationDate, List<string>? userGameIds = null)
        {
            this.userId = userId;
            this.userName = userName;
            this.creationDate = creationDate;
            this.userGameIds = userGameIds ?? new List<string>();
        }
    }
}