using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opine.Messaging
{
    public interface IMessageStore : IMessageReader, IMessageWriter
    {

    }
}