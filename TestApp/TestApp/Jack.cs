using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YutShot
{
    class Jack
    {
        //Constants
        float drawScaleSmall = 4.5f;
        float drawScaleBig = 6;
        float drawScaleInc = 0.15f;
        int tickPerFrame = 18;
        float xSpeed = 5f;
        float ySpeed = 4f;
        int numberOfFrames = 8;

        static Texture2D spriteSheet;
		Rectangle srcRect = new Rectangle (0, 0, 30, 30);
        Vector2 origin = new Vector2(15f, 30f);
        Animation currentAnim;
		Vector2 position = new Vector2(GameVars.RENDERWIDTH / 2, GameVars.RENDERHEIGHT + 150);
        float drawScale = 5;
        Action callback = null;
        bool enlarging, shrinking;
        float targetx;
        float targety;
        public int currentFrame = 0;
        int tickSinceLastFrame = 0;
        Action walkTowardsCallBack = null;
        public Jack()
        {
            Start();
            WalkTowards(GameVars.RENDERWIDTH / 2 - 67.5f);
        }
        static public void LoadContent(GraphicsDevice device)
        {
            using (var stream = TitleContainer.OpenStream("Content/sprites/jack.png"))
            {
                spriteSheet = Texture2D.FromStream(device, stream);
            }
        }
        public enum Animation
        {
            Stop,
            WalkLeft,
            Eat,
            Die,
            WalkRight,
        }
        public void animate(Animation _anim, Action _callback = null)
        {
            
            if (currentAnim != _anim)
            {
                currentAnim = _anim;
                if (_anim == Animation.Eat)
                    currentFrame = 0;
                callback = _callback;
            }
            tickPerFrame = currentAnim == Animation.Eat ? 10 : 18;
        }
        public void WalkTowards(float x, Action cb = null)
        {
            targetx = x + 93;
            targetx = Math.Min(targetx, GameVars.RENDERWIDTH - 50);
            if (position.X > targetx)
            {
                animate(Animation.WalkLeft);
            }
            else
            {
                animate(Animation.WalkRight);
            }
            walkTowardsCallBack = cb;
        }
        public void Result()
        {
            animate(Animation.Die);
            targety = 1180;
            enlarging = true;
        }
        public void Start()
        {
            animate(Animation.Stop);
            targety = GameVars.RENDERHEIGHT - 10;
            shrinking = true;
        }

        public void Update(GameTime gameTime)
        {
            setSpriteRect();
            tickSinceLastFrame++;
            if (callback != null)
            {
                if (currentFrame == numberOfFrames - 1)
                {
                    callback();
                    callback = null;
                }
            }
            if (tickSinceLastFrame > tickPerFrame)
            {
                tickSinceLastFrame = 0;
                currentFrame = (currentFrame + 1) % numberOfFrames;
            }

            if (currentFrame > numberOfFrames - 1)
            {
                currentFrame = 0;
            }
            if (Math.Abs(position.X - targetx) > xSpeed * 1.5f)
            {
                if (position.X > targetx)
                {
                    position.X -= xSpeed;
                }
                else
                {
                    position.X += xSpeed;
                }
            }
            else
            {
                if (currentAnim == Animation.WalkLeft || currentAnim == Animation.WalkRight)
                {
                    position.X = targetx;
					if (walkTowardsCallBack != null)walkTowardsCallBack();
                }
            }
            if (Math.Abs(position.Y - targety) > ySpeed * 1.5f)
            {
                if (position.Y > targety)
                {
                    position.Y -= ySpeed;
                    if (position.Y < targety)
                    {
                        position.Y = targety;
                    }
                }
                else
                {
                    position.Y += ySpeed;
                    if (position.Y > targety)
                    {
                        position.Y = targety;
                    }
                }
            }
            else
            {
                position.Y = targety;
                
            }
            if (enlarging)
            {
                drawScale += drawScaleInc;
                if (drawScale > drawScaleBig)
                {
                    enlarging = false;
                    drawScale = drawScaleBig;
                }
            }
            if (shrinking)
            {
                drawScale -= drawScaleInc;
                if (drawScale < drawScaleSmall)
                {
                    shrinking = false;
                    drawScale = drawScaleSmall;
                }
            }
        }
        void setSpriteRect()
        {
            int frame = (byte)currentAnim;
            if (currentAnim == Animation.WalkRight) frame = 1;
            frame = frame * numberOfFrames + currentFrame;
            srcRect.X = frame % 6 * 30;
            srcRect.Y = frame / 6 * 30;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, position, srcRect, Color.White, 0, origin, drawScale, currentAnim == Animation.WalkRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
    }
}
