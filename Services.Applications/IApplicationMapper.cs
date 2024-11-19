using Services.Common.Abstractions.Model;


namespace Services.Applications;

public interface IApplicationMapper<TRequest> where TRequest : class
{
    TRequest Map(Application application);
}

