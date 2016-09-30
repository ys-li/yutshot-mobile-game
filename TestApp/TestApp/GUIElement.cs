using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YutShot
{
    abstract class GUIElement
    {
		
		public float scale;
		public float opacity;
        /// <summary>
        /// Centered position
        /// </summary>
        public Vector2 position = Vector2.Zero;
		public bool disposable = false;
        float fullFadeDuration = 0;
        float currentFadeDuration = 0;

        public virtual void Update(GameTime gameTime)
        {
            if (fullFadeDuration > 0)
            {
                currentFadeDuration += (float)gameTime.ElapsedGameTime.TotalSeconds;
                opacity = 1 - (currentFadeDuration / fullFadeDuration);
                if (currentFadeDuration > fullFadeDuration) disposable = true;
            }
        }
        public void fadeOut(float duration)
        {
            fullFadeDuration = duration;
        }
        public abstract void Draw (SpriteBatch spriteBatch);
        public AnimationHandler animHandler;

    }
}
