using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace freeDusty
{
    class Dusty : NPC
    {
        public int currentBehavior = 0;
        private int startedBehavior = -1;

        public Dusty(AnimatedSprite animatedSprite, Vector2 position, int facingDir, string name)
        {
            this.Sprite = animatedSprite;
            this.Position = position;
            this.FacingDirection = facingDir;
            this.Name = name;

            this.DefaultMap = "Town";//"Farm";
            this.Speed = 1;
        }

        public override bool canTalk()
        {
            return false;
        }

        // TODO: Rewrite this shit
        public override void update(GameTime time, GameLocation location, long id, bool move)
        {
            // From pet.update
            if (this.startedBehavior != this.currentBehavior)
                this.initiateCurrentBehavior();
            base.update(time, location, id, move);          
            // Done from pet.update
           
            // Test if this goes ok
            //this.initiateCurrentBehavior();

            if (Game1.eventUp || Game1.IsClient)
                return;

            if(Game1.timeOfDay > 2000 && this.Sprite.CurrentAnimation == null && ((double) this.xVelocity == 0.0 && (double)this.yVelocity == 0.0))            
                this.currentBehavior = 1;

            switch(this.currentBehavior)
            {
                case 0:
                    if (this.Sprite.CurrentAnimation == null && Game1.random.NextDouble() < 0.01)
                    {
                        switch (Game1.random.Next(7 + (this.currentLocation is Farm/*StardewValley.Locations.Town*/ ? 1 : 0)))
                        {
                            case 0:
                            case 1:
                                this.randomSquareMovement(time);
                                break;
                            case 2:
                            case 3:
                                this.currentBehavior = 0;
                                break;
                            case 4:
                            case 5:
                                switch (this.FacingDirection)
                                {
                                    case 0:
                                    case 1:
                                    case 3:
                                        this.Halt();
                                        if (this.FacingDirection == 0)
                                            this.FacingDirection = Game1.random.NextDouble() < 0.5 ? 3 : 1;
                                        this.faceDirection(this.FacingDirection);
                                        this.Sprite.loop = false;
                                        this.currentBehavior = 50;
                                        break;
                                    case 2:
                                        this.Halt();
                                        this.faceDirection(2);
                                        this.Sprite.loop = false;
                                        this.currentBehavior = 2;
                                        break;
                                }
                                break;
                            case 6:
                            case 7:
                                this.currentBehavior = 51;
                                break;
                        }
                    }
                    else
                        break;
                    break;
                case 1:
                    if(Game1.timeOfDay < 2000 && Game1.random.NextDouble() < 0.001)
                    {
                        this.currentBehavior = 0;
                        return;
                    }
                    if (Game1.random.NextDouble() >= 0.002)
                        return;
                    this.doEmote(24, true);
                    return;
                case 2:
                    if(this.Sprite.currentFrame != 18 && this.Sprite.CurrentAnimation == null)
                    {
                        //this.currentBehavior = 2;
                        this.currentBehavior = 0;
                        break;
                    }
                    if (this.Sprite.currentFrame == 18 && Game1.random.NextDouble() < 0.01)
                    {
                        switch (Game1.random.Next(4))
                        {
                            case 0:
                                this.currentBehavior = 0;
                                this.Halt();
                                this.faceDirection(2);
                                // TODO: Probably edit this
                                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(17, 200),
                                    new FarmerSprite.AnimationFrame(16, 200),
                                    new FarmerSprite.AnimationFrame(0, 200)
                                });
                                this.Sprite.loop = false;
                                break;
                            case 1:
                                List<FarmerSprite.AnimationFrame> animation1 = new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(18, 200, false, false),/*, new AnimatedSprite.endOfAnimationBehavior(this.pantSound), false*/
                                    new FarmerSprite.AnimationFrame(19, 200)
                                };
                                int num1 = Game1.random.Next(7, 20);
                                for (int index = 0; index < num1; ++index)
                                {
                                    animation1.Add(new FarmerSprite.AnimationFrame(18, 200, false, false)); /*new AnimatedSprite.endOfAnimationBehavior(this.pantSound), false*/
                                    animation1.Add(new FarmerSprite.AnimationFrame(19, 200));
                                }
                                this.Sprite.setCurrentAnimation(animation1);
                                break;
                            case 2:
                                this.randomSquareMovement(time);
                                break;
                            case 3:
                                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(27, Game1.random.NextDouble() < 0.3 ? 500 : Game1.random.Next(2000, 15000)),
                                    new FarmerSprite.AnimationFrame(18, 1, false, false) //new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold), false)
                                });
                                this.Sprite.loop = false;
                                break;
                        }
                    }
                    else
                        break;
                    break;
                case 50:
                    if(this.withinPlayerThreshold(2))
                    {
                        /*if(!this.wagging)
                         * {
                         * this.wag(this.FacingDirection == 3);
                         * break;
                         * }*/
                        break;
                    }
                    if(this.Sprite.currentFrame != 23 && this.Sprite.CurrentAnimation == null)
                    {
                        this.Sprite.currentFrame = 23;
                        break;
                    }
                    if (this.Sprite.currentFrame == 23 && Game1.random.NextDouble() < 0.01)
                    {
                        bool flag = this.FacingDirection == 3;
                        switch (Game1.random.Next(7))
                        {
                            case 0:
                                this.currentBehavior = 0;
                                this.Halt();
                                this.faceDirection(flag ? 3 : 1);
                                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(23, 100, false, flag/*, (AnimatedSprite.endOfAnimationBehavior) null, false*/),
                                    new FarmerSprite.AnimationFrame(22, 100, false, flag/*, (AnimatedSprite.endOfAnimationBehavior) null, false*/),
                                    new FarmerSprite.AnimationFrame(21, 100, false, flag/*, (AnimatedSprite.endOfAnimationBehavior) null, false*/),
                                    new FarmerSprite.AnimationFrame(20, 100, false, flag/*, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold), false*/)
                                });
                                this.Sprite.loop = false;
                                break;
                            case 1:
                                if (Utility.isOnScreen(this.getTileLocationPoint(), 640, this.currentLocation))
                                {
                                    Game1.playSound("dog_bark");
                                    this.shake(500);
                                }
                                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(26, 500, false, flag/*, (AnimatedSprite.endOfAnimationBehavior) null, false*/),
                                    new FarmerSprite.AnimationFrame(23, 1, false, flag/*, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold), false*/)
                                });
                                break;
                            case 2:
                                //this.wag(flag);
                                break;
                            case 3:
                            case 4:
                                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                                  {
                                    new FarmerSprite.AnimationFrame(23, Game1.random.Next(2000, 6000), false, flag/*, (AnimatedSprite.endOfAnimationBehavior) null, false*/),
                                    new FarmerSprite.AnimationFrame(23, 1, false, flag/*, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold), false*/)
                                  });
                                break;
                            default:
                                this.Sprite.loop = false;
                                List<FarmerSprite.AnimationFrame> animation2 = new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(24, 200, false, flag/*, new AnimatedSprite.endOfAnimationBehavior(this.pantSound), false*/),
                                    new FarmerSprite.AnimationFrame(25, 200, false, flag/*, (AnimatedSprite.endOfAnimationBehavior) null, false*/)
                                };
                                int num2 = Game1.random.Next(5, 15);
                                for (int index = 0; index < num2; ++index)
                                {
                                    animation2.Add(new FarmerSprite.AnimationFrame(24, 200, false, flag/*, new AnimatedSprite.endOfAnimationBehavior(this.pantSound), false*/));
                                    animation2.Add(new FarmerSprite.AnimationFrame(25, 200, false, flag/*, (AnimatedSprite.endOfAnimationBehavior)null, false*/));
                                }
                                this.Sprite.setCurrentAnimation(animation2);
                                break;
                        }
                    }
                    else
                        break;
                    break;
            }
            if (this.Sprite.CurrentAnimation != null)
                this.Sprite.loop = false;
            //else
            // this.wagging = false;
            if (!Game1.IsMasterGame || this.Sprite.CurrentAnimation != null)
                return;
            //this.MovePosition(time, Game1.viewport, location);            
            this.randomSquareMovement(time);
        }

        public void initiateCurrentBehavior()
        {
            //this.sprintTimer = 0;

            // from pet.initiateCurrentBehavior();
            this.flip = false;
            bool flip1 = false;

            switch(this.currentBehavior)
            {
                case 0:
                    this.Halt();
                    this.faceDirection(Game1.random.Next(4));
                    if(Game1.IsMasterGame)
                    {
                        //this.setMovingInFacingDirection();  
                        // TODO: this causes movement problems, weirdly
                        this.walkInSquare(1, 1, 10);
                        break;
                    }
                    break;
                case 1:
                    this.Sprite.loop = true;
                    bool flip2 = Game1.random.NextDouble() < 0.5;
                    this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(28, 1000, false, flip2/*, (AnimatedSprite.endOfAnimationBehavior) null, false*/),
                        new FarmerSprite.AnimationFrame(29, 1000, false, flip2/*, (AnimatedSprite.endOfAnimationBehavior) null, false*/)
                    });
                    break;
                case 2:
                    this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(16, 100, false, flip1/*, (AnimatedSprite.endOfAnimationBehavior) null, false*/),
                        new FarmerSprite.AnimationFrame(17, 100, false, flip1/*, (AnimatedSprite.endOfAnimationBehavior) null, false*/),
                        new FarmerSprite.AnimationFrame(18, 100, false, flip1/*, new AnimatedSprite.endOfAnimationBehavior(this.hold), false*/)
                    });
                    break;
            }
            this.startedBehavior = this.currentBehavior;

            // end from pet.initiate

            flip1 = this.FacingDirection == 3;
            switch(this.currentBehavior)
            {
                case 50:
                    this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(20, 100, false, flip1/*, (AnimatedSprite.endOfAnimationBehavior) null, false*/),
                        new FarmerSprite.AnimationFrame(21, 100, false, flip1/*, (AnimatedSprite.endOfAnimationBehavior) null, false*/),
                        new FarmerSprite.AnimationFrame(22, 100, false, flip1/*, (AnimatedSprite.endOfAnimationBehavior) null, false*/),
                        new FarmerSprite.AnimationFrame(23, 100, false, flip1/*, new AnimatedSprite.endOfAnimationBehavior(((Pet) this).hold), false*/)
                    });
                    break;
                case 51:
                    this.faceDirection(Game1.random.NextDouble() < 0.5 ? 3 : 1);
                    bool flip2 = this.FacingDirection == 3;
                    // this.sprintTimer = Game1.random.Next(1000, 3500);
                    if (Utility.isOnScreen(this.getTileLocationPoint(), 64, this.currentLocation))
                        Game1.playSound("dog_bark");
                    this.Sprite.loop = true;
                    this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(32, 100, false, flip2, (AnimatedSprite.endOfAnimationBehavior) null, false),
                        new FarmerSprite.AnimationFrame(33, 100, false, flip2, (AnimatedSprite.endOfAnimationBehavior) null, false),
                        new FarmerSprite.AnimationFrame(34, 100, false, flip2/*, new AnimatedSprite.endOfAnimationBehavior(this.hitGround), false*/),
                        new FarmerSprite.AnimationFrame(33, 100, false, flip2, (AnimatedSprite.endOfAnimationBehavior) null, false)
                    });
                    this.currentBehavior = 0;
                    break;
            }
            this.startedBehavior = this.currentBehavior;
        }
    }
}
