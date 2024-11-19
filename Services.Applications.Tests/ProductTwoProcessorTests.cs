using Moq;
using Services.AdministratorTwo.Abstractions;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications.Tests;

public class ProductTwoProcessorTests
{
    [Theory]
    [InlineData(2007, 1 ,1)]    
    // Add better edge cases
    public async Task WhenAgeLessThan18_ExpectFailureEvent(int yearOfBirth, int monthOfBirth, int dayOfBirth)
    {
        // Arrange
        Mock<IAdministrationService> service = new();
        service.Setup(m => m.CreateInvestorAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Success(Guid.Empty));
        Mock<IKycService> kyc = new();
        Mock<IBus> bus = new();
        DateWrapper dateWrapper = new(TestFakes.Today);
        ProductTwoProcessor processor = new(
            service.Object,
            kyc.Object,
            bus.Object,
            dateWrapper);
            
        // Act
        await processor.Process(TestFakes.ApplicationTwo(yearOfBirth, monthOfBirth, dayOfBirth));

        // Assert
        bus.Verify(v => v.PublishAsync(It.IsAny<ApplicationFailed>()));        
    }

    [Fact]
    public async Task WhenKycCheckSuccessful_ExpectSuccessEvents()
    {
        Mock<IAdministrationService> service = new();
        service.Setup(m => m.CreateInvestorAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Success(Guid.Empty));
        service.Setup(m => m.CreateAccountAsync(It.IsAny<Guid>(), It.IsAny<ProductCode>()))
            .ReturnsAsync(Result.Success(Guid.Empty));
        Mock<IKycService> kyc = new();
        Mock<IBus> bus = new();
        kyc.Setup(m => m.GetKycReportAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Success(new KycReport(Guid.Empty, true)));
        DateWrapper dateWrapper = new(TestFakes.Today);
        ProductTwoProcessor processor = new(
            service.Object,            
            kyc.Object,
            bus.Object,
            dateWrapper);

        await processor.Process(TestFakes.ApplicationTwo());

        bus.Verify(v => v.PublishAsync(It.IsAny<InvestorCreated>()));
        bus.Verify(v => v.PublishAsync(It.IsAny<AccountCreated>()));
        bus.Verify(v => v.PublishAsync(It.IsAny<ApplicationCompleted>()));
    }

    [Fact]
    public async Task WhenKycCheckError_ExpectKycFailedEvent()
    {
        Mock<IAdministrationService> service = new();
        Mock<IKycService> kyc = new();
        kyc.Setup(m => m.GetKycReportAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Failure<KycReport>(new Error("", "", "")));
        Mock<IBus> bus = new();
        DateWrapper dateWrapper = new(TestFakes.Today);
        ProductTwoProcessor processor = new(
            service.Object,
            kyc.Object,
            bus.Object,
            dateWrapper);

        await processor.Process(TestFakes.ApplicationTwo());

        bus.Verify(v => v.PublishAsync(It.IsAny<KycFailed>()));
    }

    [Fact]
    public async Task WhenKycCheckNotVerified_ExpectKycFailedEvent()
    {
        Mock<IAdministrationService> service = new();
        Mock<IKycService> kyc = new();
        kyc.Setup(m => m.GetKycReportAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Success(new KycReport(Guid.Empty, false)));
        Mock<IBus> bus = new();
        DateWrapper dateWrapper = new(TestFakes.Today);
        ProductTwoProcessor processor = new(
        service.Object,
        kyc.Object,
        bus.Object,
        dateWrapper);

        await processor.Process(TestFakes.ApplicationTwo());

        bus.Verify(v => v.PublishAsync(It.IsAny<KycFailed>()));        
    }
}