using MvvmCross.Platform;
using System;

namespace XamarinCore.Helpers
{
	public static class MvxHelper
	{
		public static void RegisterSingletonIfNotExist<TInterface>(Func<TInterface> serviceConstructor)
			where TInterface : class
		{
			if (!Mvx.CanResolve<TInterface>())
			{
				Mvx.RegisterSingleton(serviceConstructor);
			}
		}

		public static void RegisterSingletonIfNotExist(Type tInterface, Func<object> serviceConstructor)
		{
			if (!Mvx.CanResolve(tInterface))
			{
				Mvx.RegisterSingleton(tInterface, serviceConstructor);
			}
		}

		public static void RegisterSingletonIfNotExist<TInterface>(TInterface service)
			where TInterface : class
		{
			if (!Mvx.CanResolve<TInterface>())
			{
				Mvx.RegisterSingleton(service);
			}
		}

		public static void RegisterSingletonIfNotExist(Type tInterface, object service)
		{
			if (!Mvx.CanResolve(tInterface))
			{
				Mvx.RegisterSingleton(tInterface, service);
			}
		}
	}
}
