using System;
using System.Text;
using System.Security.Cryptography;
using SimpleJson;

namespace Pomelo.DotNetClient
{
	public class Protocol
	{
		private MessageProtocol messageProtocol;
		private ProtocolState state;
		private Transporter transporter;
		private HandShakeService handshake;
		private HeartBeatService heartBeatService = null;
		private PomeloClient pc;

		RSAPKCS1SignatureFormatter _RSAFormatter;

		SHA256 _Sha256;

		internal int HeartBeatLatency
		{
			get {return heartBeatService.Latency;}
		}
		
		public bool IsCrypto {get; private set;}

		public PomeloClient getPomeloClient(){
			return this.pc;
		}

		public Protocol(PomeloClient pc, System.Net.Sockets.Socket socket){
			this.pc = pc;
			this.transporter = new Transporter (socket, this.processMessage);
			this.transporter.onDisconnect = onDisconnect;

			this.handshake = new HandShakeService(this);
			this.state = ProtocolState.start;
		}

		internal void InitRsa(RSAParameters rsaParam, RSAPKCS1SignatureFormatter rsaFormatter, SHA256 sha256)
		{
			JsonObject publicKey = new JsonObject();
			publicKey["rsa_n"] = BitConverter.ToString(rsaParam.Modulus).Replace("-", string.Empty);
			publicKey["rsa_e"] = rsaParam.Exponent[0].ToString();
			handshake.InitRsa(publicKey);

			_Sha256 = sha256;
			//Create an RSASignatureFormatter object and pass it the 
			//RSACryptoServiceProvider to transfer the key information.
			_RSAFormatter = rsaFormatter;
			IsCrypto = true;
		}

		internal void start(JsonObject user, Action<JsonObject> callback){
			this.transporter.start();
			this.handshake.request(user, callback);

			this.state = ProtocolState.handshaking;
		}
		
		//Send notify, do not need id
		internal void send(string route, JsonObject msg){
			send (route, 0, msg);
		}

		//Send request, user request id 
		internal void send(string route, uint id, JsonObject msg){
			if(this.state != ProtocolState.working) return;

			if(IsCrypto)
			{
				string jsonString = SimpleJson.SimpleJson.SerializeObject(msg);
				byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);


				//The hash to sign.
				byte[] hash;

				hash = _Sha256.ComputeHash(jsonBytes);

				//Set the hash algorithm to SHA256.
				_RSAFormatter.SetHashAlgorithm("SHA256");
				
				//Create a signature for HashValue and return it.
				byte[] SignedHash = _RSAFormatter.CreateSignature(hash);

				string signString = BitConverter.ToString(SignedHash).Replace("-", string.Empty);
				msg.Add("__crypto__", signString);
			}

			byte[] body = messageProtocol.encode (route, id, msg); 

			send (PackageType.PKG_DATA, body);
		}

		internal void send(PackageType type){
			if(this.state == ProtocolState.closed) return;
			transporter.send(PackageProtocol.encode (type));
		}

		//Send system message, these message do not use messageProtocol
		internal void send(PackageType type, JsonObject msg){
			//This method only used to send system package
			if(type == PackageType .PKG_DATA) return;
			
			byte[] body = Encoding.UTF8.GetBytes(msg.ToString());

			send (type, body);
		}

		//Send message use the transporter
		internal void send(PackageType type, byte[] body){
			if(this.state == ProtocolState.closed) return;

			byte[] pkg = PackageProtocol.encode (type, body);

			transporter.send(pkg);
		}
	
		//Invoke by Transporter, process the message
		internal void processMessage(byte[] bytes){
			Package pkg = PackageProtocol.decode(bytes);
			
			//Ignore all the message except handshading at handshake stage
			if (pkg.type == PackageType.PKG_HANDSHAKE && this.state == ProtocolState.handshaking) {

				//Ignore all the message except handshading
				string pkg_body = Encoding.UTF8.GetString(pkg.body);
				JsonObject data = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(pkg_body);
				processHandshakeData(data);
				this.state = ProtocolState.working;

			}else if (pkg.type == PackageType.PKG_HEARTBEAT && this.state == ProtocolState.working){
				this.heartBeatService.resetTimeout();
			}else if (pkg.type == PackageType.PKG_DATA && this.state == ProtocolState.working) {
				this.heartBeatService.resetTimeout();
				Message msg = messageProtocol.decode (pkg.body);
				pc.processMessage(msg);
			}else if (pkg.type == PackageType.PKG_KICK) {
				this.close();
			}
		}
		
		private void processHandshakeData(JsonObject msg){
			//Handshake error
			if(!msg.ContainsKey("code") || !msg.ContainsKey("sys") || Convert.ToInt32(msg["code"]) != 200){
				throw new Exception("Handshake error! Please check your handshake config.");
			}

			//Set compress data
			JsonObject sys = (JsonObject)msg["sys"];

			JsonObject dict = new JsonObject();
			if(sys.ContainsKey("dict")) dict = (JsonObject)sys["dict"];

			JsonObject protos = new JsonObject();
			JsonObject serverProtos = new JsonObject();
			JsonObject clientProtos = new JsonObject();

			if(sys.ContainsKey("protos")){ 
				protos = (JsonObject)sys["protos"];
				serverProtos = (JsonObject)protos["server"];
				clientProtos = (JsonObject)protos["client"];
			}

			messageProtocol = new MessageProtocol (dict, serverProtos, clientProtos);

			//Init heartbeat service
			int interval = 0;
			if(sys.ContainsKey("heartbeat")) interval = Convert.ToInt32(sys["heartbeat"]);
			heartBeatService = new HeartBeatService(interval, this);

			if(interval > 0){
				heartBeatService.start();
			}

			//send ack and change protocol state
			handshake.ack();
			this.state = ProtocolState.working;

			//Invoke handshake callback
			JsonObject user = new JsonObject();
			if(msg.ContainsKey("user")) user = (JsonObject)msg["user"];
			handshake.invokeCallback(user);
		}

		//The socket disconnect
		private void onDisconnect(){
			this.pc.disconnect();
		}

		internal void close(){
			transporter.close();

			if(heartBeatService != null) heartBeatService.stop();

			this.state = ProtocolState.closed;
		}
	}
}

