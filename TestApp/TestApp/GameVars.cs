using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YutShot
{
    static class GameVars
    {
        public static Random random = new Random();
        public const int RENDERWIDTH = 720;
        public const int RENDERHEIGHT = 1280;
        public static float roundInterval = 0.5f;
        public static string version = "1.0r";
		public static byte GUITextureNumber = 6;
        public static float gravity = 10 * 40f;
        public static float fullGUIRatio;
        /// <summary>
        /// By level
        /// </summary
      //  public static byte[] projectilesNumber = new byte[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        // public static byte[] projectilesTypeNumbers = new byte[10] { 4, 5, 6, 7, 8, 9, 10, 10, 10, 10 };
        // public static byte[] roundsAtLevel = new byte[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
         public static byte[] projectilesNumber = new byte[10] { 2, 3, 4, 5, 5, 5, 6, 6, 6, 7 };
         public static byte[] projectilesTypeNumbers = new byte[10] { 4, 5, 6, 7, 8, 9, 10, 10, 10, 10 };
         public static byte[] roundsAtLevel = new byte[10] { 0, 3, 6, 10, 17, 23, 31, 37, 42, 50 };
        public static byte maxLevel = 10;
        /// <summary>
        /// In terms of score
        /// </summary>
        public static byte projectilePixelSize = 15;
        public static byte buttonSize = 15;
        public static float projectilesInterval = .3f;
        public static float arrivalTimeLag = .2f;
        public static float bulletScale = 8.5f;
        public static byte debrisSize = 2; //in pixels
        //Projectiles 
		/// <summary>
		/// 11 is heart
		/// </summary>
        public static byte projectileTotalTypeNumber = 11;
        public static byte compensatedDebris1DCount = (byte)((projectilePixelSize + debrisSize - 1) / debrisSize);
        //Buttons
        public static byte buttonTextureNumbers = 6;
        public static byte bulletTextureNumbers = 1;
        public static byte backdropEntityTextureNumbers = 5;
    }
}
