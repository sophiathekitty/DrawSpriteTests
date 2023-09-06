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
    partial class Program : MyGridProgram
    {
        Screen progScreen;
        ScreenMenu menu;
        ScreenActionBar actionBar;
        public Program()
        {
            // setup GridInfo
            GridInfo.Init(GridTerminalSystem,IGC,Me,Echo);
            if(Storage != "") GridInfo.Load(Storage);
            //
            // setup screen
            //
            progScreen = new Screen(Me.GetSurface(0));
            //progScreen.AddTextSprite(ScreenSprite.ScreenSpriteAnchor.TopCenter,new Vector2(0,0),1.5f,Color.White,"White","Header",TextAlignment.CENTER);
            // add action bar
            actionBar = progScreen.AddActionBar(5,"Area Drills Options  Return");
            // add menu
            menu = new ScreenMenu("Mining System",200f,actionBar);
            //menu.AddLabel("test label");
            // offset
            menu.AddVariable("Forward","ForwardOffset","100");
            menu.AddVariable("Horizontal","HorizontalOffset","0");
            menu.AddVariable("Vertical","VerticalOffset","0");

            menu.AddVariable("Show Area", "ShowArea","false");
            menu.AddVariable("Drills On", "DrillsOn","false");
            menu.AddVariable("Drills Count", "DrillsCount","9");
            menu.Visible = false;
            progScreen.AddMenu(menu);
            // add toggle icon for drills
            progScreen.AddToggleIcon("Textures\\FactionLogo\\Empty.dds", "Textures\\FactionLogo\\Miners\\MinerIcon_3.dds", "DrillsOn",new Vector2(0,25),new Vector2(50,50),ScreenSprite.ScreenSpriteAnchor.CenterRight,TextAlignment.RIGHT);
            // add toggle icon for area
            progScreen.AddToggleIcon("Textures\\FactionLogo\\Empty.dds", "Textures\\FactionLogo\\Builders\\BuilderIcon_1.dds", "ShowArea",new Vector2(0,-25),new Vector2(50,50),ScreenSprite.ScreenSpriteAnchor.CenterRight,TextAlignment.RIGHT);
            //
            // setup runtime
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save()
        {
            // Save GridInfo
            Storage = GridInfo.Save();
        }
        public void Main(string argument, UpdateType updateSource)
        {
            if((updateSource == UpdateType.Update100))
            {
                progScreen.Draw();
            }
            else if(argument != "")
            {
                Me.CustomData = argument;
                string action = menu.HandleInput(argument);
                if(action == "back")
                {
                    menu.Visible = false;
                    menu.SelectedItem.Editing = false;
                    actionBar.SetActions("Area Drills Options  Return");
                }
                else if(action == "options")
                {
                    menu.Visible = true;
                    actionBar.SetActions("Up Down Select Back");
                }
                else if(action == "area")
                {
                    bool showArea = !GridInfo.GetVarAs<bool>("ShowArea");
                    GridInfo.SetVar("ShowArea",showArea.ToString());
                }
                else if(action == "drills")
                {
                    bool drillsOn = !GridInfo.GetVarAs<bool>("DrillsOn");
                    GridInfo.SetVar("DrillsOn",drillsOn.ToString());
                }
                progScreen.Draw();
            }
        }
    }
}
