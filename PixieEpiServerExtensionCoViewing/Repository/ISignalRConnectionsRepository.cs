using System.Collections.Generic;
using PixieEpiServerExtensionCoViewing.Models;

namespace PixieEpiServerExtensionCoViewing.Repository
{
    public interface ISignalRConnectionsRepository
    {
        void Save(SignalRConnection connection);
        SignalRConnection Get();
        void Delete();
    }
}
