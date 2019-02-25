using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.EffectsScripts
{
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
