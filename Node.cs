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
                n.AddConnection(this);
            }
        }

        public void GenerateRandomConnections()
        {
            List<int> takenKeys = new List<int>();

            for (int i = 0; i < Neighbors.Count; i++)
                if (Connections.Contains(Neighbors[i]))
                    takenKeys.Add(i);

            while (Connections.Count < 3)
            {
                int q = 0;
                do q++; //= r.Next(Neighbors.Count); // Random generation is really slow right now
                while (Neighbors[q].Connections.Count >= 3 || takenKeys.Contains(q));

                takenKeys.Add(q);
                AddConnection(Neighbors[q]);
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
                    OutlineThickness = 1;
                    Enabled = true;
                }
                else
                    OutlineThickness = 0;
            }
        }
    }
}
