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
        public override void Entry(IModHelper helper)
        {
            TimeEvents.AfterDayStarted += this.AfterDayStarted;
        }

        public void AfterDayStarted(object sender, EventArgs e)
        {
            Vector2 pos = new Vector2(67, 18) * 64f;

            AnimatedSprite test = new AnimatedSprite("myDusty.xnb", 0, 29, 25);

            StardewValley.NPC doggie = new StardewValley.NPC(test, pos, 0, "Dusty");

            Game1.getLocationFromName("Farm").addCharacter(doggie);
            doggie.Position = pos;
            this.Monitor.Log("Here, have a Dusty");


        }
    }
}