using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace YutShot
{
    public class Score
    {
        public Vector2 position;
        public float scale = 5f;
        private int _score = 0;
        Rectangle rect1;
        Rectangle rect10;
        Rectangle rect100;
        Vector2 origin1;
        Vector2 origin10;
        Vector2 origin100;
        public int score
        {
            get { return _score; }
            set { _score = value; setRectangles(); }
        }
        public Score()
        {
            position = new Vector2(GameVars.RENDERWIDTH / 2, GameVars.RENDERHEIGHT / 6);
            add(0);
        }
        public Score(Vector2 _pos)
        {
            position = _pos;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (score < 10)
            {
                spriteBatch.Draw(Resources.scoreTexture, position, rect1, Color.White, 0, origin1, scale, SpriteEffects.None, 0);
            }
            else if (score < 100)
            {
                spriteBatch.Draw(Resources.scoreTexture, position, rect1, Color.White, 0, origin1, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(Resources.scoreTexture, position, rect10, Color.White, 0, origin10, scale, SpriteEffects.None, 0);
            }
            else
            {

                spriteBatch.Draw(Resources.scoreTexture, position, rect1, Color.White, 0, origin1, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(Resources.scoreTexture, position, rect10, Color.White, 0, origin10, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(Resources.scoreTexture, position, rect100, Color.White, 0, origin100, scale, SpriteEffects.None, 0);
            }
        }
        public void setRectangles()
        {
            if (_score < 10)
            {
                rect1 = new Rectangle((_score % 4) * 10, (_score / 4) * 15, 10, 15);
                origin1 = new Vector2(5, 7.5f);
            }
            else if (_score < 100)
            {
                rect1 = new Rectangle((_score % 10 % 4) * 10, (_score % 10 / 4) * 15, 10, 15);
                rect10 = new Rectangle((_score / 10 % 4) * 10, _score / 40 * 15, 10, 15);
                origin1 = new Vector2(-0.5f, 7.5f);
                origin10 = new Vector2(10.5f, 7.5f);
            }
            else
            {
                rect1 = new Rectangle((_score % 10 % 4) * 10, (_score % 10 / 4) * 15, 10, 15);
                rect10 = new Rectangle((_score % 100 / 10 % 4) * 10, _score % 100 / 40 * 15, 10, 15);
                rect100 = new Rectangle((_score / 100 % 4) * 10, _score / 400 * 15, 10, 15);
                origin1 = new Vector2(-6, 7.5f);
                origin10 = new Vector2(5, 7.5f);
                origin100 = new Vector2(16, 7.5f);
            }
        }
        public void add(int _amt)
        {
            _score += _amt;
            setRectangles();
        }

    }
}