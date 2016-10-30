#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Orb;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using Color = System.Drawing.Color;
using Orbwalking = Marksman.Orb.Orbwalking;

#endregion

namespace Marksman.Champions
{
    internal class Jhin : Champion
    {
        private Spell Q { get; set; }
        private static Spell W { get; set; }
        private Spell E { get; set; }
        private Spell R { get; set; }


        public Jhin()
        {
            Q = new Spell
            {
                Slot = SpellSlot.Q,
                Range = 600,
            };
            
            W = new Spell
            {
                Slot = SpellSlot.W,
                Range = 2500,
                Delay = 0.90f,
                Width = 30,
                Collision = false,
                Speed = float.MaxValue,
                Type = SkillshotType.SkillshotLine
            };
            
            E = new Spell
            {
                Slot = SpellSlot.E,
                Range = 760,
                Delay = 1f,
                Width = 130,
                Speed = 1500,
                Type = SkillshotType.SkillshotCircle,
            };
            
            R = new Spell
            {
                Slot = SpellSlot.R,
                Range = 3500,
                Delay = 0.25f,
                Width = 80,
                Speed = 5500,
                Type = SkillshotType.SkillshotLine
            };

            Utils.Utils.PrintMessage("Jhin loaded.");
        }

        public override void GameOnUpdate(EventArgs args)
        {
            

        }

        private static List<Obj_AI_Base> CollisionObjects(Spell spell, Obj_AI_Hero source, Obj_AI_Hero target)
        {
            var input = new PredictionInput
            {
                Unit = source,
                Radius = spell.Width,
                Delay = spell.Delay,
                Speed = spell.Speed,
                CollisionObjects = {[0] = CollisionableObjects.Heroes, [1] = CollisionableObjects.YasuoWall},
            };
            
            return
                Collision.GetCollision(new List<Vector3> { target.Position }, input).Where(obj => obj.NetworkId != target.NetworkId)
                    .OrderBy(obj => obj.Distance(source, false))
                    .ToList();
        }

        public override void ExecuteCombo()
        {
            
            var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (!t.IsValidTarget())
            {
                return;
            }

            if (W.IsReady())
            {
                foreach (var e in HeroManager.Enemies.Where(e => e.IsValidTarget(W.Range) && CollisionObjects(W, ObjectManager.Player, e).Count == 0))
                {
                    if (e.Health < W.GetDamage(t))
                    {
                        W.CastIfHitchanceGreaterOrEqual(t);
                    }

                    if (e.HasBuff("jhinespotteddebuff"))
                    {
                        if (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) || t.HasBuffOfType(BuffType.Slow) || t.HasBuffOfType(BuffType.Knockup) && !t.CanMove)
                        {
                            W.CastIfHitchanceEquals(t, HitChance.High);
                        }
                        else
                        {
                            W.CastIfHitchanceGreaterOrEqual(t);
                        }
                    }
                }
            }

            if (t.IsValidTarget(W.Range) && W.IsReady() && CollisionObjects(W, ObjectManager.Player, t).Count > 0)
            {
                if (t.Health < W.GetDamage(t))
                {
                    W.CastIfHitchanceGreaterOrEqual(t);
                }

                if (t.HasBuff("jhinespotteddebuff"))
                {
                    if (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) || t.HasBuffOfType(BuffType.Slow) || t.HasBuffOfType(BuffType.Knockup) && !t.CanMove)
                    {
                        W.CastIfHitchanceEquals(t, HitChance.High);
                    }
                    else
                    {
                        W.CastIfHitchanceGreaterOrEqual(t);
                    }
                }
            }
        }
        
        public override void Drawing_OnDraw(EventArgs args)
        {
            var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (!t.IsValidTarget())
            {
                return;
            }
            var xCol = CollisionObjects(W, ObjectManager.Player, t);
                Console.WriteLine(xCol.Count.ToString());
            foreach (
               var colminion in
                   CollisionObjects(W, ObjectManager.Player, t))
            {
                Render.Circle.DrawCircle(colminion.Position, 105f, Color.Yellow);
                Console.WriteLine(colminion.CharData.BaseSkinName);
            }
            return;

            foreach (var buff in ObjectManager.Player.Buffs)
            {
                Console.WriteLine(buff.Name + " : " + buff.Count);
            }
            Console.WriteLine("-------------------");
            return;
            Spell[] spellList = { Q, W, E };
            foreach (var spell in spellList)
            {
                var menuItem = GetValue<Circle>("Draw" + spell.Slot);
                if (menuItem.Active)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
            }
        }

        public override bool ComboMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQC" + Id, "Use Q").SetValue(true));
            config.AddItem(new MenuItem("UseWC" + Id, "Use W").SetValue(new StringList(new []{"Selected target", "Can stunnable targets"}, 1)));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQH" + Id, "Use Q").SetValue(false));
            config.AddItem(new MenuItem("UseWH" + Id, "Use W").SetValue(false));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.AddItem(
                new MenuItem("DrawQ" + Id, "Q range").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.AddItem(
                new MenuItem("DrawW" + Id, "W range").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.AddItem(
                new MenuItem("DrawE" + Id, "E range").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            return true;
        }

        public override bool LaneClearMenu(Menu config)
        {
            return true;
        }
        public override bool JungleClearMenu(Menu config)
        {
            return false;
        }
    }
}