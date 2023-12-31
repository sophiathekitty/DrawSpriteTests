﻿using Sandbox.Game.EntityComponents;
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
        // ScreenMenu
        //----------------------------------------------------------------------
        public class ScreenMenu
        {
            ScreenSprite title;
            List<ScreenMenuItem> menuItems = new List<ScreenMenuItem>();
            int selectedIndex = 0;
            public float ItemHeight = 30f;
            public float ItemIndent = 20f;
            float _width = 100f;
            float headerHeight = 1.2f;
            ScreenActionBar actionBar;
            public string menuScrollActions
            {
                get
                {
                    if (actionBar != null)
                    {
                        if (actionBar.Count == 5) return "Up Down Select  Back";
                        if (actionBar.Count == 4) return "Up Down Select Back";
                    }
                    return "Up Down Select";
                }
            }
            public string menuEditActions
            {
                get
                {
                    if (actionBar != null)
                    {
                        if (actionBar.Count == 5) return "+ ++ - -- Done";
                        if (actionBar.Count == 4) return "+ -  Done";
                    }
                    return "+ - Done";
                }
            }
            public float Width
            {
                get { return _width; }
                set
                {
                    _width = value;
                    foreach (ScreenMenuItem item in menuItems)
                    {
                        item.Width = value;
                    }
                }
            }
            public float Height
            {
                get
                {
                    if (menuItems.Count == 0) return 0;
                    return ItemHeight * menuItems.Count+(ItemHeight*headerHeight);
                }
            }
            public int SelectedIndex
            {
                get { return selectedIndex; }
                set
                {
                    if (value >= 0 && value < menuItems.Count)
                    {
                        if(menuItems.Count > 0) menuItems[selectedIndex].Selected = false;
                        selectedIndex = value;
                        menuItems[selectedIndex].Selected = true;
                    }
                }
            }
            public bool Visible
            {
                get
                {
                    return title.Visible;
                }
                set
                {
                    title.Visible = value;
                    foreach (ScreenMenuItem item in menuItems)
                    {
                        item.Visible = value;
                    }
                }
            }
            public ScreenMenuItem SelectedItem
            {
                get
                {
                    if (menuItems.Count == 0) return null;
                    return menuItems[selectedIndex];
                }
            }
            public ScreenMenu(string title, float width, ScreenActionBar actionBar)
            {
                _width = width;
                this.title = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.CenterLeft, Vector2.Zero, headerHeight, Vector2.Zero, Color.White, "White", title, TextAlignment.LEFT, SpriteType.TEXT);
                this.actionBar = actionBar;
            }
            // add a label to the menu
            public void AddLabel(string label)
            {
                menuItems.Add(new ScreenMenuItem(label, _width));
            }
            // add variable to the menu
            public void AddVariable(string label, string varName, string defaultValue = "")
            {
                menuItems.Add(new ScreenMenuItem(label, varName, _width,defaultValue));
            }
            // add the sprites to the render list and positions the menu items
            public void AddSprites(ref List<ScreenSprite> sprites)
            {
                Vector2 position = new Vector2(ItemIndent,Height/-2);
                title.Position = position;
                sprites.Add(title);
                position.Y += ItemHeight*headerHeight;
                int i = 0;
                foreach (ScreenMenuItem item in menuItems)
                {
                    item.Selected = (i++ == selectedIndex);
                    item.Position = position;
                    position.Y += ItemHeight;
                    item.AddSprites(ref sprites);
                }
            }
            // remove the sprites from the render list
            public void RemoveSprites(ref List<ScreenSprite> sprites)
            {
                foreach (ScreenMenuItem item in menuItems)
                {
                    item.RemoveSprites(ref sprites);
                }
            }
            public string HandleInput(string argument)
            {
                GridInfo.Me.CustomData += "\nHandleInput " + argument;
                if (argument.ToLower().StartsWith("btn"))
                {
                    GridInfo.Me.CustomData += "\nbtn " + argument;
                    // handle button press
                    string[] args = argument.Split(' ');
                    int btn = -1;
                    if (args.Length > 1)
                    {
                        int.TryParse(args[1], out btn);
                    }
                    if (btn > 0)
                    {
                        string action = actionBar.HandleInput(btn-1);
                        GridInfo.Me.CustomData += "\naction " + action;
                        if (action == "up")
                        {
                            SelectedItem.Editing = false;
                            SelectedIndex--;
                        }
                        else if (action == "down")
                        {
                            SelectedItem.Editing = false;
                            SelectedIndex++;
                        }
                        else if (action == "select")
                        {
                            if (SelectedItem.IsAction)
                            {
                                return (SelectedItem.Label.ToLower());
                            }
                            else if (SelectedItem.IsToggle)
                            {
                                SelectedItem.Editing = true;
                                SelectedItem.DataAsBool = !SelectedItem.DataAsBool;
                            }
                            else
                            {
                                SelectedItem.Editing = true;
                                actionBar.SetActions(menuEditActions);
                            }
                        }
                        else if (action == "+")
                        {
                            SelectedItem.DataAsInt++;
                        }
                        else if (action == "++")
                        {
                            SelectedItem.DataAsInt += 10;
                        }
                        else if (action == "-")
                        {
                            SelectedItem.DataAsInt--;
                        }
                        else if (action == "--")
                        {
                            SelectedItem.DataAsInt -= 10;
                        }
                        else if (action == "done")
                        {
                            SelectedItem.Editing = false;
                            actionBar.SetActions(menuScrollActions);
                        }
                        else
                        {
                            return action;
                        }
                    }
                }
                return "";
            }

        }
        //----------------------------------------------------------------------
    }
}
