using DO;
namespace DalApi;

public interface IConfig
{
    void Reset();
    DateTime Clock { get; set; }
    TimeSpan RiskRange { get; set; }
}
