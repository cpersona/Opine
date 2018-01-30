using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opine.Messaging
{
    public interface IMessageReader
    {
         Task<IEnumerable<StoredMessage>> Read(Stream stream, long position, int count);
    }
}