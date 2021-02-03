// <copyright file="Checksum.cs" company="Genesis Engineering Services">
// Copyright (c) 2021 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2021-01-17</date>
// <summary>Various checksum algorithms</summary>

namespace GES.Communications
{
   #region Directives

   using MTI.Core;

   using System;
   using System.ComponentModel;

   #endregion

   /// <summary>
   /// Interface defining signature for checksum algorithms
   /// </summary>
   [Description("Checksum algorithms.")]
   public abstract class Checksum
   {
      /// <summary>
      /// Gets or sets the initial checksum value
      /// </summary>
      public virtual ulong InitialValue
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the index in the buffer where the checksum is inserted.
      /// </summary>
      public virtual int ChecksumIndex
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the start index of the buffer used by the checksum computation.
      /// </summary>
      public virtual int StartIndex
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the stop index of the buffer used by the checksum computation.
      /// </summary>
      public virtual int StopIndex
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the bit length of the checksum.
      /// </summary>
      public virtual int BitLength
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the byte order of the checksum
      /// </summary>
      public virtual ByteOrder ByteOrder
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or set the computed checksum
      /// </summary>
      [Provide]
      public virtual ulong ComputedChecksum
      {
         get;
         set;
      }

      /// <summary>
      /// The signature of checksum methods
      /// </summary>
      /// <param name="data">the binary buffer for which the checksum is computed</param>
      /// <param name="startIndex">the start index at which the checksum starts</param>
      /// <param name="length">the number of bytes for which the checksum is computed</param>
      /// <param name="initialValue">the initial value of the checksum algorithm</param>
      /// <returns>the checksum</returns>
      public abstract ulong Compute(byte[] data);

      protected void InsertChecksum(byte[] data)
      {
         if (this.ChecksumIndex > -1)
         {
            switch (this.BitLength)
            {
               case 8:
                  {
                     data[this.ChecksumIndex] = (byte)this.ComputedChecksum;
                     break;
                  }
               case 16:
                  {
                     byte[] bytes = BitConverter.GetBytes((ushort)this.ComputedChecksum);
                     if (BitConverter.IsLittleEndian)
                     {
                        Array.Reverse(bytes);
                     }

                     data[this.ChecksumIndex] = bytes[0];
                     data[this.ChecksumIndex + 1] = bytes[1];
                     break;
                  }

               case 32:
                  {
                     byte[] bytes = BitConverter.GetBytes((uint)this.ComputedChecksum);
                     if (BitConverter.IsLittleEndian)
                     {
                        Array.Reverse(bytes);
                     }

                     data[this.ChecksumIndex] = bytes[0];
                     data[this.ChecksumIndex + 1] = bytes[1];
                     data[this.ChecksumIndex + 2] = bytes[2];
                     data[this.ChecksumIndex + 3] = bytes[3];
                     break;
                  }

               case 64:
                  {
                     byte[] bytes = BitConverter.GetBytes(this.ComputedChecksum);
                     if (BitConverter.IsLittleEndian)
                     {
                        Array.Reverse(bytes);
                     }

                     data[this.ChecksumIndex] = bytes[0];
                     data[this.ChecksumIndex + 1] = bytes[1];
                     data[this.ChecksumIndex + 2] = bytes[2];
                     data[this.ChecksumIndex + 3] = bytes[3];
                     data[this.ChecksumIndex + 4] = bytes[4];
                     data[this.ChecksumIndex + 5] = bytes[5];
                     data[this.ChecksumIndex + 6] = bytes[6];
                     data[this.ChecksumIndex + 7] = bytes[7];
                     break;
                  }
            }
         }
      }
   }

   /// <summary>
   /// Interface defining signature for checksum algorithms
   /// </summary>
   [Description("Checksum algorithms.")]
   public class EmptyChecksum :
      Checksum
   {
      /// <summary>
      /// Gets or sets the initial checksum value
      /// </summary>
      [Browsable(false)]
      public override ulong InitialValue
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the index in the buffer where the checksum is inserted.
      /// </summary>
      [Browsable(false)]
      public override int ChecksumIndex
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the start index of the buffer used by the checksum computation.
      /// </summary>
      [Browsable(false)]
      public override int StartIndex
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the stop index of the buffer used by the checksum computation.
      /// </summary>
      [Browsable(false)]
      public override int StopIndex
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the bit length of the checksum.
      /// </summary>
      [Browsable(false)]
      public override int BitLength
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the byte order of the checksum
      /// </summary>
      [Browsable(false)]
      public override ByteOrder ByteOrder
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or set the computed checksum
      /// </summary>
      [Browsable(false)]
      public override ulong ComputedChecksum
      {
         get;
         set;
      }

      /// <summary>
      /// The signature of checksum methods
      /// </summary>
      /// <param name="data">the binary buffer for which the checksum is computed</param>
      /// <param name="startIndex">the start index at which the checksum starts</param>
      /// <param name="length">the number of bytes for which the checksum is computed</param>
      /// <param name="initialValue">the initial value of the checksum algorithm</param>
      /// <returns>the checksum</returns>
      public override ulong Compute(byte[] data) { return 0; }
   }

   /// <summary>
   ///  Compute and decode the VCDU Error Control Field of the TDRSS MA VCDU frame
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications", "GES.Checksums" })]
   [Description("Checksum algorithms.")]
   public class CCSDSCRC16 :
      Checksum
   {
      #region Fields

      /// <summary>
      /// Coefficients used to Compute the checksum
      /// </summary>
      private static ushort[] crctab = new ushort[]
      {
         0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50A5, 0x60C6, 0x70E7,
         0x8108, 0x9129, 0xA14A, 0xB16B, 0xC18C, 0xD1AD, 0xE1CE, 0xF1EF,
         0x1231, 0x0210, 0x3273, 0x2252, 0x52B5, 0x4294, 0x72F7, 0x62D6,
         0x9339, 0x8318, 0xB37B, 0xA35A, 0xD3BD, 0xC39C, 0xF3FF, 0xE3DE,
         0x2462, 0x3443, 0x0420, 0x1401, 0x64E6, 0x74C7, 0x44A4, 0x5485,
         0xA56A, 0xB54B, 0x8528, 0x9509, 0xE5EE, 0xF5CF, 0xC5AC, 0xD58D,
         0x3653, 0x2672, 0x1611, 0x0630, 0x76D7, 0x66F6, 0x5695, 0x46B4,
         0xB75B, 0xA77A, 0x9719, 0x8738, 0xF7DF, 0xE7FE, 0xD79D, 0xC7BC,
         0x48C4, 0x58E5, 0x6886, 0x78A7, 0x0840, 0x1861, 0x2802, 0x3823,
         0xC9CC, 0xD9ED, 0xE98E, 0xF9AF, 0x8948, 0x9969, 0xA90A, 0xB92B,
         0x5AF5, 0x4AD4, 0x7AB7, 0x6A96, 0x1A71, 0x0A50, 0x3A33, 0x2A12,
         0xDBFD, 0xCBDC, 0xFBBF, 0xEB9E, 0x9B79, 0x8B58, 0xBB3B, 0xAB1A,
         0x6CA6, 0x7C87, 0x4CE4, 0x5CC5, 0x2C22, 0x3C03, 0x0C60, 0x1C41,
         0xEDAE, 0xFD8F, 0xCDEC, 0xDDCD, 0xAD2A, 0xBD0B, 0x8D68, 0x9D49,
         0x7E97, 0x6EB6, 0x5ED5, 0x4EF4, 0x3E13, 0x2E32, 0x1E51, 0x0E70,
         0xFF9F, 0xEFBE, 0xDFDD, 0xCFFC, 0xBF1B, 0xAF3A, 0x9F59, 0x8F78,
         0x9188, 0x81A9, 0xB1CA, 0xA1EB, 0xD10C, 0xC12D, 0xF14E, 0xE16F,
         0x1080, 0x00A1, 0x30C2, 0x20E3, 0x5004, 0x4025, 0x7046, 0x6067,
         0x83B9, 0x9398, 0xA3FB, 0xB3DA, 0xC33D, 0xD31C, 0xE37F, 0xF35E,
         0x02B1, 0x1290, 0x22F3, 0x32D2, 0x4235, 0x5214, 0x6277, 0x7256,
         0xB5EA, 0xA5CB, 0x95A8, 0x8589, 0xF56E, 0xE54F, 0xD52C, 0xC50D,
         0x34E2, 0x24C3, 0x14A0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
         0xA7DB, 0xB7FA, 0x8799, 0x97B8, 0xE75F, 0xF77E, 0xC71D, 0xD73C,
         0x26D3, 0x36F2, 0x0691, 0x16B0, 0x6657, 0x7676, 0x4615, 0x5634,
         0xD94C, 0xC96D, 0xF90E, 0xE92F, 0x99C8, 0x89E9, 0xB98A, 0xA9AB,
         0x5844, 0x4865, 0x7806, 0x6827, 0x18C0, 0x08E1, 0x3882, 0x28A3,
         0xCB7D, 0xDB5C, 0xEB3F, 0xFB1E, 0x8BF9, 0x9BD8, 0xABBB, 0xBB9A,
         0x4A75, 0x5A54, 0x6A37, 0x7A16, 0x0AF1, 0x1AD0, 0x2AB3, 0x3A92,
         0xFD2E, 0xED0F, 0xDD6C, 0xCD4D, 0xBDAA, 0xAD8B, 0x9DE8, 0x8DC9,
         0x7C26, 0x6C07, 0x5C64, 0x4C45, 0x3CA2, 0x2C83, 0x1CE0, 0x0CC1,
         0xEF1F, 0xFF3E, 0xCF5D, 0xDF7C, 0xAF9B, 0xBFBA, 0x8FD9, 0x9FF8,
         0x6E17, 0x7E36, 0x4E55, 0x5E74, 0x2E93, 0x3EB2, 0x0ED1, 0x1EF0
      };

      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the PosixCRC32 class.
      /// </summary>
      public CCSDSCRC16()
      {
         this.InitialValue = 0xFFFFFFFF;
         this.BitLength = 16;
         this.ChecksumIndex = -1;
      }

      #endregion Constructors

      #region Public Properties
      #endregion Public Properties


      #region Public Methods

      #region Documentation

      //     Description:
      //     Compute and decode the VCDU Error Control Field of the TDRSS MA VCDU frame
      //
      //     Specification:
      //
      //   According to CDRL 4 Section 7.9 [Reference 1]
      //
      //   The VCDU Error Control Field is only used for data linked down the S-Band TDRS MA (Virtual
      //   Channel 11). This is a 16-bit cyclic redundancy code, and its purpose is to detect errors for VCDUs that
      //   do not use the Reed-Solomon encoding. The Error Control Field will be generated in accordance with
      //   CCSDS Recommendation for Advanced Orbiting Systems (CCSDS 701.0-B-3), Section 5.4.9.2.1.4.2.
      //
      //   According to CCSDS 701.0-B-3:
      //
      //   The cyclic redundancy code contained within this field shall be
      //   characterized as follows:
      //   (1) The generator polynomial shall be:
      //
      //                   16    12    5
      //      g(x) = x   + x   + x  + 1
      //
      //   (2) Both encoder and decoder shall be initialized to the "all ones" state for
      //   each VCDU.
      //   (3) Parity "P" generation shall be performed over the data space "D" as
      //   shown in Figure 5-12; i.e., .D. covers the entire VCDU excluding the
      //   final 16-bit VCDU Error Control field.
      //   (4) The generated parity symbols shall then be inserted into the VCDU
      //
      //   CCSDS 701.0-B-3 also references CCSDS 102.0-B-5 (Packet Telemetry) which states:in 5.5.1 ENCODING PROCEDURE:
      //
      //    "The encoding procedure accepts an (n-16)-bit data block and generates
      //    a systematic binary (n,n-16) block code by appending a 16-bit Frame Check
      //    Sequence (FCS) as the final 16 bits of the codeblock."
      //
      //           FCS(x) = [x**16 * M(x) + x**(n-16) * L(x)] modulo G(x)
      //
      //    where 
      //           M(x) is the (n-16)-bit data to be encoded expressed as a polynomial
      //           with binary coefficients,
      //           L(x) = x**15 + x**14 + ... + x**2 + x + 1 (all "1" polynomial), 
      //           G(x) = x**16 + x**12 + x**5 + 1 is the generator polynomial,    
      //           All addition operators are the modulo 2 additions (Exclusive OR).
      //
      //    The error detection syndrome S(x) will be zero if no error is detected
      //
      //           S(x) = [x**16 * C'(x) + x**n * L(x)] modulo G(x)
      //
      //    where 
      //           C'(x) is the received block in polynomial form.
      //
      //    Big Endian bits and bytes order.
      //
      //    References:
      //    1. GAMMA-RAY LARGE AREA SPACE TELESCOPE (GLAST)
      //       OBSERVATORY TO GROUND INTERFACE CONTROL DOCUMENT, CDRL 4,  
      //       GENERAL DYNAMICS C4 SYSTEMS SPECTRUM ASTRO SPACE SYSTEMS
      //
      //    2. CCSDS Recommendation for Advanced Orbiting Systems (CCSDS 701.0-B-3)
      //
      //    3. CCSDS Recommendation for Packet Telemetry (CCSDS CCSDS 102.0-B-5)
      //
      //    4. "Telemetry Concept and Rationale", CCSDS 100.0-G-1, Green Book, 
      //       Consultative Committee for Space Data Systems, December 1987, Annex D,
      //      "Telemetry Transfer Frame Error Detection Encoding/Decoding Guideline".
      //
      //    5. "Advanced Orbiting Systems, Network and Data Links: Summary of Concept,
      //    Rationale, and Performance", CCSDS 700.0-G-3, Green Book, Consultative
      //    Committee for Space Data Systems, November 1992, Appendix C, "Procedures
      //    for Verifying CCSDS Encoder Implementations", Page C-11.
      //
      //    6. High-Speed Computation of Cyclic Redundancy Checks,
      //    Eric E. Johnson, November 1995, NMSU-ECE-95-011
      //
      //    7. A PAINLESS GUIDE TO CRC ERROR DETECTION ALGORITHMS, Ross Williams,
      //       19 Auguest 1993
      //
      //   The code is based on a combination of sources and was validated against
      //   Reference 6 and 7.
      //
      //   This algorithm is known as CRC-CCITT. It is different from another
      //   16-bit CRC algorithm that uses a different polynomial(0x8005).
      //
      //   
      #endregion Documentation

      /// <summary>
      /// Implementation of IChecksum interface
      /// </summary>
      /// <param name="data">the binary buffer for which the checksum is computed</param>
      /// <param name="startIndex">the start index at which the checksum starts</param>
      /// <param name="length">the number of bytes for which the checksum is computed</param>
      /// <param name="initialValue">the initial value of the checksum algorithm</param>
      /// <returns>the checksum</returns>
      public override ulong Compute(byte[] data)
      {
         ushort crc = (ushort)this.InitialValue;  /* initial CRC value */
         ushort t;
         for (int i = this.StopIndex - this.StartIndex, j = (int)this.StartIndex; i > 0; i--, j++)
         {
            t = crc;
            crc = crctab[(t >> 8) ^ data[j]];
            crc = (ushort)(crc ^ (t << 8));
         }

         this.ComputedChecksum = crc;

         return crc;
      }

      #endregion Public Methods
   }

   /// <summary>
   /// Computes Posix CRC-32 checksum.
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications", "GES.Checksums" })]
   [Description("Checksum algorithms.")]
   public class PosixCRC32 :
      Checksum
   {
      #region Fields

      /// <summary>
      /// Coefficients used to Compute the checksum
      /// </summary>
      private static ulong[] crctab = new ulong[]
      {
         0x0,
         0x04c11db7, 0x09823b6e, 0x0d4326d9, 0x130476dc, 0x17c56b6b,
         0x1a864db2, 0x1e475005, 0x2608edb8, 0x22c9f00f, 0x2f8ad6d6,
         0x2b4bcb61, 0x350c9b64, 0x31cd86d3, 0x3c8ea00a, 0x384fbdbd,
         0x4c11db70, 0x48d0c6c7, 0x4593e01e, 0x4152fda9, 0x5f15adac,
         0x5bd4b01b, 0x569796c2, 0x52568b75, 0x6a1936c8, 0x6ed82b7f,
         0x639b0da6, 0x675a1011, 0x791d4014, 0x7ddc5da3, 0x709f7b7a,
         0x745e66cd, 0x9823b6e0, 0x9ce2ab57, 0x91a18d8e, 0x95609039,
         0x8b27c03c, 0x8fe6dd8b, 0x82a5fb52, 0x8664e6e5, 0xbe2b5b58,
         0xbaea46ef, 0xb7a96036, 0xb3687d81, 0xad2f2d84, 0xa9ee3033,
         0xa4ad16ea, 0xa06c0b5d, 0xd4326d90, 0xd0f37027, 0xddb056fe,
         0xd9714b49, 0xc7361b4c, 0xc3f706fb, 0xceb42022, 0xca753d95,
         0xf23a8028, 0xf6fb9d9f, 0xfbb8bb46, 0xff79a6f1, 0xe13ef6f4,
         0xe5ffeb43, 0xe8bccd9a, 0xec7dd02d, 0x34867077, 0x30476dc0,
         0x3d044b19, 0x39c556ae, 0x278206ab, 0x23431b1c, 0x2e003dc5,
         0x2ac12072, 0x128e9dcf, 0x164f8078, 0x1b0ca6a1, 0x1fcdbb16,
         0x018aeb13, 0x054bf6a4, 0x0808d07d, 0x0cc9cdca, 0x7897ab07,
         0x7c56b6b0, 0x71159069, 0x75d48dde, 0x6b93dddb, 0x6f52c06c,
         0x6211e6b5, 0x66d0fb02, 0x5e9f46bf, 0x5a5e5b08, 0x571d7dd1,
         0x53dc6066, 0x4d9b3063, 0x495a2dd4, 0x44190b0d, 0x40d816ba,
         0xaca5c697, 0xa864db20, 0xa527fdf9, 0xa1e6e04e, 0xbfa1b04b,
         0xbb60adfc, 0xb6238b25, 0xb2e29692, 0x8aad2b2f, 0x8e6c3698,
         0x832f1041, 0x87ee0df6, 0x99a95df3, 0x9d684044, 0x902b669d,
         0x94ea7b2a, 0xe0b41de7, 0xe4750050, 0xe9362689, 0xedf73b3e,
         0xf3b06b3b, 0xf771768c, 0xfa325055, 0xfef34de2, 0xc6bcf05f,
         0xc27dede8, 0xcf3ecb31, 0xcbffd686, 0xd5b88683, 0xd1799b34,
         0xdc3abded, 0xd8fba05a, 0x690ce0ee, 0x6dcdfd59, 0x608edb80,
         0x644fc637, 0x7a089632, 0x7ec98b85, 0x738aad5c, 0x774bb0eb,
         0x4f040d56, 0x4bc510e1, 0x46863638, 0x42472b8f, 0x5c007b8a,
         0x58c1663d, 0x558240e4, 0x51435d53, 0x251d3b9e, 0x21dc2629,
         0x2c9f00f0, 0x285e1d47, 0x36194d42, 0x32d850f5, 0x3f9b762c,
         0x3b5a6b9b, 0x0315d626, 0x07d4cb91, 0x0a97ed48, 0x0e56f0ff,
         0x1011a0fa, 0x14d0bd4d, 0x19939b94, 0x1d528623, 0xf12f560e,
         0xf5ee4bb9, 0xf8ad6d60, 0xfc6c70d7, 0xe22b20d2, 0xe6ea3d65,
         0xeba91bbc, 0xef68060b, 0xd727bbb6, 0xd3e6a601, 0xdea580d8,
         0xda649d6f, 0xc423cd6a, 0xc0e2d0dd, 0xcda1f604, 0xc960ebb3,
         0xbd3e8d7e, 0xb9ff90c9, 0xb4bcb610, 0xb07daba7, 0xae3afba2,
         0xaafbe615, 0xa7b8c0cc, 0xa379dd7b, 0x9b3660c6, 0x9ff77d71,
         0x92b45ba8, 0x9675461f, 0x8832161a, 0x8cf30bad, 0x81b02d74,
         0x857130c3, 0x5d8a9099, 0x594b8d2e, 0x5408abf7, 0x50c9b640,
         0x4e8ee645, 0x4a4ffbf2, 0x470cdd2b, 0x43cdc09c, 0x7b827d21,
         0x7f436096, 0x7200464f, 0x76c15bf8, 0x68860bfd, 0x6c47164a,
         0x61043093, 0x65c52d24, 0x119b4be9, 0x155a565e, 0x18197087,
         0x1cd86d30, 0x029f3d35, 0x065e2082, 0x0b1d065b, 0x0fdc1bec,
         0x3793a651, 0x3352bbe6, 0x3e119d3f, 0x3ad08088, 0x2497d08d,
         0x2056cd3a, 0x2d15ebe3, 0x29d4f654, 0xc5a92679, 0xc1683bce,
         0xcc2b1d17, 0xc8ea00a0, 0xd6ad50a5, 0xd26c4d12, 0xdf2f6bcb,
         0xdbee767c, 0xe3a1cbc1, 0xe760d676, 0xea23f0af, 0xeee2ed18,
         0xf0a5bd1d, 0xf464a0aa, 0xf9278673, 0xfde69bc4, 0x89b8fd09,
         0x8d79e0be, 0x803ac667, 0x84fbdbd0, 0x9abc8bd5, 0x9e7d9662,
         0x933eb0bb, 0x97ffad0c, 0xafb010b1, 0xab710d06, 0xa6322bdf,
         0xa2f33668, 0xbcb4666d, 0xb8757bda, 0xb5365d03, 0xb1f740b4
      };

      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the PosixCRC32 class.
      /// </summary>
      public PosixCRC32()
      {
         this.InitialValue = 0xFFFFFFFFFFFFFFFF;
         this.BitLength = 32;
         this.ChecksumIndex = -1;
      }

      #endregion Constructors

      #region Public Properties
      #endregion Public Properties

      #region Public Methods

      #region Documentation

      ////  ++PROLOG_METHOD
      ////
      ////  Checksum::PosixCrc32
      ////
      ////  PURPOSE: Implements POSIX CRC-32 Checksum.
      ////
      ////  ARGUMENTS:
      ////
      ////  TYPE            NAME     USAGE   DESCRIPTION
      ////  ----            ----     -----   -----------
      ////  unsigned char*  a_dataP    I     Pointer to bytes
      ////  unsigned        a_size     I     Number of bytes
      ////  unsigned long   a_init     I     Initial value to start Checksum with
      ////
      ////  RETURNS:
      ////
      ////  unsigned long       Result of Checksum
      ////
      ////  NOTES:
      ////
      ////  This checksum algorithm implements the Posix CRC-32 checksum as calculated
      ////  by the cksum utility.
      ////  This is the checksum of the data bytes and the data length in bytes.
      ////  The CRC-32 standard describes only the checksum computation algorithm
      ////  over an array of data.  POSIX chooses to include the file length in
      ////  the "data" used in the checksum computation.
      ////
      ////  --PROLOG_METHOD
      ////
      ////  ++PDL
      ////
      ////  Checksum::PosixCrc32
      ////
      ////  Based on BSD CRC-32 implementation crc.c8.1 (Berkeley) 6/17/93

      /*
       * Copyright (c) 1991, 1993
       *The Regents of the University of California.  All rights reserved.
       *
       * This code is derived from software contributed to Berkeley by
       * James W. Williams of NASA Goddard Space Flight Center.
       *
       * Redistribution and use in source and binary forms, with or without
       * modification, are permitted provided that the following conditions
       * are met:
       * 1. Redistributions of source code must retain the above copyright
       *    notice, this list of conditions and the following disclaimer.
       * 2. Redistributions in binary form must reproduce the above copyright
       *    notice, this list of conditions and the following disclaimer in the
       *    documentation and/or other materials provided with the distribution.
       * 3. All advertising materials mentioning features or use of this software
       *    must display the following acknowledgement:
       *This product includes software developed by the University of
       *California, Berkeley and its contributors.
       * 4. Neither the name of the University nor the names of its contributors
       *    may be used to endorse or promote products derived from this software
       *    without specific prior written permission.
       *
       * THIS SOFTWARE IS PROVIDED BY THE REGENTS AND CONTRIBUTORS ``AS IS'' AND
       * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
       * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
       * ARE DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR CONTRIBUTORS BE LIABLE
       * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
       * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
       * OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
       * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
       * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
       * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
       * SUCH DAMAGE.
       */
      #endregion Documentation

      /// <summary>
      /// Implementation of IChecksum interface
      /// </summary>
      /// <param name="data">the binary buffer for which the checksum is computed</param>
      /// <param name="startIndex">the start index at which the checksum starts</param>
      /// <param name="length">the number of bytes for which the checksum is computed</param>
      /// <param name="initialValue">the initial value of the checksum algorithm</param>
      /// <returns>the checksum</returns>
      public override ulong Compute(byte[] data)
      {
         ulong crc32 = 0;
         byte[] p = data;
         uint crc_total;

         int len = this.StopIndex - this.StartIndex;
         crc_total = (uint)~this.InitialValue;

         // Compute the CRC of each byte in the buffer
         for (int i = len, j = (int)this.StartIndex; i > 0; i--, j++)
         {
            crc_total = Compute(crc_total, p[j]);
         }

         // Include the length of the data in the checksum 
         for (; len != 0; len >>= 8)
         {
            crc_total = Compute(crc_total, (byte)(len & 0xff));
         }

         crc32 = ~crc_total;

         this.ComputedChecksum = crc32;

         return crc32;
      }

      #endregion Public Methods

      #region Private Methods

      /// <summary>
      /// Computes the next iteration of the checksum
      /// </summary>
      /// <param name="var">the current running checksum</param>
      /// <param name="ch">the current byte to be processed</param>
      /// <returns>the checksum</returns>
      private static uint Compute(uint var, byte ch)
      {
         uint index = (var >> 24) ^ ch;
         ulong element = crctab[index];
         ulong value = 8 ^ crctab[index];
         int value2 = (int)value;
         byte value3 = (byte)value;
         var = (uint)((var << 8) ^ crctab[index]);
         return var;
      }

      #endregion Private Methods
   }

   /// <summary>
   /// Computes an add checksum
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications", "GES.Checksums" })]
   [Description("Add checksum algorithm.")]
   public class AddChecksum :
      Checksum
   {
      #region Constructors

      /// <summary>
      /// Initializes a new instance of the AddChecksum class.
      /// </summary>
      public AddChecksum()
      {
         this.InitialValue = 0;
         this.BitLength = 64;
         this.ChecksumIndex = -1;
      }

      #endregion Constructors

      #region Public Properties

      #endregion Public Properties

      #region Methods

      /// <summary>
      /// Implementation of IChecksum interface
      /// </summary>
      /// <param name="data">the binary buffer for which the checksum is computed</param>
      /// <param name="startIndex">the start index at which the checksum starts</param>
      /// <param name="length">the number of bytes for which the checksum is computed</param>
      /// <param name="initialValue">the initial value of the checksum algorithm</param>
      /// <returns>the checksum</returns>
      public override ulong Compute(byte[] data)
      {
         ulong initialValue = this.InitialValue;
         for (int i = this.StartIndex, end = this.StopIndex; i < end; i++)
         {
            initialValue += data[i];
         }

         this.ComputedChecksum = initialValue;
         return initialValue;
      }

      #endregion Methods
   }

   /// <summary>
   /// Computes an XOR checksum
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications", "GES.Checksums" })]
   [Description("XOR checksum algorithm.")]
   public class XORChecksum :
      Checksum
   {
      #region Constructors

      /// <summary>
      /// Initializes a new instance of the XORChecksum class.
      /// </summary>
      public XORChecksum()
      {
         this.InitialValue = 0xFF;
         this.BitLength = 8;
         this.ChecksumIndex = -1;
      }

      #endregion Constructors

      #region Public Properties

      #endregion Public Properties

      #region Methods

      /// <summary>
      /// Implementation of IChecksum interface
      /// </summary>
      /// <param name="data">the binary buffer for which the checksum is computed</param>
      /// <param name="startIndex">the start index at which the checksum starts</param>
      /// <param name="length">the number of bytes for which the checksum is computed</param>
      /// <param name="initialValue">the initial value of the checksum algorithm</param>
      /// <returns>the checksum</returns>
      public override ulong Compute(byte[] data)
      {
         byte result = (byte)(0xFF & this.InitialValue);

         for (int i = (int)this.StopIndex - 1; i >= this.StartIndex; i--)
         {
            result ^= data[i];
         }

         this.ComputedChecksum = result;
         return (ulong)result;
      }

      #endregion Methods
   }

   /// <summary>
   /// Computes an XOR checksum
   /// </summary>
   [Provide(Categories = new string[] { "GES.Communications", "GES.Checksums" })]
   [Description("XOR checksum algorithm.")]
   public class X25Checksum :
      Checksum
   {
      #region Constructors

      /// <summary>
      /// Initializes a new instance of the XORChecksum class.
      /// </summary>
      public X25Checksum()
      {
         this.InitialValue = 0xFFFF;
         this.BitLength = 8;
         this.ChecksumIndex = -1;
      }

      #endregion Constructors

      #region Public Properties

      #endregion Public Properties

      #region Methods


      /// <summary>
      /// Accumulate the X.25 CRC by adding one char at a time.
      /// The checksum function adds the hash of one char at a time to the 16 bit checksum
      /// </summary>
      /// <param name="data">new char to hash</param>
      /// <param name="crcAccum">the already accumulated checksum</param>
      /// <returns>the accumulate checksum</returns>
      public static ushort Accumulate(byte b, ushort crc)
      {
         byte ch = (byte)(b ^ (byte)(crc & 0x00ff));
         ch ^= (byte)(ch << 4);
         return (ushort)((crc >> 8) ^ (ch << 8) ^ (ch << 3) ^ (ch >> 4));
      }


      /// <summary>
      /// Calculates the X.25 checksum on a byte buffer
      /// </summary>
      /// <param name="buffer">buffer containing the byte array to hash</param>
      /// <param name="length"></param>
      /// <returns>the checksum over the buffer bytes</returns>
      public override ulong Compute(byte[] data)
      {
         int len =  -this.StartIndex;
         if (this.StopIndex == -1)
         {
            len += data.Length;
         }
         else
         {
            len += this.StopIndex;
         }

         if (len < 1)
         {
            return 0xFFFF;
         }

         ushort crcTmp = (ushort)this.InitialValue;
         for (int i = 0; i < len; i++)
         {
            crcTmp = Accumulate(data[this.StartIndex + i], crcTmp);
         }

         this.ComputedChecksum = crcTmp;
         return crcTmp;
      }

      #endregion Methods
   }
}
      /*
**  lro_ts_crc
**
**  This program calculates the CRC of a given file using the same
**  algorithm as the LRO spacecraft cFE Table Services flight software uses.
**
**  Inputs: One string containing the filename of the file to CRC.
** 
**
**  Outputs: Prints to the terminal the filename, size, and CRC.
**           Returns the CRC.
**
**  Author: Mike Blau, GSFC Code 582
**
**  Date: 1/28/08 
**
**  Modified 4/24/08  MDB  Added option to skip a specified number of header bytes
**  Modified 2/04/09  BDT  Modified to compute cFE table services CS
**  Modified 4/01/09  STS  Modified to always skip header (116 bytes)
**  Modified 4/01/09  STS  Removed option to skip a specified number of header bytes
**  Modified 6/15/12  WFM  Replaced the CRC Table with the table used in 
**                         CFE_ES_CalculateCRC
*/
      /*
      **             Function Prologue
      **
      ** Function: CFE_ES_CalculateCRC  (taken directly from lro-cfe-4.2.1 delivery - 2/4/09)
      **
      ** Purpose:  Perform a CRC calculation on a range of memory.
      **
      */
//      uint32 CFE_ES_CalculateCRC(void* DataPtr, uint32 DataLength, uint32 InputCRC, uint32 TypeCRC)
//      {
//         int32 i;
//         int16 Index;
//         int16 Crc = 0;
//         uint8* BufPtr;

//         static const uint16 CrcTable[256] =
//         {

//       0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
//       0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
//       0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
//       0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
//       0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
//       0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
//       0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
//       0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
//       0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
//       0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
//       0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
//       0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
//       0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
//       0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
//       0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
//       0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
//       0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
//       0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
//       0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
//       0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
//       0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
//       0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
//       0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
//       0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
//       0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
//       0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
//       0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
//       0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
//       0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
//       0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
//       0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
//       0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040
//    };

//         switch (TypeCRC)
//         {
//            /*       case CFE_ES_CRC_32:                                                    */
//            /*            CFE_ES_WriteToSysLog("CFE ES Calculate CRC32 not Implemented\n"); */
//            /*            break;                                                            */

//            case CFE_ES_CRC_16:
//               Crc = (int16)(0xFFFF & InputCRC);
//               BufPtr = (uint8*)DataPtr;

//               for (i = 0; i < DataLength; i++, BufPtr++)
//               {
//                  Index = ((Crc ^ *BufPtr) & 0x00FF);
//                  Crc = ((Crc >> 8) & 0x00FF) ^ CrcTable[Index];
//               }
//               break;

//            /*       case CFE_ES_CRC_8:                                                    */
//            /*            CFE_ES_WriteToSysLog("CFE ES Calculate CRC8 not Implemented\n"); */
//            /*            break;                                                           */

//            default:
//               break;
//         }
//         return (Crc);

//      } /* End of CFE_ES_CalculateCRC() */



//      int main(int argc, char** argv)
//      {
//         int readSize;
//         int skipSize = 0;
//         int fileSize = 0;
//         uint32 fileCRC = 0;
//         int fd;
//         int done = 0;
//         char buffer[100];

//         /* check for valid input */
//         if ((argc != 2) || (strncmp(argv[1], "-help", 100) == 0))
//         {
//            printf("\ncFE TS CRC calculator for LRO files.");
//            printf("\nUsage: cfe_ts_crc [filename]\n");
//            exit(0);
//         }
//         /* Set to skip the header (116 bytes) */
//         skipSize = 116;
//         /* open the input file if possible */
//         fd = open(argv[1], O_RDONLY);
//         if (fd < 0)
//         {
//            printf("\ncfe_ts_crc error: can't open input file!\n");
//            exit(0);
//         }
//         /* seek past the number of bytes requested */
//         lseek(fd, skipSize, SEEK_SET);

//         /* read the input file 100 bytes at a time */
//         while (done == 0)
//         {
//            readSize = read(fd, buffer, 100);
//            fileCRC = CFE_ES_CalculateCRC(buffer, readSize, fileCRC, CFE_ES_CRC_16);
//            fileSize += readSize;
//            if (readSize != 100) done = 1;
//         }
//         /* print the size/CRC results */
//         printf("\nTable File Name:            %s\nTable Size:                 %d Bytes\nExpected TS Validation CRC: 0x%08X\n\n", argv[1], fileSize, fileCRC);

//         return (fileCRC);
//      }

//      /// <summary>
//      /// Computes Posix CRC-32 checksum.
//      /// </summary>
//      [Provide(Categories = new string[] { "GES.Communications", "GES.Checksums" })]
//      [Description("Checksum algorithms.")]
//      public class CFEFileChecksum :
//         Checksum
//      {
//         #region Fields

//         /// <summary>
//         /// Coefficients used to Compute the checksum
//         /// </summary>
//         private static ulong[] crctab = new ulong[]
//         {
//            0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
//            0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
//            0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
//            0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
//            0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
//            0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
//            0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
//            0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
//            0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
//            0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
//            0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
//            0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
//            0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
//            0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
//            0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
//            0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
//            0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
//            0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
//            0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
//            0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
//            0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
//            0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
//            0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
//            0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
//            0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
//            0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
//            0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
//            0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
//            0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
//            0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
//            0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
//            0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040
//         };

//         #endregion Fields

//         #region Constructors

//         /// <summary>
//         /// Initializes a new instance of the PosixCRC32 class.
//         /// </summary>
//         public CFEFileChecksum()
//         {
//            this.InitialValue = 0xFFFFFFFFFFFFFFFF;
//            this.BitLength = 32;
//            this.ChecksumIndex = -1;
//         }

//         #endregion Constructors

//         #region Public Properties
//         #endregion Public Properties

//         #region Public Methods

//         #region Documentation

//         ////  ++PROLOG_METHOD
//         ////
//         ////  Checksum::PosixCrc32
//         ////
//         ////  PURPOSE: Implements POSIX CRC-32 Checksum.
//         ////
//         ////  ARGUMENTS:
//         ////
//         ////  TYPE            NAME     USAGE   DESCRIPTION
//         ////  ----            ----     -----   -----------
//         ////  unsigned char*  a_dataP    I     Pointer to bytes
//         ////  unsigned        a_size     I     Number of bytes
//         ////  unsigned long   a_init     I     Initial value to start Checksum with
//         ////
//         ////  RETURNS:
//         ////
//         ////  unsigned long       Result of Checksum
//         ////
//         ////  NOTES:
//         ////
//         ////  This checksum algorithm implements the Posix CRC-32 checksum as calculated
//         ////  by the cksum utility.
//         ////  This is the checksum of the data bytes and the data length in bytes.
//         ////  The CRC-32 standard describes only the checksum computation algorithm
//         ////  over an array of data.  POSIX chooses to include the file length in
//         ////  the "data" used in the checksum computation.
//         ////
//         ////  --PROLOG_METHOD
//         ////
//         ////  ++PDL
//         ////
//         ////  Checksum::PosixCrc32
//         ////
//         ////  Based on BSD CRC-32 implementation crc.c8.1 (Berkeley) 6/17/93

//         /*
//          * Copyright (c) 1991, 1993
//          *The Regents of the University of California.  All rights reserved.
//          *
//          * This code is derived from software contributed to Berkeley by
//          * James W. Williams of NASA Goddard Space Flight Center.
//          *
//          * Redistribution and use in source and binary forms, with or without
//          * modification, are permitted provided that the following conditions
//          * are met:
//          * 1. Redistributions of source code must retain the above copyright
//          *    notice, this list of conditions and the following disclaimer.
//          * 2. Redistributions in binary form must reproduce the above copyright
//          *    notice, this list of conditions and the following disclaimer in the
//          *    documentation and/or other materials provided with the distribution.
//          * 3. All advertising materials mentioning features or use of this software
//          *    must display the following acknowledgement:
//          *This product includes software developed by the University of
//          *California, Berkeley and its contributors.
//          * 4. Neither the name of the University nor the names of its contributors
//          *    may be used to endorse or promote products derived from this software
//          *    without specific prior written permission.
//          *
//          * THIS SOFTWARE IS PROVIDED BY THE REGENTS AND CONTRIBUTORS ``AS IS'' AND
//          * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//          * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//          * ARE DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR CONTRIBUTORS BE LIABLE
//          * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
//          * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
//          * OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
//          * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
//          * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
//          * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
//          * SUCH DAMAGE.
//          */
//         #endregion Documentation

//         /// <summary>
//         /// Implementation of IChecksum interface
//         /// </summary>
//         /// <param name="data">the binary buffer for which the checksum is computed</param>
//         /// <param name="startIndex">the start index at which the checksum starts</param>
//         /// <param name="length">the number of bytes for which the checksum is computed</param>
//         /// <param name="initialValue">the initial value of the checksum algorithm</param>
//         /// <returns>the checksum</returns>
//         public override ulong Compute(byte[] data)
//         {
//            ulong crc32 = 0;
//            byte[] p = data;
//            uint crc_total;

//            int len = this.StopIndex - this.StartIndex;
//            crc_total = (uint)~this.InitialValue;

//            // Compute the CRC of each byte in the buffer
//            for (int i = len, j = (int)this.StartIndex; i > 0; i--, j++)
//            {
//               crc_total = Compute(crc_total, p[j]);
//            }

//            // Include the length of the data in the checksum 
//            for (; len != 0; len >>= 8)
//            {
//               crc_total = Compute(crc_total, (byte)(len & 0xff));
//            }

//            crc32 = ~crc_total;

//            this.ComputedChecksum = crc32;
//            this.InsertChecksum(data);

//            return crc32;
//         }

//         #endregion Public Methods

//         #region Private Methods

//         /// <summary>
//         /// Computes the next iteration of the checksum
//         /// </summary>
//         /// <param name="var">the current running checksum</param>
//         /// <param name="ch">the current byte to be processed</param>
//         /// <returns>the checksum</returns>
//         private static uint Compute(uint var, byte ch)
//         {
//            uint index = (var >> 24) ^ ch;
//            ulong element = crctab[index];
//            ulong value = 8 ^ crctab[index];
//            int value2 = (int)value;
//            byte value3 = (byte)value;
//            var = (uint)((var << 8) ^ crctab[index]);
//            return var;
//         }

//         #endregion Private Methods
//      }
//   }
//}
