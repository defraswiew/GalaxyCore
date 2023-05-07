using System;
using System.Diagnostics;
using GalaxyCoreCommon;
using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.Instances
{
    public class ExampleEmptyInstance : Instance
    {
        Stopwatch stopwatch = new Stopwatch();
        public override void InMessage(byte code, byte[] data, BaseClient clientConnection)
        {
            if (Owner == clientConnection.Id) ChangeOwner();
        }


        public override void Start()
        {
            Log.Info("ExampleEmptyInstance", "instance id:" + Id);
            SetFrameRate(10);
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public override void Update()
        {
        //    SendMessageToAll(111,Array.Empty<byte>(),GalaxyDeliveryType.reliableOrdered,false,10);
            /*
            stopwatch.Stop();
            var elapsedTicks = stopwatch.ElapsedTicks;
            var elapsedMilliseconds = (double)elapsedTicks / Stopwatch.Frequency * 1000.0;
            Log.Info("ExampleEmptyInstance", elapsedMilliseconds.ToString());
            stopwatch = new Stopwatch();
            stopwatch.Start();
            */
        }
    }
}