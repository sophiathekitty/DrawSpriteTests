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
        // PixelIcon
        //----------------------------------------------------------------------
        public class PixelIcon : ScreenSprite
        {
            Vector2 _size;
            Vector2 _addPosition = Vector2.Zero;
            string onIcon = "";
            string offIcon = "";
            List<string> variableIcons = new List<string>();
            bool _state = false;
            int _variableIndex = 0;
            string _variableName = "";
            public bool State
            {
                get { return _state; }
                set
                {
                    _state = value;
                    Data = _state ? onIcon : offIcon;
                }
            }
            public int Index
            {
                get { return _variableIndex; }
                set
                {
                    _variableIndex = value;
                    if(_variableIndex < 0)
                    {
                        _variableIndex = 0;
                    }
                    else if (_variableIndex >= variableIcons.Count)
                    {
                        _variableIndex = variableIcons.Count - 1;
                    }
                    Data = variableIcons[_variableIndex];
                }
            }
            // static pixel icon
            public PixelIcon(Vector2 position, string data, Color color, Vector2 size, float scale = 0.01f, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft, TextAlignment alignment = TextAlignment.LEFT)
                : base(anchor, position,scale, Vector2.Zero, color,"Monospace", data, alignment, SpriteType.TEXT)
            {
                _size = size;
            }
            // toggle pixel icon
            public PixelIcon(Vector2 position, string offIcon, string onIcon, string variableName, Color color, Vector2 size, float scale = 0.01f, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft, TextAlignment alignment = TextAlignment.LEFT)
                : base(anchor, position, scale, Vector2.Zero, color, "Monospace", offIcon, alignment, SpriteType.TEXT)
            {
                _size = size;
                this.onIcon = onIcon;
                this.offIcon = offIcon;
                _variableName = variableName;
                GridInfo.AddChangeListener(_variableName, OnBoolChanged);
                State = GridInfo.GetVarAs<bool>(_variableName);
            }
            // variable pixel icon
            public PixelIcon(Vector2 position, List<string> variableIcons, string variableName, Color color, Vector2 size, float scale = 0.01f, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft, TextAlignment alignment = TextAlignment.LEFT)
                : base(anchor, position, scale, Vector2.Zero, color, "Monospace", variableIcons[0], alignment, SpriteType.TEXT)
            {
                _size = size;
                this.variableIcons = variableIcons;
                _variableName = variableName;
                GridInfo.AddChangeListener(_variableName, OnIntChanged);
                Index = GridInfo.GetVarAs<int>(_variableName);
            }
            void OnBoolChanged(string key, string value)
            {
                bool newState = GridInfo.GetVarAs<bool>(_variableName);
                if (newState != State)
                {
                    State = newState;
                }
            }
            void OnIntChanged(string key, string value) {
                int newIndex = GridInfo.GetVarAs<int>(_variableName);
                if (newIndex != Index)
                {
                    Index = newIndex;
                }
            }
            // bytes between 0 - 7
            // usage: PixelIcon.rgb(0, 0, 7);
            public static char rgb(byte r, byte g, byte b)
            {
                return (char)(0xE100 + (r << 6) + (g << 3) + b);
            }
            // remap an int from 0-255 to a byte from 0-7
            public static byte remap(int value)
            {
                if (value < 0) return 0;
                if (value > 255) return 7;
                return (byte)(value / 32);
            }
            //
            // draw functions
            //
            // add a pixel to the icon
            // ints between 0 - 255
            public void addPixelRGB(int r, int g, int b)
            {
                if(_addPosition.Y >= _size.Y) return;
                Data += rgb(remap(r), remap(g), remap(b));
                _addPosition.X += 1f;
                if (_addPosition.X >= _size.X)
                {
                    _addPosition.X = 0f;
                    _addPosition.Y += 1f;
                    Data += "\n";
                }
            }
            // fill the icon with a color
            // ints between 0 - 255
            public void fillRGB(int r, int g, int b)
            {
                Data = "";
                for (int y = 0; y < _size.Y; y++)
                {
                    for (int x = 0; x < _size.X; x++)
                    {
                        addPixelRGB(r, g, b);
                    }
                }
            }
            public void fillRGB(Color color)
            {
                fillRGB(color.R, color.G, color.B);
            }
            // set a pixel to a color at a position
            // ints between 0 - 255
            public void setPixelRGB(int x, int y, int r, int g, int b)
            {
                if (x < 0 || x >= _size.X || y < 0 || y >= _size.Y) return;
                Data = Data.Remove((int)(y * (_size.X + 1) + x), 1);
                Data = Data.Insert((int)(y * (_size.X + 1) + x), rgb(remap(r), remap(g), remap(b)).ToString());
            }
            // draw a line from x1,y1 to x2,y2
            // ints between 0 - 255
            public void drawLineRGB(int x1, int y1, int x2, int y2, int r, int g, int b)
            {
                int dx = Math.Abs(x2 - x1);
                int dy = Math.Abs(y2 - y1);
                int sx = (x1 < x2) ? 1 : -1;
                int sy = (y1 < y2) ? 1 : -1;
                int err = dx - dy;
                while (true)
                {
                    setPixelRGB(x1, y1, r, g, b);
                    if ((x1 == x2) && (y1 == y2)) break;
                    int e2 = 2 * err;
                    if (e2 > -dy)
                    {
                        err -= dy;
                        x1 += sx;
                    }
                    if (e2 < dx)
                    {
                        err += dx;
                        y1 += sy;
                    }
                }
            }
            public void drawLineRGB(Vector2 start, Vector2 end, Color color)
            {
                drawLineRGB((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, color.R, color.G, color.B);
            }
            // draw a rectangle from x1,y1 to x2,y2
            // ints between 0 - 255
            public void drawRectRGB(int x1, int y1, int x2, int y2, int r, int g, int b)
            {
                drawLineRGB(x1, y1, x2, y1, r, g, b);
                drawLineRGB(x2, y1, x2, y2, r, g, b);
                drawLineRGB(x2, y2, x1, y2, r, g, b);
                drawLineRGB(x1, y2, x1, y1, r, g, b);
            }
            public void drawRectRGB(Vector2 start, Vector2 end, Color color)
            {
                drawRectRGB((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, color.R, color.G, color.B);
            }
            // fill a rectangle from x1,y1 to x2,y2
            // ints between 0 - 255
            public void fillRectRGB(int x1, int y1, int x2, int y2, int r, int g, int b)
            {
                for (int y = y1; y <= y2; y++)
                {
                    drawLineRGB(x1, y, x2, y, r, g, b);
                }
            }
            public void fillRectRGB(Vector2 start, Vector2 end, Color color)
            {
                fillRectRGB((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, color.R, color.G, color.B);
            }
            // draw a circle at x,y with radius r
            // ints between 0 - 255
            public void drawCircleRGB(int x, int y, int r, int red, int green, int blue)
            {
                int f = 1 - r;
                int ddF_x = 1;
                int ddF_y = -2 * r;
                int x1 = 0;
                int y1 = r;
                setPixelRGB(x, y + r, red, green, blue);
                setPixelRGB(x, y - r, red, green, blue);
                setPixelRGB(x + r, y, red, green, blue);
                setPixelRGB(x - r, y, red, green, blue);
                while (x1 < y1)
                {
                    if (f >= 0)
                    {
                        y1--;
                        ddF_y += 2;
                        f += ddF_y;
                    }
                    x1++;
                    ddF_x += 2;
                    f += ddF_x;
                    setPixelRGB(x + x1, y + y1, red, green, blue);
                    setPixelRGB(x - x1, y + y1, red, green, blue);
                    setPixelRGB(x + x1, y - y1, red, green, blue);
                    setPixelRGB(x - x1, y - y1, red, green, blue);
                    setPixelRGB(x + y1, y + x1, red, green, blue);
                    setPixelRGB(x - y1, y + x1, red, green, blue);
                    setPixelRGB(x + y1, y - x1, red, green, blue);
                    setPixelRGB(x - y1, y - x1, red, green, blue);
                }
            }
            // fill a circle at x,y with radius r
            // ints between 0 - 255
            public void fillCircleRGB(int x, int y, int r, int red, int green, int blue)
            {
                for (int y1 = -r; y1 <= r; y1++)
                    for (int x1 = -r; x1 <= r; x1++)
                        if (x1 * x1 + y1 * y1 <= r * r)
                            setPixelRGB(x + x1, y + y1, red, green, blue);
            }
        }
        //----------------------------------------------------------------------
    }
}
