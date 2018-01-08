using Ninject;

namespace Atlas.Fraud.Engine
{
	public static class Kernel
	{
		private static IKernel _kernel = null;

		public static IKernel GetKernel()
		{
			return _kernel;
		}

		public static void SetKernel(IKernel kernel)
		{
			_kernel = kernel;
		}
	}
}
