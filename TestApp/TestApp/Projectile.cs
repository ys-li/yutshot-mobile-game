using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YutShot
{
    class Projectile
    {
        byte ID;
        public float scale;
        protected float opacity = 1;
        public Vector2 position;
        public Vector2 velocity;
        protected Vector2 origin = Vector2.Zero;
        Vector2 targetCoords;
        int hitTexCoordsX, hitTexCoordsY;
        float arrivalTime;
        public float rotation;
		int width, height;
        float liveTime = 0;
        float lagTime;
        bool[,] fillMap; // true = transparent
        bool onPhysics = true;
        public bool disposable = false;
        float fadeDuration;
        Action fadeAction;
        bool fading = false;
        
        
        public void SetRotation(float rot)
        {
            rotation = rot;
        }
        public void SetScale(float _scale)
        {
            scale = _scale;
        }
        public void SetVelocity(Vector2 vel)
        {
            velocity = vel;
        }
        public void StartFade(float _duration, Action _afterFadeAction)
        {
            fading = true;
            fadeDuration = _duration;
            fadeAction = _afterFadeAction;
        }
        public void SetInitialPosition()
        {
            if (MainGame.rand.NextDouble() >= 0.5) position.X = -width * scale; else position.X = GameVars.RENDERWIDTH;
            position.Y = MainGame.rand.Next((int)(GameVars.RENDERHEIGHT * 0.2), (int)(GameVars.RENDERHEIGHT * 0.9));
        }
        public void SetPosition(Vector2 pos)
        {
            position = pos;
        }
        public Projectile(byte _ID, float _lagtime, Vector2 _target, float _arrivaltime)
        {
            ID = _ID;
            lagTime = _lagtime;
            targetCoords = _target;
            arrivalTime = _arrivaltime;
            SetScale(12.5f);
            width = 15;
            height = 15;
        }
        public virtual bool isHit(Vector2 coords)
        {
            Vector2 texspace = coords - position;
            texspace = Vector2.Divide(texspace,scale);
            if (texspace.X < 0 || texspace.Y < 0 || texspace.X > width || texspace.Y > height) return false;
            texspace.X = (int)texspace.X;
            texspace.Y = (int)texspace.Y;
            return fillMap[(int)texspace.X, (int)texspace.Y];
        }
        public void togglePhysics(bool value)
        {
            onPhysics = value;
        }
        public virtual void Initialize()
        {
           
            //Mapping alpha points
            fillMap = new bool[width, height];
            Color[] tempcolor = new Color[width * height];
            Resources.projectileTextures[ID].GetData(tempcolor);
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (tempcolor[x + y * width].A != 0)
                    {
                        fillMap[x, y] = true;
                    }
                }
            }
            
            //Randomize Hit Position
            Random rand = new Random(DateTime.Now.Millisecond);

            do
            {
                hitTexCoordsX = rand.Next(1, width);
                hitTexCoordsY = rand.Next(1, height);
            } while (!(fillMap[hitTexCoordsX, hitTexCoordsY] && fillMap[hitTexCoordsX - 1, hitTexCoordsY] && fillMap[hitTexCoordsX + 1, hitTexCoordsY] && fillMap[hitTexCoordsX, hitTexCoordsY - 1] && fillMap[hitTexCoordsX, hitTexCoordsY + 1]));

            
            //Calculating Init Velocity
            float travelTime = arrivalTime - lagTime;
            velocity.Y = (targetCoords.Y - hitTexCoordsY * scale - position.Y - (GameVars.gravity * travelTime * travelTime / 2)) / travelTime;
            velocity.X = (targetCoords.X - hitTexCoordsX * scale - position.X) / travelTime;

            
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Resources.projectileTextures[ID], position, null, Color.White * opacity, rotation, origin, scale, SpriteEffects.None, 0.1f);
        }
        public void Update(GameTime gameTime)
        {
            liveTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (liveTime > lagTime && onPhysics)
            {
                velocity.Y += GameVars.gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //velocity.X = MathHelper.Clamp(velocity.X, -GameVars.terminalVelocity.X, GameVars.terminalVelocity.X);
                //velocity.Y = MathHelper.Clamp(velocity.Y, -GameVars.terminalVelocity.Y, GameVars.terminalVelocity.Y);
                position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (position.Y > GameVars.RENDERHEIGHT) disposable = true;
            if (fading)
            {
                if (opacity > 0)
                {
                    opacity -= 1 / fadeDuration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    opacity = 0;
                    fading = false;
                    fadeAction();
                }
            }
            if (velocity.X > 0 && position.X > GameVars.RENDERWIDTH)
            {
                velocity.X = -velocity.X;
            }
            else if (velocity.X < 0 && position.X < 0)
            {
                velocity.X = -velocity.X;
            }

        }

        internal void ShatterAndDestroy(MainGame game)
        {
            for (int i = 0; i < GameVars.compensatedDebris1DCount * GameVars.compensatedDebris1DCount; i++)
            { game.Debris.Add(new DebrisEntity(ID, position, velocity, (byte)i, scale)); }
            disposable = true;

        }
    }
}
