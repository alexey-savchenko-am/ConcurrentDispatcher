using AutoFixture;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Dispatcher.Tests.UnitTests
{
    public class DispatcherTests
    {

        private int _count;

        [Theory]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public async Task CheckWheatherAllThreadsAreSuccessfullyCompleted(int threadCount)
        {
            var dispatcher = new ConcurrentDispatcher<int>();

            var tasks = Enumerable
                .Range(1, threadCount)
                .Select(x => Task.Run(
                   () => dispatcher.Dispatch(1, Handler)                   
                 ));

            await Task.WhenAll(tasks);

            while (dispatcher.IsWorking) { }

            Assert.Equal(threadCount, _count);
            Assert.Equal(0, dispatcher.DispatcherCount);

        }



        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void CheckWheatherThreadsParallelByContextCorrectly(int itemCount)
        {
            var dispatcher = new ContextBasedDispatcher<string, int>();

            var fixture = new Fixture();

            var firstArr = fixture.CreateMany<int>(itemCount);
            var secondArr = fixture.CreateMany<int>(itemCount);

            var commonSum = firstArr.Sum() + secondArr.Sum();

            var sw = new Stopwatch();

            sw.Start();

            Parallel.ForEach(firstArr, (x) =>
            {
                dispatcher.Dispatch("first", x, Handler);
            });


            Parallel.ForEach(secondArr, (x) =>
            {
                dispatcher.Dispatch("second", x, Handler);
            });

            while (dispatcher.IsWorking) { Console.WriteLine(dispatcher.ContextDispatcherCount); }

            sw.Stop();
            var ms = sw.ElapsedMilliseconds;

            Assert.Equal(commonSum, _count);
            Assert.Equal(2, dispatcher.ContextDispatcherCount);

        }


        private Task Handler(int val)
        {
            Interlocked.Add(ref _count, val);

            return Task.CompletedTask;
        }



    }
}
