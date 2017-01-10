using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;

namespace API.Models
{
    public class TokensRepository : ITokensRepository
    {
        private RegistryManager registryManager;

        public TokensRepository(string iothubConnectionString)
        {
            registryManager = RegistryManager.CreateFromConnectionString(iothubConnectionString);
        }

        public async Task<IToken> Add(string deviceId, bool demo)
        {
            try
            {
                Device device = await registryManager.AddDeviceAsync(new Device(deviceId));
                var twin = await Enable(device.Id, demo);

                return new IotHubToken()
                {
                    DeviceID = device.Id,
                    Token = device.Authentication.SymmetricKey.PrimaryKey,
                    Status = twin.Properties.Desired.Contains("status") ? twin.Properties.Desired["status"] : "Unknown"
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<Twin> Enable(string id, bool demo)
        {
            Twin twin = null;
            try
            {
                twin = await registryManager.GetTwinAsync(id);

                if (demo == true)
                {
                    var patch = new
                    {
                        tags = new { demo = true },
                        properties = new
                        {
                            desired = new { status = "enabled" }
                        }
                    };

                    twin = await registryManager.UpdateTwinAsync(id, JsonConvert.SerializeObject(patch), twin.ETag);

                }
                else
                {
                    var patch = new
                    {
                        properties = new
                        {
                            desired = new { status = "enabled" }
                        }
                    };

                    twin = await registryManager.UpdateTwinAsync(id, JsonConvert.SerializeObject(patch), twin.ETag);
                }

                return twin;

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<IToken> Get(string deviceId)
        {

            Device device = await registryManager.GetDeviceAsync(deviceId);

            if (device == null)
                return null;

            var twin = await registryManager.GetTwinAsync(device.Id);

            return new IotHubToken()
            {
                DeviceID = device.Id,
                Token = device.Authentication.SymmetricKey.PrimaryKey,
                Status = twin.Properties.Desired.Contains("status") ? twin.Properties.Desired["status"] : "Unknown"
            };

        }

        public async Task<IEnumerable<IToken>> GetAll()
        {
            List<IToken> tokens = new List<IToken>();

            try
            {
                IEnumerable<Device> devices;

                devices = await registryManager.GetDevicesAsync(100);
                foreach (Device d in devices)
                {
                    var twin = await registryManager.GetTwinAsync(d.Id);
                    tokens.Add(new DetailedIoTHubToken()
                    {
                        DeviceID = d.Id,
                        Token = d.Authentication.SymmetricKey.PrimaryKey,
                        Demo = twin.Tags.Contains("demo") ? twin.Tags["demo"] : false,
                        Status = twin.Properties.Desired.Contains("status") ? twin.Properties.Desired["status"] : "Unknown"
                    });
                }
            }
            catch (Exception ex)
            {
                throw;
            }


            return tokens;
        }

        public async Task Remove(string deviceId)
        {
            Device device = await registryManager.GetDeviceAsync(deviceId);

            if (device == null)
                throw new DeviceNotFoundException(deviceId);

            await registryManager.RemoveDeviceAsync(device);
        }
    }
}