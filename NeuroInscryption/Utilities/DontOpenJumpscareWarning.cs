using System.Collections.Generic;

namespace NeuroInscryption.Utilities;

/*
 ███▄ ▄███▓ ▄▄▄     ████████▓ ██░ ██
▓██▒▀█▀ ██▒▒████▄   ▓  ██▒ ▓▒▓██░ ██▒
▓██    ▓██░▒██  ▀█▄ ▒ ░██░ ▒░▒██▀▀██░
▒██    ▒██ ░██▄▄▄▄██░ ░██░ ░ ░▓█ ░██
▒██▒   ░██▒ ▓█   ▓██▒ ▒██▒ ░ ░▓█▒░██▓
░ ▒░   ░  ░ ▒▒   ▓▒█░ ▒ ░░    ▒ ░░▒░▒
░  ░      ░  ▒   ▒▒ ░   ░     ▒ ░▒░ ░
░      ░     ░   ▒    ░       ░  ░░ ░
       ░         ░  ░         ░  ░  ░
 */

public static class DontOpenJumpscareWarning
{
    public static List<List<T>> GenerateAllPermutationsOfSubsets<T>(List<T> list)
    {
        List<List<T>> allPermutations = [];
        List<List<T>> subsets = GetAllSubsets(list);

        foreach (List<T> subset in subsets)
        {
            List<List<T>> permutations = GetPermutations(subset);
            allPermutations.AddRange(permutations);
        }

        return allPermutations;
    }

    private static List<List<T>> GetAllSubsets<T>(List<T> list)
    {
        List<List<T>> subsets = [];
        int subsetCount = 1 << list.Count;

        for (int i = 1; i < subsetCount; i++)
        {
            List<T> subset = [];
            for (int j = 0; j < list.Count; j++)
            {
                if ((i & (1 << j)) != 0)
                {
                    subset.Add(list[j]);
                }
            }
            subsets.Add(subset);
        }

        return subsets;
    }

    private static List<List<T>> GetPermutations<T>(List<T> list)
    {
        List<List<T>> result = [];
        Permute(list, 0, result);
        return result;
    }

    private static void Permute<T>(List<T> list, int start, List<List<T>> result)
    {
        if (start >= list.Count)
        {
            result.Add([..list]);
            return;
        }

        for (int i = start; i < list.Count; i++)
        {
            (list[start], list[i]) = (list[i], list[start]);
            Permute(list, start + 1, result);
            (list[start], list[i]) = (list[i], list[start]);
        }
    }
}
