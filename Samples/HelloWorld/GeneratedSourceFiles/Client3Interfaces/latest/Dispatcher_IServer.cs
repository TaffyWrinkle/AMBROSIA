
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ambrosia;
using static Ambrosia.StreamCommunicator;
using LocalAmbrosiaRuntime;

namespace Server
{
    /// <summary>
    /// This class runs in the process of the object that implements the interface IServer
    /// and communicates with the local Ambrosia runtime.
    /// It is instantiated in ImmortalFactory.CreateServer when a bootstrapper registers a container
    /// that supports the interface IServer.
    /// </summary>
    class IServer_Dispatcher_Implementation : Immortal.Dispatcher
    {
        private readonly IServer instance;
		private readonly ExceptionSerializer exceptionSerializer = new ExceptionSerializer(new List<Type>());

        public IServer_Dispatcher_Implementation(Immortal z, ImmortalSerializerBase myImmortalSerializer, string serviceName, int receivePort, int sendPort, bool setupConnections)
            : base(z, myImmortalSerializer, serviceName, receivePort, sendPort, setupConnections)
        {
            this.instance = (IServer) z;
        }

        public  IServer_Dispatcher_Implementation(Immortal z, ImmortalSerializerBase myImmortalSerializer, string localAmbrosiaRuntime, Type newInterface, Type newImmortalType, int receivePort, int sendPort)
            : base(z, myImmortalSerializer, localAmbrosiaRuntime, newInterface, newImmortalType, receivePort, sendPort)
        {
            this.instance = (IServer) z;
        }

        public override async Task<bool> DispatchToMethod(int methodId, RpcTypes.RpcType rpcType, string senderOfRPC, long sequenceNumber, byte[] buffer, int cursor)
        {
            switch (methodId)
            {
                case 0:
                    // Entry point
                    await this.EntryPoint();
                    break;
                case 1:
                    // ReceiveMessageAsync
                    {
                        // deserialize arguments

            // arg0: System.String
            var p_0_ValueLength = buffer.ReadBufferedInt(cursor);
cursor += IntSize(p_0_ValueLength);
var p_0_ValueBuffer = new byte[p_0_ValueLength];
Buffer.BlockCopy(buffer, cursor, p_0_ValueBuffer, 0, p_0_ValueLength);
cursor += p_0_ValueLength;
var p_0 = Ambrosia.BinarySerializer.Deserialize<System.String>(p_0_ValueBuffer);

                        // call the method
						var p_1 = default(Int32);
						byte[] argExBytes = null;
						int argExSize = 0;
						Exception currEx = null;
						int arg1Size = 0;
						byte[] arg1Bytes = null;

						try 
						{
							p_1 =
								await this.instance.ReceiveMessageAsync(p_0);
						}
						catch (Exception ex)
						{
							currEx = ex;
						}

                        if (!rpcType.IsFireAndForget())
                        {
                            // serialize result and send it back
						if (currEx != null)
						{
			var argExObject = this.exceptionSerializer.Serialize(currEx);
argExBytes = Ambrosia.BinarySerializer.Serialize(argExObject);
argExSize = IntSize(argExBytes.Length) + argExBytes.Length;

						}
						else 
						{
			arg1Bytes = Ambrosia.BinarySerializer.Serialize<System.Int32>(p_1);
arg1Size = IntSize(arg1Bytes.Length) + arg1Bytes.Length;

						}
                            var wp = this.StartRPC_ReturnValue(senderOfRPC, sequenceNumber, currEx == null ? arg1Size : argExSize, currEx == null ? ReturnValueTypes.ReturnValue : ReturnValueTypes.Exception);

	
						if (currEx != null)
						{
			wp.curLength += wp.PageBytes.WriteInt(wp.curLength, argExBytes.Length);
Buffer.BlockCopy(argExBytes, 0, wp.PageBytes, wp.curLength, argExBytes.Length);
wp.curLength += argExBytes.Length;

						}
						else 
						{
            wp.curLength += wp.PageBytes.WriteInt(wp.curLength, arg1Bytes.Length);
Buffer.BlockCopy(arg1Bytes, 0, wp.PageBytes, wp.curLength, arg1Bytes.Length);
wp.curLength += arg1Bytes.Length;

						}
                            this.ReleaseBufferAndSend();
                        }
                    }
                    break;
            }

            return true;
        }
    }
}