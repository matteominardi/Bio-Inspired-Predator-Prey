using System.Threading;

public static class CanReproduce
{
    private static int preysCounter = 0;
    private static int predatorsCounter = 0;
    private static bool canPreysReproduce = true;
    private static bool canPredatorsReproduce = true;

    // public int PreysCounter
    // {
    //     get { return Interlocked.CompareExchange(ref preysCounter, 0, 1); } // compara il valore corrente e l'expectedValue, se sono uguali, il valore di preysCounter viene sostituito con newValue e ritorna il valore di preysCounter vecchio
    //     set { Interlocked.Exchange(ref preysCounter, value); }              // rimpiazza il valore di preysCounter con il valore di value e ritorna il vecchio valore di preysCounter
    // }

    // public int PredatorsCounter
    // {
    //     get { return Interlocked.CompareExchange(ref counter, 0, 0); }
    //     set { Interlocked.Exchange(ref counter, value); }
    // }

    // public bool CanPreysReproduce
    // {
    //     get { return Interlocked.CompareExchange(ref canPreysReproduce, true, true) == true; }
    //     set { Interlocked.Exchange(ref canPreysReproduce, value ? true : 0); }
    // }

    // public bool CanPredatorsReproduce
    // {
    //     get { return Interlocked.CompareExchange(ref canPredatorsReproduce, true, true) == true; }
    //     set { Interlocked.Exchange(ref canPredatorsReproduce, value ? true : 0); }
    // }

    public static void IncrementPreysCounter()
    {
        preysCounter = Interlocked.Increment(ref preysCounter);

        if (preysCounter == 100)
        {
            Interlocked.Exchange(ref canPreysReproduce, false);
        }
    }

    public static void IncrementPredatorsCounter()
    {
        predatorsCounter = Interlocked.Increment(ref predatorsCounter);

        if (predatorsCounter == 100)
        {
            Interlocked.Exchange(ref canPredatorsReproduce, false);
        }
    }

    public static void DecrementPreysCounter()
    {
        preysCounter = Interlocked.Decrement(ref preysCounter);

        if (preysCounter < 100)
        {
            Interlocked.Exchange(ref canPreysReproduce, true);
        }
    }

    public static void DecrementPredatorsCounter()
    {
        preysCounter = Interlocked.Decrement(ref predatorsCounter);

        if (predatorsCounter < 100)
        {
            Interlocked.Exchange(ref canPredatorsReproduce, true);
        }
    }
}
