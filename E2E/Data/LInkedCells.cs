using System.Collections;
using System.Collections.Generic;
using E2E.Data.Contracts;

namespace E2E.Data
{
    public class BasicLInkedCells : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new CellData("var1", "1", "1"),
                new CellData("var2", "2", "2"),
                new CellData("var3", "=var1+var2", "3")
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class BasicLinkedCellsBreakers : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new CellDataWithStatusCode("var1", "1", "1", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var2", "2", "2", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var3", "=var1+var2", "3", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var1", "hi there", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity),
                new CellDataWithStatusCode("var2", "=var1+var3", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity),
                new CellDataWithStatusCode("var3", "=var3+var1", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity)
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class EmptyStringLinking : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new CellData("var1", "", ""),
                new CellData("var2", "=var1", "0"),
                new CellData("var3", "=var1+1", "1")
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


    public class BigChainOfLindekCells : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            List<CellData> cells = new()
            {   
                new CellData("var1", "=1", "1")
            };

            for (int i = 2; i <= 100; ++i)
            {
                cells.Add(new CellData($"var{i}", $"=var{i-1}+1", $"{i}"));
            }
             
            yield return new object[]
            {
                new CellDataWithStatusCode("var1", "1", "1", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var2", "2", "2", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var3", "=var1+var2", "3", System.Net.HttpStatusCode.Created),
                new CellDataWithStatusCode("var1", "hi there", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity),
                new CellDataWithStatusCode("var2", "=var1+var3", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity),
                new CellDataWithStatusCode("var3", "=var3+var1", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity)
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}