Title: System.Security.SecureString
Published: 9/11/2011
Tags:
    - .NET Application Architecture
---
I recently had the opportunity to look into and make use of the Microsoft [System.Security.SecureString](https://docs.microsoft.com/en-us/dotnet/api/system.security.securestring?redirectedfrom=MSDN&view=net-6.0) class. This class is one of those dark corners of the .NET Framework that you don’t think about on a day-to-day basis but are really glad that it’s there when your security auditor starts asking questions about how PII data such as social security numbers are protected while resident in memory. The SecureString class takes care of this problem, helping you avoid a situation where unencrypted sensitive String data is left lingering around on the .NET heap. However, since this class does reference unmanaged memory buffers, its use is not entirely intuitive. I’ve attempted to demystify things with the explanation, drawing and code snippets in this post.

The diagram below shows that, in the case of System.String, what you get is an unencrypted string located in managed memory. Due to the immutability of String objects and the nondeterministic nature of the .NET Garbage Collector, the need for one string may result in multiple string objects scattered across managed memory, waiting to be compromised.

![.NET Secure String](https://s3.amazonaws.com/s3.beckshome.com/20110911-Secure-String.jpg)

In the case of a SecureString, you don’t have an unsecure String in managed memory. Instead, you get a [DPAPI](https://en.wikipedia.org/wiki/Data_Protection_API) encrypted array of characters in unmanaged memory. And, since SecureString implements the IDisposable interface, it’s easy to deterministically destroy the string’s secure contents. There are some limited .NET 4.0 Framework classes that accept SecureStrings as parameters, including the cryptographic service provider (CSP), X.509 certificate classes and several other security related classes. But what if you want to create your own classes that accept and deal with secure strings? How do you deal with the SecureString from managed .NET code and how do you ensure that you don’t defeat the purpose of the SecureString by leaving intermediate strings unsecure in memory buffers?

The simple console application below exhibits how a SecureString can be properly used and disposed; with the SecureString contents being made available to managed code and the intermediate memory zeroed out when no longer needed.

<pre data-enlighter-language="csharp">
using System;
using System.Security;
using System.Runtime.InteropServices;
 
namespace SecureStringExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Wrapping the SecureString with using causes it to be properly 
            // disposed, leaving no sensitive data in memory
            using (SecureString SecString = new SecureString())
            {
                Console.Write("Please enter your password: ");
                while (true)
                {
                    ConsoleKeyInfo CKI = Console.ReadKey(true);
                    if (CKI.Key == ConsoleKey.Enter) break;
                    // Use the AppendChar() method to add characters
                    // to the SecureString
                    SecString.AppendChar(CKI.KeyChar);
                    Console.Write("*");
                }
                // Make the SecureString read only
                SecString.MakeReadOnly();
                Console.WriteLine();

                // Display password by marshalling it from unmanaged memory 
                DisplaySecureString(SecString);
                Console.ReadKey();
	        }
	    }
	 
	    // Example demonstrating what needs to be done to get SecureString value to
        // managed code. This method uses unsafe code; project must be compiled
        // with /unsafe flag in the C# compiler
        private unsafe static void DisplaySecureString(SecureString SecString)
        {
            IntPtr StringPointer = Marshal.SecureStringToBSTR(SecString);
            try
            {
                // Read the decrypted string from the unmanaged memory buffer
                String NonSecureString = Marshal.PtrToStringBSTR(StringPointer);
                Console.WriteLine(NonSecureString);
            }
            finally
            {
                // Zero and free the unmanaged memory buffer containing the
                // decrypted SecureString
                Marshal.ZeroFreeBSTR(StringPointer);
                if (!SecString.IsReadOnly())
                    SecString.Clear();
            }
        }
    }
}
</pre>

This example should be useful to you in working SecureString into your own application. Like any other security measure, there’s a cost to the additional security. In the case of the SecureString, there’s overhead to adding characters to the SecureString as well as marshaling data out of unmanaged memory.  The final reference example I’ll provide is from Microsoft’s SecureString implementation, specifically the code to initialize the secure string. From this code, you can clearly see the check for platform dependencies, buffer allocation, pointer creation and the ProtectMemory() call which invokes the Win32 native RtlEncryptMemory function.

<pre data-enlighter-language="csharp">
[HandleProcessCorruptedStateExceptions, SecurityCritical]
private unsafe void InitializeSecureString(char* value, int length)
{
    this.CheckSupportedOnCurrentPlatform();
    this.AllocateBuffer(length);
    this.m_length = length;
    byte* pointer = null;
    RuntimeHelpers.PrepareConstrainedRegions();
    try
    {
        this.m_buffer.AcquirePointer(ref pointer);
        Buffer.memcpyimpl((byte*) value, pointer, length * 2);
    }
    catch (Exception)
    {
        this.ProtectMemory();
        throw;
    }
    finally
    {
        if (pointer != null)
        {
            this.m_buffer.ReleasePointer();
        }
    }
    this.ProtectMemory();
}
</pre>