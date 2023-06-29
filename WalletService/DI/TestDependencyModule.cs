using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using TechTalk.SpecFlow;
using UserService.Clients;
using UserService.Steps;
using WalletService.Clients;
using WalletService.Steps;
using UserService.DI;
using UserService.Observers;
using WalletService.observers;

namespace WalletService.DI
{
    internal class TestDependencyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WalletServiceClient>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<WalletServiceSteps>()
                .AsSelf();

            builder.RegisterType<WalletServiceAsserts>()
                .AsSelf();

            builder.RegisterType<TestDataObserver>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<TransactionTestObserver>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<ScenarioContext>()
                .AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<UserServiceClient>()
                .AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<UserSteps>()
                .AsSelf();

            builder.RegisterType<UserServiceAsserts>()
                .AsSelf();
        }
    }
}
