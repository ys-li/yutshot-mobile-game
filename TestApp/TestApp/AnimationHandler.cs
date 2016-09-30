using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YutShot
{
    class AnimationHandler
    {
		enum AnimationType
		{
			Null,
			FadeLinear,
			EnlargeLinear,
            SlideSin,
            SlideAndFade,
            ShrinkAndFade
		}
		protected GUIElement guiElement;
		float elapsed = 0;
		float duration = 0;
        float slideSinAmplitude;
        float targetScale;
        float targetY;
		public bool disposable = false;
        
		AnimationType animationType;
		/// <summary>
		/// Initializes a new instance of the <see cref="YutShot.AnimationHandler"/> class. This class is an indepedent class to handle GUI animations.
		/// </summary>
		/// <param name="_element">GUI element</param>
		/// <param name="type">Type.</param>
		/// <param name="_duration">Duration.</param>
		public AnimationHandler(GUIElement _element, byte type, float _duration)
		{
			guiElement = _element;
			duration = _duration;
			animationType = (AnimationType)type;
            if (animationType == AnimationType.Null)
            { disposable = true; return; }
            targetScale = guiElement.scale;
            targetY = guiElement.position.Y;
            if (animationType == AnimationType.SlideSin || animationType == AnimationType.SlideAndFade)
            {
                slideSinAmplitude = (GameVars.RENDERHEIGHT - targetY) / 2;
                guiElement.position.Y = GameVars.RENDERHEIGHT;
            }
		}

        public void Update(GameTime gameTime)
        {
            
            if (elapsed < duration) {

                
                
                switch (animationType) {
				case AnimationType.EnlargeLinear:
                    elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    guiElement.scale = elapsed / duration * targetScale;
					break;
				case AnimationType.FadeLinear:
                    elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    guiElement.opacity = elapsed / duration;
					break;
                case AnimationType.SlideSin:
                    float velocity = (float)Math.Cos(elapsed / duration * Math.PI);
                    elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    velocity = (float)(slideSinAmplitude * (velocity - Math.Cos(elapsed / duration * Math.PI)));
                    guiElement.position.Y -= velocity;
                    break;
                case AnimationType.SlideAndFade:
                    guiElement.opacity = elapsed / duration;
                    float velocity2 = (float)Math.Cos(elapsed / duration * Math.PI);
                    elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    velocity2 = (float)(slideSinAmplitude * (velocity2 - Math.Cos(elapsed / duration * Math.PI)));
                    guiElement.position.Y -= velocity2;
                    break;
                case AnimationType.ShrinkAndFade:
                    guiElement.opacity = elapsed / duration;
                    guiElement.scale = (2.5f - 1.5f * elapsed / duration) * targetScale;
                    break;
                default:
					break;
				}
			} else {
				guiElement.scale = targetScale;
				guiElement.opacity = 1;
                guiElement.position.Y = targetY;
                disposable = true;
			}
        }

    }
}
