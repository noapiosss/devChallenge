using System.Collections;
using System.Collections.Generic;

namespace E2E.Data
{
    public class NotValidCellIdSigns : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new()
        {
            new object[] {' '},
            new object[] {'+'},
            new object[] {'-'},
            new object[] {'*'},
            new object[] {'='},
            new object[] {'('},
            new object[] {')'},
            new object[] {','}
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}