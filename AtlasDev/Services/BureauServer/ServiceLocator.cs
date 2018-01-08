using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace Atlas.Bureau.Service
{
	public static class ServiceLocator
	{
		private static IKernel _serviceLocator;

		public static void SetServiceLocator(IKernel serviceLocator)
		{
			_serviceLocator = serviceLocator;
		}

		public static T Get<T>()
		{
			return _serviceLocator.GetAll<T>().FirstOrDefault();
		}
	}
}
