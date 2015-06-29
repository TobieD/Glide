﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics;

namespace gameProject
{
   public class GameScene
    {
        #region Variables

        //Contains Current Screensize
        Viewport m_ViewPort;

       //Declare World Variables here
       Player m_Player;
       
       SaveData m_Data;
       string SaveDataFile = "Content/XML/SaveData.xml";

       //Score Calculation
       #region Score 
       Vector2 m_StartPos;
       Vector2 m_LastGeneratedPos;
       float m_DistanceTraveled;
       float m_TotalDistanceTraveled = 0;
       float m_UpdateTime = 0.0f;
       #endregion

       //Active/Lost
       public bool isActive = false;
       public bool hasLost = false;
        
       //Managers
       BackgroundManager m_BgManager;
       LevelManager m_LevelManager;

       public bool PlayerHit = false;
       public bool PickupHit = false;
       public GameSprite HitScreen;
       public GameSprite PickScreen;
       private float m_HitOpacity = 0f;
       private float m_PickOpacity = 0f;

       #if(DEBUG)
       //Debugging farseer Use F12 to toggle between enabled and disabled
       bool m_bEnableDebug = false;
       DebugViewXNA m_MonoDebug;
       #endif   

        #endregion


       public GameScene()
       {
           
       }

       public void Initialize(RenderContext context)
       {
           //Create new savedata file if it doesn't exist yet
           if (!File.Exists(SaveDataFile))
           {
               SaveData data = new SaveData();
               data.HighestDistance = 0;

               XmlWriter.SaveScore(data, SaveDataFile);
           }
           SaveScore(); // actually loads the score

           isActive = true;

           m_ViewPort = context.GraphicsDevice.Viewport;

           //Load a font
           TextRenderer.SetFont(context.Content.Load<SpriteFont>("Font/Standard"));

           //Create player an set him at center of screen
           m_StartPos = new Vector2(context.ViewPortSize.X * 0.25f, context.ViewPortSize.Y * 0.25f);
           m_Player = new Player(context, m_StartPos);
           m_Player.Initialize(context);
           m_Player.Translate(m_StartPos);

           context.Player = m_Player;

           //Generate a random level
           m_LevelManager = new LevelManager();
           m_LevelManager.Initialize(context);

           //AddParalaxing backgrounds
           m_BgManager = new BackgroundManager(context);
           m_BgManager.AddBackgound(new Background("background", new Vector2(0, 0), 1.0f, false, context));
           m_BgManager.AddBackgound(new Background("1", new Vector2(60, 20), 1.0f, false, context));
           m_BgManager.AddBackgound(new Background("2", new Vector2(100, 70), 1, false, context));
           m_BgManager.AddBackgound(new Background("3", new Vector2(200, 40), 1, false, context));
           m_BgManager.AddBackgound(new Background("4", new Vector2(300, 50), 1, false, context));
           m_BgManager.AddBackgound(new Background("5", new Vector2(400, 200), 1, false, context));

           #if(DEBUG)
           m_MonoDebug = new DebugViewXNA(context.World);
           m_MonoDebug.AppendFlags(DebugViewFlags.DebugPanel);
           m_MonoDebug.DefaultShapeColor = Color.Red;
           m_MonoDebug.SleepingShapeColor = Color.LightGray;
           m_MonoDebug.LoadContent(context.GraphicsDevice, context.Content);
           #endif

           SoundManager.Play("Background", true);

           HitScreen = new GameSprite("Textures/GUI/hit", context);
           PickScreen = new GameSprite("Textures/GUI/pickupHit", context);
       }             

       public void Update(RenderContext context)
       {         
           #region Debug Enable/Disable
           #if(DEBUG)
           if (Keyboard.GetState().IsKeyDown(Keys.F12))
               m_bEnableDebug = !m_bEnableDebug;

           #endif
           #endregion

           m_UpdateTime += (float)context.GameTime.ElapsedGameTime.Milliseconds/100;
           if (m_UpdateTime > 5.0f && m_Player.Health > 0) // only increases score if player has more than 0 lives
           {
               m_UpdateTime = 0.0f;
               m_TotalDistanceTraveled = m_Player.Position.X - m_StartPos.X;
           }

          
           // Issue With FPS loss when adding new levels because of loading them again
           #region Generate Pieces ( CURRENTLY NOT WORKING)
           m_DistanceTraveled = (int)(m_Player.Position.X - m_LastGeneratedPos.X);
           if (m_DistanceTraveled > 500)
           {
               //m_LevelManager.Generate(5, context);               
               //Still a really tiny delay tho
               m_DistanceTraveled = 0;
               m_LastGeneratedPos = m_Player.Position;
           }
           #endregion           

           //General Updates of GameObjects
           m_LevelManager.Update(context);
           m_BgManager.Update(context, new Vector2(1, 0f));
           m_Player.Update(context);

           context.Player = m_Player;
           context.Camera.Update(context, m_Player.Position);


           //End The Game and show a restatrt button
           if (m_Player.Health <= 0)
           {      
               hasLost = true;
               m_Player.AllowInput = false;
               
           }                    

           //Go to lose screen
           
       }
        
       void Draw2D(RenderContext context, bool before3D)
       {
           //DRAW SPRITES BEFORE THE 3D
           //*************************
           if (before3D)
           {
               //Parallaxing  
               m_BgManager.DrawBefore(context);
               m_LevelManager.Draw2D(context);

               //Hit overlays
               if (PlayerHit)
               {
                   m_HitOpacity = 1;
                   PlayerHit = false;
               }
               HitScreen.Scale(context.ViewPortSize.X / HitScreen.Width, context.ViewPortSize.Y / HitScreen.Height);
               if (HitScreen != null) HitScreen.DrawAsGUI(context, m_HitOpacity);
               if (m_HitOpacity > 0)
               {
                   m_HitOpacity -= 0.01f;           
               }
               if (m_HitOpacity < 0)
               {
                   m_HitOpacity = 0;
               }

               if (PickupHit)
               {
                   m_PickOpacity = 1;
                   PickupHit = false;
               }
               PickScreen.Scale(context.ViewPortSize.X / HitScreen.Width, context.ViewPortSize.Y / HitScreen.Height);
               if (PickScreen != null) PickScreen.DrawAsGUI(context, m_PickOpacity);
               if (m_PickOpacity > 0)
                   m_PickOpacity -= 0.01f;
               if (m_PickOpacity < 0)
                   m_PickOpacity = 0;
             
               //Use For text               
               TextRenderer.DrawText("Distance Traveled: " + (int)(m_TotalDistanceTraveled * 0.02f)
                   + "\nHighest score: " + m_Data.HighestDistance, m_ViewPort.Width * 0.45f, m_ViewPort.Height * 0.02f, Color.White, context);

               #if(DEBUG)
               if (m_bEnableDebug)
               {
                   m_MonoDebug.RenderDebugData(context.Camera.Projection, context.Camera.View);
                   m_Player.DisplaySettings(context);
               }
               #endif
           }

           else if (!before3D)
           {
               m_BgManager.DrawAfter(context);

           }

       }

       void Draw3D(RenderContext context)
       {
           //Use for models
           context.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

           m_Player.Draw(context);
           m_LevelManager.Draw3D(context);

       }

        //Don't change anything here
       public void Draw(RenderContext context)
       {       
           Draw2D(context, false);
           Draw3D(context);

           Draw2D(context, true);
       }

       //Destroys all objects in the scene
       public void CleanUp()
       {
           if (isActive == true)
           {
               m_LevelManager.ClearLevel();

               m_Player.RigidBody.Dispose();
               m_Player = null;                              
               m_LevelManager = null;
               m_DistanceTraveled = 0;

               isActive = false;
               hasLost = false;

               //Stop Music
               //SoundManager.Stop("BackgroundGame");

               GC.Collect();

           }
       
       }

       public void SaveScore()
       {
           m_Data = XmlLoader.Load<SaveData>("SaveData");
           if (m_TotalDistanceTraveled >= m_Data.HighestDistance)
           {
               m_Data.HighestDistance = (int)(m_TotalDistanceTraveled * 0.02f);
           }

           XmlWriter.SaveScore(m_Data, SaveDataFile);
       }
    }
}
