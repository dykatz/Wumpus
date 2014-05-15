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

        CircleShape player = new CircleShape(30);

        int ActiveIndex = 0;

        TweenVector2f playerTween;

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

            player.FillColor = Color.Blue;
            player.OutlineColor = Color.White;
            player.OutlineThickness = 2;
            player.SetPointCount(5);
            player.Position = Nodes[ActiveIndex].Position + new Vector2f(-80, 20);
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
                    {
                        SetActive(n.Id);
                        playerTween = new TweenVector2f(player.Position, n.Position + new Vector2f(-80, 20), 0.5f);
                    }
                }
                else
                    n.OutlineThickness = 0;
            }

            if (playerTween != null)
                playerTween.Update(ref player,(float)dt);
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

                        Vertex[] ar = new Vertex[2];
                        ar[0] = new Vertex(Nodes[i].Position + offset);
                        ar[1] = new Vertex(j.Position + oPoint + offset);
                        win.Draw(ar, PrimitiveType.Lines);
                    }
                }
            }

            win.Draw(player);
        }
    }
}
