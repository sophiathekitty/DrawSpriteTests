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
        //-----------------------------------------//
        // Grid Info                               //
        // holds some basic info about the grid    //
        // and other useful stuff to have globally //
        //-----------------------------------------//
        public class GridInfo
        {
            public static long RunCount = 0; // to store how many times the script has run since compiling
            public static IMyGridTerminalSystem GridTerminalSystem; // so it can be globally available
            public static IMyIntergridCommunicationSystem IGC; // so it can be globally available
            public static IMyProgrammableBlock Me; // so it can be globally available... lol
            public static Action<string> Echo; // Echo?.Invoke("hello");
            private static string bound_vars = ""; // a list of vars that have been bound to the grid
            private static string broadcast_vars = ""; // a list of vars that have been bound to the grid
            private static Dictionary<string,long> unicast_vars = new Dictionary<string, long>(); // a list of vars that have been bound to the grid
            public static void EchoInvoke(string message)
            {
                Echo?.Invoke(message);
            }
            public static Dictionary<string, string> GridVars = new Dictionary<string, string>();
            //-------------------------------------------//
            // Get a var as a specific type of variable  //
            //                                           //
            // key - the id of the variable to get       //
            // defaultValue - the value to return if     //
            //                the variable doesn't exist //
            //-------------------------------------------//    
            public static T GetVarAs<T>(string key, T defaultValue = default(T))
            {
                if (!GridVars.ContainsKey(key)) return defaultValue; //(T)Convert.ChangeType(null,typeof(T));
                return (T)Convert.ChangeType(GridVars[key], typeof(T));
            }
            //-------------------------------------------//
            // set a grid info var                       //
            //                                           //
            // key - the id of the variable to set       //
            // value - the value (converted to a string) //
            //-------------------------------------------//
            public static void SetVar(string key, string value)
            {
                if (GridVars.ContainsKey(key)) GridVars[key] = value;
                else GridVars.Add(key, value);
                if(bound_vars.Contains(key + "║")) OnVarChanged(key, value);
            }
            //------------------------------------------------------------//
            // converts the grid info vars to a string to save in Storage //
            //------------------------------------------------------------//
            public static string Save()
            {
                StringBuilder storage = new StringBuilder();
                foreach (KeyValuePair<string, string> var in GridVars)
                {
                    storage.Append(var.Key + "║" + var.Value + "\n");
                }
                return storage.ToString();
            }
            //----------------------------------------------//
            // parse the Storage string into grid info vars //
            //----------------------------------------------//
            public static void Load(string storage)
            {
                string[] lines = storage.Split('\n');
                foreach (string line in lines)
                {
                    string[] var = line.Split('║');
                    if (var.Length == 2)
                    {
                        GridVars.Add(var[0], var[1]);
                    }
                }
            }
            //----------------------------------//
            // event for when a var is changed  //
            //----------------------------------//
            public static event Action<string,string> VarChanged;
            private static void OnVarChanged(string key, string value)
            {
                VarChanged?.Invoke(key,value);
                if (broadcast_vars.Contains(key + "║")) IGC.SendBroadcastMessage(key, value);
                if (unicast_vars.ContainsKey(key)) IGC.SendUnicastMessage(unicast_vars[key], key, value);
            }
            public static void AddChangeListener(string key, Action<string,string> handler)
            {
                bound_vars += key + "║";
                VarChanged += handler;
            }
            public static void AddChangeBroadcaster(string key)
            {
                broadcast_vars += key + "║";
            }
            public static void AddChangeUnicaster(string key, long id)
            {
                unicast_vars.Add(key, id);
            }
            //----------------------------------//
            // the world position for the block //
            //----------------------------------//
            public static Vector3D BlockWorldPosition(IMyFunctionalBlock block, Vector3D offset =  new Vector3D())
            {
                return Vector3D.Transform(offset, block.WorldMatrix);
            }
            

        }

    }
}
