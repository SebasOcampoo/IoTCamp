using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace API.Models
{
    public interface ITokensRepository
    {
        Task<IEnumerable<IToken>> GetAll();
        Task<IToken> Get(string deviceId);
        Task<IToken> Add(string deviceId, bool demo);
        Task Remove(string deviceId);
    }
}