using GalaxyCoreCommon;
using GalaxyCoreServer;
 

namespace SimpleMmoServer.Examples.Instances
{
    /// <summary>
    /// Пример быстрого поиска сущностей вокруз заданной точки
    /// </summary>
    public class ExampleOctoRoom : InstanceOpenWorld
    {
               
        public ExampleOctoRoom()
        {
          
        }
 

        public override void IncomingClient(Client clientConnection)
        {
            foreach (var item in entities.list)
            {
            item.ChangeOwner(clientConnection);
            }    
        }   

        public override void Start()
        {          
         
        }
        
        public override void Update()
        {
            // Быстрый поиск ближайших энтити 
            foreach (var item in entities.GetNearby(GalaxyVector3.Zero,10))
            {
              Log.Info("GetNearby", "entity id:" + item.netID);                 
            }
          
        }
        public override void InMessage(byte code, byte[] data, Client clientConnection)
        {

        }

        public override void OutcomingClient(Client clientConnection)
        {

        }

        public override void Close()
        {

        }
    }
}
