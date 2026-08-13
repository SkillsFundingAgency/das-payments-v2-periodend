using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.PeriodEnd.Application.Services;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Payments.PeriodEnd.Function
{
    public static class SuccessfulSubmissions
    {
        [Function("SuccessfulSubmissions")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/PeriodEnd/Submission/Successful")] HttpRequest req,
            FunctionContext context)
        {
            var serviceProvider = context.InstanceServices;
            var providersRequiringReprocessingService = serviceProvider.GetRequiredService<IProvidersRequiringReprocessingService>();
            var logger = serviceProvider.GetRequiredService<IPaymentLogger>();

            if (!short.TryParse(req.Query["academicYear"], out var academicYear) || 
                !byte.TryParse(req.Query["collectionPeriod"], out var collectionPeriod))
                return new StatusCodeResult(400);
            
            logger.LogDebug($"Entering {nameof(SuccessfulSubmissions)} Function for AcademicYear: {academicYear} and Collection Period: {collectionPeriod}");

            var submissionJobs = await providersRequiringReprocessingService.SuccessfulSubmissions(academicYear, collectionPeriod);

            logger.LogInfo("Successfully retrieved latest successful submission jobs");

            return new OkObjectResult(submissionJobs);
        }
    }
}