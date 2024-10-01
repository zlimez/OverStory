using System;

public class Countable<U>
{
    public U Data { get; private set; }
    public int Count { get; private set; }

    public Countable(U data, int Count)
    {
        this.Data = data;
        this.Count = Count;
    }

    public void AddToStock(int quantity)
    {
        this.Count += quantity;
    }

    public bool RemoveStock(int quantity)
    {
        this.Count = Math.Max(this.Count - quantity, 0);
        return this.Count == 0;
    }

    public override bool Equals(object obj)
    {
        if (obj is Countable<U> countable)
            return countable.Count == Count && countable.Data.Equals(Data);
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Data, Count);
    }

    public static bool operator ==(Countable<U> a, Countable<U> b)
    {
        if (a is null) return b is null;
        return a.Equals(b);
    }

    public static bool operator !=(Countable<U> a, Countable<U> b)
    {
        if (a is null) return b is not null;
        return !a.Equals(b);
    }
}

