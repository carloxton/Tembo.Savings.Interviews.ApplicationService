using Services.Common.Abstractions.Model;

namespace Services.Applications;

public interface IProductProcessor
{
    Task Process(Application application);
}

