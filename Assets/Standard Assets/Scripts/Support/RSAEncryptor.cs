////////////////////////////////////////////////////////////////////
/// RSAEncryptor.cs
/// © 2005 Carl Johansen
/// 
/// RSA key generation and encryption demonstration class
////////////////////////////////////////////////////////////////////
using System;

public class RSAEncryptor
{
	public delegate int ProgressWriteLineDelegate(string theMessage);
	
	public static uint MaxAllowedPrime { get { return 241; } }
	
	private uint p, q, d, e, n, phiN;
	public uint P {  get {	return p; }
		set 
		{
			if(!IsAPrime(value))
				throw new ArgumentOutOfRangeException("p", value.ToString() + " is not a prime!");
			else if(!(1 <= value && value <= MaxAllowedPrime))
				throw new ArgumentOutOfRangeException("p", "Sorry, " + MaxAllowedPrime.ToString() + " is the limit.");
			else
				p = value;
		}
	}
	public uint Q {  get {	return q; }
		set 
		{
			if(!IsAPrime(value))
				throw new ArgumentOutOfRangeException("q", value.ToString() + " is not a prime!");
			else if(!(1 <= value && value <= MaxAllowedPrime))
				throw new ArgumentOutOfRangeException("q", "Sorry, " + MaxAllowedPrime.ToString() + " is the limit.");
			else
				q = value;
		}
	}
	
	public uint D { get { return d; } }
	public uint E { get { return e; } }
	public uint N { get { return n; } }
	public uint PhiN { get { return phiN; } }
	
	public RSAEncryptor()
	{
		//
		// TODO: Add constructor logic here
		//
	}
	
	public void GenerateKeyPair()
	{
		d = 0; e = 0;
		uint c = 0, r, testLimit;
		uint testGCD;
		bool done;
		
		n = p * q;
		phiN = (p-1) * (q-1);
		done = false;
		do 
		{
			c++;
			r = c * phiN + 1; // this must be odd, since phi must be even (product of two even numbers)
			testLimit = (uint) Math.Sqrt(r) + 1;
			for(d = 3; d < testLimit && (r % d != 0); d += 2);
			if(d < testLimit)
			{ 
				e = r / d;
				//It's important that d and e are relatively prime to phi(n) (ie have no factors in common with phi(n),
				// but we have already guaranteed this because we know that dividing phi(N) by either of the numbers
				// that we have chosen will leave a remainder of 1.
				//Now we just have to test that they are relatively prime with each other
				if((testGCD = GCD(e, d)) != 1)
				{
				}
				//Obviously, if d and e are the same number then our cipher will be symmetric - no good.  We have
				// already caught that case with the GCD test above, but we must also check that d and e are 
				// not the same mod phi(n).
				else if(( (d % phiN) == (e % phiN) ))
				{
				}
				else
				{
					done = true;
				}
			}			
		} while(!done);
	}
	
	public void ValidateMessage(uint theMessage)
	{
		string errMsg;
		
		if(!(1 < theMessage && theMessage < n-1))
		{
			errMsg = String.Format("The message must be in the range 2 - {0}", n-2);
			throw new ArgumentOutOfRangeException("message", errMsg);
		}
		else if((theMessage >= p) && (theMessage % p == 0))
		{
			errMsg = String.Format("{0} is an invalid message because it is a multiple of {1} (its encryption would just be some other multiple of {1})", theMessage, p);
			throw new ArgumentOutOfRangeException("message", errMsg);
		}
		else if((theMessage >= q) && (theMessage % q == 0))
		{
			errMsg = String.Format("{0} is an invalid message because it is a multiple of {1} (its encryption would just be some other multiple of {1})", theMessage, q);
			throw new ArgumentOutOfRangeException("message", errMsg);
		}
	}
	
	public uint EncryptWithPublicKey(uint theMessage)
	{	return Encrypt(theMessage, e);	}
	
	public uint EncryptWithPrivateKey(uint theMessage)
	{	return Encrypt(theMessage, d);	}
	
	private uint Encrypt(uint theMessage, uint theKey)
	{
		return modpower(theMessage, theKey, n);
	}
	
	private static uint GCD(uint a, uint b)
	{
		uint tmp, remainder;
		if(a < b)
		{ tmp = a; a = b; b = tmp; }
		
		do 
		{
			remainder = a % b;
			if(remainder == 0)
				return b;
			a = b;
			b = remainder;
		} while(true);
	}
	
	private static uint modpower(uint a, uint b, uint c)
	{	// returns (a ^ b) mod c
		// eg 28 ^ 23 mod 55 = 7
		
		// There's a trick to this.  a ^ b is usually way too big to handle, so we break it 
		// into pieces, using the fact that xy mod c = ( x mod c )( y mod c ) mod c.
		// Please see www.carljohansen.co.uk/rsa for a full explanation.
		
		byte bLog2 = (byte) Math.Log(b, 2);
		uint prevResult = a, finalProduct = 1;
		int i;
		uint[] arrResidues = new uint[bLog2 + 1];
		
		arrResidues[0] = a;
		
		for(i = 1 ; i <= bLog2 ; i++)
		{
			arrResidues[i] = (prevResult * prevResult) % c;
			prevResult = arrResidues[i];
		}
		
		for(i = bLog2; i >= 0; i--)
			if((b & (1 << i)) > 0)	//If the ith bit of b is set...
		{
			finalProduct *= arrResidues[i];
			finalProduct %= c;
		}
		
		return finalProduct;
	}
	
	private static bool IsAPrime(uint testNum)
	{
		uint n;
		
		for(n = 2 ; n < testNum ; n+=(n==2 ? (uint)1 : (uint)2))
			if(testNum % n == 0)
				return false;
		
		return true;
	}
}