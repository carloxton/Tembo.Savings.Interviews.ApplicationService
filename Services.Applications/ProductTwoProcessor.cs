using Services.AdministratorTwo.Abstractions;

using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications;

public class ProductTwoProcessor(
    IAdministrationService adminService,
    IKycService kycService,
    IBus bus,
    DateWrapper dateWrapper)
     : IProductProcessor
{
    public async Task Process(Application application)
    {
        var age = dateWrapper.CalculateAge(application.Applicant.DateOfBirth);
        if (age is < 18)
        {
            await bus.PublishAsync(new ApplicationFailed(application.Id));
            return;
        }

        var kycResult = await kycService.GetKycReportAsync(application.Applicant!);
        if (!kycResult.IsSuccess || !kycResult.Value.IsVerified)
        {
            await bus.PublishAsync(new KycFailed(application.Applicant.Id, Guid.NewGuid()));
            return;
        }
        
        var investorResponse = await adminService.CreateInvestorAsync(application.Applicant);
        await bus.PublishAsync(new InvestorCreated(application.Applicant.Id, investorResponse.Value.ToString()));

        var accountResponse = await adminService.CreateAccountAsync(investorResponse.Value, application.ProductCode);
        await bus.PublishAsync(new AccountCreated(investorResponse.Value.ToString(), application.ProductCode, accountResponse.Value.ToString()));
        await bus.PublishAsync(new ApplicationCompleted(application.Id));
    }
}