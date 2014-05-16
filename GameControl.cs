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

        CircleShape player = new CircleShape(30), backupPlayer;

        int ActiveIndex = 0;

        TweenVector2f playerTween, backupTween;

        public GameControl(RenderWindow win_)
        {
            win = win_;

            GenerateMap();

            SetActive(r.Next(0, 29));
            Nodes[ActiveIndex].Specialty = SpecialNode.PlayerSpawn;

            GenerateSpecialies();

            player.FillColor = Color.Blue;
            player.OutlineColor = Color.White;
            player.OutlineThickness = 2;
            player.SetPointCount(5);
            player.Position = Nodes[ActiveIndex].Position + new Vector2f(-80, 20);
            backupPlayer = new CircleShape(player);
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
                Vector2f dv = new Vector2f(n.Position.X - Mouse.GetPosition(win).X - 50, n.Position.Y - Mouse.GetPosition(win).Y + 50);
                
                if (n.Radius * n.Radius > dv.X * dv.X + dv.Y * dv.Y)
                {
                    n.OutlineThickness = 2;

                    if (Mouse.IsButtonPressed(Mouse.Button.Left))
                    {
                        if (backupTween.Active)
                            backupTween.Active = false;

                        backupPlayer.Position = player.Position;
                        Vector2f backupPlayerTarget = n.Position + new Vector2f(-80, 20);

                        if (ActiveIndex % 6 == 0 && n.Id % 6 == 5)
                        {
                            backupPlayerTarget -= new Vector2f(740, 0);
                            player.Position += new Vector2f(740, 0);
                        }
                        else if (ActiveIndex % 6 == 5 && n.Id % 6 == 0)
                        {
                            backupPlayerTarget += new Vector2f(740, 0);
                            player.Position -= new Vector2f(740, 0);
                        }

                        if (ActiveIndex / 6 == 0 && n.Id / 6 == 4)
                        {
                            backupPlayerTarget -= new Vector2f(0, 600);
                            player.Position += new Vector2f(0, 600);
                        }
                        else if (ActiveIndex / 6 == 4 && n.Id / 6 == 0)
                        {
                            backupPlayerTarget += new Vector2f(0, 600);
                            player.Position -= new Vector2f(0, 600);
                        }

                        if (player.Position.X != backupPlayer.Position.X || player.Position.Y != backupPlayer.Position.Y)
                            backupTween = new TweenVector2f(backupPlayer.Position, backupPlayerTarget, 1);

                        playerTween = new TweenVector2f(player.Position, n.Position + new Vector2f(-80, 20), 1);
                        SetActive(n.Id);
                    }
                }
                else
                    n.OutlineThickness = 0;
            }

            playerTween.Update(ref player, (float)dt);
            backupTween.Update(ref backupPlayer, (float)dt);
        }

        public void Draw()
        {
            foreach (var i in Nodes)
            {
                win.Draw(i);

                if (i.Enabled)
                {
                    CircleShape c = new CircleShape(20);
                    c.OutlineThickness = 2;
                    c.OutlineColor = Color.White;
                    c.Position = i.Position + new Vector2f(-70, 30);

                    switch (i.Specialty)
                    {
                        case (SpecialNode.Hole):
                            c.FillColor = Color.Black;
                            win.Draw(c);
                            break;

                        case (SpecialNode.Bats):
                            c.FillColor = Color.Magenta;
                            win.Draw(c);
                            break;

                        case (SpecialNode.Coin):
                            c.FillColor = Color.Yellow;
                            win.Draw(c);
                            break;

                        case (SpecialNode.Arrow):
                            c.FillColor = Color.Cyan;
                            win.Draw(c);
                            break;

                        case (SpecialNode.Wumpus):
                            c.FillColor = Color.Red;
                            win.Draw(c);
                            break;
                    }
                }
            }

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

                        if (j.Active || Nodes[i].Active)
                        {
                            ar[0] = new Vertex(Nodes[i].Position + offset, Color.Blue);
                            ar[1] = new Vertex(j.Position + oPoint + offset, Color.Blue);
                        }
                        else
                        {
                            ar[0] = new Vertex(Nodes[i].Position + offset);
                            ar[1] = new Vertex(j.Position + oPoint + offset);
                        }

                        win.Draw(ar, PrimitiveType.Lines);
                    }
                }
            }

            win.Draw(player);

            if (backupTween.Active)
                win.Draw(backupPlayer);
        }

        void GenerateMap()
        {
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
        }

        void GenerateSpecialies()
        {
            int w;
            ushort numberOfHoles = 3;
            ushort numberOfBats = 3;
            ushort numberOfCoins = 8;
            ushort numberOfArrows = 5;
            List<Node> _nodes = new List<Node>(Nodes);

            do
                w = r.Next(0, _nodes.Count - 1);
            while (Nodes[_nodes[w].Id].Specialty != SpecialNode.None);

            Nodes[_nodes[w].Id].Specialty = SpecialNode.Wumpus;
            _nodes.RemoveAt(w);

            while (numberOfHoles > 0)
            {
                do
                    w = r.Next(0, _nodes.Count - 1);
                while (Nodes[_nodes[w].Id].Specialty != SpecialNode.None);

                Nodes[_nodes[w].Id].Specialty = SpecialNode.Hole;
                _nodes.RemoveAt(w);
                numberOfHoles--;
            }
            
            while (numberOfBats > 0)
            {
                do
                    w = r.Next(0, _nodes.Count - 1);
                while (Nodes[_nodes[w].Id].Specialty != SpecialNode.None);

                Nodes[_nodes[w].Id].Specialty = SpecialNode.Bats;
                _nodes.RemoveAt(w);
                numberOfBats--;
            }

            while (numberOfArrows > 0)
            {
                do
                    w = r.Next(0, _nodes.Count - 1);
                while (Nodes[_nodes[w].Id].Specialty != SpecialNode.None);

                Nodes[_nodes[w].Id].Specialty = SpecialNode.Arrow;
                _nodes.RemoveAt(w);
                numberOfArrows--;
            }

            while (numberOfCoins > 0)
            {
                do
                    w = r.Next(0, _nodes.Count - 1);
                while(Nodes[_nodes[w].Id].Specialty != SpecialNode.None);

                Nodes[_nodes[w].Id].Specialty = SpecialNode.Coin;
                _nodes.RemoveAt(w);
                numberOfCoins--;
            }
        }
    }
}
