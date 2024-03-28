using LittleSharp.Callables;
using SimpleSetSketching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tests
{
    public class TogglerTest
    {
        [Fact]
        public void TestToogling()
        {
            // Test toogling by iserting identity hash function and assign toogling function
            // Thus [0, 1, 2, 3, 4] should be [0, 1, 2, 3, 4]
            ulong size = 1025;
            int bufferSize = 4;

            ulong[] table = new ulong[size];

            Toggler<ulong[]> toggler = new OneHashIdentityXOR<ulong[]>(table, bufferSize).Toggler;

            ulong[] input = new ulong[size];
            for (ulong i = 0; i < size; i++)
            {
                input[i] = i;
            }

            ISketchStream<ulong> stream = new ArrayLongStream(input);

            var sketch = toggler.ToggleStreamToTable(stream);
            Assert.Equal(table, sketch);
        }
    }
}
