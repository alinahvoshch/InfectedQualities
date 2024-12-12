using Terraria;
using System.Reflection;

namespace InfectedQualities.Content.Extras
{
    public static class InfectedQualitiesUtilities
    {
        /// <summary>
        /// This is temporary, I have to use refection until the methods get public for the stable release.
        /// Until then, this class and this method stays here.
        /// </summary>
        public static bool RefectionMethod(int x, int y, string name)
        {
            return (bool)typeof(WorldGen).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, [x, y]);
        }
    }
}
