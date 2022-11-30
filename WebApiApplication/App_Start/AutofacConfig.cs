using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using WebApiApplication.DataAccess.Interfaces;
using WebApiApplication.DataAccess;
using WebApiApplication.Models;

namespace WebApiApplication.App_Start
{
    public class AutofacConfig
    {
        public static void Config()
        {
            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            // OPTIONAL: Register the Autofac model binder provider.
            builder.RegisterWebApiModelBinderProvider();

            SetupResolveRules(builder);

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static void SetupResolveRules(ContainerBuilder builder)
        {
            builder.RegisterType<WebApiDBContext>().As<WebApiDBContext>().InstancePerRequest();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>));

            RegisteServices(builder);
            RegisteRepositories(builder);
            RegisterDaos(builder);
        }

        private static void RegisteServices(ContainerBuilder builder)
        {
            var types = (from t in Assembly.GetExecutingAssembly().GetTypes()
                         where t.Name.EndsWith("Service")
                         && t.IsPublic
                         select t).ToArray();

            builder.RegisterTypes(types).AsSelf();
        }

        private static void RegisteRepositories(ContainerBuilder builder)
        {
            var types = (from t in Assembly.GetExecutingAssembly().GetTypes()
                         where t.Name.EndsWith("Repository")
                         && t.IsPublic
                         select t).ToArray();

            builder.RegisterTypes(types).AsImplementedInterfaces();
        }

        private static void RegisterDaos(ContainerBuilder builder)
        {
            var types = (from t in Assembly.GetExecutingAssembly().GetTypes()
                         where t.Name.EndsWith("Dao")
                         && t.IsPublic
                         select t).ToArray();

            builder.RegisterTypes(types).AsImplementedInterfaces();
        }
    }
}