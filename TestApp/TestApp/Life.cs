using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace YutShot
{
	public class Life
	{
        private int _lives;
        GUISprite cue;
		public int lives
        {
            get { return _lives; }
            set { _lives = value; g.NativeFunctions.setLife(lives); }
        }
		Vector2 pos = new Vector2(GameVars.RENDERWIDTH, GameVars.RENDERHEIGHT);
		float scale = 4.5f;
        MainGame g;
		public Life(int __lives, MainGame _g)
		{
			_lives = __lives;
            g = _g;
		}
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Resources.heartTexture, pos, null, lives > 0 ? Color.White : Color.White * 0.2f,0, new Vector2(15, 15), scale, SpriteEffects.None,0);
			spriteBatch.Draw(Resources.heartTexture, pos, null, lives > 1 ? Color.White : Color.White * 0.2f, 0, new Vector2(30, 15), scale, SpriteEffects.None, 0);
			spriteBatch.Draw(Resources.heartTexture, pos, null, lives > 2 ? Color.White : Color.White * 0.2f, 0, new Vector2(45, 15), scale, SpriteEffects.None, 0);
			if (cue != null) cue.Draw(spriteBatch);
        }
        public void Update(GameTime gameTime)
        {
			if (cue != null) {
				cue.Update (gameTime);
				if (cue.disposable)
					cue = null;
			}
        }
		public void addLife()
		{
            if (lives < 3)
            {
                lives++;
            }
            cue = new GUISprite(new Vector2(GameVars.RENDERWIDTH / 2, GameVars.RENDERHEIGHT / 2), 5, Color.White, GameVars.fullGUIRatio, 4);
            g.Timers.Add(new TimerAction(1.3f, delegate { cue.fadeOut(0.3f); }));
            //TODO: Add Life Animation here.
		}
	}
}

