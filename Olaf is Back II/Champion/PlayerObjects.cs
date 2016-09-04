using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Olaf.Champion
{
    internal class PlayerObjects
    {
        public static GameObject AxeObject { get; set; }
        public static float StartTime { get; set; }
        public static float EndTime { get; set; }

        public static Menu MenuLocal { get; private set; }

        public static void Init()
        {
            GameObject.OnCreate += GameObjectOnOnCreate;
            GameObject.OnDelete += GameObjectOnOnDelete;
        }

        private static void GameObjectOnOnCreate(GameObject obj, EventArgs args)
        {
            if (obj.Name.ToLower().Contains("olaf_base_q_axe") && obj.Name.ToLower().Contains("ally"))
            {
                AxeObject = obj;
                StartTime = Game.Time;
                EndTime = Game.Time + 8;
            }
        }
        private static void GameObjectOnOnDelete(GameObject obj, EventArgs args)
        {
            //if (obj.Name == "olaf_axe_totem_team_id_green.troy")
            if (obj.Name.ToLower().Contains("olaf_base_q_axe") && obj.Name.ToLower().Contains("ally"))
            {
                AxeObject = null;
            }
        }

       

    }
}
