using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NetCore.Utilities.Utils
{
    public static class Common
    {
        /// <summary>
        /// Data Protection 지정하기
        /// </summary>
        /// <param name="services">등록할 서비스</param>
        /// <param name="keyPath">키 경로</param>
        /// <param name="applicationName">애플리케이션 이름</param>
        /// <param name="cryptoType">암호화 유형</param>
        public static void SetDataProtection(IServiceCollection services, string keyPath, string applicationName, Enum cryptoType)
        {
            var builder = services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
                    .SetDefaultKeyLifetime(TimeSpan.FromDays(7))
                    .SetApplicationName(applicationName);

            switch (cryptoType)
            {
                case Enums.CryptoType.Unmanaged:
                    //AES
                    //Advanced Encryption Standard
                    //Two-way : 암호화, 복호화
                    builder.UseCryptographicAlgorithms(
                        new AuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                            //SHA
                            //Secure Hash Algorithm
                            //One-way : 암호화
                            ValidationAlgorithm = ValidationAlgorithm.HMACSHA512
                        });
                    break;
                case Enums.CryptoType.Managed:
                    builder.UseCustomCryptographicAlgorithms(
                        new ManagedAuthenticatedEncryptorConfiguration()
                        {
                            // A type that subclasses SymmetricAlgorithm
                            EncryptionAlgorithmType = typeof(Aes),

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256,

                            // A type that subclasses KeyedHashAlgorithm
                            ValidationAlgorithmType = typeof(HMACSHA512)
                        });
                    break;
                case Enums.CryptoType.CngGcm:
                    //Windows CNG algorithm using Galois/Counter Mode encryption
                    //Galois/Counter Mode
                    //GCM
                    builder.UseCustomCryptographicAlgorithms(
                        new CngGcmAuthenticatedEncryptorConfiguration()
                        {
                            // Passed to BCryptOpenAlgorithmProvider
                            EncryptionAlgorithm = "AES",
                            EncryptionAlgorithmProvider = null,

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256
                        });
                    break;
                case Enums.CryptoType.CngCbc:
                default:
                    //Windows CNG algorithm using CBC-mode encryption
                    //CNG algorithm
                    //Cryptography API : Next Generation
                    //CBC-mode
                    //Cipher Block Chaining
                    builder.UseCustomCryptographicAlgorithms(
                        new CngCbcAuthenticatedEncryptorConfiguration()
                        {
                            // Passed to BCryptOpenAlgorithmProvider
                            EncryptionAlgorithm = "AES",
                            EncryptionAlgorithmProvider = null,

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256,

                            // Passed to BCryptOpenAlgorithmProvider
                            HashAlgorithm = "SHA512",
                            HashAlgorithmProvider = null
                        });
                    break;
            }
        }
    }
}