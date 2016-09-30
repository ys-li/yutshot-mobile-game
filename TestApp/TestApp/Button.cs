using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YutShot
{
	class Button:GUIElement
    {
        byte textureID;
        private string _caption;
        public Color color = Color.White;
        private Action toDo;
        private float width;
        /// <summary>
        /// Rectangle Space
        /// </summary>
        private Vector2 origin;
        bool reconstruct;
        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }
        public Button(string caption, Vector2 _position, byte _textureID, Color _color, Action _toDo, byte animation, float _scale, bool reconstruct = false)
        {
            Caption = caption;
            position = _position;
            textureID = _textureID;
            if (_color != null) color = _color;
            scale = _scale;
            width = GameVars.buttonSize * scale;
            toDo = _toDo;
            opacity = 1;
            if (reconstruct) { } 
            if (animation != 0) animHandler = new AnimationHandler(this, animation, 1f);
            origin = new Vector2(Resources.buttonTextures[textureID].Width / 2, Resources.buttonTextures[textureID].Height / 2);
            this.reconstruct = reconstruct;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Resources.buttonTextures[textureID], position, null, color * opacity, 0, origin, scale, SpriteEffects.None, 0);
            //if (Caption != "") { spriteBatch.DrawString(Resources.buttonFont, Caption, position, Color.White, 0, fontorigin, 1f, SpriteEffects.None, 0); }
        }
        

        public bool isHit(Vector2 coords, MainGame game)
        {
            if (Math.Abs(position.X - coords.X) < width/2)
            {
                if (Math.Abs(position.Y - coords.Y) < width/2)
                {
                    toDo();
                    disposable = !reconstruct;
                    explode(game);
                    return true;
                }
            }
            return false;
        }
        public void explode(MainGame game)
        {
            for (int i = 0; i < GameVars.compensatedDebris1DCount * GameVars.compensatedDebris1DCount; i++)
            { game.Debris.Add(new DebrisEntity(textureID, position - (new Vector2(GameVars.projectilePixelSize, GameVars.projectilePixelSize )* scale / 2), Vector2.Zero, (byte)i, scale,DebrisEntity.TypeOfTexture.Button)); }
            Resources.explodeSound.Play(1, (float)(GameVars.random.NextDouble() - 0.5f), 0);
        }
        public override void Update(GameTime gameTime)
        {
            if (animHandler != null)
            {
                animHandler.Update(gameTime);
                if (animHandler.disposable) animHandler = null;
            }
            base.Update(gameTime);

        }
    }
}
