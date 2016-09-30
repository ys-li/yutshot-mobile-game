using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YutShot
{
    class Backdrop
    {
        /// <summary>
        /// Starts from the upper left corner, screen space
        /// </summary>
        float position = 0;
        bool sliding = false;
        float slideFullDuration = 0;
        float slideCurrentDuration = 0;
        float slideSinAmplitude;
        /// <summary>
        /// Starts with 1
        /// </summary>
        int currentLevel = 0;
        Action callback; 
        /// <summary>
        /// texel space
        /// </summary>
        int heightPerLevel;
        Rectangle destRect;
        Rectangle sourceRect;
        List<BackdropEntity> entities = new List<BackdropEntity>();
        public Backdrop()
        {
            heightPerLevel = Resources.backdropTexture.Height / GameVars.maxLevel;
            destRect = new Rectangle(0, 0, GameVars.RENDERWIDTH, GameVars.RENDERHEIGHT);
            sourceRect = new Rectangle(0, (Resources.backdropTexture.Height - heightPerLevel), Resources.backdropTexture.Width, heightPerLevel);
            Random rand = new Random(DateTime.Now.Millisecond);
            int temp = rand.Next(4) + 2;
            for (int i = 0; i < temp; i++)
            {
                entities.Add(new BackdropEntity((byte)rand.Next(5), 0.5f, 0, 0, new Vector2((float)(rand.NextDouble() * GameVars.RENDERWIDTH), (float)(rand.NextDouble() * GameVars.RENDERHEIGHT)), true, false));
            }
        }
        public void startSlideTo(int _level, float duration, Action _callback)
        {
            
            if (_level == currentLevel)
            {
                finishSliding();
                return;
            }
            
            slideSinAmplitude = (_level - currentLevel) * GameVars.RENDERHEIGHT / 2;
            
            sliding = true;
            slideFullDuration = duration;
            callback = _callback;
            
            //setting destRects and sourceRects
            destRect.Height = GameVars.RENDERHEIGHT * (Math.Abs(currentLevel - _level) + 1);
            sourceRect.Height = (heightPerLevel * (Math.Abs(currentLevel - _level) + 1));
            destRect.Y = Math.Min(0,(currentLevel - _level) * GameVars.RENDERHEIGHT);
            position = Math.Min(0, (currentLevel - _level) * GameVars.RENDERHEIGHT);
            sourceRect.Y = (Resources.backdropTexture.Height - heightPerLevel * (Math.Max(_level,currentLevel) + 1));
            
            
            Random rand = new Random(DateTime.Now.Millisecond);
            int temp = rand.Next(4) + 2;
            Vector2 tempentposition;
            if (_level > currentLevel) //slide up
            tempentposition = new Vector2(0, -(_level - currentLevel) * GameVars.RENDERHEIGHT);
            else //slide down (0)
            tempentposition = new Vector2(0, GameVars.RENDERHEIGHT);
            switch (_level)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    for (int i = 0; i < temp; i++)
                    {
                        entities.Add(new BackdropEntity((byte)rand.Next(5), 0.5f, _level, currentLevel, new Vector2((float)(rand.NextDouble() * GameVars.RENDERWIDTH), (float)(rand.NextDouble() * GameVars.RENDERHEIGHT)) + tempentposition, true, false));
                    }
                    break;
                case 9:
                    for (int i = 0; i < temp; i++)
                    {
                        entities.Add(new BackdropEntity((byte)rand.Next(5), 0.5f, _level, currentLevel, new Vector2((float)(rand.NextDouble() * GameVars.RENDERWIDTH), (float)(rand.NextDouble() * GameVars.RENDERHEIGHT)) + tempentposition, true, true));
                    }
                    break;
                default:
                    break;

            }
            currentLevel = _level;
        }
        public void restart()
        {
            startSlideTo(0, 0.7f, delegate {
                for (int i = 0; i < entities.Count; i++)
                {
                    if(entities[i].level != 0)
                    {
                        entities[i].disposable = true;
                    }
                }
            });
        }
        public void finishSliding()
        {
			if (callback != null)callback();
            destRect = new Rectangle(0, 0, GameVars.RENDERWIDTH, GameVars.RENDERHEIGHT);
            sourceRect = new Rectangle(0, (int)(Resources.backdropTexture.Height - heightPerLevel * (currentLevel + 1)), Resources.backdropTexture.Width, heightPerLevel);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Resources.backdropTexture, destRect, sourceRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            foreach (BackdropEntity e in entities)
            {
                e.Draw(spriteBatch);
            }
        }
        public void Update(GameTime gameTime)
        {
           
            for (int i = entities.Count - 1; i > -1;i--)
            {
                entities[i].Update(gameTime, sliding);
                if (entities[i].disposable) entities.RemoveAt(i);
            }
            //slide mechanism
            if (sliding)
            {
                float velocity = (float)Math.Cos(slideCurrentDuration / slideFullDuration * Math.PI);
                slideCurrentDuration += (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity = (float)(slideSinAmplitude * (velocity - Math.Cos(slideCurrentDuration / slideFullDuration * Math.PI)));
                position += velocity;
                destRect.Y = (int)position;
                foreach (BackdropEntity e in entities)
                {
                    e.moveDownBy((int)velocity);
                }
                if (slideCurrentDuration > slideFullDuration)
                {
                    sliding = false;
                    slideCurrentDuration = 0;
                    finishSliding();
                }
            }
                
                
        }
        
    }

    class BackdropEntity
    {
        /// <summary>
        /// 0: clouds, 1: birds, 2: sun, 3: stars, 
        /// </summary>
        byte typeID;
        public bool disposable = false;
        public int level = 0;
        static float maxScale = 5f;
        static float minScale = 2f;
        float scale = 1;
        /// <summary>
        /// In world space
        /// </summary>
        public Vector2 position;
        static float moveSpeed = 2f;
        static float rotationSpeed = 0.1f;
        /// <summary>
        /// From 0 to 1.
        /// </summary>
        float parallaxAmount = 0;
        Rectangle srcRect;
        bool moving;
        bool moveRight;
        bool rotating;
        float rotation;
        private void setSrcRect()
        {
            int tempnumber = level * GameVars.backdropEntityTextureNumbers + typeID;
            srcRect = new Rectangle((tempnumber % 8) * 30, (tempnumber / 8) * 30,30,30);
        }
        public BackdropEntity(byte _typeid, float _parallax, int _targetRound, int currentLevel, Vector2 _pos, bool _moving = false, bool _rotating = false, bool randparallax = true)
        {
            moving = _moving;
            Random rand = new Random(DateTime.Now.Millisecond);
            moveSpeed = (float)rand.NextDouble() * moveSpeed + 0.2f;
            moveRight = (rand.Next(2) == 1) ? true :false;
            typeID = _typeid;
            level = _targetRound;
            setSrcRect();
            rotating = _rotating;
            position = _pos;
            if (randparallax)
            {
                parallaxAmount = (float)(rand.NextDouble() / 3);
            }
            else
            {
                parallaxAmount = _parallax;
            }
            scale = (float)(rand.NextDouble() * (maxScale - minScale) + minScale);
        }
        public void moveDownBy(int amt)
        {
            position.Y += amt * (1 - parallaxAmount);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Resources.backdropEntityTexture, position, srcRect, Color.White, rotation, new Vector2(15, 15), scale, moveRight? SpriteEffects.FlipHorizontally:SpriteEffects.None, 0.1f);
        }

        public void Update(GameTime gameTime, bool sliding)
        {
            if (rotating) rotation += rotationSpeed;
            if (!sliding && position.Y > GameVars.RENDERHEIGHT) disposable = true;
            if (moving)
            {
                position.X += (moveRight ? moveSpeed : -moveSpeed);
            }
            if (position.X < -200)
            {
                position.X = GameVars.RENDERWIDTH + 200;
            }
            else if (position.X > GameVars.RENDERWIDTH + 200)
            {
                position.X = -200;
            }
        }
    }

    
}
