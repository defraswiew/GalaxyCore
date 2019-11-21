using System;
using System.Collections.Generic;
using System.Text;
using GalaxyCoreServer;
using GalaxyCoreCommon;
using GalaxyCoreServer.Api;
using GalaxyTemplate.Connecting;

namespace GalaxyTemplate
{
   public class Server
    {
        /// <summary>
        /// Пример реализации авторизации
        /// </summary>
        Authorization authorization;
      
        public Server()
        {
            authorization = new Authorization();
        }

    }
}
