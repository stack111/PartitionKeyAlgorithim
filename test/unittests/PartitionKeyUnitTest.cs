using System;
using Xunit;
using partitionkey;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace unittests
{
    public class PartitionKeyUnitTest
    {
        readonly ITestOutputHelper _output;
        public PartitionKeyUnitTest(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Theory]
        [InlineData(1000, 100, -1, 1, 1.1)]
        [InlineData(10000000, 100, -1, 1, 1.1)]
        [InlineData(100000000, 5000, -1, 1, 1.1)]
        public void Validate(uint numberOfRows, uint buckets, int bucketCountStepUp, double percent, double delta)
        {
            Dictionary<uint, uint> map = new Dictionary<uint, uint>();
            int rowCounter = 0;
            for(int i = 0; i < numberOfRows; i++)
            {
                if(bucketCountStepUp != -1 && rowCounter == bucketCountStepUp)
                {
                    buckets += 5;
                    rowCounter = 0;
                }
                PartitionKey partitionKey = new PartitionKey(buckets);
                uint bucket = partitionKey.Create();
                if(map.ContainsKey(bucket)) 
                    map[bucket]++;
                else map[bucket] = 1;
                rowCounter++;
            }
            
            double lower = percent - delta;
            double higher = percent + delta;
            foreach(var bucket in map)
            {
                double percentOfTotal =  ((double)bucket.Value / (double)numberOfRows) * 100;
                _output.WriteLine("{0} {1} {2}", percentOfTotal, lower, higher);
                Assert.InRange(percentOfTotal, lower, higher);
            }
        }
    }
}
