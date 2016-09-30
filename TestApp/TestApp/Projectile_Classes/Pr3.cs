using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YutShot
{
    class Pr3 : Projectile
    {
        public Pr3(float _lagtime, Vector2 _target, float _arrivaltime) : base(3,_lagtime,_target,_arrivaltime)
        {
            
        }
    }
}
