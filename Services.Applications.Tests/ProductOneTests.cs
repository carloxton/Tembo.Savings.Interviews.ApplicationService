using Moq;
using Services.AdministratorOne.Abstractions;
using Services.AdministratorOne.Abstractions.Model;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications.Tests;
public class ProductOneTests
{

    [Theory]
    [InlineData(2007, 1 ,1)]
    [InlineData(1983, 1, 1)]
    // Add better edge cases
    public async Task WhenAgeBetweenNot18And39_ExpectFailureEvent(int yearOfBirth, int monthOfBirth, int dayOfBirth)
    {
        // Arrange
        Mock<IAdministrationService> service = new();
        service.Setup(m => m.CreateInvestor(It.IsAny<CreateInvestorRequest>()))
            .Returns(TestFakes.CreateInvestorResponse());
        Mock<IApplicationMapper<CreateInvestorRequest>> mapper = new();
        mapper.Setup(m => m.Map(It.IsAny<Application>()))
            .Returns(new CreateInvestorRequest());
        Mock<IKycService> kyc = new();
        Mock<IBus> bus = new();
        DateWrapper dateWrapper = new(TestFakes.Today);
        IApplicationProcessor processor = new ApplicationProcessor(
            service.Object,
            mapper.Object,
            kyc.Object,
            bus.Object,
            dateWrapper);
            
        // Act
        await processor.Process(TestFakes.Application(yearOfBirth, monthOfBirth, dayOfBirth));

        // Assert
        bus.Verify(v => v.PublishAsync(It.IsAny<ApplicationFailed>()));        
    }

    [Theory]
    [InlineData(2005, 1, 1)]
    [InlineData(1985, 1, 1)]
    // Add better edge cases
    public async Task WhenAgeBetween18And39_ExpectSuccessEvent(int yearOfBirth, int monthOfBirth, int dayOfBirth)
    {
        Mock<IAdministrationService> service = new();
        service.Setup(m => m.CreateInvestor(It.IsAny<CreateInvestorRequest>()))
            .Returns(TestFakes.CreateInvestorResponse());
        Mock<IApplicationMapper<CreateInvestorRequest>> mapper = new();
        mapper.Setup(m => m.Map(It.IsAny<Application>()))
            .Returns(new CreateInvestorRequest());
        Mock<IKycService> kyc = new();
        kyc.Setup(m => m.GetKycReportAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Success(new KycReport(Guid.Empty, true)));
        Mock<IBus> bus = new();
        DateWrapper dateWrapper = new(TestFakes.Today);
        IApplicationProcessor processor = new ApplicationProcessor(
            service.Object,
            mapper.Object,
            kyc.Object,
            bus.Object,
            dateWrapper);
        
        await processor.Process(TestFakes.Application(yearOfBirth, monthOfBirth, dayOfBirth));

        bus.Verify(v => v.PublishAsync(It.IsAny<InvestorCreated>()));
        bus.Verify(v => v.PublishAsync(It.IsAny<AccountCreated>()));
        bus.Verify(v => v.PublishAsync(It.IsAny<ApplicationCompleted>()));
    }

    [Fact]
    public async Task WhenKycCheckSuccessful_ExpectSuccessEvents()
    {
        Mock<IAdministrationService> service = new();
        service.Setup(m => m.CreateInvestor(It.IsAny<CreateInvestorRequest>()))
            .Returns(TestFakes.CreateInvestorResponse());
        Mock<IApplicationMapper<CreateInvestorRequest>> mapper = new();
        mapper.Setup(m => m.Map(It.IsAny<Application>()))
            .Returns(new CreateInvestorRequest());
        Mock<IKycService> kyc = new();
        Mock<IBus> bus = new();
        kyc.Setup(m => m.GetKycReportAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Success(new KycReport(Guid.Empty, true)));
        DateWrapper dateWrapper = new(TestFakes.Today);
        IApplicationProcessor applicationProcessor = new ApplicationProcessor(
            service.Object,
            mapper.Object,
            kyc.Object,
            bus.Object,
            dateWrapper);
                
        await applicationProcessor.Process(TestFakes.Application());

        bus.Verify(v => v.PublishAsync(It.IsAny<InvestorCreated>()));
        bus.Verify(v => v.PublishAsync(It.IsAny<AccountCreated>()));
        bus.Verify(v => v.PublishAsync(It.IsAny<ApplicationCompleted>()));        
    }

    [Fact]
    public async Task WhenKycCheckError_ExpectKycFailedEvent()
    {
        Mock<IAdministrationService> service = new();
        Mock<IApplicationMapper<CreateInvestorRequest>> mapper = new();
        Mock<IKycService> kyc = new();
        kyc.Setup(m => m.GetKycReportAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Failure<KycReport>(new Error("", "", "")));
        Mock<IBus> bus = new();
        DateWrapper dateWrapper = new(TestFakes.Today);
        IApplicationProcessor processor = new ApplicationProcessor(
            service.Object,
            mapper.Object,
            kyc.Object,
            bus.Object,
            dateWrapper);        

        await processor.Process(TestFakes.Application());

        bus.Verify(v => v.PublishAsync(It.IsAny<KycFailed>()));
    }

    [Fact]
    public async Task WhenKycCheckNotVerified_ExpectKycFailedEvent()
    {
        Mock<IAdministrationService> service = new();
        Mock<IApplicationMapper<CreateInvestorRequest>> mapper = new();
        Mock<IKycService> kyc = new();
        kyc.Setup(m => m.GetKycReportAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Success(new KycReport(Guid.Empty, false)));
        Mock<IBus> bus = new();
        DateWrapper dateWrapper = new(TestFakes.Today);
        IApplicationProcessor processor = new ApplicationProcessor(
            service.Object,
            mapper.Object,
            kyc.Object,
            bus.Object,
            dateWrapper);        

        await processor.Process(TestFakes.Application());
        
        bus.Verify(v => v.PublishAsync(It.IsAny<KycFailed>()));
    }
}