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
        [InlineData(100000, 2000, -1, 0.02, 0.25)]
        public void Validate(int numberOfRows, uint buckets, int bucketCountStepUp, double percent, double delta)
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
            }
            
            double lower = percent - delta;
            double higher = percent + delta;
            foreach(var bucket in map)
            {
                double percentOfTotal =  (double)bucket.Value / (double)numberOfRows;
                _output.WriteLine("{0} {1} {2}", percentOfTotal, lower, higher);
                Assert.InRange(percentOfTotal, lower, higher);
            }
        }
    }
}
