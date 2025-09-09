using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatContracts
{
    public interface IChatClient
    {

        Task ReceiveSystemMessage(string message);
        Task UpdateUserList(List<ConnectedUser> users);
        Task ReceiveMessage(string fromUserId, string ConnectionId, string message);
    }
}
