using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        //----------------------------------------------------------------------
        // ScreenPercentageBar
        //----------------------------------------------------------------------
        public class ScreenPercentageBar
        {
            ScreenSprite border_top;
            ScreenSprite border_bottom;
            ScreenSprite border_left;
            ScreenSprite border_right;
            ScreenSprite bar;
            ScreenSprite back;
            float percent;
            string varName;
            Color barFull;
            Color barColor;
            Vector2 Size;

            public ScreenPercentageBar(string varName, Vector2 position, Vector2 size, Color borderColor, Color barColor, Color barFull, Color backColor, float percent, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.BottomCenter, float borderSize = 2f)
            {
                this.percent = GridInfo.GetVarAs<float>(varName, percent);
                this.barFull = barFull;
                this.barColor = barColor;
                Size = size;
                bar = new ScreenSprite(anchor, position, 0, new Vector2(size.X * percent, size.Y), barColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
                back = new ScreenSprite(anchor, position, 0, size, backColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
                border_top = new ScreenSprite(anchor, new Vector2(position.X, position.Y - (size.Y / 2)), 0, new Vector2(size.X, borderSize), borderColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
                border_bottom = new ScreenSprite(anchor, new Vector2(position.X, position.Y + (size.Y / 2)), 0, new Vector2(size.X, borderSize), borderColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
                border_left = new ScreenSprite(anchor, position, 0, new Vector2(borderSize, size.Y), borderColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
                border_right = new ScreenSprite(anchor, new Vector2(position.X + size.X, position.Y), 0, new Vector2(borderSize, size.Y), borderColor, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE);
                this.varName = varName;
                GridInfo.AddChangeListener(varName, OnVarUpdated);
            }
            public void AddSprites(ref List<ScreenSprite> sprites)
            {
                sprites.Add(back);
                sprites.Add(bar);
                sprites.Add(border_top);
                sprites.Add(border_bottom);
                sprites.Add(border_left);
                sprites.Add(border_right);
            }
            void OnVarUpdated(string varName, string value)
            {
                if (varName == this.varName)
                {
                    percent = float.Parse(value);
                    bar.Size = new Vector2(Size.X * percent, Size.Y);
                    if (percent > 0.75f)
                    {
                        bar.Color = barFull;
                    }
                    else
                    {
                        bar.Color = barColor;
                    }
                }
            }
        }
        //----------------------------------------------------------------------
    }
}
