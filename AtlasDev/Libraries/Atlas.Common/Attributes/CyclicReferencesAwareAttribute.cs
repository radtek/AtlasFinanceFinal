using System;
using System.ServiceModel.Description;

using Atlas.Common.Behaviors;


namespace Atlas.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
	public class CyclicReferencesAwareAttribute : Attribute, IContractBehavior, IOperationBehavior
	{
		private readonly bool _on = true;

		public CyclicReferencesAwareAttribute(bool on)
		{
			_on = on;
		}


		public bool On
		{
			get { return (_on); }
		}


		#region IOperationBehavior Members

		void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
		}

		void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation)
		{
			CyclicReferencesAwareContractBehavior.ReplaceDataContractSerializerOperationBehavior(operationDescription, On);
		}

		void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation)
		{
			CyclicReferencesAwareContractBehavior.ReplaceDataContractSerializerOperationBehavior(operationDescription, On);
		}

		void IOperationBehavior.Validate(OperationDescription operationDescription)
		{
		}

		#endregion


		#region IContractBehavior Members

		void IContractBehavior.AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
		}

		void IContractBehavior.ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
		{
			CyclicReferencesAwareContractBehavior.ReplaceDataContractSerializerOperationBehaviors(contractDescription, On);
		}

		void IContractBehavior.ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime)
		{
			CyclicReferencesAwareContractBehavior.ReplaceDataContractSerializerOperationBehaviors(contractDescription, On);
		}

		void IContractBehavior.Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
		{
		}

		#endregion

	}
}