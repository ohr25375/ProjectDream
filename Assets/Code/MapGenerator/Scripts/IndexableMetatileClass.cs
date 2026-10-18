public class IndexableMetatileClass<T>
{
    public T AA, BA, AB, BB;
    public T this[int i]
    {
        get
        {
            return i switch
            {
                0 => AA,
                1 => BA,
                2 => AB,
                3 => BB,
                _ => throw new System.IndexOutOfRangeException(),
            };
        }
        set
        {
            switch (i)
            {
                case 0: AA = value; break;
                case 1: BA = value; break;
                case 2: AB = value; break;
                case 3: BB = value; break;
                default: throw new System.IndexOutOfRangeException();
            }
        }
    }
}
