using System.Net.Sockets;

namespace CL8
{
    public class ConnectedClient(Socket socket)
    {
        public string? Login { get; private set; }
        public Socket Socket { get; } = socket;
        public NetworkStream Stream { get; } = new(socket);

        public void SetLogin(string login)
        {
            Login = login;
        }
    }
}