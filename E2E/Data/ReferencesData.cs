using System.Collections;
using System.Collections.Generic;
using E2E.Data.Contracts;

namespace E2E.Data
{
    public class ValidReferencesFunctions : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new SheedCellDataWithStatusCode[]
                {
                    new("sheet1", "var1", "1", "1", System.Net.HttpStatusCode.Created),
                    new("sheet1", "var2", "2", "2", System.Net.HttpStatusCode.Created),
                    new("sheet2", "var1", "3", "3", System.Net.HttpStatusCode.Created),
                    new("sheet2", "var2", "4", "4", System.Net.HttpStatusCode.Created),
                    new("sheet3", "var1", "=external_ref(http://localhost:8080/api/v1/sheet1/var1)", "1", System.Net.HttpStatusCode.Created),
                    new("sheet3", "var2", "=external_ref(http://localhost:8080/api/v1/sheet1/var1)+external_ref(http://localhost:8080/api/v1/sheet2/var1)", "4", System.Net.HttpStatusCode.Created),
                    new("sheet3", "var3", "=min(external_ref(http://localhost:8080/api/v1/sheet1/var1),external_ref(http://localhost:8080/api/v1/sheet1/var2))", "1", System.Net.HttpStatusCode.Created),
                    new("sheet", "var8", "=external_ref(http://localhost:8080/api/v1/sheet2/var2)/external_ref(http://localhost:8080/api/v1/sheet1/var2)", "2", System.Net.HttpStatusCode.Created)
                },
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class InvalidReferencesFunctions : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new SheedCellDataWithStatusCode[]
                {
                    new("sheet1", "var1", "1", "1", System.Net.HttpStatusCode.Created),
                    new("sheet1", "var2", "2", "2", System.Net.HttpStatusCode.Created),
                    new("sheet2", "var1", "3", "3", System.Net.HttpStatusCode.Created),
                    new("sheet2", "var2", "4", "4", System.Net.HttpStatusCode.Created),
                    new("sheet3", "var1", "=external_ref(http://localhos/var2)", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity),
                    new("sheet3", "var2", "=external_ref(http://localhost:8080/api/v1/sheet1/var1)+external_ref(http://localhost:8080/api/v1/sheet2/var1234)", "ERROR", System.Net.HttpStatusCode.UnprocessableEntity)
                },
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}