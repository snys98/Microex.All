using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Microex.All.EntityFramework
{
    public class PrettyStringGuidGenerator : ValueGenerator<string>
    {
        private long _counter = DateTime.UtcNow.Ticks;

        /// <summary>Gets a value to be assigned to a property.</summary>
        /// <para>The change tracking entry of the entity for which the value is being generated.</para>
        /// <returns> The value to be assigned to a property. </returns>
        public override string Next(EntityEntry entry)
        {
            byte[] byteArray = Guid.NewGuid().ToByteArray();
            byte[] bytes = BitConverter.GetBytes(Interlocked.Increment(ref this._counter));
            if (!BitConverter.IsLittleEndian)
                Array.Reverse((Array)bytes);
            byteArray[8] = bytes[1];
            byteArray[9] = bytes[0];
            byteArray[10] = bytes[7];
            byteArray[11] = bytes[6];
            byteArray[12] = bytes[5];
            byteArray[13] = bytes[4];
            byteArray[14] = bytes[3];
            byteArray[15] = bytes[2];
            return new Guid(byteArray).ToString("N");
        }

        /// <summary>
        ///     Gets a value indicating whether the values generated are temporary or permanent. This implementation
        ///     always returns false, meaning the generated values will be saved to the database.
        /// </summary>
        public override bool GeneratesTemporaryValues
        {
            get
            {
                return false;
            }
        }
    }
}
