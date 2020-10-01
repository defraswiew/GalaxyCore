﻿using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.Instances
{
    public class ExampleInvoke : Instance
    {
        public override void Close()
        {
           
        }

        public override void IncomingClient(Client clientConnection)
        {
           
        }

        public override void InMessage(byte code, byte[] data, Client clientConnection)
        {
            
        }

        public override void OutcomingClient(Client clientConnection)
        {
            
        }

        public override void Start()
        {
            Invoke("TestInvoke1", 2);
        }

        public override void Update()
        {
             
        }


        public void TestInvoke1()
        {
            Invoke("TestInvoke2", 2, GRand.NextInt(0,100));
            Log.Debug("TestInvoke1", "No Arg");
        }
        public void TestInvoke2(int rnd)
        {
            Invoke("TestInvoke3", 2);
            Log.Debug("TestInvoke2", "rnd="+ rnd);
        }
        public void TestInvoke3()
        {
            InvokeRepeating("TestInvokeRepeating1", 2,2);
            Invoke("CancelTestInvokeRepeating1", 10);
             
            Log.Debug("TestInvoke3", "No Arg");
        }

        public void TestInvokeRepeating1()
        {
            Log.Debug("TestInvokeRepeating1", "No Arg");
        }

        public void CancelTestInvokeRepeating1()
        {
            CancelInvoke("TestInvokeRepeating1");
            Log.Debug("Cancel", "TestInvokeRepeating1");
        }

    }
}