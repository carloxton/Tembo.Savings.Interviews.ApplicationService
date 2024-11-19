using Services.AdministratorOne.Abstractions;
using Services.AdministratorOne.Abstractions.Model;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications;

public class ProductOneProcessor(IAdministrationService productOneService,
    IApplicationMapper<CreateInvestorRequest> productOneMapper,
    IKycService kycService,
    IBus bus,
    DateWrapper dateWrapper)
     : IProductProcessor
{
    public async Task Process(Application application)
    {
        var age = dateWrapper.CalculateAge(application.Applicant.DateOfBirth);
        if (age is < 18 or > 39)
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

        var accountRequest = productOneMapper.Map(application);
        var accountResponse = productOneService.CreateInvestor(accountRequest);
        await bus.PublishAsync(new InvestorCreated(application.Applicant.Id, accountResponse.InvestorId));
        await bus.PublishAsync(new AccountCreated(accountResponse.InvestorId, application.ProductCode, accountResponse.AccountId));
        await bus.PublishAsync(new ApplicationCompleted(application.Id));
    }
}
