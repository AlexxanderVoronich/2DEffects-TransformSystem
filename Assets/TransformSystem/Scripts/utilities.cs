using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.EffectsScripts
{
    public enum eEffectMode
    {
        TERMINAL_MODE,
        PARALLEL_MODE,
        DIRECT_MODE
    };

    public enum eEffectType
    {
        COMPOUND_TYPE,
        TERMINAL_ROTATE,
        TERMINAL_MOVE_LINE_LOCAL_POS,
        TERMINAL_MOVE_LINE_GLOBAL_POS,
        TERMINAL_MOVE_ARC_LOCAL_POS,
        TERMINAL_MOVE_ARC_GLOBAL_POS,
        TERMINAL_SCALE,
        TERMINAL_ARRIVE,
        TERMINAL_HIDE,
        TERMINAL_CHANGE_SPRITE,
        TERMINAL_FILL_RECT,
        TERMINAL_ANIMATION,
        TERMINAL_CHANGE_COLOR
    };

    public class Utilities
    {
        private static scriptTrailSystem m_trail_system = null;
        public static void setTrailSystem(scriptTrailSystem _s)
        {
            m_trail_system = _s;
        }

        public static scriptTrailSystem getTrailSystem()
        {
            return m_trail_system;
        }
    }
}
