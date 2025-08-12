using System.IO;
using System.Security.Cryptography;
using System.Text;

 
namespace TheChosenProject.ServerCore
{
  public class EncryptFiles
  {
    private readonly TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

        public EncryptFiles(string key = "PRVIXYCONQUER#56")
        {
            this.des.Key = Encoding.UTF8.GetBytes(key);
            this.des.Mode = CipherMode.ECB;
        }

        public void Encrypt(string filepath)
    {
      byte[] inputBuffer = File.ReadAllBytes(filepath);
      byte[] bytes = this.des.CreateEncryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
      File.WriteAllBytes(filepath, bytes);
    }

    public void Decrypt(string filepath)
    {
      byte[] inputBuffer = File.ReadAllBytes(filepath);
      byte[] bytes = this.des.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
      File.WriteAllBytes(filepath, bytes);
    }
  }
}
