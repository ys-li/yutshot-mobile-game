using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YutShot
{
    public class DebrisEntity
    {
        public enum TypeOfTexture
        {
            Projectile = 0,
            Button = 1
        }
        byte ID = 0;
        TypeOfTexture typeOfTexture = TypeOfTexture.Projectile;
        public bool disposable = false;
        float Scale = 1f;
        Vector2 position = new Vector2(0,0);
        Vector2 velocity = new Vector2(0,0);
        Rectangle RectMask;
        float opacity = 1f;
        /// <summary>
        /// Relative to unscaled texel (raw from texture), starts from 0
        /// </summary>
        byte xcoord, ycoord;
        public DebrisEntity(byte _id, Vector2 projectilePosition, Vector2 originalVelocity, byte _pixelcount, float _scale, TypeOfTexture tot = TypeOfTexture.Projectile)
        {
            Scale = _scale;
            ID = _id;
            int smallxcoord = (_pixelcount % GameVars.compensatedDebris1DCount);
            int smallycoord = (_pixelcount / GameVars.compensatedDebris1DCount);
            xcoord = (byte)(GameVars.debrisSize * smallxcoord);
            ycoord = (byte)(GameVars.debrisSize * smallycoord);
            typeOfTexture = tot;

            velocity.X = (float)((GameVars.random).NextDouble() - 0.5) * GameVars.RENDERWIDTH / 5;
            velocity.Y = -(float)((GameVars.random).NextDouble() * 300);
            velocity.X += (originalVelocity.X * 0.5f);
            position = projectilePosition + new Vector2(xcoord * Scale, ycoord * Scale);
            int temp = GameVars.projectilePixelSize % GameVars.debrisSize;
            if (temp != 0)
            {
                RectMask = new Rectangle(xcoord, ycoord, (smallxcoord == (GameVars.compensatedDebris1DCount - 1))?temp: GameVars.debrisSize, (smallycoord == (GameVars.compensatedDebris1DCount - 1)) ?temp: GameVars.debrisSize);
            }
            else
            { 
                RectMask = new Rectangle(xcoord, ycoord, GameVars.debrisSize, GameVars.debrisSize);
            }
        }
        public void Update(GameTime gameTime)
        {
            //Update position
            velocity.Y += GameVars.gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            opacity *= 0.99f;
            if (position.Y > GameVars.RENDERHEIGHT + 100) disposable = true;
            else if (opacity <= 0.07f) disposable = true;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (typeOfTexture == TypeOfTexture.Projectile) {
            spriteBatch.Draw(Resources.projectileTextures[ID], position, RectMask, Color.White * opacity, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }
            else
            {
                spriteBatch.Draw(Resources.buttonTextures[ID], position, RectMask, Color.White * opacity, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            }
        }
    }
}
