using System.Collections;
using System.Collections.Generic;
using E2E.Data.Contracts;

namespace E2E.Data
{
    public class OnlyValidCells : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new List<CellDataWithStatusCode>()
                {
                    new("var1", "1", "1", System.Net.HttpStatusCode.Created),
                    new("var2", "2", "2", System.Net.HttpStatusCode.Created),
                    new("var3", "=var1+var2", "3", System.Net.HttpStatusCode.Created)
                },

                new List<CellDataWithStatusCode>()
                {
                    new("var1", "1", "1", System.Net.HttpStatusCode.OK),
                    new("var2", "2", "2", System.Net.HttpStatusCode.OK),
                    new("var3", "=var1+var2", "3", System.Net.HttpStatusCode.OK)
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ValidAndInvalidCellsCells : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new List<CellDataWithStatusCode>()
                {
                    new("var1", "1", "1", System.Net.HttpStatusCode.Created),
                    new("var2", "2", "2", System.Net.HttpStatusCode.Created),
                    new("var3", "=var1+var2", "3", System.Net.HttpStatusCode.Created),
                    new("var4", "asd", "asd", System.Net.HttpStatusCode.Created),
                    new("var5", "=var4+1", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity),
                    new("var2", "im string now", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity)
                },

                new List<CellDataWithStatusCode>()
                {
                    new("var1", "1", "1", System.Net.HttpStatusCode.OK),
                    new("var2", "2", "2", System.Net.HttpStatusCode.OK),
                    new("var3", "=var1+var2", "3", System.Net.HttpStatusCode.OK),                    
                    new("var4", "asd", "asd", System.Net.HttpStatusCode.OK),
                    new("var5", "", "", System.Net.HttpStatusCode.NotFound)
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ValidCellsWithRecuresion : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new List<CellDataWithStatusCode>()
                {
                    new("var1", "1", "1", System.Net.HttpStatusCode.Created),
                    new("var2", "=var1+1", "2", System.Net.HttpStatusCode.Created),
                    new("var3", "=var2+1", "3", System.Net.HttpStatusCode.Created),
                    new("var4", "=var3+1", "4", System.Net.HttpStatusCode.Created),
                    new("var5", "=var4+1", "5", System.Net.HttpStatusCode.Created),
                    new("var6", "=var5+1", "6", System.Net.HttpStatusCode.Created),
                    new("var7", "=var6+1", "7", System.Net.HttpStatusCode.Created),
                    new("var8", "=var7+1", "8", System.Net.HttpStatusCode.Created),
                    new("var9", "=var8+1", "9", System.Net.HttpStatusCode.Created),
                    new("var10", "=var9+1", "10", System.Net.HttpStatusCode.Created)
                },

                new List<CellDataWithStatusCode>()
                {
                    new("var1", "1", "1", System.Net.HttpStatusCode.OK),
                    new("var2", "=var1+1", "2", System.Net.HttpStatusCode.OK),
                    new("var3", "=var2+1", "3", System.Net.HttpStatusCode.OK),
                    new("var4", "=var3+1", "4", System.Net.HttpStatusCode.OK),
                    new("var5", "=var4+1", "5", System.Net.HttpStatusCode.OK),
                    new("var6", "=var5+1", "6", System.Net.HttpStatusCode.OK),
                    new("var7", "=var6+1", "7", System.Net.HttpStatusCode.OK),
                    new("var8", "=var7+1", "8", System.Net.HttpStatusCode.OK),
                    new("var9", "=var8+1", "9", System.Net.HttpStatusCode.OK),
                    new("var10", "=var9+1", "10", System.Net.HttpStatusCode.OK)
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ValidCellsWithRecuresionWithUpdateAtBegining : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new List<CellDataWithStatusCode>()
                {
                    new("var1", "1", "1", System.Net.HttpStatusCode.Created),
                    new("var2", "=var1+1", "2", System.Net.HttpStatusCode.Created),
                    new("var3", "=var2+1", "3", System.Net.HttpStatusCode.Created),
                    new("var4", "=var3+1", "4", System.Net.HttpStatusCode.Created),
                    new("var5", "=var4+1", "5", System.Net.HttpStatusCode.Created),
                    new("var6", "=var5+1", "6", System.Net.HttpStatusCode.Created),
                    new("var7", "=var6+1", "7", System.Net.HttpStatusCode.Created),
                    new("var8", "=var7+1", "8", System.Net.HttpStatusCode.Created),
                    new("var9", "=var8+1", "9", System.Net.HttpStatusCode.Created),
                    new("var10", "=var9+1", "10", System.Net.HttpStatusCode.Created),
                    new("var1", "2", "2", System.Net.HttpStatusCode.Created),
                },

                new List<CellDataWithStatusCode>()
                {
                    new("var1", "2", "2", System.Net.HttpStatusCode.OK),
                    new("var2", "=var1+1", "3", System.Net.HttpStatusCode.OK),
                    new("var3", "=var2+1", "4", System.Net.HttpStatusCode.OK),
                    new("var4", "=var3+1", "5", System.Net.HttpStatusCode.OK),
                    new("var5", "=var4+1", "6", System.Net.HttpStatusCode.OK),
                    new("var6", "=var5+1", "7", System.Net.HttpStatusCode.OK),
                    new("var7", "=var6+1", "8", System.Net.HttpStatusCode.OK),
                    new("var8", "=var7+1", "9", System.Net.HttpStatusCode.OK),
                    new("var9", "=var8+1", "10", System.Net.HttpStatusCode.OK),
                    new("var10", "=var9+1", "11", System.Net.HttpStatusCode.OK)
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ValidCellsWithRecuresionWithInvalidUpdateAtBegining : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new List<CellDataWithStatusCode>()
                {
                    new("var1", "1", "1", System.Net.HttpStatusCode.Created),
                    new("var2", "=var1+1", "2", System.Net.HttpStatusCode.Created),
                    new("var3", "=var2+1", "3", System.Net.HttpStatusCode.Created),
                    new("var4", "=var3+1", "4", System.Net.HttpStatusCode.Created),
                    new("var5", "=var4+1", "5", System.Net.HttpStatusCode.Created),
                    new("var6", "=var5+1", "6", System.Net.HttpStatusCode.Created),
                    new("var7", "=var6+1", "7", System.Net.HttpStatusCode.Created),
                    new("var8", "=var7+1", "8", System.Net.HttpStatusCode.Created),
                    new("var9", "=var8+1", "9", System.Net.HttpStatusCode.Created),
                    new("var10", "=var9+1", "10", System.Net.HttpStatusCode.Created),
                    new("var1", "string", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity),
                },

                new List<CellDataWithStatusCode>()
                {
                    new("var1", "1", "1", System.Net.HttpStatusCode.OK),
                    new("var2", "=var1+1", "2", System.Net.HttpStatusCode.OK),
                    new("var3", "=var2+1", "3", System.Net.HttpStatusCode.OK),
                    new("var4", "=var3+1", "4", System.Net.HttpStatusCode.OK),
                    new("var5", "=var4+1", "5", System.Net.HttpStatusCode.OK),
                    new("var6", "=var5+1", "6", System.Net.HttpStatusCode.OK),
                    new("var7", "=var6+1", "7", System.Net.HttpStatusCode.OK),
                    new("var8", "=var7+1", "8", System.Net.HttpStatusCode.OK),
                    new("var9", "=var8+1", "9", System.Net.HttpStatusCode.OK),
                    new("var10", "=var9+1", "10", System.Net.HttpStatusCode.OK)
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}