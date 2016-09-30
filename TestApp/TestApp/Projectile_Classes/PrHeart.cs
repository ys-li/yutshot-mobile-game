using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YutShot
{
    class PrHeart : Projectile
    {
		MainGame g;
        public PrHeart(float _lagtime, Vector2 _target, float _arrivaltime, MainGame _g) : base(10,_lagtime,_target,_arrivaltime)
        {
            g = _g;
        }
		public override bool isHit(Vector2 coords)
		{
			if (base.isHit(coords))
			{
				g.life.addLife();
				return true;
			}
			return false;
		}
    }
}
