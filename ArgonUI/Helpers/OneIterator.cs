using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ArgonUI.Helpers;

/// <summary>
/// A simple iterator that just wraps a single element.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct OneEnumerable<T> : IEnumerable<T>
{
    private readonly T value;

    public OneEnumerable(T value)
    {
        this.value = value;
    }

    public IEnumerator<T> GetEnumerator() => new OneIterator(value);
    IEnumerator IEnumerable.GetEnumerator() => new OneIterator(value);

    internal struct OneIterator : IEnumerator<T>
    {
        private readonly T value;
        private bool done = false;

        public readonly T Current => value;
        readonly object IEnumerator.Current => value!;

        public OneIterator(T value)
        {
            this.value = value;
        }

        public readonly void Dispose() { }
        public bool MoveNext()
        {
            bool more = !done;
            done = true;
            return more;
        }

        public void Reset() => done = false;
    }
}
