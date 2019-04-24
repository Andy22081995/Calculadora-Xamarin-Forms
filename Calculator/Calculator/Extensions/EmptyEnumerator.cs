using System.Collections;
using System.Collections.Generic;

namespace Calculator.Extensions
{
    public class EmptyEnumerator<T> : IEnumerator<T>
    {
        public T Current => default(T);

        object IEnumerator.Current
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
