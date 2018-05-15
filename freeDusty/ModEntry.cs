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
            TimeEvents.TimeOfDayChanged += this.TimeChanged;
        }

        public void TimeChanged(object sender, EventArgs e)
        {
            this.Monitor.Log(doggie.currentBehavior.ToString());
        }        

        public void AfterDayStarted(object sender, EventArgs e)
        {
            if (!Game1.isRaining && !Game1.isSnowing)
            {
                Vector2 pos = new Vector2(53, 68) * 64f; //67, 18) * 64f; <-- for farm

                // TODO: Load sprite from mod folder
                // TODO: There's a constructor with a schedule, maybe use that
                doggie = new Dusty(new AnimatedSprite("myDusty.xnb", 0, 29, 25), pos, 0, "Dusty");

                Game1.getLocationFromName("Town"/*"Farm"*/).addCharacter(doggie);

                this.Monitor.Log("Here, have a Dusty");
            }
        }
    }
}