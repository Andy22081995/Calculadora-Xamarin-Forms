using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator.Extensions
{
    public class EmptyEnumerator<T> : IEnumerator<T>
    {
        public T Current => default(T);

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }
        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {

        }
    }
}
