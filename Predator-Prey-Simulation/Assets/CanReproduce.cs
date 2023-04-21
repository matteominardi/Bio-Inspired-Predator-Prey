using System.Threading;

public static class CanReproduce
{
    private static int _preysCounter = 0;
    private static int _predatorsCounter = 0;
    private static int _canPreysReproduce = 1;
    private static int _canPredatorsReproduce = 1;
    private static int _maxPreysAllowed = 15;
    private static int _maxPredatorsAllowed = 15;

    public static void IncrementPreysCounter()
    {
        _preysCounter = Interlocked.Increment(ref _preysCounter);

        if (_preysCounter >= _maxPreysAllowed)
        {
            Interlocked.Exchange(ref _canPreysReproduce, 0);
        }
    }

    public static void IncrementPredatorsCounter()
    {
        _predatorsCounter = Interlocked.Increment(ref _predatorsCounter);

        if (_predatorsCounter >= _maxPredatorsAllowed)
        {
            Interlocked.Exchange(ref _canPredatorsReproduce, 0);
        }
    }

    public static void DecrementPreysCounter()
    {
        _preysCounter = Interlocked.Decrement(ref _preysCounter);

        if (_preysCounter < _maxPreysAllowed)
        {
            Interlocked.Exchange(ref _canPreysReproduce, 1);
        }
    }

    public static void DecrementPredatorsCounter()
    {
        _preysCounter = Interlocked.Decrement(ref _predatorsCounter);

        if (_predatorsCounter < _maxPredatorsAllowed)
        {
            Interlocked.Exchange(ref _canPredatorsReproduce, 1);
        }
    }

    public static bool CanPreysReproduce()
    {
        return _canPreysReproduce == 1;
    }

    public static bool CanPredatorsReproduce()
    {
        return _canPredatorsReproduce == 1;
    }
}
