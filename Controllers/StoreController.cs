using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using gameStoreAPI.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace gameStoreAPI.Controllers
{
    class ControllerException : ApplicationException 
    {
        public string correction {get;set;}
        public ControllerException(string correction)
        {
            this.correction = correction;
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class StoreController : ControllerBase
    {
        static private List<User> users;
        static private List<Game> games;
        static private List<Item> items;
        public StoreController()
        {
            if (users == null)
            {
                users = new List<User>();
                InitAdd(1);
            }
            if (games == null)
            {
                games = new List<Game>();
                InitAdd(2);
            }
            if (items == null)
            {
                items = new List<Item>();
                InitAdd(3);
            }
        }

        void InitAdd(int whichAdd)
        {
            if (whichAdd == 1) 
            {
                users.Add(new User("user001", "Pedro", DateTime.UtcNow, ["game001", "game002"]));
                users.Add(new User("user002", "Matheus", DateTime.UtcNow, ["game002"]));
            }
            if (whichAdd == 2) 
            {
                games.Add(new Game("game001","Food War", "Real Publisher", "1 Game", 2012, ["item001","item002"]));
                games.Add(new Game("game002","Outer Wilds", "Annapurna Interactive", "Mobius Studios", 2019));
            }
            if (whichAdd == 3) 
            {
                items.Add(new Item("item002", "Chocolate Shield", 50.70, "Sometimes the best defense is a good sugary dessert!"));
                items.Add(new Item("item001", "The Ham Axe", 150.50, "Cutting with style is the only way to taste victory!"));
            } 
        }

        // CRUD for users
        [HttpGet("GetAllUsers")]
        public List<User> GetAllUsers()
        {
            return users;
        }
        
        [HttpGet("GetUserById/{userId}")]
        public IActionResult GetUserById(string userId)
        {
            foreach (User u in users)
            {
                if (u.userId == userId)
                    return Ok(u);
            }
            return NotFound($"User with Id = {userId} not found");
        }

        [HttpPost("AddUser/{userId}/{userName}")]
        public IActionResult AddUser(string userId, string userName, [FromQuery] List<string> userGameIds)
        {
            try
            {
                if (userGameIds != null)
                {
                    List<string> convertedGameIds = new List<string>();

                    foreach (var gameId in userGameIds)
                    {
                        try
                        {
                            string convertedId = Convert.ToString(gameId);
                            convertedGameIds.Add(convertedId);
                        }
                        catch (InvalidCastException)
                        {
                            return BadRequest($"Invalid value in userGameIds. Unable to convert '{gameId}' to string.");
                        }
                    }

                    var invalidGameIds = convertedGameIds.Except(games.Select(g => g.gameId)).ToList();
                    if (invalidGameIds.Any())
                    {
                        return BadRequest($"Invalid game Ids: {string.Join(", ", invalidGameIds)}. Use null or valid game Ids");
                    }
                }
                if (users.Any(us => us.userId == userId))
                {
                    return BadRequest($"A user with Id = {userId} already exists");
                }
                foreach (string i in userGameIds)
                {
                    bool ver = false;
                    foreach (Game g in games)
                    {
                        if (i == g.gameId)
                        {
                            ver = true;
                        }
                    }
                    if (ver == false) 
                    {
                        return BadRequest("Inexistent gameId. Use null or a valid game Id");
                    }
                }

                User newUser = new User(userId, userName, DateTime.UtcNow, userGameIds);
                users.Add(newUser);

                return Ok(users);
            }
            catch (UserException e)
            {
                return BadRequest(e.correction);
            }
        }

        [HttpPut("UpdateUser/{userId}")]
        public IActionResult UpdateUser(string userId, string updatedUserName, [FromQuery] List<string> updatedUserGameIds)
        {
            try
            {
                foreach (User u in users) 
                {
                    if (u.userId == userId) 
                    {
                        if (u.userGameIds.Any(item1 => updatedUserGameIds.Any(item2 => item1.Equals(item2))))
                        {
                            return BadRequest($"User with id = {userId} already has one of the given gameIds");
                        }

                        if (updatedUserGameIds.Any(i => !games.Any(g => g.gameId == i)))
                        {
                            return BadRequest("Inexistent gameId");
                        }

                        u.userName = updatedUserName;

                        foreach (string i in updatedUserGameIds)
                        {
                            u.userGameIds.Add(i);
                        }

                        return Ok(u);
                    }
                }
                return NotFound($"User with Id = {userId} not found");
            }
            catch (UserException e)
            {
                return BadRequest(e.correction);
            }
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(string userId)
        {
            foreach (User u in users) 
            {
                if (u.userId == userId) 
                {
                    users.Remove(u);
                    return Ok(users);
                }
            }
            return NotFound($"User with Id = {userId} not found");
        }

        [HttpGet("GetAllGames")]
        public List<Game> GetAllGames() 
        {
            return games;
        }

        [HttpGet("GetGameById/{gameId}")]
        public IActionResult GetGameById(string gameId) 
        {
            try 
            {
                foreach (Game g in games)
                {
                    if (g.gameId == gameId)
                        return Ok(g);
                }
                return NotFound($"Game with Id = {gameId} not found");
            }
            catch (GameException e)
            {
                return BadRequest(e.correction);
            }
        }

        [HttpPost("AddGame/{gameId}/{gameName}/{publisher}/{developer}/{launchYear}")]
        public IActionResult AddGame(string gameId, string gameName, string publisher, string developer, int launchYear, [FromQuery] List<string> gameItemIds) 
        {
            try
            {
                if (games.Any(g => g.gameId == gameId))
                {
                    return BadRequest($"A game with Id = {gameId} already exists");
                }

                foreach (string i in gameItemIds)
                {
                    bool ver = false;
                    foreach (Item j in items)
                    {
                        if (i == j.itemId)
                        {
                            ver = true;
                        }
                    }
                    if (ver == false) 
                    {
                        return BadRequest("Inexistent itemId");
                    }
                }
                
                Game newGame = new Game(gameId, gameName, publisher, developer, launchYear, gameItemIds);
                games.Add(newGame);
            }
            catch (GameException e)
            {
                return BadRequest(e.correction);
            }
            return Ok(games);
        }

        [HttpPut("UpdateGame/{gameId}/{newGameName}/{newPublisher}/{newDeveloper}")]
        public IActionResult UpdateGame(string gameId, string newGameName, string newPublisher, string newDeveloper) 
        {
            try
            {
                foreach (Game u in games) 
                {
                    if (u.gameId == gameId) 
                    {
                        u.gameName = newGameName;
                        u.publisher = newPublisher;
                        u.developer = newDeveloper;
                        return Ok(u);
                    }
                }
            }
            catch (GameException e)
            {
                return BadRequest(e.correction);
            }
            return NotFound($"Game with Id = {gameId} not found");
        }

        [HttpDelete("DeleteGame/{gameId}")]
        public IActionResult DeleteGame(string gameId)
        {
            try
            {
                bool gameExists = false;
                foreach (Game game in games)
                {
                    if (game.gameId == gameId)
                    {
                        gameExists = true;
                        break;
                    }
                }
                
                if (gameExists == false)
                {
                    return BadRequest($"Game with Id = {gameId} not found");
                }

                games.RemoveAll(g => g.gameId == gameId);

                foreach (User u in users)
                {
                    u.userGameIds.RemoveAll(game => game == gameId);
                }

                return Ok(games);
            }
            catch (ItemException e)
            {
                return BadRequest(e.correction);
            }
        }

        [HttpGet("GetAllItems")]
        public List<Item> GetAllItems()
        {
            return items;
        }

        [HttpGet("GetItemById/{itemId}")]
        public IActionResult GetItemById(string itemId)
        {
            foreach (Item i in items)
            {
                if (i.itemId == itemId)
                    return Ok(i);
            }
            return NotFound($"Item with Id = {itemId} not found");
        }

        [HttpPost("AddItem/{itemId}/{itemName}/{itemValue}/{itemDescription}")]
        public IActionResult AddItem(string itemId, string itemName, double itemValue, string itemDescription)
        {   
            if (items.Any(i => i.itemId == itemId))
            {
                return BadRequest($"A Item with Id = {itemId} already exists");
            }

            Item newItem = new Item(itemId, itemName, itemValue, itemDescription);
            items.Add(newItem);
            return Ok(items);
        }

        [HttpPut("UpdateItem/{itemId}/{newItemName}/{newItemValue}/{newItemDescription}")]
        public IActionResult UpdateItem(string itemId, string newItemName, double newItemValue, string newItemDescription)
        {
            try
            {
                foreach (Item i in items) 
                {
                    if (i.itemId == itemId) 
                    {
                        i.itemName = newItemName;
                        i.itemValue = newItemValue;
                        i.itemDescription = newItemDescription;
                        return Ok(i);
                    }
                }
            }
            catch (ItemException e)
            {
                return BadRequest(e.correction);
            }
            return NotFound($"Item with Id = {itemId} not found");
        }

        [HttpDelete("DeleteItem/{itemId}")]
        public IActionResult DeleteItem(string itemId) 
        {
            try
            {
                bool itemExists = false;
                foreach (Item item in items)
                {
                    if (item.itemId == itemId)
                    {
                        itemExists = true;
                        break;
                    }
                }
                
                if (itemExists == false)
                {
                    return BadRequest($"Item with Id = {itemId} not found");
                }

                items.RemoveAll(i => i.itemId == itemId);

                foreach (Game g in games)
                {
                    g.gameItemIds.RemoveAll(item => item == itemId);
                }

                return Ok(items);
            }
            catch (ItemException e)
            {
                return BadRequest(e.correction);
            }
        }
    }
}
