// <copyright file="MessageExtractor.cs" company="Genesis Engineering">
// Copyright (c) 2019 All Right Reserved
// </copyright>
// <author>Fran Maher</author>
// <email>FranMaher@comcast.net</email>
// <date>2019-3-20</date>
// <summary>Assembles then dispatches messages.</summary>

namespace GES.Communications
{
   #region Directives

   using System;
   using System.Collections.Generic;
   using System.IO;
   using System.Text;
   using System.Threading.Tasks;
   using MTI.Common;
   using MTI.Core;

   #endregion Directives

   /// <summary>
   /// Assembles then dispatches messages.
   /// </summary>
   public class MessageExtractor<T>
   {
      #region Delegates

      public delegate List<T> DeserializeDelegate(List<byte[]> messages);

      #endregion

      #region Fields

      /// <summary>
      /// The thread safety incoming queue of messages
      /// </summary>
      private SynchronizedQueue<byte[]> incomingQueue;

      /// <summary>
      /// The thread safety outgoing queue of messages
      /// </summary>
      private SynchronizedQueue<T> outgoingQueue;

      /// <summary>
      /// The start markers delineating messages
      /// </summary>
      private List<byte[]> startMarkers;

      /// <summary>
      /// The end markers delineating messages
      /// </summary>
      private List<byte[]> endMarkers;

      /// <summary>
      /// Byte size of header preceding start marker
      /// </summary>
      private int headerSize;

      /// <summary>
      /// Byte size of trailer following end marker
      /// </summary>
      private int trailerSize;

      /// <summary>
      /// Indicates which delineations are specified
      /// </summary>
      private Markers markers;

      /// <summary>
      /// Cache of partial message
      /// </summary>
      private byte[] residualPacket;

      /// <summary>
      /// Value indicating to start the incomingQueue processing
      /// </summary>
      private bool active;

      List<string> allIn = new List<string>();
      List<string> allOut = new List<string>();
      List<string> allResiduals = new List<string>();

      bool writeToFile;

      #endregion Fields

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="MessageExtractor"/> class
      /// </summary>
      /// <param name="incomingQueue">the thread-safe incomingQueue</param>
      /// <param name="startMarker">the start marker</param>
      /// <param name="endMarker">the end marker</param>
      public MessageExtractor(SynchronizedQueue<byte[]> incomingQueue, SynchronizedQueue<T> outgoingQueue, byte[] startMarker, byte[] endMarker = null, int headerSize = 0, int trailerSize = 0)
      {
         this.incomingQueue = incomingQueue;
         this.outgoingQueue = outgoingQueue;
         this.Statistics = new Statistics();
         this.residualPacket = new byte[0];
         this.headerSize = headerSize;
         this.trailerSize = trailerSize;

         this.markers = Markers.None;
         this.startMarkers = new List<byte[]>();
         if (startMarker != null && startMarker.Length > 0)
         {
            this.markers = Markers.Start;
            this.startMarkers.Add(startMarker);
         }

         this.endMarkers = new List<byte[]>();
         if (endMarker != null && endMarker.Length > 0)
         {
            this.markers |= Markers.End;
            this.endMarkers.Add(endMarker);
         }

         this.incomingQueue.DataReceived += this.OnDataReceived;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="MessageExtractor"/> class
      /// </summary>
      /// <param name="incomingQueue">the thread-safe incomingQueue</param>
      /// <param name="startMarker">the start marker</param>
      /// <param name="endMarker">the end marker</param>
      //public MessageExtractor(SynchronizedQueue<byte[]> incomingQueue, SynchronizedQueue<T> outgoingQueue, List<byte[]> startMarkers, List<byte[]> endMarkers = null, int headerSize = 0, int trailerSize = 0)
      //{
      //   this.incomingQueue = incomingQueue;
      //   this.outgoingQueue = outgoingQueue;
      //   this.Statistics = new Statistics();
      //   this.residualPacket = new byte[0];
      //   this.headerSize = headerSize;
      //   this.trailerSize = trailerSize;

      //   if (startMarkers != null && startMarkers.Count > 0)
      //   {
      //      if (startMarkers.Count == 1) this.markers = Markers.Start;
      //      else this.markers = Markers.MultipleStart;
      //      this.startMarkers = startMarkers;
      //   }

      //   if (endMarkers != null && endMarkers.Count > 0)
      //   {
      //      if (endMarkers.Count == 1) this.markers |= Markers.End;
      //      else this.markers |= Markers.MultipleEnd;
      //      this.endMarkers = endMarkers;
      //   }
      //   else
      //   {
      //      if (startMarkers.Count == 1) this.markers |= Markers.End;
      //      else this.markers |= Markers.MultipleEnd;
      //      this.endMarkers = startMarkers;
      //   }

      //   this.incomingQueue.DataReceived += this.OnDataReceived;
      //}

      #endregion Constructors

      #region Enumerations

      /// <summary>
      /// Indicates whether messages are delineated at the beginning, end, or both
      /// </summary>
      private enum Markers : byte
      {
         /// <summary>
         /// Indicates that the message has no delineation
         /// </summary>
         None = 0,

         /// <summary>
         /// Indicates that the message has a start marker for message delineation
         /// </summary>
         Start = 1,

         /// <summary>
         /// Indicates that the message has multiple start markers for message delineation
         /// </summary>
         MultipleStart = 2,

         /// <summary>
         /// Indicates that the message has an end marker for message delineation
         /// </summary>
         End = 4,

         /// <summary>
         /// Indicates that the message has multiple end markers for message delineation
         /// </summary>
         MultipleEnd = 8,

      }

      #endregion Enumerations

      #region Properties

      public SynchronizedQueue<T> ExtractedMessages
      {
         get
         {
            return this.outgoingQueue;
         }
      }

      public DeserializeDelegate Deserialize
      {
         get;
         set;
      }

      /// <summary>
      /// Gets or sets the maximumum queue size to throttle the processing back until existing items are consumed
      /// </summary>
      public int MaxQueueSize
      {
         get;
         set;
      }

      public Statistics Statistics
      {
         get;
         set;
      }

      public bool WriteToTextFile
      {
         get
         {
            return writeToFile;
         }

         set
         {
            writeToFile = value;
            if (value)
            {
               File.WriteAllLines("d:\\results3.txt", allIn.ToArray());
               File.WriteAllLines("d:\\results4.txt", allOut.ToArray());
               File.WriteAllLines("d:\\results5.txt", allResiduals.ToArray());
            }
         }
      }

      #endregion Properties

      #region Public Methods

      public void Clear()
      {
         this.residualPacket = new byte[0];
         this.outgoingQueue.Clear();
      }

      public string GetResidualPacket()
      {
         return Encoding.ASCII.GetString(this.residualPacket);
      }
      /// <summary>
      /// The message dispatching task
      /// </summary>
      /// <returns>a task that returns a value indicating the status of the results</returns>
      public void OnDataReceived(object sender, EventArgs args)
      {
         try
         {
            if (this.incomingQueue.Count > 0)
            {
               DateTime startTime = DateTime.Now;
               byte[] block = this.incomingQueue.Dequeue();
               allIn.Add(DateTime.Now.ToString("hh:mm:ss.fff") + " " + Encoding.ASCII.GetString(block));
               List<byte[]> byteMessages = this.ExtractMessages(block);
               foreach(byte[] byteMessage in byteMessages)
               {
                  allOut.Add(DateTime.Now.ToString("hh:mm:ss.fff") + " " + Encoding.ASCII.GetString(byteMessage));
               }

               List<T> messages = this.Deserialize(byteMessages);
               foreach (T message in messages)
               {
                  this.outgoingQueue.Enqueue(message);
               }

               this.Statistics.UpdateStatistics<byte[], T>(startTime, this.incomingQueue);
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex.Message);
         }
      }

      /// <summary>
      /// The message dispatching task
      /// </summary>
      /// <returns>a task that returns a value indicating the status of the results</returns>
      public async Task<bool> WaitForResponseAsync()
      {
         bool result = await System.Threading.Tasks.Task.Run(() =>
         {
            while (incomingQueue.Count == 0) System.Threading.Thread.Sleep(0);
            try
            {
               do
               {
                  byte[] block = this.incomingQueue.Dequeue();
                  List<byte[]> byteMessages = this.ExtractMessages(block);
                  List<T> messages = this.Deserialize(byteMessages);
                  foreach (T message in messages)
                  {
                     this.outgoingQueue.Enqueue(message);
                  }
               } while (this.residualPacket.Length > 0 && this.outgoingQueue.Count == 0);

               return true;
            }
            catch
            {
               return false;
            }
         }).ConfigureAwait(false);

         return result;
      }

      #endregion Public Methods

      #region Private Methods

      /// <summary>
      /// Finds the next pattern of bytes in a buffer at the start index
      /// </summary>
      /// <param name="bytes">the buffer</param>
      /// <param name="pattern">the pattern to search for</param>
      /// <param name="start">the starting index</param>
      /// <returns>the index at which the pattern starts</returns>
      private static int Find(byte[] bytes, byte[] pattern, int start)
      {
         int len = pattern.Length;
         int limit = bytes.Length - len;
         for (int i = start; i <= limit; i++)
         {
            int k = 0;
            for (; k < len; k++)
            {
               if (pattern[k] != bytes[i + k]) break;
            }

            if (k == len)
            {
               return i;
            }
         }

         return -1;
      }

      /// <summary>
      /// Finds the next pattern of bytes in a buffer at the start index
      /// </summary>
      /// <param name="bytes">the buffer</param>
      /// <param name="pattern">the pattern to search for</param>
      /// <param name="start">the starting index</param>
      /// <returns>the index at which the pattern starts</returns>
      //private static int Find(byte[] bytes, List<byte[]> patterns, int start)
      //{
      //   int len = pattern.Length;
      //   int limit = bytes.Length - len;
      //   for (int i = start; i <= limit; i++)
      //   {
      //      int k = 0;
      //      for (; k < len; k++)
      //      {
      //         if (pattern[k] != bytes[i + k]) break;
      //      }

      //      if (k == len)
      //      {
      //         return i;
      //      }
      //   }

      //   return -1;
      //}

      /// <summary>
      /// Gets the subarray
      /// </summary>
      /// <param name="array">the array containing the subarray</param>
      /// <param name="startIndex">the start index of the subarray with array</param>
      /// <param name="length">the length of the subarray</param>
      /// <returns>the subarray</returns>
      private static byte[] SubArray(byte[] array, int startIndex, int length)
      {
         byte[] subarray = new byte[length];
         Array.Copy(array, startIndex, subarray, 0, length);
         return subarray;
      }

      /// <summary>
      /// Concatenates two arrays
      /// </summary>
      /// <param name="subarray1">the preceding subarray</param>
      /// <param name="subarray2">the following subarray</param>
      /// <returns>the array containing the subarray1 followed by subarray2</returns>
      private static byte[] Concatenate(byte[] subarray1, byte[] subarray2)
      {
         byte[] array = new byte[subarray1.Length + subarray2.Length];
         Array.Copy(subarray1, array, subarray1.Length);
         Array.Copy(subarray2, 0, array, subarray1.Length, subarray2.Length);
         return array;
      }

      /// <summary>
      /// Extracts and reassembles messages received
      /// </summary>
      /// <param name="packet">the data packet</param>
      /// <returns>the assembled list of messages</returns>
      private List<byte[]> ExtractMessages(byte[] packet)
      {
         List<byte[]> messages = new List<byte[]>();

         if(this.markers == Markers.None)
         {
            messages.Add(packet);
            return messages;
         }
         else if (this.residualPacket.Length > 0)
         {
            packet = MessageExtractor<T>.Concatenate(this.residualPacket, MessageExtractor<T>.SubArray(packet, 0, packet.Length));
            allResiduals.Add(DateTime.Now.ToString("hh:mm:ss.fff") + " " + Encoding.ASCII.GetString(residualPacket));
            this.residualPacket = new byte[0];
         }

         while (packet.Length != 0)
         {
            int startMarkerIndex = -1;
            int begin = this.GetBegin(packet, 0, ref startMarkerIndex);

            if (begin != -1)
            {
               int end = -1;
               if (startMarkerIndex >= 0)
               {
                  end = this.GetEnd(packet, begin + this.headerSize + this.startMarkers[startMarkerIndex].Length);
               }
               else
               {
                  end = this.GetEnd(packet, begin + this.headerSize);
               }

               if (end != -1)
               {
                  messages.Add(SubArray(packet, begin, end - begin));
                  packet = MessageExtractor<T>.SubArray(packet, end, packet.Length - end);
               }
               else
               {
                  this.residualPacket = MessageExtractor<T>.SubArray(packet, begin, packet.Length - begin);
                  packet = new byte[0];
               }
            }
            else
            {  // unable to sync
               packet = new byte[0];
            }
         }

         return messages;
      }

      private int GetBegin(byte[] packet, int startIndex, ref int startMarkerIndex)
      {
         int begin = -1;
         if ((this.markers & Markers.Start) != 0)
         {
            startMarkerIndex = 0;
            begin = MessageExtractor<T>.Find(packet, this.startMarkers[0], startIndex);
            if (begin >= this.headerSize)
            {
               begin -= this.headerSize;
            }
         }
         else if ((this.markers & Markers.MultipleStart) != 0)
         {
            for (int marker = 0; marker < this.startMarkers.Count && begin == -1; marker++)
            {
               startMarkerIndex = marker;
               begin = MessageExtractor<T>.Find(packet, this.startMarkers[marker], startIndex);
            }

            if (begin >= startIndex + this.headerSize)
            {
               begin -= this.headerSize;
            }
         }
         else
         {
            begin = 0;
         }

         return begin;
      }

      private int GetEnd(byte[] packet, int startIndex)
      {
         int end = -1;
         if ((this.markers & Markers.End) != 0)
         {
            end = MessageExtractor<T>.Find(packet, this.endMarkers[0], startIndex);
            if (end != -1)
            {
               end += this.endMarkers[0].Length + this.trailerSize;
            }
         }
         else if ((this.markers & Markers.MultipleEnd) != 0)
         {
            int endMarkerIndex = -1;
            for (int marker = 0; marker < this.endMarkers.Count && end == -1; marker++)
            {
               endMarkerIndex = marker;
               end = MessageExtractor<T>.Find(packet, this.startMarkers[marker], startIndex);
            }

            end += this.endMarkers[endMarkerIndex].Length + this.trailerSize;
         }
         else 
         {
            int startMarkerIndex = 0;
            end = this.GetBegin(packet, startIndex, ref startMarkerIndex);
         }

         return end;
      }

      #endregion Private Methods
   }
}