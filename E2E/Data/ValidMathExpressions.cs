using System.Collections;
using System.Collections.Generic;

namespace E2E.Data
{
    public class ValidMathExpressions : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new()
        {
            new object[] {"=(5 + 8) * 2 - 7 / 3", "23,66666667"},
            new object[] {"=10 * (4 - 2) + 6 / (3 + 1)", "21,5"},
            new object[] {"=(15 - 7) / 2 + 9 * (4 + 1)", "49"},
            new object[] {"=20 / (2 + 3) - 6 * (7 - 4)", "-14"},
            new object[] {"=(12 + 4) * (9 - 6) / 2", "24"},
            new object[] {"=18 / 3 + (5 * 2) - (9 - 1)", "8"},
            new object[] {"=25 / (5 - 2) + 6 * (3 + 2)", "38,33333333"},
            new object[] {"=(14 - 3) * 2 + 7 / (6 + 1)", "23"},
            new object[] {"=16 / (4 + 2) - 5 * (8 - 3)", "-22,33333333"},
            new object[] {"=(10 + 2) * (6 - 4) / 2", "12"},
            new object[] {"=28 / 4 + (9 * 3) - (6 - 1)", "29"},
            new object[] {"=30 / (3 - 1) - 8 * (5 - 2)", "-9"},
            new object[] {"=(16 + 7) * 3 - 5 / (9 + 1)", "68,5"},
            new object[] {"=24 / (6 + 3) + 4 * (7 - 2)", "22,66666667"},
            new object[] {"=(20 - 5) * 2 + 9 / (8 + 1)", "31"},
            new object[] {"=14 / (7 - 4) - 6 * (5 + 2)", "-37,33333333"},
            new object[] {"=(18 + 6) * (5 - 2) / 3", "24"},
            new object[] {"=22 / 2 + (4 * 3) - (7 - 1)", "17"},
            new object[] {"=12 / (3 + 1) - 5 * (9 - 6)", "-12"},
            new object[] {"=(15 - 2) * 4 + 6 / (8 + 1)", "52,66666667"}
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}