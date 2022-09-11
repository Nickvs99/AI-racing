using System;
using System.Collections;

/// <summary>
/// A generic wrapper class for lists. This allows for jagged arrays to be serialized.
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public class WrapperArray<T> : IEnumerator, IEnumerable
{
    public T[] array;
    private int position = -1;

    public WrapperArray() : this(new T[0])
    {

    }

    public WrapperArray(T[] _array)
    {
        array = _array;
    }

    public T this[int index]
    {
        get => array[index];
        set => array[index] = value;
    }

    public int Length { get { return array.Length; } }

    // Set of functions required for IEnumerator and IEnumerable to make foreach work
    // https://docs.microsoft.com/en-us/troubleshoot/developer/visualstudio/csharp/language-compilers/make-class-foreach-statement
    public IEnumerator GetEnumerator()
    {
        return (IEnumerator)this;
    }

    public bool MoveNext()
    {
        position++;
        return (position < array.Length);
    }

    public void Reset()
    {
        position = -1;
    }

    public object Current
    {
        get { return array[position]; }
    }
}
