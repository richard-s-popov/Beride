using System.Web.Mvc;
using Microsoft.Practices.Unity;
using TransportSystem.Logics.Impl.Membership;
using TransportSystem.Logics.Impl.SMS;
using TransportSystem.Logics.Impl.Trips;
using TransportSystem.Logics.Interfaces.Membership;
using TransportSystem.Logics.Interfaces.SMS;
using TransportSystem.Logics.Interfaces.Trips;
using Unity.Mvc3;

namespace TransportSystem.Logics.Infrastructure.Unity
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            container.RegisterType<ITripsService, TripsService>();
            container.RegisterType<IUsersService, UsersService>();
            container.RegisterType<IMembershipService, MembershipService>();
            container.RegisterType<IRequestsService, RequestsService>();
            container.RegisterType<ISMS, SMS>();

            return container;
        }
    }
}