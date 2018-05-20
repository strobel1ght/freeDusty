using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace freeDusty
{
    public class ModEntry : Mod
    {
        Dusty doggie;
        GameLocation spawnMap;

        public override void Entry(IModHelper helper)
        {
            helper.Content.AssetLoaders.Add(new DustyLoader(this.Helper));         

            TimeEvents.AfterDayStarted += this.AfterDayStarted;
            SaveEvents.BeforeSave += this.BeforeSave;

            MenuEvents.MenuChanged += this.MenuChanged;            

            //GameEvents.EighthUpdateTick += this.Second;
        }

        // Prevent Dusty from getting angry when the player digs through trash near him
        public void MenuChanged(object sender, EventArgs e)
        {
            if(Game1.activeClickableMenu is DialogueBox dialogue)
            {                
                //this.Monitor.Log("Dialogue is up with contents " + dialogue.getCurrentString());

                if (dialogue.getCurrentString().Equals("Hey, Stop that! ...Yuck!"))
                {                    
                    dialogue.closeDialogue();
                    doggie.isEmoting = false;
                  //  this.Monitor.Log("Abort, abort!");                    
                }
            }
        }

        public void Second(object sender, EventArgs e)
        {
            if(spawnMap != null && spawnMap.characters.Contains(doggie))
                this.Monitor.Log("Dusty is at " + doggie.Position.X/64 + "/" + doggie.Position.Y/64);
        }

        public void AfterDayStarted(object sender, EventArgs e)
        {
            String prefix = "";

            if (Game1.currentSeason.ToLower().Equals("spring") || Game1.currentSeason.ToLower().Equals("summer"))
                prefix = "spring";
            else if (Game1.currentSeason.ToLower().Equals("fall"))
                prefix = "fall";
            else
                prefix = "winter";

            // Spawn Dusty
            if (!Game1.isRaining && !Game1.isSnowing && Game1.player.IsMainPlayer)
            {
                spawnMap = Game1.getLocationFromName("Town");

                Vector2 inPen = new Vector2(53, 68) * 64f;
                Vector2 spawn = this.GetDustySpawn();

                // Spawn Dusty in his pen 70% of the time
                // Spawn him somewhere in town 30% of the time
                if (Game1.random.Next(1, 10) > 7)
                {
                    //this.Monitor.Log("Spawning Dusty at " + spawn.X + "/" + spawn.Y+"");
                    doggie = new Dusty(new AnimatedSprite("Dusty.xnb", 0, 29, 25), spawn, 0, "Dusty");
                }
                else
                {
                    //this.Monitor.Log("Spawning Dusty in his pen.");
                    doggie = new Dusty(new AnimatedSprite("Dusty.xnb", 0, 29, 25), inPen, 0, "Dusty");
                }

                spawnMap.addCharacter(doggie);

                // Make Dusty's area walkable
                // TODO: Figure this out
                /* for(int i=51; i<=54; i++)
                 {
                     for(int j=68; j<=70; j++)
                     {
                         spawnMap.setTileProperty(i, j, "Buildings", "Passable", "true");
                         spawnMap.setTileProperty(i, j, "Back", "Passable", "true");
                         spawnMap.setTileProperty(i, j, "Front", "Passable", "true");
                         spawnMap.setTileProperty(i, j, "AlwaysFront", "Passable", "true");
                         spawnMap.setTileProperty(i, j, "Paths", "Passable", "true");

                         this.Monitor.Log("Made tile " + i + "/" + j + " walkable");
                     }
                 }*/
                Helper.Content.AssetEditors.Add(new BoxEditor(this.Helper, prefix, false));
                //this.Monitor.Log("Added editor");
            }
            else
                Helper.Content.AssetEditors.Add(new BoxEditor(this.Helper, prefix, true));

            Helper.Content.InvalidateCache(prefix + "_town");
            Helper.Content.InvalidateCache(@"/Maps/" + prefix + "_town");
        }

        // Remove Dusty NPC at the end of the day to avoid serialization issues
        public void BeforeSave(object sender, EventArgs e)
        {
            if (spawnMap.getCharacterFromName("Dusty") != null)
            {
                //this.Monitor.Log("Removing Dusty to escape evil serialization");
                spawnMap.characters.Remove(doggie);
            }
        }

        private Vector2 GetDustySpawn()
        {
            //this.Monitor.Log("Finding spawn point...");

            Vector2 spawn = spawnMap.getRandomTile();

            // Must be within suitable areas (roughly at community center or around town, and not within boarded off area above Clint's)
            bool posFound = false;
            while (!posFound)
            {
                //this.Monitor.Log("Checking tile " + spawn.X + "/" + spawn.Y + " ...");

                // Acceptable areas (in front of community center and town area)
                if ((spawn.X >= 12 && spawn.X <= 65) && (spawn.Y >= 10 && spawn.Y <= 39))
                    posFound = true;
                else if ((spawn.X >= 4 && spawn.X <= 110) && (spawn.Y >= 54 && spawn.Y <= 96))
                    posFound = true;

                // Not in the cordoned off area above Clint's shop
                if (posFound && ((spawn.X >= 88 && spawn.X <= 106) && (spawn.Y >= 63 && spawn.Y <= 75)))
                    posFound = false;

                if (!posFound)
                    spawn = spawnMap.getRandomTile();
            }

            if (!spawnMap.isTileLocationTotallyClearAndPlaceable(spawn))
            {
                //this.Monitor.Log("Spawn location isn't clear, finding nearby clear location...");
                spawn = FindSafePosition(spawn);
            }

            return spawn * 64f;
        }

        private Vector2 FindSafePosition(Vector2 pos)
        {
            // Find a clear location around the specified position
            GameLocation theTown = Game1.getLocationFromName("Town");

            // Check the 5 surrounding circles
            for (int i = 1; i <= 5; i++)
            {
                // Check above
                if (theTown.isTileLocationTotallyClearAndPlaceable((int)pos.X, (int)pos.Y - i))// (new xTile.Dimensions.Location((int)pos.X, (int)pos.Y - i), Game1.viewport))
                    return new Vector2(pos.X, pos.Y - i);

                // Check below
                if (theTown.isTileLocationTotallyClearAndPlaceable((int)pos.X, (int)pos.Y + i)) //(new xTile.Dimensions.Location((int)pos.X, (int)pos.Y + i), Game1.viewport))
                    return new Vector2(pos.X, pos.Y + i);

                // Check left
                if (theTown.isTileLocationTotallyClearAndPlaceable((int)pos.X - i, (int)pos.Y)) //(new xTile.Dimensions.Location((int)pos.X - i, (int)pos.Y), Game1.viewport))
                    return new Vector2(pos.X - i, pos.Y);

                // Check right
                if (theTown.isTileLocationTotallyClearAndPlaceable((int)pos.X + i, (int)pos.Y)) //(new xTile.Dimensions.Location((int)pos.X + i, (int)pos.Y), Game1.viewport))
                    return new Vector2(pos.X + i, pos.Y);

                // Check top right
                if (theTown.isTileLocationTotallyClearAndPlaceable((int)pos.X + i, (int)pos.Y - i)) //(new xTile.Dimensions.Location((int)pos.X + i, (int)pos.Y - i), Game1.viewport))
                    return new Vector2(pos.X + i, pos.Y - i);

                // Check top left
                if (theTown.isTileLocationTotallyClearAndPlaceable((int)pos.X - i, (int)pos.Y - i)) //(new xTile.Dimensions.Location((int)pos.X - i, (int)pos.Y - i), Game1.viewport))
                    return new Vector2(pos.X - i, pos.Y - i);

                // Check below right
                if (theTown.isTileLocationTotallyClearAndPlaceable((int)pos.X + i, (int)pos.Y + i)) //(new xTile.Dimensions.Location((int)pos.X + i, (int)pos.Y + i), Game1.viewport))
                    return new Vector2(pos.X + i, pos.Y + i);

                // Check below left
                if (theTown.isTileLocationTotallyClearAndPlaceable((int)pos.X - i, (int)pos.Y + i)) //(new xTile.Dimensions.Location((int)pos.X - i, (int)pos.Y + i), Game1.viewport))
                    return new Vector2(pos.X - i, pos.Y + i);
            }
            //this.Monitor.Log("Didn't find a nearby safe position");
            return pos;
        }
    }
}