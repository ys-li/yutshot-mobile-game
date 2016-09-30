using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YutShot
{
    class Pr2 : Projectile
    {
        public Pr2(float _lagtime, Vector2 _target, float _arrivaltime  ) : base(2,_lagtime,_target,_arrivaltime)
        {
            
        }
    }
}
