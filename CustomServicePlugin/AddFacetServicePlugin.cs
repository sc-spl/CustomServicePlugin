using System;
using System.Linq;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Operations;
using Sitecore.XConnect.Service.Plugins;

namespace CustomServicePlugin
{
    public class AddFacetServicePlugin : IXConnectServicePlugin
    {
        private XdbContextConfiguration _config;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _config != null)
            {
                Unregister();
            }
        }

        public void Register(XdbContextConfiguration config)
        {
            _config = config;
            config.OperationExecuting += ConfigOnOperationCompleted;
        }

        public void Unregister()
        {
            _config.OperationExecuting -= ConfigOnOperationCompleted;
        }

        private void ConfigOnOperationCompleted(object sender, XdbOperationEventArgs xdbOperationEventArgs)
        {
            if (xdbOperationEventArgs.Operation is AddContactOperation operation && operation.Status != XdbOperationStatus.Canceled)
            {
                XdbOperationBatch batch = operation.Batch;

                // we add a facet only if it is not already set
                if (HasSetFacetOperation(batch))
                {
                    return;
                }

                IEntityReference<Contact> contact = operation.Entity;
                batch.Add(new SetFacetOperation(new FacetReference(contact ?? operation.Target, PersonalInformation.DefaultFacetKey), new PersonalInformation { FirstName = "Sergey", LastName = "Plashenko" }));
            }
        }

        private bool HasSetFacetOperation(XdbOperationBatch batch)
        {
            var setFacetOperations = batch.Operations.OfType<SetFacetOperation>();
            return setFacetOperations.Any(op => op.FacetReference.FacetKey == PersonalInformation.DefaultFacetKey);
        }
    }
}
