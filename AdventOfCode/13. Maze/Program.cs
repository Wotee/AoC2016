using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;

namespace _13.Maze
{
    class Program
    {
        private static int number;
        static void Main(string[] args)
        {
            number = args.Length != 0 ? Convert.ToInt32(args[0]) : 1364; // Puzzle input via paramter or hardcoded my default value
            var start = new KeyValuePair<int, int>(1,1);
            var goal = new KeyValuePair<int, int>(31,39);
            List<KeyValuePair<int,int>> route = Astar(start, goal);
            int numberOfReachAbleCoordinates = ReachableCoordinates(start, 50);
            Console.WriteLine("Phase 1: " + (route.Count-1).ToString());
            Console.WriteLine("Phase 2: " + numberOfReachAbleCoordinates);
        }

        private static int ReachableCoordinates(KeyValuePair<int, int> start, int maxSteps)
        {
            Dictionary<KeyValuePair<int,int>,int> ReachableCoordinates = new Dictionary<KeyValuePair<int, int>,int>(){};
            ReachableCoordinates.Add(start,0);
            List<KeyValuePair<int, int>> Neighbors = InitializeNeighbors(start);            
            for (int stepsTaken = 1; stepsTaken <= maxSteps; stepsTaken++)
            {
                List<KeyValuePair<int, int>> NewNeighbors = new List<KeyValuePair<int, int>>();
                foreach (var Neighbor in Neighbors)
                {
                    if (!ReachableCoordinates.ContainsKey(Neighbor))
                    {
                        ReachableCoordinates.Add(Neighbor, stepsTaken);
                        foreach(var NewNeighbor in InitializeNeighbors(Neighbor))
                        {
                            NewNeighbors.Add(NewNeighbor);
                        }
                    }
                }
                Neighbors = NewNeighbors;
            }
            return ReachableCoordinates.Count;
        }

        static List<KeyValuePair<int, int>> Astar(KeyValuePair<int, int> start, KeyValuePair<int, int> goal)
        {
            List<KeyValuePair<int,int>> ClosedSet = new List<KeyValuePair<int, int>>();
            List<KeyValuePair<int,int>> OpenSet = new List<KeyValuePair<int, int>>() {start};
            
            Dictionary<KeyValuePair<int,int>, KeyValuePair<int,int>> cameFrom = new Dictionary<KeyValuePair<int, int>, KeyValuePair<int, int>>();

            Dictionary<KeyValuePair<int,int>,int> gScore = new Dictionary<KeyValuePair<int, int>, int>();
            gScore[start] =  0;
            Dictionary<KeyValuePair<int,int>,int> fScore = new Dictionary<KeyValuePair<int, int>, int>();
            fScore[start] = HeuristicCostEstimate(start, goal);
            KeyValuePair<int, int> current = new KeyValuePair<int, int>();
            while (OpenSet.Count > 0)
            {
                foreach (var keyValuePair in fScore.OrderBy(item => item.Value))
                {
                    if (OpenSet.Contains(keyValuePair.Key))
                    {
                        if (keyValuePair.Key.Value == current.Value && keyValuePair.Key.Key == current.Key)
                            continue;
                        current = keyValuePair.Key;
                        break;
                    }
                }
                if (current.Value == goal.Value && current.Key == goal.Key)
                {
                    return GetPath(cameFrom, current);
                }
                OpenSet.Remove(current);
                ClosedSet.Add(current);
                List<KeyValuePair<int, int>> neighbors = InitializeNeighbors(current);
                foreach (var neighbor in neighbors)
                {
                    if (ClosedSet.Contains(neighbor))
                        continue;
                    int tentative_gScore = (gScore[current]) + 1;
                    if (!OpenSet.Contains(neighbor))
                        OpenSet.Add(neighbor);
                    else if (tentative_gScore >= gScore[neighbor])
                        continue;
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentative_gScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, goal);
                }
            }
            return GetPath(cameFrom, current);
        }

        private static List<KeyValuePair<int, int>> InitializeNeighbors(KeyValuePair<int, int> current)
        {
            List<KeyValuePair<int,int>> neighbors = new List<KeyValuePair<int, int>>();
            if (IsOpenSpace(current.Key + 1, current.Value))
                neighbors.Add(new KeyValuePair<int, int>(current.Key + 1, current.Value));
            if (IsOpenSpace(current.Key - 1, current.Value) && current.Key > 0)
                neighbors.Add(new KeyValuePair<int, int>(current.Key - 1, current.Value));
            if (IsOpenSpace(current.Key, current.Value + 1))
                neighbors.Add(new KeyValuePair<int, int>(current.Key, current.Value + 1));
            if (IsOpenSpace(current.Key, current.Value - 1) && current.Value > 0)
                neighbors.Add(new KeyValuePair<int, int>(current.Key, current.Value - 1));
            return neighbors;
        }

        private static bool IsOpenSpace(int x, int y)
        {
            int count = Convert.ToString(x * x + 3 * x + 2 * x * y + y + y * y + number, 2).Count(c => c == '1');
            return (count % 2 == 0) ? true : false;
        }

        private static int HeuristicCostEstimate(KeyValuePair<int, int> start, KeyValuePair<int, int> goal)
        {
            return Math.Abs(goal.Value - start.Value) + Math.Abs(goal.Key - start.Key);
        }

        static List<KeyValuePair<int, int>> GetPath(Dictionary<KeyValuePair<int, int>, KeyValuePair<int, int>> cameFrom, KeyValuePair<int, int> current)
        {
            List<KeyValuePair<int,int>> path = new List<KeyValuePair<int, int>>();
            path.Add(current);
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }
            return path;
        }
    }
}
