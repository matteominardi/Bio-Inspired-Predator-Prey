using System.Threading;

public static class CanReproduce
{
    private static readonly object _lockPreys = new object();
    private static readonly object _lockPredators = new object();
    private static int _preysCounter = 0;
    private static int _predatorsCounter = 0;
    private static int _canPreysReproduce = 1;
    private static int _canPredatorsReproduce = 1;
    private static int _maxPreysAllowed = 32;
    private static int _maxPredatorsAllowed = 21;

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
        lock (_lockPreys)
        {
            _preysCounter = Interlocked.Decrement(ref _preysCounter);

            if (_preysCounter < _maxPreysAllowed)
            {
                Interlocked.Exchange(ref _canPreysReproduce, 1);
            }
        }
    }

    public static void DecrementPredatorsCounter()
    {
        lock (_lockPredators)
        {
            _preysCounter = Interlocked.Decrement(ref _predatorsCounter);

            if (_predatorsCounter < _maxPredatorsAllowed)
            {
                Interlocked.Exchange(ref _canPredatorsReproduce, 1);
            }
        }
    }

    public static bool CanPreysReproduce()
    {
        lock (_lockPreys)
        {
            int res = Interlocked.CompareExchange(ref _canPreysReproduce, 1, 1);
            if (res == 1)
            {
                IncrementPreysCounter();
                return true;
            }
            return false;
        }
    }

    public static bool CanPredatorsReproduce()
    {
        lock (_lockPredators)
        {
            int res = Interlocked.CompareExchange(ref _canPredatorsReproduce, 1, 1);
            if (res == 1)
            {
                IncrementPredatorsCounter();
                return true;
            }
            return false;
        }
    }

    public static int MaxPreysAllowed()
    {
        return _maxPreysAllowed;
    }

    public static int MaxPredatorsAllowed()
    {
        return _maxPredatorsAllowed;
    }
}
