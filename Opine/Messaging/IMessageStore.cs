using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opine.Messaging
{
    public interface IMessageStore
    {
        Task<IEnumerable<StoredMessage>> Read(Stream stream, long position, int count);
        Task Store(Stream stream, long version, IEnumerable<StorableMessage> storableMessages);
    }
}