using System.Collections;
using SimpleJson;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Pomelo.DotNetClient
{
	public class PomeloClient : IDisposable
	{
		public const string EVENT_DISCONNECT = "disconnect";

		private EventManager eventManager;
		private Socket socket;
		private Protocol protocol;
		private bool disposed = false;
		private uint reqId = 1;
		
		/// <summary>
		/// Gets the heart beat latency.
		/// </summary>
		/// <value>The heart beat latency. The unit is millisecond.</value>
		public int HeartBeatLatency
		{
			get {return protocol.HeartBeatLatency;}
		}
		
		public PomeloClient(string host, int port) {
			this.eventManager = new EventManager();
			initClient(host, port);
			this.protocol = new Protocol(this, socket);
		}
		
		public PomeloClient(string host, int port, RSAParameters rsaParam, RSAPKCS1SignatureFormatter rsaFormatter, SHA256 sha256) {
			this.eventManager = new EventManager();
			initClient(host, port);
			this.protocol = new Protocol(this, socket);
			protocol.InitRsa(rsaParam, rsaFormatter, sha256);
		}

		private void initClient(string host, int port) {
			this.socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
	        IPEndPoint ie=new IPEndPoint(IPAddress.Parse(host), port);
	        try {
	            this.socket.Connect(ie);
	        }
	        catch (SocketException e) {
	            Console.WriteLine(String.Format("unable to connect to server: {0}", e.ToString()));
	            return;
	        }
		}
		
		public void connect(){
			protocol.start(null, null);
		}
		
		public void connect(JsonObject user){
			protocol.start(user, null);
		}
		
		public void connect(Action<JsonObject> handshakeCallback){
			protocol.start(null, handshakeCallback);
		}
		
		public bool connect(JsonObject user, Action<JsonObject> handshakeCallback){
			try{
				protocol.start(user, handshakeCallback);
				return true;
			}catch(Exception e){
				Console.WriteLine(e.ToString());
				return false;
			}
		}

		public void request(string route, Action<JsonObject> action){
			this.request(route, new JsonObject(), action);
		}
		
		public void request(string route, JsonObject msg, Action<JsonObject> action){
			this.eventManager.AddCallBack(reqId, action);
			protocol.send (route, reqId, msg);
			reqId++;
		}
		
		public void notify(string route, JsonObject msg){
			protocol.send (route, msg);
		}
		
		public void on(string eventName, Action<JsonObject> action){
			eventManager.AddOnEvent(eventName, action);
		}

		internal void processMessage(Message msg){
			if(msg.type == MessageType.MSG_RESPONSE){
				eventManager.InvokeCallBack(msg.id, msg.data);
			}else if(msg.type == MessageType.MSG_PUSH){
				UnityEngine.Debug.Log(msg.route);
				UnityEngine.Debug.Log(msg.data);
				eventManager.InvokeOnEvent(msg.route, msg.data);
			}
		}

		public void disconnect(){
			//Call disconnect callback
			if(!this.disposed) 
				eventManager.InvokeOnEvent(EVENT_DISCONNECT, null);
			Dispose ();
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	
		// The bulk of the clean-up code 
		protected virtual void Dispose(bool disposing) {
			if(this.disposed) 
				return;

			if (disposing) {
				// free managed resources
				this.protocol.close();
				try {
					this.socket.Shutdown(SocketShutdown.Both);
				}
				catch {
				}
				this.socket.Close();
				this.disposed = true;
			}
		}
	}
}

