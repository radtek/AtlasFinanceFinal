using System;

using Atlas.Enumerators;


namespace Atlas.FP.Identifier.ThreadSafe
{
  /// <summary> 
  /// Basic fingerprint template info- read-only
  /// </summary>
  public class FPBasicTemplate
  {
    public FPBasicTemplate(Int64 personId, int fingerId, byte[] data, Biometric.OrientationType orientation)
    {
      PersonId = personId;
      FingerId = fingerId;
      if (data != null)
      {
        Data = new byte[data.Length];
        Array.Copy(data, Data, data.Length);
      }    
      Orientation = orientation;     
    }


    /// <summary>
    /// DB Match: PER_Person.PersonId
    /// </summary>    
    public readonly Int64 PersonId;// { get; private set; }

    /// <summary>
    /// FingerID - 1 to 10
    /// </summary>
    public readonly int FingerId;// { get; private set; }

    /// <summary>
    /// Raw fingerprint template- readonly stops reassignment of the array, but array items can stil be changed!!! Use carefully.
    /// Making a clone (as recommended by the MS guidelines) will continually increase the Gen 2 LOB heap size and eventually cause OOM issues.
    /// Use DeepCopyData when accessing the item and use that with unmanaged code.
    /// </summary>
    public readonly byte[] Data; // { get; private set; }
    

    /// <summary>
    /// Orientation of template
    /// </summary>
    public readonly Biometric.OrientationType Orientation; // { get; private set; }
     
  }
}
