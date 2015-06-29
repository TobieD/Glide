﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gameProject
{
    public static class TextRenderer
    {
        static SpriteFont m_Font;

        public static void SetFont(SpriteFont font)
        {
            m_Font = font;
        }

       public static void DrawText(string text, float x, float y,Color color, RenderContext context)
        {
             var pos =  new Vector2(x,y);
             Matrix mat = context.Camera.View ;
             MathHelp.WorldToScreen(pos,context);

           // Matrix translation = Matrix.CreateTranslation(new Vector3(pos.X, pos.Y, 0));

           //Open and close the spritebach
            context.SpriteBatch.Begin();
            context.SpriteBatch.DrawString(m_Font, text, pos, color, 0, Vector2.Zero, 1, SpriteEffects.None, 1.0f);
            context.SpriteBatch.End();
        }   
    }
}
