using Opine.Dispatching;

namespace Opine.Messaging
{
    public interface IMetadataFactory
    {
         Metadata Create(MessageContext context);
    }
}