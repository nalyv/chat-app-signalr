using System.Collections.Generic;

namespace chat_application.Hubs
{
    public class ConnectionMapp
    {
        private readonly List<Connection> connections = new List<Connection>();

        public void Add(string Name,string ConnectionID)
        {
            connections.Add(new Connection(){
                Name = Name,
                ConnectionID = ConnectionID
            });
        }

        public void Remove(string Name,string ConnectionID)
        {
            connections.Remove(new Connection(){
                Name = Name,
                ConnectionID = ConnectionID
            });
        }
    }
    
}