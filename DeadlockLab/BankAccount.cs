namespace DeadlockLab;

public class BankAccount
{
    private static int _nextId;
    private readonly object _sync = new();
    private decimal _balance;
    public int Id { get; }

    public BankAccount(decimal initialBalance)
    {
        if (initialBalance < 0)
            throw new ArgumentOutOfRangeException(
                nameof(initialBalance)
            );

        Id = Interlocked.Increment(ref _nextId);

        _balance = initialBalance;
    }

    public decimal Balance
    {
        get
        {
            lock (_sync)
            {
                return _balance;
            }
        }
    }

    internal object SyncRoot => _sync;

    internal decimal GetBalanceUnsafe()
    {
        return _balance;
    }

    internal void SetBalanceUnsafe(decimal value)
    {
        _balance = value;
    }
}
