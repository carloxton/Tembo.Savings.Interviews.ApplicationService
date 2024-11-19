using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications;

public class ApplicationProcessor(
    ProductOneProcessor productOneProcessor,
    ProductTwoProcessor productTwoProcessor) 
    : IApplicationProcessor
{
    public async Task Process(Application application)
    {
        ArgumentNullException.ThrowIfNull(application, nameof(application));

        switch (application?.ProductCode)
        {
            case ProductCode.ProductOne:
                await productOneProcessor.Process(application);
                break;

            case ProductCode.ProductTwo:
                await productTwoProcessor.Process(application);
                break;

            default:
                throw new NotImplementedException("ProductCode is unknown.");

        }               
    }    
}

