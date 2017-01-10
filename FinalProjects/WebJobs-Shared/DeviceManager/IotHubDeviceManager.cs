using Microsoft.Azure.Devices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebJobs_Shared.DeviceManager
{
    public class IotHubDeviceManager : IDeviceManager
    {
        private RegistryManager _registryManager;

        public IotHubDeviceManager(string connectionString)
        {
            _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }

        public async Task disable(string mac)
        {
            var device = await _registryManager.GetDeviceAsync(mac);
            device.Status = DeviceStatus.Disabled;
            await _registryManager.UpdateDeviceAsync(device);
        }

        public async Task enable(string mac)
        {
            var device = await _registryManager.GetDeviceAsync(mac);
            device.Status = DeviceStatus.Enabled;
            await _registryManager.UpdateDeviceAsync(device);
        }

        public async Task remove(string mac)
        {
            await _registryManager.RemoveDeviceAsync(mac);
        }

        public async Task enableAll()
        {
            var stats = await _registryManager.GetRegistryStatisticsAsync();
            Console.WriteLine("device count: " + stats.TotalDeviceCount + " enabled: " + stats.EnabledDeviceCount + " disabled: " + stats.DisabledDeviceCount);
            var devices = await _registryManager.GetDevicesAsync((int)stats.TotalDeviceCount * 100); // * 100 because the device list is an approximation 
            devices = devices.Select(d => { d.Status = DeviceStatus.Enabled; return d; });
            await _registryManager.UpdateDevices2Async(devices);
        }

        public async Task disableAll()
        {
            var devices = await _registryManager.GetDevicesAsync(1);
            devices.Select(d => { d.Status = DeviceStatus.Disabled; return d; });
            await _registryManager.UpdateDevices2Async(devices);
        }

        #region Twin

        public IQuery CreateQuery(string query, int size)
        {
            if (size != -1)
                return _registryManager.CreateQuery(query, size);
            else
                return _registryManager.CreateQuery(query);
        }

        public async Task<Twin> GetTwin(string mac)
        {
            var twin = await _registryManager.GetTwinAsync(mac);
            return twin;
        }

        public async Task<Twin> UpdateTwin(string mac, string jsonTwinPatch, string etag)
        {
            var twin = await _registryManager.UpdateTwinAsync(mac, jsonTwinPatch, etag);
            return twin;
        }

        #endregion
    }
}
