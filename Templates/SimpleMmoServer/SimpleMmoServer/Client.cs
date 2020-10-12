using GalaxyCoreServer;

namespace SimpleMmoServer
{
    /// <summary>
    /// Пример кастомного класса клиента
    /// </summary>
   public class ExampleClient:Client
    {
      
        public ExampleClient()
        {

        }
        /// <summary>
        /// Вызывается когда игрок отключился
        /// </summary>
        public override void OnDisconnected()
        {
         
        }
    }
}
