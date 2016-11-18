using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnitTests
{
    [TestFixture]
    public class TestClass
    {
        private const int taskCount = 100000;

        private MyObject MyObjectCreateAndCallAction(Action<MyObject> p_CallIncrementAction)
        {
            var sharedObject = new MyObject();
            Parallel.For(1, 1 + taskCount, i => { p_CallIncrementAction(sharedObject); });
            Console.WriteLine($"Incremented {taskCount}x and Counter={sharedObject.Counter}");
            return sharedObject;
        }

        [Test]
        public void IncNotThreadSafe_IncrementedValueIsIncorrect()
        {
            var myObject = MyObjectCreateAndCallAction(obj => obj.IncNotThreadSafe());
            Assert.AreNotEqual(taskCount, myObject.Counter);
        }

        [Test]
        public void IncThreadSafe_IncrementedValueIsCorrect()
        {
            var myObject = MyObjectCreateAndCallAction(obj => obj.IncThreadSafe());
            Assert.AreEqual(taskCount, myObject.Counter);
        }
    }

    public class MyObject
    {
        private int _Counter;

        public int Counter => _Counter;

        public void IncThreadSafe()
        {
            Interlocked.Increment(ref _Counter);
        }

        public void IncNotThreadSafe()
        {
            //_Counter = _Counter + 1;
            _Counter++;
        }
    }
}