using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace freeDusty
{
    public class ModEntry : Mod
    {
        private Dusty _doggie;
        private GameLocation _spawnMap;
        private string _prefix = "";

        public override void Entry(IModHelper helper)
        {
            if (Context.IsMultiplayer) return;
            helper.Content.AssetLoaders.Add(new DustyLoader(Helper));
            Helper.Events.GameLoop.DayStarted += AfterDayStarted;
            Helper.Events.GameLoop.Saving += BeforeSave;
            Helper.Events.Display.MenuChanged += MenuChanged;
            Helper.Events.Player.Warped += Warped;

            //GameEvents.EighthUpdateTick += this.Second;
        }

        // Reload town sprites
        private void Warped(object sender, EventArgs e)
        {
            Helper.Content.InvalidateCache("/Maps/" + _prefix + "_town");
        }

        // Prevent Dusty from getting angry when the player digs through trash near him
        private void MenuChanged(object sender, EventArgs e)
        {
            if (!(Game1.activeClickableMenu is DialogueBox dialogue)) return;
            //this.Monitor.Log("Dialogue is up with contents " + dialogue.getCurrentString());
            if (!dialogue.getCurrentString().Equals("Hey, Stop that! ...Yuck!")) return;
            dialogue.closeDialogue();
            _doggie.isEmoting = false;
            //this.Monitor.Log("Abort, abort!");                    
        }

        public void Second(object sender, EventArgs e)
        {
            //if(spawnMap != null && spawnMap.characters.Contains(doggie))
            //this.Monitor.Log("Dusty is at " + doggie.Position.X/64 + "/" + doggie.Position.Y/64);
        }

        private void AfterDayStarted(object sender, EventArgs e)
        {
            _prefix = "";
            switch (Game1.currentSeason.ToLower())
            {
                case "spring":
                case "summer":
                    _prefix = "spring";
                    break;
                case "fall":
                    _prefix = "fall";
                    break;
                default:
                    _prefix = "winter";
                    break;
            }

            // Determine spawn map
            // If player is married to Alex, make Dusty spawn on the farm or in the farm house
            if (Game1.getCharacterFromName("Alex").isMarried())
            {
                // If it's raining or snowing
                if (Game1.isRaining || Game1.isSnowing) _spawnMap = Game1.getLocationFromName("FarmHouse");
                else _spawnMap = Game1.getLocationFromName("Farm");
            }
            else if (!Game1.isRaining && !Game1.isSnowing)
            {
                _spawnMap = Game1.getLocationFromName("Town");
            }

            // Find spawn point and spawn Dusty
            if (Game1.player.IsMainPlayer && _spawnMap != null)
            {
                _doggie = new Dusty(new AnimatedSprite("Dusty.xnb", 0, 29, 25), GetDustySpawn(), 0, "Dusty");
                _spawnMap.addCharacter(_doggie);
                Helper.Content.AssetEditors.Add(new BoxEditor(Helper, _prefix));

                //this.Monitor.Log("Added Dusty to the spawn map.");
            }
            // If no spawn map could be determined, Dusty should be in his box
            else if (_spawnMap == null)
            {
                Helper.Content.AssetEditors.Add(new BoxEditor(Helper, _prefix, true));
            }

            if (_spawnMap == null)
                //this.Monitor.Log("Did not spawn Dusty today... or did I?");
                foreach (var n in Game1.getLocationFromName("Farm").getCharacters()
                    .Where(n => n.getName().Equals("Dusty")))
                {
                    Game1.getLocationFromName("Farm").characters.Remove(n);
                    //this.Monitor.Log("Found a stray Dusty and removed him.");
                    break;
                }

            /* // TODO: Figure this out
                /* 
                 * if(spawnMap.Name.Equals("Town")) {
                 * for(int i=51; i<=54; i++)
                 {
                     for(int j=68; j<=70; j++)
                     {
                         spawnMap.setTileProperty(i, j, "Buildings", "Passable", "true");
                         spawnMap.setTileProperty(i, j, "Back", "Passable", "true");
                         spawnMap.setTileProperty(i, j, "Front", "Passable", "true");
                         spawnMap.setTileProperty(i, j, "AlwaysFront", "Passable", "true");
                         spawnMap.setTileProperty(i, j, "Paths", "Passable", "true");

                         //this.Monitor.Log("Made tile " + i + "/" + j + " walkable");
                     }
                 }
                 }
               */
            Helper.Content.InvalidateCache(_prefix + "_town");
            Helper.Content.InvalidateCache(@"/Maps/" + _prefix + "_town");
        }

        // Remove Dusty NPC at the end of the day to avoid serialization issues
        private void BeforeSave(object sender, EventArgs e)
        {
            if (_spawnMap != null && _doggie != null && _doggie.currentLocation.characters.Contains(_doggie))
                //this.Monitor.Log("Removing Dusty from " + doggie.currentLocation.Name + " ... is he on that map? " + doggie.currentLocation.characters.Contains(doggie) + " ... he is at " + doggie.currentLocation.ToString());
                //this.Monitor.Log("Removing Dusty to escape evil serialization");
                _doggie.currentLocation.characters.Remove(_doggie);
        }

        private Vector2 GetDustySpawn()
        {
            //this.Monitor.Log("Finding spawn point...");
            Vector2 spawn;
            // 70% chance to just spawn him in his pen
            // When married to Alex, spawn him near the water bowl instead
            // If spawning inside the farmhouse, leave him alone
            if (Game1.random.Next(1, 10) <= 7)
                switch (_spawnMap.Name)
                {
                    case "Town":
                        //this.Monitor.Log("Spawning at pen", LogLevel.Debug);
                        spawn.X = 53;
                        spawn.Y = 68;
                        break;
                    case "Farm":
                        //this.Monitor.Log("Spawning near the water bowl", LogLevel.Debug);
                        spawn.X = 52;
                        spawn.Y = 7;
                        break;
                    default:
                        spawn = GetRandomSpawn();
                        break;
                }
            else
                spawn = GetRandomSpawn();

            return spawn * 64f;
        }

        private Vector2 GetRandomSpawn()
        {
            var spawn = _spawnMap.getRandomTile();

            // Must be within suitable areas (roughly at community center or around town, and not within boarded off area above Clint's)
            var posFound = false;
            while (!posFound)
            {
                //this.Monitor.Log("Checking tile " + spawn.X + "/" + spawn.Y + " on map "+spawnMap+"...");
                switch (_spawnMap.Name)
                {
                    case "Town":
                    {
                        // Acceptable areas (in front of community center and town area)
                        if (spawn.X >= 12 && spawn.X <= 65 && spawn.Y >= 10 && spawn.Y <= 39) posFound = true;
                        else if (spawn.X >= 4 && spawn.X <= 110 && spawn.Y >= 54 && spawn.Y <= 96) posFound = true;

                        // Not in the cordoned off area above Clint's shop
                        if (posFound && spawn.X >= 88 && spawn.X <= 106 && spawn.Y >= 63 && spawn.Y <= 75)
                            posFound = false;
                        break;
                    }
                    case "Farm":
                    {
                        // Acceptable area around the farm
                        if (spawn.X >= 42 && spawn.X <= 73 && spawn.Y >= 10 && spawn.Y <= 30) posFound = true;
                        break;
                    }
                    case "FarmHouse":
                    {
                        // Take max upgrade and also check for .isonmap
                        if (_spawnMap.isTileOnMap(spawn) && spawn.X >= 1 && spawn.X <= 40 && spawn.Y >= 4 &&
                            spawn.Y <= 22) posFound = true;
                        break;
                    }
                }

                if (!posFound) spawn = _spawnMap.getRandomTile();
            }

            //this.Monitor.Log("Found position " + spawn.X + "/" + spawn.Y + " on map " + spawnMap);
            if (!_spawnMap.isTileLocationTotallyClearAndPlaceable(spawn))
                //this.Monitor.Log("Spawn location isn't clear, finding nearby clear location...");
                spawn = FindSafePosition(spawn);

            //this.Monitor.Log("Seems to be clear for spawn.");
            return spawn;
        }

        private Vector2 FindSafePosition(Vector2 pos)
        {
            // Find a clear location around the specified position
            var theLocation = _spawnMap;

            // Check the 5 surrounding circles
            for (var i = 1; i <= 5; i++)
            {
                // Check above
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int) pos.X, (int) pos.Y - i)
                ) // (new xTile.Dimensions.Location((int)pos.X, (int)pos.Y - i), Game1.viewport))
                    return new Vector2(pos.X, pos.Y - i);

                // Check below
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int) pos.X, (int) pos.Y + i)
                ) //(new xTile.Dimensions.Location((int)pos.X, (int)pos.Y + i), Game1.viewport))
                    return new Vector2(pos.X, pos.Y + i);

                // Check left
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int) pos.X - i, (int) pos.Y)
                ) //(new xTile.Dimensions.Location((int)pos.X - i, (int)pos.Y), Game1.viewport))
                    return new Vector2(pos.X - i, pos.Y);

                // Check right
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int) pos.X + i, (int) pos.Y)
                ) //(new xTile.Dimensions.Location((int)pos.X + i, (int)pos.Y), Game1.viewport))
                    return new Vector2(pos.X + i, pos.Y);

                // Check top right
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int) pos.X + i, (int) pos.Y - i)
                ) //(new xTile.Dimensions.Location((int)pos.X + i, (int)pos.Y - i), Game1.viewport))
                    return new Vector2(pos.X + i, pos.Y - i);

                // Check top left
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int) pos.X - i, (int) pos.Y - i)
                ) //(new xTile.Dimensions.Location((int)pos.X - i, (int)pos.Y - i), Game1.viewport))
                    return new Vector2(pos.X - i, pos.Y - i);

                // Check below right
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int) pos.X + i, (int) pos.Y + i)
                ) //(new xTile.Dimensions.Location((int)pos.X + i, (int)pos.Y + i), Game1.viewport))
                    return new Vector2(pos.X + i, pos.Y + i);

                // Check below left
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int) pos.X - i, (int) pos.Y + i)
                ) //(new xTile.Dimensions.Location((int)pos.X - i, (int)pos.Y + i), Game1.viewport))
                    return new Vector2(pos.X - i, pos.Y + i);
            }

            //this.Monitor.Log("Didn't find a nearby safe position");
            return pos;
        }
    }
}