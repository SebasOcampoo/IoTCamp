using API.Models;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebJobs_Shared.DeviceManager;

namespace API.Controllers
{
    public class TokensController : ApiController
    {
        private ITokensRepository _tokenRepository;
        private IDeviceValidator _deviceValidator;

        public TokensController(ITokensRepository tokenRepository, IDeviceValidator deviceValidator)
        {
            _tokenRepository = tokenRepository;
            _deviceValidator = deviceValidator;
        }

        // GET: api/Tokens
        public async Task<IEnumerable<IToken>> Get()
        {
            return await _tokenRepository.GetAll();
        }

        // GET: api/Tokens/{id}
        public async Task<IToken> Get(string id)
        {
            id = id.ToUpper();
            IToken token = await _tokenRepository.Get(id);

            if (token == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Device \"" + id + "\" not found."),
                    ReasonPhrase = "Device not found"
                });
            }

            return token;
        }

        // POST: api/Tokens
        public async Task<IToken> Post([FromBody]string id, bool demo = false) // id = uuid+mac
        {
            if (id == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("ID must not be null"),
                    ReasonPhrase = "ID must not be null"
                });
            }

            string uuid = null;
            string mac = null;

            var r = id.Split('+');
            if (r.Length != 2)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    Content = new StringContent("ID format error"),
                    ReasonPhrase = "ID format error"
                });
            }

            uuid = r[0];
            mac = r[1].ToUpper();

            if (!_deviceValidator.isValid(uuid))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("Device \"" + uuid + "\" is not valid."),
                    ReasonPhrase = "Device is not valid"
                });
            }

            // TODO: Add check on MAC Address on length and vendor ID

            try
            {
                var token = await _tokenRepository.Add(mac, demo);
                return token;
            }
            catch (DeviceAlreadyExistsException ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Device \"" + mac + "\" already exists."),
                    ReasonPhrase = "Device already exists"
                });
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Bad Request"),
                    ReasonPhrase = "Bad Request"
                });
            }

        }

        public async Task Delete(string deviceId)
        {
            if (deviceId == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("ID must not be null"),
                    ReasonPhrase = "ID must not be null"
                });
            }

            try
            {
                await _tokenRepository.Remove(deviceId);
            }
            catch (DeviceNotFoundException ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = "Device not found"
                });
            }
        }
    }
}
