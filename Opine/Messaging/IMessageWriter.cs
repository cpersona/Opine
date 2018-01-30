using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opine.Messaging
{
    public interface IMessageWriter
    {
         Task Store(Stream stream, long version, IEnumerable<StorableMessage> storableMessages);
    }
}