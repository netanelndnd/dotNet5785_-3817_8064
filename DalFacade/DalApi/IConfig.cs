namespace DalApi;

public interface IConfig
{
    void reset();
    DateTime Clock { get; set; }
    TimeSpan RiskRange { get; set; }
}
