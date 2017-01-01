using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Elevator
{
    class Program
    {
        private static Dictionary<string, string> previousStates = new Dictionary<string, string>();
        private static List<string> currentStates = new List<string>();
        private static Dictionary<string, string> nextStates = new Dictionary<string, string>();
        private static int stepsTaken;
        private static bool solutionFound;

        static void Main(string[] args)
        {
            for (int phase = 1; phase <= 2; phase++)
            {
                stepsTaken = 0;
                solutionFound = false;
                previousStates = new Dictionary<string, string>();
                currentStates = new List<string>();
                switch (phase)
                {
                    case 1:
                        currentStates.Add(InitializeList("input.txt").DeSerializeToString(1));
                        break;
                    case 2:
                        currentStates.Add(InitializeList("input.txt").DeSerializeToString(11111));
                        break;
                }
                while (!solutionFound)
                {
                    nextStates = new Dictionary<string, string>();
                    if (currentStates.Count == 0)
                    {
                        Console.WriteLine("No more possible steps!");
                        break;
                    }
                    for (int i = 0; i < currentStates.Count; i++)
                    {
                        if (solutionFound)
                            break;
                        int floor = 0;
                        var state = currentStates[i].SerializeToList(out floor);
                        Move(state, floor);
                        previousStates.Add(currentStates[i], "");
                    }
                    stepsTaken++;
                    Console.CursorLeft = 0;
                    Console.Write(stepsTaken + " steps taken..");
                    currentStates = nextStates.Keys.ToList();
                }
                Console.WriteLine(" Solution for phase " + phase + " found! ");
            }
        }

        /// <summary>
        /// Do all the valid movements,
        /// and check if new state burns chips.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="pair"></param>
        private static void Move(List<KeyValuePair<int, int>> state, int floor)
        {
            KeyValuePair<int, int> firstChange;
            KeyValuePair<int, int> secondChange;
            // Check which stuff is on the same floor with the elevator
            List<int> RTGs = new List<int>();
            List<int> chips = new List<int>();
            List<int> pairs = new List<int>();
            for (int i = 0; i < state.Count; i++)
            {
                if (state[i].Key == floor)
                    chips.Add(i);
                if (state[i].Value == floor)
                    RTGs.Add(i);
                if (state[i].Key == floor && state[i].Value == floor)
                    pairs.Add(i);
            }
            if (RTGs.Count >= 2) // 2 RTGs up or down
            {
                for (int i = 0; i < RTGs.Count - 1; i++)
                {
                    for (int j = i + 1; j < RTGs.Count; j++)
                    {
                        if (state[RTGs[i]].Value < 4 && state[RTGs[j]].Value < 4)
                        {
                            firstChange = new KeyValuePair<int, int>(state[RTGs[i]].Key, state[RTGs[i]].Value + 1);
                            secondChange = new KeyValuePair<int, int>(state[RTGs[j]].Key, state[RTGs[j]].Value + 1);
                            CheckValidityAndAddIfValid(floor + 1, state, firstChange, RTGs[i], secondChange, RTGs[j]);
                        }
                        if (state[RTGs[i]].Value > 1 && state[RTGs[j]].Value > 1)
                        {
                            firstChange = new KeyValuePair<int, int>(state[RTGs[i]].Key, state[RTGs[i]].Value - 1);
                            secondChange = new KeyValuePair<int, int>(state[RTGs[j]].Key, state[RTGs[j]].Value - 1);
                            CheckValidityAndAddIfValid(floor - 1, state, firstChange, RTGs[i], secondChange, RTGs[j]);
                        }
                    }
                }
            }
            if (RTGs.Count >= 1) // 1 RTG up or down
            {
                for (int i = 0; i < RTGs.Count; i++)
                {
                    if (state[RTGs[i]].Value < 4)
                    {
                        firstChange = new KeyValuePair<int, int>(state[RTGs[i]].Key, state[RTGs[i]].Value + 1);
                        CheckValidityAndAddIfValid(floor + 1, state, firstChange, RTGs[i]);
                    }
                    if (state[RTGs[i]].Value > 1)
                    {
                        firstChange = new KeyValuePair<int, int>(state[RTGs[i]].Key, state[RTGs[i]].Value - 1);
                        CheckValidityAndAddIfValid(floor - 1, state, firstChange, RTGs[i]);
                    }
                }
            }
            if (chips.Count >= 2) // 2 chips up or down
            {
                for (int i = 0; i < chips.Count - 1; i++)
                {
                    for (int j = i + 1; j < chips.Count; j++)
                    {
                        if (state[chips[i]].Value < 4 && state[chips[j]].Value < 4)
                        {
                            firstChange = new KeyValuePair<int, int>(state[chips[i]].Key, state[chips[i]].Value + 1);
                            secondChange = new KeyValuePair<int, int>(state[chips[j]].Key, state[chips[j]].Value + 1);
                            CheckValidityAndAddIfValid(floor + 1, state, firstChange, chips[i], secondChange, chips[j]);
                        }
                        if (state[chips[i]].Value > 1 && state[chips[j]].Value > 1)
                        {
                            firstChange = new KeyValuePair<int, int>(state[chips[i]].Key, state[chips[i]].Value - 1);
                            secondChange = new KeyValuePair<int, int>(state[chips[j]].Key, state[chips[j]].Value - 1);
                            CheckValidityAndAddIfValid(floor - 1, state, firstChange, chips[i], secondChange, chips[j]);
                        }
                    }
                }
            }
            if (chips.Count >= 1) // 1 chip up or down
            {
                for (int i = 0; i < chips.Count; i++)
                {
                    if (state[chips[i]].Value < 4)
                    {
                        firstChange = new KeyValuePair<int, int>(state[chips[i]].Key, state[chips[i]].Value + 1);
                        CheckValidityAndAddIfValid(floor + 1, state, firstChange, chips[i]);
                    }
                    if (state[chips[i]].Value > 1)
                    {
                        firstChange = new KeyValuePair<int, int>(state[chips[i]].Key, state[chips[i]].Value - 1);
                        CheckValidityAndAddIfValid(floor - 1, state, firstChange, chips[i]);
                    }
                }
            }
            if (pairs.Count >= 1) // pairs up or down
            {
                for (int i = 0; i < pairs.Count; i++)
                {
                    if (state[pairs[i]].Value < 4)
                    {
                        firstChange = new KeyValuePair<int, int>(state[pairs[i]].Key + 1, state[pairs[i]].Value + 1);
                        CheckValidityAndAddIfValid(floor + 1, state, firstChange, chips[i]);
                    }
                    if (state[pairs[i]].Value > 1)
                    {
                        firstChange = new KeyValuePair<int, int>(state[pairs[i]].Key - 1, state[pairs[i]].Value - 1);
                        CheckValidityAndAddIfValid(floor - 1, state, firstChange, pairs[i]);
                    }
                }
            }
        }

        private static void CheckValidityAndAddIfValid(int floor, List<KeyValuePair<int, int>> state,
            KeyValuePair<int, int> firstChange, int firstChangeLocation,
            KeyValuePair<int, int> secondChange = new KeyValuePair<int, int>(),
            int secondChangeLocation = Int32.MaxValue)
        {
            // Create the state to be checked.
            List<KeyValuePair<int, int>> newState = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < state.Count; i++)
            {
                if (i == firstChangeLocation)
                    newState.Add(firstChange);
                else if (i == secondChangeLocation)
                    newState.Add(secondChange);
                else
                    newState.Add(state[i]);
            }

            // Check location of each RTG, and then check that if microchip is in same floor, it is connected to it's own RTG
            List<int> chips = new List<int>();
            List<int> RTGs = new List<int>();

            foreach (var pair in newState)
            {
                if (pair.Key != pair.Value)
                    if (!chips.Contains(pair.Key))
                        chips.Add(pair.Key);
                if (!RTGs.Contains(pair.Value))
                    RTGs.Add(pair.Value);
            }

            foreach (int value in chips)
                if (RTGs.Contains(value)) // Chips burned
                    return;

            string newStateStringified = newState.DeSerializeToString(floor, firstChange, firstChangeLocation,
                secondChange, secondChangeLocation);
            Regex regex = new Regex(@"^4+$");
            if (regex.IsMatch(newStateStringified.Substring(0, newStateStringified.Length - 1)))
            {
                solutionFound = true;
                return;
            }
            if (!previousStates.ContainsKey(newStateStringified) && !nextStates.ContainsKey(newStateStringified))
                nextStates.Add(newStateStringified, "");
        }

        /// <summary>
        /// Read input to dictionary, where key is the substance,
        /// and the value is pair, where key shows microchips floor,
        /// and value shows the generators floor.
        /// </summary>
        /// <param name="path">Path to input file</param>
        /// <returns>Dictionary of pairs</returns>
        private static List<KeyValuePair<int, int>> InitializeList(string path)
        {
            Dictionary<string, KeyValuePair<int, int>> pairs = new Dictionary<string, KeyValuePair<int, int>>();

            string[] input = File.ReadAllLines(path);
            Regex regex =
                new Regex(
                    @"The \w+ floor contains (nothing relevant|(and )?a (?<substance>\w+)(-compatible)? (?<unit>\w+)(, |\. | )?)*");
            for (int i = 0; i < input.Length; i++)
            {
                Match match = regex.Match(input[i]);
                for (int j = 0; j < match.Groups["substance"].Captures.Count; j++)
                {
                    if (!pairs.ContainsKey(match.Groups["substance"].Captures[j].Value))
                        pairs.Add(match.Groups["substance"].Captures[j].Value, new KeyValuePair<int, int>(0, 0));
                    KeyValuePair<int, int> current = pairs[match.Groups["substance"].Captures[j].Value];
                    if (match.Groups["unit"].Captures[j].Value == "microchip")
                        current = new KeyValuePair<int, int>(i + 1, current.Value);
                    else
                        current = new KeyValuePair<int, int>(current.Key, i + 1);
                    pairs[match.Groups["substance"].Captures[j].Value] = current;
                }
            }
            return pairs.Values.ToList();
        }
    }

    public static class ExtensionMethods
    {
        public static List<KeyValuePair<int, int>> SerializeToList(this string status, out int floor)
        {
            List<KeyValuePair<int, int>> Status = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < status.Length-1; i = i + 2)
            {
                Status.Add(new KeyValuePair<int, int>(status[i] - '0', status[i + 1] - '0'));
            }
            if (status.Length%2 == 0)
                floor = 1;
            else
                floor = Convert.ToInt16(status.Substring(status.Length - 1));
            return Status;
        }

        public static string DeSerializeToString(this List<KeyValuePair<int, int>> Status, int floor, KeyValuePair<int,int> firstChange = new KeyValuePair<int, int>(), int firstChangeLocation = int.MaxValue, KeyValuePair<int,int> secondChange = new KeyValuePair<int, int>(), int secondChangeLocation = Int32.MaxValue)
        {
            string deserialized = String.Empty;
            for (int i = 0; i < Status.Count; i++)
            {
                if (i == firstChangeLocation)
                    deserialized += firstChange.Key.ToString() + firstChange.Value;
                else if (i == secondChangeLocation)
                    deserialized += secondChange.Key.ToString() + secondChange.Value;
                else
                    deserialized += Status[i].Key.ToString() + Status[i].Value;
            }
            return deserialized + floor;
        }
    }
}
