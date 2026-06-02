namespace TLAManager.Application.Interfaces;

public interface IAcceptTlaApplicationService
{
    Task AcceptTlaAsync(string groupName, string tlaName);
}
