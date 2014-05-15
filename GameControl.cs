using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;

namespace HuntTheWumpus
{
    class GameControl
    {
        RenderWindow win;
        List<Node> Nodes = new List<Node>();
        
        static Vector2f offset = new Vector2f(-50, 50);
        static Random r = new Random(DateTime.Now.Second);

        int ActiveIndex = 0;

        public GameControl(RenderWindow win_)
        {
            win = win_;

            for (int i = 0; i < 30; i++) // Create Nodes
            {
                Node n = new Node(i);
                n.Position = new Vector2f((i % 6 + 1) * 120, (float)Math.Floor(i / 6.0) * 120 + (i % 2) * 60);
                Nodes.Add(n);
            }

            for (int i = 0; i < Nodes.Count; i++) // Create Neighbors
            {
                Nodes[i].Neighbors.Add(Nodes[i / 6 == 0 ? i + 24 : i - 6]);
                Nodes[i].Neighbors.Add(Nodes[i / 6 == 4 ? i - 24 : i + 6]);
                Nodes[i].Neighbors.Add(Nodes[i - 1 < 0 ? Nodes.Count - 1 : i - 1]);
                Nodes[i].Neighbors.Add(Nodes[i + 1 >= Nodes.Count ? 0 : i + 1]);

                if (i % 2 == 0 & i % 6 != 0)
                    Nodes[i].Neighbors.Add(Nodes[i < 7 ? i + 23 : i - 7]);
                else
                    Nodes[i].Neighbors.Add(Nodes[i >= 25 ? i - 25 : i + 5]);

                if (i % 2 == 0 | i % 6 == 5)
                    Nodes[i].Neighbors.Add(Nodes[i < 5 ? i + 25 : i - 5]);
                else
                    Nodes[i].Neighbors.Add(Nodes[i >= 23 ? i - 23 : i + 7]);
            }

            foreach (var i in Nodes)
                i.GenerateRandomConnections();

            SetActive(r.Next(0, 29));
        }

        void SetActive(int id)
        {
            Nodes[ActiveIndex].Active = false;
            Nodes[id].Active = true;
            ActiveIndex = id;
        }

        public void Update(double dt)
        {
            foreach (var n in Nodes[ActiveIndex].Connections)
            {
                Vector2f distanceVector = new Vector2f(n.Position.X - Mouse.GetPosition(win).X - 50, n.Position.Y - Mouse.GetPosition(win).Y + 50);
                
                if (n.Radius * n.Radius > distanceVector.X * distanceVector.X + distanceVector.Y * distanceVector.Y)
                {
                    n.OutlineThickness = 2;

                    if (Mouse.IsButtonPressed(Mouse.Button.Left))
                        SetActive(n.Id);
                }
                else
                {
                    n.OutlineThickness = 0;
                }
            }
        }

        public void Draw()
        {
            foreach (var i in Nodes)
                win.Draw(i);

            for (int i = 0; i < Nodes.Count; i++)
            {
                foreach (var j in Nodes[i].Connections)
                {
                    if (j.Enabled || Nodes[i].Enabled)
                    {
                        Vector2f oPoint = new Vector2f(
                            i % 6 == 0 & j.Id % 6 == 5 ? -720 : (i % 6 == 5 & j.Id % 6 == 0 ? 720 : 0),
                            i / 6 == 0 & j.Id / 6 == 4 ? -600 : (i / 6 == 4 & j.Id / 6 == 0 ? 600 : 0));

                        /*Vector2f oRect = oPoint - Nodes[i].Position;
                        float oRectLength = (float)Math.Sqrt((double)(oRect.X * oRect.X + oRect.Y * oRect.Y));
                        float ty = oRect.Y;
                        oRect.Y = oRect.X;
                        oRect.X = -ty;
                        oRect.X /= oRectLength;
                        oRect.Y /= oRectLength;*/

                        Vertex[] ar = new Vertex[2];
                        ar[0] = new Vertex(Nodes[i].Position + offset /*+ oRect*/);
                        //ar[1] = new Vertex(Nodes[i].Position + offset - oRect);
                        ar[1] = new Vertex(j.Position + oPoint + offset /*- oRect*/);
                        //ar[3] = new Vertex(j.Position + oPoint + offset + oRect);
                        win.Draw(ar, PrimitiveType.Lines);
                    }
                }
            }
        }
    }
}
