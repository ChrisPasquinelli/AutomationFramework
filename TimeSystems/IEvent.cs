namespace GES.TimeSystems
{
   public interface IEvent
   {
      Time StartTime
      {
         get;
         set;
      }

      Time StopTime
      {
         get;
         set;
      }

      byte[] Messages
      {
         get;
         set;
      }
   }
}
