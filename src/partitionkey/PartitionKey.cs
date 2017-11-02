using System;
using System.Security.Cryptography;

namespace partitionkey
{
    public class PartitionKey
    {
        private readonly uint _buckets;
        public PartitionKey(uint buckets)
        {
            _buckets = buckets;
        }
        public uint Create() 
        {
            using(SHA256Managed hash = new SHA256Managed())
            {
                Guid g = Guid.NewGuid();
                // https://msdn.microsoft.com/en-us/library/system.guid.tobytearray(v=vs.110).aspx
                byte[] bytes = g.ToByteArray();
                byte[] bucket = hash.ComputeHash(bytes);
                return BitConverter.ToUInt32(bucket, 0) % _buckets;
            }
        }
    }
}
