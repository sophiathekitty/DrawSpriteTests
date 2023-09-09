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
        // ScreenActionBar
        //----------------------------------------------------------------------
        public class ScreenActionBar
        {
            private string[] actions;
            List<ScreenSprite> sprites = new List<ScreenSprite>();
            public int Count { get { return actions.Length; } }
            
            public ScreenActionBar(int actionCount, string actionString = "")
            {
                actions = new string[actionCount];
                if(actionString != "")
                {
                    SetActions(actionString);
                }
            }
            // set actions from a string separated by spaces
            public void SetActions(string actionString)
            {
                string[] _actions = actionString.Split(' ');
                for (int i = 0; i < actions.Length; i++)
                {
                    if (i < _actions.Length)
                    {
                        actions[i] = _actions[i];
                    }
                    else
                    {
                        actions[i] = "";
                    }
                    if (sprites.Count > i)
                    {
                        sprites[i].Data = actions[i];
                    }

                }
            }
            // set a sprite for an action
            public void AddSprite(ScreenSprite sprite)
            {
                if (sprites.Count < actions.Length)
                {
                    sprites.Add(sprite);
                    sprite.Data = actions[sprites.Count - 1];
                }
            }
            // handle action bar input takes an int and returns a string
            public string HandleInput(int input)
            {
                if (input >= 0 && input < actions.Length)
                {
                    return actions[input].ToLower();
                }
                return "";
            }   
        }
        //----------------------------------------------------------------------
    }
}
