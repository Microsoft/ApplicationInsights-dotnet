﻿namespace Microsoft.ApplicationInsights.Extensibility.Implementation.Endpoints
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory("Endpoints")]
    public class EndpointProviderTests
    {
        [TestMethod]
        public void TestDefaultEndpoints()
        {
            RunTest(
                connectionString: "InstrumentationKey=00000000-0000-0000-0000-000000000000",
                expectedBreezeEndpoint: Constants.BreezeEndpoint,
                expectedLiveMetricsEndpoint: Constants.LiveMetricsEndpoint,
                expectedProfilerEndpoint: Constants.ProfilerEndpoint,
                expectedSnapshotEndpoint: Constants.SnapshotEndpoint);
        }

        [TestMethod]
        public void TestEndpointSuffix()
        {
            RunTest(
                connectionString: "InstrumentationKey=00000000-0000-0000-0000-000000000000;EndpointSuffix=ai.contoso.com",
                expectedBreezeEndpoint: "https://dc.ai.contoso.com/",
                expectedLiveMetricsEndpoint: "https://live.ai.contoso.com/",
                expectedProfilerEndpoint: "https://profiler.ai.contoso.com/",
                expectedSnapshotEndpoint: "https://snapshot.ai.contoso.com/");
        }

        [TestMethod]
        public void TestEndpointSuffix_WithExplicitOverride()
        {
            RunTest(
                connectionString: "InstrumentationKey=00000000-0000-0000-0000-000000000000;EndpointSuffix=ai.contoso.com;ProfilerEndpoint=https://custom.profiler.contoso.com:444/",
                expectedBreezeEndpoint: "https://dc.ai.contoso.com/",
                expectedLiveMetricsEndpoint: "https://live.ai.contoso.com/",
                expectedProfilerEndpoint: "https://custom.profiler.contoso.com:444/",
                expectedSnapshotEndpoint: "https://snapshot.ai.contoso.com/"); 
        }

        [TestMethod]
        public void TestEndpointSuffix_WithLocation()
        {
            RunTest(
                connectionString: "InstrumentationKey=00000000-0000-0000-0000-000000000000;EndpointSuffix=ai.contoso.com;Location=westus2",
                expectedBreezeEndpoint: "https://westus2.dc.ai.contoso.com/",
                expectedLiveMetricsEndpoint: "https://westus2.live.ai.contoso.com/",
                expectedProfilerEndpoint: "https://westus2.profiler.ai.contoso.com/",
                expectedSnapshotEndpoint: "https://westus2.snapshot.ai.contoso.com/");
        }

        [TestMethod]
        public void TestEndpointSuffix_WithLocation_WithExplicitOverride()
        {
            RunTest(
                connectionString: "InstrumentationKey=00000000-0000-0000-0000-000000000000;EndpointSuffix=ai.contoso.com;Location=westus2;ProfilerEndpoint=https://custom.profiler.contoso.com:444/",
                expectedBreezeEndpoint: "https://westus2.dc.ai.contoso.com/",
                expectedLiveMetricsEndpoint: "https://westus2.live.ai.contoso.com/",
                expectedProfilerEndpoint: "https://custom.profiler.contoso.com:444/",
                expectedSnapshotEndpoint: "https://westus2.snapshot.ai.contoso.com/");
        }

        [TestMethod]
        public void TestExpliticOverride_PreservesSchema()
        {
            RunTest(
                connectionString: "InstrumentationKey=00000000-0000-0000-0000-000000000000;ProfilerEndpoint=http://custom.profiler.contoso.com:444/",
                expectedBreezeEndpoint: Constants.BreezeEndpoint,
                expectedLiveMetricsEndpoint: Constants.LiveMetricsEndpoint,
                expectedProfilerEndpoint: "http://custom.profiler.contoso.com:444/",
                expectedSnapshotEndpoint: Constants.SnapshotEndpoint);
        }

        [TestMethod]
        [ExpectedException(typeof(ConnectionStringInvalidEndpointException))]
        public void TestExpliticOverride_InvalidValue()
        {
            RunTest(
                connectionString: "InstrumentationKey=00000000-0000-0000-0000-000000000000;ProfilerEndpoint=https:////custom.profiler.contoso.com",
                expectedBreezeEndpoint: Constants.BreezeEndpoint,
                expectedLiveMetricsEndpoint: Constants.LiveMetricsEndpoint,
                expectedProfilerEndpoint: Constants.ProfilerEndpoint,
                expectedSnapshotEndpoint: Constants.SnapshotEndpoint);
        }

        [TestMethod]
        [ExpectedException(typeof(ConnectionStringInvalidEndpointException))]
        public void TestExpliticOverride_InvalidValue2()
        {
            RunTest(
                connectionString: "InstrumentationKey=00000000-0000-0000-0000-000000000000;ProfilerEndpoint=https://www.~!@#$%&^*()_{}{}><?<?>:L\":\"_+_+_",
                expectedBreezeEndpoint: Constants.BreezeEndpoint,
                expectedLiveMetricsEndpoint: Constants.LiveMetricsEndpoint,
                expectedProfilerEndpoint: Constants.ProfilerEndpoint,
                expectedSnapshotEndpoint: Constants.SnapshotEndpoint);
        }

        [TestMethod]
        [ExpectedException(typeof(ConnectionStringMissingInstrumentationKeyException))]
        public void TestEndpointProvider_NoInstrumentationKey()
        {
            var endpoint = new EndpointProvider()
            {
                ConnectionString = "key1=value1;key2=value2;key3=value3"
            };

            endpoint.GetInstrumentationKey();
        }

        [TestMethod]
        public void TestEndpointProvider_NoConnectionStringShouldReturnDefaultEndpoints()
        {
            var endpoint = new EndpointProvider();

            Assert.AreEqual(Constants.BreezeEndpoint, endpoint.GetEndpoint(EndpointName.Breeze).AbsoluteUri);
            Assert.AreEqual(Constants.LiveMetricsEndpoint, endpoint.GetEndpoint(EndpointName.LiveMetrics).AbsoluteUri);
            Assert.AreEqual(Constants.ProfilerEndpoint, endpoint.GetEndpoint(EndpointName.Profiler).AbsoluteUri);
            Assert.AreEqual(Constants.SnapshotEndpoint, endpoint.GetEndpoint(EndpointName.Snapshot).AbsoluteUri);
        }

        private void RunTest(string connectionString, string expectedBreezeEndpoint, string expectedLiveMetricsEndpoint, string expectedProfilerEndpoint, string expectedSnapshotEndpoint)
        {
            var endpoint = new EndpointProvider()
            {
                ConnectionString = connectionString
            };

            var breezeTest = endpoint.GetEndpoint(EndpointName.Breeze);
            Assert.AreEqual(expectedBreezeEndpoint, breezeTest.AbsoluteUri);

            var liveMetricsTest = endpoint.GetEndpoint(EndpointName.LiveMetrics);
            Assert.AreEqual(expectedLiveMetricsEndpoint, liveMetricsTest.AbsoluteUri);

            var profilerTest = endpoint.GetEndpoint(EndpointName.Profiler);
            Assert.AreEqual(expectedProfilerEndpoint, profilerTest.AbsoluteUri);

            var snapshotTest = endpoint.GetEndpoint(EndpointName.Snapshot);
            Assert.AreEqual(expectedSnapshotEndpoint, snapshotTest.AbsoluteUri);
        }
    }
}
