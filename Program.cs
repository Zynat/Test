using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
namespace ZyTrynd
{
    class Program
    {
        public static string ChampName = "Tryndamere";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Base Player = ObjectManager.Player; // Instead of typing ObjectManager.Player you can just type Player
        public static Spell Q, W, E, R;

        public static Menu Zy;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {

            Q = new Spell(SpellSlot.Q, 0);
            W = new Spell(SpellSlot.W, 400);
            E = new Spell(SpellSlot.E, 660);
            R = new Spell(SpellSlot.R, 0);
            //Base menu
            Zy = new Menu("ZyTrynd", "Zy", true);
            //Orbwalker and menu
            Zy.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Zy.SubMenu("Orbwalker"));
            //Target selector and menu
            var ts = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(ts);
            Zy.AddSubMenu(ts);
            //Combo menu
            Zy.AddSubMenu(new Menu("Combo", "Combo"));
            Zy.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q?").SetValue(true));
            Zy.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W?").SetValue(true));
            Zy.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E?").SetValue(true));
            Zy.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R?").SetValue(true));
            Zy.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            Zy.SubMenu("Combo").AddItem(new MenuItem("QonHp", "Q on % hp")).SetValue(new Slider(25, 100, 0));
            Zy.SubMenu("Combo").AddItem(new MenuItem("RonHp", "R on % hp")).SetValue(new Slider(10, 100, 0));
            Zy.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw; // Add onDraw
            Game.OnGameUpdate += Game_OnGameUpdate; // adds OnGameUpdate (Same as onTick in bol)

        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (Zy.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
        }

        public static void Combo()
        {
            var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Physical);
            useRSmart();
            useQSmart();
            //UseE();
            useWSmart(target);
        }

        public static void useQSmart()
        {
            if (!Q.IsReady())
                return;
            if (CurrHP() <= Zy.Item("QonHp").GetValue<Slider>().Value)
                Q.Cast();
        }

        public static void useWSmart(Obj_AI_Hero target)
        {
            if (!W.IsReady())
                return;

            float trueAARange = Player.AttackRange + target.BoundingRadius;
            float trueERange = target.BoundingRadius + W.Range;

            float dist = Player.Distance(target);
            Vector2 dashPos = new Vector2();
            if (target.IsMoving)
            {
                Vector2 tpos = target.Position.To2D();
                Vector2 path = target.Path[0].To2D() - tpos;
                path.Normalize();
                dashPos = tpos + (path * 100);
            }
            float targ_ms = (target.IsMoving && Player.Distance(dashPos) > dist) ? target.MoveSpeed : 0;
            float msDif = (Player.MoveSpeed - targ_ms) == 0 ? 0.0001f : (Player.MoveSpeed - targ_ms);
            float timeToReach = (dist - trueAARange) / msDif;
            if (dist > trueAARange && dist < trueERange)
            {
                if (timeToReach > 1.7f || timeToReach < 0.0f)
                {
                    W.Cast();
                }
            }
        }

        public static void UseE()
        {
            return;
        }

        public static void useRSmart()
        {
            if (!R.IsReady())
                return;
            if (CurrHP() <= Zy.Item("RonHp").GetValue<Slider>().Value)
                R.Cast();
        }


        public static int CurrHP()
        {
            return (int)((Player.Health / Player.MaxHealth) * 100);
        }
    }
}
