using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace HuntTheWumpus
{
    class Node : CircleShape
    {
        public int Id;
        public List<Node> Neighbors = new List<Node>();
        public List<Node> Connections = new List<Node>();
        static Random r = new Random(DateTime.Now.Second);
        bool enabled = false;
        bool active = false;
        ushort openConnections = 3;

        public Node(int id)
            : base(50)
        {
            Id = id;
            SetPointCount(6);
            Rotation = 90;
            Enabled = false;
            OutlineColor = Color.White;
        }

        public void AddConnection(Node n)
        {
            if (!Connections.Contains(n))
            {
                Connections.Add(n);
                openConnections--;
                n.AddConnection(this);
            }
        }

        public void GenerateRandomConnections()
        {
            while (Neighbors.Count > 0 && openConnections > 0)
            {
                var lookingAt = Neighbors[r.Next(0, Neighbors.Count - 1)];
                if (lookingAt.openConnections > 0)
                    AddConnection(lookingAt);
                Neighbors.Remove(lookingAt);
            }
        }

        public bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
                enabled = value;
                if (value)
                    FillColor = Color.Green;
                else
                    FillColor = new Color(100, 100, 100);
            }
        }

        public bool Active
        {
            get
            {
                return active;
            }

            set
            {
                active = value;
                if (value)
                {
                    OutlineThickness = 2;
                    OutlineColor = Color.Blue;
                    Enabled = true;
                }
                else
                {
                    OutlineThickness = 0;
                    OutlineColor = Color.White;
                }
            }
        }
    }
}
