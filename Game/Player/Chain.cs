﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;


namespace gameProject
{
    public class Chain : GameModel
    {
        public Vector2 Size;
        public Vector2 Anchor;
        float m_Scale = 0f;

        public bool Active = true;
        public Chain(RenderContext context,float scale)
            : base("PlayerV2")
        {
            m_Scale = scale;
            RigidBody = BodyFactory.CreateRectangle(context.World, ConvertUnits.ToSimUnits(2), ConvertUnits.ToSimUnits(2), 1, Vector2.Zero);
            RigidBody.BodyType = BodyType.Dynamic;
            RigidBody.Mass = 0f;
            ObjectScale =  new Vector3(m_Scale,m_Scale,m_Scale);
            
            base.Name = "ChainPiece";
            RigidBody.OnCollision += RigidBody_OnCollision;
            RigidBody.UserData = this; //Add this class as data for easy access in collision
            RigidBody.Restitution = 0.0f;

            Scale(0.3f);
            Rotate(0, 90, 0);
            Depth = 0;

            Alpha = 0.95f;

            Active = true;
        }
        bool RigidBody_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            return false;
        }
        
        public void Update(float rotation, Vector2 position)
        {
            //RigidBody.LinearVelocity = new Vector2(Velocity.X /1.5f, Velocity.Y /1.5f);
            Position = position;
            Rotate(-rotation, 90f, 0f);
            RigidBody.Friction = 0f;
            base.Update();
        }
    }
};
