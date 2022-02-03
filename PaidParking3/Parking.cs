﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaidParking3
{
    class Parking
    {
        public enum Sample
        {
            Road,
            TPS,
            CPS,
            Entry,
            Exit,
            TicketOffice,
            Lawn
        }
        Sample[,] topology;
        struct Vertice
        {
            int x;
            int y;
            public Sample s;
            public Vertice(int x, int y, Sample s)
            {
                this.x = x;
                this.y = y;
                this.s = s;
            }
        }

        public Parking(Sample[,] topology)
        {
            this.topology = topology;
        }

        public static Parking Validation(Sample[,] topology)
        {
            int entryX = -1, entryY = -1, ticketOfficeX = -1, ticketOfficeY = -1;
            bool isEntry = false, isExit = false, isTicketOffice = false;
            for (int i = 0; i < topology.GetLength(0); i++)
            {
                for (int j = 0; j < topology.GetLength(1); j++)
                {
                    if (topology[i, j] == Sample.Entry)
                    {
                        isEntry = true;
                        entryX = i;
                        entryY = j;
                        if (i != topology.GetLength(0) - 1)
                        {
                            return null;
                        }
                    }
                    if (topology[i, j] == Sample.TicketOffice)
                    {
                        isTicketOffice = true;
                        ticketOfficeX = i;
                        ticketOfficeY = j;
                    }
                    if (topology[i, j] == Sample.Exit)
                    {
                        isExit = true;
                        if (i != topology.GetLength(0) - 1)
                        {
                            return null;
                        }
                    }
                }
            }
            if (!(isExit && isEntry && isTicketOffice))
            {
                return null;
            }
            if (!(Math.Abs(entryY - ticketOfficeY) == 1 && entryX == ticketOfficeX || Math.Abs(entryX - ticketOfficeX) == 1 && entryY == ticketOfficeY))
            {
                return null;
            }
            if (!DijkstrasAlgorithm(topology))
            {
                return null;
            }
            return new Parking(topology);
        }

        private static bool DijkstrasAlgorithm(Sample[,] topology)
        {
            List<Vertice> vertices = new List<Vertice>();
            List<List<Vertice>> adjacencyList = new List<List<Vertice>>();
            GraphFormation(topology, out vertices, out adjacencyList);

            int[] d = new int[vertices.Count];
            bool[] u = new bool[vertices.Count];
            for (int k = 0; k < 1; k++)
            {
                for (int i = 0; i < vertices.Count; i++)
                {
                    if (k == 0 && vertices[i].s == Sample.Entry)
                        d[i] = 0;
                    else if (k == 1 && vertices[i].s == Sample.Exit)
                        d[i] = 0;
                    else
                        d[i] = int.MaxValue;
                    u[i] = false;
                }
                for (int i = 0; i < vertices.Count; i++)
                {
                    int min = d.Where((elem, idx) => u[idx] == false).Min();
                    int minIndex = Array.IndexOf(d, min);
                    u[minIndex] = true;
                    for (int j = 0; j < adjacencyList[minIndex].Count; j++)
                    {
                        int idx = vertices.FindIndex(v => v.Equals(adjacencyList[minIndex][j]));
                        d[idx] = Math.Min(d[idx], d[minIndex] + 1);
                    }
                }
                if (d.Contains(int.MaxValue))
                    return false;
            }

            return true;
        }

        private static void GraphFormation(Sample[,] topology, out List<Vertice> vertices, out List<List<Vertice>> adjacencyList)
        {
            vertices = new List<Vertice>();
            adjacencyList = new List<List<Vertice>>();
            for (int i = 0; i < topology.GetLength(0); i++)
            {
                for (int j = 0; j < topology.GetLength(1); j++)
                {
                    if (topology[i, j] == Sample.Road)
                    {
                        vertices.Add(new Vertice(i, j, topology[i, j]));
                        adjacencyList.Add(getAdjacentVertices(i, j, topology));
                    }
                    else if (topology[i, j] == Sample.TPS)
                    {
                        if (i != 0 && topology[i - 1, j] == Sample.TPS)
                        {
                            int idx = vertices.FindIndex(pair => pair.Equals(new Vertice(i - 1, j, Sample.TPS)));
                            adjacencyList[idx].AddRange(getAdjacentVertices(i, j, topology));
                        }
                        else
                        {
                            vertices.Add(new Vertice(i, j, topology[i, j]));
                            adjacencyList.Add(getAdjacentVertices(i, j, topology));
                        }
                    }
                    else if (topology[i, j] == Sample.CPS)
                    {
                        vertices.Add(new Vertice(i, j, topology[i, j]));
                        adjacencyList.Add(getAdjacentVertices(i, j, topology));
                    }
                    else if (topology[i, j] == Sample.Entry)
                    {
                        vertices.Add(new Vertice(i, j, topology[i, j]));
                        adjacencyList.Add(getAdjacentVertices(i, j, topology));
                    }
                    else if (topology[i, j] == Sample.Exit)
                    {
                        vertices.Add(new Vertice(i, j, topology[i, j]));
                        adjacencyList.Add(getAdjacentVertices(i, j, topology));
                    }
                }
            }
        }

        private static List<Vertice> getAdjacentVertices(int i, int j, Sample[,] topology)
        {
            List<Vertice> res = new List<Vertice>();
            if (topology[i, j] == Sample.CPS || topology[i, j] == Sample.TPS)
                return res;
            if (i != -1 && (topology[i - 1, j] == Sample.Road || topology[i - 1, j] == Sample.TPS || topology[i - 1, j] == Sample.CPS))
            {
                res.Add(new Vertice(i - 1, j, topology[i - 1, j]));
            }
            if (i != topology.GetLength(0) && (topology[i + 1, j] == Sample.Road || topology[i + 1, j] == Sample.TPS || topology[i + 1, j] == Sample.CPS))
            {
                res.Add(new Vertice(i + 1, j, topology[i + 1, j]));
            }
            if (j != -1 && (topology[i, j - 1] == Sample.Road || topology[i, j - 1] == Sample.TPS || topology[i, j - 1] == Sample.CPS))
            {
                res.Add(new Vertice(i, j - 1, topology[i, j - 1]));
            }
            if (j != topology.GetLength(1) && (topology[i, j + 1] == Sample.Road || topology[i, j + 1] == Sample.TPS || topology[i, j + 1] == Sample.CPS))
            {
                res.Add(new Vertice(i, j + 1, topology[i, j + 1]));
            }
            return res;
        }
    }
}