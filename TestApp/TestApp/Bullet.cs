using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YutShot
{
    class Bullet
    {
        Vector2 position;
        float opacity = 1;
        bool fading = false;
        float fadeDuration;
        Action fadeAction;
        float rotation = 0;
        Vector2 origin = Vector2.Zero;
        float scale;


        public Bullet(Vector2 pos)
        {
            position = pos;
            origin = new Vector2(Resources.bulletTextures[0].Width / 2, Resources.bulletTextures[0].Height / 2);
            scale = GameVars.bulletScale;
        }

        public void StartFade(float _duration, Action _afterFadeAction)
        {
            fading = true;
            fadeDuration = _duration;
            fadeAction = _afterFadeAction;
            
        }

        public bool checkHit(Projectile pj)
        {
            return pj.isHit(position);
        }
        

        public void Update(GameTime gameTime)
        {
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
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Resources.bulletTextures[0], position, null, Color.White * opacity, rotation, origin, scale, SpriteEffects.None, 0);
        }
    }
}
