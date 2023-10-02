using System.Collections;
using System.Collections.Generic;

namespace E2E.Data
{
    public class NotValidMathExpressions : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new()
        {
            new object[] {"=(5 + 8) * 2 - 7 / 0"},
            new object[] {"=10 * (4 - 2) + 6 / 3 + 1)"},
            new object[] {"=(15 - 7 / 2 + 9 * (4 + 1)"},
            new object[] {"=20 / (2 + 3) -()- 6 * (7 - 4)"},
            new object[] {"=(12 + 4) * (9 +*- 6) / 2"},
            new object[] {"=18 / 0 + (5 * 2) - (9 - 1)"},
            new object[] {"=25 +-+/ (5 - 2)/0 + 6 * (3 + 2)"},
            new object[] {"=(14 - 3) * 2 + 7 / (-)(6 + 1)"}
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}