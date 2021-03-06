﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.GameLogic;

namespace STVRogue.Utils
{
    /* Providing some useful predicates and functions to extract information from various
     * game entities.
     */
    public static class Predicates
    {
        /* Return a shortest path between node u and node v, reversed return so a path from v to u*/
        public static List<Node> shortestpath(Node u, Node v, int size)
        {            
            List<Node> shortest_path = new List<Node>() { };
            int graph_size = size;
            Queue<Node> q = new Queue<Node> { };
            Node[] parent = new Node[size];
            bool[] visited = new bool[size];
            //set_array_to_false(visited);
            q.Enqueue(u);

            while (q.Count > 0)
            {
                Node x = q.Dequeue();
                int options = x.neighbors.Count;
                for (int i = 0; i < options; i++)
                {
                    int witas = int.Parse(x.neighbors[i].id);       //witas is short for "why is this a string"
                    if (!visited[witas])
                    {
                        q.Enqueue(x.neighbors[i]);
                        visited[witas] = true;
                        parent[witas] = x;

                        if (witas == int.Parse(v.id))
                        {
                            shortest_path.Add(x.neighbors[i]);
                            while (witas != int.Parse(u.id))
                            {
                                shortest_path.Add(parent[witas]);
                                witas = int.Parse(parent[witas].id);
                            }
                            q.Clear();
                            break;
                        }
                    }
                }
            }
            //Check if found path is complete
            if (shortest_path.Count == 0)
            {
               // Console.WriteLine("No path found.");
                return shortest_path;
            }
           // Console.WriteLine("found path with size: " + shortest_path.Count);
            return shortest_path;

        }

        private static void set_array_to_false(bool[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = false;
            }
        }

        private static void set_array_to_zero(int[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = 0;
            }
        }

        public static Boolean isPath(List<Node> path)
        {
            Node[] path_ = path.ToArray();
            if (path_.Length <= 1) return true;
            Node a = path_[0];
            for (int i = 1; i < path_.Length; i++)
            {
                if (!a.neighbors.Contains(path_[i])) return false;
                a = path_[i];
            }
            return true;
        }

      


        public static int find_all_paths_extract_shortest(Node u, Node v, int size)
        {
            //List<Node> shortest_path = new List<Node>() { };
            int graph_size = size;
            int steps = int.MaxValue;
            Queue<Node> q = new Queue<Node> { };
            Node[] parent = new Node[graph_size];
            bool[] visited = new bool[graph_size];
            set_array_to_false(visited);
            q.Enqueue(u);

            while (q.Count > 0)
            {
                Node x = q.Dequeue();
                int options = x.neighbors.Count;

                for (int i = 0; i < options; i++)
                {
                    int witas = int.Parse(x.neighbors[i].id);       //witas is short for "why is this a string"
                    if (!visited[witas])
                    {
                        q.Enqueue(x.neighbors[i]);
                        visited[witas] = true;
                        parent[witas] = x;

                        if (witas == int.Parse(v.id))
                        {
                            //shortest_path.Add(x.neighbors[i]);
                            int d = 1; //count start node also
                            while (witas != int.Parse(u.id))
                            {
                                //shortest_path.Add(parent[witas]);
                                d++;
                                witas = int.Parse(parent[witas].id);
                            }
                            if(d < steps)
                            {
                                steps = d;
                            }
                        }
                    }
                }
            }
                if(steps > 9999)
            {
                return 0;
            }
                return steps;
        }

        public static List<Node> reachableNodes(Node x0)
        {
            List<Node> seen = new List<Node>();
            List<Node> todo = new List<Node>();
            todo.Add(x0);
            while (todo.Count != 0)
            {
                x0 = todo[0]; todo.RemoveAt(0);
                seen.Add(x0);
                //Console.WriteLine("++ marking " + x0.id + " as seen");
                foreach (Node y in x0.neighbors)
                {
                    //Console.WriteLine("-- considering " + x0.id + " -> " + y.id);
                    if (!seen.Contains(y) && !todo.Contains(y))
                    {
                        //Console.WriteLine("++ adding " + y.id + " to todo-list");
                        todo.Add(y);
                    }
                }
            }
            seen.Distinct();
            return seen;
        }

        public static Boolean isReachable(Node u, Node v)
        {
            return reachableNodes(u).Contains(v);
        }


        /* Check if a node is actually a bridge node. */
        public static Boolean isBridge(Node startNode, Node exitNode, Node nd)
        {
            if (nd == startNode || nd == exitNode) return false;
            List<Node> around = nd.neighbors;
            // temporarily disconnect the bridge 
            foreach (Node a in around.ToList()) a.neighbors.Remove(nd);
            Boolean isBridge = true;
            if (isReachable(startNode, exitNode)) isBridge = false;
            // restore the connections
            foreach (Node a in around.ToList()) a.neighbors.Add(nd);
            return isBridge;
        }





        /* Count the number of bridges between the given start and exit node. */
        public static uint countNumberOfBridges(Node startNode, Node exitNode)
        {
            List<Node> nodes = reachableNodes(startNode);
            uint n = 0;
            foreach (Node nd in nodes)
                if (isBridge(startNode, exitNode, nd)) { n++; }

            return n;
        }

        /* Check if a graph beween start and end nodes forms a valid dungeon of the
         * specified level.
         */
        public static Boolean isValidDungeon(Node startNode, Node exitNode, uint level)
        {
            if (startNode is Bridge || exitNode is Bridge) return false;
            if (countNumberOfBridges(startNode, exitNode) != level) return false;

            List<Node> nodes = reachableNodes(startNode);
            if (!nodes.Contains(exitNode)) return false;
            int totalConnectivityDegree = 0;
            foreach (Node nd in nodes)
            {
                foreach (Node nd2 in nd.neighbors)
                {
                    // check that each connection is bi-directional
                    if (!nd2.neighbors.Contains(nd)) return false;
                }
                // check the connectivity degree
                if (nd.neighbors.Count > 4) return false;
                totalConnectivityDegree += nd.neighbors.Count;
                // check bridge
                Boolean isBridge_ = isBridge(startNode, exitNode, nd);

                if (!(nd is Bridge) && isBridge_) return false;
            }
            float avrgConnectivity = (float)totalConnectivityDegree / (float)nodes.Count;
            if (avrgConnectivity > 3) return false;
            return true;
        }


        public static Boolean hpProperty(List<HealingPotion> totalPotions, List<Monster> aliveMonsters)
        {
            float potionsValue = 0, monsterHealthValue = 0;
            foreach (HealingPotion potion in totalPotions)
            {
                if (!potion.used) potionsValue += potion.HPvalue;
            }
            foreach (Monster monster in aliveMonsters)
            {
                monsterHealthValue += monster.HP;
            }

            return (potionsValue <= 0.8 * monsterHealthValue);
        }

    }

}

