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
        // Screen - encapsulates a drawing surface
        //----------------------------------------------------------------------
        public class Screen
        {
            private IMyTextSurface _drawingSurface;
            private RectangleF _viewport;
            List<ScreenSprite> _sprites = new List<ScreenSprite>();
            //
            // constructor
            //
            public Screen(IMyTextSurface drawingSurface)
            {
                _drawingSurface = drawingSurface;
                _drawingSurface.ContentType = ContentType.SCRIPT;
                _drawingSurface.Script = "";
                // calculate the viewport offset by centering the surface size onto the texture size
                _viewport = new RectangleF(
                                       (_drawingSurface.TextureSize - _drawingSurface.SurfaceSize) / 2f,
                                                          _drawingSurface.SurfaceSize
                                                                         );
            }
            //
            // DrawSprites - draw sprites to the screen
            //
            public void Draw()
            {
                var frame = _drawingSurface.DrawFrame();
                DrawSprites(ref frame);
                frame.Dispose();
            }
            //
            // DrawSprites - draw sprites to the screen
            //
            private void DrawSprites(ref MySpriteDrawFrame frame)
            {
                // draw all the sprites
                foreach (ScreenSprite sprite in _sprites)
                {
                    if(sprite.Visible) frame.Add(sprite.ToMySprite(_viewport));
                }
            }
            // add a text sprite
            public ScreenSprite AddTextSprite(ScreenSprite.ScreenSpriteAnchor anchor, Vector2 position, float rotationOrScale, Color color, string fontId, string data, TextAlignment alignment)
            {
                ScreenSprite sprite = new ScreenSprite(anchor, position, rotationOrScale, new Vector2(0, 0), color, fontId, data, alignment, SpriteType.TEXT);
                _sprites.Add(sprite);
                return sprite;
            }
            // add a texture sprite
            public ScreenSprite AddTextureSprite(ScreenSprite.ScreenSpriteAnchor anchor, Vector2 position, float rotationOrScale, Vector2 size, Color color, string data)
            {
                ScreenSprite sprite = new ScreenSprite(anchor, position, rotationOrScale, size, color, "", data, TextAlignment.CENTER, SpriteType.TEXTURE);
                _sprites.Add(sprite);
                return sprite;
            }
            // add an Action Bar
            public ScreenActionBar AddActionBar(int actionCount, string actionString = "", float fontSize = 1f)
            {
                ScreenActionBar actionBar = new ScreenActionBar(actionCount, actionString);
                // calculate how much width each action has
                Vector2 actionWidth =  new Vector2(_viewport.Width / actionCount,0);
                Vector2 actionPosition = new Vector2(actionWidth.X/2f,-30*fontSize);
                for (int i = 0; i < actionCount; i++)
                {
                    ScreenSprite sprite = AddTextSprite(ScreenSprite.ScreenSpriteAnchor.BottomLeft,actionPosition, fontSize, Color.White, "White", "", TextAlignment.CENTER);
                    actionPosition += actionWidth;
                    actionBar.AddSprite(sprite);
                    _sprites.Add(sprite);
                }
                return actionBar;
            }
            // add a menu
            public void AddMenu(ScreenMenu menu)
            {
                menu.AddSprites(ref _sprites);
            }
            public ScreenToggleIcon AddToggleIcon(string offIcon, string onIcon, string variableName, Vector2 position, Vector2 size, ScreenSprite.ScreenSpriteAnchor anchor = ScreenSprite.ScreenSpriteAnchor.TopLeft, TextAlignment alignment = TextAlignment.LEFT)
            {
                ScreenToggleIcon toggleIcon = new ScreenToggleIcon(offIcon, onIcon, variableName, position, size, anchor, alignment);
                _sprites.Add(toggleIcon.Sprite);
                return toggleIcon;
            }
        }
        //----------------------------------------------------------------------
        // screen sprite - encapsulates a sprite
        //----------------------------------------------------------------------
        public class ScreenSprite
        {
            public enum ScreenSpriteAnchor
            {
                TopLeft,
                TopCenter,
                TopRight,
                CenterLeft,
                Center,
                CenterRight,
                BottomLeft,
                BottomCenter,
                BottomRight
            }
            public ScreenSpriteAnchor Anchor { get; set; }
            public Vector2 Position { get; set; }
            public float RotationOrScale { get; set; } 
            public Vector2 Size { get; set; }
            public Color Color { get; set; }
            public string FontId { get; set; }
            public string Data { get; set; }
            public TextAlignment Alignment { get; set; }
            public SpriteType Type { get; set; }
            public bool Visible { get; set; } = true;
            public ScreenSprite()
            {
                Anchor = ScreenSpriteAnchor.Center;
                Position = Vector2.Zero;
                RotationOrScale = 1f;
                Size = Vector2.Zero;
                Color = Color.White;
                FontId = "White";
                Data = "";
                Alignment = TextAlignment.CENTER;
                Type = SpriteType.TEXT;
            }
            public ScreenSprite(ScreenSpriteAnchor anchor, Vector2 position, float rotationOrScale, Vector2 size, Color color, string fontId, string data, TextAlignment alignment, SpriteType type)
            {
                Anchor = anchor;
                Position = position;
                RotationOrScale = rotationOrScale;
                Size = size;
                Color = color;
                FontId = fontId;
                Data = data;
                Alignment = alignment;
                Type = type;
            }
            // convert the sprite to a MySprite
            public MySprite ToMySprite(RectangleF _viewport)
            {
                if(Type == SpriteType.TEXT)
                {
                    return new MySprite()
                    {
                        Type = Type,
                        Data = Data,
                        Position = GetPosition(_viewport),
                        RotationOrScale = RotationOrScale,
                        Color = Color,
                        Alignment = Alignment,
                        FontId = FontId
                    };
                }
                return new MySprite()
                {
                    Type = Type,
                    Data = Data,
                    Position = GetPosition(_viewport),
                    RotationOrScale = RotationOrScale,
                    Color = Color,
                    Alignment = Alignment,
                    Size = Size,
                    FontId = FontId
                };
            }
            private Vector2 GetPosition(RectangleF _viewport)
            {
                Vector2 _position = Position + _viewport.Position;
                switch (Anchor)
                {
                    case ScreenSpriteAnchor.TopCenter:
                        _position = Position + new Vector2(_viewport.Center.X, _viewport.Y);
                        break;
                    case ScreenSpriteAnchor.TopRight:
                        _position = Position + new Vector2(_viewport.Right, _viewport.Y);
                        break;
                    case ScreenSpriteAnchor.CenterLeft:
                        _position = Position + new Vector2(_viewport.X, _viewport.Center.Y);
                        break;
                    case ScreenSpriteAnchor.Center:
                        _position = Position + _viewport.Center;
                        break;
                    case ScreenSpriteAnchor.CenterRight:
                        _position = Position + new Vector2(_viewport.Right, _viewport.Center.Y);
                        break;
                    case ScreenSpriteAnchor.BottomLeft:
                        _position = Position + new Vector2(_viewport.X, _viewport.Bottom);
                        break;
                    case ScreenSpriteAnchor.BottomCenter:
                        _position = Position + new Vector2(_viewport.Center.X, _viewport.Bottom);
                        break;
                    case ScreenSpriteAnchor.BottomRight:
                        _position = Position + new Vector2(_viewport.Right, _viewport.Bottom);
                        break;
                }
                return _position;
            }
            
        }
    }
}
