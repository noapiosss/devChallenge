using System.Collections;
using System.Collections.Generic;
using E2E.Data.Contracts;

namespace E2E.Data
{
    public class ValidMathExpressions : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new CellDataWithStatusCode("var1", "=(5 + 8) * 2 - 7 / 3", "23,66666667", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var2", "=10 * (4 - 2) + 6 / (3 + 1)", "21,5", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var3", "=(15 - 7) / 2 + 9 * (4 + 1)", "49", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var4", "=20 / (2 + 3) - 6 * (7 - 4)", "-14", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var5", "=(12 + 4) * (9 - 6) / 2", "24", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var6", "=18 / 3 + (5 * 2) - (9 - 1)", "8", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var7", "=25 / (5 - 2) + 6 * (3 + 2)", "38,33333333", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var8", "=(14 - 3) * 2 + 7 / (6 + 1)", "23", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var9", "=16 / (4 + 2) - 5 * (8 - 3)", "-22,33333333", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var10", "=(10 + 2) * (6 - 4) / 2", "12", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var11", "=28 / 4 + (9 * 3) - (6 - 1)", "29", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var12", "=30 / (3 - 1) - 8 * (5 - 2)", "-9", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var13", "=(16 + 7) * 3 - 5 / (9 + 1)", "68,5", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var14", "=24 / (6 + 3) + 4 * (7 - 2)", "22,66666667", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var15", "=(20 - 5) * 2 + 9 / (8 + 1)", "31", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var16", "=14 / (7 - 4) - 6 * (5 + 2)", "-37,33333333", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var17", "=(18 + 6) * (5 - 2) / 3", "24", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var18", "=22 / 2 + (4 * 3) - (7 - 1)", "17", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var19", "=12 / (3 + 1) - 5 * (9 - 6)", "-12", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var20", "=(15 - 2) * 4 + 6 / (8 + 1)", "52,66666667", System.Net.HttpStatusCode.Created)
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class NotValidMathExpressions : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new CellDataWithStatusCode("var1", "=(5 + 8) * 2 - 7 / 0", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity),
                new CellDataWithStatusCode("var2", "=10 * (4 - 2) + 6 / 3 + 1)", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity),
                new CellDataWithStatusCode("var3", "=(15 - 7 / 2 + 9 * (4 + 1)", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity),
                new CellDataWithStatusCode("var4", "=20 / (2 + 3) -()- 6 * (7 - 4)", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity),
                new CellDataWithStatusCode("var5", "=(12 + 4) * (9 +*- 6) / 2", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity),
                new CellDataWithStatusCode("var6", "=18 / 0 + (5 * 2) - (9 - 1)", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity),
                new CellDataWithStatusCode("var7", "=25 +-+/ (5 - 2)/0 + 6 * (3 + 2)", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity),
                new CellDataWithStatusCode("var8", "=(14 - 3) * 2 + 7 / (-)(6 + 1)", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity)
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}