using GalaxyCoreCommon;
using GalaxyCoreServer; 

namespace SimpleMmoServer.Examples.Instances
{
    public class ExampleRoomMovers : Instance
    {       
        float timer;
        int moverCount;
        int moverMax = 1000;
        public override void OutcomingClient(Client client)
        {                     
            
        }

        public override void Close()
        {
            
        }
        
        public override void IncomingClient(Client client)
        {
           
        }

        public override void Start()
        {            
            SetFrameRate(5);   
        }
        public override void InMessage(byte code, byte[] data, Client client)
        {
          
        }

      
        public override void Update()
        {                 
            timer += Time.deltaTime;
            if (timer > 0.1f)
            {
                if (moverCount > moverMax) return;
                timer = 0;              
                Examples.NetEntitys.ExampleRandomMove mover = new Examples.NetEntitys.ExampleRandomMove(this,new GalaxyVector3(0,1,0));
                mover.Init();
                moverCount++;
            }              
              
        }

     }
            
}
             
 
 
