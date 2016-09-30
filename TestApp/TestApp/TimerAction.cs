using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YutShot
{
    public class TimerAction
    {
        public bool disposable = false;
        bool repeat = false;
        float elapsed = 0;
        float interval = 1;
        Action task;

        public TimerAction(float _interval, Action _task, bool _repeat = false)
        {
            interval = _interval;
            repeat = _repeat;
            task = _task;
        }

        public void Update(GameTime gameTime)
        {
            if (!disposable)
            { 
                elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (elapsed > interval)
                {
                    task();
                    if (repeat)
                    {
                        elapsed = 0;
                    }
                    else
                    {
                        disposable = true;
                    }
                }
           }
       }
    }
}
