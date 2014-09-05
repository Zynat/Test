#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace TestAs
{
    internal class Program
    {
        public const string TestAs = "TestAs";
        //Menu
        public static Menu Config;
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {

            //Create the menu
            Config = new Menu("TestMenu", "TestMenu", true);

            Config.AddSubMenu(new Menu("Spam", "Spam"));
            Config.SubMenu("Spam")
                .AddItem(new MenuItem("SpamAll", "Spam All").SetValue(new KeyBind(32, KeyBindType.Press)));
            Config.AddToMainMenu();
            //Events
            Game.OnGameUpdate += Game_OnGameUpdate;

        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Config.Item("SpamAll").GetValue<KeyBind>().Active)
            {
                SpamAlllol();
            }
        }

        private static void SpamAlllol()
        {
            Game.Say("/all Fuck you.");
        }

    }
}