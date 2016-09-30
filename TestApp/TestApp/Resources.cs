using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace YutShot
{
    public class Resources
    {
        public static Texture2D[] projectileTextures;
        public static Texture2D[] bulletTextures;
        public static Texture2D[] buttonTextures;
        public static Texture2D levelResultTexture;
        /// <summary>
        /// 0 main menu
        /// 1 about
        /// 2 results
        /// </summary>
		public static Texture2D[] GUITextures;
        public static Texture2D scoreTexture;
		public static Texture2D heartTexture;
        public static Texture2D backdropTexture;
        public static Texture2D backdropEntityTexture;
        static public SpriteFont titleFont;
        static public SpriteFont debugFont;
        static public SoundEffect explodeSound;
        static public SoundEffect dingSound;
        static public SoundEffect throwSound;
        static public SoundEffect eatSound;
        static public SoundEffect loseSound;
        static public SoundEffect shotSound;
        internal static void LoadContent(ContentManager content, GraphicsDevice device)
        {
            bulletTextures = new Texture2D[GameVars.bulletTextureNumbers];
            //font
#if DEBUG
            debugFont = content.Load<SpriteFont>("fonts/debug");
#endif
            explodeSound = content.Load<SoundEffect>("sounds/explode");
            dingSound = content.Load<SoundEffect>("sounds/ding");
            throwSound = content.Load<SoundEffect>("sounds/throw");
            eatSound = content.Load<SoundEffect>("sounds/eat");
            loseSound = content.Load<SoundEffect>("sounds/lose");
            shotSound = content.Load<SoundEffect>("sounds/shot");
            //titleFont = content.Load<SpriteFont> ("fonts/title");
            //bullet
            using (var stream = TitleContainer.OpenStream("Content/sprites/bullet.png"))
            {
                bulletTextures[0] = Texture2D.FromStream(device, stream);
            }
            //projectile
            projectileTextures = new Texture2D[GameVars.projectileTotalTypeNumber];
            for (int i = 0; i < GameVars.projectileTotalTypeNumber; i++)
            {
                using (var stream = TitleContainer.OpenStream("Content/sprites/projectiles/" + i + ".png"))
                {
                    projectileTextures[i] = Texture2D.FromStream(device, stream);
                }
            }

            //button
            buttonTextures = new Texture2D[GameVars.buttonTextureNumbers];
            for (int i = 0; i < GameVars.buttonTextureNumbers; i++)
            {
                using (var stream = TitleContainer.OpenStream("Content/sprites/button/" + i + ".png"))
                {
                    buttonTextures[i] = Texture2D.FromStream(device, stream);
                }
            }

            //backdrop
            using (var stream = TitleContainer.OpenStream("Content/sprites/backdrop/0.png"))
            {
                backdropTexture = Texture2D.FromStream(device, stream);
            }

            //backdropEntities
            using (var stream = TitleContainer.OpenStream("Content/sprites/backdrop_entities/sheet.png"))
            {
                backdropEntityTexture = Texture2D.FromStream(device, stream);
            }
            //levelresults
            using (var stream = TitleContainer.OpenStream("Content/sprites/level_results/sheet.png"))
            {
                levelResultTexture = Texture2D.FromStream(device, stream);
            }
            //GUI
            GUITextures = new Texture2D[GameVars.GUITextureNumber + 1];
            for (int i = 0; i < GameVars.GUITextureNumber; i++)
            {
                using (var stream = TitleContainer.OpenStream("Content/sprites/GUI/" + i + ".png"))
                {
                    GUITextures[i] = Texture2D.FromStream(device, stream);
                }
            }
            GUITextures[GameVars.GUITextureNumber] = new Texture2D(device, 1, 1);
            GUITextures[GameVars.GUITextureNumber].SetData(new Color[1] { new Color(0, 0, 0) });
            //Scores
            GameVars.fullGUIRatio = GameVars.RENDERHEIGHT / GUITextures[0].Height;
            
            using (var stream = TitleContainer.OpenStream("Content/sprites/scores/sheet.png"))
            {
                scoreTexture = Texture2D.FromStream(device, stream);
            }
			heartTexture = projectileTextures[10];
        }
    }
}

