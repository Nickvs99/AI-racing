using System;
using System.Collections;

/// <summary>
/// A generic wrapper class for lists. This allows for jagged arrays to be serialized.
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public class WrapperArray<T> : IEnumerable
{
    public T[] array;
    public int Length { get { return array.Length; } }

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


    // Set of functions required for IEnumerator and IEnumerable to make foreach work
    // https://docs.microsoft.com/en-us/troubleshoot/developer/visualstudio/csharp/language-compilers/make-class-foreach-statement
    private class MyEnumerator: IEnumerator
    {
        public T[] array;
        private int position = -1;

        public MyEnumerator(T[] _array)
        {
            array = _array;
        }

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

    public IEnumerator GetEnumerator()
    {
        return new MyEnumerator(array);
    }
}
