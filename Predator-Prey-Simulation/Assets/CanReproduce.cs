using System.Threading;
using UnityEngine;

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
        string print = "";
        print += "\n--------------- IncrementPreysCounter ----------------\n";
        print += "\npreys counter before incrementing " + _preysCounter;
        _preysCounter = Interlocked.Increment(ref _preysCounter);
        print += "\npreys counter after incrementing " + _preysCounter + " /  max preys allowed " + _maxPreysAllowed;

        if (_preysCounter >= _maxPreysAllowed)
        {
            print += "\npreys counter is greater than max preys allowed !!!!!";
            print += "\ncan preys reproduce before exchange " + _canPreysReproduce;
            Interlocked.Exchange(ref _canPreysReproduce, 0);
            print += "\ncan preys reproduce after exchange " + _canPreysReproduce;

        }
        print += "\n\n-------------------------------";
        Debug.Log(print);

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
        string print = "";

        print += "\n--------------- DecrementPreysCounter ----------------\n";

        print += "\npreys counter before decrementing " + _preysCounter;

        // lock (_lockPreys)
        // {
            _preysCounter = Interlocked.Decrement(ref _preysCounter);
        print += "\npreys counter after decrementing " + _preysCounter + " /  max preys allowed " + _maxPreysAllowed;


            if (_preysCounter < _maxPreysAllowed)
            {
                print += "\npreys counter is lower than max preys allowed !!!!!";
                print += "\ncan preys reproduce before exchange " + _canPreysReproduce;
                Interlocked.Exchange(ref _canPreysReproduce, 1);
                print += "\ncan preys reproduce after exchange " + _canPreysReproduce;
            }
        print += "\n\n-------------------------------";
        Debug.Log(print);
        
        // }
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
        // lock (_lockPreys)
        // {
            string print = "";
            print += "\n--------------- CanPreysReproduce ----------------\n";
            print += "\npreys counter before checking " + _preysCounter;
            print += "\ncan preys reproduce before checking " + _canPreysReproduce;

            int res = Interlocked.CompareExchange(ref _canPreysReproduce, 1, 1);
            print += "\ncan preys reproduce after checking and exchanging " + _canPreysReproduce;
           

            if (res == 1)
            {
                print += "\nres == 1 !!!!!";
                IncrementPreysCounter();
                print += "\npreys counter after checking " + _preysCounter;
                print += "\ncan preys reproduce after checking " + _canPreysReproduce;
                print += "\n\n-------------------------------";
                Debug.Log(print);
                return true;
            }
            print += "\npreys counter after checking but res != 1 " + _preysCounter;
            print += "\ncan preys reproduce but res != 1 " + _canPreysReproduce;
            print += "\n\n-------------------------------";
            Debug.Log(print);
            return false;
        // }
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

    public static int PreysCounter()
    {
        return _preysCounter;
    }
}
