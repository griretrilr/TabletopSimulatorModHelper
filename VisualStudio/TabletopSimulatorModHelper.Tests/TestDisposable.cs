using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TabletopSimulatorModHelper.Tests
{
    [TestClass]
    public class TestDisposable
    {
        private class Mock 
        { 
            private class Disposable: TabletopSimulatorModHelper.Disposable
            {
                public Disposable(Mock mock)
                {
                    Mock = mock;
                }

                private Mock Mock { get; set; }

                protected override void Dispose(bool disposing)
                {
                    Mock.OnDispose(disposing);
                }

                ~Disposable()
                {
                    Mock.OnFinalize();
                }
            }

            public Mock()
            {
                Object = new Disposable(this);
            }

            public IDisposable Object { get; private set;  }

            private bool ExpectingDispose { get; set; }

            private bool ExpectingDisposingArgument { get; set; }

            private bool ReceivedDispose { get; set; }

            private bool ExpectingFinalize { get; set; }

            private bool ReceivedFinalize { get; set; }

            public void ExpectDispose(bool disposing)
            {
                Assert.IsFalse(ExpectingDispose);
                ExpectingDispose = true;
                ExpectingDisposingArgument = disposing;
            }

            public void ExpectFinalize()
            {
                Assert.IsFalse(ExpectingFinalize);
                ExpectingFinalize = true;
            }

            private void OnDispose(bool disposing)
            {
                Assert.IsTrue(ExpectingDispose && !ReceivedDispose);
                Assert.AreEqual(disposing, ExpectingDisposingArgument);
                ReceivedDispose = true;
            }

            private void OnFinalize()
            {
                Assert.IsTrue(ExpectingFinalize && !ReceivedFinalize);
                ReceivedFinalize = true;
            }

            public void Verify()
            {
                Assert.AreEqual(ExpectingDispose, ReceivedDispose);
                Assert.AreEqual(ExpectingFinalize, ReceivedFinalize);
            }

            public void ReleaseObject()
            {
                Object = null;
            }
        }

        [TestMethod]
        public void TestExplicitDispose()
        {
            var mock = new Mock();
            mock.ExpectDispose(true);
            mock.Object.Dispose();
            mock.Verify();

            mock.ReleaseObject();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            mock.Verify();
        }

        [TestMethod]
        public void TestImplicitDispose()
        {
            var mock = new Mock();

            mock.ExpectFinalize();
            mock.ExpectDispose(false);
            mock.ReleaseObject();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            mock.Verify();
        }
    }
}
