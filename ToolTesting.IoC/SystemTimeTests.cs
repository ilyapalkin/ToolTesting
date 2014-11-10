using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using FluentAssertions;
using Microsoft.Practices.Unity;

using Xunit;
using Xunit.Extensions;

namespace ToolTesting.IoC
{
    /// <summary>
    /// See: https://vimeo.com/111091466 at 19:49
    /// 
    /// It might lead to 'failed' tests since they use shared context.
    /// 
    /// There might be a solution but I do not know about it.
    /// </summary>
    public class StaticSystemTimeTests
    {
        [Fact]
        public void Yesterday()
        {
            SystemTime.Now = () => DateTime.Now.AddDays(-1);
            SystemTime.Now().Should().Be(DateTime.Now.AddDays(-1));
        }

        [Fact]
        public void Today()
        {
            SystemTime.Now().Should().BeCloseTo(DateTime.Now);
        }

        [Fact]
        public void Tomorrow()
        {
            SystemTime.Now = () => DateTime.Now.AddDays(1);
            SystemTime.Now().Should().Be(DateTime.Now.AddDays(1));
        }

        static class SystemTime
        {
            public static Func<DateTime> Now = () => DateTime.Now;
        }
    }

    public class InjectedSystemTimeTests
    {
        readonly IWindsorContainer container;

        public InjectedSystemTimeTests()
        {
            container = new WindsorContainer();
        }

        [Fact]
        public void Yesterday()
        {
            container.Register(Component
                .For<ISystemTime>()
                .UsingFactoryMethod(() => new FakeSystemTime(DateTime.Now.AddDays(-1))));

            container.Resolve<ISystemTime>().Now().Should().BeCloseTo(DateTime.Now.AddDays(-1));
        }

        [Fact]
        public void Today()
        {
            container.Register(Component
                .For<ISystemTime>()
                .ImplementedBy<SystemTime>());

            container.Resolve<ISystemTime>().Now().Should().BeCloseTo(DateTime.Now);
        }

        [Fact]
        public void Tomorrow()
        {
            container.Register(Component
                .For<ISystemTime>()
                .UsingFactoryMethod(() => new FakeSystemTime(DateTime.Now.AddDays(1))));

            container.Resolve<ISystemTime>().Now().Should().BeCloseTo(DateTime.Now.AddDays(1));
        }

        interface ISystemTime
        {
            Func<DateTime> Now { get; }
        }

        class FakeSystemTime : ISystemTime
        {
            private DateTime now;

            public FakeSystemTime(DateTime now)
            {
                this.now = now;
            }

            public Func<DateTime> Now
            {
                get { return () => now; }
            }
        }

        class SystemTime : ISystemTime
        {
            public Func<DateTime> Now
            {
                get { return () => DateTime.Now; }
            }
        }
    }
}