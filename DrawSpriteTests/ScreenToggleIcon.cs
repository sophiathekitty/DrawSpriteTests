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
        // ScreenToggleIcon
        //----------------------------------------------------------------------
        public class ScreenToggleIcon
        {
            string off_icon;
            string on_icon;
            bool _state = false;
            ScreenSprite sprite;
            string variableName;
            public bool State
            {
                get { return _state; }
                set
                {
                    _state = value;
                    if (_state)
                    {
                        sprite.Data = on_icon;
                    }
                    else
                    {
                        sprite.Data = off_icon;
                    }
                }
            }
            public ScreenSprite Sprite
            {
                get { return sprite; }
            }
            public ScreenToggleIcon(string offIcon, string onIcon, string variableName, Vector2 position, Vector2 size, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft, TextAlignment alignment = TextAlignment.LEFT)
            {
                off_icon = offIcon;
                on_icon = onIcon;
                sprite = new ScreenSprite(anchor,position,0f,size,Color.White,"",offIcon,alignment,SpriteType.TEXTURE);
                this.variableName = variableName;
                GridInfo.AddChangeListener(variableName, OnVariableChanged);
                State = GridInfo.GetVarAs<bool>(variableName, false);
            }
            public void OnVariableChanged(string name, string value)
            {
                bool newState = GridInfo.GetVarAs<bool>(variableName);
                if (newState != State)
                {
                    State = newState;
                }
            }
        }
        //----------------------------------------------------------------------
    }
}
