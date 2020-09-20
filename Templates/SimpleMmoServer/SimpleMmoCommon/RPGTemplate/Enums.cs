using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoCommon.RPGTemplate
{
  public enum MobState
    {
        none = 0,
        idle = 1,
        move = 2,
        moveBack = 3,
        follow = 4,
        attack = 10,
        death = 20,
    }
}
