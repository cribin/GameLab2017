using ColonizingBastards.Base.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Cameras;
using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.GameClasses.Match.MatchObjects.Animals;

namespace ColonizingBastards.Base.Objects
{
	class Actor : Entity
	{
		// Graphical representation of this actor
		protected RenderObject rep;

        // Position of this actor
        protected Vector3 position;
        // Rotation of this actor
        protected Vector3 rotation;
        // Size of this actor
        protected Vector3 size;

		public Actor(RenderObject rep) : base()
		{
            this.rep = rep;
		}

        public void SetPosition(Vector3 position)
        {
            this.position = position;
            if (rep != null)
            {
                rep.SetPosition(position);
            }
        }

	    public void SetPosition(Vector2 position)
	    {
	        this.position.X = position.X;
	        this.position.Y = position.Y;
            if(rep != null)
                rep.SetPosition(this.position);
	    }

        public void SetRotation(Vector3 rotation)
        {
            this.rotation = rotation;
            if (rep != null)
            {
                rep.SetRotation(rotation);
            }
        }

        public void SetSize(Vector3 size)
        {
            this.size = size;
            if (rep != null)
            {
                rep.SetSize(size);
            }
        }

		public Vector3 GetPosition()
		{
			return position;
		}

	    public Vector3 GetGroundPosition()
	    {
	        float x = position.X + 0.5f * size.X;
	        float y = position.Y + 1f * size.Y;
	        float z = position.Z + 0.5f * size.Z;

            return new Vector3(x, y, z);
	    }

	    public Vector3 GetCenterPosition()
	    {
	        return position + (0.5f * size);
	    }

		public Vector3 GetRotation()
		{
			return rotation;
		}

		public Vector3 GetSize()
		{
			return size;
		}

		public virtual void Draw(SpriteBatch batch)
		{
            rep.Draw(batch);
		}

        public virtual void Draw(SpriteBatch batch, Camera camera)
        {
            rep.Draw(batch);
        }

        public virtual void Draw(SpriteBatch batch, Color color)
        {
            rep.Draw(batch);
        }

        public override void Update(GameTime gameTime)
        {
			rep.Update(gameTime);
        }

		public RenderObject getRepresentation()
		{
			return rep;
		}

	}
}
