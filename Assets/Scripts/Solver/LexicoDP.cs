using System.Collections.Generic;

static class LexicoDP
{
    const int N = 13;
    static bool[] mark = new bool[N];
    static int[] fact = new int[N];
    static int[,] C = new int[N, N];
    static LexicoDP()
    {
        mark[0] = false;
        fact[0] = 1;
        for (int i = 1; i < N; i++)
        {
            mark[i] = false;
            fact[i] = fact[i - 1] * i;
        }
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; i++)
            {
                C[i, j] = 0;
            }
        }
        for (int i = 0; i < N; i++)
        {
            C[i, 0] = 1;
        }
        for (int i = 1; i < N; i++)
        {
            for (int j = 1; j <= i; j++)
            {
                C[i, j] = C[i - 1, j - 1] + C[i - 1, j];
            }
        }
    }
    public static int PermToIdx(List<int> perm)
    {
        int n = perm.Count;
        for (int i = 0; i < n; i++)
        {
            mark[i] = false;
        }
        int idx = 0;
        for (int i = 0; i < n; i++)
        {
            for (int d = 0; d < perm[i]; d++)
            {
                if (mark[d]) continue;
                idx += fact[n - i - 1];
            }
            mark[perm[i]] = true;
        }
        return idx;
    }
    public static List<int> IdxToPerm(int idx, int n)
    {
        List<int> perm = new List<int>();
        for (int i = 0; i < n; i++)
        {
            mark[i] = false;
        }
        for (int i = 0; i < n; i++)
        {
            for (int d = 0; d < n; d++)
            {
                if (mark[d]) continue;
                if (idx - fact[n-i-1] < 0)
                {
                    mark[d] = true;
                    perm.Add(d);
                    break;
                } else
                {
                    idx -= fact[n - i - 1];
                }
            }
        }
        return perm;
    }

    public static int CombToIdx(List<int> comb, int n)
    {
        int k = comb.Count;
        int idx = 0;
        for (int i = 0; i < k; i++)
        {
            for (int d = (i == 0 ? 0 : comb[i-1]+1); d < comb[i]-1; d++)
            {
                idx += C[n - d - 1, k - i - 1];
            }
        }
        return idx;
    }

    public static List<int> IdxToComb(int idx, int n, int k)
    {
        List<int> Comb = new List<int>();
        for (int i = 0; i < k; i++)
        {
            int d = (i == 0 ? 0 : Comb[i - 1] + 1);
            while (true)
            {
                if (idx - C[n - d - 1, k - i - 1] < 0)
                {
                    Comb.Add(d);
                    break;
                } else
                {
                    idx -= C[n - d - 1, k - i - 1];
                    d++;
                }
            }
        }
        return Comb;
    }
}