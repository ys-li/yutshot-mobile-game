using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YutShot
{
	class GUISprite:GUIElement
    {
        static byte textureID;
        public Color color = Color.White;
        Texture2D texture;
        /// <summary>
        /// Rectangle Space
        /// </summary>
        private Vector2 origin;
        Rectangle? srcRect = null;
        
        
		
  
		/// <summary>
		/// Initializes a new instance of the <see cref="YutShot.GUISprite"/> class.
		/// </summary>
		/// <param name="_position">Position.</param>
		/// <param name="_textureID">Texture I.</param>
		/// <param name="_color">Color.</param>
		/// <param name="_scale">Scale.</param>
		/// <param name="animation">Type of animation. 0 is nil, 1 - fadelinear, 2 - enlargelinear</param>
		public GUISprite(Vector2 _position, byte _textureID, Color _color, float _scale, byte animation)
        {
            position = _position;
            textureID = _textureID;
            if (_color != null) color = _color;
			scale = _scale;
			if (animation != 0) {
				animHandler = new AnimationHandler (this, animation, 0.6f);
			}
			
            opacity = 1;
            if (textureID > 99)//level
            {
                texture = Resources.levelResultTexture;
                origin = new Vector2(30, 10);
                int temp = textureID - 100;
                srcRect = new Rectangle((temp % 4) * 60, (temp / 4) * 20, 60, 20);
            }
            else
            {
                texture = Resources.GUITextures[textureID];
                origin = new Vector2(texture.Width / 2, texture.Height / 2);
            }
            
        }
        public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw (texture, position, srcRect, color * opacity, 0, origin, scale, SpriteEffects.None, 0);
		}
        
        
        public override void Update(GameTime gameTime)
        {
			if (animHandler != null) {
				animHandler.Update (gameTime);
				if (animHandler.disposable)
					animHandler = null;
			}
            base.Update(gameTime);
        }
    }
}
