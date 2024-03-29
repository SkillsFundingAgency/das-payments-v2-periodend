using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.JobContextMessageHandling;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.PeriodEnd.PeriodEndService
{
    public class PeriodEndService : StatelessService
    {
        private readonly ILifetimeScope lifetimeScope;
        private readonly IPaymentLogger logger;
        private IJobContextManagerService jobContextManagerService;

        public PeriodEndService(StatelessServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger logger)
            : base(context)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new List<ServiceInstanceListener>
            {
                new ServiceInstanceListener(context => lifetimeScope.Resolve<IStatelessEndpointCommunicationListener>())
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogDebug("Starting the DC JobContextMessageService for the Period End service.");
                jobContextManagerService = lifetimeScope.Resolve<IJobContextManagerService>();
                await jobContextManagerService.RunAsync(cancellationToken);
            }
            catch (Exception ex) when (!(ex is TaskCanceledException))
            {
                logger.LogError($"Error starting the job context manager. Error: {ex.Message}", ex);
                throw;
            }
        }
    }
}