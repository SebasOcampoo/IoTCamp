using Microsoft.Azure.Devices;
using System.Threading.Tasks;

namespace WebJobs_Shared.DeviceManager
{
    public interface IDeviceManager
    {
        Task enable(string mac);
        Task disable(string mac);
        Task remove(string mac);
        Task enableAll();
        Task disableAll();

        #region Twin

        IQuery CreateQuery(string query, int size = -1);
        Task<Twin> GetTwin(string mac);
        Task<Twin> UpdateTwin(string mac, string jsonTwinPatch, string etag);

        #endregion
    }
}
