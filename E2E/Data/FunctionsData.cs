using System.Collections;
using System.Collections.Generic;
using E2E.Data.Contracts;

namespace E2E.Data
{
    public class ValidFunctions : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new CellDataWithStatusCode[]
                {
                    new("var1", "1", "1", System.Net.HttpStatusCode.Created),
                    new("var2", "2", "2", System.Net.HttpStatusCode.Created),
                    new("var3", "3", "3", System.Net.HttpStatusCode.Created),
                    new("var4", "4", "4", System.Net.HttpStatusCode.Created),
                    new("var5", "=sum(var1,var2,var3,var4)", "10", System.Net.HttpStatusCode.Created),
                    new("var6", "=avg(var5,var2)", "6", System.Net.HttpStatusCode.Created),
                    new("var7", "=min(var1,var2,var3,var4,var5)", "1", System.Net.HttpStatusCode.Created),
                    new("var8", "=max(var1,var2,var3,var4,var5)", "10", System.Net.HttpStatusCode.Created)
                },
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ValidNestedFunctions : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new CellDataWithStatusCode[]
                {
                    new("var1", "1", "1", System.Net.HttpStatusCode.Created),
                    new("var2", "2", "2", System.Net.HttpStatusCode.Created),
                    new("var3", "3", "3", System.Net.HttpStatusCode.Created),
                    new("var4", "4", "4", System.Net.HttpStatusCode.Created),
                    new("var5", "=sum(min(var1,var2),min(var3,var4))", "4", System.Net.HttpStatusCode.Created),
                    new("var6", "=max(avg(var1,var3),min(var3,var4))", "3", System.Net.HttpStatusCode.Created),
                    new("var6", "=max(avg(var1,var3),min(var3,var4),min(432,234))", "234", System.Net.HttpStatusCode.Created),
                },
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}