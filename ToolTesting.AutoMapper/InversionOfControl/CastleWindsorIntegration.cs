using System.Collections.Generic;
using AutoMapper;
using AutoMapper.Mappers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FluentAssertions;
using Xunit;

namespace ToolTesting.AutoMapper.InversionOfControl
{
	/// <summary>
	/// Castle Windsor example.
	/// </summary>
	/// <remarks>
	/// StructureMap example
	/// see  https://github.com/AutoMapper/AutoMapper/blob/develop/src/AutoMapperSamples/CastleWindsorIntegration.cs
	/// </remarks>
	public class CastleWindsorIntegration
	{
		private readonly IWindsorContainer container;

		public CastleWindsorIntegration()
		{
			container = new WindsorContainer();
		}

		[Fact]
		public void Example()
		{
			container.Install(new ConfigurationInstaller());

			var configuration1 = container.Resolve<IConfiguration>();
			var configuration2 = container.Resolve<IConfiguration>();
			configuration1.Should().BeSameAs(configuration2);

			var configurationProvider = container.Resolve<IConfigurationProvider>();
			configurationProvider.Should().BeSameAs(configuration1);

			var configuration = container.Resolve<ConfigurationStore>();
			configuration.Should().BeSameAs(configuration1);

			configuration1.CreateMap<Source, Destination>();

			var engine = container.Resolve<IMappingEngine>();

			var destination = engine.Map<Source, Destination>(new Source { Value = 15 });

			destination.Value.Should().Be(15);
		}

		[Fact]
		public void Example2()
		{
			container.Install(new MappingEngineInstaller());

			Mapper.Reset();

			Mapper.CreateMap<Source, Destination>();

			var engine = container.Resolve<IMappingEngine>();

			var destination = engine.Map<Source, Destination>(new Source { Value = 15 });

			destination.Value.Should().Be(15);
		}

		private class Source
		{
			public int Value { get; set; }
		}

		private class Destination
		{
			public int Value { get; set; }
		}

		public class ConfigurationInstaller : IWindsorInstaller
		{
			public void Install(IWindsorContainer container, IConfigurationStore store)
			{
				container.Register(
					Component.For<IEnumerable<IObjectMapper>>()
						.LifestyleSingleton()
						.UsingFactoryMethod(() => MapperRegistry.Mappers),
					Component.For<ConfigurationStore>().ImplementedBy<ConfigurationStore>(),
					Component.For<IConfigurationProvider>().UsingFactoryMethod(k => k.Resolve<ConfigurationStore>()),
					Component.For<IConfiguration>().UsingFactoryMethod(k => k.Resolve<ConfigurationStore>()),
					Component.For<IMappingEngine>().ImplementedBy<MappingEngine>(),
					Component.For<ITypeMapFactory>().ImplementedBy<TypeMapFactory>());
			}
		}

		public class MappingEngineInstaller : IWindsorInstaller
		{
			public void Install(IWindsorContainer container, IConfigurationStore store)
			{
				container.Register(
					Component.For<IMappingEngine>().UsingFactoryMethod(() => Mapper.Engine));
			}
		}
	}
}