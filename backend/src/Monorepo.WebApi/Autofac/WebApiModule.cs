using Autofac;
using FluentValidation;
using Monorepo.Domain.Constants;
using Monorepo.Domain.Interfaces;
using Monorepo.WebApi.Configurations.Factories;
using Monorepo.WebApi.Shared.Persistence;
using Monorepo.WebApi.Shared.Persistence.Accessors;
using Monorepo.WebApi.Shared.Persistence.Connections;
using Monorepo.WebApi.Shared.Persistence.Contexts;
using Monorepo.WebApi.Shared.Persistence.Dapper;
using Monorepo.WebApi.Shared.Services.Implementations;
using Monorepo.WebApi.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Monorepo.WebApi.Autofac;

public sealed class WebApiModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var assembly = typeof(Program).Assembly;

        // Infraestrutura Geral

        builder.RegisterType<Microsoft.IO.RecyclableMemoryStreamManager>()
            .AsSelf()
            .SingleInstance();

        builder
            .RegisterType<HttpContextAccessor>()
            .As<IHttpContextAccessor>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<LeitorDeArquivos>()
            .As<ILeitorDeArquivos>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<LogControleService>()
            .As<ILogControleService>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<HttpResponseFactory>()
            .AsImplementedInterfaces()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(IValidator<>))
            .InstancePerLifetimeScope();

        // Persistência e Transação

        builder
            .RegisterType<UnitOfWork>()
            .As<IUnitOfWork>()
            .InstancePerLifetimeScope();

        builder.RegisterType<DbContextAccessor>()
            .As<IDbContextAccessor<AppDbContext>>()
            .InstancePerLifetimeScope();

        // Dapper e Conexões

        builder
            .RegisterType<DapperMapper>()
            .AsSelf()
            .SingleInstance()
            .OnActivated(_ => DapperMapper.Iniciar())
            .AutoActivate();

        builder
            .RegisterType<DbConnectionFactoryProvider>()
            .As<IDbConnectionFactoryProvider>()
            .InstancePerLifetimeScope();



        builder
            .Register(c =>
            {
                var configuration = c.Resolve<IConfiguration>();
                var connectionString = SecretConfiguration.GetSecretConfigurationValue("DapperConnection", configuration);
                return new DbConnectionFactory(connectionString);
            })
            .Named<IDbConnectionFactory>(DbConnections.Dapper)
            .As<IDbConnectionFactory>()
            .InstancePerLifetimeScope();

        builder
            .Register(c =>
            {
                var configuration = c.Resolve<IConfiguration>();
                var connectionString = SecretConfiguration.GetSecretConfigurationValue("LogConnection", configuration);
                return new DbConnectionFactory(connectionString);
            })
            .Named<IDbConnectionFactory>(DbConnections.Log)
            .As<IDbConnectionFactory>()
            .InstancePerLifetimeScope();

        // CQRS - Handlers

        builder
            .RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(IQueryHandler<,>))
            .InstancePerLifetimeScope();

        builder
            .RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(ICommandHandler<,>))
            .InstancePerLifetimeScope();

        builder
            .RegisterAssemblyTypes(assembly)
            .Where(t => t.IsClosedTypeOf(typeof(IService<>)))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        // EF Core

        builder
            .Register(c =>
            {
                var configuration = c.Resolve<IConfiguration>();
                var env = c.Resolve<IHostEnvironment>();
                var connectionString = SecretConfiguration.GetSecretConfigurationValue("SicredMatoneConnection", configuration);

                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder
                    .UseSqlServer(connectionString, sqlOptions => { sqlOptions.MigrationsAssembly("Juridico.Inicial.Migrations"); })
                    .UseLazyLoadingProxies();

                if (env.IsDevelopment())
                {
                    optionsBuilder
                        .EnableDetailedErrors() // Detalha erros de conversão de dados
                        .EnableSensitiveDataLogging(); // Mostra os valores dos parâmetros no Log
                }

                return new AppDbContext(optionsBuilder.Options);
            })
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}
