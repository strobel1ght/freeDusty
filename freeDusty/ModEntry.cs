using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace freeDusty
{
    public class ModEntry : Mod
    {
        Dusty doggie;

        // TODO: Edit cursors(?) to remove Dusty's evil eyes from his box        
        public override void Entry(IModHelper helper)
        {
            TimeEvents.AfterDayStarted += this.AfterDayStarted;
            SaveEvents.BeforeSave += this.BeforeSave;
            GameEvents.HalfSecondTick += this.OneSecond;
        }

        public void OneSecond(object sender, EventArgs e)
        {
            if(Game1.currentLocation != null) { 
                //this.Monitor.Log("Collides? "+doggie.collides.ToString()+" -- moved? "+doggie.moved);
                //this.Monitor.Log("Next position: " + doggie.nextPosition(doggie.FacingDirection).ToString());
            }
            // this.Monitor.Log(Game1.currentLocation.ToString());
        }        

        public void AfterDayStarted(object sender, EventArgs e)
        {
            if (!Game1.isRaining && !Game1.isSnowing /*&& !Game1.IsMultiplayer/*&& !Game1.player.IsMainPlayer*/)
            {
                Vector2 pos = new Vector2(/*53, 68) * 64f; */67, 18) * 64f; //<-- for farm

                // TODO: Load sprite from mod folder       
                // TODO: Make him spawn randomly around town but with a slight preferance for his hut 
                // TODO: Remove eyes from hut if he's out and about
                 doggie = new Dusty(new AnimatedSprite("myDusty.xnb", 0, 29, 25), pos, 0, "Dusty");
                
                Game1.getLocationFromName(/*"Town"*/"Farm").addCharacter(doggie);

                this.Monitor.Log("Here, have a Dusty");               
            }
        }

        public void BeforeSave(object sender, EventArgs e)
        {
            if (Game1.getLocationFromName("Farm").getCharacterFromName("Dusty") != null)
            {
                this.Monitor.Log("Removing Dusty to escape evil serialization");
                Game1.getLocationFromName("Farm").characters.Remove(doggie);
            }
            else
                this.Monitor.Log("No Dusty to remove");
        }
    }
}