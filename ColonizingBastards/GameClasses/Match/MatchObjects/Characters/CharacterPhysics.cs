 using System;
using System.Collections.Generic;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.MiscUtil;
using ColonizingBastards.Base.MiscUtil.Collidables;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.GameClasses.Config;
using ColonizingBastards.GameClasses.Match.Config;
 using ColonizingBastards.GameClasses.Match.MatchObjects.Ai;
 using ColonizingBastards.GameClasses.Match.MatchObjects.MatchShopKeeper;
 using ColonizingBastards.GameClasses.Match.MatchObjects.Projectiles;
using ColonizingBastards.GameClasses.Match.MatchObjects.Vegetation;
 using ColonizingBastards.GameClasses.Match.MatchSound;
 using ColonizingBastards.GameClasses.Match.ParticleSystem;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;
 using Microsoft.Xna.Framework.Input;
 using Ray = ColonizingBastards.Base.MiscUtil.Collidables.Ray;

namespace ColonizingBastards.GameClasses.Match.MatchObjects.Characters
{
    /// <summary>
    ///     This class simulates the physics behaviour of the character and checks for collision detection
    /// </summary>
    internal class CharacterPhysics
    {
        private readonly DefaultCharacter character;
        private readonly Scene scene;
	    private Vector3 position;
        private readonly Vector3 initPosition;
        private float accX, accY, decX, maxSpeedX;
	    public Vector3 speed;
		private Vector3 previousSpeed;
        private float jumpSpeed;
        private bool jumping;
        private bool jumpKeyDown;
        private bool facingRight = true;

        private float delta;
        public Rectangle hitboxOffset { get; protected set; }
        public Rectangle BoundingBox { get; protected set; }

        // Flags to specify what kind of collision has occurred
        private bool contactX = true, contactYbottom = true, contactYtop = true;
        private bool contactLeft = true;
        private bool contactRight = true;
        private bool contactFoliage;
        public bool IsInAir { get; protected set; }
        public bool IsStunned { get; set; }
        public bool IsLayingTrap { get; set; }
        private float stunTimeCounter;
        private CollidableResults rayCastResult;
        private CollidableResults insideClimbingArea;
        //private bool isGrounded;
        //List contains all foliages which are currently touched by user
        private readonly List<Actor> touchedActors;

        private bool isInClimbingArea;
        public bool IsClimbing { get; protected set; }

        private float climbableYMax;
        private float climableYCenter;

        private readonly int collisionTestIterations = 3;

        public bool IsTouchingShopkeeper { get; private set; }

        public List<Projectile> Projectiles { get; protected set; }

        public ParticleEffectManager FootstepEffectManager { get; }
        private ParticleEffectFactory footstepEffectFactory;

        private int playerIndex = -1;

        public float CurrentStunTime { get; set; }

        public Vector2 TrapPlacementPos { get; private set; }
        public bool TrapPlacementPossible { get; private set; }
        private CollidableResults trapPlacementCastResult;

        public CharacterPhysics(Character character, Vector3 characterPosition, Vector3 characterSize, Rectangle hitboxOffset, Scene scene)
        {
            this.character = (DefaultCharacter) character;
            initPosition = characterPosition;
            position = initPosition;
            this.hitboxOffset = hitboxOffset;
            BoundingBox = new Rectangle((int) (characterPosition.X + hitboxOffset.X),
                (int) (characterPosition.Y + hitboxOffset.Y), hitboxOffset.Width + 5, hitboxOffset.Height);

            this.scene = scene;

            touchedActors = new List<Actor>();

            Projectiles = new List<Projectile>(); 
            InitMovementFields();

            rayCastResult = new CollidableResults();
            insideClimbingArea = new CollidableResults();

            //Init Particle effects for footsteps
            footstepEffectFactory = new FootstepParticleEffectFactory(new Vector2(BoundingBox.Left, BoundingBox.Bottom), 20, 
                250, Vector2.Zero, Vector2.Zero, 1f,  0.7f, Color.White, Color.White, 250);
            FootstepEffectManager = new ParticleEffectManager(ParticleConfig.FOOTSTEP_TEXTURE,
                footstepEffectFactory, 1, 128, true, BlendState.NonPremultiplied) {Paused = true};
            scene.ParticleEffects.Add(FootstepEffectManager);

            Player currentPlayer = character.GetPosession() as Player;
            if (currentPlayer != null)
                playerIndex = (int)currentPlayer.playerIndex;
        }

        private void InitMovementFields()
        {
            accX = CharacterConfig.BASE_ACC_X;
            accY = CharacterConfig.BASE_ACC_Y;
            decX = CharacterConfig.BASE_DEC_X;

            maxSpeedX = CharacterConfig.MAX_SPEED_X;

            speed = Vector3.Zero;

            jumping = false;
            jumpKeyDown = false;
        }

        public void Update(GameTime gameTime, ActionSet actionSet)
        {
            delta = (float) gameTime.ElapsedGameTime.TotalSeconds;
            character.currentAnimRestartCounter -= (float) gameTime.ElapsedGameTime.TotalMilliseconds;

            if (IsFacingRight())
                TrapPlacementPos = new Vector2(BoundingBox.X + (BoundingBox.Width / 2) + 10, BoundingBox.Y + BoundingBox.Height + 20);
            else
                TrapPlacementPos = new Vector2(BoundingBox.X + (BoundingBox.Width / 2) - 65, BoundingBox.Y + BoundingBox.Height + 20);

            if ((!IsStunned && !IsLayingTrap) || (IsInAir && !IsClimbing))
            {
                CheckForCollisions();

                UpdatePlayerPosition(actionSet);
            }
            else
            {
                stunTimeCounter += delta;

                //Make player drop treasure if he carries one
                if (!IsLayingTrap && character.InvTreasure != null)
                {
                    if (IsClimbing)
                        character.LooseTreasure(new Vector2(position.X, climbableYMax));
                    else
                        character.LooseTreasure(new Vector2(position.X, BoundingBox.Bottom));
                }

                if (stunTimeCounter > CurrentStunTime)
                {
                    IsStunned = false;
                    IsLayingTrap = false;
                    stunTimeCounter = 0f;
                }

                /*if (playerIndex > -1 && !IsLayingTrap)
                {
                    if (stunTimeCounter > 0 && stunTimeCounter < CurrentStunTime/3)
                        GamePad.SetVibration(playerIndex, 1.0f, 1.0f);
                    else
                        GamePad.SetVibration(playerIndex, 0.0f, 0.0f);
                }*/

                if (facingRight)
                    ((Spritesheet) character.getRepresentation()).PlayAnimation("stunned");
                else
                    ((Spritesheet) character.getRepresentation()).PlayAnimation("stunned", 1F, true);
            }

            UpdateBullets(gameTime);

            if (scene.CurrWeatherSystem.IsRaining || contactFoliage)
                FootstepEffectManager.Paused = true;
        }

        private void CheckForCollisions()
        {
            contactX = true;
            contactYbottom = true;
            contactYtop = true;
            contactLeft = true;
            contactRight = true;
            contactFoliage = false;
            isInClimbingArea = false;
            insideClimbingArea = null;
            IsInAir = true;
            bool ray1Intersect = false, ray2Intersect = false;
            bool rayLeftIntersect = false, rayRightIntersect = false;
            TrapPlacementPossible = false;
            //Multiple iterations are used for collision detection, since one correction could cause another collision
            Collidable trapPlacementRay = new Ray(new Vector2(TrapPlacementPos.X, TrapPlacementPos.Y - 50), new Vector2(TrapPlacementPos.X, TrapPlacementPos.Y + 50));
            for (var iteration = 0;
                iteration < collisionTestIterations && (contactX || contactYbottom || contactYtop);
                iteration++)
            {
                // Calculate the amount of X and Y movement expected by the player this frame
                var nextMove = speed * delta;

                // Store the original final expected movement of the player so we can
                // see if it has been modified due to a collision or potential collision later
                var originalMove = nextMove;

                // No collisions found yet
                contactX = contactYbottom = contactYtop = contactLeft = contactRight = false;

                insideClimbingArea = new CollidableResults()
                {
                    IntersectLeft = false,
                    IntersectRight = false,
                    IntersectUp = false,
                    IntersectDown = false,
                };

                rayCastResult = new CollidableResults()
                {
                    Intersect = false
                };

                trapPlacementCastResult = new CollidableResults()
                {
                    Intersect = false
                };
                //check for potential collisions with polygons
                float tempX, tempY, tempWidth, tempHeight;
                tempX = (float)Math.Round(position.X + hitboxOffset.X - 1, MidpointRounding.AwayFromZero);
                tempY = (float)Math.Round(position.Y + hitboxOffset.Y - 1, MidpointRounding.AwayFromZero);
                tempWidth = (float)Math.Round(tempX + BoundingBox.Width + 1 , MidpointRounding.AwayFromZero);
                tempHeight = (float)Math.Round(tempY + BoundingBox.Height + 1, MidpointRounding.AwayFromZero);

                //Calculate the bounding polygon of the player at the current position
                var playerNextBounds = new Polygon(new List<Vector2>
                    {
                        new Vector2(tempX, tempY),
                        new Vector2(tempWidth, tempY),
                        new Vector2(tempWidth, tempHeight),
                        new Vector2(tempX, tempHeight)
                    }, false, false);

                var rayHeight = (tempHeight + tempY) / 2f;

                Collidable ray1 = new Ray(new Vector2(tempX, tempHeight - 10),
                           new Vector2(tempX, tempHeight + 10));
                Collidable ray2 = new Ray(new Vector2(tempWidth, tempHeight - 10),
                    new Vector2(tempWidth, tempHeight + 10));

                Collidable rayLeft = new Ray(new Vector2(tempX + 10, rayHeight),
                           new Vector2(tempX - 10, rayHeight));
                Collidable rayRight = new Ray(new Vector2(tempWidth - 10, rayHeight),
                           new Vector2(tempWidth + 10, rayHeight));

                climableYCenter = -1f;

                for (var i = 0; i < scene.GetCollidables().Count; i++)
                {

                    var currentCollidable = scene.GetCollidables()[i];

                    if (!currentCollidable.IsClimbable)
                    {
                        //----------------- Ray - Slope detection ---------------------------//
                        //The following code section shoots rays from the bottom of the player's bounding box to the ground.
                        //The results of the rays are used to check, whether the player is standing on a slope. If yes, the ..
                        //.. movement of the players is adjusted according to the steepness of the slope.
                        //!contactX && !contactYbottom && !contactYtop

                        if (!rayCastResult.Intersect)
                        {
                            rayCastResult = RayCast.DetectSlope(ray1, ray2, currentCollidable);
                        }
                        if (!ray1Intersect)
                            ray1Intersect = RayCast.CheckIfInsideClimbingArea(ray1, currentCollidable);
                        if (!ray2Intersect)
                            ray2Intersect = RayCast.CheckIfInsideClimbingArea(ray2, currentCollidable);
                        if (ray1Intersect || ray2Intersect)
                            IsInAir = false;

                        //Check if player collides left or right using rays
                        if (!rayLeftIntersect)
                            rayLeftIntersect = RayCast.CheckIfInsideClimbingArea(rayLeft, currentCollidable);
                        if (!rayRightIntersect)
                            rayRightIntersect = RayCast.CheckIfInsideClimbingArea(rayRight, currentCollidable);


                        //----------------- Ray - Slope detection -END---------------------------//
                    }
                    //------------------- Trap Placement Check -------------------------------//

                    if (!trapPlacementCastResult.Intersect)
                    {
                        trapPlacementCastResult.Intersect = RayCast.CheckIfInsideClimbingArea(trapPlacementRay, currentCollidable);
                    }

                    //------------------- Trap Placement Check  - END -------------------------------//

                    var polygonCollisionResult = currentCollidable.CheckForCollision(playerNextBounds,
                        speed * delta);

                    
                    //Only adjust speed of the player if the polygon can't be climbed up or down( => IsClimbable)
                    if (polygonCollisionResult.WillIntersect && !currentCollidable.IsClimbable)
                    {
                        nextMove = polygonCollisionResult.MinimumTranslationVector + speed * delta;
                      
                    }
                    else if ((polygonCollisionResult.WillIntersect || polygonCollisionResult.Intersect) &&
                             currentCollidable.IsClimbable)
                    {
                        isInClimbingArea = true;
                       
                        bool atTop = false;

                        //--------- Ray- Climbing area detection ----------- //
                        //The following code section shoots ray left and right out of the players bounding box, in order
                        //to verify, whether the player is still inside the bounds of the climbing area and limit his movement otherwise
                        if (IsClimbing)
                        {
                            //check if player is still inside the climbable area
                            rayHeight = (tempHeight + tempY) / 2f;
                            var rayWidth = (tempWidth + tempX) / 2f;
                            var deltaHeight = (tempHeight - tempY)/2f;
                            float collidableXMin = currentCollidable.MinWidth;
                            float collidableXMax = currentCollidable.MaxWidth;
                            float collidableYMax = currentCollidable.MaxHeight;
                            float collidableYMin = currentCollidable.ClimbGroupMinY;

                         
                            if (rayHeight - currentCollidable.CenterYClimbGroup < 0)
                                atTop = true;
                            

                            if (tempX >= collidableXMin)
                            {
                                Collidable leftRay = new Ray(new Vector2(collidableXMin - 10, rayHeight),
                                    new Vector2(tempX + 10, rayHeight));
                                if (RayCast.CheckIfInsideClimbingArea(leftRay, currentCollidable))
                                    insideClimbingArea.IntersectLeft = true;
                            }

                            if (tempWidth <= collidableXMax)
                            {
                                Collidable rightRay = new Ray(new Vector2(tempWidth - 10, rayHeight),
                                    new Vector2(collidableXMax + 10, rayHeight));
                                if (RayCast.CheckIfInsideClimbingArea(rightRay, currentCollidable))
                                    insideClimbingArea.IntersectRight = true;

                            }

                            if (tempHeight <= collidableYMax)
                            {
                                Collidable downRayLeft = new Ray(new Vector2(tempX, tempHeight + 20),
                                    new Vector2(tempX, collidableYMax + 30));
                                Collidable downRayRight = new Ray(new Vector2(tempWidth, tempHeight + 20),
                                    new Vector2(tempWidth, collidableYMax + 30));
                                Collidable downRayMiddle = new Ray(new Vector2(rayWidth, tempHeight - 2),
                                    new Vector2(rayWidth, collidableYMax + 10));

                                if (atTop) 
                                {
                                    if (RayCast.CheckIfInsideClimbingArea(downRayMiddle, currentCollidable))
                                        insideClimbingArea.IntersectDown = true;
                                }
                                else
                                {
                                    if (RayCast.CheckIfInsideClimbingArea(downRayLeft, currentCollidable) && RayCast.CheckIfInsideClimbingArea(downRayRight, currentCollidable))
                                        insideClimbingArea.IntersectDown = true;
                                }
                            }

                            if (atTop)
                            {
                                insideClimbingArea.IntersectUp = !(tempY + deltaHeight < collidableYMin);
                            }
                            else
                                insideClimbingArea.IntersectUp = true;

                            climbableYMax = currentCollidable.ClimbGroupMaxY;
                            climableYCenter = currentCollidable.CenterYClimbGroup;
                        }

                        //--------- Ray- Climbing area detection -END----------- //
                    }
                   

                    // Detect what type of contact has occurred based on a comparison of
                    // the original expected movement vector and the new one

                    if (nextMove.Y > originalMove.Y)
                        contactYtop = true;

                    if (nextMove.Y < originalMove.Y)
                        contactYbottom = true;

                    if (nextMove.X < originalMove.X)
                        contactRight = true;

                    if (nextMove.X > originalMove.X)
                        contactLeft = true;

                    contactX = contactLeft || contactRight;

                    if (contactX && contactYtop && speed.Y < 0)
                        speed.Y = nextMove.Y = 0;

                    if (contactX && contactYbottom && speed.Y > 0)
                        speed.Y = nextMove.Y = 0;

                   
                }

                if (!isInClimbingArea)
                    IsClimbing = false;

                if (IsClimbing)
                {
                    nextMove = originalMove;
                    var anyContact = contactYbottom || contactYtop || contactX;
                    if (anyContact && Math.Abs(originalMove.X) > 0.001)
                    {
                        if (!insideClimbingArea.IntersectLeft && originalMove.X < 0 && contactLeft)
                        {
                            nextMove.X = 0;
                            speed.X = 0;
                        }
                        if (!insideClimbingArea.IntersectRight && originalMove.X > 0 && contactRight)
                        {
                            nextMove.X = 0;
                            speed.X = 0;
                        }
                    }

                    if (anyContact && originalMove.Y > 0.001)
                    {
                        if ((insideClimbingArea.IntersectLeft || insideClimbingArea.IntersectRight) &&
                            !insideClimbingArea.IntersectDown)
                        {
                            nextMove.Y = 0;
                            speed.Y = 0;
                            IsClimbing = false;
                        }
                    }

                    if (originalMove.Y < 0f && !insideClimbingArea.IntersectUp)
                    {
                        nextMove.Y -= CharacterConfig.JUMP_START_SPEED_Y / 10f;
                        IsClimbing = false;
                    }

                }
                // If a contact has been detected, apply the re-calculated movement vector
                // and disable any further movement this frame (in either X or Y as appropriate)
                if (contactYtop || contactYbottom)
                {
                   
                    position.Y += nextMove.Y;
                
                    speed.Y = 0;

                    if (contactYbottom)
                    {
                        jumping = false;
                        IsInAir = false;
                    }
                }


                if (contactX)
                {
                   
                    if (rayCastResult.Intersect && !contactYtop && !(rayLeftIntersect && originalMove.X <0) && !(rayRightIntersect && originalMove.X > 0))
                    {
                        position.X += nextMove.X * 0.5f * Math.Abs(rayCastResult.CollisionNormal.X);

                        if (Math.Abs(nextMove.X) > 0.0001f)
                        {
                            float newSpeed = speed.X * (1- 0.3f * Math.Abs(rayCastResult.CollisionNormal.X));
                            speed.X = 0.6f * speed.X + 0.4f * newSpeed;
                        }
                        else
                            speed.X = 0; 
                    }
                    else
                    {
                        if(!(rayLeftIntersect && nextMove.X < 0) && !(rayRightIntersect && nextMove.X > 0))
                            position.X += nextMove.X;

                        speed.X = 0;
                    }
                                                                                                        
                }else if (insideClimbingArea != null)
                {
                    position.X += nextMove.X;

                }

                if (jumping && IsClimbing)
                {
                    jumping = false;
                    speed.X = 0;
                }

                if (((contactX || contactYtop) && (IsInAir) && !IsClimbing))
                {
                    speed.X = nextMove.X = 0;
                    speed.Y += 3* accY * delta;
                    nextMove.Y = speed.Y * delta;
                }

                if (IsInAir && !jumping)
                {
                    speed.X *= 0.9f;
                }

                if (trapPlacementCastResult.Intersect && !(rayLeftIntersect && !IsFacingRight()) && !(rayRightIntersect && IsFacingRight()))
                    TrapPlacementPossible = true;
            }

            //Check if the player intersects any of the other actors (like undergrowth, treasure, base etc.)
            IsTouchingShopkeeper = false;
            contactFoliage = false;
            touchedActors.Clear();

            foreach (var actor in scene.GetActors())
            {
                Rectangle boundingBoxActor;
                if (actor.GetType() == typeof(ShopKeeper))
                {
                    ShopKeeper shopKeeper = actor as ShopKeeper;
                    boundingBoxActor = shopKeeper.BoundingBox;
                    if (!BoundingBox.Intersects(boundingBoxActor))
                        continue;
                    IsTouchingShopkeeper = true;
                    if(character.InvTreasure != null)
                        shopKeeper.ContactPlayer = true;
                     
                }
                else
                {

                    boundingBoxActor = new Rectangle((int) actor.GetPosition().X,
                        (int) (actor.GetPosition().Y - actor.GetSize().Y),
                        (int) actor.GetSize().X, (int) actor.GetSize().Y);

                    if (!BoundingBox.Intersects(boundingBoxActor))
                        continue;

                    if (actor.GetType() == typeof(Projectile) && character.GetPosession().GetType() != typeof(AiIndigenController))
                    {
                        scene.MatchSoundManager.PlaySoundEffect(SoundEffectEnumeration.Hit);
                        ((Projectile) actor).DestroyProjectile();
                        IsStunned = true;
                        CurrentStunTime = ProjectileConfig.BULLET_STUN_TIME;
                    }
                }

                touchedActors.Add(actor);

            }

            foreach (Foliage foliage in scene.GetFoliage())
            {
                Rectangle boundingBoxActor = new Rectangle((int)foliage.GetPosition().X,
                        (int)(foliage.GetPosition().Y - foliage.GetSize().Y),
                        (int)foliage.GetSize().X, (int)foliage.GetSize().Y);

                if (!BoundingBox.Intersects(boundingBoxActor))
                    continue;

                if (foliage.Active)
                    contactFoliage = true;
                touchedActors.Add(foliage);
            }
        } 

        private void UpdatePlayerPosition(ActionSet actionSet)
        {
            position += speed * delta;

            character.SetPosition(position);
            BoundingBox = new Rectangle((int) (position.X + hitboxOffset.X), (int) (position.Y + hitboxOffset.Y),
                BoundingBox.Width, BoundingBox.Height);
            var moveRequest = false;
            FootstepEffectManager.Paused = true;

            //lower character's speed, if he is carrying treasure or walking on undergrowth
            if (contactFoliage && character.InvTreasure == null)
            {
                accX = CharacterConfig.FOLIAGE_ACC_X;
                maxSpeedX = CharacterConfig.MAX_SPEED_X_FOLIAGE;
                jumpSpeed = CharacterConfig.FOLIAGE_JUMP_SPEED_Y;
            }
            else if (contactFoliage && character.InvTreasure != null)
            {
                accX = CharacterConfig.FOLIAGE_AND_CARRY_ACC_X;
                maxSpeedX = CharacterConfig.MAX_SPEED_X_FOLIAGE;
                jumpSpeed = CharacterConfig.FOLIAGE_AND_CARRY_JUMP_SPEED_Y;
            }
            else if (!contactFoliage && character.InvTreasure != null)
            {
                accX = CharacterConfig.CARRY_ACC_X;
                maxSpeedX = CharacterConfig.MAX_SPEED_X_CARRY;
                jumpSpeed = CharacterConfig.CARRY_JUMP_SPEED_Y;
            }
            else
            {
                accX = CharacterConfig.BASE_ACC_X;
                maxSpeedX = CharacterConfig.MAX_SPEED_X;
                jumpSpeed = CharacterConfig.JUMP_START_SPEED_Y;
            } 

            //Handle axisinputs
            foreach (var t in actionSet.axisActions)
            {
                var movement = t.Item2;

                if (t.Item1 == InputConfig.Actions.MOVE_DIRECTION)
                {
                    
                    if (movement.X < -CharacterConfig.INPUT_X_SENS)
                    {
                        //move left                      
                        moveRequest = true;
                        facingRight = false;
                        if (IsClimbing)
                        {
                            speed.X = -CharacterConfig.CLIMB_SPEED_X;
                            ((Spritesheet)character.getRepresentation()).PlayAnimation("climb", 1F, true);
                        }
                        else
                        {
                            FootstepEffectManager.Paused = false;
                            speed.X = -maxSpeedX;//accX * delta;
                            ((Spritesheet) character.getRepresentation()).PlayAnimation("run", 1F, true);
                            CreateFootstepParticles();
                        }

                    }
                    if (movement.X > CharacterConfig.INPUT_X_SENS)
                    {
                        //move right
                        moveRequest = true;
                        facingRight = true;
                        if (IsClimbing)
                        {
                            speed.X = CharacterConfig.CLIMB_SPEED_X;
                            ((Spritesheet)character.getRepresentation()).PlayAnimation("climb");
                        }
                        else
                        {
                            FootstepEffectManager.Paused = false;
                            speed.X = maxSpeedX; //accX * delta;
                            ((Spritesheet) character.getRepresentation()).PlayAnimation("run");
                            CreateFootstepParticles();

                        }

                    }

                    if (movement.Y < -CharacterConfig.INPUT_Y_SENS && isInClimbingArea)
                    {
                        IsClimbing = true;
                        //climb down
                        speed.Y = 2f* CharacterConfig.CLIMB_SPEED_Y;
                        moveRequest = true;
                        if (facingRight)                
                            ((Spritesheet) character.getRepresentation()).PlayAnimation("climb");                     
                        else                       
                            ((Spritesheet) character.getRepresentation()).PlayAnimation("climb", 1F, true);
                        
                    }

                    if (movement.Y > CharacterConfig.INPUT_X_SENS && isInClimbingArea)
                    {
                        IsClimbing = true;
                        //climb up
                        speed.Y = -CharacterConfig.CLIMB_SPEED_Y;                       
                        moveRequest = true;
                        if (facingRight)
                            ((Spritesheet) character.getRepresentation()).PlayAnimation("climb");                        
                        else
                            ((Spritesheet) character.getRepresentation()).PlayAnimation("climb", 1F, true);
                        
                    }
                }
            }

            //Handle actions

            if (actionSet.actions.Contains(InputConfig.Actions.JUMP) && !IsClimbing && !jumpKeyDown && !IsInAir)
            {
                //jump
                moveRequest = true;
                jumping = true;
                jumpKeyDown = true;
                speed.Y = -jumpSpeed;
                if (facingRight)
                    ((Spritesheet) character.getRepresentation()).PlayAnimation("jump");
                else
                    ((Spritesheet) character.getRepresentation()).PlayAnimation("jump", 1F, true);
                actionSet.actions.Remove(InputConfig.Actions.JUMP);
            }

            //Allow player to fall while climbing , if the JUMP Button is pressed
            if (actionSet.actions.Contains(InputConfig.Actions.JUMP) && IsClimbing)
            {
                if (climableYCenter > 0f && BoundingBox.Bottom < climableYCenter - 20)
                    position.Y += 100;
                jumping = false;
                IsClimbing = false;
                moveRequest = true;
            }

            if (IsInAir && !IsClimbing && speed.Y > 0f) //&& speed.Y > 0f && jumping
            {
                moveRequest = true;
                if (facingRight)
                    ((Spritesheet) character.getRepresentation()).PlayAnimation("fall");
                else
                    ((Spritesheet) character.getRepresentation()).PlayAnimation("fall", 1F, true);
            }

            if (IsInAir)
            {
                FootstepEffectManager.Paused = true;
            }

            if (actionSet.actions.Count == 0)
                jumpKeyDown = false;

            actionSet.axisActions.Clear();

            // Apply the force of gravity
            if (!IsClimbing)
                speed.Y += accY * delta;

            // Limit the sideways acceleration of the player
            if (speed.X > maxSpeedX) speed.X = maxSpeedX;
            if (speed.X < -maxSpeedX)
                speed.X = -maxSpeedX;
            // Limit the force of gravity (terminal velocity) if (speedY > maxSpeedY) speedY = maxSpeedY;
            if (speed.Y < -CharacterConfig.MAX_SPEED_Y) speed.Y = -CharacterConfig.MAX_SPEED_Y;

            // Decelerate the player's sideways movement if left or right wasn't pressed
            if (!moveRequest)
            {
                if (!IsClimbing)
                {
                    if (character.currentAnimRestartCounter <= 0f)
                    {
                        if (facingRight)
                            ((Spritesheet) character.getRepresentation()).PlayAnimation("idle");
                        else
                            ((Spritesheet) character.getRepresentation()).PlayAnimation("idle", 1F, true);
                    }
                }
                else
                {
                    speed.Y = 0;
                    if (character.currentAnimRestartCounter <= 0f)
                    {
                        if (facingRight)
                            ((Spritesheet) character.getRepresentation()).PlayAnimation("climbidle");
                        else
                            ((Spritesheet) character.getRepresentation()).PlayAnimation("climbidle", 1F, true);
                    }
                }

                

                if (speed.X < 0) speed.X += decX * delta;
                if (speed.X > 0) speed.X -= decX * delta;

                // Deceleration may produce a speed that is greater than zero but
                // smaller than the smallest unit of deceleration. These lines ensure
                // that the player does not keep travelling at slow speed forever after
                // decelerating.
                if (speed.X > 0 && speed.X < decX) speed.X = 0;
                if (speed.X < 0 && speed.X > -decX) speed.X = 0;

                FootstepEffectManager.Paused = true;
            }
        }

        private void UpdateBullets(GameTime gameTime)
        {
            //Update and remove non active bullets
            for (var i = Projectiles.Count - 1; i > -1; i--)
                if (Projectiles[i].IsActive)
                {
                    Projectiles[i].Update(gameTime);
                }
                else
                {
                    scene.RemoveActor(Projectiles[i]);
                    Projectiles.RemoveAt(i);
                }
        }

        public void AddBullet(Projectile projectile)
        {
            Projectiles.Add(projectile);
            scene.RegisterObject(projectile);
        }
         
        public List<Actor> GetTouchedActors()
        {
            return touchedActors;
        }

        public bool IsFacingRight()
        {
            return facingRight;
        }

        private void CreateFootstepParticles()
        {
            if(facingRight)
                footstepEffectFactory.EmitterLocation = new Vector2(BoundingBox.Left, BoundingBox.Bottom);
            else
                footstepEffectFactory.EmitterLocation = new Vector2(BoundingBox.Right, BoundingBox.Bottom);
            
        }

        // Computes, on which platform the character stands (if any)
        public Collidable QueryCurrentPlatform()
        {

            if (IsInAir)
                return null;

            // Adjust result depending on climbing direction (TODO)
            /*if (IsClimbing)
            {
                return null;
            }*/

            //check for potential collisions with polygons
            float tempX, tempY, tempWidth, tempHeight;
            tempX = (float)Math.Round(position.X + hitboxOffset.X - 1, MidpointRounding.AwayFromZero);
            tempY = (float)Math.Round(position.Y + hitboxOffset.Y - 1, MidpointRounding.AwayFromZero);
            tempWidth = BoundingBox.Width;
            tempHeight = BoundingBox.Height;

			// Point below character
			Vector2 pos = new Vector2(tempX + 0.5f * tempWidth, tempY);
			Vector2 p = new Vector2(pos.X, tempY + 2.2f * tempHeight);

			// Check for Collidable at this point
			foreach (Collidable c in scene.WalkableCollidables)
            {
				// Return the first collision (assume only one at every point) - maybe extend to multiple results
				//if (c.IsWithinCollidable(p))
				if (c.IntersectsWithPolygon(pos, p - pos))
					return c;
            }

            // No collision
            return null;
        }

	    public Vector3 GetPosition()
	    {
		    return position;
	    }

	    public Vector3 GetSpeed()
	    {
		    return speed;
	    }
    } 
}
