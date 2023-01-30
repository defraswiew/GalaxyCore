using GalaxyCoreServer;

namespace SimpleMmoServer
{
    /// <summary>
    /// Пример кастомного класса клиента
    /// </summary>
    public class ExampleClient : BaseClient
    {
        public ExampleClient()
        {
        }

        /// <summary>
        /// Вызывается когда игрок отключился
        /// </summary>
        protected override void OnDisconnected()
        {
        
        }

        protected override void OnConnected()
        {
            base.OnConnected();
        }
    }
}