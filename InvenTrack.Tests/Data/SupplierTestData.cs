using InvenTrack.DTOs.Supplier;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace InvenTrack.Tests.Data
{
    public class SupplierTestData: IEnumerable<object[]>
    {

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                  new UpdateSupplierDto
            {
                Name = "ABC Supplier",
                Email = "abc@gmail.com",
                Phone = "9999999999",
                Address = "Delhi"
            }
            };


            yield return new object[]
      {
            new UpdateSupplierDto
            {
                Name = "XYZ Supplier",
                Email = "xyz@gmail.com",
                Phone = "8888888888",
                Address = "Mumbai"
            }
      };

            yield return new object[]
            {
            new UpdateSupplierDto
            {
                Name = "PQR Supplier",
                Email = "pqr@gmail.com",
                Phone = "7777777777",
                Address = "Pune"
            }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
